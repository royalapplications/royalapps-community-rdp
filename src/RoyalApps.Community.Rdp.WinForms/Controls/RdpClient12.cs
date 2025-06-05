using System;
using System.Windows.Forms;
using AxMSTSCLib;
using Microsoft.Extensions.Logging;
using MSTSCLib;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Interfaces;
using RoyalApps.Community.Rdp.WinForms.Logging;
using System.ComponentModel;

namespace RoyalApps.Community.Rdp.WinForms.Controls;

/// <inheritdoc cref="IRdpClient"/>
public class RdpClient12 : AxMsRdpClient11NotSafeForScripting, IRdpClient
{
    private bool _redirectCameras;
    private string _redirectDriveLetters = string.Empty;

    /// <summary>
    /// Provides access to IMsRdpClientAdvancedSettings8 AdvancedSettings9
    /// </summary>
    private IMsRdpClientAdvancedSettings8 RdpAdvancedSettings => AdvancedSettings9;

    /// <summary>
    /// Provides access to IMsRdpClientNonScriptable7
    /// </summary>
    private IMsRdpClientNonScriptable7 RdpClientNonScriptable => (IMsRdpClientNonScriptable7)GetOcx()!;

    /// <summary>
    /// Provides access to IMsRdpClientSecuredSettings2 SecuredSettings3
    /// </summary>
    private IMsRdpClientSecuredSettings2 RdpSecuredSettings => SecuredSettings3;

    /// <summary>
    /// Provides access to IMsRdpClientTransportSettings2 TransportSettings2
    /// </summary>
    private IMsRdpClientTransportSettings4 RdpTransportSettings => TransportSettings4;

    /// <inheritdoc cref="AcceleratorPassthrough"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AcceleratorPassthrough { get => RdpAdvancedSettings.AcceleratorPassthrough != 0; set => RdpAdvancedSettings.AcceleratorPassthrough = value ? 1 : 0; }

    /// <inheritdoc cref="AllowBackgroundInput"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AllowBackgroundInput { get => RdpAdvancedSettings.allowBackgroundInput > 0; set => RdpAdvancedSettings.allowBackgroundInput = value ? 1 : 0; }

    /// <inheritdoc cref="AudioCaptureRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AudioCaptureRedirectionMode { get => RdpAdvancedSettings.AudioCaptureRedirectionMode; set => RdpAdvancedSettings.AudioCaptureRedirectionMode = value; }

    /// <inheritdoc cref="AudioQualityMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioQualityMode AudioQualityMode { get => (AudioQualityMode)RdpAdvancedSettings.AudioQualityMode; set => RdpAdvancedSettings.AudioQualityMode = (uint)value; }

    /// <inheritdoc cref="AudioRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioRedirectionMode AudioRedirectionMode { get => (AudioRedirectionMode)RdpSecuredSettings.AudioRedirectionMode; set => RdpSecuredSettings.AudioRedirectionMode = (int)value; }

    /// <inheritdoc cref="AuthenticationLevel"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AuthenticationLevel AuthenticationLevel { get => (AuthenticationLevel)RdpAdvancedSettings.AuthenticationLevel; set => RdpAdvancedSettings.AuthenticationLevel = (uint)value; }

    /// <inheritdoc cref="AuthenticationServiceClass"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AuthenticationServiceClass { get => RdpAdvancedSettings.AuthenticationServiceClass; set => RdpAdvancedSettings.AuthenticationServiceClass = value; }

    /// <inheritdoc cref="AxName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AxName { get => axName; set => axName = value; }

    /// <inheritdoc cref="BandwidthDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BandwidthDetection { get => RdpAdvancedSettings.BandwidthDetection; set => RdpAdvancedSettings.BandwidthDetection = value; }

    /// <inheritdoc cref="BitmapCaching"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BitmapCaching { get => RdpAdvancedSettings.BitmapPersistence > 0; set => RdpAdvancedSettings.BitmapPersistence = value ? 1 : 0; }

    /// <inheritdoc cref="ClientProtocolSpec"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClientSpec ClientProtocolSpec { get => RdpAdvancedSettings.ClientProtocolSpec; set => RdpAdvancedSettings.ClientProtocolSpec = value; }

