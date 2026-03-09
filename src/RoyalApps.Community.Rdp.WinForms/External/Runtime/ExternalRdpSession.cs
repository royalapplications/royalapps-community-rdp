using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Internal;
using RoyalApps.Community.Rdp.WinForms.Controls.Events;
using RoyalApps.Community.Rdp.WinForms.External.Files;

namespace RoyalApps.Community.Rdp.WinForms.External.Runtime;

internal sealed class ExternalRdpSession : IDisposable
{
    private readonly IExternalRdpSessionDependencies _dependencies;
    private readonly ILogger _logger;
    private ITemporaryRdpFileLease? _rdpFileLease;
    private IDisposable? _credentialScopes;
    private RdpConnectionContext? _connectionContext;
    private IExternalProcess? _process;
    private CancellationTokenSource? _windowMonitorCancellationSource;
    private int? _processId;
    private nint _currentWindowHandle;
    private bool _disconnectRequested;
    private bool _disposed;

    public ExternalRdpSession(ILogger logger)
        : this(logger, ExternalRdpSessionDependencies.Instance)
    {
    }

    internal ExternalRdpSession(ILogger logger, IExternalRdpSessionDependencies dependencies)
    {
        _logger = logger;
        _dependencies = dependencies;
    }

    public event EventHandler? Connected;

    public event EventHandler<DisconnectedEventArgs>? Disconnected;

    public event EventHandler<ExternalWindowChangedEventArgs>? WindowChanged;

    public Process? Process => _process?.UnderlyingProcess;

    public int? ProcessId => _processId;

    public nint CurrentWindowHandle => _currentWindowHandle;

    public bool TryActivateWindow(out nint windowHandle)
    {
        if (_processId.HasValue)
            return _dependencies.TryActivateWindow(_processId.Value, out windowHandle);

        windowHandle = IntPtr.Zero;
        return false;

    }

    public bool TryGetWindowHandle(out nint windowHandle)
    {
        if (_processId.HasValue)
            return _dependencies.TryGetWindowHandle(_processId.Value, out windowHandle);

        windowHandle = IntPtr.Zero;
        return false;

    }

    public void Connect(RdpConnectionContext connectionContext)
    {
        ArgumentNullException.ThrowIfNull(connectionContext);
        var configuration = connectionContext.Configuration;

        ThrowIfDisposed();

        if (_process is { HasExited: false })
            throw new InvalidOperationException("The external RDP session is already running.");

        _disconnectRequested = false;
        _connectionContext = connectionContext;
        _credentialScopes = _dependencies.CreateCredentialScopes(configuration, _logger);
        _rdpFileLease = _dependencies.CreateTemporaryRdpFileLease(connectionContext, _logger);

        try
        {
            var launcherPath = _dependencies.ResolveLauncherPath(configuration, _logger);
            var processStartInfo = new ProcessStartInfo
            {
                FileName = launcherPath,
                Arguments = BuildArguments(configuration, _rdpFileLease.FilePath),
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(launcherPath) ?? Environment.CurrentDirectory,
                Environment =
                {
                    ["MSRDPEX_LOG_ENABLED"] = configuration.LogEnabled ? "1" : "0",
                    ["MSRDPEX_LOG_LEVEL"] = configuration.LogLevel,
                    ["MSRDPEX_LOG_FILE_PATH"] = configuration.LogFilePath
                }
            };

            _process = _dependencies.StartProcess(processStartInfo);
            _process.EnableRaisingEvents = true;
            _process.Exited += ProcessOnExited;
            _processId = _process.Id;

            if (configuration.External.KillProcessOnHostExit)
                _dependencies.TrackProcessLifetime(_process, _logger);

            StartWindowMonitor(_processId.Value);
            Connected?.Invoke(this, EventArgs.Empty);
        }
        catch
        {
            DisposeSessionResources();
            throw;
        }
    }

