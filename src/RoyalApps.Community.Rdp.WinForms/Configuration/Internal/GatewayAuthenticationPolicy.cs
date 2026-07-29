using RoyalApps.Community.Rdp.WinForms.Configuration.Connection;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Internal;

internal static class GatewayAuthenticationPolicy
{
    public static bool UsesPluggableAuthentication(GatewayConfiguration gateway) =>
        !string.IsNullOrEmpty(gateway.GatewayAccessToken?.GetValue());

    public static GatewayProfileUsageMethod GetEffectiveProfileUsageMethod(GatewayConfiguration gateway) =>
        UsesPluggableAuthentication(gateway)
            ? GatewayProfileUsageMethod.Explicit
            : gateway.GatewayProfileUsageMethod;

    public static GatewayCredentialSource GetEffectiveCredentialSource(GatewayConfiguration gateway) =>
        UsesPluggableAuthentication(gateway)
            ? GatewayCredentialSource.CookieBasedAuthentication
            : gateway.GatewayCredsSource;
}
