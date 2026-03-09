using System;
using RoyalApps.Community.Rdp.WinForms.Configuration.Applications;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Internal;

internal static class RemoteAppConfigurationExtensions
{
    public static EffectiveRemoteAppSettings GetEffectiveSettings(this RemoteAppConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return new EffectiveRemoteAppSettings(
            configuration.Enabled,
            Normalize(configuration.Program),
            Normalize(configuration.Name),
            Normalize(configuration.CommandLine),
            configuration.ExpandCommandLine,
            Normalize(configuration.WorkingDirectory),
            configuration.ExpandWorkingDirectory,
            Normalize(configuration.File),
            Normalize(configuration.Icon),
            configuration.DisableCapabilitiesCheck);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
