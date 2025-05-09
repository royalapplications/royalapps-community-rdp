using System;
using System.ComponentModel;

namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Performance related configuration and settings.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public class PerformanceConfiguration : ExpandableObjectConverter
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
    /// Specifies whether bitmap caching is enabled. Persistent caching can improve performance but requires additional disk space.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-bitmappersistence</cref>
    /// </seealso>
    public bool BitmapCaching { get; set; }

    /// <summary>
    /// Gets or sets the type of network connection used between the client and server. The network connection type information passed on to the server helps the server tune several parameters based on the network connection type
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings7-networkconnectiontype</cref>
    /// </see>
    public NetworkConnectionType NetworkConnectionType { get; set; } = NetworkConnectionType.BroadbandHigh;

    /// <summary>
    /// Specifies if bandwidth changes are automatically detected.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings8-bandwidthdetection</cref>
    /// </see>
    public bool BandwidthDetection { get; set; }

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
    /// Specifies whether or not DirectX is enabled or not.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings7-redirectdirectx</cref>
    /// </see>\
    [Obsolete("According to the documentation, this property is not used and has no effect. Will be removed in a future update.")]
    public bool RedirectDirectX { get; set; }

    /// <summary>
    ///  If set to true, hardware assist with graphics decoding is attempted.
    /// </summary>
    public bool EnableHardwareMode { get; set; }

    /// <summary>
    /// Specifies the remote desktop protocol used between the client and the server.
    /// </summary>
    public ClientProtocolSpec ClientProtocolSpec { get; set; }

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns>Empty string.</returns>
    public override string ToString()
    {
        return string.Empty;
    }

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
