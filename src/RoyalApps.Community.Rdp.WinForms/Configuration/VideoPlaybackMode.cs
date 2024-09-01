namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Specifies the video playback mode, which specifies if video decoding and rendering is redirected to the client.
/// </summary>
public enum VideoPlaybackMode
{
    /// <summary>
    /// Decode and render on the server.
    /// </summary>
    DecodeAndRenderOnServer = 0,
    /// <summary>
    /// Decode and render on the client.
    /// </summary>
    DecodeAndRenderOnClient = 1,
}