namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Specifies the audio redirection settings, which specify whether to redirect sounds or play sounds at the Remote Desktop Session Host (RD Session Host) server.
/// </summary>
public enum AudioRedirectionMode
{
    /// <summary>
    /// Redirect sounds to the client.This is the default value.
    /// </summary>
    RedirectToClient = 0,
    /// <summary>
    /// Play sounds at the remote computer.
    /// </summary>
    PlayRemote = 1,
    /// <summary>
    /// Disable sound redirection; do not play sounds at the server.
    /// </summary>
    DisableSound = 2
}
