using System.ComponentModel;

namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Security related settings for the remote desktop connection.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public class SecurityConfiguration : ExpandableObjectConverter
{
    /// <summary>
    /// Specifies the authentication level to use for the connection.
    /// </summary>
    /// <seealso>
    ///     <cref>https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings4-authenticationlevel</cref>
    /// </seealso>
    public AuthenticationLevel AuthenticationLevel { get; set; } = AuthenticationLevel.NoAuthenticationOfServer;

    /// <summary>
    /// Retrieves or specifies whether the ActiveX control should attempt to connect to the server for administrative purposes.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings6-connecttoadministerserver</cref>
    /// </seealso>
    public bool ConnectToAdministerServer { get; set; }

    /// <summary>
    /// Sets or retrieves the configuration for public mode. Public mode prevents the client from caching user data to the local system.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings5-publicmode</cref>
    /// </seealso>
    public bool PublicMode { get; set; }

    /// <summary>
    /// Remote Credential Guard helps you protect your credentials over a Remote Desktop connection by redirecting Kerberos requests back to the device that's requesting the connection. It also provides single sign-on experiences for Remote Desktop sessions. When set to true, RedirectedAuthentication and DisableCredentialsDelegation is set to true.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/security/identity-protection/remote-credential-guard</cref>
    /// </seealso>
    public bool RemoteCredentialGuard { get; set; }

    /// <summary>
    /// Connects you to the remote PC or server in Restricted Administration mode. In this mode, credentials won't be sent to remote PC or server,
    /// which can protect you if you connect to a PC that has been compromised. However, connections made from the remote PC might not be authenticated by other PCs
    /// and servers, which might impact app functionality and compatibility.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/archive/blogs/kfalde/restricted-admin-mode-for-rdp-in-windows-8-1-2012-r2</cref>
    /// </seealso>
    public bool RestrictedAdminMode { get; set; }

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns>Empty string.</returns>
    public override string ToString()
    {
        return string.Empty;
    }
}
