namespace RoyalApps.Community.Rdp.WinForms.Configuration.Connection;

/// <summary>
/// Specifies the RDP protocol profile used between the client and the server.
/// </summary>
public enum ClientProtocolSpec
{
    /// <summary>
    /// Use the full protocol profile with the highest memory usage.
    /// </summary>
    FullMode,
    /// <summary>
    /// Use the thin-client profile with the smallest memory footprint.
    /// </summary>
    ThinClientMode,
    /// <summary>
    /// Use the full protocol profile with smaller caches.
    /// </summary>
    SmallCacheMode
}
