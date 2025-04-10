namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// The behavior when the control is resized.
/// </summary>
public enum ResizeBehavior
{
    /// <summary>
    /// Can show scrollbars.
    /// </summary>
    Scrollbars,
    /// <summary>
    /// Can shrink remote desktop.
    /// </summary>
    SmartSizing,
    /// <summary>
    /// Reconnects and adapts remote desktop size.
    /// </summary>
    SmartReconnect
}