    /// <inheritdoc cref="Compression"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Compression { get => RdpAdvancedSettings.Compress > 0; set => RdpAdvancedSettings.Compress = value ? 1 : 0; }

    /// <inheritdoc cref="ConnectionState"/>
    public ConnectionState ConnectionState => (ConnectionState)Connected;

    /// <inheritdoc cref="ConnectToAdministerServer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ConnectToAdministerServer { get => RdpAdvancedSettings.ConnectToAdministerServer; set => RdpAdvancedSettings.ConnectToAdministerServer = value; }

    /// <inheritdoc cref="ContainerHandledFullScreen"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ContainerHandledFullScreen { get => RdpAdvancedSettings.ContainerHandledFullScreen; set => RdpAdvancedSettings.ContainerHandledFullScreen = value; }

    /// <inheritdoc cref="DesktopScaleFactor"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint DesktopScaleFactor { get => this.GetProperty<uint>(RdpProperties.DesktopScaleFactor); set => this.SetProperty(RdpProperties.DesktopScaleFactor, value); }

    /// <inheritdoc cref="DeviceScaleFactor"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint DeviceScaleFactor { get => this.GetProperty<uint>(RdpProperties.DeviceScaleFactor); set => this.SetProperty(RdpProperties.DeviceScaleFactor, value); }

    /// <inheritdoc cref="DisableClickDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableClickDetection { get; set; }

    /// <inheritdoc cref="DisableCredentialsDelegation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableCredentialsDelegation { get => this.GetProperty<bool>(RdpProperties.DisableCredentialsDelegation); set => this.SetProperty(RdpProperties.DisableCredentialsDelegation, value); }

    /// <inheritdoc cref="DisableUdpTransport"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableUdpTransport { get => this.GetProperty<bool>(RdpProperties.DisableUdpTransport); set => this.SetProperty(RdpProperties.DisableUdpTransport, value); }

    /// <inheritdoc cref="DisplayConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisplayConnectionBar { get => RdpAdvancedSettings.DisplayConnectionBar; set => RdpAdvancedSettings.DisplayConnectionBar = value; }

    /// <inheritdoc cref="EnableAutoReconnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableAutoReconnect { get => RdpAdvancedSettings.EnableAutoReconnect; set => RdpAdvancedSettings.EnableAutoReconnect = value; }

    /// <inheritdoc cref="EnableHardwareMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableHardwareMode { get => this.GetProperty<bool>(RdpProperties.EnableHardwareMode); set => this.SetProperty(RdpProperties.EnableHardwareMode, value); }

    /// <inheritdoc cref="EnableMouseJiggler"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableMouseJiggler { get => this.GetProperty<bool>(RdpProperties.EnableMouseJiggler); set => this.SetProperty(RdpProperties.EnableMouseJiggler, value); }

    /// <inheritdoc cref="EnableRdsAadAuth"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableRdsAadAuth { get => this.GetProperty<bool>(RdpProperties.EnableRdsAadAuth); set => this.SetProperty(RdpProperties.EnableRdsAadAuth, value); }

    /// <inheritdoc cref="EnableWindowsKey"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableWindowsKey { get => RdpAdvancedSettings.EnableWindowsKey != 0; set => RdpAdvancedSettings.EnableWindowsKey = value ? 1 : 0; }

    /// <inheritdoc cref="GatewayCredSharing"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GatewayCredSharing { get => RdpTransportSettings.GatewayCredSharing != 0; set => RdpTransportSettings.GatewayCredSharing = value ? 1U : 0U; }

    /// <inheritdoc cref="GatewayCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayCredsSource { get => (GatewayCredentialSource)RdpTransportSettings.GatewayCredsSource; set => RdpTransportSettings.GatewayCredsSource = (uint)value; }

    /// <inheritdoc cref="GatewayDomain"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayDomain { get => RdpTransportSettings.GatewayDomain; set => RdpTransportSettings.GatewayDomain = value; }

    /// <inheritdoc cref="GatewayHostname"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayHostname { get => RdpTransportSettings.GatewayHostname; set => RdpTransportSettings.GatewayHostname = value; }

    /// <inheritdoc cref="GatewayPassword"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayPassword { set => RdpTransportSettings.GatewayPassword = value; }

