using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Microsoft.Extensions.Logging;
using MSTSCLib;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Interfaces;

namespace RoyalApps.Community.Rdp.WinForms.Controls;

internal static class RdpClientExtensions
{
    public static readonly string DesktopScaleFactor = "DesktopScaleFactor";
    public static readonly string DeviceScaleFactor = "DeviceScaleFactor";
    public static readonly string DisableCredentialsDelegation = "DisableCredentialsDelegation";
    public static readonly string DisableUdpTransport = "DisableUDPTransport";
    public static readonly string EnableHardwareMode = "EnableHardwareMode";
    public static readonly string PasswordContainsSmartcardPin = "PasswordContainsSCardPin";
    public static readonly string RestrictedLogon = "RestrictedLogon";
    public static readonly string RedirectedAuthentication = "RedirectedAuthentication";

    /// <summary>
    /// Applies the RdpClientConfiguration to the RdpClient.
    /// </summary>
    /// <param name="rdpControl">The RdpControl instance.</param>
    /// <param name="configuration">The RdpClientConfiguration instance.</param>
    public static void ApplyRdpClientConfiguration(this RdpControl rdpControl, RdpClientConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrEmpty(configuration.Server, nameof(configuration.Server));

        var rdpClient = rdpControl.RdpClient;
        var logger = rdpControl.Logger;

        if (rdpClient is null)
            throw new InvalidOperationException("Cannot apply configuration because there is no IRdpClient instance.");
        
        try
        {
            logger.LogTrace("Set {Server} and {Port}", configuration.Server, configuration.Port);
            rdpClient.Server = configuration.Server.Trim();
            rdpClient.Port = configuration.Port;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "The computer name or IP address is invalid");
        }

        if (!string.IsNullOrWhiteSpace(configuration.PluginDlls))
        {
            logger.LogTrace("Set {PluginDlls}", configuration.PluginDlls);
            rdpClient.PluginDlls = configuration.PluginDlls;
        }

        TraceConfigurationData(logger, configuration.Credentials);
        if (!string.IsNullOrEmpty(configuration.Credentials.Username))
            rdpClient.UserName = configuration.Credentials.Username;
        if (!string.IsNullOrEmpty(configuration.Credentials.Domain))
            rdpClient.Domain = configuration.Credentials.Domain;
        var password = configuration.Credentials.Password?.GetValue();
        if (!string.IsNullOrEmpty(password))
            rdpClient.Password = password;
        
        rdpClient.NetworkLevelAuthentication = configuration.Credentials.NetworkLevelAuthentication;

        if (configuration.Credentials.PasswordContainsSmartCardPin)
            rdpClient.PasswordContainsSmartCardPin = configuration.Credentials.PasswordContainsSmartCardPin;

        TraceConfigurationData(logger, configuration.Display);
        if (configuration.Display is {DesktopWidth: > 0, DesktopHeight: > 0})
        {
            rdpClient.DesktopWidth = configuration.Display.DesktopWidth;
            rdpClient.DesktopHeight = configuration.Display.DesktopHeight;
        }
        rdpClient.ColorDepth = configuration.Display.ColorDepth switch
        {
            ColorDepth.ColorDepth8Bpp => 8,
            ColorDepth.ColorDepth15Bpp => 15,
            ColorDepth.ColorDepth16Bpp => 16,
            ColorDepth.ColorDepth24Bpp => 24,
            ColorDepth.ColorDepth32Bpp => 32,
            _ => 32
        };
        rdpClient.SmartSizing = configuration.Display is {UseLocalScaling: false, ResizeBehavior: ResizeBehavior.SmartSizing};
        if (!string.IsNullOrWhiteSpace(configuration.Display.FullScreenTitle))
            rdpClient.FullScreenTitle = configuration.Display.FullScreenTitle;
        rdpClient.ContainerHandledFullScreen = configuration.Display.ContainerHandledFullScreen ? 1 : 0;
        rdpClient.DisplayConnectionBar = configuration.Display.DisplayConnectionBar;
        rdpClient.PinConnectionBar = configuration.Display.PinConnectionBar;
        rdpClient.UseMultimon = configuration.Display.UseMultimon;

