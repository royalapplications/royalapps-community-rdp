using System;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
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
    /// <param name="client">The RdpClient instance.</param>
    /// <param name="configuration">The RdpClientConfiguration instance.</param>
    public static void ApplyRdpClientConfiguration(this IRdpClient client, RdpClientConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrEmpty(configuration.Server, nameof(configuration.Server));
        
        client.Server = configuration.Server;
        client.Port = configuration.Port;
        
        client.UserName = configuration.Credentials.Username;
        client.Domain = configuration.Credentials.Domain;
        client.Password = configuration.Credentials.Password;
        client.NetworkLevelAuthentication = configuration.Credentials.NetworkLevelAuthentication;
        client.PasswordContainsSmartCardPin = configuration.Credentials.PasswordContainsSmartCardPin;

        if (configuration.Display is {DesktopWidth: > 0, DesktopHeight: > 0})
        {
            client.DesktopWidth = configuration.Display.DesktopWidth;
            client.DesktopHeight = configuration.Display.DesktopHeight;
        }
        client.ColorDepth = configuration.Display.ColorDepth switch
        {
            ColorDepth.ColorDepth8Bpp => 8,
            ColorDepth.ColorDepth15Bpp => 15,
            ColorDepth.ColorDepth16Bpp => 16,
            ColorDepth.ColorDepth24Bpp => 24,
            ColorDepth.ColorDepth32Bpp => 32,
            _ => 32
        };
        client.SmartSizing = configuration.Display is {UseLocalScaling: false, ResizeBehavior: ResizeBehavior.SmartSizing};
        if (!string.IsNullOrWhiteSpace(configuration.Display.FullScreenTitle))
            client.FullScreenTitle = configuration.Display.FullScreenTitle;
        client.ContainerHandledFullScreen = configuration.Display.ContainerHandledFullScreen ? 1 : 0;
        client.DisplayConnectionBar = configuration.Display.DisplayConnectionBar;
        client.PinConnectionBar = configuration.Display.PinConnectionBar;
        client.UseMultimon = configuration.Display.UseMultimon;

        client.AuthenticationLevel = configuration.Security.AuthenticationLevel;
        client.Compression = configuration.Connection.Compression;
        client.BitmapCaching = configuration.Performance.BitmapCaching;
        client.PublicMode = configuration.Security.PublicMode;
        client.AllowBackgroundInput = configuration.Input.AllowBackgroundInput;
        client.EnableAutoReconnect = configuration.Connection.EnableAutoReconnect;
        client.MaxReconnectAttempts = configuration.Connection.MaxReconnectAttempts;
        client.ConnectToAdministerServer = configuration.Security.ConnectToAdministerServer;
        client.UseRedirectionServerName = configuration.Connection.UseRedirectionServerName;
        if (!string.IsNullOrWhiteSpace(configuration.Connection.LoadBalanceInfo))
            client.LoadBalanceInfo = configuration.Connection.LoadBalanceInfo;
        if (!string.IsNullOrWhiteSpace(configuration.PluginDlls))
            client.PluginDlls = configuration.PluginDlls;
        client.GrabFocusOnConnect = configuration.Input.GrabFocusOnConnect;
        client.RelativeMouseMode = configuration.Input.RelativeMouseMode;
        client.RemoteCredentialGuard = configuration.Security.RemoteCredentialGuard;
        client.RestrictedAdminMode = configuration.Security.RestrictedAdminMode;
        client.NetworkConnectionType = configuration.Performance.NetworkConnectionType switch
        {
            NetworkConnectionType.Modem => 1,
            NetworkConnectionType.BroadbandLow => 2,
            NetworkConnectionType.Satellite => 3,
            NetworkConnectionType.BroadbandHigh => 4,
            NetworkConnectionType.WAN => 5,
            NetworkConnectionType.LAN => 6,
            _ => 4
        };
        client.PerformanceFlags = configuration.Performance.GetPerformanceFlags();
        //client.RedirectDirectX = configuration.Performance.RedirectDirectX;
        //client.BandwidthDetection = configuration.Performance.BandwidthDetection;
        client.EnableHardwareMode = configuration.Performance.EnableHardwareMode;
        client.ClientProtocolSpec = configuration.Performance.ClientProtocolSpec switch
        {
            ClientProtocolSpec.FullMode => ClientSpec.FullMode,
            ClientProtocolSpec.SmallCacheMode => ClientSpec.SmallCacheMode,
            ClientProtocolSpec.ThinClientMode => ClientSpec.ThinClientMode,
            _ => ClientSpec.FullMode
        };

        client.AudioRedirectionMode = configuration.Redirection.AudioRedirectionMode;
        client.AudioCaptureRedirectionMode = configuration.Redirection.AudioCaptureRedirectionMode;
        client.RedirectPrinters = configuration.Redirection.RedirectPrinters;
        client.RedirectClipboard = configuration.Redirection.RedirectClipboard;
        client.RedirectSmartCards = configuration.Redirection.RedirectSmartCards;
        client.RedirectPorts = configuration.Redirection.RedirectPorts;
        client.RedirectDevices = configuration.Redirection.RedirectDevices;
        client.RedirectPOSDevices = configuration.Redirection.RedirectPointOfServiceDevices;
        client.RedirectDrives = configuration.Redirection.RedirectDrives;
        if (!string.IsNullOrWhiteSpace(configuration.Redirection.RedirectDriveLetters))
            client.RedirectDriveLetters = configuration.Redirection.RedirectDriveLetters;

        client.AcceleratorPassthrough = configuration.Input.AcceleratorPassthrough;
        client.EnableWindowsKey = configuration.Input.EnableWindowsKey;
        client.KeyboardHookMode = configuration.Input.KeyboardHookMode ? 1 : 0;
        if (!string.IsNullOrWhiteSpace(configuration.Input.KeyBoardLayoutStr))
            client.KeyBoardLayoutStr = configuration.Input.KeyBoardLayoutStr;

        if (!string.IsNullOrWhiteSpace(configuration.Program.StartProgram))
            client.StartProgram = configuration.Program.StartProgram;
        if (!string.IsNullOrWhiteSpace(configuration.Program.WorkDir))
            client.WorkDir = configuration.Program.WorkDir;
        client.MaximizeShell = configuration.Program.MaximizeShell;

        if (configuration.Gateway.GatewayUsageMethod != GatewayUsageMethod.Never)
        {
            client.GatewayUsageMethod = configuration.Gateway.GatewayUsageMethod;
            client.GatewayProfileUsageMethod = configuration.Gateway.GatewayProfileUsageMethod;
            client.GatewayCredsSource = configuration.Gateway.GatewayCredsSource;
            client.GatewayUserSelectedCredsSource = configuration.Gateway.GatewayUserSelectedCredsSource;
            client.GatewayCredSharing = configuration.Gateway.GatewayCredSharing;
            if (!string.IsNullOrWhiteSpace(configuration.Gateway.GatewayHostname))
                client.GatewayHostname = configuration.Gateway.GatewayHostname;
            if (!string.IsNullOrWhiteSpace(configuration.Gateway.GatewayUsername))
                client.GatewayUsername = configuration.Gateway.GatewayUsername;
            if (!string.IsNullOrWhiteSpace(configuration.Gateway.GatewayDomain))
                client.GatewayDomain = configuration.Gateway.GatewayDomain;
            if (!string.IsNullOrWhiteSpace(configuration.Gateway.GatewayPassword))
                client.GatewayPassword = configuration.Gateway.GatewayPassword;
        }

        if (!string.IsNullOrWhiteSpace(configuration.HyperV.Instance))
        {
            client.Port = configuration.HyperV.HyperVPort;
            client.AuthenticationLevel = AuthenticationLevel.NoAuthenticationOfServer;
            client.AuthenticationServiceClass = "Microsoft Virtual Console Service";
            client.NetworkLevelAuthentication = true;
            client.NegotiateSecurityLayer = false;
            client.DisableCredentialsDelegation = true;
            client.PCB = configuration.HyperV.Instance;
            if (configuration.HyperV.EnhancedSessionMode)
                client.PCB = $"{client.PCB};EnhancedMode=1";
        }
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