#nullable enable
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using AxMSTSCLib;
using Microsoft.Extensions.Logging;
using MSTSCLib;
using RoyalApps.Community.Rdp.WinForms.Interfaces;
using RoyalApps.Community.Rdp.WinForms.Logging;

namespace RoyalApps.Community.Rdp.WinForms;

/// <summary>
/// A WinForms control to establish and work in a RDP remote desktop session.
/// </summary>
public class RdpControl : UserControl
{
    /// <summary>
    /// The ILogger instance used for logging. If not set, a default logger is used for debug logging.
    /// </summary>
    public ILogger Logger { get; set; } = DebugLoggerFactory.Create();

    private bool _canScale;
    private bool _nlaReconnect;

    private int _currentZoomLevel = 100;
    private Size _previousClientSize = Size.Empty;
    private bool _smartReconnect;
    
    private readonly System.Windows.Forms.Timer _timerResizeInProgress;
    private readonly System.Windows.Forms.Timer _timerSmartReconnect;

    /// <summary>
    /// Access to the RDP client and their events, methods and properties.
    /// </summary>
    public IRdpClient RdpClient { get; private set; } = null!;

    /// <summary>
    /// The Rdp Client Version to use. Default value is 0 which tries to get the highest possible client version.
    /// </summary>
    public int RdpClientVersion { get; set; } = 0;
    
    /// <summary>
    /// If set to true, DPI scaling will be applied automatically.
    /// </summary>
    public bool AutoScaling { get; set; }

    /// <summary>
    /// Use local scaling instead of setting the DPI in the remote session.
    /// </summary>
    public bool UseLocalScaling { get; set; }

    /// <summary>
    /// Gets the current zoom level in percent of the remote desktop session.
    /// </summary>
    public int CurrentZoomLevel => _currentZoomLevel;
    
    /// <summary>
    /// The initial zoom level in percent (default is 100).
    /// </summary>
    public int InitialZoomLevel { get; set; } = 100;
    
    /// <summary>
    /// If true, the RdpClient will enter the full screen mode (by setting UseMultiMon).
    /// </summary>
    public bool SetFullScreenOnConnect { get; set; }

    /// <summary>
    /// If set to true, the control will update the desktop size automatically when its size changes. 
    /// </summary>
    public bool SmartReconnect
    {
        get => _smartReconnect;
        set
        {
            _smartReconnect = value;
            _timerSmartReconnect.Enabled = value;
        }
    }

    /// <summary>
    /// Returns true if a connection has been established successfully.
    /// </summary>
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
    /// Raised before the remote desktop size is going to change.
    /// This event can be used to cancel the resize event using the CancelEventArgs.
    /// </summary>
    public event EventHandler<CancelEventArgs>? BeforeRemoteDesktopSizeChanged;
    
    /// <summary>
    /// Raised when the remote desktop size has changed.
    /// </summary>
    public event EventHandler? RemoteDesktopSizeChanged;

    /// <summary>
    /// Creates a new instance of the RdpControl.
    /// </summary>
    public RdpControl()
    {
        _timerResizeInProgress = new System.Windows.Forms.Timer
        {
            Interval = 1000
        };
        _timerSmartReconnect = new System.Windows.Forms.Timer
        {
            Interval = 1000
        };
        SetStyle(ControlStyles.Selectable, true);
        SetStyle(ControlStyles.ContainerControl, false);
        TabStop = true;
        TabIndex = 0;
    }

    /// <summary>
    /// The RdpClient instance is created here.
    /// </summary>
    /// <inheritdoc cref="OnLoad"/>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        RdpClient = RdpClientFactory.Create(RdpClientVersion);
        RegisterEvents();

