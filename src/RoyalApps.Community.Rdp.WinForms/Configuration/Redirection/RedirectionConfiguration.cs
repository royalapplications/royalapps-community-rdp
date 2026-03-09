using System.ComponentModel;
using RoyalApps.Community.Rdp.WinForms.Configuration.Performance;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Redirection;

/// <summary>
/// Configuration and settings for device redirection.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public class RedirectionConfiguration : ExpandableObjectConverter
{
    /// <summary>
    /// Specifies the audio redirection settings, which specify whether to redirect sounds or play sounds at the Remote Desktop Session Host (RD Session Host) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientsecuredsettings-autoredirectionmode</cref>
    /// </see>
    public AudioRedirectionMode AudioRedirectionMode { get; set; }

    /// <summary>
    /// Specifies the audio quality mode.
    /// </summary>
    /// <see>
    ///     <cref>https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings7-audioqualitymode</cref>
    /// </see>
    public AudioQualityMode AudioQualityMode { get; set; }

    /// <summary>
    /// Specifies or retrieves a Boolean value that indicates whether the default audio input device is redirected from the client to the remote session.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings7-audiocaptureredirectionmode</cref>
    /// </see>
    public bool AudioCaptureRedirectionMode { get; set; }

    /// <summary>
    /// Specifies whether video decoding and rendering are redirected to the client.
    /// </summary>
    /// <see>
    ///     <cref>https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings7-videoplaybackmode</cref>
    /// </see>
    public bool RedirectVideoRendering { get; set; }

    /// <summary>
    /// Specifies whether printer redirection is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-redirectprinters</cref>
    /// </see>
    public bool RedirectPrinters { get; set; }

    /// <summary>
    /// Sets or retrieves the configuration for clipboard redirection.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings5-redirectclipboard</cref>
    /// </see>
    public bool RedirectClipboard { get; set; }

    /// <summary>
    /// Specifies whether smart card redirection is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-redirectsmartcards</cref>
    /// </see>
    public bool RedirectSmartCards { get; set; }

    /// <summary>
    /// Specifies whether local port redirection, for example COM and LPT, is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-redirectports</cref>
    /// </see>
    public bool RedirectPorts { get; set; }

    /// <summary>
    /// Sets or retrieves the configuration for device redirection.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings5-redirectdevices</cref>
    /// </see>
    public bool RedirectDevices { get; set; }

    /// <summary>
    /// Sets or retrieves the configuration for Point of Service device redirection.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings5-redirectposdevices</cref>
    /// </see>
    public bool RedirectPointOfServiceDevices { get; set; }

    /// <summary>
    /// Specifies whether drive redirection is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-redirectdrives</cref>
    /// </see>
    public bool RedirectDrives { get; set; }

    /// <summary>
    /// A string representing the drive letters which should be redirected (e.g. 'CEH' for the drives C:, E: and H:)
    /// If empty, all drives are redirected when RedirectDrives property is true.
    /// </summary>
    public string? RedirectDriveLetters { get; set; }

    /// <summary>
    /// Specifies whether camera redirection is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpclientnonscriptable7-cameraredirconfigcollection</cref>
    /// </see>
    public bool RedirectCameras { get; set; }

    /// <summary>
    /// Specifies whether location redirection is allowed.
    /// </summary>
    /// <see>
    ///     EnableLocationRedirection <cref>https://learn.microsoft.com/de-de/windows/win32/termserv/imsrdpextendedsettings-property#property-value</cref>
    /// </see>
    public bool RedirectLocation { get; set; }

    /// <summary>
    /// Returns an empty string so this configuration group appears as a blank expandable node in PropertyGrid-style editors.
    /// </summary>
    /// <returns>An empty string.</returns>
    public override string ToString()
    {
        return string.Empty;
    }
}
