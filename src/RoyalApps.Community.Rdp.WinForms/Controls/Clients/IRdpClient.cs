using System;
using AxMSTSCLib;
using Microsoft.Extensions.Logging;
using MSTSCLib;
using RoyalApps.Community.Rdp.WinForms.Configuration.Connection;
using RoyalApps.Community.Rdp.WinForms.Configuration.Input;
using RoyalApps.Community.Rdp.WinForms.Configuration.Performance;
using IMsTscAxEvents_OnConfirmCloseEventHandler = AxMSTSCLib.IMsTscAxEvents_OnConfirmCloseEventHandler;
using IMsTscAxEvents_OnDisconnectedEventHandler = AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEventHandler;

namespace RoyalApps.Community.Rdp.WinForms.Controls.Clients;

/// <summary>
/// Provides safe, logged access to the properties, methods, and events exposed by the underlying RDP ActiveX control.
/// </summary>
/// <inheritdoc cref="AxMsRdpClient11NotSafeForScripting"/>
public interface IRdpClient : IDisposable
{
    /// <summary>
    /// Specifies whether keyboard accelerators should be passed to the server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-acceleratorpassthrough</cref>
    /// </see>
    bool AcceleratorPassthrough { get; set; }

    /// <summary>
    /// Specifies whether the client can accept input while it does not have focus.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-allowbackgroundinput</cref>
    /// </seealso>
    bool AllowBackgroundInput { get; set; }

    /// <summary>
    /// Specifies whether the default audio input device is redirected from the client to the remote session.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings7-audiocaptureredirectionmode</cref>
    /// </see>
    bool AudioCaptureRedirectionMode { get; set; }

    /// <summary>
    /// Specifies the audio quality mode.
    /// </summary>
    /// <see>
    ///     <cref>https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings7-audioqualitymode</cref>
    /// </see>
    AudioQualityMode AudioQualityMode { get; set; }

    /// <summary>
    /// Specifies how audio output is handled for the remote session.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientsecuredsettings-autoredirectionmode</cref>
    /// </see>
    AudioRedirectionMode AudioRedirectionMode { get; set; }

    /// <summary>
    /// Specifies the server authentication level used for the connection.
    /// </summary>
    AuthenticationLevel AuthenticationLevel { get; set; }

    /// <summary>
    /// Specifies the service principal name (SPN) to use for authentication to the server.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings6-authenticationserviceclass</cref>
    /// </seealso>
    string AuthenticationServiceClass { get; set; }

    /// <summary>
    /// Gets or sets the name of the underlying RDP ActiveX implementation, for example <c>mstsc</c> or <c>msrdc</c>.
    /// </summary>
    string AxName { get; set; }

    /// <summary>
    /// Specifies whether bandwidth changes are detected automatically.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings8-bandwidthdetection</cref>
    /// </see>
    bool BandwidthDetection { get; set; }

    /// <summary>
    /// Specifies whether bitmap caching is enabled. Persistent caching can improve performance but requires additional disk space.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-bitmappersistence</cref>
    /// </seealso>
    bool BitmapCaching { get; set; }

    /// <summary>
    /// Specifies the RDP protocol profile used between the client and the server.
    /// </summary>
    ClientSpec ClientProtocolSpec { get; set; }

    /// <summary>
    /// The color depth (in bits per pixel) for the control's connection.
    /// Values include 8, 15, 16, 24, and 32 bits per pixel.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclient-colordepth</cref>
    /// </seealso>
    int ColorDepth { get; set; }

    /// <summary>
    /// Specifies whether compression is enabled.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-compress</cref>
    /// </seealso>
    bool Compression { get; set; }

    /// <summary>
    /// Gets the current connection state of the control.
    /// </summary>
    ConnectionState ConnectionState { get; }

    /// <summary>
    /// Specifies whether the ActiveX control should attempt to connect to the server for administrative purposes.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings6-connecttoadministerserver</cref>
    /// </seealso>
    bool ConnectToAdministerServer { get; set; }

    /// <summary>
    /// Specifies whether the container-handled full-screen mode is enabled.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-containerhandledfullscreen</cref>
    /// </seealso>
    int ContainerHandledFullScreen { get; set; }

    /// <inheritdoc cref="ContainsFocus"/>
    bool ContainsFocus { get; }

    /// <summary>
    /// Specifies the initial remote desktop height, in pixels.
    /// This property must be set before connecting.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-desktopheight</cref>
    /// </seealso>
    int DesktopHeight { get; set; }

