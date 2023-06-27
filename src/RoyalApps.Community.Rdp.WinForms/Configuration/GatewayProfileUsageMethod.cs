namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Specifies whether to use default Remote Desktop Gateway (RD Gateway) settings.
/// </summary>
public enum GatewayProfileUsageMethod
{
    /// <summary>
    /// Use the default profile mode, as specified by the administrator.
    /// </summary>
    Default = 0,
    /// <summary>
    /// Use explicit settings, as specified by the user.
    /// </summary>
    Explicit = 1
}
