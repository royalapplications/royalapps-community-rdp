using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;
using MsRdpEx;

// ReSharper disable InconsistentNaming

namespace RoyalApps.Community.Rdp.WinForms.Controls.ActiveX;

internal class MsRdpExManager
{
    private static readonly RdpCoreApi _coreApi;
    private static readonly bool _axHookEnabled = true;
    private static readonly object _syncRoot = new();
    private bool _loggingConfigured;

    private static readonly Lazy<MsRdpExManager> _instance = new(() => new MsRdpExManager(), LazyThreadSafetyMode.ExecutionAndPublication);
    public static MsRdpExManager Instance => _instance.Value;

    public RdpCoreApi CoreApi => _coreApi;
    public bool AxHookEnabled => _axHookEnabled;

    static MsRdpExManager()
    {
        _coreApi = LoadCoreApi();
    }

    private static RdpCoreApi LoadCoreApi()
    {
        var coreApi = new RdpCoreApi();
        coreApi.LogEnabled = false;
        coreApi.PcapEnabled = false;
        coreApi.AxHookEnabled = _axHookEnabled;
        coreApi.Load();

        return coreApi;
    }

    public void ConfigureLogging(bool enabled, string? level, string? filePath, ILogger logger)
    {
        lock (_syncRoot)
        {
            ApplyLogging(enabled, level, filePath, logger);
            _loggingConfigured = true;
        }
    }

    public void EnsureLoggingConfigured(bool enabled, string? level, string? filePath, ILogger logger)
    {
        lock (_syncRoot)
        {
            if (_loggingConfigured)
                return;

            ApplyLogging(enabled, level, filePath, logger);
            _loggingConfigured = true;
        }
    }

    private static void ApplyLogging(bool enabled, string? level, string? filePath, ILogger logger)
    {
        try
        {
            _coreApi.LogEnabled = false;
            if (!enabled)
                return;

            if (!MsRdpExLoggingOptions.TryCreate(level, filePath, out var options, out var error))
            {
                logger.LogWarning("MsRdpEx logging was not enabled: {Reason}", error);
                return;
            }

            var directory = Path.GetDirectoryName(options.FilePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            _coreApi.LogLevel = options.Level;
            _coreApi.LogFilePath = options.FilePath;
            _coreApi.LogEnabled = true;

            logger.LogDebug(
                "Configured process-wide MsRdpEx logging: Level={LogLevel}, FilePath={LogFilePath}",
                options.Level,
                options.FilePath);
        }
        catch (Exception ex)
        {
            try
            {
                _coreApi.LogEnabled = false;
            }
            catch
            {
                // ignored: diagnostics must never prevent an RDP connection
            }

            logger.LogWarning(ex, "MsRdpEx logging could not be configured; the RDP connection will continue without native tracing");
        }
    }
}
