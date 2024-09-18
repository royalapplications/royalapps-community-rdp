using System;
using System.Windows.Forms;
using AxMSTSCLib;
using Microsoft.Extensions.Logging;
using MSTSCLib;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Interfaces;
using RoyalApps.Community.Rdp.WinForms.Logging;
using System.ComponentModel;

// ReSharper disable IdentifierTypo
// ReSharper disable ValueParameterNotUsed
// ReSharper disable HeapView.BoxingAllocation
// ReSharper disable InconsistentNaming

namespace RoyalApps.Community.Rdp.WinForms.Controls;

/// <inheritdoc cref="IRdpClient"/>
public class RdpClient2 : AxMsRdpClientNotSafeForScripting, IRdpClient
{
    #region --- Events ---
    
    /// <inheritdoc cref="OnClientAreaClicked"/>
    public event EventHandler? OnClientAreaClicked;

    #endregion
    
    #region --- Rdp Settings ---

    #region ::: General :::

    // public string Server { get; set; }

    /// <inheritdoc cref="Port"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Port
    {
        get => AdvancedSettings2.RDPPort;
        set => AdvancedSettings2.RDPPort = value;
    }

    /// <inheritdoc cref="ConnectionState"/>
    public ConnectionState ConnectionState => (ConnectionState)Connected;

    #endregion

    #region ::: Credentials :::

    // public string UserName { get; set; }

    // public string Domain { get; set; }

    /// <inheritdoc cref="Password"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Password
    {
        set => AdvancedSettings2.ClearTextPassword = value;
    }


    /// <summary>
    /// This client does not support this property. 
    /// </summary>
    /// <inheritdoc cref="NetworkLevelAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NetworkLevelAuthentication
    {
        get
        {
            LogFeatureNotSupported(nameof(NetworkLevelAuthentication));
            return false;
        }
        set => LogFeatureNotSupported(nameof(NetworkLevelAuthentication));
    }

