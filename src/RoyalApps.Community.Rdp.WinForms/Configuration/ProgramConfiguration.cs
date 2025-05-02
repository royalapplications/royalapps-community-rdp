using System.ComponentModel;

namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Configuration related to a program to be started on the remote server upon connection.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public class ProgramConfiguration : ExpandableObjectConverter
{
    /// <summary>
    /// Specifies the program to be started on the remote server upon connection.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscsecuredsettings-startprogram</cref>
    /// </see>
    public string? StartProgram { get; set; }

    /// <summary>
    /// Specifies the working directory of the start program.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscsecuredsettings-workdir</cref>
    /// </see>
    public string? WorkDir { get; set; }

    /// <summary>
    /// Specifies if programs launched with the StartProgram property should be maximized.
    /// </summary>
    /// <see>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-maximizeshell</cref>
    /// </see>
    public bool MaximizeShell { get; set; } = true;

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns>Empty string.</returns>
    public override string ToString()
    {
        return string.Empty;
    }
}
