namespace RoyalApps.Community.Rdp.WinForms.Configuration.Connection;

/// <summary>
/// Specifies the Remote Desktop Gateway (RD Gateway) credential source.
/// </summary>
public enum GatewayCredentialSource
{
    /// <summary>
    /// Use a user name and password.
    /// </summary>
    UsernameAndPassword = 0,
    /// <summary>
    /// Use a smart card.
    /// </summary>
    SmartCard = 1,
    /// <summary>
    /// Use the credentials of the currently signed-in user.
    /// </summary>
    UseLoggedOnUserCredentials = 2,

    /// <summary>
    /// Prompt the user for credentials.
    /// </summary>
    PromptForCredentials = 3,

    /// <summary>
    /// Let the user or client choose the credential source later.
    /// </summary>
    ChooseLater = 4,

    /// <summary>
    /// Compatibility alias for <see cref="ChooseLater"/>.
    /// </summary>
    Any = ChooseLater,

    /// <summary>
    /// Use cookie-based authentication.
    /// </summary>
    CookieBasedAuthentication = 5
}