    /// <inheritdoc cref="GatewayProfileUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayProfileUsageMethod GatewayProfileUsageMethod { get => (GatewayProfileUsageMethod)RdpTransportSettings.GatewayProfileUsageMethod; set => RdpTransportSettings.GatewayProfileUsageMethod = (uint)value; }

    /// <inheritdoc cref="GatewayUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayUsageMethod GatewayUsageMethod { get => (GatewayUsageMethod)RdpTransportSettings.GatewayUsageMethod; set => RdpTransportSettings.GatewayUsageMethod = (uint) value; }

    /// <inheritdoc cref="GatewayUsername"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayUsername { get => RdpTransportSettings.GatewayUsername; set => RdpTransportSettings.GatewayUsername = value; }

    /// <inheritdoc cref="GatewayUserSelectedCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayUserSelectedCredsSource { get => (GatewayCredentialSource)RdpTransportSettings.GatewayUserSelectedCredsSource; set => RdpTransportSettings.GatewayUserSelectedCredsSource = (uint)value; }

    /// <inheritdoc cref="GrabFocusOnConnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GrabFocusOnConnect { get => RdpAdvancedSettings.GrabFocusOnConnect; set => RdpAdvancedSettings.GrabFocusOnConnect = value; }

    /// <inheritdoc cref="KeepAliveInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int KeepAliveInterval { get => RdpAdvancedSettings.keepAliveInterval; set => RdpAdvancedSettings.keepAliveInterval = value; }

    /// <inheritdoc cref="KeyboardHookMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int KeyboardHookMode { get => RdpSecuredSettings.KeyboardHookMode; set => RdpSecuredSettings.KeyboardHookMode = value; }

    /// <inheritdoc cref="KeyboardHookToggleShortcutEnabled"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool KeyboardHookToggleShortcutEnabled
    {
        get => this.GetProperty<bool>(RdpProperties.KeyboardHookToggleShortcutEnabled);
        set
        {
            this.SetProperty(RdpProperties.KeyboardHookToggleShortcutEnabled, value);
            this.SetProperty(RdpProperties.KeyboardHookToggleShortcutKey, "space");
        }
    }

    /// <inheritdoc cref="KeyBoardLayoutStr"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string KeyBoardLayoutStr { set => RdpAdvancedSettings.KeyBoardLayoutStr = value; }

    /// <inheritdoc cref="LoadBalanceInfo"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string LoadBalanceInfo { get => RdpAdvancedSettings.LoadBalanceInfo; set => RdpClientExtensions.SetLoadBalanceInfo(value, RdpAdvancedSettings); }

    /// <inheritdoc cref="Logger"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ILogger Logger { get; set; } = DebugLoggerFactory.Create();

    /// <inheritdoc cref="MaximizeShell"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool MaximizeShell { get => RdpAdvancedSettings.MaximizeShell != 0; set => RdpAdvancedSettings.MaximizeShell = value ? 1 : 0; }

    /// <inheritdoc cref="MaxReconnectAttempts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MaxReconnectAttempts { get => RdpAdvancedSettings.MaxReconnectAttempts; set => RdpAdvancedSettings.MaxReconnectAttempts = value; }

    /// <inheritdoc cref="MouseJigglerInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MouseJigglerInterval { get => this.GetProperty<int>(RdpProperties.MouseJigglerInterval); set => this.SetProperty(RdpProperties.MouseJigglerInterval, value); }

    /// <inheritdoc cref="MouseJigglerMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public KeepAliveMethod MouseJigglerMethod { get => this.GetProperty<KeepAliveMethod>(RdpProperties.MouseJigglerMethod); set => this.SetProperty(RdpProperties.MouseJigglerMethod, value); }

    /// <inheritdoc cref="NegotiateSecurityLayer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NegotiateSecurityLayer
    {
        get
        {
            try { return RdpClientNonScriptable.NegotiateSecurityLayer; }
            catch (Exception ex) { this.LogPropertyGetFailed(ex, nameof(NegotiateSecurityLayer)); }
            return false;
        }
        set
        {
            try { RdpClientNonScriptable.NegotiateSecurityLayer = value; }
            catch (Exception ex) { this.LogPropertySetFailed(ex, nameof(NegotiateSecurityLayer), value); }
        }
    }

