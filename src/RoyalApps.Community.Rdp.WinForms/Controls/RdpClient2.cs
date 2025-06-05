using System;
using System.ComponentModel;
using System.Windows.Forms;
using AxMSTSCLib;
using Microsoft.Extensions.Logging;
using MSTSCLib;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Interfaces;
using RoyalApps.Community.Rdp.WinForms.Logging;

namespace RoyalApps.Community.Rdp.WinForms.Controls;

/// <inheritdoc cref="IRdpClient"/>
public class RdpClient2 : AxMsRdpClientNotSafeForScripting, IRdpClient
{
    private string _redirectDriveLetters = string.Empty;

    /// <summary>
    /// Provides access to IMsRdpClientAdvancedSettings AdvancedSettings2
    /// </summary>
    private IMsRdpClientAdvancedSettings RdpAdvancedSettings => AdvancedSettings2;

    /// <summary>
    /// Provides access to IMsRdpClientNonScriptable3
    /// </summary>
    private IMsRdpClientNonScriptable3 RdpClientNonScriptable => (IMsRdpClientNonScriptable3)GetOcx()!;

    /// <summary>
    /// Provides access to IMsRdpClientSecuredSettings SecuredSettings2
    /// </summary>
    private IMsRdpClientSecuredSettings RdpSecuredSettings => SecuredSettings2;

    /// <inheritdoc cref="AcceleratorPassthrough"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AcceleratorPassthrough { get => RdpAdvancedSettings.AcceleratorPassthrough != 0; set => RdpAdvancedSettings.AcceleratorPassthrough = value ? 1 : 0; }

    /// <inheritdoc cref="AllowBackgroundInput"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AllowBackgroundInput { get => RdpAdvancedSettings.allowBackgroundInput > 0; set => RdpAdvancedSettings.allowBackgroundInput = value ? 1 : 0; }

    /// <inheritdoc cref="AudioCaptureRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AudioCaptureRedirectionMode { get => false; set => this.LogPropertyNotSupported(nameof(AudioCaptureRedirectionMode), value); }

    /// <inheritdoc cref="AudioQualityMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioQualityMode AudioQualityMode { get => AudioQualityMode.Dynamic; set => this.LogPropertyNotSupported(nameof(AudioQualityMode), value); }

    /// <inheritdoc cref="AudioRedirectionMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioRedirectionMode AudioRedirectionMode { get => AudioRedirectionMode.RedirectToClient; set => this.LogPropertyNotSupported(nameof(AudioRedirectionMode), value); }

    /// <inheritdoc cref="AuthenticationLevel"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AuthenticationLevel AuthenticationLevel { get => AuthenticationLevel.NoAuthenticationOfServer; set => this.LogPropertyNotSupported(nameof(AuthenticationLevel), value); }

    /// <inheritdoc cref="AuthenticationServiceClass"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AuthenticationServiceClass { get => string.Empty; set => this.LogPropertyNotSupported(nameof(AuthenticationServiceClass), value); }

    /// <inheritdoc cref="AxName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AxName { get => axName; set => axName = value; }

    /// <inheritdoc cref="BandwidthDetection"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BandwidthDetection { get => false; set => this.LogPropertyNotSupported(nameof(BandwidthDetection), value); }

    /// <inheritdoc cref="BitmapCaching"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BitmapCaching { get => RdpAdvancedSettings.BitmapPersistence > 0; set => RdpAdvancedSettings.BitmapPersistence = value ? 1 : 0; }

    /// <inheritdoc cref="ClientProtocolSpec"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClientSpec ClientProtocolSpec { get => ClientSpec.FullMode; set => this.LogPropertyNotSupported(nameof(ClientProtocolSpec), value); }

    /// <inheritdoc cref="Compression"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Compression { get => RdpAdvancedSettings.Compress > 0; set => RdpAdvancedSettings.Compress = value ? 1 : 0; }

    /// <inheritdoc cref="ConnectionState"/>
    public ConnectionState ConnectionState => (ConnectionState)Connected;

    /// <inheritdoc cref="ConnectToAdministerServer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ConnectToAdministerServer { get => RdpAdvancedSettings.ConnectToServerConsole; set => RdpAdvancedSettings.ConnectToServerConsole = value; }

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
    public bool DisableCredentialsDelegation { get => false; set => this.LogPropertyNotSupported(nameof(DisableCredentialsDelegation), value); }

    /// <inheritdoc cref="DisableUdpTransport"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisableUdpTransport { get => this.GetProperty<bool>(RdpProperties.DisableUdpTransport); set => this.SetProperty(RdpProperties.DisableUdpTransport, value); }

    /// <inheritdoc cref="DisplayConnectionBar"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DisplayConnectionBar { get => RdpAdvancedSettings.DisplayConnectionBar; set => RdpAdvancedSettings.DisplayConnectionBar = value; }

