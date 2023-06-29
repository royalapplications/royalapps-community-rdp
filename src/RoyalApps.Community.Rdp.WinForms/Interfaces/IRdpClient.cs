using System;
using AxMSTSCLib;
using Microsoft.Extensions.Logging;
using MSTSCLib;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using IMsTscAxEvents_OnConfirmCloseEventHandler = AxMSTSCLib.IMsTscAxEvents_OnConfirmCloseEventHandler;
using IMsTscAxEvents_OnDisconnectedEventHandler = AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEventHandler;

namespace RoyalApps.Community.Rdp.WinForms.Interfaces;

#region --- Enums ---

/// <inheritdoc cref="ConnectionState"/>
public enum ConnectionState
{
    /// <inheritdoc cref="Disconnected"/>
    Disconnected = 0,
    /// <inheritdoc cref="Connected"/>
    Connected = 1,
    /// <inheritdoc cref="Connecting"/>
    Connecting = 2
}

#endregion

/// <summary>
/// RdpClient provides safe and logged access to properties, methods, events to the underlying ActiveX.
/// </summary>
/// <inheritdoc cref="AxMsRdpClient9NotSafeForScripting"/>
public interface IRdpClient : IDisposable
{
    #region --- Events ---

    /// <inheritdoc cref="OnConnected"/>
    event EventHandler OnConnected;
    /// <inheritdoc cref="OnDisconnected"/>
    event IMsTscAxEvents_OnDisconnectedEventHandler OnDisconnected;
    /// <inheritdoc cref="OnConfirmClose"/>
    event IMsTscAxEvents_OnConfirmCloseEventHandler OnConfirmClose;
    /// <inheritdoc cref="OnRequestContainerMinimize"/>
    event EventHandler OnRequestContainerMinimize;
    /// <inheritdoc cref="OnRequestLeaveFullScreen"/>
    event EventHandler OnRequestLeaveFullScreen;
    /// <summary>
    /// This event is raised when a user clicks into a remote desktop session and DisableClickDetection is not set. 
    /// </summary>
    event EventHandler OnClientAreaClicked;

    #endregion

    #region --- RDP Settings ---

    #region ::: General :::

    /// <summary>
    /// Specifies the name of the server to which the current control is connected.
    /// The new server name. This parameter can be a DNS name or IP address.
    /// This property must be set before calling the Connect method. It is the only property that must be set before connecting.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-server</cref>
    /// </seealso>
    string Server { get; set; }

    /// <summary>
    /// Specifies the connection port. The default value is 3389.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-rdpport</cref>
    /// </seealso>
    int Port { get; set; }

    /// <summary>
    /// Retrieves the connection state of the current control.
    /// This can be one of the following values.
    /// 0 = The control is not connected.
    /// 1 = The control is connected.
    /// 2 = The control is establishing a connection.
    /// </summary>
    ConnectionState ConnectionState { get; }

    #endregion

    #region ::: Credentials :::

    /// <summary>
    /// Specifies the user name logon credential.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-username</cref>
    /// </seealso>
    string? UserName { get; set; }

    /// <summary>
    /// Specifies the domain to which the current user logs on.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-domain</cref>
    /// </seealso>
    string? Domain { get; set; }

    /// <summary>
    /// Sets the Remote Desktop ActiveX control password in plaintext format.
    /// The password is passed to the server in the safely encrypted RDP communications channel. After a plaintext password is set, it cannot be retrieved in plaintext format.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscnonscriptable-cleartextpassword</cref>
    /// </seealso>
    string? Password { set; }

    /// <summary>
    /// This property is not supported in this version.
    /// Specifies whether the Credential Security Service Provider (CredSSP) is enabled for this connection.
    /// This property is only supported by Remote Desktop Connection 6.1 and 7.0 clients.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings6-enablecredsspsupport</cref>
    /// </seealso>
    bool NetworkLevelAuthentication { get; set; }

    /// <summary>
    /// Indicates that the password contains a smart card personal identification number (PIN). Minimum required RDP Version 6 or higher.
    /// </summary>
    bool PasswordContainsSmartCardPin { get; set; }

    #endregion

    #region ::: Display Settings :::

