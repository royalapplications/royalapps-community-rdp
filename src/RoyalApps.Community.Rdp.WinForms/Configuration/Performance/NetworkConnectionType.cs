namespace RoyalApps.Community.Rdp.WinForms.Configuration.Performance;

/// <summary>
/// Specifies the network connection type reported to the server.
/// </summary>
public enum NetworkConnectionType
{
    /// <summary>
    /// No connection type is specified.
    /// </summary>
    NotSet,
    /// <summary>
    /// Modem (56 Kbps).
    /// </summary>
    Modem,
    /// <summary>
    /// Low-speed broadband (256 Kbps to 2 Mbps).
    /// </summary>
    BroadbandLow,
    /// <summary>
    /// Satellite (2 Mbps to 16 Mbps, with high latency).
    /// </summary>
    Satellite,
    /// <summary>
    /// High-speed broadband (2 Mbps to 10 Mbps).
    /// </summary>
    BroadbandHigh,
    /// <summary>
    /// Wide area network (WAN), typically 10 Mbps or higher with high latency.
    /// </summary>
    WAN,
    /// <summary>
    /// Local area network (LAN), typically 10 Mbps or higher.
    /// </summary>
    LAN,
    /// <summary>
    /// Detect the network connection type automatically.
    /// </summary>
    Automatic,
}
