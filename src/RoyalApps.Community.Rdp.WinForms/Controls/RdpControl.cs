using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using AxMSTSCLib;
using Microsoft.Extensions.Logging;
using MSTSCLib;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Interfaces;
using RoyalApps.Community.Rdp.WinForms.Logging;
using Timer = System.Windows.Forms.Timer;

namespace RoyalApps.Community.Rdp.WinForms.Controls;

/// <summary>
/// A WinForms control to establish and work in a RDP remote desktop session.
/// </summary>
public class RdpControl : UserControl
{
    /// <summary>
    /// The ILogger instance used for logging. If not set, a default logger is used for debug logging.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ILogger Logger { get; set; } = DebugLoggerFactory.Create();

    private bool _canScale;
    private bool _nlaReconnect;

    private int _currentZoomLevel = 100;
    private Size _previousClientSize = Size.Empty;
    
    private readonly Timer _timerResizeInProgress;
    private readonly HashSet<string> _rdClientSearchPaths =
    [
        @"%ProgramFiles%\Remote Desktop",
        @"%ProgramFiles(x86)%\Remote Desktop",
        @"%ProgramFiles(ARM)%\Remote Desktop",
        @"%LocalAppData%\Apps\Remote Desktop"
    ];

    /// <summary>
    /// Access to the RDP client and their events, methods and properties.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IRdpClient? RdpClient { get; private set; }

    /// <summary>
    /// Access to the RDP client configuration which will be used to instantiate the RdpClient.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RdpClientConfiguration RdpConfiguration { get; set; }
    
    /// <summary>
    /// Gets the current zoom level in percent of the remote desktop session.
    /// </summary>
    public int CurrentZoomLevel => _currentZoomLevel;
    
    /// <summary>
    /// Returns true if a connection has been established successfully.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool WasSuccessfullyConnected { get; private set; }

    /// <summary>
    /// Raised when the RDP ActiveX has been connected.
    /// </summary>
    public event EventHandler<ConnectedEventArgs>? OnConnected;
    
    /// <summary>
    /// Raised when the RDP ActiveX has been disconnected.
    /// </summary>
    public event EventHandler<DisconnectedEventArgs>? OnDisconnected;

    /// <summary>
    /// Raised when the user presses the Minimize button on the connection bar in full-screen mode. The firing of this event is a request that the container application minimize itself.
    /// </summary>
    public event EventHandler? OnRequestContainerMinimize;

    /// <summary>
    /// Raised when the client requests to leave full-screen mode or the ContainerHandledFullScreen property has been set to a nonzero value.
    /// </summary>
    public event EventHandler? OnRequestLeaveFullScreen;

    /// <summary>
    /// Raised when the client calls the RequestClose method.
    /// </summary>
    public event EventHandler<IMsTscAxEvents_OnConfirmCloseEvent>? OnConfirmClose;

    /// <summary>
    /// This event is raised when a user clicks into a remote desktop session and DisableClickDetection is not set. 
    /// </summary>
    public event EventHandler? OnClientAreaClicked;

    /// <summary>
    /// Raised before the remote desktop size is going to change.
    /// This event can be used to cancel the resize event using the CancelEventArgs.
    /// </summary>
    public event EventHandler<CancelEventArgs>? BeforeRemoteDesktopSizeChanged;
    
    /// <summary>
    /// Raised when the remote desktop size has changed.
    /// </summary>
    public event EventHandler? RemoteDesktopSizeChanged;

    /// <summary>
    /// Raised after the <see cref="IRdpClient"/> was created and configured, but before the connection is established.
    /// This event allows users to customize the RdpClient in extra ways not covered by the <see cref="RdpConfiguration"/>.
    /// </summary>
    public event EventHandler? RdpClientConfigured;

    /// <summary>
    /// Creates a new instance of the RdpControl.
    /// </summary>
    public RdpControl()
    {
        RdpConfiguration = new();
        
        _timerResizeInProgress = new Timer
        {
            Interval = 1000
        };
        SetStyle(ControlStyles.Selectable, true);
        SetStyle(ControlStyles.ContainerControl, false);
        TabStop = true;
        TabIndex = 0;
    }

    /// <inheritdoc cref="Dispose"/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CleanupRdpClient();

