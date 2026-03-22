namespace RoyalApps.Community.Rdp.WinForms.Configuration.Display;

/// <summary>
/// The behavior when the control is resized.
/// </summary>
public enum ResizeBehavior
{
    /// <summary>
    /// Keep the remote desktop size and show scroll bars if needed.
    /// </summary>
    Scrollbars,
    /// <summary>
    /// Scale the remote desktop to fit the control.
    /// </summary>
    SmartSizing,
    /// <summary>
    /// Update the active session display size without reconnecting.
    /// </summary>
    UpdateDesktopSize,
    /// <summary>
    /// Reconnect and adapt the remote desktop size to the control.
    /// </summary>
    SmartReconnect
}