    /// <inheritdoc cref="NetworkConnectionType"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint NetworkConnectionType { get => RdpAdvancedSettings.NetworkConnectionType; set => RdpAdvancedSettings.NetworkConnectionType = value; }

    /// <inheritdoc cref="NetworkLevelAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NetworkLevelAuthentication { get => RdpAdvancedSettings.EnableCredSspSupport; set => RdpAdvancedSettings.EnableCredSspSupport = value; }

    /// <inheritdoc cref="Password"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Password { set => RdpAdvancedSettings.ClearTextPassword = value; }

    /// <inheritdoc cref="PasswordContainsSmartCardPin"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PasswordContainsSmartCardPin { get => this.GetProperty<bool>(RdpProperties.PasswordContainsSmartcardPin); set => this.SetProperty(RdpProperties.PasswordContainsSmartcardPin, value); }

    /// <inheritdoc cref="PCB"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PCB { get => RdpAdvancedSettings.PCB; set => RdpAdvancedSettings.PCB = value; }

    /// <inheritdoc cref="PerformanceFlags"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int PerformanceFlags { get => RdpAdvancedSettings.PerformanceFlags; set => RdpAdvancedSettings.PerformanceFlags = value; }

    /// <inheritdoc cref="PinConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PinConnectionBar { get => RdpAdvancedSettings.PinConnectionBar; set => RdpAdvancedSettings.PinConnectionBar = value; }

    /// <inheritdoc cref="PluginDlls"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PluginDlls { set => RdpAdvancedSettings.PluginDlls = value; }

    /// <inheritdoc cref="Port"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Port { get => RdpAdvancedSettings.RDPPort; set => RdpAdvancedSettings.RDPPort = value; }

    /// <inheritdoc cref="PublicMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PublicMode { get => RdpAdvancedSettings.PublicMode; set => RdpAdvancedSettings.PublicMode = value; }

    /// <inheritdoc cref="RdpExDll"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RdpExDll { get => rdpExDll; set => rdpExDll = value; }

    /// <inheritdoc cref="RedirectCameras"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectCameras
    {
        get => _redirectCameras;
        set
        {
            _redirectCameras = value;
            this.SetCameraRedirection(RdpClientNonScriptable, value);
        }
    }

    /// <inheritdoc cref="RedirectClipboard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectClipboard { get => RdpAdvancedSettings.RedirectClipboard; set => RdpAdvancedSettings.RedirectClipboard = value; }

    /// <inheritdoc cref="RedirectDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDevices { get => RdpAdvancedSettings.RedirectDevices; set => RdpAdvancedSettings.RedirectDevices = value; }

    /// <inheritdoc cref="RedirectDirectX"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDirectX { get => RdpAdvancedSettings.RedirectDirectX; set => RdpAdvancedSettings.RedirectDirectX = value; }

    /// <inheritdoc cref="RedirectDriveLetters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RedirectDriveLetters
    {
        get => _redirectDriveLetters;
        set
        {
            _redirectDriveLetters = value;
            this.SetDriveRedirection(RdpClientNonScriptable, _redirectDriveLetters);
        }
    }

    /// <inheritdoc cref="RedirectDrives"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDrives { get => RdpAdvancedSettings.RedirectDrives; set => RdpAdvancedSettings.RedirectDrives = value; }

    /// <inheritdoc cref="RedirectedAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectedAuthentication { get => this.GetProperty<bool>(RdpProperties.RedirectedAuthentication); set => this.SetProperty(RdpProperties.RedirectedAuthentication, value); }

    /// <inheritdoc cref="RedirectLocation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectLocation { get => this.GetProperty<bool>(RdpProperties.EnableLocationRedirection); set => this.SetProperty(RdpProperties.EnableLocationRedirection, value); }

    /// <inheritdoc cref="RedirectPorts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPorts { get => RdpAdvancedSettings.RedirectPorts; set => RdpAdvancedSettings.RedirectPorts = value; }

    /// <inheritdoc cref="RedirectPOSDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPOSDevices { get => RdpAdvancedSettings.RedirectPOSDevices; set => RdpAdvancedSettings.RedirectPOSDevices = value; }

