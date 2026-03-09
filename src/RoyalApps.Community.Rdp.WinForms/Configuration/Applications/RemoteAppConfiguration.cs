using System.ComponentModel;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Applications;

/// <summary>
/// Configuration for launching a RemoteApp instead of a full desktop session.
/// Support: external sessions only.
/// External mode maps these values to standard <c>.rdp</c> properties.
/// Embedded mode is intentionally unsupported because RemoteApp windows are not truly hosted inside the control.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public class RemoteAppConfiguration : ExpandableObjectConverter
{
    /// <summary>
    /// Enables RemoteApp mode.
    /// Support: external sessions only.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Specifies the RemoteApp alias or executable name, for example <c>EXCEL</c>.
    /// Support: external sessions only.
    /// </summary>
    public string? Program { get; set; }

    /// <summary>
    /// Specifies the display name shown for the RemoteApp, for example <c>Microsoft Excel</c>.
    /// Support: external sessions only.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Optional command-line parameters passed to the RemoteApp.
    /// Support: external sessions only.
    /// </summary>
    public string? CommandLine { get; set; }

    /// <summary>
    /// Determines whether environment variables in <see cref="CommandLine"/> are expanded in the remote session.
    /// Support: external sessions only.
    /// </summary>
    public bool ExpandCommandLine { get; set; } = true;

    /// <summary>
    /// Specifies the working directory for the RemoteApp.
    /// In the generated <c>.rdp</c> file this maps to the shell working directory.
    /// Support: external sessions only.
    /// </summary>
    public string? WorkingDirectory { get; set; }

    /// <summary>
    /// Determines whether environment variables in <see cref="WorkingDirectory"/> are expanded in the remote session.
    /// Support: external sessions only.
    /// </summary>
    public bool ExpandWorkingDirectory { get; set; } = true;

    /// <summary>
    /// Optional file path to open with the RemoteApp.
    /// Support: external sessions only.
    /// </summary>
    public string? File { get; set; }

    /// <summary>
    /// Optional icon path shown by clients that support RemoteApp icon metadata.
    /// Support: external sessions only.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Disables the capability check for RemoteApp support.
    /// For external mode it is emitted as a custom <c>.rdp</c> property.
    /// Support: external sessions only.
    /// </summary>
    public bool DisableCapabilitiesCheck { get; set; }

    /// <summary>
    /// Returns an empty string so this configuration group appears as a blank expandable node in PropertyGrid-style editors.
    /// </summary>
    /// <returns>An empty string.</returns>
    public override string ToString()
    {
        return string.Empty;
    }
}
