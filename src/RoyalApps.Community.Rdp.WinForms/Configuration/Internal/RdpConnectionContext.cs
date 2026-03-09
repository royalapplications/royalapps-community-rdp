using System;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Internal;

internal sealed class RdpConnectionContext
{
    public RdpConnectionContext(
        RdpClientConfiguration configuration,
        EffectiveSecuritySettings effectiveSecurity,
        EffectiveRemoteAppSettings effectiveRemoteApp)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        EffectiveSecurity = effectiveSecurity;
        EffectiveRemoteApp = effectiveRemoteApp;
    }

    public RdpClientConfiguration Configuration { get; }

    public EffectiveSecuritySettings EffectiveSecurity { get; }

    public EffectiveRemoteAppSettings EffectiveRemoteApp { get; }

    public bool IsExternalMode => Configuration.SessionMode == RdpSessionMode.External;

    public bool IsEmbeddedMode => !IsExternalMode;
}
