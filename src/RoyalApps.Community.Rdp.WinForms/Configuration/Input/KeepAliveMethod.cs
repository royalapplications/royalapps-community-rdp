namespace RoyalApps.Community.Rdp.WinForms.Configuration.Input;

/// <summary>
/// Specifies how keep-alive actions are simulated.
/// </summary>
public enum KeepAliveMethod
{
    /// <summary>
    /// Simulate a subtle mouse movement.
    /// </summary>
    MouseMove = 0,
    /// <summary>
    /// Simulate an <c>F15</c> key press.
    /// </summary>
    KeyboardInput = 1
}