        TraceConfigurationData(logger, configuration.Security);
        rdpClient.AuthenticationLevel = configuration.Security.AuthenticationLevel;
        rdpClient.ConnectToAdministerServer = configuration.Security.ConnectToAdministerServer;
        rdpClient.PublicMode = configuration.Security.PublicMode;
        if (configuration.Security.RemoteCredentialGuard)
            rdpClient.RemoteCredentialGuard = configuration.Security.RemoteCredentialGuard;
        if (configuration.Security.RestrictedAdminMode)
            rdpClient.RestrictedAdminMode = configuration.Security.RestrictedAdminMode;

        TraceConfigurationData(logger, configuration.Connection);
        rdpClient.Compression = configuration.Connection.Compression;
        rdpClient.EnableAutoReconnect = configuration.Connection.EnableAutoReconnect;
        if (!string.IsNullOrWhiteSpace(configuration.Connection.LoadBalanceInfo))
            rdpClient.LoadBalanceInfo = configuration.Connection.LoadBalanceInfo;
        rdpClient.MaxReconnectAttempts = configuration.Connection.MaxReconnectAttempts;
        rdpClient.UseRedirectionServerName = configuration.Connection.UseRedirectionServerName;

        TraceConfigurationData(logger, configuration.Performance);
        rdpClient.BitmapCaching = configuration.Performance.BitmapCaching;
        rdpClient.NetworkConnectionType = configuration.Performance.NetworkConnectionType switch
        {
            NetworkConnectionType.Modem => 1,
            NetworkConnectionType.BroadbandLow => 2,
            NetworkConnectionType.Satellite => 3,
            NetworkConnectionType.BroadbandHigh => 4,
            NetworkConnectionType.WAN => 5,
            NetworkConnectionType.LAN => 6,
            _ => 4
        };
        rdpClient.PerformanceFlags = configuration.Performance.GetPerformanceFlags();
        rdpClient.RedirectDirectX = configuration.Performance.RedirectDirectX;
        rdpClient.BandwidthDetection = configuration.Performance.BandwidthDetection;
        rdpClient.EnableHardwareMode = configuration.Performance.EnableHardwareMode;
        rdpClient.ClientProtocolSpec = configuration.Performance.ClientProtocolSpec switch
        {
            ClientProtocolSpec.FullMode => ClientSpec.FullMode,
            ClientProtocolSpec.SmallCacheMode => ClientSpec.SmallCacheMode,
            ClientProtocolSpec.ThinClientMode => ClientSpec.ThinClientMode,
            _ => ClientSpec.FullMode
        };

        TraceConfigurationData(logger, configuration.Input);
        rdpClient.AllowBackgroundInput = configuration.Input.AllowBackgroundInput;
        rdpClient.GrabFocusOnConnect = configuration.Input.GrabFocusOnConnect;
        rdpClient.RelativeMouseMode = configuration.Input.RelativeMouseMode;
        rdpClient.AcceleratorPassthrough = configuration.Input.AcceleratorPassthrough;
        rdpClient.EnableWindowsKey = configuration.Input.EnableWindowsKey;
        rdpClient.KeyboardHookMode = configuration.Input.KeyboardHookMode ? 1 : 0;
        if (!string.IsNullOrWhiteSpace(configuration.Input.KeyBoardLayoutStr))
            rdpClient.KeyBoardLayoutStr = configuration.Input.KeyBoardLayoutStr;

        TraceConfigurationData(logger, configuration.Redirection);
        rdpClient.AudioRedirectionMode = configuration.Redirection.AudioRedirectionMode;
        rdpClient.AudioCaptureRedirectionMode = configuration.Redirection.AudioCaptureRedirectionMode;
        rdpClient.RedirectPrinters = configuration.Redirection.RedirectPrinters;
        rdpClient.RedirectClipboard = configuration.Redirection.RedirectClipboard;
        rdpClient.RedirectSmartCards = configuration.Redirection.RedirectSmartCards;
        rdpClient.RedirectPorts = configuration.Redirection.RedirectPorts;
        rdpClient.RedirectDevices = configuration.Redirection.RedirectDevices;
        rdpClient.RedirectPOSDevices = configuration.Redirection.RedirectPointOfServiceDevices;
        rdpClient.RedirectDrives = configuration.Redirection.RedirectDrives;
        if (!string.IsNullOrWhiteSpace(configuration.Redirection.RedirectDriveLetters))
            rdpClient.RedirectDriveLetters = configuration.Redirection.RedirectDriveLetters;

