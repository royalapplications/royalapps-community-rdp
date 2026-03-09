using System;
using System.Drawing;
using RoyalApps.Community.Rdp.WinForms.Configuration;

namespace RoyalApps.Community.Rdp.WinForms.Controls.Events;

/// <summary>
/// Provides data for the <see cref="RdpControl.OnConnected"/> event.
/// </summary>
public class ConnectedEventArgs : EventArgs
{
    /// <summary>
    /// Indicates how the session is hosted.
    /// </summary>
    public RdpSessionMode SessionMode { get; init; } = RdpSessionMode.Embedded;

    /// <summary>
    /// Gets a value indicating whether multi-monitor full-screen mode was requested.
    /// </summary>
    public bool MultiMonFullScreen { get; init; }
    /// <summary>
    /// Gets the bounds used to cover all screens in multi-monitor full-screen mode.
    /// </summary>
    public Rectangle Bounds { get; init; }

    /// <summary>
    /// Gets the external client process ID when <see cref="SessionMode"/> is <see cref="RdpSessionMode.External"/>.
    /// </summary>
    public int? ExternalProcessId { get; init; }
}
