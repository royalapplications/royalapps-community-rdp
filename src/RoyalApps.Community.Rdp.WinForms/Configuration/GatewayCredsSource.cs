namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Specifies or retrieves the Remote Desktop Gateway (RD Gateway) authentication method.
/// </summary>
public enum GatewayCredentialSource
{
    /// <summary>
    /// Use a password (NTLM) as the authentication method for RD Gateway.
    /// </summary>
    UsernameAndPassword = 0,
    /// <summary>
    /// Use a smart card as the authentication method for RD Gateway.
    /// </summary>
    SmartCard = 1,
    /// <summary>
    /// Use any authentication method for RD Gateway.
    /// </summary>
    Any = 4
}