        TraceConfigurationData(logger, configuration.Program);
        if (!string.IsNullOrWhiteSpace(configuration.Program.StartProgram))
        {
            rdpClient.StartProgram = configuration.Program.StartProgram;
            // ATTENTION:
            // only touch the 'MaximizeShell' property when a StartProgram is defined
            // xrdp server do not like this if you set this to true or false and the connection
            // will be closed immediately
            rdpClient.MaximizeShell = configuration.Program.MaximizeShell;
        }
        if (!string.IsNullOrWhiteSpace(configuration.Program.WorkDir))
            rdpClient.WorkDir = configuration.Program.WorkDir;

        if (configuration.Gateway.GatewayUsageMethod != GatewayUsageMethod.Never)
        {
            TraceConfigurationData(logger, configuration.Gateway);
            rdpClient.GatewayUsageMethod = configuration.Gateway.GatewayUsageMethod;
            rdpClient.GatewayProfileUsageMethod = configuration.Gateway.GatewayProfileUsageMethod;
            rdpClient.GatewayCredsSource = configuration.Gateway.GatewayCredsSource;
            rdpClient.GatewayUserSelectedCredsSource = configuration.Gateway.GatewayUserSelectedCredsSource;
            rdpClient.GatewayCredSharing = configuration.Gateway.GatewayCredSharing;
            if (!string.IsNullOrWhiteSpace(configuration.Gateway.GatewayHostname))
                rdpClient.GatewayHostname = configuration.Gateway.GatewayHostname;
            if (!string.IsNullOrWhiteSpace(configuration.Gateway.GatewayUsername))
                rdpClient.GatewayUsername = configuration.Gateway.GatewayUsername;
            if (!string.IsNullOrWhiteSpace(configuration.Gateway.GatewayDomain))
                rdpClient.GatewayDomain = configuration.Gateway.GatewayDomain;
            var gatewayPassword = configuration.Gateway.GatewayPassword?.GetValue();
            if (!string.IsNullOrWhiteSpace(gatewayPassword))
                rdpClient.GatewayPassword = gatewayPassword;
        }
        else
        {
            rdpClient.GatewayUsageMethod = GatewayUsageMethod.Never;
            rdpClient.GatewayProfileUsageMethod = GatewayProfileUsageMethod.Default;
        }

