namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// The display settings used for the remote desktop session.
/// </summary>
public class DisplayConfiguration
{
    /// <summary>
    /// Specifies the current control's width, in pixels, on the initial remote desktop.
    /// Setting the DesktopWidth property is optional, but it must be set before calling the Connect method.
    /// If a desktop width is not specified, or is set to zero, the desktop width is set to the width of the control.
    /// The minimum and maximum values are dependent upon the operating system version of the Remote Desktop client.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-desktopwidth</cref>
    /// </seealso>
    public int DesktopWidth { get; set; }

    /// <summary>
    /// Specifies the current control's height, in pixels, on the initial remote desktop.
    /// Setting the DesktopHeight property is optional, but it must be set before calling the Connect method.
    /// If a desktop height is not specified, or is set to zero, the desktop height is set to the height of the control.
    /// The minimum and maximum values are dependent upon the operating system version of the Remote Desktop client.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-desktopheight</cref>
    /// </seealso>
    public int DesktopHeight { get; set; }

    /// <summary>
    /// The color depth (in bits per pixel) for the control's connection.
    /// Values include 8, 15, 16, 24, and 32 bits per pixel.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclient-colordepth</cref>
    /// </seealso>
    public ColorDepth ColorDepth { get; set; } = ColorDepth.ColorDepth32Bpp;

    /// <summary>
    /// If set to true, DPI scaling will be applied automatically.
    /// </summary>
    public bool AutoScaling { get; set; } = true;

    /// <summary>
    /// Use local scaling instead of setting the DPI in the remote session.
    /// </summary>
    public bool UseLocalScaling { get; set; }
    
    /// <summary>
    /// The initial zoom level in percent (default is 100).
    /// </summary>
    public int InitialZoomLevel { get; set; } = 100;

    /// <summary>
    /// The behavior when the control is resized.
    /// </summary>
    public ResizeBehavior ResizeBehavior { get; set; } = ResizeBehavior.SmartReconnect;
    
    ///  <summary>
    /// Determines whether the client control is in full-screen mode.
    ///  True to enter full-screen mode, False to leave full-screen mode and return to window mode.
    ///  Note: You can set this property when the control is connected.
    ///  </summary>
    ///  <seealso>
    ///      <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclient-fullscreen</cref>
    ///  </seealso>
    public bool FullScreen { get; set; }

    /// <summary>
    /// Specifies whether the container-handled full-screen mode is enabled.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-containerhandledfullscreen</cref>
    /// </seealso>
    public bool ContainerHandledFullScreen { get; set; }
    
    /// <summary>
    /// Specifies the window title displayed when the control is in full-screen mode.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-fullscreentitle</cref>
    /// </seealso>
    public string? FullScreenTitle { get; set; }
    
    /// <summary>
    /// Specifies whether to use the connection bar. The default value is true, which enables the property.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-displayconnectionbar</cref>
    /// </seealso>
    public bool DisplayConnectionBar { get; set; }

    /// <summary>
    /// Specifies the state of the UI connection bar.
    /// Setting this property to true sets the state to "lowered", that is, invisible to the user and unavailable for input.
    /// False sets the state to "raised" and available for user input.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-pinconnectionbar</cref>
    /// </seealso>
    public bool PinConnectionBar { get; set; }

    /// <summary>
    /// Not supported in this version
    /// Specifies whether the Remote Desktop ActiveX control should use multiple monitors.
    /// If this property is true, the control will use multiple monitors.
    /// If this property is false, the control will not use multiple monitors.
    /// Note: You cannot set this property while the connection has been established.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientnonscriptable5-usemultimon</cref>
    /// </seealso>
    public bool UseMultimon { get; set; }
}