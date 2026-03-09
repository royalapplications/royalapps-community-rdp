using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Internal;
using RoyalApps.Community.Rdp.WinForms.External.Files;

namespace RoyalApps.Community.Rdp.WinForms.External.Runtime;

internal interface IExternalRdpSessionDependencies
{
    IDisposable? CreateCredentialScopes(RdpClientConfiguration configuration, ILogger logger);

    ITemporaryRdpFileLease CreateTemporaryRdpFileLease(RdpConnectionContext connectionContext, ILogger logger);

    string ResolveLauncherPath(RdpClientConfiguration configuration, ILogger logger);

    IExternalProcess StartProcess(ProcessStartInfo startInfo);

    void TrackProcessLifetime(IExternalProcess process, ILogger logger);

    bool TryActivateWindow(int processId, out nint windowHandle);

    bool TryGetWindowHandle(int processId, out nint windowHandle);
}