    /// <summary>
    /// The color depth (in bits per pixel) for the control's connection.
    /// Values include 8, 15, 16, 24, and 32 bits per pixel.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclient-colordepth</cref>
    /// </seealso>
    int ColorDepth { get; set; }

    /// <summary>
    /// Specifies the current control's width, in pixels, on the initial remote desktop.
    /// Setting the DesktopWidth property is optional, but it must be set before calling the Connect method.
    /// If a desktop width is not specified, or is set to zero, the desktop width is set to the width of the control.
    /// The minimum and maximum values are dependent upon the operating system version of the Remote Desktop client.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-desktopwidth</cref>
    /// </seealso>
    int DesktopWidth { get; set; }

    /// <summary>
    /// Specifies the current control's height, in pixels, on the initial remote desktop.
    /// Setting the DesktopHeight property is optional, but it must be set before calling the Connect method.
    /// If a desktop height is not specified, or is set to zero, the desktop height is set to the height of the control.
    /// The minimum and maximum values are dependent upon the operating system version of the Remote Desktop client.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-desktopheight</cref>
    /// </seealso>
    int DesktopHeight { get; set; }

    /// <summary>
    /// Specifies if the display should be scaled to fit the client area of the control. Note that scroll bars do not appear when the SmartSizing property is enabled.
    /// Unlike most other properties, this property can be set when the control is connected.
    /// This property is read/write.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-smartsizing</cref>
    /// </seealso>
    bool SmartSizing { get; set; }

    ///  <summary>
    /// Determines whether the client control is in full-screen mode.
    ///  True to enter full-screen mode, False to leave full-screen mode and return to window mode.
    ///  Note: You can set this property when the control is connected.
    ///  </summary>
    ///  <seealso>
    ///      <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclient-fullscreen</cref>
    ///  </seealso>
    bool FullScreen { get; set; }

    /// <summary>
    /// Specifies the window title displayed when the control is in full-screen mode.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-fullscreentitle</cref>
    /// </seealso>
    string FullScreenTitle { set; }

    /// <summary>
    /// Specifies whether the container-handled full-screen mode is enabled.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-containerhandledfullscreen</cref>
    /// </seealso>
    int ContainerHandledFullScreen { get; set; }

    /// <summary>
    /// Specifies whether to use the connection bar. The default value is true, which enables the property.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-displayconnectionbar</cref>
    /// </seealso>
    bool DisplayConnectionBar { get; set; }

    /// <summary>
    /// Specifies the state of the UI connection bar.
    /// Setting this property to true sets the state to "lowered", that is, invisible to the user and unavailable for input.
    /// False sets the state to "raised" and available for user input.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-pinconnectionbar</cref>
    /// </seealso>
    bool PinConnectionBar { get; set; }

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
    bool UseMultimon { get; set; }

    /// <summary>
    /// Not supported in this version!
    /// Gets the number of remote monitors.
    /// Note: You can only access this property when the connection has been established.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientnonscriptable5-remotemonitorcount</cref>
    /// </seealso>
    uint RemoteMonitorCount { get; }

    /// <summary>
    /// A 32-bit, unsigned integer. The device scale factor of the monitor. This value MUST be ignored if it is not set to 100%, 140%, or 180% or desktopScaleFactor is less than 100% or greater than 500%.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-rdpbcgr/8fb3a83c-f3e2-4a81-8824-8173af03b6bc</cref>
    /// </seealso>
    uint DeviceScaleFactor { get; set; }

    /// <summary>
    /// A 32-bit, unsigned integer. The desktop scale factor of the monitor. This value MUST be ignored if it is less than 100% or greater than 500% or deviceScaleFactor is not 100%, 140% or 180%.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-rdpbcgr/8fb3a83c-f3e2-4a81-8824-8173af03b6bc</cref>
    /// </seealso>
    uint DesktopScaleFactor { get; set; }

    #endregion

    #region ::: Advanced :::

    /// <summary>
    /// Specifies the authentication level to use for the connection.
    /// The value of the AuthenticationLevel property.
    /// 0 = No authentication of the server.
    /// 1 = Server authentication is required and must complete successfully for the connection to proceed.
    /// 2 = Attempt authentication of the server.If authentication fails, the user will be prompted with the option to cancel the connection or to proceed without server authentication.
    /// </summary>
    AuthenticationLevel AuthenticationLevel { get; set; }

