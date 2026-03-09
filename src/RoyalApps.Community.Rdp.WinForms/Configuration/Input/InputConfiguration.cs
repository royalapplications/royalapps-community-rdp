using System.ComponentModel;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Input;

/// <summary>
/// Input related configuration and settings.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public class InputConfiguration : ExpandableObjectConverter
{
    /// <summary>
    /// Specifies whether background input mode is enabled. When background input is enabled the client can accept input when the client does not have focus.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-allowbackgroundinput</cref>
    /// </seealso>
    public bool AllowBackgroundInput { get; set; }

    /// <summary>
    /// When <see langword="true"/>, a mouse click in the remote session does not transfer input focus to the session.
    /// </summary>
    public bool DisableClickDetection { get; set; }

    /// <summary>
    /// Specifies whether the client control should have focus while connecting. The control does not attempt to grab focus from a window running in a different process.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-grabfocusonconnect</cref>
    /// </seealso>
    public bool GrabFocusOnConnect { get; set; }

    /// <summary>
    /// Specifies whether the mouse should use relative mode. Contains VARIANT_TRUE if the mouse is in relative mode or VARIANT_FALSE if the mouse is in absolute mode.
    /// The mouse mode indicates how the ActiveX control calculates the mouse coordinates that it sends to the Remote Desktop Session Host (RD Session Host) server. When the mouse is in relative mode, the ActiveX control calculates mouse coordinates relative to the mouse's last position. When the mouse is in absolute mode, the ActiveX control calculates mouse coordinates relative to the desktop of the RD Session Host server.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings6-relativemousemode</cref>
    /// </seealso>
    public bool RelativeMouseMode { get; set; } = true;

    /// <summary>
    /// Specifies whether keyboard accelerators should be passed to the server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-acceleratorpassthrough</cref>
    /// </see>
    public bool AcceleratorPassthrough { get; set; }

    /// <summary>
    /// Specifies whether the Windows key can be used in the remote session.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-enablewindowskey</cref>
    /// </see>
    public bool EnableWindowsKey { get; set; }

    /// <summary>
    /// Specifies the keyboard redirection settings, which specify how and when to apply Windows keyboard shortcut (for example, ALT+TAB).
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientsecuredsettings-keyboardhookmode</cref>
    /// </see>
    public bool KeyboardHookMode { get; set; }

    /// <summary>
    /// When enabled, ctrl+alt+space can be used to toggle between local and remote keyboard hook modes. Only works when the session is in full screen mode.
    /// </summary>
    public bool KeyboardHookToggleShortcutEnabled { get; set; }

    /// <summary>
    /// Specifies the name of the active input locale identifier (formerly called the keyboard layout) to use for the connection.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-keyboardlayoutstr</cref>
    /// </see>
    public string? KeyBoardLayoutStr { get; set; }

    /// <summary>
    /// Returns an empty string so this configuration group appears as a blank expandable node in PropertyGrid-style editors.
    /// </summary>
    /// <returns>An empty string.</returns>
    public override string ToString()
    {
        return string.Empty;
    }
}