        var control = (Control) RdpClient;
        control.Dock = DockStyle.Fill;
        Controls.Add(control);
    }

    /// <summary>
    /// In some situations, the immediate Dispose call can take a long time resulting in blocking the UI.
    /// Use the UnregisterEvents method to unsubscribe internal event wire-up and delay disposal.
    /// </summary>
    /// <inheritdoc cref="Dispose"/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            UnregisterEvents();
            
            _timerResizeInProgress.Dispose();
            _timerSmartReconnect.Dispose();

            RdpClient.Dispose();
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
        if (!_canScale || !AutoScaling)
            return;

        var newScaleFactor = LogicalToDeviceUnits(100);
        if (SetZoomLevel(newScaleFactor))
            _currentZoomLevel = newScaleFactor;
    }

    private void RegisterEvents()
    {
        _timerResizeInProgress.Tick += TimerResizeInProgress_Tick;
        _timerSmartReconnect.Tick += TimerSmartReconnect_Tick;
        RdpClient.OnConnected += RdpClient_OnConnected;
        RdpClient.OnDisconnected += RdpClient_OnDisconnected;
    }

    /// <summary>
    /// Unregisters internal events. Useful when delaying disposal of the control.
    /// </summary>
    public void UnregisterEvents()
    {
        _timerResizeInProgress.Tick -= TimerResizeInProgress_Tick;
        _timerSmartReconnect.Tick -= TimerSmartReconnect_Tick;
        RdpClient.OnConnected -= RdpClient_OnConnected;
        RdpClient.OnDisconnected -= RdpClient_OnDisconnected;
    }

    /// <summary>
    /// Helper method to set the logon credentials.
    /// </summary>
    /// <param name="username">The username (preferably without a domain component, except when provided as UPN)</param>
    /// <param name="domain">The user domain.</param>
    /// <param name="password">The password.</param>
    public void ApplyCredentials(string? username, string? domain, string? password)
    {
        RdpClient.UserName = string.IsNullOrEmpty(username) 
            ? null 
            : username;

        RdpClient.Domain = string.IsNullOrEmpty(domain) 
            ? null 
            : domain;

        if (!string.IsNullOrEmpty(password))
            RdpClient.Password = password;
    }

    /// <summary>
    /// Helper method to set the gateway credentials.
    /// </summary>
    /// <param name="username">The username (preferably without a domain component, except when provided as UPN)</param>
    /// <param name="domain">The user domain.</param>
    /// <param name="password">The password.</param>
    public void ApplyGatewayCredentials(string? username, string? domain, string? password)
    {
        if (!string.IsNullOrEmpty(username))
            RdpClient.GatewayUsername = username;

        if (!string.IsNullOrEmpty(domain))
            RdpClient.GatewayDomain = domain;

        if (!string.IsNullOrEmpty(password))
            RdpClient.GatewayPassword = password;
    }

    /// <summary>
    /// Starts the remote desktop connection. Make sure all properties, settings, events are correctly set.
    /// </summary>
    public void Connect()
    {
        _canScale = false;

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

        // set default values with no configuration
        RdpClient.ContainerHandledFullScreen = 1;
        RdpClient.RelativeMouseMode = true;
        RdpClient.GrabFocusOnConnect = false;

        ApplyInitialScaling();

        RdpClient.Connect();
    }

    /// <summary>
    /// Disconnects the remote desktop session.
    /// </summary>
    public void Disconnect()
    {
        if (RdpClient.ConnectionState == ConnectionState.Disconnected) 
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
    /// Ensure the ActiveX control updates the client size according to the control size.
    /// </summary>
    /// <returns></returns>
    public bool UpdateClientSize()
    {
        if (_previousClientSize.Equals(Size))
            return true;
            
        _previousClientSize = Size;

        return UpdateClientSizeWithoutReconnect() || UpdateClientSizeWithReconnect();
    }

    /// <summary>
    /// Set the focus to the RdpClient control.
    /// </summary>
    public void FocusRdpClient()
    {
        // notify "parent" that the rdp client control has been focused
        InvokeGotFocus(this, EventArgs.Empty);
        RdpClient.Focus();
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
            RdpClient.SendRemoteAction(action);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to send remote action: {Action}", action);
        }
    }

    private void RdpClient_OnConnected(object? sender, EventArgs e)
    {
        _previousClientSize = Size;
        ConnectedEventArgs? ea = null;
        if (SetFullScreenOnConnect)
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
        else if (UseLocalScaling)
        {
            BeginInvoke(new MethodInvoker(() => { SetZoomLevelLocal(_currentZoomLevel); }));
        }

        WasSuccessfullyConnected = true;

        OnConnected?.Invoke(sender, ea ?? new ConnectedEventArgs());
        _canScale = true;
    }

    private void RdpClient_OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
    {
        switch (e.discReason)
        {
            // ignore this one. a reconnect is in progress (RDP 8)
            case 4360:
                return;
            case 2825 when !RdpClient.NetworkLevelAuthentication && !_nlaReconnect:
                // NLA seems to be required and was not reconnected using the setting
                RdpClient.NetworkLevelAuthentication = true;
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

    private void ApplyInitialScaling()
    {
        _currentZoomLevel = InitialZoomLevel;

        if (UseLocalScaling)
            return;

        try
        {
            RdpClient.DeviceScaleFactor = GetDeviceScaleFactor(_currentZoomLevel); // must be 100, 140 or 180
            RdpClient.DesktopScaleFactor = GetDesktopScaleFactor(_currentZoomLevel); // must be 100, 125, 150 or 200
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Cannot apply initial scale factor {ScaleFactor}", _currentZoomLevel);
            _currentZoomLevel = 100;
        }
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

    private bool SetZoomLevel(int desiredZoomLevel)
    {
        return UseLocalScaling
            ? SetZoomLevelLocal(desiredZoomLevel)
            : SetZoomLevelRemote(desiredZoomLevel);
    }

    private bool SetZoomLevelRemote(int desiredZoomLevel)
    {
        try
        {
            RdpClient.UpdateSessionDisplaySettings(
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
            
        if (RdpClient.TrySetProperty("ZoomLevel", ref zl, out var ex))
            return true;

        Logger.LogWarning(ex,
            "Unable to set local zoom level to {ZoomLevel}. Not all RDP and OS versions support different DPIs in remote sessions. Check the log for more information",
            desiredZoomLevel);
        return false;
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
            200 => 250,
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
            250 => 200,
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

    /// <summary>
    /// Used to adapt the session size (if SmartReconnect is set to true).
    /// </summary>
    /// <inheritdoc cref="OnSizeChanged"/>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        if (!SmartReconnect)
            return;
        _timerResizeInProgress.Start();
    }

    private void TimerResizeInProgress_Tick(object? sender, EventArgs e)
    {
        if (MouseButtons == MouseButtons.Left)
            return;
        _timerResizeInProgress.Stop();
        RaiseRemoteDesktopSizeChanged();
    }

    private void TimerSmartReconnect_Tick(object? sender, EventArgs e)
    {
        if (MouseButtons == MouseButtons.Left)
            return;
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

        RemoteDesktopSizeChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Resets the keyboard buffer. Keys can get stuck when using certain key combinations.
    /// </summary>
    public void ResetKeyboardBuffer()
    {
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

    private bool UpdateClientSizeWithoutReconnect()
    {
        try
        {
            RdpClient.UpdateSessionDisplaySettings(
                (uint) Width,
                (uint) Height,
                0,
                0,
                0,
                GetDesktopScaleFactor(_currentZoomLevel),
                GetDeviceScaleFactor(_currentZoomLevel)
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
        var success = RdpClient.Reconnect((uint) Width, (uint) Height) == ControlReconnectStatus.controlReconnectStarted;
        Logger.LogDebug("UpdateClientSizeWithReconnect result: {Result}", success ? "Success" : "Failed");
        return success;
    }
}