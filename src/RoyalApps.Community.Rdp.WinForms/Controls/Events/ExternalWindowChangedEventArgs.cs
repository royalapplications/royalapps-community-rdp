using System;

namespace RoyalApps.Community.Rdp.WinForms.Controls.Events;

/// <summary>
/// Provides data for changes to the tracked top-level window of an external RDP session.
/// </summary>
public sealed class ExternalWindowChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the external client process ID when one is available.
    /// </summary>
    public int? ExternalProcessId { get; init; }

    /// <summary>
    /// Gets the resolved top-level window handle.
    /// This is <see cref="IntPtr.Zero"/> when no visible window is currently tracked.
    /// </summary>
    public nint WindowHandle { get; init; }

    /// <summary>
    /// Gets a value indicating whether a visible top-level window is currently tracked.
    /// </summary>
    public bool HasWindow => WindowHandle != IntPtr.Zero;
}