    /// <inheritdoc cref="RedirectPrinters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPrinters { get => RdpAdvancedSettings.RedirectPrinters; set => RdpAdvancedSettings.RedirectPrinters = value; }

    /// <inheritdoc cref="RedirectSmartCards"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectSmartCards { get => RdpAdvancedSettings.RedirectSmartCards; set => RdpAdvancedSettings.RedirectSmartCards = value; }

    /// <inheritdoc cref="RelativeMouseMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RelativeMouseMode { get => RdpAdvancedSettings.RelativeMouseMode; set => RdpAdvancedSettings.RelativeMouseMode = value; }

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

    /// <inheritdoc cref="RemoteMonitorCount"/>
    public uint RemoteMonitorCount
    {
        get
        {
            try { return RdpClientNonScriptable.RemoteMonitorCount; }
            catch (Exception ex) { this.LogPropertyGetFailed(ex, nameof(RemoteMonitorCount)); }
            return 1;
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

    /// <inheritdoc cref="RestrictedLogon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedLogon { get => this.GetProperty<bool>(RdpProperties.RestrictedLogon); set => this.SetProperty(RdpProperties.RestrictedLogon, value); }

    /// <inheritdoc cref="ShowConnectionInformation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowConnectionInformation { get => this.GetProperty<bool>(RdpProperties.ShowConnectionInformation); set => this.SetProperty(RdpProperties.ShowConnectionInformation, value); }

    /// <inheritdoc cref="SmartSizing"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SmartSizing { get => RdpAdvancedSettings.SmartSizing; set => RdpAdvancedSettings.SmartSizing = value; }

    /// <inheritdoc cref="StartProgram"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string StartProgram { get => RdpSecuredSettings.StartProgram; set => RdpSecuredSettings.StartProgram = value; }

    /// <inheritdoc cref="UseMultimon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseMultimon
    {
        get
        {
            try { return RdpClientNonScriptable.UseMultimon; }
            catch (Exception ex) { this.LogPropertyGetFailed(ex, nameof(UseMultimon)); }
            return false;
        }
        set
        {
            try { RdpClientNonScriptable.UseMultimon = value; }
            catch (Exception ex) { this.LogPropertySetFailed(ex, nameof(UseMultimon), value); }
        }
    }

    /// <inheritdoc cref="UseRedirectionServerName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseRedirectionServerName { get => this.GetProperty<bool>(RdpProperties.UseRedirectionServerName); set => this.SetProperty(RdpProperties.UseRedirectionServerName, value); }

    /// <inheritdoc cref="VideoPlaybackMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VideoPlaybackMode VideoPlaybackMode { get => (VideoPlaybackMode)RdpAdvancedSettings.VideoPlaybackMode; set => RdpAdvancedSettings.VideoPlaybackMode = (uint)value; }

    /// <inheritdoc cref="WorkDir"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string WorkDir { get => RdpSecuredSettings.WorkDir; set => RdpSecuredSettings.WorkDir = value; }

    /// <inheritdoc cref="GetErrorDescription"/>
    public string GetErrorDescription(int disconnectReasonCode) => GetErrorDescription((uint)disconnectReasonCode, (uint)ExtendedDisconnectReason);

    /// <inheritdoc cref="GetRemoteMonitorsBoundingBox"/>
    public void GetRemoteMonitorsBoundingBox(out int left, out int top, out int right, out int bottom)
    {
        left = 0;
        top = 0;
        right = 0;
        bottom = 0;

        try { RdpClientNonScriptable.GetRemoteMonitorsBoundingBox(out left, out top, out right, out bottom); }
        catch (Exception ex) { this.LogMethodFailed(ex, nameof(GetRemoteMonitorsBoundingBox)); }
    }

    /// <inheritdoc cref="OnClientAreaClicked"/>
    public event EventHandler? OnClientAreaClicked;

    /// <inheritdoc cref="RaiseClientAreaClicked"/>
    public void RaiseClientAreaClicked() => OnClientAreaClicked?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc cref="WndProc"/>
    protected override void WndProc(ref Message m)
    {
        if (MessageFilter.Filter(this, ref m))
            return;
        base.WndProc(ref m);
    }
}
