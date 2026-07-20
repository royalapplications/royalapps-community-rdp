using System.ComponentModel;
using RoyalApps.Community.Rdp.WinForms.Configuration.Input;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Connection;

/// <summary>
/// Connection-related configuration and settings.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public class ConnectionConfiguration : ExpandableObjectConverter
{
    /// <summary>
    /// Specifies whether compression is enabled.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-compress</cref>
    /// </seealso>
    public bool Compression { get; set; }

    /// <summary>
    /// Specifies whether to enable the client control to reconnect automatically to a session in the event of a network disconnection.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings2-enableautoreconnect</cref>
    /// </seealso>
    public bool EnableAutoReconnect { get; set; }

    /// <summary>
    /// Specifies the number of times to try to reconnect during automatic reconnection. Valid values are 0 to 200.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings2-maxreconnectattempts</cref>
    /// </seealso>
    public int MaxReconnectAttempts { get; set; }

    /// <summary>
    /// Disables RDP UDP transport and forces TCP transport. This is commonly useful when connecting through UDP-based VPN links.
    /// </summary>
    public bool DisableUdpTransport { get; set; }

    /// <summary>
    /// Specifies whether the redirected server name should be used when the broker redirects the connection.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdppreferredredirectioninfo-useredirectionservername</cref>
    /// </seealso>
    public bool UseRedirectionServerName { get; set; }

    /// <summary>
    /// Specifies the load balancing cookie that will be placed in the X.224 Connection Request packet in the Remote Desktop Session Host (RD Session Host) server protocol connection sequence.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-loadbalanceinfo</cref>
    /// </seealso>
    public string? LoadBalanceInfo { get; set; }

    /// <summary>
    /// Gets or sets the KDC proxy URL used for Kerberos over HTTPS scenarios.
    /// Support: external sessions through MsRdpEx and embedded ActiveX sessions hosted through the MsRdpEx hook.
    /// Written or applied as <c>KDCProxyURL</c>.
    /// </summary>
    public string? KdcProxyUrl { get; set; }

    /// <summary>
    /// Gets or sets the server name that should be presented to the remote endpoint instead of the transport address.
    /// Support: external sessions through MsRdpEx only.
    /// Written as <c>UserSpecifiedServerName</c> when external MsRdpEx hooks are enabled.
    /// </summary>
    public string? UserSpecifiedServerName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether MsRdpEx should enable its mouse jiggler keep-alive behavior.
    /// Support: external sessions through MsRdpEx only.
    /// Written as <c>EnableMouseJiggler</c> when external MsRdpEx hooks are enabled.
    /// </summary>
    public bool EnableMouseJiggler { get; set; }

    /// <summary>
    /// Gets or sets the mouse jiggler interval in seconds.
    /// Support: external sessions through MsRdpEx only.
    /// Written as <c>MouseJigglerInterval</c> when external MsRdpEx hooks are enabled and <see cref="EnableMouseJiggler"/> is true.
    /// MsRdpEx uses its own default interval when this value is zero or negative.
    /// </summary>
    public int MouseJigglerInterval { get; set; }

    /// <summary>
    /// Gets or sets the mouse jiggler method used by MsRdpEx.
    /// Support: external sessions through MsRdpEx only.
    /// Written as <c>MouseJigglerMethod</c> when external MsRdpEx hooks are enabled and <see cref="EnableMouseJiggler"/> is true.
    /// </summary>
    public KeepAliveMethod MouseJigglerMethod { get; set; } = KeepAliveMethod.MouseMove;

    /// <summary>
    /// The interval in seconds between keep-alive packets.
    /// </summary>
    /// <seealso>
    ///     <cref>https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-keepaliveinterval</cref>
    /// </seealso>
    public int? ConnectionKeepAliveInterval { get; set; }

    /// <summary>
    /// When <see langword="true"/>, the session is kept alive through periodic, non-intrusive input simulation to help avoid idle-time disconnects.
    /// </summary>
    public bool KeepAlive { get; set; }

    /// <summary>
    /// Gets or sets the interval, in seconds, between keep-alive actions.
    /// </summary>
    public int KeepAliveInterval { get; set; } = 60;

    /// <summary>
    /// Gets or sets the method used to perform the keep-alive action.
    /// </summary>
    public KeepAliveMethod KeepAliveMethod { get; set; } = KeepAliveMethod.MouseMove;

    /// <summary>
    /// Determines whether the client will use Microsoft Entra ID to authenticate to the remote PC. In Azure Virtual Desktop, this provides a single sign-on experience.
    /// Support: embedded and external sessions.
    /// External mode writes the standard <c>enablerdsaadauth</c> RDP property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For embedded sessions, the host executable must embed a Windows compatibility manifest that declares Windows 10 and Windows 11 support with the
    /// <c>supportedOS</c> GUID <c>{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}</c>.
    /// </para>
    /// <para>
    /// Without this declaration, the ActiveX control can accept <c>EnableRdsAadAuth</c> while presenting the classic Windows Security credential prompt
    /// instead of the Microsoft Entra Web Account Manager flow.
    /// </para>
    /// <para>
    /// External sessions are not subject to the host application's manifest because <c>mstsc.exe</c> supplies its own compatibility manifest.
    /// </para>
    /// </remarks>
    /// <see>
    ///     <cref>https://learn.microsoft.com/en-us/windows/win32/sbscs/application-manifests#supportedos</cref>
    /// </see>
    public bool EnableRdsAadAuth { get; set; }

    /// <summary>
    /// Returns an empty string, so this configuration group appears as a blank expandable node in PropertyGrid-style editors.
    /// </summary>
    /// <returns>An empty string.</returns>
    public override string ToString()
    {
        return string.Empty;
    }
}
