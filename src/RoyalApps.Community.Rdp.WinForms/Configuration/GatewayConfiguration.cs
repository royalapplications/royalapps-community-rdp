namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Remote Desktop Gateway configuration and settings
/// </summary>
public class GatewayConfiguration
{
    /// <summary>
    /// Specifies when to use a Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayusagemethod</cref>
    /// </see>
    public GatewayUsageMethod GatewayUsageMethod { get; set; }

    /// <summary>
    /// Specifies whether to use default Remote Desktop Gateway (RD Gateway) settings.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayprofileusagemethod</cref>
    /// </see>
    public GatewayProfileUsageMethod GatewayProfileUsageMethod { get; set; }

    /// <summary>
    /// Specifies or retrieves the Remote Desktop Gateway (RD Gateway) authentication method.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewaycredssource</cref>
    /// </see>
    public GatewayCredentialSource GatewayCredsSource { get; set; }

    /// <summary>
    /// Sets or retrieves the user-specified Remote Desktop Gateway (RD Gateway) credential source.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayuserselectedcredssource</cref>
    /// </see>
    public GatewayCredentialSource GatewayUserSelectedCredsSource { get; set; }

    /// <summary>
    /// Specifies or retrieves the setting for whether the Remote Desktop Gateway (RD Gateway) credential sharing feature is enabled. When the feature is enabled, the Remote Desktop ActiveX control tries to use the same credentials to authenticate to the Remote Desktop Session Host (RD Session Host) server and to the RD Gateway server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewaycredsharing</cref>
    /// </see>
    public bool GatewayCredSharing { get; set; }

    /// <summary>
    /// Specifies the host name of the Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings-gatewayhostname</cref>
    /// </see>
    public string? GatewayHostname { get; set; }

    /// <summary>
    /// Specifies or retrieves the user name that is provided to the Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewayusername</cref>
    /// </see>
    public string? GatewayUsername { get; set; }

    /// <summary>
    /// Specifies or retrieves the domain name of a user that is provided to the Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewaydomain</cref>
    /// </see>
    public string? GatewayDomain { get; set; }

    /// <summary>
    /// Specifies the password that a user provides to connect to the Remote Desktop Gateway (RD Gateway) server.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclienttransportsettings2-gatewaypassword</cref>
    /// </see>
    public string? GatewayPassword { get; set; }
}