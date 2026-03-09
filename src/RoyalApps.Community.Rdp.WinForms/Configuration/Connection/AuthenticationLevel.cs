namespace RoyalApps.Community.Rdp.WinForms.Configuration.Connection;

/// <summary>
/// Specifies the server authentication level used for the connection.
/// </summary>
public enum AuthenticationLevel
{
    /// <summary>
    /// Do not authenticate the server.
    /// </summary>
    NoAuthenticationOfServer = 0,
    /// <summary>
    /// Require server authentication before the connection can proceed.
    /// </summary>
    ServerAuthenticationRequired = 1,
    /// <summary>
    /// Attempt server authentication and prompt the user if authentication fails.
    /// </summary>
    PromptIfAuthenticationFails = 2
}