    /// <summary>
    /// Specifies whether compression is enabled.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-compress</cref>
    /// </seealso>
    bool Compression { get; set; }

    /// <summary>
    /// Specifies whether bitmap caching is enabled. Persistent caching can improve performance but requires additional disk space.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-bitmappersistence</cref>
    /// </seealso>
    bool BitmapCaching { get; set; }

    /// <summary>
    /// Sets or retrieves the configuration for public mode. Public mode prevents the client from caching user data to the local system.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings5-publicmode</cref>
    /// </seealso>
    bool PublicMode { get; set; }

    /// <summary>
    /// Specifies whether background input mode is enabled. When background input is enabled the client can accept input when the client does not have focus.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-allowbackgroundinput</cref>
    /// </seealso>
    bool AllowBackgroundInput { get; set; }

    /// <summary>
    /// Specifies whether to enable the client control to reconnect automatically to a session in the event of a network disconnection.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings2-enableautoreconnect</cref>
    /// </seealso>
    bool EnableAutoReconnect { get; set; }

    /// <summary>
    /// Specifies the number of times to try to reconnect during automatic reconnection. Valid values are 0 to 200.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings2-maxreconnectattempts</cref>
    /// </seealso>
    int MaxReconnectAttempts { get; set; }

    /// <summary>
    /// Retrieves or specifies whether the ActiveX control should attempt to connect to the server for administrative purposes.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings6-connecttoadministerserver</cref>
    /// </seealso>
    bool ConnectToAdministerServer { get; set; }

    /// <summary>
    /// Gets and sets whether to use the redirection server name.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdppreferredredirectioninfo-useredirectionservername</cref>
    /// </seealso>
    bool UseRedirectionServerName { get; set; }

    /// <summary>
    /// Specifies the load balancing cookie that will be placed in the X.224 Connection Request packet in the Remote Desktop Session Host (RD Session Host) server protocol connection sequence.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-loadbalanceinfo</cref>
    /// </seealso>
    string LoadBalanceInfo { get; set; }

    /// <summary>
    /// Specifies the names of virtual channel client DLLs to be loaded. Virtual channel client DLLs are also referred to as Plug-in DLLs.
    /// Comma-separated list of the names of the virtual channel client DLLs to be loaded. The DLL names must contain only alphanumeric characters.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-plugindlls</cref>
    /// </seealso>
    string PluginDlls { set; }

    /// <summary>
    /// Specifies if the client control should have the focus while connecting. The control will not attempt to grab focus from a window running in a different process.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-grabfocusonconnect</cref>
    /// </seealso>
    bool GrabFocusOnConnect { get; set; }

    /// <summary>
    /// Specifies whether the mouse should use relative mode. Contains VARIANT_TRUE if the mouse is in relative mode or VARIANT_FALSE if the mouse is in absolute mode.
    /// The mouse mode indicates how the ActiveX control calculates the mouse coordinates that it sends to the Remote Desktop Session Host (RD Session Host) server. When the mouse is in relative mode, the ActiveX control calculates mouse coordinates relative to the mouse's last position. When the mouse is in absolute mode, the ActiveX control calculates mouse coordinates relative to the desktop of the RD Session Host server.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings6-relativemousemode</cref>
    /// </seealso>
    bool RelativeMouseMode { get; set; }

    /// <summary>
    ///  If True, credentials are not sent to the remote server.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpextendedsettings-property</cref>
    /// </seealso>
    bool DisableCredentialsDelegation { get; set; }

    /// <summary>
    ///  If True, authentication redirection is enabled. This property and DisableCredentialsDelegation must be true for RemoteCredentialGuard.
    /// </summary>
    bool RedirectedAuthentication { get; set; }

    /// <summary>
    ///  If True, authentication redirection is enabled. This property and DisableCredentialsDelegation must be true for RemoteCredentialGuard.
    /// </summary>
    bool RestrictedLogon { get; set; }

    /// <summary>
    /// Remote Credential Guard helps you protect your credentials over a Remote Desktop connection by redirecting Kerberos requests back to the device that's requesting the connection. It also provides single sign-on experiences for Remote Desktop sessions. When set to true, RedirectedAuthentication and DisableCredentialsDelegation is set to true.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/security/identity-protection/remote-credential-guard</cref>
    /// </seealso>
    bool RemoteCredentialGuard { get; set; }

