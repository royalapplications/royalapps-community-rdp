using System;
using RoyalApps.Community.Rdp.WinForms.Configuration;

namespace RoyalApps.Community.Rdp.WinForms.Controls.Events;

/// <summary>
/// Provides data for the <see cref="RdpControl.OnDisconnected"/> event.
/// </summary>
public class DisconnectedEventArgs : EventArgs
{
    /// <summary>
    /// Indicates how the session was hosted.
    /// </summary>
    public RdpSessionMode SessionMode { get; init; } = RdpSessionMode.Embedded;

    /// <summary>
    /// Gets the disconnect code reported by the ActiveX control or external session wrapper.
    /// </summary>
    public required int DisconnectCode { get; init; }
    /// <summary>
    /// Gets the description associated with <see cref="DisconnectCode"/>.
    /// </summary>
    public required string Description { get; init; }
    /// <summary>
    /// Gets a value indicating whether the disconnect should be treated as an error.
    /// </summary>
    public required bool ShowError { get; init; }
    /// <summary>
    /// Gets a value indicating whether the session ended because of user interaction.
    /// </summary>
    public required bool UserInitiated { get; init; }

    /// <summary>
    /// Gets the external client process ID when <see cref="SessionMode"/> is <see cref="RdpSessionMode.External"/>.
    /// </summary>
    public int? ExternalProcessId { get; init; }
}
