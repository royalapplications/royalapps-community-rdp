using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using AxMSTSCLib;
using Microsoft.Extensions.Logging;
using MsRdpEx;
using MSTSCLib;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Display;
using RoyalApps.Community.Rdp.WinForms.Configuration.Internal;
using RoyalApps.Community.Rdp.WinForms.Controls.ActiveX;
using RoyalApps.Community.Rdp.WinForms.Controls.Clients;
using RoyalApps.Community.Rdp.WinForms.Controls.Events;
using RoyalApps.Community.Rdp.WinForms.External.Runtime;
using RoyalApps.Community.Rdp.WinForms.Logging;
using Timer = System.Windows.Forms.Timer;

namespace RoyalApps.Community.Rdp.WinForms;

/// <summary>
/// A WinForms control that hosts an embedded RDP session or launches an external RDP client process.
/// </summary>
public class RdpControl : UserControl
{
    private HWND _outputPresenterHandle = HWND.Null;

    /// <summary>
    /// Gets or sets the logger used by the control.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ILogger Logger { get; set; } = DebugLoggerFactory.Create();

    /// <summary>
    /// Gets the last captured screenshot of the session, when session capture is enabled.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Bitmap? SessionCapture { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether a session screenshot should be captured once per second.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EnableSessionCapture
    {
        get => _sessionCaptureTimer.Enabled;
        set => _sessionCaptureTimer.Enabled = value;
    }

    private bool _canScale;
    private bool _nlaReconnect;
    private IMsRdpExInstance? _rdpInstance;
    private ExternalRdpSession? _externalSession;

    private int _currentZoomLevel = 100;
    private Size _previousClientSize = Size.Empty;

    private readonly Timer _timerResizeInProgress;
    private readonly Timer _sessionCaptureTimer;
    private readonly HashSet<string> _rdClientSearchPaths =
    [
        @"%ProgramFiles%\Remote Desktop",
        @"%ProgramFiles(x86)%\Remote Desktop",
        @"%ProgramFiles(ARM)%\Remote Desktop",
        @"%LocalAppData%\Apps\Remote Desktop"
    ];

    /// <summary>
    /// Access to the embedded RDP client and its events, methods, and properties.
    /// Returns <see langword="null"/> while <see cref="RdpConfiguration"/> uses <see cref="RdpSessionMode.External"/>.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IRdpClient? RdpClient { get; private set; }

    /// <summary>
    /// Gets or sets the configuration used to create and connect the RDP session.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RdpClientConfiguration RdpConfiguration { get; set; }

    /// <summary>
    /// Gets the current zoom level in percentage of the remote desktop session.
    /// </summary>
    public int CurrentZoomLevel => _currentZoomLevel;

    /// <summary>
    /// Gets a value indicating whether the session has connected successfully at least once.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool WasSuccessfullyConnected { get; private set; }

    /// <summary>
    /// Gets the external process ID while <see cref="RdpConfiguration"/> uses <see cref="RdpSessionMode.External"/> and the process is still running.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int? ExternalProcessId => _externalSession?.ProcessId;

    /// <summary>
    /// Gets the last known top-level external session window handle while <see cref="RdpConfiguration"/> uses <see cref="RdpSessionMode.External"/>.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public nint? ExternalSessionWindowHandle => _externalSession?.CurrentWindowHandle is { } windowHandle && windowHandle != IntPtr.Zero
        ? windowHandle
        : null;

    /// <summary>
    /// Raised when the session has started.
    /// </summary>
    public event EventHandler<ConnectedEventArgs>? OnConnected;

    /// <summary>
    /// Raised when the session has been disconnected or the external client process exits.
    /// </summary>
    public event EventHandler<DisconnectedEventArgs>? OnDisconnected;

    /// <summary>
    /// Raised when the tracked top-level window for an external session is discovered, changes, or disappears.
    /// </summary>
    public event EventHandler<ExternalWindowChangedEventArgs>? ExternalSessionWindowChanged;

