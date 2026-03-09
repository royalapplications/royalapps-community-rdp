using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using RoyalApps.Community.Rdp.WinForms.External.Files;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.External;

/// <summary>
/// Configuration for launching an external Remote Desktop client process.
/// Support: external sessions only.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class ExternalSessionConfiguration
{
    /// <summary>
    /// Gets or sets the full path to <c>mstsc.exe</c>. When not set, the system default is used.
    /// </summary>
    public string? MstscPath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the external connection should be launched through <c>mstscex.exe</c> when it is available.
    /// </summary>
    public bool UseMsRdpExHooks { get; set; }

    /// <summary>
    /// Gets or sets the full path to <c>mstscex.exe</c>. When not set, well-known installation folders are probed.
    /// </summary>
    public string? MsRdpExLauncherPath { get; set; }

    /// <summary>
    /// Gets additional directories or full file paths that should be probed for <c>mstscex.exe</c> before the built-in well-known installation folders.
    /// Support: external sessions through MsRdpEx only.
    /// This is intended for app-local deployment scenarios where the MsRdpEx release files are shipped next to the host application.
    /// </summary>
    public Collection<string> MsRdpExSearchPaths { get; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether a credential should be materialized in the Windows credential manager before launch.
    /// </summary>
    public bool UseCredentialManager { get; set; } = true;

    /// <summary>
    /// Gets or sets the credential target name. When not set, a <c>TERMSRV/...</c> target is derived from the configured server and port.
    /// </summary>
    public string? CredentialTarget { get; set; }

    /// <summary>
    /// Gets or sets the directory used for the temporary <c>.rdp</c> file. When not set, the system temp directory is used.
    /// </summary>
    public string? TemporaryDirectory { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the generated <c>.rdp</c> file should be kept after the session ends.
    /// </summary>
    public bool KeepTemporaryRdpFile { get; set; }

    /// <summary>
    /// Gets or sets additional command-line arguments appended after the generated <c>.rdp</c> file path.
    /// </summary>
    public string? AdditionalArguments { get; set; }

    /// <summary>
    /// Gets or sets the comma-separated monitor identifiers written to the <c>selectedmonitors</c> RDP property.
    /// This setting is only applied in external mode and requires multi-monitor mode.
    /// Monitor identifiers can be discovered with <c>mstsc.exe /l</c>.
    /// Embedded mode rejects this value instead of trying to approximate the monitor layout.
    /// </summary>
    public string? SelectedMonitors { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the external process should be assigned to a Windows job so it is terminated when the host process exits unexpectedly.
    /// </summary>
    public bool KillProcessOnHostExit { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="System.Diagnostics.Process.Kill(bool)"/> should be used if graceful shutdown does not finish within <see cref="CloseTimeout"/>.
    /// </summary>
    public bool KillProcessOnDisconnect { get; set; } = true;

    /// <summary>
    /// Gets or sets the amount of time to wait for a graceful shutdown after requesting the external process to close.
    /// </summary>
    public TimeSpan CloseTimeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets additional standard RDP settings that are appended after the built-in mapping.
    /// </summary>
    public Collection<RdpFileSetting> AdditionalRdpSettings { get; } = [];

    /// <summary>
    /// Gets typed MsRdpEx-specific settings used when <see cref="UseMsRdpExHooks"/> is enabled.
    /// Support: external sessions through MsRdpEx only.
    /// </summary>
    public MsRdpExConfiguration MsRdpEx { get; set; } = new();

    /// <summary>
    /// Gets additional MsRdpEx-specific RDP settings that are appended after the built-in mapping when <see cref="UseMsRdpExHooks"/> is enabled.
    /// </summary>
    public Collection<RdpFileSetting> AdditionalMsRdpExSettings { get; } = [];

    /// <summary>
    /// Returns an empty string so this configuration group appears as a blank expandable node in PropertyGrid-style editors.
    /// </summary>
    /// <returns>An empty string.</returns>
    public override string ToString()
    {
        return string.Empty;
    }
}
