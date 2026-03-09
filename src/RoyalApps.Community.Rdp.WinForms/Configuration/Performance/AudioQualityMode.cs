namespace RoyalApps.Community.Rdp.WinForms.Configuration.Performance;

/// <summary>
/// Specifies the audio quality mode.
/// </summary>
public enum AudioQualityMode
{
    /// <summary>
    /// Use dynamic audio quality based on network conditions and client and server capabilities.
    /// </summary>
    Dynamic = 0,
    /// <summary>
    /// Use a fixed, compressed audio format.
    /// </summary>
    Medium = 1,
    /// <summary>
    /// Use high-quality uncompressed PCM audio.
    /// </summary>
    High = 2,
}