    /// <summary>
    /// Connects you to the remote PC or server in Restricted Administration mode. In this mode, credentials won't be sent to remote PC or server,
    /// which can protect you if you connect to a PC that has been compromised. However, connections made from the remote PC might not be authenticated by other PCs
    /// and servers, which might impact app functionality and compatibility.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/archive/blogs/kfalde/restricted-admin-mode-for-rdp-in-windows-8-1-2012-r2</cref>
    /// </seealso>
    bool RestrictedAdminMode { get; set; }

    #endregion

    #region ::: Performance :::

    /// <summary>
    /// Gets or sets the type of network connection used between the client and server. The network connection type information passed on to the server helps the server tune several parameters based on the network connection type
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings7-networkconnectiontype</cref>
    /// </see>
    uint NetworkConnectionType { get; set; }

    /// <summary>
    /// Specifies a set of features that can be set at the server to improve performance.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-performanceflags</cref>
    /// </see>
    internal int PerformanceFlags { get; set; }

    /// <summary>
    /// Specifies whether or not DirectX is enabled or not.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings7-redirectdirectx</cref>
    /// </see>
    bool RedirectDirectX { get; set; }

    /// <summary>
    /// Specifies if bandwidth changes are automatically detected.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings8-bandwidthdetection</cref>
    /// </see>
    bool BandwidthDetection { get; set; }

    /// <summary>
    ///  If set to true, hardware assist with graphics decoding is attempted.
    /// </summary>
    bool EnableHardwareMode { get; set; }

    /// <summary>
    /// Specifies the remote desktop protocol used between the client and the server.
    /// FullMode (0, full Windows 8 remote desktop protocol with max. memory footprint)
    /// ThinClientMode (1: limited to using the Win7 SP1 RemoteFX codec and smallest memory footprint)
    /// SmallCacheMode (2: the protocol is the same as full mode but with smaller cache size)
    /// </summary>
    ClientSpec ClientProtocolSpec { get; set; }

    #endregion

    #region ::: Redirection :::

    /// <summary>
    /// Specifies the audio redirection settings, which specify whether to redirect sounds or play sounds at the Remote Desktop Session Host (RD Session Host) server.
    /// 0 = Redirect sounds to the client.This is the default value.
    /// 1 = Play sounds at the remote computer.
    /// 2 = Disable sound redirection; do not play sounds at the server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientsecuredsettings-autoredirectionmode</cref>
    /// </see>
    AudioRedirectionMode AudioRedirectionMode { get; set; }

    /// <summary>
    /// Specifies or retrieves a Boolean value that indicates whether the default audio input device is redirected from the client to the remote session.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings7-audiocaptureredirectionmode</cref>
    /// </see>
    bool AudioCaptureRedirectionMode { get; set; }

    /// <summary>
    /// Specifies if redirection of printers is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-redirectprinters</cref>
    /// </see>
    bool RedirectPrinters { get; set; }

    /// <summary>
    /// Sets or retrieves the configuration for clipboard redirection.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings5-redirectclipboard</cref>
    /// </see>
    bool RedirectClipboard { get; set; }

    /// <summary>
    /// Specifies if redirection of smart cards is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-redirectsmartcards</cref>
    /// </see>
    bool RedirectSmartCards { get; set; }

    /// <summary>
    /// Specifies if redirection of local ports (for example, COM and LPT) is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-redirectports</cref>
    /// </see>
    bool RedirectPorts { get; set; }

    /// <summary>
    /// Sets or retrieves the configuration for device redirection.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings5-redirectdevices</cref>
    /// </see>
    bool RedirectDevices { get; set; }

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Sets or retrieves the configuration for Point of Service device redirection.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings5-redirectposdevices</cref>
    /// </see>
    bool RedirectPOSDevices { get; set; }

    /// <summary>
    /// Specifies if redirection of disk drives is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-redirectdrives</cref>
    /// </see>
    bool RedirectDrives { get; set; }

