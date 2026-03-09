using System;
using RoyalApps.Community.Rdp.WinForms.Configuration.Internal;

namespace RoyalApps.Community.Rdp.WinForms.Controls.ActiveX;

internal static class EmbeddedRdpClientConfigurationMapper
{
    public static void Apply(RdpControl rdpControl, RdpConnectionContext connectionContext)
    {
        ArgumentNullException.ThrowIfNull(rdpControl);
        ArgumentNullException.ThrowIfNull(connectionContext);

        rdpControl.ApplyRdpClientConfiguration(connectionContext);
    }
}