    /// <inheritdoc cref="EnableAutoReconnect"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableAutoReconnect { get => false; set => this.LogPropertyNotSupported(nameof(EnableAutoReconnect), value); }

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
    public bool GatewayCredSharing { get => false; set => this.LogPropertyNotSupported(nameof(GatewayCredSharing), value); }

    /// <inheritdoc cref="GatewayCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayCredsSource { get => GatewayCredentialSource.Any; set => this.LogPropertyNotSupported(nameof(GatewayCredsSource), value); }

    /// <inheritdoc cref="GatewayDomain"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayDomain { get => string.Empty; set => this.LogPropertyNotSupported(nameof(GatewayDomain), value); }

    /// <inheritdoc cref="GatewayHostname"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayHostname { get => string.Empty; set => this.LogPropertyNotSupported(nameof(GatewayHostname), value); }

    /// <inheritdoc cref="GatewayPassword"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayPassword { set => this.LogPropertyNotSupported(nameof(GatewayPassword), value); }

    /// <inheritdoc cref="GatewayProfileUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayProfileUsageMethod GatewayProfileUsageMethod { get => GatewayProfileUsageMethod.Default; set => this.LogPropertyNotSupported(nameof(GatewayProfileUsageMethod), value); }

    /// <inheritdoc cref="GatewayUsageMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayUsageMethod GatewayUsageMethod { get => GatewayUsageMethod.Never; set => this.LogPropertyNotSupported(nameof(GatewayUsageMethod), value); }

    /// <inheritdoc cref="GatewayUsername"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string GatewayUsername { get => string.Empty; set => this.LogPropertyNotSupported(nameof(GatewayUsername), value); }

    /// <inheritdoc cref="GatewayUserSelectedCredsSource"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GatewayCredentialSource GatewayUserSelectedCredsSource { get => GatewayCredentialSource.Any; set => this.LogPropertyNotSupported(nameof(GatewayUserSelectedCredsSource), value); }

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
    public int MaxReconnectAttempts { get => 0; set => this.LogPropertyNotSupported(nameof(MaxReconnectAttempts), value); }

    /// <inheritdoc cref="MouseJigglerInterval"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MouseJigglerInterval { get => this.GetProperty<int>(RdpProperties.MouseJigglerInterval); set => this.SetProperty(RdpProperties.MouseJigglerInterval, value); }

    /// <inheritdoc cref="MouseJigglerMethod"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public KeepAliveMethod MouseJigglerMethod { get => this.GetProperty<KeepAliveMethod>(RdpProperties.MouseJigglerMethod); set => this.SetProperty(RdpProperties.MouseJigglerMethod, value); }

    /// <inheritdoc cref="NegotiateSecurityLayer"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NegotiateSecurityLayer { get => false; set => this.LogPropertyNotSupported(nameof(NegotiateSecurityLayer), value); }

    /// <inheritdoc cref="NetworkConnectionType"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public uint NetworkConnectionType { get => 0; set => this.LogPropertyNotSupported(nameof(NetworkConnectionType), value); }

    /// <inheritdoc cref="NetworkLevelAuthentication"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool NetworkLevelAuthentication { get => false; set => this.LogPropertyNotSupported(nameof(NetworkLevelAuthentication), value); }

    /// <inheritdoc cref="Password"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Password { set => RdpAdvancedSettings.ClearTextPassword = value; }

    /// <inheritdoc cref="PasswordContainsSmartCardPin"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool PasswordContainsSmartCardPin { get => this.GetProperty<bool>(RdpProperties.PasswordContainsSmartcardPin); set => this.SetProperty(RdpProperties.PasswordContainsSmartcardPin, value); }

    /// <inheritdoc cref="PCB"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PCB { get => string.Empty; set => this.LogPropertyNotSupported(nameof(PCB), value); }

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
    public bool PublicMode { get => false; set => this.LogPropertyNotSupported(nameof(PublicMode), value); }

    /// <inheritdoc cref="RdpExDll"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RdpExDll { get => rdpExDll; set => rdpExDll = value; }

    /// <inheritdoc cref="RedirectCameras"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectCameras { get => false; set => this.LogPropertyNotSupported(nameof(RedirectCameras), value); }

    /// <inheritdoc cref="RedirectClipboard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectClipboard { get => false; set => this.LogPropertyNotSupported(nameof(RedirectClipboard), value); }

    /// <inheritdoc cref="RedirectDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDevices { get => false; set => this.LogPropertyNotSupported(nameof(RedirectDevices), value); }

