using RoyalApps.Community.Rdp.WinForms.Configuration.Connection;

namespace RoyalApps.Community.Rdp.WinForms.Controls.ActiveX;

internal static class MsRdpExHookPolicy
{
    public static bool ShouldEnable(
        bool useMsRdc,
        bool enableSessionCapture,
        bool logEnabled,
        GatewayUsageMethod gatewayUsageMethod) =>
        // Gateway isolation requires hooks even when diagnostics and capture are disabled.
        useMsRdc || enableSessionCapture || logEnabled || gatewayUsageMethod != GatewayUsageMethod.Never;
}
