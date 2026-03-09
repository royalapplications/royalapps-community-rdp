using System;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Internal;

internal static class RdpConfigurationNormalizer
{
    public static RdpConnectionContext Normalize(RdpClientConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return new RdpConnectionContext(
            configuration,
            configuration.Security.GetEffectiveSettings(),
            configuration.RemoteApp.GetEffectiveSettings());
    }
}
