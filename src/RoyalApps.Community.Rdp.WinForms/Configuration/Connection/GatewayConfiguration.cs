using System.ComponentModel;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Connection;

/// <summary>
/// Remote Desktop Gateway configuration and settings.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public class GatewayConfiguration : ExpandableObjectConverter
{
    /// <summary>
    /// Specifies when to use a Remote Desktop Gateway (RD Gateway) server.
    /// Support: embedded and external sessions.
    /// External mode writes <c>gatewayusagemethod</c>.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayusagemethod</cref>
    /// </see>
    public GatewayUsageMethod GatewayUsageMethod { get; set; }

    /// <summary>
    /// Specifies whether to use default Remote Desktop Gateway (RD Gateway) settings.
    /// Support: embedded and external sessions.
    /// External mode writes <c>gatewayprofileusagemethod</c>.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayprofileusagemethod</cref>
    /// </see>
    public GatewayProfileUsageMethod GatewayProfileUsageMethod { get; set; }

    /// <summary>
    /// Specifies or retrieves the Remote Desktop Gateway (RD Gateway) authentication method.
    /// Support: embedded and external sessions.
    /// External mode writes <c>gatewaycredentialssource</c>.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewaycredssource</cref>
    /// </see>
    public GatewayCredentialSource GatewayCredsSource { get; set; }

    /// <summary>
    /// Sets or retrieves the user-specified Remote Desktop Gateway (RD Gateway) credential source.
    /// Support: embedded and external sessions.
    /// External mode writes <c>gatewayuserselectedcredentialssource</c>.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayuserselectedcredssource</cref>
    /// </see>
    public GatewayCredentialSource GatewayUserSelectedCredsSource { get; set; }

    /// <summary>
    /// Specifies or retrieves the setting for whether the Remote Desktop Gateway (RD Gateway) credential sharing feature is enabled. When the feature is enabled, the Remote Desktop ActiveX control tries to use the same credentials to authenticate to the Remote Desktop Session Host (RD Session Host) server and to the RD Gateway server.
    /// Support: embedded and external sessions.
    /// External mode writes <c>promptcredentialonce</c>.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewaycredsharing</cref>
    /// </see>
    public bool GatewayCredSharing { get; set; }

    /// <summary>
    /// Specifies the host name of the Remote Desktop Gateway (RD Gateway) server.
    /// Support: embedded and external sessions.
    /// External mode writes <c>gatewayhostname</c>.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayhostname</cref>
    /// </see>
    public string? GatewayHostname { get; set; }

    /// <summary>
    /// Specifies or retrieves the user name that is provided to the Remote Desktop Gateway (RD Gateway) server.
    /// Support: embedded and external sessions.
    /// External mode writes <c>gatewayusername</c>.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewayusername</cref>
    /// </see>
    public string? GatewayUsername { get; set; }

    /// <summary>
    /// Specifies or retrieves the domain name of a user that is provided to the Remote Desktop Gateway (RD Gateway) server.
    /// Support: embedded and external sessions.
    /// External mode writes <c>gatewaydomain</c>.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewaydomain</cref>
    /// </see>
    public string? GatewayDomain { get; set; }

    /// <summary>
    /// Specifies the password that is used to authenticate to the Remote Desktop Gateway (RD Gateway) server.
    /// Support: embedded sessions directly.
    /// External mode does not serialize the password into the generated <c>.rdp</c> file, but can stage it in Windows Credential Manager when credential manager integration is enabled.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewaypassword</cref>
    /// </see>
    [TypeConverter(typeof(SensitiveStringConverter))]
    public SensitiveString? GatewayPassword { get; set; }

    /// <summary>
    /// Returns an empty string so this configuration group appears as a blank expandable node in PropertyGrid-style editors.
    /// </summary>
    /// <returns>An empty string.</returns>
    public override string ToString()
    {
        return string.Empty;
    }
}