    /// <summary>
    /// Gets or sets the desktop scale factor for the monitor.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-rdpbcgr/8fb3a83c-f3e2-4a81-8824-8173af03b6bc</cref>
    /// </seealso>
    uint DesktopScaleFactor { get; set; }

    /// <summary>
    /// Specifies the initial remote desktop width, in pixels.
    /// This property must be set before connecting.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-desktopwidth</cref>
    /// </seealso>
    int DesktopWidth { get; set; }

    /// <summary>
    /// Gets or sets the device scale factor for the monitor.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-rdpbcgr/8fb3a83c-f3e2-4a81-8824-8173af03b6bc</cref>
    /// </seealso>
    uint DeviceScaleFactor { get; set; }

    /// <summary>
    /// When <see langword="true"/>, a click in the remote session does not focus the control and does not trigger the <see cref="OnClientAreaClicked"/> event.
    /// </summary>
    bool DisableClickDetection { get; set; }

    /// <summary>
    /// When <see langword="true"/>, reusable credentials are not delegated to the remote server.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpextendedsettings-property</cref>
    /// </seealso>
    bool DisableCredentialsDelegation { get; set; }

    /// <summary>
    /// Disables RDP UDP transport and forces TCP transport. This is commonly useful when connecting through UDP-based VPN links.
    /// </summary>
    bool DisableUdpTransport { get; set; }

    /// <summary>
    /// Specifies whether to use the connection bar. The default value is true, which enables the property.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-displayconnectionbar</cref>
    /// </seealso>
    bool DisplayConnectionBar { get; set; }

    /// <summary>
    /// Specifies the domain to which the current user logs on.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-domain</cref>
    /// </seealso>
    string? Domain { get; set; }

    /// <summary>
    /// Specifies whether the client control should reconnect automatically after a network interruption.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings2-enableautoreconnect</cref>
    /// </seealso>
    bool EnableAutoReconnect { get; set; }

    /// <summary>
    /// When <see langword="true"/>, hardware-assisted graphics decoding is attempted.
    /// </summary>
    bool EnableHardwareMode { get; set; }

    /// <summary>
    /// When <see langword="true"/>, the session is kept alive through periodic, non-intrusive input simulation to help avoid idle-time disconnects.
    /// </summary>
    bool EnableMouseJiggler { get; set; }

    /// <summary>
    /// Specifies whether Microsoft Entra ID is used to authenticate to the remote PC.
    /// </summary>
    bool EnableRdsAadAuth { get; set; }

    /// <summary>
    /// Specifies whether the Windows key can be used in the remote session.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-enablewindowskey</cref>
    /// </see>
    bool EnableWindowsKey { get; set; }

    ///  <summary>
    /// Specifies whether the client control is in full-screen mode.
    /// This property can be changed while connected.
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
    /// Specifies whether the same credentials should be reused for both the RD Gateway server and the target session host.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewaycredsharing</cref>
    /// </see>
    bool GatewayCredSharing { get; set; }

    /// <summary>
    /// Specifies the Remote Desktop Gateway (RD Gateway) authentication method.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewaycredssource</cref>
    /// </see>
    GatewayCredentialSource GatewayCredsSource { get; set; }

    /// <summary>
    /// Specifies the domain name used to authenticate to the Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewaydomain</cref>
    /// </see>
    string GatewayDomain { get; set; }

    /// <summary>
    /// Specifies the host name of the Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayhostname</cref>
    /// </see>
    string GatewayHostname { get; set; }

    /// <summary>
    /// Specifies the password used to authenticate to the Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewaypassword</cref>
    /// </see>
    string GatewayPassword { set; }

    /// <summary>
    /// Specifies whether to use default Remote Desktop Gateway (RD Gateway) settings.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayprofileusagemethod</cref>
    /// </see>
    GatewayProfileUsageMethod GatewayProfileUsageMethod { get; set; }

    /// <summary>
    /// Specifies when to use a Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayusagemethod</cref>
    /// </see>
    GatewayUsageMethod GatewayUsageMethod { get; set; }

    /// <summary>
    /// Specifies the user name used to authenticate to the Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewayusername</cref>
    /// </see>
    string GatewayUsername { get; set; }

    /// <summary>
    /// Specifies the user-selected Remote Desktop Gateway (RD Gateway) credential source.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayuserselectedcredssource</cref>
    /// </see>
    GatewayCredentialSource GatewayUserSelectedCredsSource { get; set; }

