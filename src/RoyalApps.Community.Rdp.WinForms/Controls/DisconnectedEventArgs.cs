using System;

namespace RoyalApps.Community.Rdp.WinForms.Controls;

/// <summary>
/// Event args for the OnDisconnected event. 
/// </summary>
public class DisconnectedEventArgs : EventArgs
{
    /// <summary>
    /// The disconnect code provided by the ActiveX control.
    /// </summary>
    public required int DisconnectCode { get; init; }
    /// <summary>
    /// The error description for the disconnect code.
    /// </summary>
    public required string Description { get; init; }
    /// <summary>
    /// True when the session was disconnected caused by an error.
    /// </summary>
    public required bool ShowError { get; init; }
    /// <summary>
    /// Has the session ended because of user interaction.
    /// </summary>
    public required bool UserInitiated { get; init; }
}