    /// <inheritdoc cref="PasswordContainsSmartCardPin"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PasswordContainsSmartCardPin
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.PasswordContainsSmartcardPin, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.PasswordContainsSmartcardPin);
            return value;
        }
        set
        {
            object passwordContainsSCardPin = value;
            if (!this.TrySetProperty(RdpClientExtensions.PasswordContainsSmartcardPin, ref passwordContainsSCardPin, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.PasswordContainsSmartcardPin, passwordContainsSCardPin);
        }
    }

    #endregion

    #region ::: Display Settings :::

    // public int ColorDepth { get; set; }

    // public long DesktopWidth { get; set; }

    // public long DesktopHeight { get; set; }

    /// <inheritdoc cref="SmartSizing"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SmartSizing
    {
        get => AdvancedSettings2.SmartSizing;
        set => AdvancedSettings2.SmartSizing = value;
    }

    // public bool FullScreen { get; set; }

    // public string FullScreenTitle { set; }

    /// <inheritdoc cref="ContainerHandledFullScreen"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ContainerHandledFullScreen
    {
        get => AdvancedSettings2.ContainerHandledFullScreen;
        set => AdvancedSettings2.ContainerHandledFullScreen = value;
    }

    /// <inheritdoc cref="DisplayConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisplayConnectionBar { get => AdvancedSettings2.DisplayConnectionBar; set => AdvancedSettings2.DisplayConnectionBar = value; }

    /// <inheritdoc cref="PinConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PinConnectionBar { get => AdvancedSettings2.PinConnectionBar; set => AdvancedSettings2.PinConnectionBar = value; }

    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="UseMultimon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseMultimon
    {
        get
        {
            LogFeatureNotSupported(nameof(UseMultimon));
            return false;
        }
        set => LogFeatureNotSupported(nameof(UseMultimon));
    }

    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="UseMultimon"/>
    public uint RemoteMonitorCount
    {
        get
        {
            LogFeatureNotSupported(nameof(RemoteMonitorCount));
            return 1;
        }
    }

    /// <inheritdoc cref="DeviceScaleFactor"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint DeviceScaleFactor
    {
        get
        {
            if (!this.TryGetProperty<uint>(RdpClientExtensions.DeviceScaleFactor, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DeviceScaleFactor);
            return value;
        } 
        set
        {
            object deviceScaleFactor = value;
            if (!this.TrySetProperty(RdpClientExtensions.DeviceScaleFactor, ref deviceScaleFactor, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DeviceScaleFactor, deviceScaleFactor);
        }
    }

    /// <inheritdoc cref="DesktopScaleFactor"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint DesktopScaleFactor
    {
        get
        {
            if (!this.TryGetProperty<uint>(RdpClientExtensions.DesktopScaleFactor, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DesktopScaleFactor);
            return value;
        }
        set
        {
            object desktopScaleFactor = value;
            if (!this.TrySetProperty(RdpClientExtensions.DesktopScaleFactor, ref desktopScaleFactor, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DesktopScaleFactor, desktopScaleFactor);
        }
    }

    #endregion

    #region ::: Advanced :::

    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="AuthenticationLevel"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AuthenticationLevel AuthenticationLevel
    {
        get => AuthenticationLevel.NoAuthenticationOfServer;
        set => LogFeatureNotSupported(nameof(AuthenticationLevel));
    }

    /// <inheritdoc cref="Compression"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Compression
    {
        get => AdvancedSettings2.Compress > 0;
        set => AdvancedSettings2.Compress = value ? 1 : 0;
    }

    /// <inheritdoc cref="BitmapCaching"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BitmapCaching
    {
        get => AdvancedSettings2.BitmapPersistence > 0;
        set
        {
            AdvancedSettings2.BitmapPersistence = value ? 1 : 0;
            AdvancedSettings2.BitmapPeristence = value ? 1 : 0;
        }
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="PublicMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PublicMode
    {
        get => false;
        set => LogFeatureNotSupported(nameof(PublicMode));
    }

    /// <inheritdoc cref="AllowBackgroundInput"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AllowBackgroundInput
    {
        get => AdvancedSettings2.allowBackgroundInput > 0;
        set => AdvancedSettings2.allowBackgroundInput = value ? 1 : 0;
    }

    /// <inheritdoc cref="DisableUdpTransport"/>>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableUdpTransport
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.DisableUdpTransport, out var value, out var ex))
                Logger.LogWarning(ex,"Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DisableUdpTransport);
            return value;
        }
        set
        {
            object disableUdpTransport = value;
            if (!this.TrySetProperty(RdpClientExtensions.DisableUdpTransport, ref disableUdpTransport, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DisableUdpTransport, disableUdpTransport);
        }
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="EnableAutoReconnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableAutoReconnect
    {
        get => false;
        set => LogFeatureNotSupported(nameof(EnableAutoReconnect));
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="MaxReconnectAttempts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MaxReconnectAttempts
    {
        get => 0;
        set => LogFeatureNotSupported(nameof(MaxReconnectAttempts));
    }

    /// <inheritdoc cref="ConnectToAdministerServer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ConnectToAdministerServer
    {
        get => AdvancedSettings2.ConnectToServerConsole;
        set => AdvancedSettings2.ConnectToServerConsole = value;
    }

    /// <inheritdoc cref="UseRedirectionServerName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseRedirectionServerName
    {
        get
        {
            try { return this.GetPreferredRedirectionInfo().UseRedirectionServerName; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "UseRedirectionServerName"); }
            return false;
        }
        set
        {
            try { this.GetPreferredRedirectionInfo().UseRedirectionServerName = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "UseRedirectionServerName", value); }
        }
    }

    /// <inheritdoc cref="KeepAliveInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int KeepAliveInterval
    {
        get => AdvancedSettings2.keepAliveInterval;
        set => AdvancedSettings2.keepAliveInterval = value;
    }

    /// <inheritdoc cref="LoadBalanceInfo"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string LoadBalanceInfo
    {
        get => AdvancedSettings2.LoadBalanceInfo;
        set => RdpClientExtensions.SetLoadBalanceInfo(value, AdvancedSettings2);
    }

    /// <inheritdoc cref="PluginDlls"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PluginDlls { set => AdvancedSettings2.PluginDlls = value; }

    /// <inheritdoc cref="GrabFocusOnConnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GrabFocusOnConnect
    {
        get => AdvancedSettings2.GrabFocusOnConnect;
        set => AdvancedSettings2.GrabFocusOnConnect = value;
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="RelativeMouseMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RelativeMouseMode
    {
        get
        {
            LogFeatureNotSupported(nameof(RelativeMouseMode));
            return false;
        }

        set => LogFeatureNotSupported(nameof(RelativeMouseMode));
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="DisableCredentialsDelegation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableCredentialsDelegation
    {
        get => false;
        set => LogFeatureNotSupported(nameof(DisableCredentialsDelegation));
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="RedirectedAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectedAuthentication
    {
        get => false;
        set => LogFeatureNotSupported(nameof(RedirectedAuthentication));
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="RestrictedLogon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedLogon
    {
        get => false;
        set => LogFeatureNotSupported(nameof(RestrictedLogon));
    }


    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="RemoteCredentialGuard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RemoteCredentialGuard
    {
        get => false;
        set => LogFeatureNotSupported(nameof(RemoteCredentialGuard));
    }        

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="RestrictedAdminMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedAdminMode
    {
        get => false;
        set => LogFeatureNotSupported(nameof(RestrictedAdminMode));
    }

    /// <inheritdoc cref="EnableMouseJiggler"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableMouseJiggler
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableMouseJiggler, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableMouseJiggler);
            return value;
        }
        set
        {
            object enableMouseJiggler = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableMouseJiggler, ref enableMouseJiggler, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableMouseJiggler, enableMouseJiggler);
        }
    }

    /// <inheritdoc cref="MouseJigglerInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MouseJigglerInterval
    {
        get
        {
            if (!this.TryGetProperty<int>(RdpClientExtensions.MouseJigglerInterval, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.MouseJigglerInterval);
            return value;
        }
        set
        {
            object mouseJigglerInterval = value;
            if (!this.TrySetProperty(RdpClientExtensions.MouseJigglerInterval, ref mouseJigglerInterval, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.MouseJigglerInterval, mouseJigglerInterval);
        }
    }

    /// <inheritdoc cref="MouseJigglerMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public KeepAliveMethod MouseJigglerMethod
    {
        get
        {
            if (!this.TryGetProperty<int>(RdpClientExtensions.MouseJigglerMethod, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.MouseJigglerMethod);
            return (KeepAliveMethod)value;
        }
        set
        {
            object mouseJigglerMethod = (int)value;
            if (!this.TrySetProperty(RdpClientExtensions.MouseJigglerMethod, ref mouseJigglerMethod, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.MouseJigglerMethod, mouseJigglerMethod);
        }
    }

    /// <inheritdoc cref="EnableRdsAadAuth"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableRdsAadAuth
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableRdsAadAuth, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableRdsAadAuth);
            return value;
        }
        set
        {
            object enableRdsAadAuth = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableRdsAadAuth, ref enableRdsAadAuth, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableRdsAadAuth, enableRdsAadAuth);
        }
    }

    #endregion

    #region ::: Performance :::

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="NetworkConnectionType"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint NetworkConnectionType
    {
        get => 0;
        set => LogFeatureNotSupported(nameof(NetworkConnectionType));
    }

    /// <inheritdoc cref="PerformanceFlags"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int PerformanceFlags
    {
        get => AdvancedSettings2.PerformanceFlags;
        set => AdvancedSettings2.PerformanceFlags = value;
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="RedirectDirectX"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDirectX
    {
        get => false;
        set => LogFeatureNotSupported(nameof(RedirectDirectX));
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="BandwidthDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BandwidthDetection
    {
        get => false;
        set => LogFeatureNotSupported(nameof(BandwidthDetection));
    }

    /// <inheritdoc cref="EnableHardwareMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableHardwareMode
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableHardwareMode, out var value, out var ex))
                Logger.LogWarning(ex,"Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableHardwareMode);
            return value;
        }
        set
        {
            object enableHardwareMode = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableHardwareMode, ref enableHardwareMode, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableHardwareMode, enableHardwareMode);
        }
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="ClientProtocolSpec"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClientSpec ClientProtocolSpec
    {
        get => ClientSpec.FullMode;
        set => LogFeatureNotSupported(nameof(ClientProtocolSpec));
    }

    #endregion

    #region ::: Redirection :::

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="AudioRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioRedirectionMode AudioRedirectionMode
    {
        get => AudioRedirectionMode.RedirectToClient;
        set => LogFeatureNotSupported(nameof(AudioRedirectionMode));
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioQualityMode AudioQualityMode
    {
        get => AudioQualityMode.Dynamic;
        set => LogFeatureNotSupported(nameof(AudioQualityMode));
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="AudioCaptureRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AudioCaptureRedirectionMode
    {
        get => false;
        set => LogFeatureNotSupported(nameof(AudioCaptureRedirectionMode));
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VideoPlaybackMode VideoPlaybackMode
    {
        get => VideoPlaybackMode.DecodeAndRenderOnServer;
        set => LogFeatureNotSupported(nameof(VideoPlaybackMode));
    }

    /// <inheritdoc cref="RedirectPrinters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPrinters
    {
        get => AdvancedSettings2.RedirectPrinters;
        set => AdvancedSettings2.RedirectPrinters = value;
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="RedirectClipboard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectClipboard
    {
        get => false;
        set => LogFeatureNotSupported(nameof(RedirectClipboard));
    }

    /// <inheritdoc cref="RedirectSmartCards"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectSmartCards
    {
        get => AdvancedSettings2.RedirectSmartCards;
        set => AdvancedSettings2.RedirectSmartCards = value;
    }
    /// <inheritdoc cref="RedirectPorts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPorts
    {
        get => AdvancedSettings2.RedirectPorts;
        set => AdvancedSettings2.RedirectPorts = value;
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="RedirectDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDevices
    {
        get => false;
        set => LogFeatureNotSupported(nameof(RedirectDevices));
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="RedirectPOSDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPOSDevices
    {
        get => false;
        set => LogFeatureNotSupported(nameof(RedirectPOSDevices));
    }

    /// <inheritdoc cref="RedirectDrives"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDrives
    {
        get => AdvancedSettings2.RedirectDrives;
        set => AdvancedSettings2.RedirectDrives = value;
    }

    private string _redirectDriveLetters = string.Empty;
    /// <inheritdoc cref="RedirectDriveLetters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RedirectDriveLetters
    {
        get => _redirectDriveLetters;
        set
        {
            _redirectDriveLetters = value;
            if (RedirectDrives && !this.SetupDriveRedirection(_redirectDriveLetters, out var ex))
            {
                Logger.LogWarning(ex, "One or more errors occurred during drive redirection");
            }
        }
    }

    /// <inheritdoc cref="RedirectCameras"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectCameras 
    {
        get => false;
        set => LogFeatureNotSupported(nameof(RedirectCameras));
    }

    /// <inheritdoc cref="RedirectLocation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectLocation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableLocationRedirection, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableLocationRedirection);
            return value;
        }
        set
        {
            object enableLocationRedirect = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableLocationRedirection, ref enableLocationRedirect, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableLocationRedirection, enableLocationRedirect);
        }
    }

    #endregion

    #region ::: Keyboard :::

    /// <inheritdoc cref="AcceleratorPassthrough"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AcceleratorPassthrough
    {
        get => AdvancedSettings2.AcceleratorPassthrough != 0;
        set => AdvancedSettings2.AcceleratorPassthrough = value ? 1 : 0;
    }

    /// <inheritdoc cref="EnableWindowsKey"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableWindowsKey
    {
        get => AdvancedSettings2.EnableWindowsKey != 0;
        set => AdvancedSettings2.EnableWindowsKey = value ? 1 : 0;
    }

    /// <inheritdoc cref="KeyboardHookMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int KeyboardHookMode
    {
        get => SecuredSettings2.KeyboardHookMode;
        set => SecuredSettings2.KeyboardHookMode = value;
    }

    /// <inheritdoc cref="KeyBoardLayoutStr"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string KeyBoardLayoutStr
    {
        set => AdvancedSettings2.KeyBoardLayoutStr = value;
    }

    #endregion

    #region ::: Program :::

    /// <inheritdoc cref="StartProgram"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string StartProgram
    {
        get => SecuredSettings2.StartProgram;
        set => SecuredSettings2.StartProgram = value;
    }

    /// <inheritdoc cref="WorkDir"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string WorkDir
    {
        get => SecuredSettings2.WorkDir;
        set => SecuredSettings2.WorkDir = value;
    }

    /// <inheritdoc cref="MaximizeShell"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool MaximizeShell
    {
        get => AdvancedSettings2.MaximizeShell != 0;
        set => AdvancedSettings2.MaximizeShell = value ? 1 : 0;
    }

    #endregion

    #region ::: Gateway :::

    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="GatewayUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayUsageMethod GatewayUsageMethod
    {
        get => GatewayUsageMethod.Never;
        set => LogFeatureNotSupported(nameof(GatewayUsageMethod));
    }
    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="GatewayProfileUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayProfileUsageMethod GatewayProfileUsageMethod
    {
        get => GatewayProfileUsageMethod.Default;
        set => LogFeatureNotSupported(nameof(GatewayProfileUsageMethod));
    }
    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="GatewayCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayCredsSource
    {
        get => GatewayCredentialSource.Any;
        set => LogFeatureNotSupported(nameof(GatewayCredsSource));
    }
    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="GatewayUserSelectedCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayUserSelectedCredsSource
    {
        get => GatewayCredentialSource.Any;
        set => LogFeatureNotSupported(nameof(GatewayUserSelectedCredsSource));
    }
    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="GatewayCredSharing"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GatewayCredSharing
    {
        get => false;
        set => LogFeatureNotSupported(nameof(GatewayCredSharing));
    }
    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="GatewayHostname"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayHostname
    {
        get => string.Empty;
        set => LogFeatureNotSupported(nameof(GatewayHostname));
    }
    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="GatewayUsername"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayUsername
    {
        get => string.Empty;
        set => LogFeatureNotSupported(nameof(GatewayUsername));
    }
    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="GatewayDomain"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayDomain
    {
        get => string.Empty;
        set => LogFeatureNotSupported(nameof(GatewayDomain));
    }
    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="GatewayPassword"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayPassword
    {
        set => LogFeatureNotSupported(nameof(GatewayPassword));
    }

    #endregion

    #region ::: HyperV :::

    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="AuthenticationServiceClass"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AuthenticationServiceClass
    {
        get => string.Empty;
        set => LogFeatureNotSupported(nameof(AuthenticationServiceClass));
    }

    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="PCB"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PCB
    {
        get => string.Empty;
        set => LogFeatureNotSupported(nameof(PCB));
    }

    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="NegotiateSecurityLayer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NegotiateSecurityLayer
    {
        get => false;
        set => LogFeatureNotSupported(nameof(NegotiateSecurityLayer));
    }

    /// <inheritdoc cref="ShowConnectionInformation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowConnectionInformation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.ShowConnectionInformation, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.ShowConnectionInformation);
            return value;
        }
        set
        {
            object showConnectionInformation = value;
            if (!this.TrySetProperty(RdpClientExtensions.ShowConnectionInformation, ref showConnectionInformation, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.ShowConnectionInformation, showConnectionInformation);
        }
    }

    #endregion

    #region ::: Misc :::

    /// <inheritdoc cref="Logger"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ILogger Logger { get; set; } = DebugLoggerFactory.Create();

    /// <inheritdoc cref="DisableClickDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableClickDetection { get; set; }

    /// <inheritdoc cref="RaiseClientAreaClicked"/>
    public void RaiseClientAreaClicked() => OnClientAreaClicked?.Invoke(this, EventArgs.Empty);

    #endregion

    #region ::: MsRdpEx :::

    /// <inheritdoc cref="AxName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AxName
    {
        get => axName;
        set => axName = value;
    }

    /// <inheritdoc cref="RdpExDll"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RdpExDll
    {
        get => rdpExDll;
        set => rdpExDll = value;
    }

    #endregion

    #endregion

    #region --- Public Methods ---

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="Reconnect"/>
    public ControlReconnectStatus Reconnect(uint width, uint height)
    {
        LogFeatureNotSupported("Reconnect");
        return ControlReconnectStatus.controlReconnectBlocked;
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="GetRemoteMonitorsBoundingBox"/>
    public void GetRemoteMonitorsBoundingBox(out int left, out int top, out int right, out int bottom)
    {
        LogFeatureNotSupported("GetRemoteMonitorsBoundingBox");
        left = 0;
        top = 0;
        right = 0;
        bottom = 0;
    }

    /// <inheritdoc cref="GetErrorDescription"/>
    public string GetErrorDescription(int disconnectReasonCode) => $"Disconnected with reason code {disconnectReasonCode}. See https://docs.microsoft.com/en-us/windows/desktop/termserv/imstscaxevents-ondisconnected.";

    /// <inheritdoc cref="LogFeatureNotSupported"/>
    public void LogFeatureNotSupported(string feature) => Logger.LogDebug("The feature '{Feature}' is not supported in Rdp Client V2", feature);

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="SendRemoteAction"/>
    public void SendRemoteAction(RemoteSessionActionType action)
    {
        LogFeatureNotSupported("SendRemoteAction");
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="UpdateSessionDisplaySettings"/>
    public void UpdateSessionDisplaySettings(uint desktopWidth, uint desktopHeight, uint physicalWidth, uint physicalHeight, uint orientation, uint desktopScaleFactor, uint deviceScaleFactor)
    {
        LogFeatureNotSupported("UpdateSessionDisplaySettings");
    }

    #endregion

    #region --- Message Processing ---

    /// <inheritdoc cref="WndProc"/>
    protected override void WndProc(ref Message m)
    {
        if (MessageFilter.Filter(this, ref m))
            return;
        base.WndProc(ref m);
    }

    #endregion
}

/// <inheritdoc cref="IRdpClient"/>
public class RdpClient7 : AxMsRdpClient6NotSafeForScripting, IRdpClient
{
    #region --- Events ---
    
    /// <inheritdoc cref="OnClientAreaClicked"/>
    public event EventHandler? OnClientAreaClicked;

    #endregion

    #region --- Rdp Settings ---

    #region ::: General :::

    // public string Server { get; set; }

    /// <inheritdoc cref="Port"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Port { get => AdvancedSettings7.RDPPort; set => AdvancedSettings7.RDPPort = value; }

    /// <inheritdoc cref="ConnectionState"/>
    public ConnectionState ConnectionState => (ConnectionState)Connected;

    #endregion

    #region ::: Credentials :::

    // public string UserName { get; set; }

    // public string Domain { get; set; }

    /// <inheritdoc cref="Password"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Password { set => AdvancedSettings7.ClearTextPassword = value; }

    /// <inheritdoc cref="NetworkLevelAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NetworkLevelAuthentication { get => AdvancedSettings7.EnableCredSspSupport; set => AdvancedSettings7.EnableCredSspSupport = value; }

    /// <inheritdoc cref="PasswordContainsSmartCardPin"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PasswordContainsSmartCardPin
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.PasswordContainsSmartcardPin, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.PasswordContainsSmartcardPin);
            return value;
        }
        set
        {
            object passwordContainsSCardPin = value;
            if (!this.TrySetProperty(RdpClientExtensions.PasswordContainsSmartcardPin, ref passwordContainsSCardPin, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.PasswordContainsSmartcardPin, passwordContainsSCardPin);
        }
    }

    #endregion

    #region ::: Display Settings :::

    // public int ColorDepth { get; set; }

    // public int DesktopWidth { get; set; }

    // public int DesktopHeight { get; set; }

    /// <inheritdoc cref="SmartSizing"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SmartSizing { get => AdvancedSettings7.SmartSizing; set => AdvancedSettings7.SmartSizing = value; }

    // public bool FullScreen { get; set; }

    /// <inheritdoc cref="ContainerHandledFullScreen"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ContainerHandledFullScreen
    {
        get => AdvancedSettings7.ContainerHandledFullScreen;
        set => AdvancedSettings7.ContainerHandledFullScreen = value;
    }

    /// <inheritdoc cref="DisplayConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisplayConnectionBar { get => AdvancedSettings7.DisplayConnectionBar; set => AdvancedSettings7.DisplayConnectionBar = value; }

    /// <inheritdoc cref="PinConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PinConnectionBar { get => AdvancedSettings7.PinConnectionBar; set => AdvancedSettings7.PinConnectionBar = value; }

    /// <inheritdoc cref="UseMultimon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseMultimon
    {
        get
        {
            try { return this.GetNonScriptable5().UseMultimon; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "UseMultimon"); }
            return false;
        }
        set
        {
            try { this.GetNonScriptable5().UseMultimon = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "UseMultimon", value); }
        }
    }

    /// <inheritdoc cref="RemoteMonitorCount"/>
    public uint RemoteMonitorCount
    {
        get
        {
            try { return this.GetNonScriptable5().RemoteMonitorCount; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "RemoteMonitorCount"); }
            return 1;
        }
    }

    /// <inheritdoc cref="DeviceScaleFactor"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint DeviceScaleFactor
    {
        get
        {
            if (!this.TryGetProperty<uint>(RdpClientExtensions.DeviceScaleFactor, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DeviceScaleFactor);
            return value;
        }
        set
        {
            object deviceScaleFactor = value;
            if (!this.TrySetProperty(RdpClientExtensions.DeviceScaleFactor, ref deviceScaleFactor, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DeviceScaleFactor, deviceScaleFactor);
        }
    }

    /// <inheritdoc cref="DesktopScaleFactor"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint DesktopScaleFactor
    {
        get
        {
            if (!this.TryGetProperty<uint>(RdpClientExtensions.DesktopScaleFactor, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DesktopScaleFactor);
            return value;
        }
        set
        {
            object desktopScaleFactor = value;
            if (!this.TrySetProperty(RdpClientExtensions.DesktopScaleFactor, ref desktopScaleFactor, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DesktopScaleFactor, desktopScaleFactor);
        }
    }

    #endregion

    #region ::: Advanced :::

    /// <inheritdoc cref="AuthenticationLevel"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AuthenticationLevel AuthenticationLevel
    {
        get => (AuthenticationLevel)AdvancedSettings7.AuthenticationLevel;
        set => AdvancedSettings7.AuthenticationLevel = (uint)value;
    }

    /// <inheritdoc cref="Compression"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Compression
    {
        get => AdvancedSettings7.Compress > 0;
        set => AdvancedSettings7.Compress = value ? 1 : 0;
    }

    /// <inheritdoc cref="BitmapCaching"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BitmapCaching
    {
        get => AdvancedSettings7.BitmapPersistence > 0;
        set
        {
            AdvancedSettings7.BitmapPersistence = value ? 1 : 0;
            AdvancedSettings7.BitmapPeristence = value ? 1 : 0;
        }
    }

    /// <inheritdoc cref="PublicMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PublicMode
    {
        get => AdvancedSettings7.PublicMode;
        set => AdvancedSettings7.PublicMode = value;
    }

    /// <inheritdoc cref="AllowBackgroundInput"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AllowBackgroundInput
    {
        get => AdvancedSettings7.allowBackgroundInput > 0;
        set => AdvancedSettings7.allowBackgroundInput = value ? 1 : 0;
    }

    /// <inheritdoc cref="DisableUdpTransport"/>>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableUdpTransport
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.DisableUdpTransport, out var value, out var ex))
                Logger.LogWarning(ex,"Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DisableUdpTransport);
            return value;
        }
        set
        {
            object disableUdpTransport = value;
            if (!this.TrySetProperty(RdpClientExtensions.DisableUdpTransport, ref disableUdpTransport, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DisableUdpTransport, disableUdpTransport);
        }
    }

    /// <inheritdoc cref="EnableAutoReconnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableAutoReconnect
    {
        get => AdvancedSettings7.EnableAutoReconnect;
        set => AdvancedSettings7.EnableAutoReconnect = value;
    }

    /// <inheritdoc cref="MaxReconnectAttempts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MaxReconnectAttempts
    {
        get => AdvancedSettings7.MaxReconnectAttempts;
        set => AdvancedSettings7.MaxReconnectAttempts = value;
    }

    /// <inheritdoc cref="ConnectToAdministerServer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ConnectToAdministerServer
    {
        get => AdvancedSettings7.ConnectToAdministerServer;
        set => AdvancedSettings7.ConnectToAdministerServer = value;
    }

    /// <inheritdoc cref="UseRedirectionServerName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseRedirectionServerName
    {
        get
        {
            try { return this.GetPreferredRedirectionInfo().UseRedirectionServerName; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "UseRedirectionServerName"); }
            return false;
        }
        set
        {
            try { this.GetPreferredRedirectionInfo().UseRedirectionServerName = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "UseRedirectionServerName", value); }
        }
    }
    
    /// <inheritdoc cref="KeepAliveInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int KeepAliveInterval
    {
        get => AdvancedSettings2.keepAliveInterval;
        set => AdvancedSettings2.keepAliveInterval = value;
    }

    /// <inheritdoc cref="LoadBalanceInfo"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string LoadBalanceInfo
    {
        get => AdvancedSettings7.LoadBalanceInfo;
        set => RdpClientExtensions.SetLoadBalanceInfo(value, AdvancedSettings7);
    }

    /// <inheritdoc cref="PluginDlls"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PluginDlls { set => AdvancedSettings7.PluginDlls = value; }

    /// <inheritdoc cref="GrabFocusOnConnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GrabFocusOnConnect
    {
        get => AdvancedSettings7.GrabFocusOnConnect;
        set => AdvancedSettings7.GrabFocusOnConnect = value;
    }

    /// <inheritdoc cref="RelativeMouseMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RelativeMouseMode { get => AdvancedSettings7.RelativeMouseMode; set => AdvancedSettings7.RelativeMouseMode = value; }

    /// <inheritdoc cref="DisableCredentialsDelegation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableCredentialsDelegation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.DisableCredentialsDelegation, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DisableCredentialsDelegation);
            return value;
        }
        set
        {
            object disableCredentialsDelegation = value;
            if (!this.TrySetProperty(RdpClientExtensions.DisableCredentialsDelegation, ref disableCredentialsDelegation, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DisableCredentialsDelegation, disableCredentialsDelegation);
        }
    }

    /// <inheritdoc cref="RedirectedAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectedAuthentication
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.RedirectedAuthentication, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.RedirectedAuthentication);
            return value;
        }
        set
        {
            object redirectedAuthentication = value;
            if (!this.TrySetProperty(RdpClientExtensions.RedirectedAuthentication, ref redirectedAuthentication, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.RedirectedAuthentication, redirectedAuthentication);
        }
    }

    /// <inheritdoc cref="RestrictedLogon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedLogon
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.RestrictedLogon, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.RestrictedLogon);
            return value;
        }
        set
        {
            object restrictedLogon = value;
            if (!this.TrySetProperty(RdpClientExtensions.RestrictedLogon, ref restrictedLogon, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.RestrictedLogon, restrictedLogon);
        }
    }


    /// <inheritdoc cref="RemoteCredentialGuard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RemoteCredentialGuard
    {
        get => DisableCredentialsDelegation && RedirectedAuthentication;
        set
        {
            RedirectedAuthentication = value;
            DisableCredentialsDelegation = value;
        }
    }

    /// <inheritdoc cref="RestrictedAdminMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedAdminMode
    {
        get => DisableCredentialsDelegation && RestrictedLogon;
        set
        {
            RestrictedLogon = value;
            DisableCredentialsDelegation = value;
        }
    }

    /// <inheritdoc cref="EnableMouseJiggler"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableMouseJiggler
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableMouseJiggler, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableMouseJiggler);
            return value;
        }
        set
        {
            object enableMouseJiggler = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableMouseJiggler, ref enableMouseJiggler, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableMouseJiggler, enableMouseJiggler);
        }
    }

    /// <inheritdoc cref="MouseJigglerInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MouseJigglerInterval
    {
        get
        {
            if (!this.TryGetProperty<int>(RdpClientExtensions.MouseJigglerInterval, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.MouseJigglerInterval);
            return value;
        }
        set
        {
            object mouseJigglerInterval = value;
            if (!this.TrySetProperty(RdpClientExtensions.MouseJigglerInterval, ref mouseJigglerInterval, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.MouseJigglerInterval, mouseJigglerInterval);
        }
    }

    /// <inheritdoc cref="MouseJigglerMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public KeepAliveMethod MouseJigglerMethod
    {
        get
        {
            if (!this.TryGetProperty<int>(RdpClientExtensions.MouseJigglerMethod, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.MouseJigglerMethod);
            return (KeepAliveMethod)value;
        }
        set
        {
            object mouseJigglerMethod = (int)value;
            if (!this.TrySetProperty(RdpClientExtensions.MouseJigglerMethod, ref mouseJigglerMethod, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.MouseJigglerMethod, mouseJigglerMethod);
        }
    }

    /// <inheritdoc cref="EnableRdsAadAuth"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableRdsAadAuth
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableRdsAadAuth, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableRdsAadAuth);
            return value;
        }
        set
        {
            object enableRdsAadAuth = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableRdsAadAuth, ref enableRdsAadAuth, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableRdsAadAuth, enableRdsAadAuth);
        }
    }

    #endregion

    #region ::: Performance :::

    /// <inheritdoc cref="NetworkConnectionType"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint NetworkConnectionType
    {
        get => 0;
        set => LogFeatureNotSupported(nameof(NetworkConnectionType));
    }

    /// <inheritdoc cref="PerformanceFlags"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int PerformanceFlags
    {
        get => AdvancedSettings7.PerformanceFlags;
        set => AdvancedSettings7.PerformanceFlags = value;
    }
    /// <inheritdoc cref="RedirectDirectX"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDirectX
    {
        get => false;
        set => LogFeatureNotSupported(nameof(RedirectDirectX));
    }

    /// <inheritdoc cref="BandwidthDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BandwidthDetection
    {
        get => false;
        set => LogFeatureNotSupported(nameof(BandwidthDetection));
    }

    /// <inheritdoc cref="EnableHardwareMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableHardwareMode
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableHardwareMode, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableHardwareMode);
            return value;
        }
        set
        {
            object enableHardwareMode = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableHardwareMode, ref enableHardwareMode, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableHardwareMode, enableHardwareMode);
        }
    }

    /// <inheritdoc cref="ClientProtocolSpec"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClientSpec ClientProtocolSpec
    {
        get => ClientSpec.FullMode;
        set => LogFeatureNotSupported(nameof(ClientProtocolSpec));
    }

    #endregion

    #region ::: Redirection :::

    /// <inheritdoc cref="AudioRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioRedirectionMode AudioRedirectionMode
    {
        get => (AudioRedirectionMode)SecuredSettings2.AudioRedirectionMode;
        set => SecuredSettings2.AudioRedirectionMode = (int)value;
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioQualityMode AudioQualityMode
    {
        get => AudioQualityMode.Dynamic;
        set => LogFeatureNotSupported(nameof(AudioQualityMode));
    }

    /// <inheritdoc cref="AudioCaptureRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AudioCaptureRedirectionMode
    {
        get => false;
        set => LogFeatureNotSupported(nameof(AudioCaptureRedirectionMode));
    }
    
    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VideoPlaybackMode VideoPlaybackMode
    {
        get => VideoPlaybackMode.DecodeAndRenderOnServer;
        set => LogFeatureNotSupported(nameof(VideoPlaybackMode));
    }

    /// <inheritdoc cref="RedirectPrinters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPrinters
    {
        get => AdvancedSettings7.RedirectPrinters;
        set => AdvancedSettings7.RedirectPrinters = value;
    }

    /// <inheritdoc cref="RedirectClipboard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectClipboard
    {
        get => AdvancedSettings7.RedirectClipboard;
        set => AdvancedSettings7.RedirectClipboard = value;
    }

    /// <inheritdoc cref="RedirectSmartCards"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectSmartCards
    {
        get => AdvancedSettings7.RedirectSmartCards;
        set => AdvancedSettings7.RedirectSmartCards = value;
    }
    /// <inheritdoc cref="RedirectPorts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPorts
    {
        get => AdvancedSettings7.RedirectPorts;
        set => AdvancedSettings7.RedirectPorts = value;
    }

    /// <inheritdoc cref="RedirectDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDevices
    {
        get => AdvancedSettings7.RedirectDevices;
        set => AdvancedSettings7.RedirectDevices = value;
    }

    /// <inheritdoc cref="RedirectPOSDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPOSDevices
    {
        get => AdvancedSettings7.RedirectPOSDevices;
        set => AdvancedSettings7.RedirectPOSDevices = value;
    }

    /// <inheritdoc cref="RedirectDrives"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDrives
    {
        get => AdvancedSettings7.RedirectDrives;
        set => AdvancedSettings7.RedirectDrives = value;
    }

    private string _redirectDriveLetters = string.Empty;
    /// <inheritdoc cref="RedirectDriveLetters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RedirectDriveLetters
    {
        get => _redirectDriveLetters;
        set
        {
            _redirectDriveLetters = value;
            if (RedirectDrives && !this.SetupDriveRedirection(_redirectDriveLetters, out var ex))
            {
                Logger.LogWarning(ex, "One or more errors occurred during drive redirection");
            }
        }
    }

    /// <inheritdoc cref="RedirectCameras"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectCameras 
    {
        get => false;
        set => LogFeatureNotSupported(nameof(RedirectCameras));
    }

    /// <inheritdoc cref="RedirectLocation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectLocation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableLocationRedirection, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableLocationRedirection);
            return value;
        }
        set
        {
            object enableLocationRedirect = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableLocationRedirection, ref enableLocationRedirect, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableLocationRedirection, enableLocationRedirect);
        }
    }

    #endregion

    #region ::: Keyboard :::

    /// <inheritdoc cref="AcceleratorPassthrough"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AcceleratorPassthrough
    {
        get => AdvancedSettings7.AcceleratorPassthrough != 0;
        set => AdvancedSettings7.AcceleratorPassthrough = value ? 1 : 0;
    }

    /// <inheritdoc cref="EnableWindowsKey"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableWindowsKey
    {
        get => AdvancedSettings7.EnableWindowsKey != 0;
        set => AdvancedSettings7.EnableWindowsKey = value ? 1 : 0;
    }

    /// <inheritdoc cref="KeyboardHookMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int KeyboardHookMode
    {
        get => SecuredSettings2.KeyboardHookMode;
        set => SecuredSettings2.KeyboardHookMode = value;
    }

    /// <inheritdoc cref="KeyBoardLayoutStr"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string KeyBoardLayoutStr
    {
        set => AdvancedSettings7.KeyBoardLayoutStr = value;
    }

    #endregion

    #region ::: Program :::

    /// <inheritdoc cref="StartProgram"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string StartProgram
    {
        get => SecuredSettings2.StartProgram;
        set => SecuredSettings2.StartProgram = value;
    }

    /// <inheritdoc cref="WorkDir"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string WorkDir
    {
        get => SecuredSettings2.WorkDir;
        set => SecuredSettings2.WorkDir = value;
    }

    /// <inheritdoc cref="MaximizeShell"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool MaximizeShell
    {
        get => AdvancedSettings7.MaximizeShell != 0;
        set => AdvancedSettings7.MaximizeShell = value ? 1 : 0;
    }

    #endregion

    #region ::: Gateway :::

    /// <inheritdoc cref="GatewayUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayUsageMethod GatewayUsageMethod
    {
        get => (GatewayUsageMethod)TransportSettings2.GatewayUsageMethod;
        set => TransportSettings2.GatewayUsageMethod = (uint)value;
    }
    /// <inheritdoc cref="GatewayProfileUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayProfileUsageMethod GatewayProfileUsageMethod
    {
        get => (GatewayProfileUsageMethod)TransportSettings2.GatewayProfileUsageMethod;
        set => TransportSettings2.GatewayProfileUsageMethod = (uint)value;
    }
    /// <inheritdoc cref="GatewayCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayCredsSource
    {
        get => (GatewayCredentialSource)TransportSettings2.GatewayCredsSource;
        set => TransportSettings2.GatewayCredsSource = (uint)value;
    }
    /// <inheritdoc cref="GatewayUserSelectedCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayUserSelectedCredsSource
    {
        get => (GatewayCredentialSource)TransportSettings2.GatewayUserSelectedCredsSource;
        set => TransportSettings2.GatewayUserSelectedCredsSource = (uint)value;
    }
    /// <inheritdoc cref="GatewayCredSharing"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GatewayCredSharing
    {
        get => TransportSettings2.GatewayCredSharing != 0;
        set => TransportSettings2.GatewayCredSharing = value ? 1U : 0U;
    }
    /// <inheritdoc cref="GatewayHostname"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayHostname
    {
        get => TransportSettings2.GatewayHostname;
        set => TransportSettings2.GatewayHostname = value;
    }
    /// <inheritdoc cref="GatewayUsername"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayUsername
    {
        get => TransportSettings2.GatewayUsername;
        set => TransportSettings2.GatewayUsername = value;
    }
    /// <inheritdoc cref="GatewayDomain"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayDomain
    {
        get => TransportSettings2.GatewayDomain;
        set => TransportSettings2.GatewayDomain = value;
    }
    /// <inheritdoc cref="GatewayPassword"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayPassword
    {
        set => TransportSettings2.GatewayPassword = value;
    }

    #endregion

    #region ::: HyperV :::

    /// <inheritdoc cref="AuthenticationServiceClass"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AuthenticationServiceClass
    {
        get => AdvancedSettings7.AuthenticationServiceClass;
        set => AdvancedSettings7.AuthenticationServiceClass = value;
    }

    /// <inheritdoc cref="PCB"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PCB
    {
        get => AdvancedSettings7.PCB;
        set => AdvancedSettings7.PCB = value;
    }

    /// <inheritdoc cref="NegotiateSecurityLayer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NegotiateSecurityLayer
    {
        get
        {
            try { return this.GetNonScriptable4().NegotiateSecurityLayer; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}","NegotiateSecurityLayer"); }
            return false;
        }
        set
        {
            try { this.GetNonScriptable4().NegotiateSecurityLayer = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "NegotiateSecurityLayer", value); }
        }
    }

    /// <inheritdoc cref="ShowConnectionInformation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowConnectionInformation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.ShowConnectionInformation, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.ShowConnectionInformation);
            return value;
        }
        set
        {
            object showConnectionInformation = value;
            if (!this.TrySetProperty(RdpClientExtensions.ShowConnectionInformation, ref showConnectionInformation, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.ShowConnectionInformation, showConnectionInformation);
        }
    }

    #endregion

    #region ::: Misc :::

    /// <inheritdoc cref="Logger"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ILogger Logger { get; set; } = DebugLoggerFactory.Create();

    /// <inheritdoc cref="DisableClickDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableClickDetection { get; set; }

    /// <inheritdoc cref="RaiseClientAreaClicked"/>
    public void RaiseClientAreaClicked()
    {
        OnClientAreaClicked?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region ::: MsRdpEx :::

    /// <inheritdoc cref="AxName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AxName
    {
        get => axName;
        set => axName = value;
    }

    /// <inheritdoc cref="RdpExDll"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RdpExDll
    {
        get => rdpExDll;
        set => rdpExDll = value;
    }

    #endregion

    #endregion

    #region --- Public Methods ---

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="Reconnect"/>
    public ControlReconnectStatus Reconnect(uint width, uint height)
    {
        LogFeatureNotSupported("Reconnect");
        return ControlReconnectStatus.controlReconnectBlocked;
    }

    /// <inheritdoc cref="GetRemoteMonitorsBoundingBox"/>
    public void GetRemoteMonitorsBoundingBox(out int left, out int top, out int right, out int bottom)
    {
        try
        {
            this.GetNonScriptable5().GetRemoteMonitorsBoundingBox(out left, out top, out right, out bottom);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to call RDP client method: {MethodName}", "GetRemoteMonitorsBoundingBox");
            left = 0;
            top = 0;
            right = 0;
            bottom = 0;
        }
    }

    /// <inheritdoc cref="GetErrorDescription"/>
    public string GetErrorDescription(int disconnectReasonCode) => GetErrorDescription((uint)disconnectReasonCode, (uint)ExtendedDisconnectReason);

    /// <inheritdoc cref="LogFeatureNotSupported"/>
    public void LogFeatureNotSupported(string feature) => Logger.LogDebug("The feature '{Feature}' is not supported in Rdp Client V7", feature);

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="SendRemoteAction"/>
    public void SendRemoteAction(RemoteSessionActionType action)
    {
        LogFeatureNotSupported("SendRemoteAction");
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="UpdateSessionDisplaySettings"/>
    public void UpdateSessionDisplaySettings(uint desktopWidth, uint desktopHeight, uint physicalWidth, uint physicalHeight, uint orientation, uint desktopScaleFactor, uint deviceScaleFactor)
    {
        LogFeatureNotSupported("UpdateSessionDisplaySettings");
    }

    #endregion

    #region --- Message Processing ---

    /// <inheritdoc cref="WndProc"/>
    protected override void WndProc(ref Message m)
    {
        if (MessageFilter.Filter(this, ref m))
            return;
        base.WndProc(ref m);
    }

    #endregion
}

/// <inheritdoc cref="IRdpClient"/>
public class RdpClient8 : AxMsRdpClient7NotSafeForScripting, IRdpClient
{
    #region --- Events ---
    
    /// <inheritdoc cref="OnClientAreaClicked"/>
    public event EventHandler? OnClientAreaClicked;

    #endregion

    #region --- Rdp Settings ---

    #region ::: General :::

    // public string Server { get; set; }

    /// <inheritdoc cref="Port"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Port { get => AdvancedSettings8.RDPPort; set => AdvancedSettings8.RDPPort = value; }

    /// <inheritdoc cref="ConnectionState"/>
    public ConnectionState ConnectionState => (ConnectionState)Connected;

    #endregion

    #region ::: Credentials :::

    // public string UserName { get; set; }

    // public string Domain { get; set; }

    /// <inheritdoc cref="Password"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Password { set => AdvancedSettings8.ClearTextPassword = value; }

    /// <inheritdoc cref="NetworkLevelAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NetworkLevelAuthentication { get => AdvancedSettings8.EnableCredSspSupport; set => AdvancedSettings8.EnableCredSspSupport = value; }

    /// <inheritdoc cref="PasswordContainsSmartCardPin"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PasswordContainsSmartCardPin
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.PasswordContainsSmartcardPin, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.PasswordContainsSmartcardPin);
            return value;
        }
        set
        {
            object passwordContainsSCardPin = value;
            if (!this.TrySetProperty(RdpClientExtensions.PasswordContainsSmartcardPin, ref passwordContainsSCardPin, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.PasswordContainsSmartcardPin, passwordContainsSCardPin);
        }
    }

    #endregion

    #region ::: Display Settings :::

    // public int ColorDepth { get; set; }

    // public int DesktopWidth { get; set; }

    // public int DesktopHeight { get; set; }

    /// <inheritdoc cref="SmartSizing"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SmartSizing { get => AdvancedSettings8.SmartSizing; set => AdvancedSettings8.SmartSizing = value; }

    // public bool FullScreen { get; set; }

    // public string FullScreenTitle { set; }

    /// <inheritdoc cref="ContainerHandledFullScreen"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ContainerHandledFullScreen
    {
        get => AdvancedSettings8.ContainerHandledFullScreen;
        set => AdvancedSettings8.ContainerHandledFullScreen = value;
    }

    /// <inheritdoc cref="DisplayConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisplayConnectionBar { get => AdvancedSettings8.DisplayConnectionBar; set => AdvancedSettings8.DisplayConnectionBar = value; }

    /// <inheritdoc cref="PinConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PinConnectionBar { get => AdvancedSettings8.PinConnectionBar; set => AdvancedSettings8.PinConnectionBar = value; }

    /// <inheritdoc cref="UseMultimon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseMultimon
    {
        get
        {
            try { return this.GetNonScriptable5().UseMultimon; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "UseMultimon"); }
            return false;
        }
        set
        {
            try { this.GetNonScriptable5().UseMultimon = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "UseMultimon", value); }
        }
    }

    /// <inheritdoc cref="RemoteMonitorCount"/>
    public uint RemoteMonitorCount
    {
        get
        {
            try { return this.GetNonScriptable5().RemoteMonitorCount; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "RemoteMonitorCount"); }
            return 1;
        }
    }

    #endregion

    #region ::: Advanced :::

    /// <inheritdoc cref="AuthenticationLevel"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AuthenticationLevel AuthenticationLevel
    {
        get => (AuthenticationLevel)AdvancedSettings8.AuthenticationLevel;
        set => AdvancedSettings8.AuthenticationLevel = (uint)value;
    }

    /// <inheritdoc cref="Compression"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Compression
    {
        get => AdvancedSettings8.Compress > 0;
        set => AdvancedSettings8.Compress = value ? 1 : 0;
    }

    /// <inheritdoc cref="BitmapCaching"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BitmapCaching
    {
        get => AdvancedSettings8.BitmapPersistence > 0;
        set
        {
            AdvancedSettings8.BitmapPersistence = value ? 1 : 0;
            AdvancedSettings8.BitmapPeristence = value ? 1 : 0;
        }
    }

    /// <inheritdoc cref="PublicMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PublicMode
    {
        get => AdvancedSettings8.PublicMode;
        set => AdvancedSettings8.PublicMode = value;
    }

    /// <inheritdoc cref="AllowBackgroundInput"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AllowBackgroundInput
    {
        get => AdvancedSettings8.allowBackgroundInput > 0;
        set => AdvancedSettings8.allowBackgroundInput = value ? 1 : 0;
    }

    /// <inheritdoc cref="DisableUdpTransport"/>>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableUdpTransport
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.DisableUdpTransport, out var value, out var ex))
                Logger.LogWarning(ex,"Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DisableUdpTransport);
            return value;
        }
        set
        {
            object disableUdpTransport = value;
            if (!this.TrySetProperty(RdpClientExtensions.DisableUdpTransport, ref disableUdpTransport, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DisableUdpTransport, disableUdpTransport);
        }
    }

    /// <inheritdoc cref="EnableAutoReconnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableAutoReconnect
    {
        get => AdvancedSettings8.EnableAutoReconnect;
        set => AdvancedSettings8.EnableAutoReconnect = value;
    }

    /// <inheritdoc cref="MaxReconnectAttempts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MaxReconnectAttempts
    {
        get => AdvancedSettings8.MaxReconnectAttempts;
        set => AdvancedSettings8.MaxReconnectAttempts = value;
    }

    /// <inheritdoc cref="ConnectToAdministerServer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ConnectToAdministerServer
    {
        get => AdvancedSettings8.ConnectToAdministerServer;
        set => AdvancedSettings8.ConnectToAdministerServer = value;
    }

    /// <inheritdoc cref="UseRedirectionServerName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseRedirectionServerName
    {
        get
        {
            try { return this.GetPreferredRedirectionInfo().UseRedirectionServerName; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "UseRedirectionServerName"); }
            return false;
        }
        set
        {
            try { this.GetPreferredRedirectionInfo().UseRedirectionServerName = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "UseRedirectionServerName", value); }
        }
    }

    /// <inheritdoc cref="KeepAliveInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int KeepAliveInterval
    {
        get => AdvancedSettings2.keepAliveInterval;
        set => AdvancedSettings2.keepAliveInterval = value;
    }

    /// <inheritdoc cref="LoadBalanceInfo"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string LoadBalanceInfo
    {
        get => AdvancedSettings8.LoadBalanceInfo;
        set => RdpClientExtensions.SetLoadBalanceInfo(value, AdvancedSettings8);
    }

    /// <inheritdoc cref="GrabFocusOnConnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GrabFocusOnConnect
    {
        get => AdvancedSettings8.GrabFocusOnConnect;
        set => AdvancedSettings8.GrabFocusOnConnect = value;
    }

    /// <inheritdoc cref="PluginDlls"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PluginDlls { set => AdvancedSettings8.PluginDlls = value; }

    /// <inheritdoc cref="RelativeMouseMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RelativeMouseMode { get => AdvancedSettings8.RelativeMouseMode; set => AdvancedSettings8.RelativeMouseMode = value; }

    /// <inheritdoc cref="DisableCredentialsDelegation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableCredentialsDelegation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.DisableCredentialsDelegation, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DisableCredentialsDelegation);
            return value;
        }
        set
        {
            object disableCredentialsDelegation = value;
            if (!this.TrySetProperty(RdpClientExtensions.DisableCredentialsDelegation, ref disableCredentialsDelegation, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DisableCredentialsDelegation, disableCredentialsDelegation);
        }
    }

    /// <inheritdoc cref="RedirectedAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectedAuthentication
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.RedirectedAuthentication, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.RedirectedAuthentication);
            return value;
        }
        set
        {
            object redirectedAuthentication = value;
            if (!this.TrySetProperty(RdpClientExtensions.RedirectedAuthentication, ref redirectedAuthentication, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.RedirectedAuthentication, redirectedAuthentication);
        }
    }

    /// <inheritdoc cref="RestrictedLogon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedLogon
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.RestrictedLogon, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.RestrictedLogon);
            return value;
        }
        set
        {
            object restrictedLogon = value;
            if (!this.TrySetProperty(RdpClientExtensions.RestrictedLogon, ref restrictedLogon, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.RestrictedLogon, restrictedLogon);
        }
    }


    /// <inheritdoc cref="RemoteCredentialGuard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RemoteCredentialGuard
    {
        get => DisableCredentialsDelegation && RedirectedAuthentication;
        set
        {
            RedirectedAuthentication = value;
            DisableCredentialsDelegation = value;
        }
    }

    /// <inheritdoc cref="RestrictedAdminMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedAdminMode
    {
        get => DisableCredentialsDelegation && RestrictedLogon;
        set
        {
            RestrictedLogon = value;
            DisableCredentialsDelegation = value;
        }
    }

    /// <inheritdoc cref="EnableMouseJiggler"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableMouseJiggler
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableMouseJiggler, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableMouseJiggler);
            return value;
        }
        set
        {
            object enableMouseJiggler = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableMouseJiggler, ref enableMouseJiggler, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableMouseJiggler, enableMouseJiggler);
        }
    }

    /// <inheritdoc cref="MouseJigglerInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MouseJigglerInterval
    {
        get
        {
            if (!this.TryGetProperty<int>(RdpClientExtensions.MouseJigglerInterval, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.MouseJigglerInterval);
            return value;
        }
        set
        {
            object mouseJigglerInterval = value;
            if (!this.TrySetProperty(RdpClientExtensions.MouseJigglerInterval, ref mouseJigglerInterval, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.MouseJigglerInterval, mouseJigglerInterval);
        }
    }

    /// <inheritdoc cref="MouseJigglerMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public KeepAliveMethod MouseJigglerMethod
    {
        get
        {
            if (!this.TryGetProperty<int>(RdpClientExtensions.MouseJigglerMethod, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.MouseJigglerMethod);
            return (KeepAliveMethod)value;
        }
        set
        {
            object mouseJigglerMethod = (int)value;
            if (!this.TrySetProperty(RdpClientExtensions.MouseJigglerMethod, ref mouseJigglerMethod, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.MouseJigglerMethod, mouseJigglerMethod);
        }
    }

    /// <inheritdoc cref="EnableRdsAadAuth"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableRdsAadAuth
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableRdsAadAuth, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableRdsAadAuth);
            return value;
        }
        set
        {
            object enableRdsAadAuth = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableRdsAadAuth, ref enableRdsAadAuth, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableRdsAadAuth, enableRdsAadAuth);
        }
    }

    #endregion

    #region ::: Performance :::

    /// <inheritdoc cref="NetworkConnectionType"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint NetworkConnectionType
    {
        get => AdvancedSettings8.NetworkConnectionType;
        set => AdvancedSettings8.NetworkConnectionType = value;
    }

    /// <inheritdoc cref="PerformanceFlags"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int PerformanceFlags
    {
        get => AdvancedSettings8.PerformanceFlags;
        set => AdvancedSettings8.PerformanceFlags = value;
    }

    /// <inheritdoc cref="RedirectDirectX"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDirectX
    {
        get => AdvancedSettings8.RedirectDirectX;
        set => AdvancedSettings8.RedirectDirectX = value;
    }

    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="BandwidthDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BandwidthDetection
    {
        get => false;
        set => LogFeatureNotSupported(nameof(BandwidthDetection));
    }

    /// <inheritdoc cref="EnableHardwareMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableHardwareMode
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableHardwareMode, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableHardwareMode);
            return value;
        }
        set
        {
            object enableHardwareMode = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableHardwareMode, ref enableHardwareMode, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableHardwareMode, enableHardwareMode);
        }
    }

    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="ClientProtocolSpec"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClientSpec ClientProtocolSpec
    {
        get => ClientSpec.FullMode;
        set => LogFeatureNotSupported(nameof(ClientProtocolSpec));
    }

    #endregion

    #region ::: Redirection :::

    /// <inheritdoc cref="AudioRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioRedirectionMode AudioRedirectionMode
    {
        get => (AudioRedirectionMode)SecuredSettings3.AudioRedirectionMode;
        set => SecuredSettings3.AudioRedirectionMode = (int)value;
    }

    /// <inheritdoc cref="AudioQualityMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioQualityMode AudioQualityMode
    {
        get => (AudioQualityMode)AdvancedSettings8.AudioQualityMode;
        set => AdvancedSettings8.AudioQualityMode = (uint)value;
    }

    /// <inheritdoc cref="AudioCaptureRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AudioCaptureRedirectionMode
    {
        get => AdvancedSettings8.AudioCaptureRedirectionMode;
        set => AdvancedSettings8.AudioCaptureRedirectionMode = value;
    }

    /// <inheritdoc cref="VideoPlaybackMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VideoPlaybackMode VideoPlaybackMode
    {
        get => (VideoPlaybackMode)AdvancedSettings8.VideoPlaybackMode;
        set => AdvancedSettings8.VideoPlaybackMode = (uint)value;
    }

    /// <inheritdoc cref="RedirectPrinters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPrinters
    {
        get => AdvancedSettings8.RedirectPrinters;
        set => AdvancedSettings8.RedirectPrinters = value;
    }

    /// <inheritdoc cref="RedirectClipboard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectClipboard
    {
        get => AdvancedSettings8.RedirectClipboard;
        set => AdvancedSettings8.RedirectClipboard = value;
    }

    /// <inheritdoc cref="RedirectSmartCards"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectSmartCards
    {
        get => AdvancedSettings8.RedirectSmartCards;
        set => AdvancedSettings8.RedirectSmartCards = value;
    }
    /// <inheritdoc cref="RedirectPorts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPorts
    {
        get => AdvancedSettings8.RedirectPorts;
        set => AdvancedSettings8.RedirectPorts = value;
    }

    /// <inheritdoc cref="RedirectDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDevices
    {
        get => AdvancedSettings8.RedirectDevices;
        set => AdvancedSettings8.RedirectDevices = value;
    }

    /// <inheritdoc cref="RedirectPOSDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPOSDevices
    {
        get => AdvancedSettings8.RedirectPOSDevices;
        set => AdvancedSettings8.RedirectPOSDevices = value;
    }

    /// <inheritdoc cref="RedirectDrives"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDrives
    {
        get => AdvancedSettings8.RedirectDrives;
        set => AdvancedSettings8.RedirectDrives = value;
    }

    private string _redirectDriveLetters = string.Empty;
    /// <inheritdoc cref="RedirectDriveLetters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RedirectDriveLetters
    {
        get => _redirectDriveLetters;
        set
        {
            _redirectDriveLetters = value;
            if (RedirectDrives && !this.SetupDriveRedirection(_redirectDriveLetters, out var ex))
            {
                Logger.LogWarning(ex, "One or more errors occurred during drive redirection");
            }
        }
    }

    /// <inheritdoc cref="RedirectCameras"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectCameras 
    {
        get => false;
        set => LogFeatureNotSupported(nameof(RedirectCameras));
    }

    /// <inheritdoc cref="RedirectLocation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectLocation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableLocationRedirection, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableLocationRedirection);
            return value;
        }
        set
        {
            object enableLocationRedirect = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableLocationRedirection, ref enableLocationRedirect, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableLocationRedirection, enableLocationRedirect);
        }
    }

    #endregion

    #region ::: Keyboard :::

    /// <inheritdoc cref="AcceleratorPassthrough"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AcceleratorPassthrough
    {
        get => AdvancedSettings8.AcceleratorPassthrough != 0;
        set => AdvancedSettings8.AcceleratorPassthrough = value ? 1 : 0;
    }

    /// <inheritdoc cref="EnableWindowsKey"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableWindowsKey
    {
        get => AdvancedSettings8.EnableWindowsKey != 0;
        set => AdvancedSettings8.EnableWindowsKey = value ? 1 : 0;
    }

    /// <inheritdoc cref="KeyboardHookMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int KeyboardHookMode
    {
        get => SecuredSettings3.KeyboardHookMode;
        set => SecuredSettings3.KeyboardHookMode = value;
    }

    /// <inheritdoc cref="KeyBoardLayoutStr"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string KeyBoardLayoutStr
    {
        set => AdvancedSettings8.KeyBoardLayoutStr = value;
    }

    #endregion

    #region ::: Program :::

    /// <inheritdoc cref="StartProgram"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string StartProgram
    {
        get => SecuredSettings3.StartProgram;
        set => SecuredSettings3.StartProgram = value;
    }

    /// <inheritdoc cref="WorkDir"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string WorkDir
    {
        get => SecuredSettings3.WorkDir;
        set => SecuredSettings3.WorkDir = value;
    }

    /// <inheritdoc cref="MaximizeShell"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool MaximizeShell
    {
        get => AdvancedSettings8.MaximizeShell != 0;
        set => AdvancedSettings8.MaximizeShell = value ? 1 : 0;
    }

    #endregion

    #region ::: Gateway :::

    /// <inheritdoc cref="GatewayUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayUsageMethod GatewayUsageMethod
    {
        get => (GatewayUsageMethod)TransportSettings3.GatewayUsageMethod;
        set => TransportSettings3.GatewayUsageMethod = (uint)value;
    }
    /// <inheritdoc cref="GatewayProfileUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayProfileUsageMethod GatewayProfileUsageMethod
    {
        get => (GatewayProfileUsageMethod)TransportSettings3.GatewayProfileUsageMethod;
        set => TransportSettings3.GatewayProfileUsageMethod = (uint)value;
    }
    /// <inheritdoc cref="GatewayCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayCredsSource
    {
        get => (GatewayCredentialSource)TransportSettings3.GatewayCredsSource;
        set => TransportSettings3.GatewayCredsSource = (uint)value;
    }
    /// <inheritdoc cref="GatewayUserSelectedCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayUserSelectedCredsSource
    {
        get => (GatewayCredentialSource)TransportSettings3.GatewayUserSelectedCredsSource;
        set => TransportSettings3.GatewayUserSelectedCredsSource = (uint)value;
    }
    /// <inheritdoc cref="GatewayCredSharing"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GatewayCredSharing
    {
        get => TransportSettings3.GatewayCredSharing != 0;
        set => TransportSettings3.GatewayCredSharing = value ? 1U : 0U;
    }
    /// <inheritdoc cref="GatewayHostname"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayHostname
    {
        get => TransportSettings3.GatewayHostname;
        set => TransportSettings3.GatewayHostname = value;
    }
    /// <inheritdoc cref="GatewayUsername"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayUsername
    {
        get => TransportSettings3.GatewayUsername;
        set => TransportSettings3.GatewayUsername = value;
    }
    /// <inheritdoc cref="GatewayDomain"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayDomain
    {
        get => TransportSettings3.GatewayDomain;
        set => TransportSettings3.GatewayDomain = value;
    }
    /// <inheritdoc cref="GatewayPassword"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayPassword
    {
        set => TransportSettings3.GatewayPassword = value;
    }

    #endregion

    #region ::: HyperV :::

    /// <inheritdoc cref="AuthenticationServiceClass"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AuthenticationServiceClass
    {
        get => AdvancedSettings8.AuthenticationServiceClass;
        set => AdvancedSettings8.AuthenticationServiceClass = value;
    }

    /// <inheritdoc cref="PCB"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PCB
    {
        get => AdvancedSettings8.PCB;
        set => AdvancedSettings8.PCB = value;
    }

    /// <inheritdoc cref="NegotiateSecurityLayer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NegotiateSecurityLayer
    {
        get
        {
            try { return this.GetNonScriptable5().NegotiateSecurityLayer; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "NegotiateSecurityLayer"); }
            return false;
        }
        set
        {
            try { this.GetNonScriptable5().NegotiateSecurityLayer = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "NegotiateSecurityLayer", value); }
        }
    }

    /// <inheritdoc cref="ShowConnectionInformation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowConnectionInformation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.ShowConnectionInformation, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.ShowConnectionInformation);
            return value;
        }
        set
        {
            object showConnectionInformation = value;
            if (!this.TrySetProperty(RdpClientExtensions.ShowConnectionInformation, ref showConnectionInformation, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.ShowConnectionInformation, showConnectionInformation);
        }
    }

    #endregion

    #region ::: Misc :::

    /// <inheritdoc cref="Logger"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ILogger Logger { get; set; } = DebugLoggerFactory.Create();

    /// <inheritdoc cref="DisableClickDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableClickDetection { get; set; }

    /// <inheritdoc cref="RaiseClientAreaClicked"/>
    public void RaiseClientAreaClicked() => OnClientAreaClicked?.Invoke(this, EventArgs.Empty);

    #endregion

    #region ::: MsRdpEx :::

    /// <inheritdoc cref="AxName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AxName
    {
        get => axName;
        set => axName = value;
    }

    /// <inheritdoc cref="RdpExDll"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RdpExDll
    {
        get => rdpExDll;
        set => rdpExDll = value;
    }

    #endregion

    #endregion

    #region --- Public Methods ---

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="Reconnect"/>
    public ControlReconnectStatus Reconnect(uint width, uint height)
    {
        LogFeatureNotSupported("Reconnect");
        return ControlReconnectStatus.controlReconnectBlocked;
    }

    /// <inheritdoc cref="GetRemoteMonitorsBoundingBox"/>
    public void GetRemoteMonitorsBoundingBox(out int left, out int top, out int right, out int bottom)
    {
        try
        {
            this.GetNonScriptable5().GetRemoteMonitorsBoundingBox(out left, out top, out right, out bottom);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to call RDP client method: {MethodName}", "GetRemoteMonitorsBoundingBox");
            left = 0;
            top = 0;
            right = 0;
            bottom = 0;
        }
    }

    /// <inheritdoc cref="GetErrorDescription"/>
    public string GetErrorDescription(int disconnectReasonCode) => GetErrorDescription((uint)disconnectReasonCode, (uint)ExtendedDisconnectReason);

    /// <inheritdoc cref="LogFeatureNotSupported"/>
    public void LogFeatureNotSupported(string feature)
    {
        Logger.LogDebug("The feature '{Feature}' is not supported in Rdp Client V8", feature);
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="SendRemoteAction"/>
    public void SendRemoteAction(RemoteSessionActionType action)
    {
        LogFeatureNotSupported("SendRemoteAction");
    }

    /// <summary>
    /// This client does not support this feature. 
    /// </summary>
    /// <inheritdoc cref="UpdateSessionDisplaySettings"/>
    public void UpdateSessionDisplaySettings(uint desktopWidth, uint desktopHeight, uint physicalWidth, uint physicalHeight, uint orientation, uint desktopScaleFactor, uint deviceScaleFactor)
    {
        LogFeatureNotSupported("UpdateSessionDisplaySettings");
    }

    /// <inheritdoc cref="DeviceScaleFactor"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint DeviceScaleFactor
    {
        get
        {
            if (!this.TryGetProperty<uint>(RdpClientExtensions.DeviceScaleFactor, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DeviceScaleFactor);
            return value;
        }
        set
        {
            object deviceScaleFactor = value;
            if (!this.TrySetProperty(RdpClientExtensions.DeviceScaleFactor, ref deviceScaleFactor, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DeviceScaleFactor, deviceScaleFactor);
        }
    }

    /// <inheritdoc cref="DesktopScaleFactor"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint DesktopScaleFactor
    {
        get
        {
            if (!this.TryGetProperty<uint>(RdpClientExtensions.DesktopScaleFactor, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DesktopScaleFactor);
            return value;
        }
        set
        {
            object desktopScaleFactor = value;
            if (!this.TrySetProperty(RdpClientExtensions.DesktopScaleFactor, ref desktopScaleFactor, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DesktopScaleFactor, desktopScaleFactor);
        }
    }

    #endregion

    #region --- Message Processing ---

    /// <inheritdoc cref="WndProc"/>
    protected override void WndProc(ref Message m)
    {
        if (MessageFilter.Filter(this, ref m))
            return;
        base.WndProc(ref m);
    }

    #endregion
}

/// <inheritdoc cref="IRdpClient"/>
public class RdpClient9 : AxMsRdpClient8NotSafeForScripting, IRdpClient
{
    #region --- Events ---
    
    /// <inheritdoc cref="OnClientAreaClicked"/>
    public event EventHandler? OnClientAreaClicked;

    #endregion

    #region --- Rdp Settings ---

    #region ::: General :::

    // public string Server { get; set; }

    /// <inheritdoc cref="Port"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Port { get => AdvancedSettings9.RDPPort; set => AdvancedSettings9.RDPPort = value; }

    /// <inheritdoc cref="ConnectionState"/>
    public ConnectionState ConnectionState => (ConnectionState)Connected;

    #endregion

    #region ::: Credentials :::

    // public string UserName { get; set; }

    // public string Domain { get; set; }

    /// <inheritdoc cref="Password"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Password { set => AdvancedSettings9.ClearTextPassword = value; }

    /// <inheritdoc cref="NetworkLevelAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NetworkLevelAuthentication { get => AdvancedSettings9.EnableCredSspSupport; set => AdvancedSettings9.EnableCredSspSupport = value; }

    /// <inheritdoc cref="PasswordContainsSmartCardPin"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PasswordContainsSmartCardPin
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.PasswordContainsSmartcardPin, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.PasswordContainsSmartcardPin);
            return value;
        }
        set
        {
            object passwordContainsSCardPin = value;
            if (!this.TrySetProperty(RdpClientExtensions.PasswordContainsSmartcardPin, ref passwordContainsSCardPin, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.PasswordContainsSmartcardPin, passwordContainsSCardPin);
        }
    }

    #endregion

    #region ::: Display Settings :::

    // public int ColorDepth { get; set; }

    // public int DesktopWidth { get; set; }

    // public int DesktopHeight { get; set; }

    /// <inheritdoc cref="SmartSizing"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SmartSizing { get => AdvancedSettings9.SmartSizing; set => AdvancedSettings9.SmartSizing = value; }

    // public bool FullScreen { get; set; }

    // public string FullScreenTitle { set; }

    /// <inheritdoc cref="ContainerHandledFullScreen"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ContainerHandledFullScreen
    {
        get => AdvancedSettings9.ContainerHandledFullScreen;
        set => AdvancedSettings9.ContainerHandledFullScreen = value;
    }

    /// <inheritdoc cref="DisplayConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisplayConnectionBar { get => AdvancedSettings9.DisplayConnectionBar; set => AdvancedSettings9.DisplayConnectionBar = value; }

    /// <inheritdoc cref="PinConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PinConnectionBar { get => AdvancedSettings9.PinConnectionBar; set => AdvancedSettings9.PinConnectionBar = value; }

    /// <inheritdoc cref="UseMultimon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseMultimon
    {
        get
        {
            try { return this.GetNonScriptable5().UseMultimon; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "UseMultimon"); }
            return false;
        }
        set
        {
            try { this.GetNonScriptable5().UseMultimon = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "UseMultimon", value); }
        }
    }

    /// <inheritdoc cref="RemoteMonitorCount"/>
    public uint RemoteMonitorCount
    {
        get
        {
            try { return this.GetNonScriptable5().RemoteMonitorCount; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "RemoteMonitorCount"); }
            return 1;
        }
    }

    /// <inheritdoc cref="DeviceScaleFactor"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint DeviceScaleFactor
    {
        get
        {
            if (!this.TryGetProperty<uint>(RdpClientExtensions.DeviceScaleFactor, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DeviceScaleFactor);
            return value;
        }
        set
        {
            object deviceScaleFactor = value;
            if (!this.TrySetProperty(RdpClientExtensions.DeviceScaleFactor, ref deviceScaleFactor, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DeviceScaleFactor, deviceScaleFactor);
        }
    }

    /// <inheritdoc cref="DesktopScaleFactor"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint DesktopScaleFactor
    {
        get
        {
            if (!this.TryGetProperty<uint>(RdpClientExtensions.DesktopScaleFactor, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DesktopScaleFactor);
            return value;
        }
        set
        {
            object desktopScaleFactor = value;
            if (!this.TrySetProperty(RdpClientExtensions.DesktopScaleFactor, ref desktopScaleFactor, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DesktopScaleFactor, desktopScaleFactor);
        }
    }

    #endregion

    #region ::: Advanced :::

    /// <inheritdoc cref="AuthenticationLevel"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AuthenticationLevel AuthenticationLevel
    {
        get => (AuthenticationLevel)AdvancedSettings9.AuthenticationLevel;
        set => AdvancedSettings9.AuthenticationLevel = (uint)value;
    }

    /// <inheritdoc cref="Compression"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Compression
    {
        get => AdvancedSettings9.Compress > 0;
        set => AdvancedSettings9.Compress = value ? 1 : 0;
    }

    /// <inheritdoc cref="BitmapCaching"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BitmapCaching
    {
        get => AdvancedSettings9.BitmapPersistence > 0;
        set
        {
            AdvancedSettings9.BitmapPersistence = value ? 1 : 0;
            AdvancedSettings9.BitmapPeristence = value ? 1 : 0;
        }
    }

    /// <inheritdoc cref="PublicMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PublicMode
    {
        get => AdvancedSettings9.PublicMode;
        set => AdvancedSettings9.PublicMode = value;
    }

    /// <inheritdoc cref="AllowBackgroundInput"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AllowBackgroundInput
    {
        get => AdvancedSettings9.allowBackgroundInput > 0;
        set => AdvancedSettings9.allowBackgroundInput = value ? 1 : 0;
    }

    /// <inheritdoc cref="DisableUdpTransport"/>>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableUdpTransport
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.DisableUdpTransport, out var value, out var ex))
                Logger.LogWarning(ex,"Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DisableUdpTransport);
            return value;
        }
        set
        {
            object disableUdpTransport = value;
            if (!this.TrySetProperty(RdpClientExtensions.DisableUdpTransport, ref disableUdpTransport, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DisableUdpTransport, disableUdpTransport);
        }
    }

    /// <inheritdoc cref="EnableAutoReconnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableAutoReconnect
    {
        get => AdvancedSettings9.EnableAutoReconnect;
        set => AdvancedSettings9.EnableAutoReconnect = value;
    }

    /// <inheritdoc cref="MaxReconnectAttempts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MaxReconnectAttempts
    {
        get => AdvancedSettings9.MaxReconnectAttempts;
        set => AdvancedSettings9.MaxReconnectAttempts = value;
    }

    /// <inheritdoc cref="ConnectToAdministerServer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ConnectToAdministerServer
    {
        get => AdvancedSettings9.ConnectToAdministerServer;
        set => AdvancedSettings9.ConnectToAdministerServer = value;
    }

    /// <inheritdoc cref="UseRedirectionServerName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseRedirectionServerName
    {
        get
        {
            try { return this.GetPreferredRedirectionInfo().UseRedirectionServerName; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "UseRedirectionServerName"); }
            return false;
        }
        set
        {
            try { this.GetPreferredRedirectionInfo().UseRedirectionServerName = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "UseRedirectionServerName", value); }
        }
    }

    /// <inheritdoc cref="KeepAliveInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int KeepAliveInterval
    {
        get => AdvancedSettings2.keepAliveInterval;
        set => AdvancedSettings2.keepAliveInterval = value;
    }

    /// <inheritdoc cref="LoadBalanceInfo"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string LoadBalanceInfo
    {
        get => AdvancedSettings9.LoadBalanceInfo;
        set => RdpClientExtensions.SetLoadBalanceInfo(value, AdvancedSettings9);
    }

    /// <inheritdoc cref="PluginDlls"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PluginDlls { set => AdvancedSettings9.PluginDlls = value; }

    /// <inheritdoc cref="GrabFocusOnConnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GrabFocusOnConnect
    {
        get => AdvancedSettings9.GrabFocusOnConnect;
        set => AdvancedSettings9.GrabFocusOnConnect = value;
    }

    /// <inheritdoc cref="RelativeMouseMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RelativeMouseMode { get => AdvancedSettings9.RelativeMouseMode; set => AdvancedSettings9.RelativeMouseMode = value; }

    /// <inheritdoc cref="DisableCredentialsDelegation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableCredentialsDelegation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.DisableCredentialsDelegation, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DisableCredentialsDelegation);
            return value;
        }
        set
        {
            object disableCredentialsDelegation = value;
            if (!this.TrySetProperty(RdpClientExtensions.DisableCredentialsDelegation, ref disableCredentialsDelegation, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DisableCredentialsDelegation, disableCredentialsDelegation);
        }
    }

    /// <inheritdoc cref="RedirectedAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectedAuthentication
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.RedirectedAuthentication, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.RedirectedAuthentication);
            return value;
        }
        set
        {
            object redirectedAuthentication = value;
            if (!this.TrySetProperty(RdpClientExtensions.RedirectedAuthentication, ref redirectedAuthentication, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.RedirectedAuthentication, redirectedAuthentication);
        }
    }

    /// <inheritdoc cref="RestrictedLogon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedLogon
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.RestrictedLogon, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.RestrictedLogon);
            return value;
        }
        set
        {
            object restrictedLogon = value;
            if (!this.TrySetProperty(RdpClientExtensions.RestrictedLogon, ref restrictedLogon, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.RestrictedLogon, restrictedLogon);
        }
    }


    /// <inheritdoc cref="RemoteCredentialGuard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RemoteCredentialGuard
    {
        get => DisableCredentialsDelegation && RedirectedAuthentication;
        set
        {
            RedirectedAuthentication = value;
            DisableCredentialsDelegation = value;
        }
    }

    /// <inheritdoc cref="RestrictedAdminMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedAdminMode
    {
        get => DisableCredentialsDelegation && RestrictedLogon;
        set
        {
            RestrictedLogon = value;
            DisableCredentialsDelegation = value;
        }
    }

    /// <inheritdoc cref="EnableMouseJiggler"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableMouseJiggler
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableMouseJiggler, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableMouseJiggler);
            return value;
        }
        set
        {
            object enableMouseJiggler = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableMouseJiggler, ref enableMouseJiggler, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableMouseJiggler, enableMouseJiggler);
        }
    }

    /// <inheritdoc cref="MouseJigglerInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MouseJigglerInterval
    {
        get
        {
            if (!this.TryGetProperty<int>(RdpClientExtensions.MouseJigglerInterval, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.MouseJigglerInterval);
            return value;
        }
        set
        {
            object mouseJigglerInterval = value;
            if (!this.TrySetProperty(RdpClientExtensions.MouseJigglerInterval, ref mouseJigglerInterval, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.MouseJigglerInterval, mouseJigglerInterval);
        }
    }

    /// <inheritdoc cref="MouseJigglerMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public KeepAliveMethod MouseJigglerMethod
    {
        get
        {
            if (!this.TryGetProperty<int>(RdpClientExtensions.MouseJigglerMethod, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.MouseJigglerMethod);
            return (KeepAliveMethod)value;
        }
        set
        {
            object mouseJigglerMethod = (int)value;
            if (!this.TrySetProperty(RdpClientExtensions.MouseJigglerMethod, ref mouseJigglerMethod, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.MouseJigglerMethod, mouseJigglerMethod);
        }
    }

    /// <inheritdoc cref="EnableRdsAadAuth"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableRdsAadAuth
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableRdsAadAuth, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableRdsAadAuth);
            return value;
        }
        set
        {
            object enableRdsAadAuth = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableRdsAadAuth, ref enableRdsAadAuth, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableRdsAadAuth, enableRdsAadAuth);
        }
    }

    #endregion

    #region ::: Performance :::

    /// <inheritdoc cref="NetworkConnectionType"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint NetworkConnectionType
    {
        get => AdvancedSettings9.NetworkConnectionType;
        set => AdvancedSettings9.NetworkConnectionType = value;
    }

    /// <inheritdoc cref="PerformanceFlags"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int PerformanceFlags
    {
        get => AdvancedSettings9.PerformanceFlags;
        set => AdvancedSettings9.PerformanceFlags = value;
    }

    /// <inheritdoc cref="RedirectDirectX"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDirectX
    {
        get => AdvancedSettings9.RedirectDirectX;
        set => AdvancedSettings9.RedirectDirectX = value;
    }

    /// <inheritdoc cref="BandwidthDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BandwidthDetection
    {
        get => AdvancedSettings9.BandwidthDetection;
        set => AdvancedSettings9.BandwidthDetection = value;
    }

    /// <inheritdoc cref="EnableHardwareMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableHardwareMode
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableHardwareMode, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableHardwareMode);
            return value;
        }
        set
        {
            object enableHardwareMode = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableHardwareMode, ref enableHardwareMode, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableHardwareMode, enableHardwareMode);
        }
    }

    /// <inheritdoc cref="ClientProtocolSpec"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClientSpec ClientProtocolSpec
    {
        get => AdvancedSettings9.ClientProtocolSpec;
        set => AdvancedSettings9.ClientProtocolSpec = value;
    }

    #endregion

    #region ::: Redirection :::

    /// <inheritdoc cref="AudioRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioRedirectionMode AudioRedirectionMode
    {
        get => (AudioRedirectionMode)SecuredSettings3.AudioRedirectionMode;
        set => SecuredSettings3.AudioRedirectionMode = (int)value;
    }

    /// <inheritdoc cref="AudioQualityMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioQualityMode AudioQualityMode
    {
        get => (AudioQualityMode)AdvancedSettings8.AudioQualityMode;
        set => AdvancedSettings8.AudioQualityMode = (uint)value;
    }

    /// <inheritdoc cref="AudioCaptureRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AudioCaptureRedirectionMode
    {
        get => AdvancedSettings9.AudioCaptureRedirectionMode;
        set => AdvancedSettings9.AudioCaptureRedirectionMode = value;
    }

    /// <inheritdoc cref="VideoPlaybackMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VideoPlaybackMode VideoPlaybackMode
    {
        get => (VideoPlaybackMode)AdvancedSettings8.VideoPlaybackMode;
        set => AdvancedSettings8.VideoPlaybackMode = (uint)value;
    }

    /// <inheritdoc cref="RedirectPrinters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPrinters
    {
        get => AdvancedSettings9.RedirectPrinters;
        set => AdvancedSettings9.RedirectPrinters = value;
    }

    /// <inheritdoc cref="RedirectClipboard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectClipboard
    {
        get => AdvancedSettings9.RedirectClipboard;
        set => AdvancedSettings9.RedirectClipboard = value;
    }

    /// <inheritdoc cref="RedirectSmartCards"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectSmartCards
    {
        get => AdvancedSettings9.RedirectSmartCards;
        set => AdvancedSettings9.RedirectSmartCards = value;
    }
    /// <inheritdoc cref="RedirectPorts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPorts
    {
        get => AdvancedSettings9.RedirectPorts;
        set => AdvancedSettings9.RedirectPorts = value;
    }

    /// <inheritdoc cref="RedirectDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDevices
    {
        get => AdvancedSettings9.RedirectDevices;
        set => AdvancedSettings9.RedirectDevices = value;
    }

    /// <inheritdoc cref="RedirectPOSDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPOSDevices
    {
        get => AdvancedSettings9.RedirectPOSDevices;
        set => AdvancedSettings9.RedirectPOSDevices = value;
    }

    /// <inheritdoc cref="RedirectDrives"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDrives
    {
        get => AdvancedSettings9.RedirectDrives;
        set => AdvancedSettings9.RedirectDrives = value;
    }

    private string _redirectDriveLetters = string.Empty;
    /// <inheritdoc cref="RedirectDriveLetters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RedirectDriveLetters
    {
        get => _redirectDriveLetters;
        set
        {
            _redirectDriveLetters = value;
            if (RedirectDrives && !this.SetupDriveRedirection(_redirectDriveLetters, out var ex))
            {
                Logger.LogWarning(ex, "One or more errors occurred during drive redirection");
            }
        }
    }

    /// <inheritdoc cref="RedirectCameras"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectCameras 
    {
        get => false;
        set => LogFeatureNotSupported(nameof(RedirectCameras));
    }

    /// <inheritdoc cref="RedirectLocation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectLocation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableLocationRedirection, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableLocationRedirection);
            return value;
        }
        set
        {
            object enableLocationRedirect = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableLocationRedirection, ref enableLocationRedirect, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableLocationRedirection, enableLocationRedirect);
        }
    }

    #endregion

    #region ::: Keyboard :::

    /// <inheritdoc cref="AcceleratorPassthrough"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AcceleratorPassthrough
    {
        get => AdvancedSettings9.AcceleratorPassthrough != 0;
        set => AdvancedSettings9.AcceleratorPassthrough = value ? 1 : 0;
    }

    /// <inheritdoc cref="EnableWindowsKey"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableWindowsKey
    {
        get => AdvancedSettings9.EnableWindowsKey != 0;
        set => AdvancedSettings9.EnableWindowsKey = value ? 1 : 0;
    }

    /// <inheritdoc cref="KeyboardHookMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int KeyboardHookMode
    {
        get => SecuredSettings3.KeyboardHookMode;
        set => SecuredSettings3.KeyboardHookMode = value;
    }

    /// <inheritdoc cref="KeyBoardLayoutStr"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string KeyBoardLayoutStr
    {
        set => AdvancedSettings9.KeyBoardLayoutStr = value;
    }

    #endregion

    #region ::: Program :::

    /// <inheritdoc cref="StartProgram"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string StartProgram
    {
        get => SecuredSettings3.StartProgram;
        set => SecuredSettings3.StartProgram = value;
    }

    /// <inheritdoc cref="WorkDir"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string WorkDir
    {
        get => SecuredSettings3.WorkDir;
        set => SecuredSettings3.WorkDir = value;
    }

    /// <inheritdoc cref="MaximizeShell"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool MaximizeShell
    {
        get => AdvancedSettings9.MaximizeShell != 0;
        set => AdvancedSettings9.MaximizeShell = value ? 1 : 0;
    }

    #endregion

    #region ::: Gateway :::

    /// <inheritdoc cref="GatewayUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayUsageMethod GatewayUsageMethod
    {
        get => (GatewayUsageMethod)TransportSettings3.GatewayUsageMethod;
        set => TransportSettings3.GatewayUsageMethod = (uint)value;
    }
    /// <inheritdoc cref="GatewayProfileUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayProfileUsageMethod GatewayProfileUsageMethod
    {
        get => (GatewayProfileUsageMethod)TransportSettings3.GatewayProfileUsageMethod;
        set => TransportSettings3.GatewayProfileUsageMethod = (uint)value;
    }
    /// <inheritdoc cref="GatewayCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayCredsSource
    {
        get => (GatewayCredentialSource)TransportSettings3.GatewayCredsSource;
        set => TransportSettings3.GatewayCredsSource = (uint)value;
    }
    /// <inheritdoc cref="GatewayUserSelectedCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayUserSelectedCredsSource
    {
        get => (GatewayCredentialSource)TransportSettings3.GatewayUserSelectedCredsSource;
        set => TransportSettings3.GatewayUserSelectedCredsSource = (uint)value;
    }
    /// <inheritdoc cref="GatewayCredSharing"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GatewayCredSharing
    {
        get => TransportSettings3.GatewayCredSharing != 0;
        set => TransportSettings3.GatewayCredSharing = value ? 1U : 0U;
    }
    /// <inheritdoc cref="GatewayHostname"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayHostname
    {
        get => TransportSettings3.GatewayHostname;
        set => TransportSettings3.GatewayHostname = value;
    }
    /// <inheritdoc cref="GatewayUsername"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayUsername
    {
        get => TransportSettings3.GatewayUsername;
        set => TransportSettings3.GatewayUsername = value;
    }
    /// <inheritdoc cref="GatewayDomain"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayDomain
    {
        get => TransportSettings3.GatewayDomain;
        set => TransportSettings3.GatewayDomain = value;
    }
    /// <inheritdoc cref="GatewayPassword"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayPassword
    {
        set => TransportSettings3.GatewayPassword = value;
    }

    #endregion

    #region ::: HyperV :::

    /// <inheritdoc cref="AuthenticationServiceClass"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AuthenticationServiceClass
    {
        get => AdvancedSettings9.AuthenticationServiceClass;
        set => AdvancedSettings9.AuthenticationServiceClass = value;
    }

    /// <inheritdoc cref="PCB"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PCB
    {
        get => AdvancedSettings9.PCB;
        set => AdvancedSettings9.PCB = value;
    }

    /// <inheritdoc cref="NegotiateSecurityLayer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NegotiateSecurityLayer
    {
        get
        {
            try { return this.GetNonScriptable5().NegotiateSecurityLayer; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "NegotiateSecurityLayer"); }
            return false;
        }
        set
        {
            try { this.GetNonScriptable5().NegotiateSecurityLayer = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "NegotiateSecurityLayer", value); }
        }
    }

    /// <inheritdoc cref="ShowConnectionInformation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowConnectionInformation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.ShowConnectionInformation, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.ShowConnectionInformation);
            return value;
        }
        set
        {
            object showConnectionInformation = value;
            if (!this.TrySetProperty(RdpClientExtensions.ShowConnectionInformation, ref showConnectionInformation, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.ShowConnectionInformation, showConnectionInformation);
        }
    }

    #endregion

    #region ::: Misc :::

    /// <inheritdoc cref="Logger"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ILogger Logger { get; set; } = DebugLoggerFactory.Create();

    /// <inheritdoc cref="DisableClickDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableClickDetection { get; set; }

    /// <inheritdoc cref="RaiseClientAreaClicked"/>
    public void RaiseClientAreaClicked() => OnClientAreaClicked?.Invoke(this, EventArgs.Empty);

    #endregion

    #region ::: MsRdpEx :::

    /// <inheritdoc cref="AxName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AxName
    {
        get => axName;
        set => axName = value;
    }

    /// <inheritdoc cref="RdpExDll"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RdpExDll
    {
        get => rdpExDll;
        set => rdpExDll = value;
    }

    #endregion
        
    #endregion

    #region --- Public Methods ---

    /// <inheritdoc cref="GetRemoteMonitorsBoundingBox"/>
    public void GetRemoteMonitorsBoundingBox(out int left, out int top, out int right, out int bottom)
    {
        try
        {
            this.GetNonScriptable5().GetRemoteMonitorsBoundingBox(out left, out top, out right, out bottom);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to call RDP client method: {MethodName}", "GetRemoteMonitorsBoundingBox");
            left = 0;
            top = 0;
            right = 0;
            bottom = 0;
        }
    }

    /// <inheritdoc cref="GetErrorDescription"/>
    public string GetErrorDescription(int disconnectReasonCode) => GetErrorDescription((uint)disconnectReasonCode, (uint)ExtendedDisconnectReason);

    /// <inheritdoc cref="LogFeatureNotSupported"/>
    public void LogFeatureNotSupported(string feature) => Logger.LogDebug("The feature '{Feature}' is not supported in Rdp Client V9", feature);

    /// <summary>
    /// This client does not support this feature.
    /// </summary>
    /// <inheritdoc cref="UpdateSessionDisplaySettings"/>
    public void UpdateSessionDisplaySettings(uint desktopWidth, uint desktopHeight, uint physicalWidth, uint physicalHeight, uint orientation, uint desktopScaleFactor, uint deviceScaleFactor)
    {
        LogFeatureNotSupported("UpdateSessionDisplaySettings");
    }

    #endregion

    #region --- Message Processing ---

    /// <inheritdoc cref="WndProc"/>
    protected override void WndProc(ref Message m)
    {
        if (MessageFilter.Filter(this, ref m))
            return;
        base.WndProc(ref m);
    }

    #endregion
}