    /// <summary>
    /// Specifies whether the client control should have focus while connecting. The control does not attempt to grab focus from a window running in a different process.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-grabfocusonconnect</cref>
    /// </seealso>
    bool GrabFocusOnConnect { get; set; }

    /// <inheritdoc cref="Handle"/>
    IntPtr Handle { get; }

    /// <inheritdoc cref="IsDisposed"/>
    bool IsDisposed { get; }

    /// <inheritdoc cref="IsHandleCreated"/>
    bool IsHandleCreated { get; }

    /// <summary>
    /// Gets or sets the interval, in milliseconds, between keep-alive packets.
    /// </summary>
    /// <seealso>
    ///     <cref>https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-keepaliveinterval</cref>
    /// </seealso>
    int KeepAliveInterval { get; set; }

    /// <summary>
    /// Specifies how Windows keyboard shortcuts, for example ALT+TAB, are redirected to the remote session.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientsecuredsettings-keyboardhookmode</cref>
    /// </see>
    int KeyboardHookMode { get; set; }

    /// <summary>
    /// When enabled, ctrl+alt+space can be used to toggle between local and remote keyboard hook modes. Only works when the session is in full screen mode.
    /// </summary>
    bool KeyboardHookToggleShortcutEnabled { get; set; }

    /// <summary>
    /// Specifies the input locale identifier, formerly called the keyboard layout, used for the connection.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-keyboardlayoutstr</cref>
    /// </see>
    string KeyBoardLayoutStr { set; }

    /// <summary>
    /// Specifies the load-balancing cookie written into the initial X.224 connection request.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-loadbalanceinfo</cref>
    /// </seealso>
    string LoadBalanceInfo { get; set; }

    /// <summary>
    /// Gets or sets the logger used by the RDP client wrapper.
    /// </summary>
    ILogger Logger { get; set; }

    /// <summary>
    /// Specifies whether programs launched with <see cref="StartProgram"/> should be maximized.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-maximizeshell</cref>
    /// </see>
    bool MaximizeShell { get; set; }

    /// <summary>
    /// Specifies the number of times to try to reconnect during automatic reconnection. Valid values are 0 to 200.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings2-maxreconnectattempts</cref>
    /// </seealso>
    int MaxReconnectAttempts { get; set; }

    /// <summary>
    /// Gets or sets the interval, in seconds, between mouse-jiggler keep-alive actions.
    /// </summary>
    int MouseJigglerInterval { get; set; }

    /// <summary>
    /// Gets or sets the mouse-jiggler method. <c>MouseMove</c> simulates mouse input and <c>KeyboardInput</c> simulates an <c>F15</c> key press.
    /// </summary>
    KeepAliveMethod MouseJigglerMethod { get; set; }

    /// <summary>
    /// Specifies or retrieves whether the negotiation security layer is enabled for the connection.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientnonscriptable3-negotiatesecuritylayer</cref>
    /// </seealso>
    bool NegotiateSecurityLayer { get; set; }

    /// <summary>
    /// Gets or sets the network connection type reported to the server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings7-networkconnectiontype</cref>
    /// </see>
    uint NetworkConnectionType { get; set; }

    /// <summary>
    /// Specifies whether the Credential Security Support Provider (CredSSP) and network-level authentication are enabled for this connection.
    /// Availability depends on the underlying ActiveX client version.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings6-enablecredsspsupport</cref>
    /// </seealso>
    bool NetworkLevelAuthentication { get; set; }

    /// <summary>
    /// Sets the Remote Desktop ActiveX control password in plaintext form.
    /// After it is set, it cannot be retrieved in plaintext form.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscnonscriptable-cleartextpassword</cref>
    /// </seealso>
    string? Password { set; }

    /// <summary>
    /// Specifies whether the password contains a smart card PIN.
    /// </summary>
    bool PasswordContainsSmartCardPin { get; set; }

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Specifies the pre-connection BLOB (PCB) value sent to the server before connecting.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings6-pcb</cref>
    /// </seealso>
    string PCB { get; set; }

    /// <summary>
    /// Specifies a set of features that can be set at the server to improve performance.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-performanceflags</cref>
    /// </see>
    internal int PerformanceFlags { get; set; }

    /// <summary>
    /// Specifies whether the UI connection bar is pinned in the lowered state.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-pinconnectionbar</cref>
    /// </seealso>
    bool PinConnectionBar { get; set; }

    /// <summary>
    /// Specifies the comma-separated names of virtual channel plug-in DLLs to load.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-plugindlls</cref>
    /// </seealso>
    string PluginDlls { set; }

