using System.ComponentModel;
using RoyalApps.Community.Rdp.WinForms.Configuration.Connection;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Security;

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
    /// Specifies the service principal name (SPN) service class used when authenticating to the target.
    /// The default service class for RDP is typically <c>TERMSRV</c>.
    /// This is primarily useful when connecting to endpoints that register a non-default SPN, such as Hyper-V VM console sessions.
    /// Support: embedded sessions only.
    /// This setting is not written into generated external <c>.rdp</c> files.
    /// </summary>
    public string? AuthenticationServiceClass { get; set; }

    /// <summary>
    /// Prevents the client from delegating credentials to the remote server.
    /// This is a building block used by Remote Credential Guard and Restricted Administration / Restricted Logon scenarios.
    /// Support: embedded sessions directly, external sessions through MsRdpEx-specific mapping when <see cref="global::RoyalApps.Community.Rdp.WinForms.Configuration.External.ExternalSessionConfiguration.UseMsRdpExHooks"/> is enabled.
    /// </summary>
    public bool DisableCredentialsDelegation { get; set; }

    /// <summary>
    /// Enables authentication redirection back to the local device.
    /// This is a building block used by Remote Credential Guard and typically requires <see cref="DisableCredentialsDelegation"/>.
    /// Support: embedded sessions directly, external sessions through MsRdpEx-specific mapping when <see cref="global::RoyalApps.Community.Rdp.WinForms.Configuration.External.ExternalSessionConfiguration.UseMsRdpExHooks"/> is enabled.
    /// </summary>
    public bool RedirectedAuthentication { get; set; }

    /// <summary>
    /// Enables restricted logon mode.
    /// This is a building block used by Restricted Administration mode and typically requires <see cref="DisableCredentialsDelegation"/>.
    /// Support: embedded sessions directly, external sessions through MsRdpEx-specific mapping when <see cref="global::RoyalApps.Community.Rdp.WinForms.Configuration.External.ExternalSessionConfiguration.UseMsRdpExHooks"/> is enabled.
    /// </summary>
    public bool RestrictedLogon { get; set; }

    /// <summary>
    /// Remote Credential Guard helps protect credentials over a Remote Desktop connection by redirecting Kerberos requests back to the device that initiated the connection. It also provides single sign-on for Remote Desktop sessions. When enabled, <see cref="RedirectedAuthentication"/> and <see cref="DisableCredentialsDelegation"/> are set to <see langword="true"/>.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/security/identity-protection/remote-credential-guard</cref>
    /// </seealso>
    public bool RemoteCredentialGuard { get; set; }

    /// <summary>
    /// Connects to the remote PC or server in Restricted Administration mode. In this mode, credentials are not sent to the remote PC or server,
    /// which can help protect you if you connect to a compromised host. However, connections made from the remote PC might not be authenticated by other PCs
    /// and servers, which can affect app functionality and compatibility.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/archive/blogs/kfalde/restricted-admin-mode-for-rdp-in-windows-8-1-2012-r2</cref>
    /// </seealso>
    public bool RestrictedAdminMode { get; set; }

    /// <summary>
    /// Returns an empty string so this configuration group appears as a blank expandable node in PropertyGrid-style editors.
    /// </summary>
    /// <returns>An empty string.</returns>
    public override string ToString()
    {
        return string.Empty;
    }
}
