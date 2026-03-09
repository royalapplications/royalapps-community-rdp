namespace RoyalApps.Community.Rdp.WinForms.Configuration.Connection;

/// <summary>
/// Specifies how RD Gateway profile settings are resolved.
/// </summary>
public enum GatewayProfileUsageMethod
{
    /// <summary>
    /// Use the default profile mode configured by the administrator.
    /// </summary>
    Default = 0,
    /// <summary>
    /// Use explicit settings provided by the caller.
    /// </summary>
    Explicit = 1
}

