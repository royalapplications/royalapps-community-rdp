using System;
using System.Drawing;

namespace RoyalApps.Community.Rdp.WinForms;

/// <summary>
/// Event args for the OnConnected event.
/// </summary>
public class ConnectedEventArgs : EventArgs
{
    /// <summary>
    /// Is true when full screen for multiple screen was requested.
    /// </summary>
    public bool MultiMonFullScreen { get; init; }
    /// <summary>
    /// The bounds of the form to cover all screens for multi monitor fullscreen.
    /// </summary>
    public Rectangle Bounds { get; init; }
}