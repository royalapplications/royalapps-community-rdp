using System;

namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// The remote desktop client configuration.
/// </summary>
public class RdpClientConfiguration
{
    /// <summary>
    /// Specifies the name of the server to which the current control is connected.
    /// The new server name. This parameter can be a DNS name or IP address.
    /// This property must be set before calling the Connect method. It is the only property that must be set before connecting.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscax-server</cref>
    /// </seealso>
    public string? Server { get; set; }

    /// <summary>
    /// Specifies the connection port. The default value is 3389.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-rdpport</cref>
    /// </seealso>
    public int Port { get; set; } = 3389;

    /// <summary>
    /// The client version to use. By default (value is 0) the highest available client will be used.
    /// </summary>
    public int ClientVersion { get; set; }

    /// <summary>
    /// Specifies the names of virtual channel client DLLs to be loaded. Virtual channel client DLLs are also referred to as Plug-in DLLs.
    /// Comma-separated list of the names of the virtual channel client DLLs to be loaded. The DLL names must contain only alphanumeric characters.
    /// </summary>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imstscadvancedsettings-plugindlls</cref>
    /// </seealso>
    public string? PluginDlls { get; set; }

    /// <summary>
    /// If true, Microsoft's Remote Desktop Client is used (when installed) instead of the legacy MSTSC ActiveX control.
    /// </summary>
    public bool UseMsRdc { get; set; }

    /// <summary>
    /// If set, the Microsoft Remote Desktop Client files (rdclientax.dll) will be searched here at first.
    /// </summary>
    public string? MsRdcPath { get; set; }

    /// <summary>
    /// If true, a detailed log file will be written to the file system (see: LogFilePath)
    /// </summary>
    public bool LogEnabled { get; set; }

    /// <summary>
    /// The following log levels are available: TRACE, DEBUG, INFO, WARN, ERROR, FATAL, OFF
    /// </summary>
    public string LogLevel { get; set; } = "TRACE";

    /// <summary>
    /// The file path to the log file when LogEnabled is set to true.
    /// </summary>
    public string LogFilePath { get; set; } = Environment.ExpandEnvironmentVariables(@"%TEMP%\MsRdpEx.log");
    
    /// <summary>
    /// The credential configuration used to log on to the remote desktop session.
    /// </summary>
    public CredentialConfiguration Credentials { get; set; } = new();

    /// <summary>
    /// Connection related configuration and settings.
    /// </summary>
    public ConnectionConfiguration Connection { get; set; } = new();
    
    /// <summary>
    /// The display settings used for the remote desktop session.
    /// </summary>
    public DisplayConfiguration Display { get; set; } = new();

    /// <summary>
    /// Remote Desktop Gateway configuration and settings
    /// </summary>
    public GatewayConfiguration Gateway { get; set; } = new();

    /// <summary>
    /// Configuration and settings related to Hyper-V connections.
    /// </summary>
    public HyperVConfiguration HyperV { get; set; } = new();
    
    /// <summary>
    /// Input related configuration and settings.
    /// </summary>
    public InputConfiguration Input { get; set; } = new();
    
    /// <summary>
    /// Performance related configuration and settings.
    /// </summary>
    public PerformanceConfiguration Performance { get; set; } = new();

    /// <summary>
    /// Configuration related to a program to be started on the remote server upon connection.
    /// </summary>
    public ProgramConfiguration Program { get; set; } = new();
    
    /// <summary>
    /// Configuration and settings for device redirection.
    /// </summary>
    public RedirectionConfiguration Redirection { get; set; } = new();

    /// <summary>
    /// Security related settings for the remote desktop connection.
    /// </summary>
    public SecurityConfiguration Security { get; set; } = new();
}