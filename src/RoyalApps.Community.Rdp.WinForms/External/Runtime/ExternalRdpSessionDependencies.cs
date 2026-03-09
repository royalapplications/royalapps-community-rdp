using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Internal;
using RoyalApps.Community.Rdp.WinForms.External.Credentials;
using RoyalApps.Community.Rdp.WinForms.External.Files;

namespace RoyalApps.Community.Rdp.WinForms.External.Runtime;

internal sealed class ExternalRdpSessionDependencies : IExternalRdpSessionDependencies
{
    public static ExternalRdpSessionDependencies Instance { get; } = new();

    private ExternalRdpSessionDependencies()
    {
    }

    public IDisposable? CreateCredentialScopes(RdpClientConfiguration configuration, ILogger logger)
    {
        return ExternalCredentialScopes.Create(configuration, logger);
    }

    public ITemporaryRdpFileLease CreateTemporaryRdpFileLease(RdpConnectionContext connectionContext, ILogger logger)
    {
        return TemporaryRdpFileLease.Create(connectionContext, logger);
    }

    public string ResolveLauncherPath(RdpClientConfiguration configuration, ILogger logger)
    {
        var msRdpExPath = MsRdpExLocator.ResolveLauncherPath(configuration, logger);
        if (!string.IsNullOrWhiteSpace(msRdpExPath))
            return msRdpExPath;

        var mstscPath = string.IsNullOrWhiteSpace(configuration.External.MstscPath)
            ? Environment.ExpandEnvironmentVariables(@"%SystemRoot%\System32\mstsc.exe")
            : Environment.ExpandEnvironmentVariables(configuration.External.MstscPath);

        if (System.IO.File.Exists(mstscPath))
            return mstscPath;

        throw new System.IO.FileNotFoundException("The Remote Desktop client executable could not be found.", mstscPath);
    }

    public IExternalProcess StartProcess(ProcessStartInfo startInfo)
    {
        var process = Process.Start(startInfo) ?? throw new InvalidOperationException("The external RDP process could not be started.");
        return new ExternalProcessAdapter(process);
    }

    public void TrackProcessLifetime(IExternalProcess process, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(process);
        ArgumentNullException.ThrowIfNull(logger);

        if (process.UnderlyingProcess is null)
            return;

        ProcessLifetimeTracker.Track(process.UnderlyingProcess, logger);
    }

    public bool TryActivateWindow(int processId, out nint windowHandle)
    {
        return ExternalSessionWindowTracker.TryActivateWindow(processId, out windowHandle);
    }

    public bool TryGetWindowHandle(int processId, out nint windowHandle)
    {
        return ExternalSessionWindowTracker.TryGetWindowHandle(processId, out windowHandle);
    }
}
