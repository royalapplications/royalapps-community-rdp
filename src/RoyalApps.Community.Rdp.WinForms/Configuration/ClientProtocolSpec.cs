namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Specifies the remote desktop protocol used between the client and the server.
/// </summary>
public enum ClientProtocolSpec
{
    /// <summary>
    /// FullMode (0, full Windows 8 remote desktop protocol with max. memory footprint)
    /// </summary>
    FullMode,
    /// <summary>
    /// ThinClientMode (1: limited to using the Win7 SP1 RemoteFX codec and smallest memory footprint)
    /// </summary>
    ThinClientMode,
    /// <summary>
    /// SmallCacheMode (2: the protocol is the same as full mode but with smaller cache size)
    /// </summary>
    SmallCacheMode
}
