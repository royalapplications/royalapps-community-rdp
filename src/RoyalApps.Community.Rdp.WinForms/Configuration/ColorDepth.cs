namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Remote Desktop Color Depth
/// </summary>
public enum ColorDepth
{
    /// <summary>
    /// 8 Bit-per-pixel, 256 colors
    /// </summary>
    ColorDepth8Bpp = 8,
    /// <summary>
    /// 15 Bit-per-pixel, high color
    /// </summary>
    ColorDepth15Bpp = 15,
    /// <summary>
    /// 16 Bit-per-pixel, high color
    /// </summary>
    ColorDepth16Bpp = 16,
    /// <summary>
    /// 24 Bit-per-pixel, true color
    /// </summary>
    ColorDepth24Bpp = 24,
    /// <summary>
    /// 32 Bit-per-pixel, true color (deep color)
    /// </summary>
    ColorDepth32Bpp = 32
}
