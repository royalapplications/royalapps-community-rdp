namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Configuration and settings related to Hyper-V connections.
/// </summary>
public class HyperVConfiguration
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
    /// If true, enhanced session mode is used when possible.
    /// </summary>
    public bool EnhancedSessionMode { get; set; }
}