        if (!string.IsNullOrWhiteSpace(configuration.HyperV.Instance))
        {
            TraceConfigurationData(logger, configuration.HyperV);
            rdpClient.Port = configuration.HyperV.HyperVPort;
            rdpClient.AuthenticationLevel = AuthenticationLevel.NoAuthenticationOfServer;
            rdpClient.AuthenticationServiceClass = "Microsoft Virtual Console Service";
            rdpClient.NetworkLevelAuthentication = true;
            rdpClient.NegotiateSecurityLayer = false;
            rdpClient.DisableCredentialsDelegation = true;
            rdpClient.PCB = configuration.HyperV.Instance;
            if (configuration.HyperV.EnhancedSessionMode)
                rdpClient.PCB = $"{rdpClient.PCB};EnhancedMode=1";
        }
    }

    private static void TraceConfigurationData(ILogger logger, object configuration)
    {
        if (!logger.IsEnabled(LogLevel.Trace))
            return;
        
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Configuration class: {configuration.GetType().Name}");
        var properties = configuration.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var property in properties)
        {
            stringBuilder.AppendLine($"      {property.Name}: {property.GetValue(configuration)}");
        }

        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
        logger.LogTrace(stringBuilder.ToString());
    }
    
    /// <summary>
    /// Provides access to the non-scriptable properties (version 4) of a client's remote session on the Remote Desktop ActiveX control.
    /// </summary>
    /// <param name="rdpClient">The RDP client instance.</param>
    /// <returns>IMsRdpClientNonScriptable4</returns>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientnonscriptable5</cref>
    /// </seealso>
    public static IMsRdpClientNonScriptable4 GetNonScriptable4(this IRdpClient rdpClient)
    {
        return (IMsRdpClientNonScriptable4)rdpClient.GetOcx();
    }

    /// <summary>
    /// Provides access to the non-scriptable properties (version 5) of a client's remote session on the Remote Desktop ActiveX control.
    /// </summary>
    /// <param name="rdpClient">The RDP client instance.</param>
    /// <returns>IMsRdpClientNonScriptable5</returns>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientnonscriptable5</cref>
    /// </seealso>
    public static IMsRdpClientNonScriptable5 GetNonScriptable5(this IRdpClient rdpClient)
    {
        return (IMsRdpClientNonScriptable5)rdpClient.GetOcx();
    }

    /// <summary>
    /// Provides access to the extended settings interface of a client's remote session on the Remote Desktop ActiveX control.
    /// </summary>
    /// <param name="rdpClient">The RDP client instance.</param>
    /// <returns>IMsRdpExtendedSettings</returns>
    /// <seealso>
    ///     <cref>https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpextendedsettings</cref>
    /// </seealso>
    public static IMsRdpExtendedSettings GetExtendedSettings(this IRdpClient rdpClient)
    {
        return (IMsRdpExtendedSettings)rdpClient.GetOcx();
    }

    /// <summary>
    /// Provides access to the IMsRdpExtendedSettings interface of the Remote Desktop ActiveX control.
    /// </summary>
    /// <param name="rdpClient">The RDP client instance.</param>
    /// <param name="propertyName">The name of the property to get.</param>
    /// <param name="value">The value.</param>
    /// <param name="exception">The exception in case the call fails.</param>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpextendedsettings-property</cref>
    /// </seealso>
    public static bool TryGetProperty<T>(this IRdpClient rdpClient, string propertyName, out T? value, out Exception? exception)
    {
        exception = null;
        value = default;
        try
        {
            value = (T)rdpClient.GetExtendedSettings().get_Property(propertyName);
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
        return true;
    }

    /// <summary>
    /// Provides access to the IMsRdpExtendedSettings interface of the Remote Desktop ActiveX control.
    /// </summary>
    /// <param name="rdpClient">The RDP client instance.</param>
    /// <param name="propertyName">The name of the property to set.</param>
    /// <param name="value">The value</param>
    /// <param name="exception">The exception in case the call fails.</param>
    /// <returns>IMsRdpExtendedSettings</returns>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpextendedsettings-property</cref>
    /// </seealso>
    public static bool TrySetProperty(this IRdpClient rdpClient, string propertyName, ref object value, out Exception? exception)
    {
        exception = null;
        try
        {
            rdpClient.GetExtendedSettings().set_Property(propertyName, ref value);
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
        return true;
    }

    /// <summary>
    /// Provides a property to control using a redirection server.
    /// </summary>
    /// <param name="rdpClient"></param>
    /// <returns>IMsRdpPreferredRedirectionInfo</returns>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdppreferredredirectioninfo</cref>
    /// </seealso>
    public static IMsRdpPreferredRedirectionInfo GetPreferredRedirectionInfo(this IRdpClient rdpClient)
    {
        return (IMsRdpPreferredRedirectionInfo) rdpClient.GetOcx();
    }

    /// <summary>
    /// Ensures the load balance info string is in the correct format
    /// </summary>
    /// <param name="loadBalanceInfo">The string provided by the user</param>
    /// <param name="advancedSettings">Instance of the AdvancedSettings class of the rdp client</param>
    /// <returns>The load balance info in the correct format.</returns>
    /// <seealso>
    ///     <cref>https://docs.microsoft.com/en-us/windows/win32/termserv/imsrdpclientadvancedsettings-loadbalanceinfo</cref>
    /// </seealso>
    public static void SetLoadBalanceInfo(string loadBalanceInfo, IMsRdpClientAdvancedSettings advancedSettings)
    {
        loadBalanceInfo += "\r\n";
        var bytes = Encoding.UTF8.GetBytes(loadBalanceInfo);
        var byteLen = (uint)bytes.Length;

        var lbiStringPtr = PInvoke.SysAllocStringByteLen(new PCSTR(), byteLen);
        Marshal.Copy(bytes, 0, lbiStringPtr, (int)byteLen);

        // ReSharper disable once SuspiciousTypeConversion.Global
        var lbSettings = (IMsRdpClientAdvancedSettingsLB) advancedSettings;
        lbSettings.LoadBalanceInfo = lbiStringPtr;

        Marshal.ZeroFreeBSTR(lbiStringPtr);
    }

    /// <summary>
    /// Sets up drive redirection for the remote desktop client using the non scriptable client version 4.
    /// </summary>
    /// <param name="rdpClient">The remote desktop client.</param>
    /// <param name="driveLetters">A string containing all drive letters to setup redirection for..</param>
    /// <param name="exception">An exception as output parameter in case one or more drives cannot be redirected.</param>
    /// <returns>True if all drives could be redirected. False, if one or more drives failed to redirect. Check exception output parameter for more details.</returns>
    public static bool SetupDriveRedirection(this IRdpClient rdpClient, string driveLetters, out Exception? exception)
    {
        if (string.IsNullOrWhiteSpace(driveLetters))
        {
            exception = null;
            return true;
        }

        var logMessage = new StringBuilder();
        var nonScriptable = rdpClient.GetNonScriptable4();

        logMessage.AppendLine($"Setup drive redirection for '{driveLetters}'.");

        try
        {
            nonScriptable.DriveCollection.RescanDrives(true);
            logMessage.AppendLine("Successfully scanned local drives.");
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }

        var success = true;
        for (uint i = 0; i < nonScriptable.DriveCollection.DriveCount; i++)
        {
            try
            {
                logMessage.AppendLine($"Processing Drive: {i}");
                var volumeName = nonScriptable.DriveCollection.DriveByIndex[i].Name;
                var driveLetter = string.Empty;

                // http://stackoverflow.com/questions/2819934/detect-windows-7-in-net
                if ((Environment.OSVersion.Version.Major > 5) &&
                    (Environment.OSVersion.Version.Minor > 0) &&
                    (volumeName.Substring(volumeName.Length - 3, 1) == ":"))
                {
                    logMessage.AppendLine("OS Version: Windows 7 / Windows 2008R2 or later.");
                    // win7/2008r2 returns a different name property like: "BOOTCAMP (C:)"
                    driveLetter = volumeName.Substring(volumeName.Length - 4, 1);
                }
                else
                {
                    logMessage.AppendLine("OS Version: Pre-Windows 7 / Pre-Windows 2008R2.");
                    // pre win7/2008r2: "C: BOOTCAMP"
                    // look for the first colon (:)
                    var colonPosition = volumeName.IndexOf(":", StringComparison.Ordinal);
                    if (colonPosition > 0)
                    {
                        // take the char before the colon as drive letter
                        driveLetter = volumeName.Substring(colonPosition - 1, 1);
                    }
                }

                logMessage.AppendFormat("Volume: '{0}'{1}", volumeName, Environment.NewLine);
                logMessage.AppendFormat("DriveLetter: '{0}'{1}", driveLetter, Environment.NewLine);

                if (driveLetter.Length == 1)
                {
                    var redirectionState = driveLetters.IndexOf(driveLetter, StringComparison.OrdinalIgnoreCase) >= 0;
                    nonScriptable.DriveCollection.DriveByIndex[i].RedirectionState = redirectionState;
                    logMessage.AppendFormat("Redirection{0}set.{1}", redirectionState ? " " : " not ", Environment.NewLine);
                }
                logMessage.AppendLine();
            }
            catch (Exception ex)
            {
                success = false;
                logMessage.AppendLine(ex.Message);
                logMessage.AppendLine();
            }
        }

        exception = success ? null : new Exception(logMessage.ToString());
        return success;
    }
}