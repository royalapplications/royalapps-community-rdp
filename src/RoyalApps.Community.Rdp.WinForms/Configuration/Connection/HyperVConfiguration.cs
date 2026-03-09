using System.ComponentModel;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Connection;

/// <summary>
/// Configuration and settings related to Hyper-V connections.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public class HyperVConfiguration : ExpandableObjectConverter
{
    /// <summary>
    /// The Hyper-V instance used to set the PCB property.
    /// </summary>
    public string? Instance { get; set; }

    /// <summary>
    /// The Hyper-V console RDP port.
    /// </summary>
    public int HyperVPort { get; set; } = 2179;

    /// <summary>
    /// When <see langword="true"/>, enhanced session mode is used when possible.
    /// </summary>
    public bool EnhancedSessionMode { get; set; }

    /// <summary>
    /// Returns an empty string so this configuration group appears as a blank expandable node in PropertyGrid-style editors.
    /// </summary>
    /// <returns>An empty string.</returns>
    public override string ToString()
    {
        return string.Empty;
    }
}
