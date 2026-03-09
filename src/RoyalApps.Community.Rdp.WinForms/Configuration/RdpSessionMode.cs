namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Selects how the Remote Desktop session is hosted.
/// </summary>
public enum RdpSessionMode
{
    /// <summary>
    /// Host the session inside the embedded ActiveX control in <see cref="global::RoyalApps.Community.Rdp.WinForms.RdpControl"/>.
    /// </summary>
    Embedded,

    /// <summary>
    /// Launch an external Remote Desktop client process and leave the control empty.
    /// </summary>
    External
}
