using System.ComponentModel;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Applications;

/// <summary>
/// Configuration related to a program to be started on the remote server upon connection.
/// This models alternate-shell startup for full desktop sessions, not RemoteApp.
/// Support: embedded and external desktop sessions.
/// This configuration cannot be combined with <see cref="RdpClientConfiguration.RemoteApp"/>.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public class ProgramConfiguration : ExpandableObjectConverter
{
    /// <summary>
    /// Specifies the program to be started on the remote server upon connection.
    /// Support: embedded and external desktop sessions.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscsecuredsettings-startprogram</cref>
    /// </see>
    public string? StartProgram { get; set; }

    /// <summary>
    /// Specifies the working directory of the start program.
    /// Support: embedded and external desktop sessions.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscsecuredsettings-workdir</cref>
    /// </see>
    public string? WorkDir { get; set; }

    /// <summary>
    /// Specifies whether programs launched with <see cref="StartProgram"/> should be maximized.
    /// Support: embedded and external desktop sessions.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-maximizeshell</cref>
    /// </see>
    public bool MaximizeShell { get; set; } = true;

    /// <summary>
    /// Returns an empty string so this configuration group appears as a blank expandable node in PropertyGrid-style editors.
    /// </summary>
    /// <returns>An empty string.</returns>
    public override string ToString()
    {
        return string.Empty;
    }
}