    /// <inheritdoc cref="RedirectDirectX"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectDirectX { get => false; set => this.LogPropertyNotSupported(nameof(RedirectDirectX), value); }

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
    public bool RedirectedAuthentication { get => false; set => this.LogPropertyNotSupported(nameof(RedirectedAuthentication), value); }

    /// <inheritdoc cref="RedirectLocation"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectLocation { get => this.GetProperty<bool>(RdpProperties.EnableLocationRedirection); set => this.SetProperty(RdpProperties.EnableLocationRedirection, value); }

    /// <inheritdoc cref="RedirectPorts"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPorts { get => RdpAdvancedSettings.RedirectPorts; set => RdpAdvancedSettings.RedirectPorts = value; }

    /// <inheritdoc cref="RedirectPOSDevices"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPOSDevices { get => false; set => this.LogPropertyNotSupported(nameof(RedirectPOSDevices), value); }

    /// <inheritdoc cref="RedirectPrinters"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectPrinters { get => RdpAdvancedSettings.RedirectPrinters; set => RdpAdvancedSettings.RedirectPrinters = value; }

    /// <inheritdoc cref="RedirectSmartCards"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RedirectSmartCards { get => RdpAdvancedSettings.RedirectSmartCards; set => RdpAdvancedSettings.RedirectSmartCards = value; }

    /// <inheritdoc cref="RelativeMouseMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RelativeMouseMode { get => false; set => this.LogPropertyNotSupported(nameof(RelativeMouseMode), value); }

    /// <inheritdoc cref="RemoteCredentialGuard"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RemoteCredentialGuard { get => false; set => this.LogPropertyNotSupported(nameof(RemoteCredentialGuard), value); }

    /// <inheritdoc cref="UseMultimon"/>
    public uint RemoteMonitorCount => 1;

    /// <inheritdoc cref="RestrictedAdminMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedAdminMode { get => false; set => this.LogPropertyNotSupported(nameof(RestrictedAdminMode), value); }

    /// <inheritdoc cref="RestrictedLogon"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RestrictedLogon { get => false; set => this.LogPropertyNotSupported(nameof(RestrictedLogon), value); }

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
    public bool UseMultimon { get => false; set => this.LogPropertyNotSupported(nameof(UseMultimon), value); }

    /// <inheritdoc cref="UseRedirectionServerName"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseRedirectionServerName { get => this.GetProperty<bool>(RdpProperties.UseRedirectionServerName); set => this.SetProperty(RdpProperties.UseRedirectionServerName, value); }

    /// <inheritdoc cref="VideoPlaybackMode"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VideoPlaybackMode VideoPlaybackMode { get => VideoPlaybackMode.DecodeAndRenderOnServer; set => this.LogPropertyNotSupported(nameof(VideoPlaybackMode), value); }

    /// <inheritdoc cref="WorkDir"/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string WorkDir { get => RdpSecuredSettings.WorkDir; set => RdpSecuredSettings.WorkDir = value; }

    /// <inheritdoc cref="GetErrorDescription"/>
    public string GetErrorDescription(int disconnectReasonCode) => $"Disconnected with reason code {disconnectReasonCode}. See https://docs.microsoft.com/en-us/windows/desktop/termserv/imstscaxevents-ondisconnected.";

    /// <inheritdoc cref="GetRemoteMonitorsBoundingBox"/>
    public void GetRemoteMonitorsBoundingBox(out int left, out int top, out int right, out int bottom)
    {
        left = 0;
        top = 0;
        right = 0;
        bottom = 0;
        this.LogMethodNotSupported(nameof(GetRemoteMonitorsBoundingBox));
    }

    /// <inheritdoc cref="OnClientAreaClicked"/>
    public event EventHandler? OnClientAreaClicked;

    /// <inheritdoc cref="RaiseClientAreaClicked"/>
    public void RaiseClientAreaClicked() => OnClientAreaClicked?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc cref="Reconnect"/>
    public ControlReconnectStatus Reconnect(uint width, uint height)
    {
        this.LogMethodNotSupported(nameof(Reconnect));
        return ControlReconnectStatus.controlReconnectBlocked;
    }

    /// <inheritdoc cref="SendRemoteAction"/>
    public void SendRemoteAction(RemoteSessionActionType action) => this.LogMethodNotSupported(nameof(SendRemoteAction));

    /// <inheritdoc cref="UpdateSessionDisplaySettings"/>
    public void UpdateSessionDisplaySettings(uint desktopWidth, uint desktopHeight, uint physicalWidth, uint physicalHeight, uint orientation, uint desktopScaleFactor, uint deviceScaleFactor) => this.LogMethodNotSupported(nameof(UpdateSessionDisplaySettings));

    /// <inheritdoc cref="WndProc"/>
    protected override void WndProc(ref Message m)
    {
        if (MessageFilter.Filter(this, ref m))
            return;
        base.WndProc(ref m);
    }
}