/// <inheritdoc cref="IRdpClient"/>
public class RdpClient10 : AxMsRdpClient9NotSafeForScripting, IRdpClient
{
    #region --- Events ---
    
    /// <inheritdoc cref="OnClientAreaClicked"/>
    public event EventHandler? OnClientAreaClicked;

    #endregion

    #region --- Rdp Settings ---

    #region ::: General :::

    // public string Server { get; set; }

    /// <inheritdoc cref="Port"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Port { get => AdvancedSettings9.RDPPort; set => AdvancedSettings9.RDPPort = value; }

    /// <inheritdoc cref="ConnectionState"/>
    public ConnectionState ConnectionState => (ConnectionState)Connected;

    #endregion

    #region ::: Credentials :::

    // public string UserName { get; set; }

    // public string Domain { get; set; }

    /// <inheritdoc cref="Password"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Password { set => AdvancedSettings9.ClearTextPassword = value; }

    /// <inheritdoc cref="NetworkLevelAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NetworkLevelAuthentication { get => AdvancedSettings9.EnableCredSspSupport; set => AdvancedSettings9.EnableCredSspSupport = value; }

    /// <inheritdoc cref="PasswordContainsSmartCardPin"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PasswordContainsSmartCardPin
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.PasswordContainsSmartcardPin, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.PasswordContainsSmartcardPin);
            return value;
        }
        set
        {
            object passwordContainsSCardPin = value;
            if (!this.TrySetProperty(RdpClientExtensions.PasswordContainsSmartcardPin, ref passwordContainsSCardPin, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.PasswordContainsSmartcardPin, passwordContainsSCardPin);
        }
    }

    #endregion

    #region ::: Display Settings :::

    // public int ColorDepth { get; set; }

    // public int DesktopWidth { get; set; }

    // public int DesktopHeight { get; set; }

    /// <inheritdoc cref="SmartSizing"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SmartSizing { get => AdvancedSettings9.SmartSizing; set => AdvancedSettings9.SmartSizing = value; }

    // public bool FullScreen { get; set; }

    // public string FullScreenTitle { set; }

    /// <inheritdoc cref="ContainerHandledFullScreen"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ContainerHandledFullScreen
    {
        get => AdvancedSettings9.ContainerHandledFullScreen;
        set => AdvancedSettings9.ContainerHandledFullScreen = value;
    }

    /// <inheritdoc cref="DisplayConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisplayConnectionBar { get => AdvancedSettings9.DisplayConnectionBar; set => AdvancedSettings9.DisplayConnectionBar = value; }

    /// <inheritdoc cref="PinConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PinConnectionBar { get => AdvancedSettings9.PinConnectionBar; set => AdvancedSettings9.PinConnectionBar = value; }

    /// <inheritdoc cref="UseMultimon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseMultimon
    {
        get
        {
            try { return this.GetNonScriptable5().UseMultimon; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "UseMultimon"); }
            return false;
        }
        set
        {
            try { this.GetNonScriptable5().UseMultimon = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "UseMultimon", value); }
        }
    }

    /// <inheritdoc cref="RemoteMonitorCount"/>
    public uint RemoteMonitorCount
    {
        get
        {
            try { return this.GetNonScriptable5().RemoteMonitorCount; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "RemoteMonitorCount"); }
            return 1;
        }
    }

    /// <inheritdoc cref="DeviceScaleFactor"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint DeviceScaleFactor
    {
        get
        {
            if (!this.TryGetProperty<uint>(RdpClientExtensions.DeviceScaleFactor, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DeviceScaleFactor);
            return value;
        }
        set
        {
            object deviceScaleFactor = value;
            if (!this.TrySetProperty(RdpClientExtensions.DeviceScaleFactor, ref deviceScaleFactor, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DeviceScaleFactor, deviceScaleFactor);
        }
    }

    /// <inheritdoc cref="DesktopScaleFactor"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint DesktopScaleFactor
    {
        get
        {
            if (!this.TryGetProperty<uint>(RdpClientExtensions.DesktopScaleFactor, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DesktopScaleFactor);
            return value;
        }
        set
        {
            object desktopScaleFactor = value;
            if (!this.TrySetProperty(RdpClientExtensions.DesktopScaleFactor, ref desktopScaleFactor, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DesktopScaleFactor, desktopScaleFactor);
        }
    }


    #endregion

    #region ::: Advanced :::

    /// <inheritdoc cref="AuthenticationLevel"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AuthenticationLevel AuthenticationLevel
    {
        get => (AuthenticationLevel)AdvancedSettings9.AuthenticationLevel;
        set => AdvancedSettings9.AuthenticationLevel = (uint)value;
    }

    /// <inheritdoc cref="Compression"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Compression
    {
        get => AdvancedSettings9.Compress > 0;
        set => AdvancedSettings9.Compress = value ? 1 : 0;
    }

    /// <inheritdoc cref="BitmapCaching"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BitmapCaching
    {
        get => AdvancedSettings9.BitmapPersistence > 0;
        set
        {
            AdvancedSettings9.BitmapPersistence = value ? 1 : 0;
            AdvancedSettings9.BitmapPeristence = value ? 1 : 0;
        }
    }

    /// <inheritdoc cref="PublicMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PublicMode
    {
        get => AdvancedSettings9.PublicMode;
        set => AdvancedSettings9.PublicMode = value;
    }

    /// <inheritdoc cref="AllowBackgroundInput"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AllowBackgroundInput
    {
        get => AdvancedSettings9.allowBackgroundInput > 0;
        set => AdvancedSettings9.allowBackgroundInput = value ? 1 : 0;
    }

    /// <inheritdoc cref="DisableUdpTransport"/>>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableUdpTransport
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.DisableUdpTransport, out var value, out var ex))
                Logger.LogWarning(ex,"Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DisableUdpTransport);
            return value;
        }
        set
        {
            object disableUdpTransport = value;
            if (!this.TrySetProperty(RdpClientExtensions.DisableUdpTransport, ref disableUdpTransport, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DisableUdpTransport, disableUdpTransport);
        }
    }

    /// <inheritdoc cref="EnableAutoReconnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableAutoReconnect
    {
        get => AdvancedSettings9.EnableAutoReconnect;
        set => AdvancedSettings9.EnableAutoReconnect = value;
    }

    /// <inheritdoc cref="MaxReconnectAttempts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MaxReconnectAttempts
    {
        get => AdvancedSettings9.MaxReconnectAttempts;
        set => AdvancedSettings9.MaxReconnectAttempts = value;
    }

    /// <inheritdoc cref="ConnectToAdministerServer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ConnectToAdministerServer
    {
        get => AdvancedSettings9.ConnectToAdministerServer;
        set => AdvancedSettings9.ConnectToAdministerServer = value;
    }

    /// <inheritdoc cref="UseRedirectionServerName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseRedirectionServerName
    {
        get
        {
            try { return this.GetPreferredRedirectionInfo().UseRedirectionServerName; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "UseRedirectionServerName"); }
            return false;
        }
        set
        {
            try { this.GetPreferredRedirectionInfo().UseRedirectionServerName = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "UseRedirectionServerName", value); }
        }
    }

    /// <inheritdoc cref="KeepAliveInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int KeepAliveInterval
    {
        get => AdvancedSettings2.keepAliveInterval;
        set => AdvancedSettings2.keepAliveInterval = value;
    }

    /// <inheritdoc cref="LoadBalanceInfo"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string LoadBalanceInfo
    {
        get => AdvancedSettings9.LoadBalanceInfo;
        set => RdpClientExtensions.SetLoadBalanceInfo(value, AdvancedSettings9);
    }

    /// <inheritdoc cref="PluginDlls"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PluginDlls { set => AdvancedSettings9.PluginDlls = value; }

    /// <inheritdoc cref="GrabFocusOnConnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GrabFocusOnConnect
    {
        get => AdvancedSettings9.GrabFocusOnConnect;
        set => AdvancedSettings9.GrabFocusOnConnect = value;
    }

    /// <inheritdoc cref="RelativeMouseMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RelativeMouseMode { get => AdvancedSettings9.RelativeMouseMode; set => AdvancedSettings9.RelativeMouseMode = value; }

    /// <inheritdoc cref="DisableCredentialsDelegation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableCredentialsDelegation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.DisableCredentialsDelegation, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DisableCredentialsDelegation);
            return value;
        }
        set
        {
            object disableCredentialsDelegation = value;
            if (!this.TrySetProperty(RdpClientExtensions.DisableCredentialsDelegation, ref disableCredentialsDelegation, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DisableCredentialsDelegation, disableCredentialsDelegation);
        }
    }

    /// <inheritdoc cref="RedirectedAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectedAuthentication
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.RedirectedAuthentication, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.RedirectedAuthentication);
            return value;
        }
        set
        {
            object redirectedAuthentication = value;
            if (!this.TrySetProperty(RdpClientExtensions.RedirectedAuthentication, ref redirectedAuthentication, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.RedirectedAuthentication, redirectedAuthentication);
        }
    }

    /// <inheritdoc cref="RestrictedLogon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedLogon
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.RestrictedLogon, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.RestrictedLogon);
            return value;
        }
        set
        {
            object restrictedLogon = value;
            if (!this.TrySetProperty(RdpClientExtensions.RestrictedLogon, ref restrictedLogon, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.RestrictedLogon, restrictedLogon);
        }
    }


    /// <inheritdoc cref="RemoteCredentialGuard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RemoteCredentialGuard
    {
        get => DisableCredentialsDelegation && RedirectedAuthentication;
        set
        {
            RedirectedAuthentication = value;
            DisableCredentialsDelegation = value;
        }
    }

    /// <inheritdoc cref="RestrictedAdminMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedAdminMode
    {
        get => DisableCredentialsDelegation && RestrictedLogon;
        set
        {
            RestrictedLogon = value;
            DisableCredentialsDelegation = value;
        }
    }

    /// <inheritdoc cref="EnableMouseJiggler"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableMouseJiggler
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableMouseJiggler, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableMouseJiggler);
            return value;
        }
        set
        {
            object enableMouseJiggler = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableMouseJiggler, ref enableMouseJiggler, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableMouseJiggler, enableMouseJiggler);
        }
    }

    /// <inheritdoc cref="MouseJigglerInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MouseJigglerInterval
    {
        get
        {
            if (!this.TryGetProperty<int>(RdpClientExtensions.MouseJigglerInterval, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.MouseJigglerInterval);
            return value;
        }
        set
        {
            object mouseJigglerInterval = value;
            if (!this.TrySetProperty(RdpClientExtensions.MouseJigglerInterval, ref mouseJigglerInterval, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.MouseJigglerInterval, mouseJigglerInterval);
        }
    }

    /// <inheritdoc cref="MouseJigglerMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public KeepAliveMethod MouseJigglerMethod
    {
        get
        {
            if (!this.TryGetProperty<int>(RdpClientExtensions.MouseJigglerMethod, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.MouseJigglerMethod);
            return (KeepAliveMethod)value;
        }
        set
        {
            object mouseJigglerMethod = (int)value;
            if (!this.TrySetProperty(RdpClientExtensions.MouseJigglerMethod, ref mouseJigglerMethod, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.MouseJigglerMethod, mouseJigglerMethod);
        }
    }

    /// <inheritdoc cref="EnableRdsAadAuth"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableRdsAadAuth
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableRdsAadAuth, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableRdsAadAuth);
            return value;
        }
        set
        {
            object enableRdsAadAuth = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableRdsAadAuth, ref enableRdsAadAuth, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableRdsAadAuth, enableRdsAadAuth);
        }
    }

    #endregion

    #region ::: Performance :::

    /// <inheritdoc cref="NetworkConnectionType"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint NetworkConnectionType
    {
        get => AdvancedSettings9.NetworkConnectionType;
        set => AdvancedSettings9.NetworkConnectionType = value;
    }

    /// <inheritdoc cref="PerformanceFlags"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int PerformanceFlags
    {
        get => AdvancedSettings9.PerformanceFlags;
        set => AdvancedSettings9.PerformanceFlags = value;
    }

    /// <inheritdoc cref="RedirectDirectX"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDirectX
    {
        get => AdvancedSettings9.RedirectDirectX;
        set => AdvancedSettings9.RedirectDirectX = value;
    }

    /// <inheritdoc cref="BandwidthDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BandwidthDetection
    {
        get => AdvancedSettings9.BandwidthDetection;
        set => AdvancedSettings9.BandwidthDetection = value;
    }

    /// <inheritdoc cref="EnableHardwareMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableHardwareMode
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableHardwareMode, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableHardwareMode);
            return value;
        }
        set
        {
            object enableHardwareMode = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableHardwareMode, ref enableHardwareMode, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableHardwareMode, enableHardwareMode);
        }
    }

    /// <inheritdoc cref="ClientProtocolSpec"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClientSpec ClientProtocolSpec
    {
        get => AdvancedSettings9.ClientProtocolSpec;
        set => AdvancedSettings9.ClientProtocolSpec = value;
    }

    #endregion

    #region ::: Redirection :::

    /// <inheritdoc cref="AudioRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioRedirectionMode AudioRedirectionMode
    {
        get => (AudioRedirectionMode)SecuredSettings3.AudioRedirectionMode;
        set => SecuredSettings3.AudioRedirectionMode = (int)value;
    }

    /// <inheritdoc cref="AudioQualityMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioQualityMode AudioQualityMode
    {
        get => (AudioQualityMode)AdvancedSettings8.AudioQualityMode;
        set => AdvancedSettings8.AudioQualityMode = (uint)value;
    }

    /// <inheritdoc cref="AudioCaptureRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AudioCaptureRedirectionMode
    {
        get => AdvancedSettings9.AudioCaptureRedirectionMode;
        set => AdvancedSettings9.AudioCaptureRedirectionMode = value;
    }

    /// <inheritdoc cref="VideoPlaybackMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VideoPlaybackMode VideoPlaybackMode
    {
        get => (VideoPlaybackMode)AdvancedSettings8.VideoPlaybackMode;
        set => AdvancedSettings8.VideoPlaybackMode = (uint)value;
    }

    /// <inheritdoc cref="RedirectPrinters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPrinters
    {
        get => AdvancedSettings9.RedirectPrinters;
        set => AdvancedSettings9.RedirectPrinters = value;
    }

    /// <inheritdoc cref="RedirectClipboard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectClipboard
    {
        get => AdvancedSettings9.RedirectClipboard;
        set => AdvancedSettings9.RedirectClipboard = value;
    }

    /// <inheritdoc cref="RedirectSmartCards"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectSmartCards
    {
        get => AdvancedSettings9.RedirectSmartCards;
        set => AdvancedSettings9.RedirectSmartCards = value;
    }
    /// <inheritdoc cref="RedirectPorts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPorts
    {
        get => AdvancedSettings9.RedirectPorts;
        set => AdvancedSettings9.RedirectPorts = value;
    }

    /// <inheritdoc cref="RedirectDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDevices
    {
        get => AdvancedSettings9.RedirectDevices;
        set => AdvancedSettings9.RedirectDevices = value;
    }

    /// <inheritdoc cref="RedirectPOSDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPOSDevices
    {
        get => AdvancedSettings9.RedirectPOSDevices;
        set => AdvancedSettings9.RedirectPOSDevices = value;
    }

    /// <inheritdoc cref="RedirectDrives"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDrives
    {
        get => AdvancedSettings9.RedirectDrives;
        set => AdvancedSettings9.RedirectDrives = value;
    }

    private string _redirectDriveLetters = string.Empty;
        
    /// <inheritdoc cref="RedirectDriveLetters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RedirectDriveLetters
    {
        get => _redirectDriveLetters;
        set
        {
            _redirectDriveLetters = value;
            if (RedirectDrives && !this.SetupDriveRedirection(_redirectDriveLetters, out var ex))
            {
                Logger.LogWarning(ex, "One or more errors occurred during drive redirection");
            }
        }
    }

    /// <inheritdoc cref="RedirectCameras"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectCameras 
    {
        get => false;
        set => LogFeatureNotSupported(nameof(RedirectCameras));
    }

    /// <inheritdoc cref="RedirectLocation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectLocation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableLocationRedirection, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableLocationRedirection);
            return value;
        }
        set
        {
            object enableLocationRedirect = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableLocationRedirection, ref enableLocationRedirect, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableLocationRedirection, enableLocationRedirect);
        }
    }

    #endregion

    #region ::: Keyboard :::

    /// <inheritdoc cref="AcceleratorPassthrough"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AcceleratorPassthrough
    {
        get => AdvancedSettings9.AcceleratorPassthrough != 0;
        set => AdvancedSettings9.AcceleratorPassthrough = value ? 1 : 0;
    }

    /// <inheritdoc cref="EnableWindowsKey"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableWindowsKey
    {
        get => AdvancedSettings9.EnableWindowsKey != 0;
        set => AdvancedSettings9.EnableWindowsKey = value ? 1 : 0;
    }

    /// <inheritdoc cref="KeyboardHookMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int KeyboardHookMode
    {
        get => SecuredSettings3.KeyboardHookMode;
        set => SecuredSettings3.KeyboardHookMode = value;
    }

    /// <inheritdoc cref="KeyBoardLayoutStr"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string KeyBoardLayoutStr
    {
        set => AdvancedSettings9.KeyBoardLayoutStr = value;
    }

    #endregion

    #region ::: Program :::

    /// <inheritdoc cref="StartProgram"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string StartProgram
    {
        get => SecuredSettings3.StartProgram;
        set => SecuredSettings3.StartProgram = value;
    }

    /// <inheritdoc cref="WorkDir"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string WorkDir
    {
        get => SecuredSettings3.WorkDir;
        set => SecuredSettings3.WorkDir = value;
    }

    /// <inheritdoc cref="MaximizeShell"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool MaximizeShell
    {
        get => AdvancedSettings9.MaximizeShell != 0;
        set => AdvancedSettings9.MaximizeShell = value ? 1 : 0;
    }

    #endregion

    #region ::: Gateway :::

    /// <inheritdoc cref="GatewayUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayUsageMethod GatewayUsageMethod 
    {
        get => (GatewayUsageMethod)TransportSettings4.GatewayUsageMethod;
        set => TransportSettings4.GatewayUsageMethod = (uint) value;
    }
    /// <inheritdoc cref="GatewayProfileUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayProfileUsageMethod GatewayProfileUsageMethod
    {
        get => (GatewayProfileUsageMethod)TransportSettings4.GatewayProfileUsageMethod;
        set => TransportSettings4.GatewayProfileUsageMethod = (uint)value;
    }
    /// <inheritdoc cref="GatewayCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayCredsSource
    {
        get => (GatewayCredentialSource)TransportSettings4.GatewayCredsSource;
        set => TransportSettings4.GatewayCredsSource = (uint)value;
    }
    /// <inheritdoc cref="GatewayUserSelectedCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayUserSelectedCredsSource
    {
        get => (GatewayCredentialSource)TransportSettings4.GatewayUserSelectedCredsSource;
        set => TransportSettings4.GatewayUserSelectedCredsSource = (uint)value;
    }
    /// <inheritdoc cref="GatewayCredSharing"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GatewayCredSharing
    {
        get => TransportSettings4.GatewayCredSharing != 0;
        set => TransportSettings4.GatewayCredSharing = value ? 1U : 0U;
    }
    /// <inheritdoc cref="GatewayHostname"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayHostname
    {
        get => TransportSettings4.GatewayHostname;
        set => TransportSettings4.GatewayHostname = value;
    }
    /// <inheritdoc cref="GatewayUsername"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayUsername
    {
        get => TransportSettings4.GatewayUsername;
        set => TransportSettings4.GatewayUsername = value;
    }
    /// <inheritdoc cref="GatewayDomain"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayDomain
    {
        get => TransportSettings4.GatewayDomain;
        set => TransportSettings4.GatewayDomain = value;
    }
    /// <inheritdoc cref="GatewayPassword"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayPassword
    {
        set => TransportSettings4.GatewayPassword = value;
    }

    #endregion

    #region ::: HyperV :::

    /// <inheritdoc cref="AuthenticationServiceClass"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AuthenticationServiceClass
    {
        get => AdvancedSettings9.AuthenticationServiceClass;
        set => AdvancedSettings9.AuthenticationServiceClass = value;
    }

    /// <inheritdoc cref="PCB"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PCB
    {
        get => AdvancedSettings9.PCB;
        set => AdvancedSettings9.PCB = value;
    }

    /// <inheritdoc cref="NegotiateSecurityLayer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NegotiateSecurityLayer
    {
        get
        {
            try { return this.GetNonScriptable5().NegotiateSecurityLayer; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "NegotiateSecurityLayer"); }
            return false;
        }
        set
        {
            try { this.GetNonScriptable5().NegotiateSecurityLayer = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "NegotiateSecurityLayer", value); }
        }
    }

    /// <inheritdoc cref="ShowConnectionInformation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowConnectionInformation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.ShowConnectionInformation, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.ShowConnectionInformation);
            return value;
        }
        set
        {
            object showConnectionInformation = value;
            if (!this.TrySetProperty(RdpClientExtensions.ShowConnectionInformation, ref showConnectionInformation, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.ShowConnectionInformation, showConnectionInformation);
        }
    }

    #endregion

    #region ::: Misc :::

    /// <inheritdoc cref="Logger"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ILogger Logger { get; set; } = DebugLoggerFactory.Create();

    /// <inheritdoc cref="DisableClickDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableClickDetection { get; set; }

    /// <inheritdoc cref="RaiseClientAreaClicked"/>
    public void RaiseClientAreaClicked() => OnClientAreaClicked?.Invoke(this, EventArgs.Empty);

    #endregion

    #region ::: MsRdpEx :::

    /// <inheritdoc cref="AxName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AxName
    {
        get => axName;
        set => axName = value;
    }

    /// <inheritdoc cref="RdpExDll"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RdpExDll
    {
        get => rdpExDll;
        set => rdpExDll = value;
    }

    #endregion
        
    #endregion

    #region --- Public Methods ---

    /// <inheritdoc cref="GetRemoteMonitorsBoundingBox"/>
    public void GetRemoteMonitorsBoundingBox(out int left, out int top, out int right, out int bottom)
    {
        try
        {
            this.GetNonScriptable5().GetRemoteMonitorsBoundingBox(out left, out top, out right, out bottom);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to call RDP client method: {MethodName}", "GetRemoteMonitorsBoundingBox");
            left = 0;
            top = 0;
            right = 0;
            bottom = 0;
        }
    }

    /// <inheritdoc cref="GetErrorDescription"/>
    public string GetErrorDescription(int disconnectReasonCode) => GetErrorDescription((uint)disconnectReasonCode, (uint)ExtendedDisconnectReason);

    /// <inheritdoc cref="LogFeatureNotSupported"/>
    public void LogFeatureNotSupported(string feature)
    {
        Logger.LogDebug("The feature '{Feature}' is not supported in Rdp Client V10", feature);
    }

    #endregion

    #region --- Message Processing ---

    /// <inheritdoc cref="WndProc"/>
    protected override void WndProc(ref Message m)
    {
        if (MessageFilter.Filter(this, ref m))
            return;
        base.WndProc(ref m);
    }
    
    #endregion
}