            _timerResizeInProgress.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// The zoom level will be set here in case AutoScaling is enabled.
    /// </summary>
    /// <inheritdoc cref="OnDpiChangedAfterParent"/>
    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        if (!_canScale || !RdpConfiguration.Display.AutoScaling)
            return;

        var newScaleFactor = LogicalToDeviceUnits(100);
        if (SetZoomLevel(newScaleFactor))
            _currentZoomLevel = newScaleFactor;
    }

    /// <summary>
    /// Used to adapt the session size (if SmartReconnect is set to true).
    /// </summary>
    /// <inheritdoc cref="OnSizeChanged"/>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        if (RdpConfiguration.Display.ResizeBehavior != ResizeBehavior.SmartReconnect)
            return;
        _timerResizeInProgress.Start();
    }

    /// <summary>
    /// Unregisters internal events. Useful when delaying disposal of the control.
    /// </summary>
    public void UnregisterEvents()
    {
        _timerResizeInProgress.Tick -= TimerResizeInProgress_Tick;
        
        if (RdpClient == null)
            return;
        
        RdpClient.OnConnected -= RdpClient_OnConnected;
        RdpClient.OnDisconnected -= RdpClient_OnDisconnected;
        RdpClient.OnRequestContainerMinimize -= RdpClient_OnRequestContainerMinimize;
        RdpClient.OnRequestLeaveFullScreen -= RdpClient_OnRequestLeaveFullScreen;
        RdpClient.OnConfirmClose -= RdpClient_OnConfirmClose;
        RdpClient.OnClientAreaClicked -= RdpClient_OnClientAreaClicked;
    }

    /// <summary>
    /// Starts the remote desktop connection. Make sure all properties, settings, events are correctly set.
    /// </summary>
    public void Connect()
    {
        CleanupRdpClient(true);
        CreateRdpClient();

        _canScale = false;

        ApplyInitialScaling();

        RdpClientConfigured?.Invoke(this, EventArgs.Empty);

        RdpClient!.Connect();
    }

    /// <summary>
    /// Disconnects the remote desktop session.
    /// </summary>
    public void Disconnect()
    {
        if (RdpClient == null || RdpClient.GetOcx() == null! || RdpClient.ConnectionState == ConnectionState.Disconnected) 
            return;

        try
        {
            RdpClient.Disconnect();
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "RDP disconnect failed");
        }
    }

    /// <summary>
    /// Set the focus to the RdpClient control.
    /// </summary>
    public void FocusRdpClient()
    {
        // notify "parent" that the rdp client control has been focused
        InvokeGotFocus(this, EventArgs.Empty);
        RdpClient?.Focus();
    }

    /// <summary>
    /// Resets the keyboard buffer. Keys can get stuck when using certain key combinations.
    /// </summary>
    public void ResetKeyboardBuffer()
    {
        if (RdpClient == null)
            return;
        
        var handle = new HWND(RdpClient.Handle);
        PInvoke.PostMessage(handle, PInvoke.WM_SETFOCUS, new WPARAM(UIntPtr.Zero), new LPARAM(IntPtr.Zero));
        PInvoke.PostMessage(handle, PInvoke.WM_KEYDOWN, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_CONTROL), new LPARAM(IntPtr.Zero));
        PInvoke.PostMessage(handle, PInvoke.WM_KEYDOWN, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_MENU), new LPARAM(IntPtr.Zero));
        PInvoke.PostMessage(handle, PInvoke.WM_KEYDOWN, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_LEFT), new LPARAM(IntPtr.Zero));
        PInvoke.PostMessage(handle, PInvoke.WM_KEYDOWN, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_LEFT), new LPARAM(IntPtr.Zero));
        PInvoke.PostMessage(handle, PInvoke.WM_KEYUP, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_LEFT), new LPARAM(IntPtr.Zero));
        PInvoke.PostMessage(handle, PInvoke.WM_KEYUP, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_LEFT), new LPARAM(IntPtr.Zero));
        PInvoke.PostMessage(handle, PInvoke.WM_KEYUP, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_MENU), new LPARAM(IntPtr.Zero));
        PInvoke.PostMessage(handle, PInvoke.WM_KEYUP, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_CONTROL), new LPARAM(IntPtr.Zero));

        new Thread(() =>
        {
            Thread.Sleep(50);
            PInvoke.PostMessage(handle, PInvoke.WM_KEYDOWN, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_CONTROL), new LPARAM(IntPtr.Zero));
            PInvoke.PostMessage(handle, PInvoke.WM_KEYDOWN, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_MENU), new LPARAM(IntPtr.Zero));
            PInvoke.PostMessage(handle, PInvoke.WM_KEYDOWN, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_LEFT), new LPARAM(IntPtr.Zero));
            PInvoke.PostMessage(handle, PInvoke.WM_KEYDOWN, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_LEFT), new LPARAM(IntPtr.Zero));
            PInvoke.PostMessage(handle, PInvoke.WM_KEYUP, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_LEFT), new LPARAM(IntPtr.Zero));
            PInvoke.PostMessage(handle, PInvoke.WM_KEYUP, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_LEFT), new LPARAM(IntPtr.Zero));
            PInvoke.PostMessage(handle, PInvoke.WM_KEYUP, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_MENU), new LPARAM(IntPtr.Zero));
            PInvoke.PostMessage(handle, PInvoke.WM_KEYUP, new WPARAM((UIntPtr)VIRTUAL_KEY.VK_CONTROL), new LPARAM(IntPtr.Zero));
        })
        {
            IsBackground = true
        }.Start();
    }

    /// <summary>
    /// Sends a remote action to the session.
    /// </summary>
    /// <param name="action">The remote action type.</param>
    public void SendRemoteAction(RemoteSessionActionType action)
    {
        Logger.LogDebug("Entered {MethodName}", nameof(SendRemoteAction));
        try
        {
            // the rdp client ActiveX control must be focused first,
            // otherwise the SendRemoteAction call will fail!
            FocusRdpClient();
            RdpClient?.SendRemoteAction(action);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to send remote action: {Action}", action);
        }
    }

    /// <summary>
    /// Change the resize behavior while connected.
    /// </summary>
    /// <param name="resizeBehavior">The desired resize behavior.</param>
    public void SetResizeBehavior(ResizeBehavior resizeBehavior)
    {
        RdpConfiguration.Display.ResizeBehavior = resizeBehavior;
        if (RdpClient is not {ConnectionState: ConnectionState.Connected})
            return;
        RdpClient.SmartSizing = resizeBehavior == ResizeBehavior.SmartSizing;
    }

    /// <summary>
    /// Shows the connection information dialog.
    /// </summary>
    public void ShowConnectionInfo()
    {
        if (RdpClient is null)
            return;
        RdpClient.ShowConnectionInformation = true;
    }
    
    /// <summary>
    /// Ensure the ActiveX control updates the client size according to the control size.
    /// </summary>
    /// <returns></returns>
    public bool UpdateClientSize()
    {
        if (_previousClientSize.Equals(Size))
            return true;
            
        _previousClientSize = Size;

        if (RdpConfiguration.Display.UseLocalScaling)
        {
            if (RdpConfiguration.Display.HasDesktopSize)
                return true;
            
            var scaleFactor = _currentZoomLevel / 100.00;
            var width = (int)(Width / scaleFactor);
            var height = (int)(Height / scaleFactor);
            var success = UpdateClientSizeWithoutReconnect(width, height, 100);
            if (success)
                SetZoomLevelLocal(_currentZoomLevel);
            return success;
        }
        
        return UpdateClientSizeWithoutReconnect() || UpdateClientSizeWithReconnect();
    }

    /// <summary>
    /// Increases the zoom level.
    /// </summary>
    public void ZoomIn()
    {
        var newScaleFactor = _currentZoomLevel switch
        {
            100 => 125,
            125 => 150,
            150 => 175,
            175 => 200,
            200 => 225,
            225 => 250,
            250 => 300,
            300 => 350,
            350 => 400,
            400 => 450,
            450 => 500,
            _ => 100
        };
        if (SetZoomLevel(newScaleFactor))
            _currentZoomLevel = newScaleFactor;
    }

    /// <summary>
    /// Decreases the zoom level.
    /// </summary>
    public void ZoomOut()
    {
        var newScaleFactor = _currentZoomLevel switch
        {
            500 => 450,
            450 => 400,
            400 => 350,
            350 => 300,
            300 => 250,
            250 => 225,
            225 => 200,
            200 => 175,
            175 => 150,
            150 => 125,
            125 => 100,
            100 => 100,
            _ => 100
        };
        if (SetZoomLevel(newScaleFactor))
            _currentZoomLevel = newScaleFactor;
    }

    private void RdpClient_OnConnected(object? sender, EventArgs e)
    {
        if (RdpClient == null)
            return;
        
        _previousClientSize = Size;
        ConnectedEventArgs? ea = null;
        if (RdpConfiguration.Display.FullScreen)
        {
            RdpClient.FullScreen = true;
            if (RdpClient is {UseMultimon: true, RemoteMonitorCount: > 1})
            {
                RdpClient.GetRemoteMonitorsBoundingBox(
                    out var left, 
                    out var top, 
                    out var right,
                    out var bottom);
                        
                var originalLeft = left;
                var originalTop = top;
                        
                if (left < 0)
                    left *= -1;
                if (right < 0)
                    right *= -1;
                if (top < 0)
                    top *= -1;
                if (bottom < 0)
                    bottom *= -1;

                ea = new ConnectedEventArgs
                {
                    Bounds = new Rectangle(
                        originalLeft,
                        originalTop,
                        left + right + 1,
                        top + bottom + 1),
                    MultiMonFullScreen = true
                };
                _previousClientSize = ea.Bounds.Size;
            }
        }

        var setLocalZoom = RdpConfiguration.Display.UseLocalScaling &&
                           (!RdpClient.UseMultimon || RdpClient is { RemoteMonitorCount: 1 }); 
        
        if (setLocalZoom)
            BeginInvoke(new MethodInvoker(() => { SetZoomLevelLocal(_currentZoomLevel); }));

        WasSuccessfullyConnected = true;

        OnConnected?.Invoke(sender, ea ?? new ConnectedEventArgs());
        _canScale = true;
    }

    private void RdpClient_OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
    {
        if (RdpClient == null)
            return;
        
        switch (e.discReason)
        {
            // ignore this one. a reconnect is in progress (RDP 8)
            case 4360:
                return;
            case 2825 when !RdpClient.NetworkLevelAuthentication && !_nlaReconnect:
                // NLA seems to be required and was not reconnected using the setting
                RdpConfiguration.Credentials.NetworkLevelAuthentication = true;
                _nlaReconnect = true;
                Connect();
                return;
            default:
                var description = e.discReason switch
                {
                    0 => "No information is available. (Disconnect Reason = 0).",
                    1 => "Local disconnection. (Disconnect Reason = 1).",
                    2 => "Remote disconnection by user. (Disconnect Reason = 2).",
                    _ => RdpClient.GetErrorDescription(e.discReason)
                };

                // shows an error message instead of the disconnect hint and closes the tab
                var showError = true;

                // find out if we need to show a messagebox
                // documentation about error codes: http://msdn.microsoft.com/en-us/library/windows/desktop/aa382170%28v=vs.85%29.aspx
                switch (e.discReason)
                {
                    case 260: // DNS name lookup failure.
                    case 516: // windows socket connect failed
                    case 520: // host not found
                    case 776: // the IP address specified is not valid
                    case 1288: // DNS lookup failed
                    case 1540: // get host by name failed
                    case 2052: // bad IP address
                        break;
                    default:
                        showError = false;
                        break;
                }

                var userInitiated = e.discReason is > 0 and < 4;

                var ea = new DisconnectedEventArgs
                {
                    DisconnectCode = e.discReason, 
                    Description = description, 
                    ShowError = showError, 
                    UserInitiated = userInitiated
                };
                OnDisconnected?.Invoke(sender, ea);
                break;
        }
    }

    private void RdpClient_OnRequestContainerMinimize(object? sender, EventArgs e)
    {
        OnRequestContainerMinimize?.Invoke(sender, e);
    }

    private void RdpClient_OnRequestLeaveFullScreen(object? sender, EventArgs e)
    {
        OnRequestLeaveFullScreen?.Invoke(sender, e);
    }

    private void RdpClient_OnConfirmClose(object sender, IMsTscAxEvents_OnConfirmCloseEvent e)
    {
        OnConfirmClose?.Invoke(sender, e);
    }

    private void RdpClient_OnClientAreaClicked(object? sender, EventArgs e)
    {
        OnClientAreaClicked?.Invoke(sender, e);
    }

    private void ApplyInitialScaling()
    {
        _currentZoomLevel = RdpConfiguration.Display.AutoScaling 
            ? LogicalToDeviceUnits(100) : 
            RdpConfiguration.Display.InitialZoomLevel;

        if (RdpConfiguration.Display.UseLocalScaling)
        {
            if (RdpConfiguration.Display.HasDesktopSize)
                return;
        
            var scaleFactor = _currentZoomLevel / 100.00;
            RdpClient!.DesktopWidth = (int)(Width / scaleFactor);
            RdpClient.DesktopHeight = (int)(Height / scaleFactor);
            return;
        }

        try
        {
            RdpClient!.DeviceScaleFactor = GetDeviceScaleFactor(_currentZoomLevel); // must be 100, 140 or 180
            RdpClient.DesktopScaleFactor = GetDesktopScaleFactor(_currentZoomLevel); // must be 100, 125, 150 or 200
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Cannot apply initial scale factor {ScaleFactor}", _currentZoomLevel);
            _currentZoomLevel = 100;
        }
    }

    private void CleanupRdpClient(bool performDisconnect = false)
    {
        if (RdpClient == null)
            return;
        
        if (performDisconnect && RdpClient.ConnectionState != ConnectionState.Disconnected)
            RdpClient.Disconnect();
        
        UnregisterEvents();
        RdpClient.Dispose();
    }

    private void CreateRdpClient()
    {
        var msTscAxDllPath = GetMsTscAxDllPath();
        var msRdcAxDllPath = GetRdClientAxDllPath();
        var rdcClientInstalled = !string.IsNullOrWhiteSpace(msRdcAxDllPath);

        RdpClient = RdpClientFactory.Create(
            RdpConfiguration.ClientVersion, 
            MsRdpExManager.Instance.AxHookEnabled ? MsRdpExManager.Instance.CoreApi.MsRdpExDllPath : null,
            RdpConfiguration.UseMsRdc && rdcClientInstalled);
        
        ((ISupportInitialize)RdpClient).BeginInit();
        Environment.SetEnvironmentVariable("MSRDPEX_LOG_ENABLED", RdpConfiguration.LogEnabled ? "1" : "0");
        Environment.SetEnvironmentVariable("MSRDPEX_LOG_LEVEL", RdpConfiguration.LogLevel);
        Environment.SetEnvironmentVariable("MSRDPEX_LOG_FILE_PATH", RdpConfiguration.LogFilePath);
        
        var control = (Control) RdpClient;
        control.Dock = DockStyle.Fill;
        Controls.Add(control);

        if (RdpConfiguration.UseMsRdc && !string.IsNullOrWhiteSpace(msRdcAxDllPath))
        {
            Logger.LogDebug("Microsoft Remote Desktop Client (rdclientax.dll) will be used");
            Environment.SetEnvironmentVariable("MSRDPEX_RDCLIENTAX_DLL", msRdcAxDllPath);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(msRdcAxDllPath))
                Logger.LogDebug("Microsoft Remote Desktop Client will not be used, rdclientax.dll was not found");
        
            Environment.SetEnvironmentVariable("MSRDPEX_MSTSCAX_DLL", msTscAxDllPath);
                
            RdpConfiguration.UseMsRdc = false;
        }

        // workaround to ensure msrdcax.dll can be used even when hooking not enabled:
        // https://github.com/Devolutions/MsRdpEx/blob/01995487f22fd697262525ece2e0a6f02908715b/dotnet/MsRdpEx_App/MainDlg.cs#L307
        try 
        {
            object requestUseNewOutputPresenter = true;
            RdpClient.GetExtendedSettings().set_Property("RequestUseNewOutputPresenter", ref requestUseNewOutputPresenter);
        }
        catch
        {
            // ignored
        }

        this.ApplyRdpClientConfiguration(RdpConfiguration);

        RegisterEvents();
        ((ISupportInitialize)RdpClient).EndInit();
    }

    private uint GetDesktopScaleFactor(int zoomLevel)
    {
        return zoomLevel > 500 ? 500 : (uint) zoomLevel;
    }

    private uint GetDeviceScaleFactor(int zoomLevel) =>
        zoomLevel switch
        {
            <= 100 => 100,
            < 200 => 140,
            >= 200 => 180
        };

    private string GetMsTscAxDllPath() => Environment.ExpandEnvironmentVariables(@"%SystemRoot%\System32\mstscax.dll");

    private string? GetRdClientAxDllPath()
    {
        if (!string.IsNullOrWhiteSpace(RdpConfiguration.MsRdcPath) && File.Exists(RdpConfiguration.MsRdcPath))
            return RdpConfiguration.MsRdcPath;

        foreach (var path in _rdClientSearchPaths)
        {
            var searchPath = Path.Combine(Environment.ExpandEnvironmentVariables(path), "rdclientax.dll");
            Logger.LogDebug("Searching file {SearchPath}", searchPath);
            if (!Path.Exists(searchPath))
                continue;
            return searchPath;
        }

        return null;
    }

    private void RegisterEvents()
    {
        _timerResizeInProgress.Tick += TimerResizeInProgress_Tick;
        
        RdpClient!.OnConnected += RdpClient_OnConnected;
        RdpClient.OnDisconnected += RdpClient_OnDisconnected;
        RdpClient.OnRequestContainerMinimize += RdpClient_OnRequestContainerMinimize;
        RdpClient.OnRequestLeaveFullScreen += RdpClient_OnRequestLeaveFullScreen;
        RdpClient.OnConfirmClose += RdpClient_OnConfirmClose;
        RdpClient.OnClientAreaClicked += RdpClient_OnClientAreaClicked;
    }

    private bool SetZoomLevel(int desiredZoomLevel)
    {
        return RdpConfiguration.Display.UseLocalScaling
            ? SetZoomLevelLocal(desiredZoomLevel)
            : SetZoomLevelRemote(desiredZoomLevel);
    }

    private bool SetZoomLevelRemote(int desiredZoomLevel)
    {
        try
        {
            RdpClient!.UpdateSessionDisplaySettings(
                (uint) ClientSize.Width,
                (uint) ClientSize.Height,
                0,
                0,
                0,
                GetDesktopScaleFactor(desiredZoomLevel),
                GetDeviceScaleFactor(desiredZoomLevel)
            );
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex,
                "Unable to set remote zoom level to {ZoomLevel}. Not all RDP and OS versions support different DPIs in remote sessions. Check the log for more information",
                desiredZoomLevel);
            return false;
        }

        return true;
    }

    private bool SetZoomLevelLocal(int desiredZoomLevel)
    {
        object zl = (uint) desiredZoomLevel;
        Logger.LogDebug("Setting local zoom level to {ZoomLevel}", desiredZoomLevel);
            
        if (RdpClient!.TrySetProperty("ZoomLevel", ref zl, out var ex))
            return true;

        Logger.LogWarning(ex,
            "Unable to set local zoom level to {ZoomLevel}. Not all RDP and OS versions support different DPIs in remote sessions. Check the log for more information",
            desiredZoomLevel);
        return false;
    }

    private void TimerResizeInProgress_Tick(object? sender, EventArgs e)
    {
        if (MouseButtons == MouseButtons.Left)
            return;
        _timerResizeInProgress.Stop();
        RaiseRemoteDesktopSizeChanged();
    }

    private bool RaiseBeforeRemoteDesktopSizeChangedAndCancel()
    {
        var args = new CancelEventArgs();
        BeforeRemoteDesktopSizeChanged?.Invoke(this, args);
        return args.Cancel;
    }

    private void RaiseRemoteDesktopSizeChanged()
    {
        // make sure that Size 0,0 (when minimized) is also ignored
        if (Size.Width == 0 ||
            Size.Height == 0 ||
            _previousClientSize.IsEmpty ||
            _previousClientSize.Equals(Size))
            return;

        if (RaiseBeforeRemoteDesktopSizeChangedAndCancel())
            return;

        UpdateClientSize();
        RemoteDesktopSizeChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool UpdateClientSizeWithoutReconnect()
    {
        return UpdateClientSizeWithoutReconnect(Width, Height, _currentZoomLevel);
    }
    
    private bool UpdateClientSizeWithoutReconnect(int width, int height, int zoomLevel)
    {
        try
        {
            RdpClient!.UpdateSessionDisplaySettings(
                (uint) width,
                (uint) height,
                0,
                0,
                0,
                GetDesktopScaleFactor(zoomLevel),
                GetDeviceScaleFactor(zoomLevel)
            );
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogDebug(ex, "UpdateSessionDisplaySettings failed");
        }

        return false;
    }

    private bool UpdateClientSizeWithReconnect()
    {
        var success = RdpClient!.Reconnect((uint) Width, (uint) Height) == ControlReconnectStatus.controlReconnectStarted;
        Logger.LogDebug("UpdateClientSizeWithReconnect result: {Result}", success ? "Success" : "Failed");
        return success;
    }
}