    /// <summary>
    /// A string representing the drive letters which should be redirected (e.g. 'CEH' for the drives C:, E: and H:)
    /// If empty, all drives are redirected when RedirectDrives property is true.
    /// </summary>
    string RedirectDriveLetters { get; set; }

    #endregion

    #region ::: Keyboard :::

    /// <summary>
    /// Specifies if keyboard accelerators should be passed to the server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-acceleratorpassthrough</cref>
    /// </see>
    bool AcceleratorPassthrough { get; set; }

    /// <summary>
    /// Specifies if the Windows key can be used in the remote session.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-enablewindowskey</cref>
    /// </see>
    bool EnableWindowsKey { get; set; }

    /// <summary>
    /// Specifies the keyboard redirection settings, which specify how and when to apply Windows keyboard shortcut (for example, ALT+TAB).
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientsecuredsettings-keyboardhookmode</cref>
    /// </see>
    int KeyboardHookMode { get; set; }

    /// <summary>
    /// Specifies the name of the active input locale identifier (formerly called the keyboard layout) to use for the connection.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-keyboardlayoutstr</cref>
    /// </see>
    string KeyBoardLayoutStr { set; }

    #endregion

    #region ::: Program :::

    /// <summary>
    /// Specifies the program to be started on the remote server upon connection.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscsecuredsettings-startprogram</cref>
    /// </see>
    string StartProgram { get; set; }

    /// <summary>
    /// Specifies the working directory of the start program.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscsecuredsettings-workdir</cref>
    /// </see>
    string WorkDir { get; set; }

    /// <summary>
    /// Specifies if programs launched with the StartProgram property should be maximized.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-maximizeshell</cref>
    /// </see>
    bool MaximizeShell { get; set; }

    #endregion

    #region ::: Gateway :::

    /// <summary>
    /// Specifies when to use a Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayusagemethod</cref>
    /// </see>
    GatewayUsageMethod GatewayUsageMethod { get; set; }

    /// <summary>
    /// Specifies whether to use default Remote Desktop Gateway (RD Gateway) settings.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayprofileusagemethod</cref>
    /// </see>
    GatewayProfileUsageMethod GatewayProfileUsageMethod { get; set; }

    /// <summary>
    /// Specifies or retrieves the Remote Desktop Gateway (RD Gateway) authentication method.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewaycredssource</cref>
    /// </see>
    GatewayCredentialSource GatewayCredsSource { get; set; }

    /// <summary>
    /// Sets or retrieves the user-specified Remote Desktop Gateway (RD Gateway) credential source.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayuserselectedcredssource</cref>
    /// </see>
    GatewayCredentialSource GatewayUserSelectedCredsSource { get; set; }

    /// <summary>
    /// Specifies or retrieves the setting for whether the Remote Desktop Gateway (RD Gateway) credential sharing feature is enabled. When the feature is enabled, the Remote Desktop ActiveX control tries to use the same credentials to authenticate to the Remote Desktop Session Host (RD Session Host) server and to the RD Gateway server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewaycredsharing</cref>
    /// </see>
    bool GatewayCredSharing { get; set; }

    /// <summary>
    /// Specifies the host name of the Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayhostname</cref>
    /// </see>
    string GatewayHostname { get; set; }

    /// <summary>
    /// Specifies or retrieves the user name that is provided to the Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewayusername</cref>
    /// </see>
    string GatewayUsername { get; set; }

    /// <summary>
    /// Specifies or retrieves the domain name of a user that is provided to the Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewaydomain</cref>
    /// </see>
    string GatewayDomain { get; set; }

    /// <summary>
    /// Specifies the password that a user provides to connect to the Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewaypassword</cref>
    /// </see>
    string GatewayPassword { set; }

    #endregion

    #region ::: HyperV :::

    /// <summary>
    /// Specifies the service principal name (SPN) to use for authentication to the server.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings6-authenticationserviceclass</cref>
    /// </seealso>
    string AuthenticationServiceClass { get; set; }

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Specifies the pre-connection BLOB (PCB) setting to use prior to connecting for transmission to the server.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings6-pcb</cref>
    /// </seealso>
    string PCB { get; set; }

    /// <summary>
    /// Specifies or retrieves whether the negotiation security layer is enabled for the connection.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientnonscriptable3-negotiatesecuritylayer</cref>
    /// </seealso>
    bool NegotiateSecurityLayer { get; set; }

