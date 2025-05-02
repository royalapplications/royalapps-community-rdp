namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// The network connection type information passed on to the server helps the server tune several parameters based on the network connection type.
/// </summary>
public enum NetworkConnectionType
{
    /// <summary>
    /// No network connection type is set.
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
    /// Wide area network (WAN) (10 Mbps or higher, with high latency).
    /// </summary>
    WAN,
    /// <summary>
    /// Local area network (LAN) (10 Mbps or higher).
    /// </summary>
    LAN,
    /// <summary>
    /// Automatically detect the network connection type.
    /// </summary>
    Automatic,
}
