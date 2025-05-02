namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Specifies when to use a Remote Desktop Gateway (RD Gateway) server.
/// </summary>
public enum GatewayUsageMethod
{
    /// <summary>
    /// Do not use an RD Gateway server. In the Remote Desktop Connection (RDC) client UI, the Bypass RD Gateway server for local addresses check box is cleared.
    /// </summary>
    Never = 0,
    /// <summary>
    /// Always use an RD Gateway server. In the RDC client UI, the Bypass RD Gateway server for local addresses check box is cleared.
    /// </summary>
    Always = 1,
    /// <summary>
    /// Use an RD Gateway server if a direct connection cannot be made to the RD Session Host server. In the RDC client UI, the Bypass RD Gateway server for local addresses check box is selected.
    /// </summary>
    OnDemand = 2
}

