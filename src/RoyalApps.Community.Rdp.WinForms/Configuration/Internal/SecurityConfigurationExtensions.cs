using System;
using RoyalApps.Community.Rdp.WinForms.Configuration.Security;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Internal;

internal static class SecurityConfigurationExtensions
{
    public static EffectiveSecuritySettings GetEffectiveSettings(this SecurityConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return new EffectiveSecuritySettings(
            string.IsNullOrWhiteSpace(configuration.AuthenticationServiceClass)
                ? null
                : configuration.AuthenticationServiceClass.Trim(),
            configuration.DisableCredentialsDelegation || configuration.RemoteCredentialGuard || configuration.RestrictedAdminMode,
            configuration.RedirectedAuthentication || configuration.RemoteCredentialGuard,
            configuration.RestrictedLogon || configuration.RestrictedAdminMode);
    }
}
