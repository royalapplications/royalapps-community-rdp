namespace RoyalApps.Community.Rdp.WinForms.Configuration.Performance;

/// <summary>
/// Specifies how audio output is handled for the remote session.
/// </summary>
public enum AudioRedirectionMode
{
    /// <summary>
    /// Redirect sounds to the client.
    /// </summary>
    RedirectToClient = 0,
    /// <summary>
    /// Play sounds on the remote computer.
    /// </summary>
    PlayRemote = 1,
    /// <summary>
    /// Disable audio redirection.
    /// </summary>
    DisableSound = 2
}
