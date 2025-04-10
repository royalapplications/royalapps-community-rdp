namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Specifies the authentication level to use for the connection.
/// </summary>
public enum AuthenticationLevel
{
    /// <summary>
    /// No authentication of the server.
    /// </summary>
    NoAuthenticationOfServer = 0,
    /// <summary>
    /// Server authentication is required and must complete successfully for the connection to proceed.
    /// </summary>
    ServerAuthenticationRequired = 1,
    /// <summary>
    /// Attempt authentication of the server. If authentication fails, the user will be prompted with the option to cancel the connection or to proceed without server authentication.
    /// </summary>
    PromptIfAuthenticationFails = 2
}