    /// <summary>
    /// Raised when the user presses the Minimize button on the connection bar in full-screen mode.
    /// This is a request for the container application to minimize itself.
    /// </summary>
    public event EventHandler? OnRequestContainerMinimize;

    /// <summary>
    /// Raised when the client requests to leave full-screen mode or <c>ContainerHandledFullScreen</c> has been set to a nonzero value.
    /// </summary>
    public event EventHandler? OnRequestLeaveFullScreen;

    /// <summary>
    /// Raised when the client calls the RequestClose method.
    /// </summary>
    public event EventHandler<IMsTscAxEvents_OnConfirmCloseEvent>? OnConfirmClose;

    /// <summary>
    /// Raised when the user clicks inside the remote desktop session and click detection is enabled.
    /// </summary>
    public event EventHandler? OnClientAreaClicked;

    /// <summary>
    /// Raised before the remote desktop size changes.
    /// Set <see cref="CancelEventArgs.Cancel"/> to cancel the resize operation.
    /// </summary>
    public event EventHandler<CancelEventArgs>? BeforeRemoteDesktopSizeChanged;

    /// <summary>
    /// Raised when the remote desktop size has changed.
    /// </summary>
    public event EventHandler? RemoteDesktopSizeChanged;

    /// <summary>
    /// Raised after the <see cref="IRdpClient"/> has been created and configured, but before the connection is established.
    /// This allows additional customization that is not covered by <see cref="RdpConfiguration"/>.
    /// </summary>
    public event EventHandler? RdpClientConfigured;

    /// <summary>
    /// Initializes a new instance of the <see cref="RdpControl"/> class.
    /// </summary>
    public RdpControl()
    {
        RdpConfiguration = new();

        _timerResizeInProgress = new Timer
        {
            Interval = 1000
        };

        _sessionCaptureTimer = new Timer
        {
            Interval = 1000
        };
        _sessionCaptureTimer.Tick += SessionCaptureTimer_Tick;

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
            CleanupExternalSession();
            CleanupRdpClient();

            _sessionCaptureTimer.Tick -= SessionCaptureTimer_Tick;
            _sessionCaptureTimer.Dispose();

            _timerResizeInProgress.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Updates the zoom level when automatic scaling is enabled.
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
    /// Starts deferred resize handling when a resize mode updates the active session after the control size changes.
    /// </summary>
    /// <inheritdoc cref="OnSizeChanged"/>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        if (RdpConfiguration.Display.ResizeBehavior is not (ResizeBehavior.SmartReconnect or ResizeBehavior.UpdateDesktopSize))
            return;
        _timerResizeInProgress.Start();
    }