    /// <summary>
    /// Specifies the connection port. The default value is 3389.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-rdpport</cref>
    /// </seealso>
    int Port { get; set; }


    /// <summary>
    /// Specifies whether public mode is enabled. Public mode prevents the client from caching user data locally.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings5-publicmode</cref>
    /// </seealso>
    bool PublicMode { get; set; }

    /// <summary>
    /// Gets or sets the path to the MsRdpEx hook library used by the underlying ActiveX client.
    /// </summary>
    string RdpExDll { get; set; }

    /// <summary>
    /// Specifies whether camera redirection is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpclientnonscriptable7-cameraredirconfigcollection</cref>
    /// </see>
    bool RedirectCameras { get; set; }

    /// <summary>
    /// Specifies whether clipboard redirection is enabled.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings5-redirectclipboard</cref>
    /// </see>
    bool RedirectClipboard { get; set; }

    /// <summary>
    /// Specifies whether general device redirection is enabled.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings5-redirectdevices</cref>
    /// </see>
    bool RedirectDevices { get; set; }

    /// <summary>
    /// Specifies whether DirectX redirection is enabled.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings7-redirectdirectx</cref>
    /// </see>
    bool RedirectDirectX { get; set; }

    /// <summary>
    /// Specifies the drive letters that should be redirected, for example <c>CEH</c> for drives C:, E:, and H:.
    /// If empty, all drives are redirected when <see cref="RedirectDrives"/> is enabled.
    /// </summary>
    string RedirectDriveLetters { get; set; }

    /// <summary>
    /// Specifies whether drive redirection is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-redirectdrives</cref>
    /// </see>
    bool RedirectDrives { get; set; }

    /// <summary>
    /// When <see langword="true"/>, authentication is redirected back to the local device. This property and <see cref="DisableCredentialsDelegation"/> must both be enabled for <see cref="RemoteCredentialGuard"/>.
    /// </summary>
    bool RedirectedAuthentication { get; set; }

    /// <summary>
    /// Specifies whether location redirection is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpextendedsettings-property#property-value</cref>
    /// </see>
    bool RedirectLocation { get; set; }

    /// <summary>
    /// Specifies whether local port redirection, for example COM and LPT, is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-redirectports</cref>
    /// </see>
    bool RedirectPorts { get; set; }

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Specifies whether Point of Service device redirection is enabled.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings5-redirectposdevices</cref>
    /// </see>
    bool RedirectPOSDevices { get; set; }

    /// <summary>
    /// Specifies whether printer redirection is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-redirectprinters</cref>
    /// </see>
    bool RedirectPrinters { get; set; }

    /// <summary>
    /// Specifies whether smart card redirection is allowed.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-redirectsmartcards</cref>
    /// </see>
    bool RedirectSmartCards { get; set; }

    /// <summary>
    /// Specifies whether the mouse should use relative mode instead of absolute mode.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings6-relativemousemode</cref>
    /// </seealso>
    bool RelativeMouseMode { get; set; }

    /// <summary>
    /// Remote Credential Guard helps protect credentials over a Remote Desktop connection by redirecting Kerberos requests back to the device that initiated the connection. It also provides single sign-on for Remote Desktop sessions. When enabled, <see cref="RedirectedAuthentication"/> and <see cref="DisableCredentialsDelegation"/> are set to <see langword="true"/>.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/security/identity-protection/remote-credential-guard</cref>
    /// </seealso>
    bool RemoteCredentialGuard { get; set; }

    /// <summary>
    /// Gets the number of monitors in the active remote session.
    /// Availability depends on the underlying ActiveX client version, and the property can only be read after the connection has been established.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientnonscriptable5-remotemonitorcount</cref>
    /// </seealso>
    uint RemoteMonitorCount { get; }

    /// <summary>
    /// Specifies whether Restricted Administration mode is enabled.
    /// In this mode, credentials are not sent to the remote PC or server.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/archive/blogs/kfalde/restricted-admin-mode-for-rdp-in-windows-8-1-2012-r2</cref>
    /// </seealso>
    bool RestrictedAdminMode { get; set; }

    /// <summary>
    /// When <see langword="true"/>, restricted logon mode is enabled. This is one of the building blocks used by restricted administration scenarios together with <see cref="DisableCredentialsDelegation"/>.
    /// </summary>
    bool RestrictedLogon { get; set; }

    /// <summary>
    /// Specifies the target server name or IP address.
    /// This property must be set before connecting.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-server</cref>
    /// </seealso>
    string Server { get; set; }