/// <inheritdoc cref="IRdpClient"/>
public class RdpClient12 : AxMsRdpClient11NotSafeForScripting, IRdpClient
{
    #region --- Events ---
    
    /// <inheritdoc cref="OnClientAreaClicked"/>
    public event EventHandler? OnClientAreaClicked;

    #endregion

    #region --- Rdp Settings ---

    #region ::: General :::

    // public string Server { get; set; }

    /// <inheritdoc cref="Port"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Port { get => AdvancedSettings9.RDPPort; set => AdvancedSettings9.RDPPort = value; }

    /// <inheritdoc cref="ConnectionState"/>
    public ConnectionState ConnectionState => (ConnectionState)Connected;

    #endregion

    #region ::: Credentials :::

    // public string UserName { get; set; }

    // public string Domain { get; set; }

    /// <inheritdoc cref="Password"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Password { set => AdvancedSettings9.ClearTextPassword = value; }

    /// <inheritdoc cref="NetworkLevelAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NetworkLevelAuthentication { get => AdvancedSettings9.EnableCredSspSupport; set => AdvancedSettings9.EnableCredSspSupport = value; }

    /// <inheritdoc cref="PasswordContainsSmartCardPin"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PasswordContainsSmartCardPin
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.PasswordContainsSmartcardPin, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.PasswordContainsSmartcardPin);
            return value;
        }
        set
        {
            object passwordContainsSCardPin = value;
            if (!this.TrySetProperty(RdpClientExtensions.PasswordContainsSmartcardPin, ref passwordContainsSCardPin, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.PasswordContainsSmartcardPin, passwordContainsSCardPin);
        }
    }

    #endregion

    #region ::: Display Settings :::

    // public int ColorDepth { get; set; }

    // public int DesktopWidth { get; set; }

    // public int DesktopHeight { get; set; }

    /// <inheritdoc cref="SmartSizing"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SmartSizing { get => AdvancedSettings9.SmartSizing; set => AdvancedSettings9.SmartSizing = value; }

    // public bool FullScreen { get; set; }

    // public string FullScreenTitle { set; }

    /// <inheritdoc cref="ContainerHandledFullScreen"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ContainerHandledFullScreen
    {
        get => AdvancedSettings9.ContainerHandledFullScreen;
        set => AdvancedSettings9.ContainerHandledFullScreen = value;
    }

    /// <inheritdoc cref="DisplayConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisplayConnectionBar { get => AdvancedSettings9.DisplayConnectionBar; set => AdvancedSettings9.DisplayConnectionBar = value; }

    /// <inheritdoc cref="PinConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PinConnectionBar { get => AdvancedSettings9.PinConnectionBar; set => AdvancedSettings9.PinConnectionBar = value; }

    /// <inheritdoc cref="UseMultimon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseMultimon
    {
        get
        {
            try { return this.GetNonScriptable5().UseMultimon; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "UseMultimon"); }
            return false;
        }
        set
        {
            try { this.GetNonScriptable5().UseMultimon = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "UseMultimon", value); }
        }
    }

    /// <inheritdoc cref="RemoteMonitorCount"/>
    public uint RemoteMonitorCount
    {
        get
        {
            try { return this.GetNonScriptable5().RemoteMonitorCount; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "RemoteMonitorCount"); }
            return 1;
        }
    }

    /// <inheritdoc cref="DeviceScaleFactor"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint DeviceScaleFactor
    {
        get
        {
            if (!this.TryGetProperty<uint>(RdpClientExtensions.DeviceScaleFactor, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DeviceScaleFactor);
            return value;
        }
        set
        {
            object deviceScaleFactor = value;
            if (!this.TrySetProperty(RdpClientExtensions.DeviceScaleFactor, ref deviceScaleFactor, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DeviceScaleFactor, deviceScaleFactor);
        }
    }

    /// <inheritdoc cref="DesktopScaleFactor"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint DesktopScaleFactor
    {
        get
        {
            if (!this.TryGetProperty<uint>(RdpClientExtensions.DesktopScaleFactor, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DesktopScaleFactor);
            return value;
        }
        set
        {
            object desktopScaleFactor = value;
            if (!this.TrySetProperty(RdpClientExtensions.DesktopScaleFactor, ref desktopScaleFactor, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DesktopScaleFactor, desktopScaleFactor);
        }
    }


    #endregion

    #region ::: Advanced :::

    /// <inheritdoc cref="AuthenticationLevel"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AuthenticationLevel AuthenticationLevel
    {
        get => (AuthenticationLevel)AdvancedSettings9.AuthenticationLevel;
        set => AdvancedSettings9.AuthenticationLevel = (uint)value;
    }

    /// <inheritdoc cref="Compression"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Compression
    {
        get => AdvancedSettings9.Compress > 0;
        set => AdvancedSettings9.Compress = value ? 1 : 0;
    }

    /// <inheritdoc cref="BitmapCaching"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BitmapCaching
    {
        get => AdvancedSettings9.BitmapPersistence > 0;
        set
        {
            AdvancedSettings9.BitmapPersistence = value ? 1 : 0;
            AdvancedSettings9.BitmapPeristence = value ? 1 : 0;
        }
    }

    /// <inheritdoc cref="PublicMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PublicMode
    {
        get => AdvancedSettings9.PublicMode;
        set => AdvancedSettings9.PublicMode = value;
    }

    /// <inheritdoc cref="AllowBackgroundInput"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AllowBackgroundInput
    {
        get => AdvancedSettings9.allowBackgroundInput > 0;
        set => AdvancedSettings9.allowBackgroundInput = value ? 1 : 0;
    }

    /// <inheritdoc cref="DisableUdpTransport"/>>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableUdpTransport
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.DisableUdpTransport, out var value, out var ex))
                Logger.LogWarning(ex,"Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DisableUdpTransport);
            return value;
        }
        set
        {
            object disableUdpTransport = value;
            if (!this.TrySetProperty(RdpClientExtensions.DisableUdpTransport, ref disableUdpTransport, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DisableUdpTransport, disableUdpTransport);
        }
    }

    /// <inheritdoc cref="EnableAutoReconnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableAutoReconnect
    {
        get => AdvancedSettings9.EnableAutoReconnect;
        set => AdvancedSettings9.EnableAutoReconnect = value;
    }

    /// <inheritdoc cref="MaxReconnectAttempts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MaxReconnectAttempts
    {
        get => AdvancedSettings9.MaxReconnectAttempts;
        set => AdvancedSettings9.MaxReconnectAttempts = value;
    }

    /// <inheritdoc cref="ConnectToAdministerServer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ConnectToAdministerServer
    {
        get => AdvancedSettings9.ConnectToAdministerServer;
        set => AdvancedSettings9.ConnectToAdministerServer = value;
    }

    /// <inheritdoc cref="UseRedirectionServerName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseRedirectionServerName
    {
        get
        {
            try { return this.GetPreferredRedirectionInfo().UseRedirectionServerName; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "UseRedirectionServerName"); }
            return false;
        }
        set
        {
            try { this.GetPreferredRedirectionInfo().UseRedirectionServerName = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "UseRedirectionServerName", value); }
        }
    }

    /// <inheritdoc cref="KeepAliveInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int KeepAliveInterval
    {
        get => AdvancedSettings2.keepAliveInterval;
        set => AdvancedSettings2.keepAliveInterval = value;
    }

    /// <inheritdoc cref="LoadBalanceInfo"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string LoadBalanceInfo
    {
        get => AdvancedSettings9.LoadBalanceInfo;
        set => RdpClientExtensions.SetLoadBalanceInfo(value, AdvancedSettings9);
    }

    /// <inheritdoc cref="PluginDlls"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PluginDlls { set => AdvancedSettings9.PluginDlls = value; }

    /// <inheritdoc cref="GrabFocusOnConnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GrabFocusOnConnect
    {
        get => AdvancedSettings9.GrabFocusOnConnect;
        set => AdvancedSettings9.GrabFocusOnConnect = value;
    }

    /// <inheritdoc cref="RelativeMouseMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RelativeMouseMode { get => AdvancedSettings9.RelativeMouseMode; set => AdvancedSettings9.RelativeMouseMode = value; }

    /// <inheritdoc cref="DisableCredentialsDelegation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableCredentialsDelegation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.DisableCredentialsDelegation, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.DisableCredentialsDelegation);
            return value;
        }
        set
        {
            object disableCredentialsDelegation = value;
            if (!this.TrySetProperty(RdpClientExtensions.DisableCredentialsDelegation, ref disableCredentialsDelegation, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.DisableCredentialsDelegation, disableCredentialsDelegation);
        }
    }

    /// <inheritdoc cref="RedirectedAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectedAuthentication
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.RedirectedAuthentication, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.RedirectedAuthentication);
            return value;
        }
        set
        {
            object redirectedAuthentication = value;
            if (!this.TrySetProperty(RdpClientExtensions.RedirectedAuthentication, ref redirectedAuthentication, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.RedirectedAuthentication, redirectedAuthentication);
        }
    }

    /// <inheritdoc cref="RestrictedLogon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedLogon
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.RestrictedLogon, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.RestrictedLogon);
            return value;
        }
        set
        {
            object restrictedLogon = value;
            if (!this.TrySetProperty(RdpClientExtensions.RestrictedLogon, ref restrictedLogon, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.RestrictedLogon, restrictedLogon);
        }
    }


    /// <inheritdoc cref="RemoteCredentialGuard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RemoteCredentialGuard
    {
        get => DisableCredentialsDelegation && RedirectedAuthentication;
        set
        {
            RedirectedAuthentication = value;
            DisableCredentialsDelegation = value;
        }
    }

    /// <inheritdoc cref="RestrictedAdminMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedAdminMode
    {
        get => DisableCredentialsDelegation && RestrictedLogon;
        set
        {
            RestrictedLogon = value;
            DisableCredentialsDelegation = value;
        }
    }

    /// <inheritdoc cref="EnableMouseJiggler"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableMouseJiggler
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableMouseJiggler, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableMouseJiggler);
            return value;
        }
        set
        {
            object enableMouseJiggler = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableMouseJiggler, ref enableMouseJiggler, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableMouseJiggler, enableMouseJiggler);
        }
    }

    /// <inheritdoc cref="MouseJigglerInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MouseJigglerInterval
    {
        get
        {
            if (!this.TryGetProperty<int>(RdpClientExtensions.MouseJigglerInterval, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.MouseJigglerInterval);
            return value;
        }
        set
        {
            object mouseJigglerInterval = value;
            if (!this.TrySetProperty(RdpClientExtensions.MouseJigglerInterval, ref mouseJigglerInterval, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.MouseJigglerInterval, mouseJigglerInterval);
        }
    }

    /// <inheritdoc cref="MouseJigglerMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public KeepAliveMethod MouseJigglerMethod
    {
        get
        {
            if (!this.TryGetProperty<int>(RdpClientExtensions.MouseJigglerMethod, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.MouseJigglerMethod);
            return (KeepAliveMethod)value;
        }
        set
        {
            object mouseJigglerMethod = (int)value;
            if (!this.TrySetProperty(RdpClientExtensions.MouseJigglerMethod, ref mouseJigglerMethod, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.MouseJigglerMethod, mouseJigglerMethod);
        }
    }

    /// <inheritdoc cref="EnableRdsAadAuth"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableRdsAadAuth
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableRdsAadAuth, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableRdsAadAuth);
            return value;
        }
        set
        {
            object enableRdsAadAuth = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableRdsAadAuth, ref enableRdsAadAuth, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableRdsAadAuth, enableRdsAadAuth);
        }
    }

    #endregion

    #region ::: Performance :::

    /// <inheritdoc cref="NetworkConnectionType"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint NetworkConnectionType
    {
        get => AdvancedSettings9.NetworkConnectionType;
        set => AdvancedSettings9.NetworkConnectionType = value;
    }

    /// <inheritdoc cref="PerformanceFlags"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int PerformanceFlags
    {
        get => AdvancedSettings9.PerformanceFlags;
        set => AdvancedSettings9.PerformanceFlags = value;
    }

    /// <inheritdoc cref="RedirectDirectX"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDirectX
    {
        get => AdvancedSettings9.RedirectDirectX;
        set => AdvancedSettings9.RedirectDirectX = value;
    }

    /// <inheritdoc cref="BandwidthDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BandwidthDetection
    {
        get => AdvancedSettings9.BandwidthDetection;
        set => AdvancedSettings9.BandwidthDetection = value;
    }

    /// <inheritdoc cref="EnableHardwareMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableHardwareMode
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableHardwareMode, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableHardwareMode);
            return value;
        }
        set
        {
            object enableHardwareMode = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableHardwareMode, ref enableHardwareMode, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableHardwareMode, enableHardwareMode);
        }
    }

    /// <inheritdoc cref="ClientProtocolSpec"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClientSpec ClientProtocolSpec
    {
        get => AdvancedSettings9.ClientProtocolSpec;
        set => AdvancedSettings9.ClientProtocolSpec = value;
    }

    #endregion

    #region ::: Redirection :::

    /// <inheritdoc cref="AudioRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioRedirectionMode AudioRedirectionMode
    {
        get => (AudioRedirectionMode)SecuredSettings3.AudioRedirectionMode;
        set => SecuredSettings3.AudioRedirectionMode = (int)value;
    }

    /// <inheritdoc cref="AudioQualityMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioQualityMode AudioQualityMode
    {
        get => (AudioQualityMode)AdvancedSettings8.AudioQualityMode;
        set => AdvancedSettings8.AudioQualityMode = (uint)value;
    }

    /// <inheritdoc cref="AudioCaptureRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AudioCaptureRedirectionMode
    {
        get => AdvancedSettings9.AudioCaptureRedirectionMode;
        set => AdvancedSettings9.AudioCaptureRedirectionMode = value;
    }

    /// <inheritdoc cref="VideoPlaybackMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VideoPlaybackMode VideoPlaybackMode
    {
        get => (VideoPlaybackMode)AdvancedSettings8.VideoPlaybackMode;
        set => AdvancedSettings8.VideoPlaybackMode = (uint)value;
    }

    /// <inheritdoc cref="RedirectPrinters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPrinters
    {
        get => AdvancedSettings9.RedirectPrinters;
        set => AdvancedSettings9.RedirectPrinters = value;
    }

    /// <inheritdoc cref="RedirectClipboard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectClipboard
    {
        get => AdvancedSettings9.RedirectClipboard;
        set => AdvancedSettings9.RedirectClipboard = value;
    }

    /// <inheritdoc cref="RedirectSmartCards"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectSmartCards
    {
        get => AdvancedSettings9.RedirectSmartCards;
        set => AdvancedSettings9.RedirectSmartCards = value;
    }
    /// <inheritdoc cref="RedirectPorts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPorts
    {
        get => AdvancedSettings9.RedirectPorts;
        set => AdvancedSettings9.RedirectPorts = value;
    }

    /// <inheritdoc cref="RedirectDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDevices
    {
        get => AdvancedSettings9.RedirectDevices;
        set => AdvancedSettings9.RedirectDevices = value;
    }

    /// <inheritdoc cref="RedirectPOSDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPOSDevices
    {
        get => AdvancedSettings9.RedirectPOSDevices;
        set => AdvancedSettings9.RedirectPOSDevices = value;
    }

    /// <inheritdoc cref="RedirectDrives"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDrives
    {
        get => AdvancedSettings9.RedirectDrives;
        set => AdvancedSettings9.RedirectDrives = value;
    }

    private string _redirectDriveLetters = string.Empty;
        
    /// <inheritdoc cref="RedirectDriveLetters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RedirectDriveLetters
    {
        get => _redirectDriveLetters;
        set
        {
            _redirectDriveLetters = value;
            if (RedirectDrives && !this.SetupDriveRedirection(_redirectDriveLetters, out var ex))
            {
                Logger.LogWarning(ex, "One or more errors occurred during drive redirection");
            }
        }
    }

    private bool _redirectCameras;
    /// <inheritdoc cref="RedirectCameras"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectCameras 
    {
        get => _redirectCameras;
        set
        {
            _redirectCameras = value;
            if (!this.SetupCameraRedirection(_redirectCameras, out var ex))
            {
                Logger.LogWarning(ex, "One or more errors occurred during camera redirection");
            }
        }
    }

    /// <inheritdoc cref="RedirectLocation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectLocation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.EnableLocationRedirection, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.EnableLocationRedirection);
            return value;
        }
        set
        {
            object enableLocationRedirect = value;
            if (!this.TrySetProperty(RdpClientExtensions.EnableLocationRedirection, ref enableLocationRedirect, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.EnableLocationRedirection, enableLocationRedirect);
        }
    }

    #endregion

    #region ::: Keyboard :::

    /// <inheritdoc cref="AcceleratorPassthrough"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AcceleratorPassthrough
    {
        get => AdvancedSettings9.AcceleratorPassthrough != 0;
        set => AdvancedSettings9.AcceleratorPassthrough = value ? 1 : 0;
    }

    /// <inheritdoc cref="EnableWindowsKey"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableWindowsKey
    {
        get => AdvancedSettings9.EnableWindowsKey != 0;
        set => AdvancedSettings9.EnableWindowsKey = value ? 1 : 0;
    }

    /// <inheritdoc cref="KeyboardHookMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int KeyboardHookMode
    {
        get => SecuredSettings3.KeyboardHookMode;
        set => SecuredSettings3.KeyboardHookMode = value;
    }

    /// <inheritdoc cref="KeyBoardLayoutStr"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string KeyBoardLayoutStr
    {
        set => AdvancedSettings9.KeyBoardLayoutStr = value;
    }

    #endregion

    #region ::: Program :::

    /// <inheritdoc cref="StartProgram"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string StartProgram
    {
        get => SecuredSettings3.StartProgram;
        set => SecuredSettings3.StartProgram = value;
    }

    /// <inheritdoc cref="WorkDir"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string WorkDir
    {
        get => SecuredSettings3.WorkDir;
        set => SecuredSettings3.WorkDir = value;
    }

    /// <inheritdoc cref="MaximizeShell"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool MaximizeShell
    {
        get => AdvancedSettings9.MaximizeShell != 0;
        set => AdvancedSettings9.MaximizeShell = value ? 1 : 0;
    }

    #endregion

    #region ::: Gateway :::

    /// <inheritdoc cref="GatewayUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayUsageMethod GatewayUsageMethod 
    {
        get => (GatewayUsageMethod)TransportSettings4.GatewayUsageMethod;
        set => TransportSettings4.GatewayUsageMethod = (uint) value;
    }
    /// <inheritdoc cref="GatewayProfileUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayProfileUsageMethod GatewayProfileUsageMethod
    {
        get => (GatewayProfileUsageMethod)TransportSettings4.GatewayProfileUsageMethod;
        set => TransportSettings4.GatewayProfileUsageMethod = (uint)value;
    }
    /// <inheritdoc cref="GatewayCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayCredsSource
    {
        get => (GatewayCredentialSource)TransportSettings4.GatewayCredsSource;
        set => TransportSettings4.GatewayCredsSource = (uint)value;
    }
    /// <inheritdoc cref="GatewayUserSelectedCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayUserSelectedCredsSource
    {
        get => (GatewayCredentialSource)TransportSettings4.GatewayUserSelectedCredsSource;
        set => TransportSettings4.GatewayUserSelectedCredsSource = (uint)value;
    }
    /// <inheritdoc cref="GatewayCredSharing"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GatewayCredSharing
    {
        get => TransportSettings4.GatewayCredSharing != 0;
        set => TransportSettings4.GatewayCredSharing = value ? 1U : 0U;
    }
    /// <inheritdoc cref="GatewayHostname"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayHostname
    {
        get => TransportSettings4.GatewayHostname;
        set => TransportSettings4.GatewayHostname = value;
    }
    /// <inheritdoc cref="GatewayUsername"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayUsername
    {
        get => TransportSettings4.GatewayUsername;
        set => TransportSettings4.GatewayUsername = value;
    }
    /// <inheritdoc cref="GatewayDomain"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayDomain
    {
        get => TransportSettings4.GatewayDomain;
        set => TransportSettings4.GatewayDomain = value;
    }
    /// <inheritdoc cref="GatewayPassword"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayPassword
    {
        set => TransportSettings4.GatewayPassword = value;
    }

    #endregion

    #region ::: HyperV :::

    /// <inheritdoc cref="AuthenticationServiceClass"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AuthenticationServiceClass
    {
        get => AdvancedSettings9.AuthenticationServiceClass;
        set => AdvancedSettings9.AuthenticationServiceClass = value;
    }

    /// <inheritdoc cref="PCB"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PCB
    {
        get => AdvancedSettings9.PCB;
        set => AdvancedSettings9.PCB = value;
    }

    /// <inheritdoc cref="NegotiateSecurityLayer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NegotiateSecurityLayer
    {
        get
        {
            try { return this.GetNonScriptable5().NegotiateSecurityLayer; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", "NegotiateSecurityLayer"); }
            return false;
        }
        set
        {
            try { this.GetNonScriptable5().NegotiateSecurityLayer = value; }
            catch (Exception ex) { Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", "NegotiateSecurityLayer", value); }
        }
    }

    /// <inheritdoc cref="ShowConnectionInformation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowConnectionInformation
    {
        get
        {
            if (!this.TryGetProperty<bool>(RdpClientExtensions.ShowConnectionInformation, out var value, out var ex))
                Logger.LogWarning(ex, "Failed to get RDP client property: {PropertyName}", RdpClientExtensions.ShowConnectionInformation);
            return value;
        }
        set
        {
            object showConnectionInformation = value;
            if (!this.TrySetProperty(RdpClientExtensions.ShowConnectionInformation, ref showConnectionInformation, out var ex))
                Logger.LogWarning(ex, "Failed to set RDP client property: {PropertyName} to {PropertyValue}", RdpClientExtensions.ShowConnectionInformation, showConnectionInformation);
        }
    }

    #endregion

    #region ::: Misc :::

    /// <inheritdoc cref="Logger"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ILogger Logger { get; set; } = DebugLoggerFactory.Create();

    /// <inheritdoc cref="DisableClickDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableClickDetection { get; set; }

    /// <inheritdoc cref="RaiseClientAreaClicked"/>
    public void RaiseClientAreaClicked() => OnClientAreaClicked?.Invoke(this, EventArgs.Empty);

    #endregion

    #region ::: MsRdpEx :::

    /// <inheritdoc cref="AxName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AxName
    {
        get => axName;
        set => axName = value;
    }

    /// <inheritdoc cref="RdpExDll"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RdpExDll
    {
        get => rdpExDll;
        set => rdpExDll = value;
    }

    #endregion
        
    #endregion

    #region --- Public Methods ---

    /// <inheritdoc cref="GetRemoteMonitorsBoundingBox"/>
    public void GetRemoteMonitorsBoundingBox(out int left, out int top, out int right, out int bottom)
    {
        try
        {
            this.GetNonScriptable5().GetRemoteMonitorsBoundingBox(out left, out top, out right, out bottom);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to call RDP client method: {MethodName}", "GetRemoteMonitorsBoundingBox");
            left = 0;
            top = 0;
            right = 0;
            bottom = 0;
        }
    }

    /// <inheritdoc cref="GetErrorDescription"/>
    public string GetErrorDescription(int disconnectReasonCode) => GetErrorDescription((uint)disconnectReasonCode, (uint)ExtendedDisconnectReason);

    /// <inheritdoc cref="LogFeatureNotSupported"/>
    public void LogFeatureNotSupported(string feature)
    {
        Logger.LogDebug("The feature '{Feature}' is not supported in Rdp Client V10", feature);
    }

    #endregion

    #region --- Message Processing ---

    /// <inheritdoc cref="WndProc"/>
    protected override void WndProc(ref Message m)
    {
        if (MessageFilter.Filter(this, ref m))
            return;
        base.WndProc(ref m);
    }
    
    #endregion
}
