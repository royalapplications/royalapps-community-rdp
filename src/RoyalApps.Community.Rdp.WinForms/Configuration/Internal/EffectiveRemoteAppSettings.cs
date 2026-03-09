namespace RoyalApps.Community.Rdp.WinForms.Configuration.Internal;

internal readonly record struct EffectiveRemoteAppSettings(
    bool Enabled,
    string? Program,
    string? Name,
    string? CommandLine,
    bool ExpandCommandLine,
    string? WorkingDirectory,
    bool ExpandWorkingDirectory,
    string? File,
    string? Icon,
    bool DisableCapabilitiesCheck);