    /// <summary>
    /// Shows the Connection Information dialog.
    /// </summary>
    bool ShowConnectionInformation { get; set; }

    /// <summary>
    /// Specifies whether the display should be scaled to fit the client area of the control.
    /// This property can be changed while connected.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-smartsizing</cref>
    /// </seealso>
    bool SmartSizing { get; set; }

    /// <summary>
    /// Specifies the program to be started on the remote server upon connection.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscsecuredsettings-startprogram</cref>
    /// </see>
    string StartProgram { get; set; }

    /// <summary>
    /// Specifies whether the Remote Desktop ActiveX control should use multiple monitors.
    /// Availability depends on the underlying ActiveX client version.
    /// This property must be set before the connection is established.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientnonscriptable5-usemultimon</cref>
    /// </seealso>
    bool UseMultimon { get; set; }

    /// <summary>
    /// Specifies whether the redirected server name should be used when the broker redirects the connection.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdppreferredredirectioninfo-useredirectionservername</cref>
    /// </seealso>
    bool UseRedirectionServerName { get; set; }

    /// <summary>
    /// Specifies the user name logon credential.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-username</cref>
    /// </seealso>
    string? UserName { get; set; }

    /// <summary>
    /// Specifies whether video decoding and rendering are redirected to the client.
    /// </summary>
    /// <see>
    ///     <cref>https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings7-videoplaybackmode</cref>
    /// </see>
    VideoPlaybackMode VideoPlaybackMode { get; set; }

    /// <summary>
    /// Specifies the working directory of the start program.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscsecuredsettings-workdir</cref>
    /// </see>
    string WorkDir { get; set; }

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

    /// <inheritdoc cref="Focus"/>
    bool Focus();

    /// <summary>
    /// Retrieves the error description for the session disconnect events.
    /// </summary>
    /// <param name="disconnectReasonCode">The disconnect code.</param>
    /// <returns>The error message text.</returns>
    string GetErrorDescription(int disconnectReasonCode);

    /// <summary>
    /// Retrieves a reference to the underlying ActiveX control.
    /// </summary>
    /// <returns>The ActiveX control.</returns>
    object? GetOcx();

    /// <summary>
    /// Retrieves the bounding rectangle of the remote monitor layout.
    /// This method can only be called after the connection has been established.
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
    /// Raised when the user clicks inside the remote desktop session and <see cref="DisableClickDetection"/> is disabled.
    /// </summary>
    event EventHandler OnClientAreaClicked;

    /// <inheritdoc cref="OnConfirmClose"/>
    event IMsTscAxEvents_OnConfirmCloseEventHandler OnConfirmClose;

    /// <inheritdoc cref="OnConnected"/>
    event EventHandler OnConnected;

    /// <inheritdoc cref="OnDisconnected"/>
    event IMsTscAxEvents_OnDisconnectedEventHandler OnDisconnected;

    /// <inheritdoc cref="OnRequestContainerMinimize"/>
    event EventHandler OnRequestContainerMinimize;

    /// <inheritdoc cref="OnRequestLeaveFullScreen"/>
    event EventHandler OnRequestLeaveFullScreen;

    /// <summary>
    /// Raises the OnClientAreaClicked event.
    /// </summary>
    void RaiseClientAreaClicked();

    /// <inheritdoc cref="Reconnect"/>
    ControlReconnectStatus Reconnect(uint width, uint height);

    /// <summary>
    /// Performs an action in the remote session.
    /// </summary>
    /// <param name="action">The remote session action to perform.</param>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclient8-sendremoteaction</cref>
    /// </seealso>
    void SendRemoteAction(RemoteSessionActionType action);

    /// <summary>
    /// Updates the remote session display settings.
    /// </summary>
    /// <param name="desktopWidth">Desktop width.</param>
    /// <param name="desktopHeight">Desktop height.</param>
    /// <param name="physicalWidth">Physical width.</param>
    /// <param name="physicalHeight">Physical height.</param>
    /// <param name="orientation">The display orientation.</param>
    /// <param name="desktopScaleFactor">Desktop scale factor.</param>
    /// <param name="deviceScaleFactor">Device scale factor.</param>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/mt703457(v=vs.85)</cref>
    /// </seealso>
    void UpdateSessionDisplaySettings(uint desktopWidth, uint desktopHeight, uint physicalWidth, uint physicalHeight, uint orientation, uint desktopScaleFactor, uint deviceScaleFactor);
}
