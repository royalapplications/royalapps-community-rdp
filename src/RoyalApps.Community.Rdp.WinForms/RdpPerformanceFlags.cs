namespace RoyalApps.Community.Rdp.WinForms;

internal struct RdpPerformanceFlags
{
    private const int TS_PERF_DISABLE_WALLPAPER          = 0x00000001;
    private const int TS_PERF_DISABLE_FULLWINDOWDRAG     = 0x00000002;
    private const int TS_PERF_DISABLE_MENUANIMATIONS     = 0x00000004;
    private const int TS_PERF_DISABLE_THEMING            = 0x00000008;
    private const int TS_PERF_DISABLE_ENHANCED_GRAPHICS  = 0x00000010;
    private const int TS_PERF_DISABLE_CURSOR_SHADOW      = 0x00000020;
    private const int TS_PERF_DISABLE_CURSORSETTINGS     = 0x00000040;
    private const int TS_PERF_ENABLE_FONT_SMOOTHING      = 0x00000080;
    private const int TS_PERF_ENABLE_DESKTOP_COMPOSITION = 0x00000100;

    public bool DisableWallpaper { get; set; }
    public bool DisableFullWindowDrag { get; set; }
    public bool DisableMenuAnimations { get; set; }
    public bool DisableTheming { get; set; }
    public bool DisableEnhancedGraphics { get; set; }
    public bool DisableCursorShadow { get; set; }
    public bool DisableCursorSettings { get; set; }
    public bool EnableFontSmoothing { get; set; }
    public bool EnableDesktopComposition { get; set; }

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
        if (DisableEnhancedGraphics)
            flags += TS_PERF_DISABLE_ENHANCED_GRAPHICS;
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
}