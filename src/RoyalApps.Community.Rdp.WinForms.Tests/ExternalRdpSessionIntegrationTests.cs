using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Internal;
using RoyalApps.Community.Rdp.WinForms.External.Files;
using RoyalApps.Community.Rdp.WinForms.External.Runtime;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public sealed class ExternalRdpSessionIntegrationTests
{
    [Fact]
    public void Connect_ComposesLauncherArguments_TracksProcess_AndDeletesTemporaryFileOnDisconnect()
    {
        var tempDirectory = CreateTemporaryDirectory();
        try
        {
            var configuration = CreateConfiguration(tempDirectory);
            configuration.External.AdditionalArguments = "/f /admin";
            configuration.External.KeepTemporaryRdpFile = false;

            var dependencies = new FakeExternalRdpSessionDependencies();
            using var session = new ExternalRdpSession(NullLogger.Instance, dependencies);

            session.Connect(RdpConnectionContextFactory.Create(configuration));

            Assert.NotNull(dependencies.StartInfo);
            Assert.Equal(@"C:\Tools\mstsc.exe", dependencies.StartInfo!.FileName);
            Assert.EndsWith(" /f /admin", dependencies.StartInfo.Arguments, StringComparison.Ordinal);
            Assert.True(dependencies.TrackProcessLifetimeCalled);
            Assert.True(File.Exists(dependencies.CreatedLease!.FilePath));

            session.Disconnect();

            Assert.False(File.Exists(dependencies.CreatedLease.FilePath));
            Assert.Equal(1, dependencies.Process.CloseMainWindowCallCount);
            Assert.Equal(1, dependencies.Process.WaitForExitCallCount);
            Assert.False(dependencies.Process.KillCalled);
        }
        finally
        {
            TryDeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Connect_KeepsTemporaryFile_WhenConfigured()
    {
        var tempDirectory = CreateTemporaryDirectory();
        try
        {
            var configuration = CreateConfiguration(tempDirectory);
            configuration.External.KeepTemporaryRdpFile = true;

            var dependencies = new FakeExternalRdpSessionDependencies();
            using (var session = new ExternalRdpSession(NullLogger.Instance, dependencies))
            {
                session.Connect(RdpConnectionContextFactory.Create(configuration));
            }

            Assert.True(File.Exists(dependencies.CreatedLease!.FilePath));
        }
        finally
        {
            TryDeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Connect_DisposesArtifacts_WhenProcessStartFails()
    {
        var tempDirectory = CreateTemporaryDirectory();
        try
        {
            var configuration = CreateConfiguration(tempDirectory);
            var dependencies = new FakeExternalRdpSessionDependencies
            {
                StartProcessException = new InvalidOperationException("boom")
            };

            using var session = new ExternalRdpSession(NullLogger.Instance, dependencies);

            var exception = Assert.Throws<InvalidOperationException>(() => session.Connect(RdpConnectionContextFactory.Create(configuration)));

            Assert.Equal("boom", exception.Message);
            Assert.NotNull(dependencies.CreatedLease);
            Assert.True(dependencies.CredentialScopesCreated);
            Assert.True(dependencies.CredentialScopesDisposed);
            Assert.False(File.Exists(dependencies.CreatedLease!.FilePath));
        }
        finally
        {
            TryDeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Disconnect_KillsProcess_WhenGracefulShutdownFails_AndKillOnDisconnectIsEnabled()
    {
        var tempDirectory = CreateTemporaryDirectory();
        try
        {
            var configuration = CreateConfiguration(tempDirectory);
            configuration.External.KillProcessOnDisconnect = true;

            var dependencies = new FakeExternalRdpSessionDependencies();
            dependencies.Process.CloseMainWindowResult = false;

            using var session = new ExternalRdpSession(NullLogger.Instance, dependencies);
            session.Connect(RdpConnectionContextFactory.Create(configuration));

            session.Disconnect();

            Assert.True(dependencies.Process.KillCalled);
        }
        finally
        {
            TryDeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void Disconnect_DoesNotKillProcess_WhenGracefulShutdownFails_AndKillOnDisconnectIsDisabled()
    {
        var tempDirectory = CreateTemporaryDirectory();
        try
        {
            var configuration = CreateConfiguration(tempDirectory);
            configuration.External.KillProcessOnDisconnect = false;

            var dependencies = new FakeExternalRdpSessionDependencies();
            dependencies.Process.CloseMainWindowResult = false;

            using var session = new ExternalRdpSession(NullLogger.Instance, dependencies);
            session.Connect(RdpConnectionContextFactory.Create(configuration));

            session.Disconnect();

            Assert.False(dependencies.Process.KillCalled);
        }
        finally
        {
            TryDeleteDirectory(tempDirectory);
        }
    }

    private static RdpClientConfiguration CreateConfiguration(string temporaryDirectory)
    {
        return new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.External,
            External =
            {
                TemporaryDirectory = temporaryDirectory,
                UseCredentialManager = true,
                KillProcessOnHostExit = true
            }
        };
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "RoyalApps.Rdp.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void TryDeleteDirectory(string path)
    {
        if (!Directory.Exists(path))
            return;

        try
        {
            Directory.Delete(path, recursive: true);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private sealed class FakeExternalRdpSessionDependencies : IExternalRdpSessionDependencies
    {
        public FakeExternalProcess Process { get; } = new();

        public FakeCredentialScopes CredentialScopes { get; } = new();

        public ProcessStartInfo? StartInfo { get; private set; }

        public TemporaryRdpFileLease? CreatedLease { get; private set; }

        public bool CredentialScopesCreated { get; private set; }

        public bool CredentialScopesDisposed => CredentialScopes.IsDisposed;

        public bool TrackProcessLifetimeCalled { get; private set; }

        public Exception? StartProcessException { get; set; }

        public IDisposable? CreateCredentialScopes(RdpClientConfiguration configuration, Microsoft.Extensions.Logging.ILogger logger)
        {
            CredentialScopesCreated = true;
            return CredentialScopes;
        }

        public ITemporaryRdpFileLease CreateTemporaryRdpFileLease(RdpConnectionContext connectionContext, Microsoft.Extensions.Logging.ILogger logger)
        {
            CreatedLease = TemporaryRdpFileLease.Create(connectionContext, logger);
            return CreatedLease;
        }

        public string ResolveLauncherPath(RdpClientConfiguration configuration, Microsoft.Extensions.Logging.ILogger logger)
        {
            return configuration.External.UseMsRdpExHooks
                ? @"C:\Tools\mstscex.exe"
                : @"C:\Tools\mstsc.exe";
        }

        public IExternalProcess StartProcess(ProcessStartInfo startInfo)
        {
            StartInfo = startInfo;
            if (StartProcessException is not null)
                throw StartProcessException;

            return Process;
        }

        public void TrackProcessLifetime(IExternalProcess process, Microsoft.Extensions.Logging.ILogger logger)
        {
            TrackProcessLifetimeCalled = true;
        }

        public bool TryActivateWindow(int processId, out nint windowHandle)
        {
            windowHandle = IntPtr.Zero;
            return false;
        }

        public bool TryGetWindowHandle(int processId, out nint windowHandle)
        {
            windowHandle = IntPtr.Zero;
            return false;
        }
    }

    private sealed class FakeExternalProcess : IExternalProcess
    {
        public event EventHandler? Exited;

        public Process? UnderlyingProcess => null;

        public int Id { get; } = 1234;

        public bool HasExited { get; private set; }

        public int ExitCode { get; private set; }

        public bool EnableRaisingEvents { get; set; }

        public int CloseMainWindowCallCount { get; private set; }

        public int WaitForExitCallCount { get; private set; }

        public bool CloseMainWindowResult { get; set; } = true;

        public bool WaitForExitResult { get; set; } = true;

        public bool KillCalled { get; private set; }

        public bool DisposeCalled { get; private set; }

        public bool CloseMainWindow()
        {
            CloseMainWindowCallCount++;
            return CloseMainWindowResult;
        }

        public bool WaitForExit(TimeSpan timeout)
        {
            WaitForExitCallCount++;
            if (WaitForExitResult)
            {
                RaiseExited();
                return true;
            }

            HasExited = false;
            return WaitForExitResult;
        }

        public void Kill(bool entireProcessTree)
        {
            KillCalled = true;
            HasExited = true;
        }

        public void Dispose()
        {
            DisposeCalled = true;
        }

        public void RaiseExited(int exitCode = 0)
        {
            HasExited = true;
            ExitCode = exitCode;
            Exited?.Invoke(this, EventArgs.Empty);
        }
    }

    private sealed class FakeCredentialScopes : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
