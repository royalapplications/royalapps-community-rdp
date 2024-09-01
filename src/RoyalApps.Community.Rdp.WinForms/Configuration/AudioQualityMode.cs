namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Specifies the audio quality mode.
/// </summary>
public enum AudioQualityMode
{
    /// <summary>
    /// Dynamic audio quality. This is the default audio quality setting. The server dynamically adjusts audio output quality in response to network conditions and the client and server capabilities.
    /// </summary>
    Dynamic = 0,
    /// <summary>
    /// Medium audio quality. The server uses a fixed but compressed format for audio output.
    /// </summary>
    Medium = 1,
    /// <summary>
    /// High audio quality. The server provides audio output in uncompressed PCM format with lower processing overhead for latency.
    /// </summary>
    High = 2,
}