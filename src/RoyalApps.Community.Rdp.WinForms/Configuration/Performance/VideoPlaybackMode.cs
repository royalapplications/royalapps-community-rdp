namespace RoyalApps.Community.Rdp.WinForms.Configuration.Performance;

/// <summary>
/// Specifies the video playback mode, including whether video decoding and rendering are redirected to the client.
/// </summary>
public enum VideoPlaybackMode
{
    /// <summary>
    /// Decode and render video on the server.
    /// </summary>
    DecodeAndRenderOnServer = 0,
    /// <summary>
    /// Decode and render video on the client.
    /// </summary>
    DecodeAndRenderOnClient = 1,
}
