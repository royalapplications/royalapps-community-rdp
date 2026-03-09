using System;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Internal;

internal static class RdpConnectionContextFactory
{
    public static RdpConnectionContext Create(RdpClientConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var context = RdpConfigurationNormalizer.Normalize(configuration);
        RdpConfigurationValidator.ValidateForConnect(context);
        return context;
    }
}
