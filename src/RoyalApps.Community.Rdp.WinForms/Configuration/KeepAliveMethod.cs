namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// The method used to perform keep-alive actions
/// </summary>
public enum KeepAliveMethod
{
    /// <summary>
    /// A subtle mouse movement (+1/-1 pixel) is performed
    /// </summary>
    MouseMove = 0,
    /// <summary>
    /// The F15 function key, which is probably ignored by all applications, is used to perform the keep-alive action
    /// </summary>
    KeyboardInput = 1
}