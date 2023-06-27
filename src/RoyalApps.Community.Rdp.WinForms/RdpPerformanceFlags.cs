// ReSharper disable IdentifierTypo
namespace RoyalApps.Community.Rdp.WinForms;

/// <summary>
/// Handles RDP performance flags.
/// </summary>
public struct RdpPerformanceFlags
{
    private const int TS_PERF_DISABLE_WALLPAPER          = 0x00000001;
    private const int TS_PERF_DISABLE_FULLWINDOWDRAG     = 0x00000002;
    private const int TS_PERF_DISABLE_MENUANIMATIONS     = 0x00000004;
    private const int TS_PERF_DISABLE_THEMING            = 0x00000008;
    private const int TS_PERF_ENABLE_ENHANCED_GRAPHICS   = 0x00000010;
    private const int TS_PERF_DISABLE_CURSOR_SHADOW      = 0x00000020;
    private const int TS_PERF_DISABLE_CURSORSETTINGS     = 0x00000040;
    private const int TS_PERF_ENABLE_FONT_SMOOTHING      = 0x00000080;
    private const int TS_PERF_ENABLE_DESKTOP_COMPOSITION = 0x00000100;

    /// <summary>
    /// If true, desktop wallpaper is disabled in the remote session.
    /// </summary>
    public bool DisableWallpaper { get; set; }
    /// <summary>
    /// If true, moving window will only show the border of the window during the drag operation.
    /// </summary>
    public bool DisableFullWindowDrag { get; set; }
    /// <summary>
    /// If true, menu animations are disabled.
    /// </summary>
    public bool DisableMenuAnimations { get; set; }
    /// <summary>
    /// If true, theming in the remote session is disabled.
    /// </summary>
    public bool DisableTheming { get; set; }
    /// <summary>
    /// If true, enhanced graphics is enabled.
    /// </summary>
    public bool EnableEnhancedGraphics { get; set; }
    /// <summary>
    /// If true, cursor shadow is disabled.
    /// </summary>
    public bool DisableCursorShadow { get; set; }
    /// <summary>
    /// If true, cursor blinking is disabled.
    /// </summary>
    public bool DisableCursorSettings { get; set; }
    /// <summary>
    /// If true, font smoothing is enabled.
    /// </summary>
    public bool EnableFontSmoothing { get; set; }
    /// <summary>
    /// If true, desktop composition is enabled.
    /// </summary>
    public bool EnableDesktopComposition { get; set; }

    /// <summary>
    /// Returns the configured performance flags as integer.
    /// </summary>
    /// <returns>An integer representing the configured performance flags.</returns>
    public int GetPerformanceFlags()
    {
        var flags = 0;
        if (DisableWallpaper)
            flags += TS_PERF_DISABLE_WALLPAPER;
        if (DisableFullWindowDrag)
            flags += TS_PERF_DISABLE_FULLWINDOWDRAG;
        if (DisableMenuAnimations)
            flags += TS_PERF_DISABLE_MENUANIMATIONS;
        if (DisableTheming)
            flags += TS_PERF_DISABLE_THEMING;
        if (EnableEnhancedGraphics)
            flags += TS_PERF_ENABLE_ENHANCED_GRAPHICS;
        if (DisableCursorShadow)
            flags += TS_PERF_DISABLE_CURSOR_SHADOW;
        if (DisableCursorSettings)
            flags += TS_PERF_DISABLE_CURSORSETTINGS;
        if (EnableFontSmoothing)
            flags += TS_PERF_ENABLE_FONT_SMOOTHING;
        if (EnableDesktopComposition)
            flags += TS_PERF_ENABLE_DESKTOP_COMPOSITION;
        return flags;
    }

    /// <summary>
    /// Sets the performance flags based on the integer.
    /// </summary>
    /// <param name="performanceFlags">The performance flags as integer.</param>
    public void SetPerformanceFlags(int performanceFlags)
    {
        DisableWallpaper = (performanceFlags & TS_PERF_DISABLE_WALLPAPER) == TS_PERF_DISABLE_WALLPAPER;
        DisableFullWindowDrag = (performanceFlags & TS_PERF_DISABLE_FULLWINDOWDRAG) == TS_PERF_DISABLE_FULLWINDOWDRAG;
        DisableMenuAnimations = (performanceFlags & TS_PERF_DISABLE_MENUANIMATIONS) == TS_PERF_DISABLE_MENUANIMATIONS;
        DisableTheming = (performanceFlags & TS_PERF_DISABLE_THEMING) == TS_PERF_DISABLE_THEMING;
        EnableEnhancedGraphics = (performanceFlags & TS_PERF_ENABLE_ENHANCED_GRAPHICS) == TS_PERF_ENABLE_ENHANCED_GRAPHICS;
        DisableCursorShadow = (performanceFlags & TS_PERF_DISABLE_CURSOR_SHADOW) == TS_PERF_DISABLE_CURSOR_SHADOW;
        DisableCursorSettings = (performanceFlags & TS_PERF_DISABLE_CURSORSETTINGS) == TS_PERF_DISABLE_CURSORSETTINGS;
        EnableFontSmoothing = (performanceFlags & TS_PERF_ENABLE_FONT_SMOOTHING) == TS_PERF_ENABLE_FONT_SMOOTHING;
        EnableDesktopComposition = (performanceFlags & TS_PERF_ENABLE_DESKTOP_COMPOSITION) == TS_PERF_ENABLE_DESKTOP_COMPOSITION;
    }
    
}