using System;
using System.Collections.Generic;
using IMsTscAxEvents_OnConfirmCloseEventHandler = AxMSTSCLib.IMsTscAxEvents_OnConfirmCloseEventHandler;
using IMsTscAxEvents_OnDisconnectedEventHandler = AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEventHandler;
using MSTSCLib;
using Microsoft.Extensions.Logging;
using RoyalApps.Community.Rdp.WinForms.Configuration.Input;
using RoyalApps.Community.Rdp.WinForms.Configuration.Performance;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using RoyalApps.Community.Rdp.WinForms.Configuration.Connection;
using RoyalApps.Community.Rdp.WinForms.Configuration.Display;
using RoyalApps.Community.Rdp.WinForms.Configuration.Internal;
using RoyalApps.Community.Rdp.WinForms.Controls.ActiveX;
using RoyalApps.Community.Rdp.WinForms.Controls.Clients;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class RdpLocalZoomTests
{
    [Fact]
    public void TrySetZoomLevel_WithoutHook_ReachesNativeControl()
    {
        var client = new RecordingRdpClient();
        var recording = (RecordingRdpClient)client;
        var nativeFailure = new InvalidOperationException("Native control reached");
        recording.GetOcxException = nativeFailure;

        Assert.False(client.TrySetProperty(RdpProperties.ZoomLevel, 150u, out var exception));
        Assert.Same(nativeFailure, exception);
        Assert.Equal(1, recording.GetOcxCalls);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ApplyConfiguration_ForwardsHardwareMode_AndDisablesSmartSizingForLocalZoom(bool hardwareMode)
    {
        WithControl((control, recording) =>
        {
            control.RdpConfiguration.Server = "rdp.example.test";
            control.RdpConfiguration.Performance.EnableHardwareMode = hardwareMode;
            control.RdpConfiguration.Display.UseLocalScaling = true;
            control.RdpConfiguration.Display.ResizeBehavior = ResizeBehavior.SmartSizing;

            control.ApplyRdpClientConfiguration(RdpConnectionContextFactory.Create(control.RdpConfiguration));

            Assert.Equal(hardwareMode, Assert.IsType<bool>(recording.Properties[nameof(IRdpClient.EnableHardwareMode)]));
            Assert.False(Assert.IsType<bool>(recording.Properties[nameof(IRdpClient.SmartSizing)]));
        });
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void SetResizeBehavior_SmartSizing_RespectsLocalZoom(bool useLocalScaling, bool expectedSmartSizing)
    {
        WithControl((control, recording) =>
        {
            recording.Properties[nameof(IRdpClient.ConnectionState)] = ConnectionState.Connected;
            control.RdpConfiguration.Display.UseLocalScaling = useLocalScaling;

            control.SetResizeBehavior(ResizeBehavior.SmartSizing);

            Assert.Equal(expectedSmartSizing, Assert.IsType<bool>(recording.Properties[nameof(IRdpClient.SmartSizing)]));
        });
    }

    private static void WithControl(Action<RdpControl, RecordingRdpClient> action)
    {
        Exception? testException = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var control = new RdpControl { Logger = NullLogger.Instance };
                var clientProperty = typeof(RdpControl).GetProperty(nameof(RdpControl.RdpClient))!;
                var client = new RecordingRdpClient();
                clientProperty.SetValue(control, client);
                try
                {
                    action(control, (RecordingRdpClient)client);
                }
                finally
                {
                    // The recording client is not a WinForms control; leave native disposal to native tests.
                    clientProperty.SetValue(control, null);
                }
            }
            catch (Exception ex)
            {
                testException = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(30)), "The STA zoom regression test did not complete within 30 seconds.");
        if (testException is not null)
            ExceptionDispatchInfo.Capture(testException).Throw();
    }

    public class RecordingRdpClient : IRdpClient
    {
        public Dictionary<string, object?> Properties { get; } = [];
        public Exception? GetOcxException { get; set; }
        public int GetOcxCalls { get; private set; }

        private T Read<T>(string property) => Properties.TryGetValue(property, out var value) ? (T)value! : default!;

        public bool AcceleratorPassthrough { get => Read<bool>("AcceleratorPassthrough"); set => Properties["AcceleratorPassthrough"] = value; }
        public bool AllowBackgroundInput { get => Read<bool>("AllowBackgroundInput"); set => Properties["AllowBackgroundInput"] = value; }
        public bool AudioCaptureRedirectionMode { get => Read<bool>("AudioCaptureRedirectionMode"); set => Properties["AudioCaptureRedirectionMode"] = value; }
        public AudioQualityMode AudioQualityMode { get => Read<AudioQualityMode>("AudioQualityMode"); set => Properties["AudioQualityMode"] = value; }
        public AudioRedirectionMode AudioRedirectionMode { get => Read<AudioRedirectionMode>("AudioRedirectionMode"); set => Properties["AudioRedirectionMode"] = value; }
        public AuthenticationLevel AuthenticationLevel { get => Read<AuthenticationLevel>("AuthenticationLevel"); set => Properties["AuthenticationLevel"] = value; }
        public string AuthenticationServiceClass { get => Read<string>("AuthenticationServiceClass"); set => Properties["AuthenticationServiceClass"] = value; }
        public string AxName { get => Read<string>("AxName"); set => Properties["AxName"] = value; }
        public bool BandwidthDetection { get => Read<bool>("BandwidthDetection"); set => Properties["BandwidthDetection"] = value; }
        public bool BitmapCaching { get => Read<bool>("BitmapCaching"); set => Properties["BitmapCaching"] = value; }
        public ClientSpec ClientProtocolSpec { get => Read<ClientSpec>("ClientProtocolSpec"); set => Properties["ClientProtocolSpec"] = value; }
        public int ColorDepth { get => Read<int>("ColorDepth"); set => Properties["ColorDepth"] = value; }
        public bool Compression { get => Read<bool>("Compression"); set => Properties["Compression"] = value; }
        public ConnectionState ConnectionState { get => Read<ConnectionState>("ConnectionState"); }
        public bool ConnectToAdministerServer { get => Read<bool>("ConnectToAdministerServer"); set => Properties["ConnectToAdministerServer"] = value; }
        public int ContainerHandledFullScreen { get => Read<int>("ContainerHandledFullScreen"); set => Properties["ContainerHandledFullScreen"] = value; }
        public bool ContainsFocus { get => Read<bool>("ContainsFocus"); }
        public int DesktopHeight { get => Read<int>("DesktopHeight"); set => Properties["DesktopHeight"] = value; }
        public uint DesktopScaleFactor { get => Read<uint>("DesktopScaleFactor"); set => Properties["DesktopScaleFactor"] = value; }
        public int DesktopWidth { get => Read<int>("DesktopWidth"); set => Properties["DesktopWidth"] = value; }
        public uint DeviceScaleFactor { get => Read<uint>("DeviceScaleFactor"); set => Properties["DeviceScaleFactor"] = value; }
        public bool DisableClickDetection { get => Read<bool>("DisableClickDetection"); set => Properties["DisableClickDetection"] = value; }
        public bool DisableCredentialsDelegation { get => Read<bool>("DisableCredentialsDelegation"); set => Properties["DisableCredentialsDelegation"] = value; }
        public bool DisableUdpTransport { get => Read<bool>("DisableUdpTransport"); set => Properties["DisableUdpTransport"] = value; }
        public bool DisplayConnectionBar { get => Read<bool>("DisplayConnectionBar"); set => Properties["DisplayConnectionBar"] = value; }
        public string? Domain { get => Read<string?>("Domain"); set => Properties["Domain"] = value; }
        public bool EnableAutoReconnect { get => Read<bool>("EnableAutoReconnect"); set => Properties["EnableAutoReconnect"] = value; }
        public bool EnableHardwareMode { get => Read<bool>("EnableHardwareMode"); set => Properties["EnableHardwareMode"] = value; }
        public bool EnableMouseJiggler { get => Read<bool>("EnableMouseJiggler"); set => Properties["EnableMouseJiggler"] = value; }
        public bool EnableRdsAadAuth { get => Read<bool>("EnableRdsAadAuth"); set => Properties["EnableRdsAadAuth"] = value; }
        public bool EnableWindowsKey { get => Read<bool>("EnableWindowsKey"); set => Properties["EnableWindowsKey"] = value; }
        public bool FullScreen { get => Read<bool>("FullScreen"); set => Properties["FullScreen"] = value; }
        public string FullScreenTitle { set => Properties["FullScreenTitle"] = value; }
        public bool GatewayCredSharing { get => Read<bool>("GatewayCredSharing"); set => Properties["GatewayCredSharing"] = value; }
        public GatewayCredentialSource GatewayCredsSource { get => Read<GatewayCredentialSource>("GatewayCredsSource"); set => Properties["GatewayCredsSource"] = value; }
        public string GatewayDomain { get => Read<string>("GatewayDomain"); set => Properties["GatewayDomain"] = value; }
        public string GatewayHostname { get => Read<string>("GatewayHostname"); set => Properties["GatewayHostname"] = value; }
        public string GatewayPassword { set => Properties["GatewayPassword"] = value; }
        public GatewayProfileUsageMethod GatewayProfileUsageMethod { get => Read<GatewayProfileUsageMethod>("GatewayProfileUsageMethod"); set => Properties["GatewayProfileUsageMethod"] = value; }
        public GatewayUsageMethod GatewayUsageMethod { get => Read<GatewayUsageMethod>("GatewayUsageMethod"); set => Properties["GatewayUsageMethod"] = value; }
        public string GatewayUsername { get => Read<string>("GatewayUsername"); set => Properties["GatewayUsername"] = value; }
        public GatewayCredentialSource GatewayUserSelectedCredsSource { get => Read<GatewayCredentialSource>("GatewayUserSelectedCredsSource"); set => Properties["GatewayUserSelectedCredsSource"] = value; }
        public bool GrabFocusOnConnect { get => Read<bool>("GrabFocusOnConnect"); set => Properties["GrabFocusOnConnect"] = value; }
        public IntPtr Handle { get => Read<IntPtr>("Handle"); }
        public bool IsDisposed { get => Read<bool>("IsDisposed"); }
        public bool IsHandleCreated { get => Read<bool>("IsHandleCreated"); }
        public int KeepAliveInterval { get => Read<int>("KeepAliveInterval"); set => Properties["KeepAliveInterval"] = value; }
        public int KeyboardHookMode { get => Read<int>("KeyboardHookMode"); set => Properties["KeyboardHookMode"] = value; }
        public bool KeyboardHookToggleShortcutEnabled { get => Read<bool>("KeyboardHookToggleShortcutEnabled"); set => Properties["KeyboardHookToggleShortcutEnabled"] = value; }
        public string KeyBoardLayoutStr { set => Properties["KeyBoardLayoutStr"] = value; }
        public string LoadBalanceInfo { get => Read<string>("LoadBalanceInfo"); set => Properties["LoadBalanceInfo"] = value; }
        public string? KdcProxyUrl { get => Read<string?>("KdcProxyUrl"); set => Properties["KdcProxyUrl"] = value; }
        public ILogger Logger { get => Read<ILogger>("Logger"); set => Properties["Logger"] = value; }
        public bool MaximizeShell { get => Read<bool>("MaximizeShell"); set => Properties["MaximizeShell"] = value; }
        public int MaxReconnectAttempts { get => Read<int>("MaxReconnectAttempts"); set => Properties["MaxReconnectAttempts"] = value; }
        public int MouseJigglerInterval { get => Read<int>("MouseJigglerInterval"); set => Properties["MouseJigglerInterval"] = value; }
        public KeepAliveMethod MouseJigglerMethod { get => Read<KeepAliveMethod>("MouseJigglerMethod"); set => Properties["MouseJigglerMethod"] = value; }
        public bool NegotiateSecurityLayer { get => Read<bool>("NegotiateSecurityLayer"); set => Properties["NegotiateSecurityLayer"] = value; }
        public uint NetworkConnectionType { get => Read<uint>("NetworkConnectionType"); set => Properties["NetworkConnectionType"] = value; }
        public bool NetworkLevelAuthentication { get => Read<bool>("NetworkLevelAuthentication"); set => Properties["NetworkLevelAuthentication"] = value; }
        public string? Password { set => Properties["Password"] = value; }
        public bool PasswordContainsSmartCardPin { get => Read<bool>("PasswordContainsSmartCardPin"); set => Properties["PasswordContainsSmartCardPin"] = value; }
        public string PCB { get => Read<string>("PCB"); set => Properties["PCB"] = value; }
        int IRdpClient.PerformanceFlags { get => Read<int>("PerformanceFlags"); set => Properties["PerformanceFlags"] = value; }
        public bool PinConnectionBar { get => Read<bool>("PinConnectionBar"); set => Properties["PinConnectionBar"] = value; }
        public string PluginDlls { set => Properties["PluginDlls"] = value; }
        public int Port { get => Read<int>("Port"); set => Properties["Port"] = value; }
        public bool PublicMode { get => Read<bool>("PublicMode"); set => Properties["PublicMode"] = value; }
        public string RdpExDll { get => Read<string>("RdpExDll"); set => Properties["RdpExDll"] = value; }
        public bool RedirectCameras { get => Read<bool>("RedirectCameras"); set => Properties["RedirectCameras"] = value; }
        public bool RedirectClipboard { get => Read<bool>("RedirectClipboard"); set => Properties["RedirectClipboard"] = value; }
        public bool RedirectDevices { get => Read<bool>("RedirectDevices"); set => Properties["RedirectDevices"] = value; }
        public bool RedirectDirectX { get => Read<bool>("RedirectDirectX"); set => Properties["RedirectDirectX"] = value; }
        public string RedirectDriveLetters { get => Read<string>("RedirectDriveLetters"); set => Properties["RedirectDriveLetters"] = value; }
        public bool RedirectDrives { get => Read<bool>("RedirectDrives"); set => Properties["RedirectDrives"] = value; }
        public bool RedirectedAuthentication { get => Read<bool>("RedirectedAuthentication"); set => Properties["RedirectedAuthentication"] = value; }
        public bool RedirectLocation { get => Read<bool>("RedirectLocation"); set => Properties["RedirectLocation"] = value; }
        public bool RedirectPorts { get => Read<bool>("RedirectPorts"); set => Properties["RedirectPorts"] = value; }
        public bool RedirectPOSDevices { get => Read<bool>("RedirectPOSDevices"); set => Properties["RedirectPOSDevices"] = value; }
        public bool RedirectPrinters { get => Read<bool>("RedirectPrinters"); set => Properties["RedirectPrinters"] = value; }
        public bool RedirectSmartCards { get => Read<bool>("RedirectSmartCards"); set => Properties["RedirectSmartCards"] = value; }
        public bool RelativeMouseMode { get => Read<bool>("RelativeMouseMode"); set => Properties["RelativeMouseMode"] = value; }
        public bool RemoteCredentialGuard { get => Read<bool>("RemoteCredentialGuard"); set => Properties["RemoteCredentialGuard"] = value; }
        public uint RemoteMonitorCount { get => Read<uint>("RemoteMonitorCount"); }
        public bool RestrictedAdminMode { get => Read<bool>("RestrictedAdminMode"); set => Properties["RestrictedAdminMode"] = value; }
        public bool RestrictedLogon { get => Read<bool>("RestrictedLogon"); set => Properties["RestrictedLogon"] = value; }
        public string Server { get => Read<string>("Server"); set => Properties["Server"] = value; }
        public bool ShowConnectionInformation { get => Read<bool>("ShowConnectionInformation"); set => Properties["ShowConnectionInformation"] = value; }
        public bool SmartSizing { get => Read<bool>("SmartSizing"); set => Properties["SmartSizing"] = value; }
        public string StartProgram { get => Read<string>("StartProgram"); set => Properties["StartProgram"] = value; }
        public bool UseMultimon { get => Read<bool>("UseMultimon"); set => Properties["UseMultimon"] = value; }
        public bool UseRedirectionServerName { get => Read<bool>("UseRedirectionServerName"); set => Properties["UseRedirectionServerName"] = value; }
        public string? UserName { get => Read<string?>("UserName"); set => Properties["UserName"] = value; }
        public VideoPlaybackMode VideoPlaybackMode { get => Read<VideoPlaybackMode>("VideoPlaybackMode"); set => Properties["VideoPlaybackMode"] = value; }
        public string WorkDir { get => Read<string>("WorkDir"); set => Properties["WorkDir"] = value; }
        public event EventHandler OnClientAreaClicked { add { } remove { } }
        public event IMsTscAxEvents_OnConfirmCloseEventHandler OnConfirmClose { add { } remove { } }
        public event EventHandler OnConnected { add { } remove { } }
        public event IMsTscAxEvents_OnDisconnectedEventHandler OnDisconnected { add { } remove { } }
        public event EventHandler OnRequestContainerMinimize { add { } remove { } }
        public event EventHandler OnRequestLeaveFullScreen { add { } remove { } }
        public object? GetOcx()
        {
            GetOcxCalls++;
            if (GetOcxException is not null)
                throw GetOcxException;
            return null;
        }

        public void Dispose() { }
        public void Connect() { }
        public void Disconnect() { }
        public bool Focus() => false;
        public string GetErrorDescription(int disconnectReasonCode) => string.Empty;
        public void GetRemoteMonitorsBoundingBox(out int left, out int top, out int right, out int bottom) => left = top = right = bottom = 0;
        public void RaiseClientAreaClicked() { }
        public ControlReconnectStatus Reconnect(uint width, uint height) => default;
        public void SendRemoteAction(RemoteSessionActionType action) { }
        public void UpdateSessionDisplaySettings(uint desktopWidth, uint desktopHeight, uint physicalWidth, uint physicalHeight, uint orientation, uint desktopScaleFactor, uint deviceScaleFactor) { }
    }
}
