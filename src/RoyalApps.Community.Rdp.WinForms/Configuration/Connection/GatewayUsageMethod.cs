namespace RoyalApps.Community.Rdp.WinForms.Configuration.Connection;

/// <summary>
/// Specifies when an RD Gateway server should be used.
/// </summary>
public enum GatewayUsageMethod
{
    /// <summary>
    /// Never use an RD Gateway server.
    /// </summary>
    Never = 0,
    /// <summary>
    /// Always use an RD Gateway server.
    /// </summary>
    Always = 1,
    /// <summary>
    /// Use an RD Gateway server only if a direct connection cannot be made.
    /// </summary>
    OnDemand = 2,

    /// <summary>
    /// Use the default RD Gateway settings configured for the published resource or workspace.
    /// </summary>
    UseDefaultSettings = 3,

    /// <summary>
    /// Bypass the RD Gateway server for local addresses.
    /// </summary>
    BypassLocalAddresses = 4
}