    #endregion

    #region ::: Misc :::

    /// <summary>
    /// ILogger instance. If not set, a default debug logger is used during debug mode.
    /// </summary>
    ILogger Logger { get; set; }
    /// <inheritdoc cref="ContainsFocus"/>
    bool ContainsFocus { get; }
    /// <summary>
    /// If true, a click in the remote session does not focus the control and does not trigger the OnClientAreaClicked event.
    /// </summary>
    bool DisableClickDetection { get; set; }
    /// <inheritdoc cref="Handle"/>
    IntPtr Handle { get; }
    /// <inheritdoc cref="IsDisposed"/>
    bool IsDisposed { get; }
    /// <inheritdoc cref="IsHandleCreated"/>
    bool IsHandleCreated { get; }
    /// <inheritdoc cref="Focus"/>
    bool Focus();
    /// <summary>
    /// Raises the OnClientAreaClicked event.
    /// </summary>
    void RaiseClientAreaClicked();
   
    #endregion

    #region ::: MsRdpEx :::

    /// <inheritdoc cref="AxName"/>
    string AxName { get; set; }
    /// <inheritdoc cref="RdpExDll"/>
    string RdpExDll { get; set; }

    #endregion

    #endregion

    #region --- Public Methods ---

    /// <summary>
    /// Initiates a connection using the properties currently set on the control.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-connect</cref>
    /// </seealso>
    void Connect();

    /// <summary>
    /// Disconnects the active connection.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-disconnect</cref>
    /// </seealso>
    void Disconnect();

    /// <inheritdoc cref="Reconnect"/>
    ControlReconnectStatus Reconnect(uint width, uint height);

    /// <summary>
    /// Retrieves a reference to the underlying ActiveX control.
    /// </summary>
    /// <returns>The ActiveX control.</returns>
    object GetOcx();

    /// <summary>
    /// Specifies the bounding rectangle of the remote monitor.
    /// Can only be called after the connection has been established.
    /// </summary>
    /// <param name="left">Receives the left edge of the rectangle.</param>
    /// <param name="top">Receives the top edge of the rectangle.</param>
    /// <param name="right">Receives the right edge of the rectangle.</param>
    /// <param name="bottom">Receives the bottom edge of the rectangle.</param>
    /// <remarks>All coordinates are in virtual screen coordinates, which are relative to the upper-left corner of the primary monitor. If this is not the primary monitor, some or all of these values may be negative.</remarks>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientnonscriptable5-getremotemonitorsboundingbox</cref>
    /// </seealso>
    void GetRemoteMonitorsBoundingBox(out int left, out int top, out int right, out int bottom);

    /// <summary>
    /// Retrieves the error description for the session disconnect events.
    /// </summary>
    /// <param name="disconnectReasonCode">The disconnect code.</param>
    /// <returns>The error message text.</returns>
    string GetErrorDescription(int disconnectReasonCode);

    /// <summary>
    /// Writes a log entry that the specified feature is not supported.
    /// </summary>
    /// <param name="feature">The name of the unsupported feature.</param>
    void LogFeatureNotSupported(string feature);

    /// <summary>
    /// Causes an action to be performed in the remote session.
    /// </summary>
    /// <param name="action"></param>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclient8-sendremoteaction</cref>
    /// </seealso>
    void SendRemoteAction(RemoteSessionActionType action);

    /// <summary>
    /// Updates session display settings.
    /// </summary>
    /// <param name="desktopWidth">Desktop width.</param>
    /// <param name="desktopHeight">Desktop height.</param>
    /// <param name="physicalWidth">Physical width.</param>
    /// <param name="physicalHeight">Physical height.</param>
    /// <param name="orientation">Orientation</param>
    /// <param name="desktopScaleFactor">Desktop scale factor.</param>
    /// <param name="deviceScaleFactor">Device scale factor.</param>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/mt703457(v=vs.85)</cref>
    /// </seealso>
    void UpdateSessionDisplaySettings(uint desktopWidth, uint desktopHeight, uint physicalWidth, uint physicalHeight, uint orientation, uint desktopScaleFactor, uint deviceScaleFactor);

    #endregion
}