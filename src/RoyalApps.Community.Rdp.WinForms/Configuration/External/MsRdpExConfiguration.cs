using System.ComponentModel;
using RoyalApps.Community.Rdp.WinForms.Configuration.Input;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.External;

/// <summary>
/// Typed MsRdpEx-specific settings used for external sessions when <see cref="ExternalSessionConfiguration.UseMsRdpExHooks"/> is enabled.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class MsRdpExConfiguration
{
    /// <summary>
    /// Gets or sets the KDC proxy URL used for Kerberos over HTTPS scenarios.
    /// Support: external sessions through MsRdpEx only.
    /// Written as <c>KDCProxyURL</c> when <see cref="ExternalSessionConfiguration.UseMsRdpExHooks"/> is enabled.
    /// </summary>
    public string? KdcProxyUrl { get; set; }

    /// <summary>
    /// Gets or sets the server name that should be presented to the remote endpoint instead of the transport address.
    /// Support: external sessions through MsRdpEx only.
    /// Written as <c>UserSpecifiedServerName</c> when <see cref="ExternalSessionConfiguration.UseMsRdpExHooks"/> is enabled.
    /// </summary>
    public string? UserSpecifiedServerName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether MsRdpEx should enable its mouse jiggler keep-alive behavior.
    /// Support: external sessions through MsRdpEx only.
    /// Written as <c>EnableMouseJiggler</c> when <see cref="ExternalSessionConfiguration.UseMsRdpExHooks"/> is enabled.
    /// </summary>
    public bool EnableMouseJiggler { get; set; }

    /// <summary>
    /// Gets or sets the mouse jiggler interval in seconds.
    /// Support: external sessions through MsRdpEx only.
    /// Written as <c>MouseJigglerInterval</c> when <see cref="ExternalSessionConfiguration.UseMsRdpExHooks"/> is enabled and <see cref="EnableMouseJiggler"/> is true.
    /// MsRdpEx uses its own default interval when this value is zero or negative.
    /// </summary>
    public int MouseJigglerInterval { get; set; }

    /// <summary>
    /// Gets or sets the mouse jiggler method used by MsRdpEx.
    /// Support: external sessions through MsRdpEx only.
    /// Written as <c>MouseJigglerMethod</c> when <see cref="ExternalSessionConfiguration.UseMsRdpExHooks"/> is enabled and <see cref="EnableMouseJiggler"/> is true.
    /// </summary>
    public KeepAliveMethod MouseJigglerMethod { get; set; } = KeepAliveMethod.MouseMove;

    /// <summary>
    /// Returns an empty string so this configuration group appears as a blank expandable node in PropertyGrid-style editors.
    /// </summary>
    /// <returns>An empty string.</returns>
    public override string ToString()
    {
        return string.Empty;
    }
}