    /// <summary>
    /// Unregisters internal event handlers.
    /// </summary>
    public void UnregisterEvents()
    {
        if (_externalSession != null)
        {
            _externalSession.Connected -= ExternalSession_OnConnected;
            _externalSession.Disconnected -= ExternalSession_OnDisconnected;
            _externalSession.WindowChanged -= ExternalSession_OnWindowChanged;
        }

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
    /// Starts the remote desktop connection. Depending on <see cref="RdpClientConfiguration.SessionMode"/>, this either creates the embedded client control or launches an external client process.
    /// </summary>
    public void Connect() => Connect(RdpConnectionContextFactory.Create(RdpConfiguration));

    /// <summary>
    /// Disconnects the remote desktop session or requests the external client process to close.
    /// </summary>
    public void Disconnect()
    {
        if (IsExternalMode)
        {
            _externalSession?.Disconnect();
            return;
        }

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
    /// Moves focus to the embedded RDP client control.
    /// </summary>
    public void FocusRdpClient()
    {
        if (IsExternalMode)
            return;

        // notify "parent" that the rdp client control has been focused
        InvokeGotFocus(this, EventArgs.Empty);
        RdpClient?.Focus();
    }

    /// <summary>
    /// Resets the keyboard buffer. Keys can get stuck when using certain key combinations.
    /// </summary>
    public void ResetKeyboardBuffer()
    {
        if (IsExternalMode)
        {
            Logger.LogDebug("ResetKeyboardBuffer is not supported for external RDP sessions.");
            return;
        }

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
        if (IsExternalMode)
        {
            Logger.LogDebug("SendRemoteAction is not supported for external RDP sessions.");
            return;
        }

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
    /// Changes the resize behavior while connected.
    /// </summary>
    /// <param name="resizeBehavior">The desired resize behavior.</param>
    public void SetResizeBehavior(ResizeBehavior resizeBehavior)
    {
        RdpConfiguration.Display.ResizeBehavior = resizeBehavior;
        if (IsExternalMode)
            return;

        if (RdpClient is not {ConnectionState: ConnectionState.Connected})
            return;

        RdpClient.SmartSizing = resizeBehavior == ResizeBehavior.SmartSizing;

        if (resizeBehavior == ResizeBehavior.UpdateDesktopSize)
            UpdateClientSize();
    }

    /// <summary>
    /// Shows the connection information dialog.
    /// </summary>
    public void ShowConnectionInfo()
    {
        if (IsExternalMode)
        {
            Logger.LogDebug("ShowConnectionInfo is not supported for external RDP sessions.");
            return;
        }

        RdpClient?.ShowConnectionInformation = true;
    }

    /// <summary>
    /// Attempts to bring the external session window to the foreground.
    /// </summary>
    /// <returns><see langword="true"/> when a visible external window was found and activated; otherwise <see langword="false"/>.</returns>
    public bool ActivateExternalSessionWindow()
    {
        if (!IsExternalMode)
            return false;

        return _externalSession?.TryActivateWindow(out _) == true;
    }

    /// <summary>
    /// Attempts to resolve the current external session window handle.
    /// </summary>
    /// <param name="windowHandle">The resolved top-level window handle.</param>
    /// <returns><see langword="true"/> when a visible external window was found; otherwise <see langword="false"/>.</returns>
    public bool TryGetExternalSessionWindowHandle(out nint windowHandle)
    {
        if (IsExternalMode && _externalSession != null)
            return _externalSession.TryGetWindowHandle(out windowHandle);
        windowHandle = IntPtr.Zero;
        return false;

    }

    /// <summary>
    /// Updates the remote client size to match the control size.
    /// </summary>
    /// <returns><see langword="true"/> when the size update succeeds or is unnecessary; otherwise <see langword="false"/>.</returns>
    public bool UpdateClientSize()
    {
        if (IsExternalMode)
            return true;

        var currentClientSize = GetCurrentClientSize();
        if (currentClientSize.Width <= 0 || currentClientSize.Height <= 0)
            return true;

        if (_previousClientSize.Equals(currentClientSize))
            return true;

        _previousClientSize = currentClientSize;

        if (RdpConfiguration.Display.UseLocalScaling)
        {
            if (RdpConfiguration.Display.HasDesktopSize)
                return true;

            var scaleFactor = _currentZoomLevel / 100.00;
            var width = (int)(currentClientSize.Width / scaleFactor);
            var height = (int)(currentClientSize.Height / scaleFactor);
            var success = UpdateClientSizeWithoutReconnect(width, height, 100);
            if (success)
                SetZoomLevelLocal(_currentZoomLevel);
            return success;
        }

        return RdpConfiguration.Display.ResizeBehavior switch
        {
            ResizeBehavior.UpdateDesktopSize => UpdateClientSizeWithoutReconnect(),
            ResizeBehavior.SmartReconnect => UpdateClientSizeWithReconnect(),
            _ => true
        };
    }

    /// <summary>
    /// Increases the zoom level.
    /// </summary>
    public void ZoomIn()
    {
        if (IsExternalMode)
        {
            Logger.LogDebug("ZoomIn is not supported for external RDP sessions.");
            return;
        }

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
        if (IsExternalMode)
        {
            Logger.LogDebug("ZoomOut is not supported for external RDP sessions.");
            return;
        }

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
            _ => 100
        };
        if (SetZoomLevel(newScaleFactor))
            _currentZoomLevel = newScaleFactor;
    }

    private void RdpClient_OnConnected(object? sender, EventArgs e)
    {
        if (RdpClient == null)
            return;

        _previousClientSize = GetCurrentClientSize();
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

        OnConnected?.Invoke(
            sender,
            ea ?? new ConnectedEventArgs
            {
                SessionMode = RdpSessionMode.Embedded
            });
        _canScale = true;
    }

    private void RdpClient_OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
    {
        if (RdpClient == null)
            return;

        switch (e.discReason)
        {
            // ignore this one. a reconnection is in progress (RDP 8)
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

                // find out if we need to show messagebox
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
                    UserInitiated = userInitiated,
                    SessionMode = RdpSessionMode.Embedded
                };
                OnDisconnected?.Invoke(sender, ea);
                break;
        }
    }

    private void RdpClient_OnRequestContainerMinimize(object? sender, EventArgs e) => OnRequestContainerMinimize?.Invoke(sender, e);

    private void RdpClient_OnRequestLeaveFullScreen(object? sender, EventArgs e) => OnRequestLeaveFullScreen?.Invoke(sender, e);

    private void RdpClient_OnConfirmClose(object sender, IMsTscAxEvents_OnConfirmCloseEvent e) => OnConfirmClose?.Invoke(sender, e);

    private void RdpClient_OnClientAreaClicked(object? sender, EventArgs e) => OnClientAreaClicked?.Invoke(sender, e);

    private void ApplyInitialScaling()
    {
        _currentZoomLevel = RdpConfiguration.Display.AutoScaling
            ? LogicalToDeviceUnits(100) :
            RdpConfiguration.Display.InitialZoomLevel;

        var currentClientSize = GetCurrentClientSize();

        LogDebugSizing(
            "ApplyInitialScaling input",
            currentClientSize,
            RdpConfiguration.Display.DesktopWidth,
            RdpConfiguration.Display.DesktopHeight);

        if (RdpConfiguration.Display.UseLocalScaling)
        {
            if (RdpConfiguration.Display.HasDesktopSize)
                return;

            if (currentClientSize.Width <= 0 || currentClientSize.Height <= 0)
                return;

            var scaleFactor = _currentZoomLevel / 100.00;
            RdpClient!.DesktopWidth = (int)(currentClientSize.Width / scaleFactor);
            RdpClient.DesktopHeight = (int)(currentClientSize.Height / scaleFactor);
            LogDebugSizing(
                "ApplyInitialScaling local-scaling assigned",
                currentClientSize,
                RdpClient.DesktopWidth,
                RdpClient.DesktopHeight);
            return;
        }

        if (!RdpConfiguration.Display.HasDesktopSize)
        {
            RdpClient!.DesktopWidth = 0;
            RdpClient.DesktopHeight = 0;
            LogDebugSizing(
                "ApplyInitialScaling automatic-sizing preserved",
                currentClientSize,
                RdpClient.DesktopWidth,
                RdpClient.DesktopHeight);
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
        ReleaseOutputMirror();

        if (RdpClient == null)
            return;

        if (performDisconnect && RdpClient.ConnectionState != ConnectionState.Disconnected)
            RdpClient.Disconnect();

        UnregisterEvents();
        RdpClient.Dispose();
        RdpClient = null;
    }

    private void CleanupExternalSession()
    {
        if (_externalSession == null)
            return;

        _externalSession.Connected -= ExternalSession_OnConnected;
        _externalSession.Disconnected -= ExternalSession_OnDisconnected;
        _externalSession.WindowChanged -= ExternalSession_OnWindowChanged;
        _externalSession.Dispose();
        _externalSession = null;
    }

    private void CreateRdpClient(RdpConnectionContext connectionContext)
    {
        var msTscAxDllPath = GetMsTscAxDllPath();
        var msRdcAxDllPath = GetRdClientAxDllPath();
        var useMsRdc = RdpConfiguration.UseMsRdc && !string.IsNullOrWhiteSpace(msRdcAxDllPath);
        var enableMsRdpExHook = useMsRdc || EnableSessionCapture || RdpConfiguration.LogEnabled;

        RdpClient = RdpClientFactory.Create(
            RdpConfiguration.ClientVersion,
            enableMsRdpExHook && MsRdpExManager.Instance.AxHookEnabled ? MsRdpExManager.Instance.CoreApi.MsRdpExDllPath : null,
            useMsRdc);

        ((ISupportInitialize)RdpClient).BeginInit();
        ConfigureMsRdpExEnvironment(enableMsRdpExHook);

        var control = (Control) RdpClient;
        control.Dock = DockStyle.Fill;
        Controls.Add(control);
        LogDebugSizing(
            "CreateRdpClient attached",
            GetCurrentClientSize(),
            RdpConfiguration.Display.DesktopWidth,
            RdpConfiguration.Display.DesktopHeight);

        if (useMsRdc)
        {
            Logger.LogDebug("Microsoft Remote Desktop Client (rdclientax.dll) will be used");
            Environment.SetEnvironmentVariable("MSRDPEX_RDCLIENTAX_DLL", msRdcAxDllPath);
        }
        else
        {
            if (RdpConfiguration.UseMsRdc && string.IsNullOrWhiteSpace(msRdcAxDllPath))
                Logger.LogDebug("Microsoft Remote Desktop Client will not be used, rdclientax.dll was not found");

            if (enableMsRdpExHook)
                Environment.SetEnvironmentVariable("MSRDPEX_MSTSCAX_DLL", msTscAxDllPath);

            RdpConfiguration.UseMsRdc = false;
        }

        if (useMsRdc || EnableSessionCapture)
        {
            // The new output presenter is only needed for the modern rdclient ActiveX or explicit session capture.
            try
            {
                RdpClient.SetProperty(RdpProperties.RequestUseNewOutputPresenter, true);
            }
            catch
            {
                // ignored
            }
        }

        EmbeddedRdpClientConfigurationMapper.Apply(this, connectionContext);

        if (enableMsRdpExHook && EnableSessionCapture && MsRdpExManager.Instance.AxHookEnabled)
        {
            _outputPresenterHandle = GetOutputPresenterHandle(RdpClient.Handle);
            MsRdpExManager.Instance.CoreApi.iface.OpenInstanceForWindowHandle(_outputPresenterHandle, out var instance);
            _rdpInstance = (IMsRdpExInstance)instance;
            _rdpInstance.SetOutputMirrorEnabled(true);
        }

        RegisterEvents();
        ((ISupportInitialize)RdpClient).EndInit();
        EnsureEmbeddedClientLayout(control);
        LogDebugSizing(
            "CreateRdpClient prepared",
            GetCurrentClientSize(),
            RdpClient.DesktopWidth,
            RdpClient.DesktopHeight);
    }

    private void Connect(RdpConnectionContext connectionContext)
    {
        WasSuccessfullyConnected = false;
        _nlaReconnect = false;

        CleanupExternalSession();
        CleanupRdpClient(true);

        if (connectionContext.IsExternalMode)
        {
            CreateExternalSession();
            _externalSession!.Connect(connectionContext);
            return;
        }

        CreateRdpClient(connectionContext);

        _canScale = false;

        ApplyInitialScaling();

        RdpClientConfigured?.Invoke(this, EventArgs.Empty);
        EnsureEmbeddedClientLayout(RdpClient as Control);
        LogDebugSizing(
            "Connect pre-connect",
            GetCurrentClientSize(),
            RdpClient?.DesktopWidth ?? 0,
            RdpClient?.DesktopHeight ?? 0);

        if (RdpClient is null || RdpClient.ConnectionState != ConnectionState.Disconnected)
            return;

        RdpClient.Connect();
    }

    private void ConfigureMsRdpExEnvironment(bool enabled)
    {
        if (!enabled)
        {
            Environment.SetEnvironmentVariable("MSRDPEX_LOG_ENABLED", null);
            Environment.SetEnvironmentVariable("MSRDPEX_LOG_LEVEL", null);
            Environment.SetEnvironmentVariable("MSRDPEX_LOG_FILE_PATH", null);
            Environment.SetEnvironmentVariable("MSRDPEX_RDCLIENTAX_DLL", null);
            Environment.SetEnvironmentVariable("MSRDPEX_MSTSCAX_DLL", null);
            return;
        }

        Environment.SetEnvironmentVariable("MSRDPEX_LOG_ENABLED", RdpConfiguration.LogEnabled ? "1" : "0");
        Environment.SetEnvironmentVariable("MSRDPEX_LOG_LEVEL", RdpConfiguration.LogLevel);
        Environment.SetEnvironmentVariable("MSRDPEX_LOG_FILE_PATH", RdpConfiguration.LogFilePath);
    }

    private void ReleaseOutputMirror()
    {
        if (_rdpInstance is not null)
        {
            try
            {
                _rdpInstance.SetOutputMirrorEnabled(false);
            }
            catch
            {
                // ignored
            }

            try
            {
                if (Marshal.IsComObject(_rdpInstance))
                    Marshal.FinalReleaseComObject(_rdpInstance);
            }
            catch
            {
                // ignored
            }

            _rdpInstance = null;
        }

        _outputPresenterHandle = HWND.Null;
    }

    private void CreateExternalSession()
    {
        _externalSession = new ExternalRdpSession(Logger);
        _externalSession.Connected += ExternalSession_OnConnected;
        _externalSession.Disconnected += ExternalSession_OnDisconnected;
        _externalSession.WindowChanged += ExternalSession_OnWindowChanged;
    }

    private void ExternalSession_OnConnected(object? sender, EventArgs e)
    {
        WasSuccessfullyConnected = true;
        OnConnected?.Invoke(
            sender,
            new ConnectedEventArgs
            {
                SessionMode = RdpSessionMode.External,
                ExternalProcessId = _externalSession?.ProcessId
            });
    }

    private void ExternalSession_OnDisconnected(object? sender, DisconnectedEventArgs e)
    {
        OnDisconnected?.Invoke(sender, e);
    }

    private void ExternalSession_OnWindowChanged(object? sender, ExternalWindowChangedEventArgs e)
    {
        ExternalSessionWindowChanged?.Invoke(sender, e);
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
        var currentClientSize = GetCurrentClientSize();
        if (currentClientSize.Width <= 0 || currentClientSize.Height <= 0)
            return false;

        try
        {
            RdpClient!.UpdateSessionDisplaySettings(
                (uint) currentClientSize.Width,
                (uint) currentClientSize.Height,
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
        if (RdpClient is null)
            return false;

        Logger.LogDebug("Setting local zoom level to {ZoomLevel}", desiredZoomLevel);
        return RdpClient.SetProperty(RdpProperties.ZoomLevel, (uint)desiredZoomLevel);
    }

    private void TimerResizeInProgress_Tick(object? sender, EventArgs e)
    {
        if (MouseButtons == MouseButtons.Left)
            return;
        _timerResizeInProgress.Stop();
        RaiseRemoteDesktopSizeChanged();
    }

    private void SessionCaptureTimer_Tick(object? sender, EventArgs e)
    {
        if (RdpClient is null || RdpClient.Handle == IntPtr.Zero)
            return;

        Bitmap? screenshot = null;
        try
        {
            screenshot = GetBitmap();
        }
        catch (Exception exception)
        {
            Logger.LogDebug(exception, "Failed to capture session bitmap.");
            EnableSessionCapture = false;
        }

        if (screenshot is null)
            return;
        var oldScreenshot = SessionCapture;
        SessionCapture = screenshot;
        oldScreenshot?.Dispose();
    }

    private bool RaiseBeforeRemoteDesktopSizeChangedAndCancel()
    {
        var args = new CancelEventArgs();
        BeforeRemoteDesktopSizeChanged?.Invoke(this, args);
        return args.Cancel;
    }

    private void RaiseRemoteDesktopSizeChanged()
    {
        var currentClientSize = GetCurrentClientSize();

        // make sure that Size 0,0 (when minimized) is also ignored
        if (currentClientSize.Width == 0 ||
            currentClientSize.Height == 0 ||
            _previousClientSize.IsEmpty ||
            _previousClientSize.Equals(currentClientSize))
            return;

        if (RaiseBeforeRemoteDesktopSizeChangedAndCancel())
            return;

        UpdateClientSize();
        RemoteDesktopSizeChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool UpdateClientSizeWithoutReconnect()
    {
        var currentClientSize = GetCurrentClientSize();
        return UpdateClientSizeWithoutReconnect(currentClientSize.Width, currentClientSize.Height, _currentZoomLevel);
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
        var currentClientSize = GetCurrentClientSize();
        if (currentClientSize.Width <= 0 || currentClientSize.Height <= 0)
            return false;

        var success = RdpClient!.Reconnect((uint) currentClientSize.Width, (uint) currentClientSize.Height) == ControlReconnectStatus.controlReconnectStarted;
        Logger.LogDebug("UpdateClientSizeWithReconnect result: {Result}", success ? "Success" : "Failed");
        return success;
    }

    private Size GetCurrentClientSize()
    {
        if (RdpClient is Control { IsDisposed: false } activeXControl)
        {
            if (activeXControl.ClientSize is { Width: > 0, Height: > 0 })
                return activeXControl.ClientSize;

            if (activeXControl.Size is { Width: > 0, Height: > 0 })
                return activeXControl.Size;
        }

        if (ClientSize is { Width: > 0, Height: > 0 })
            return ClientSize;

        return Size;
    }

    private void EnsureEmbeddedClientLayout(Control? activeXControl)
    {
        SuspendLayout();

        try
        {
            CreateControl();
            PerformLayout();

            if (activeXControl is null || activeXControl.IsDisposed)
            {
                Update();
                return;
            }

            activeXControl.CreateControl();
            if (activeXControl.Dock != DockStyle.Fill)
                activeXControl.Dock = DockStyle.Fill;

            activeXControl.PerformLayout();
            activeXControl.Update();
            Update();
        }
        finally
        {
            ResumeLayout(true);
        }
    }

    private void LogDebugSizing(string stage, Size currentClientSize, int desktopWidth, int desktopHeight)
    {
        if (!Logger.IsEnabled(LogLevel.Debug))
            return;

        var activeXControl = RdpClient as Control;
        Logger.LogDebug(
            "RDP control sizing ({Stage}): currentClient={CurrentClientWidth}x{CurrentClientHeight}, outerClient={OuterClientWidth}x{OuterClientHeight}, outerSize={OuterWidth}x{OuterHeight}, activeXClient={ActiveXClientWidth}x{ActiveXClientHeight}, activeXSize={ActiveXWidth}x{ActiveXHeight}, configuredDesktop={DesktopWidth}x{DesktopHeight}, hasDesktopSize={HasDesktopSize}, localScaling={LocalScaling}, autoScaling={AutoScaling}, zoom={Zoom}, outerDeviceDpi={OuterDeviceDpi}, activeXDeviceDpi={ActiveXDeviceDpi}",
            stage,
            currentClientSize.Width,
            currentClientSize.Height,
            ClientSize.Width,
            ClientSize.Height,
            Width,
            Height,
            activeXControl?.ClientSize.Width ?? 0,
            activeXControl?.ClientSize.Height ?? 0,
            activeXControl?.Width ?? 0,
            activeXControl?.Height ?? 0,
            desktopWidth,
            desktopHeight,
            RdpConfiguration.Display.HasDesktopSize,
            RdpConfiguration.Display.UseLocalScaling,
            RdpConfiguration.Display.AutoScaling,
            _currentZoomLevel,
            DeviceDpi,
            activeXControl?.DeviceDpi ?? 0);
    }

    private Bitmap? GetBitmap()
    {
        if (_rdpInstance is null)
            return null;

        var hShadowDc = IntPtr.Zero;
        var hShadowBitmap = IntPtr.Zero;
        var shadowData = IntPtr.Zero;
        uint shadowWidth = 0;
        uint shadowHeight = 0;
        uint shadowStep = 0;

        var captureWidth = RdpConfiguration.Display.DesktopWidth > 0
            ? RdpConfiguration.Display.DesktopWidth
            : Width;

        var captureHeight = RdpConfiguration.Display.DesktopHeight > 0
            ? RdpConfiguration.Display.DesktopHeight
            : Height;

        if (!_rdpInstance.GetShadowBitmap(
                ref hShadowDc,
                ref hShadowBitmap,
                ref shadowData,
                ref shadowWidth,
                ref shadowHeight,
                ref shadowStep))
            return null;

        _rdpInstance.LockShadowBitmap();
        var bitmap = ShadowToBitmap(_outputPresenterHandle, new HDC(hShadowDc), (int)shadowWidth, (int)shadowHeight, captureWidth, captureHeight);
        _rdpInstance.UnlockShadowBitmap();

        return bitmap;
    }

    private Bitmap? ShadowToBitmap(HWND hWnd, HDC hShadowDc, int shadowWidth, int shadowHeight, int captureWidth, int captureHeight)
    {
        using var g = Graphics.FromHwnd(hWnd);

        if (shadowWidth == 0 || shadowHeight == 0)
            return null;

        var width = captureWidth;
        var height = captureHeight;

        var srcX = 0;
        var srcY = 0;
        var dstX = 0;
        var dstY = 0;
        float percent;
        var srcWidth = shadowWidth;
        var srcHeight = shadowHeight;

        var percentW = width / (float)srcWidth;
        var percentH = height / (float)srcHeight;
        if (percentH < percentW)
        {
            percent = percentH;
            dstX = Convert.ToInt16((width - srcWidth * percent) / 2);
        }
        else
        {
            percent = percentW;
            dstY = Convert.ToInt16((height - srcHeight * percent) / 2);
        }

        var dstWidth = (int)(srcWidth * percent);
        var dstHeight = (int)(srcHeight * percent);

        if (width < 1 || height < 1)
            return null;

        var bmp = new Bitmap(width, height, g);
        using var memoryGraphics = Graphics.FromImage(bmp);
        var dc = new HDC(memoryGraphics.GetHdc());

        PInvoke.SetStretchBltMode(dc, STRETCH_BLT_MODE.HALFTONE);

        if (!PInvoke.StretchBlt(
                dc,
                dstX,
                dstY,
                dstWidth,
                dstHeight,
                hShadowDc,
                srcX,
                srcY,
                srcWidth,
                srcHeight,
                ROP_CODE.SRCCOPY))
        {
            bmp = null;
        }

        memoryGraphics.ReleaseHdc(dc);

        return bmp;
    }

    private HWND GetOutputPresenterHandle(IntPtr rdpClientHandle)
    {
        var clientHandle = new HWND(rdpClientHandle);

        var hUIMainClassWnd = PInvoke.FindWindowEx(clientHandle, HWND.Null, "UIMainClass", null);
        var hUIContainerClassWnd = PInvoke.FindWindowEx(hUIMainClassWnd, HWND.Null, "UIContainerClass", null);
        var hOPContainerClassWnd = PInvoke.FindWindowEx(hUIContainerClassWnd, HWND.Null, "OPContainerClass", null);
        var hOPWindowClassWnd = PInvoke.FindWindowEx(hOPContainerClassWnd, HWND.Null, "OPWindowClass", null);

        if (hOPWindowClassWnd == HWND.Null)
            hOPWindowClassWnd = PInvoke.FindWindowEx(hOPContainerClassWnd, HWND.Null, "OPWindowClass_mstscax", null);

        if (hOPWindowClassWnd == HWND.Null)
            hOPWindowClassWnd = PInvoke.FindWindowEx(hOPContainerClassWnd, HWND.Null, "OPWindowClass_rdclientax", null);

        return hOPWindowClassWnd;
    }

    private bool IsExternalMode => RdpConfiguration.SessionMode == RdpSessionMode.External;
}