    public void Disconnect()
    {
        var configuration = _connectionContext?.Configuration;
        if (configuration is null)
            return;

        if (_process is not { HasExited: false })
        {
            DisposeSessionResources();
            return;
        }

        _disconnectRequested = true;

        try
        {
            if (!_process.CloseMainWindow())
            {
                KillIfConfigured(configuration);
                return;
            }

            if (!_process.WaitForExit(configuration.External.CloseTimeout))
                KillIfConfigured(configuration);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to close the external RDP process gracefully.");
            KillIfConfigured(configuration);
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (_process != null)
            _process.Exited -= ProcessOnExited;

        DisposeSessionResources();
    }

    private void ProcessOnExited(object? sender, EventArgs eventArgs)
    {
        var exitCode = 0;
        int? processId = null;
        try
        {
            if (sender is IExternalProcess process)
            {
                process.Exited -= ProcessOnExited;
                processId = process.Id;
                exitCode = process.ExitCode;
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Reading the external RDP process state failed.");
        }

        DisposeSessionResources();

        Disconnected?.Invoke(
            this,
            new DisconnectedEventArgs
            {
                DisconnectCode = exitCode,
                Description = exitCode == 0
                    ? "The external Remote Desktop client exited."
                    : $"The external Remote Desktop client exited with code {exitCode}.",
                ShowError = exitCode != 0 && !_disconnectRequested,
                UserInitiated = _disconnectRequested || exitCode == 0,
                SessionMode = RdpSessionMode.External,
                ExternalProcessId = processId
            });
    }

    private void StartWindowMonitor(int processId)
    {
        StopWindowMonitor();

        _windowMonitorCancellationSource = new CancellationTokenSource();
        Task.Run(() => MonitorWindowAsync(processId, _windowMonitorCancellationSource.Token));
    }

    private async Task MonitorWindowAsync(int processId, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var windowHandle = _dependencies.TryGetWindowHandle(processId, out var trackedWindowHandle)
                    ? trackedWindowHandle
                    : IntPtr.Zero;

                UpdateTrackedWindow(windowHandle);
                await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to poll the external RDP session window for process {ProcessId}.", processId);
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private void StopWindowMonitor()
    {
        if (_windowMonitorCancellationSource is null)
            return;

        try
        {
            _windowMonitorCancellationSource.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // ignored
        }

        _windowMonitorCancellationSource.Dispose();
        _windowMonitorCancellationSource = null;
    }

    private void UpdateTrackedWindow(nint windowHandle, bool force = false)
    {
        if (!force && _currentWindowHandle == windowHandle)
            return;

        _currentWindowHandle = windowHandle;
        WindowChanged?.Invoke(
            this,
            new ExternalWindowChangedEventArgs
            {
                ExternalProcessId = _processId,
                WindowHandle = windowHandle
            });
    }

    private string BuildArguments(RdpClientConfiguration configuration, string rdpFilePath)
    {
        var fileArgument = $"\"{rdpFilePath}\"";
        return string.IsNullOrWhiteSpace(configuration.External.AdditionalArguments)
            ? fileArgument
            : $"{fileArgument} {configuration.External.AdditionalArguments}";
    }

    private void DisposeSessionResources()
    {
        StopWindowMonitor();
        if (_currentWindowHandle != IntPtr.Zero || _processId.HasValue)
            UpdateTrackedWindow(IntPtr.Zero, force: true);

        _process?.Dispose();
        _process = null;
        _processId = null;

        _rdpFileLease?.Dispose();
        _rdpFileLease = null;

        _credentialScopes?.Dispose();
        _credentialScopes = null;

        _connectionContext = null;
    }

    private void KillIfConfigured(RdpClientConfiguration configuration)
    {
        if (_process is not { HasExited: false })
            return;

        if (!configuration.External.KillProcessOnDisconnect)
            return;

        _process.Kill(entireProcessTree: true);
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
