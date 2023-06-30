using System.ComponentModel;

namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// The credential configuration used to log on to the remote desktop session.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public class CredentialConfiguration : ExpandableObjectConverter
{
    /// <summary>
    /// Specifies the user name logon credential.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-username</cref>
    /// </seealso>
    public string? Username { get; set; }

    /// <summary>
    /// Specifies the domain to which the current user logs on.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-domain</cref>
    /// </seealso>
    public string? Domain { get; set; }

    /// <summary>
    /// Sets the Remote Desktop ActiveX control password in plaintext format.
    /// The password is passed to the server in the safely encrypted RDP communications channel. After a plaintext password is set, it cannot be retrieved in plaintext format.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscnonscriptable-cleartextpassword</cref>
    /// </seealso>
    [TypeConverter(typeof(SensitiveStringConverter))]
    public SensitiveString? Password { get; set; }

    /// <summary>
    /// This property is not supported in this version.
    /// Specifies whether the Credential Security Service Provider (CredSSP) is enabled for this connection.
    /// This property is only supported by Remote Desktop Connection 6.1 and 7.0 clients.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings6-enablecredsspsupport</cref>
    /// </seealso>
    public bool NetworkLevelAuthentication { get; set; }

    /// <summary>
    /// Indicates that the password contains a smart card personal identification number (PIN). Minimum required RDP Version 6 or higher.
    /// Experimental feature!
    /// </summary>
    public bool PasswordContainsSmartCardPin { get; set; }

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns>Empty string.</returns>
    public override string ToString()
    {
        return string.Empty;
    }
}