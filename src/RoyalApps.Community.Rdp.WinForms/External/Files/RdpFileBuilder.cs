using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Connection;
using RoyalApps.Community.Rdp.WinForms.Configuration.Display;
using RoyalApps.Community.Rdp.WinForms.Configuration.Internal;
using RoyalApps.Community.Rdp.WinForms.Configuration.Performance;
using RoyalApps.Community.Rdp.WinForms.External.Credentials;

namespace RoyalApps.Community.Rdp.WinForms.External.Files;

internal static class RdpFileBuilder
{
    public static string Build(RdpConnectionContext connectionContext)
    {
        ArgumentNullException.ThrowIfNull(connectionContext);
        var configuration = connectionContext.Configuration;

        var settings = new List<RdpFileSetting>();
        var userName = CredentialTargetResolver.FormatUserName(configuration.Credentials);
        var hasPassword = !string.IsNullOrEmpty(configuration.Credentials.Password?.GetValue());
        var fullAddress = BuildFullAddress(configuration);
        var useMultimon = configuration.Display.UseMultimon || !string.IsNullOrWhiteSpace(configuration.External.SelectedMonitors);

        SetString(settings, "full address", fullAddress);
        SetInteger(settings, "server port", configuration.Port);

        if (!string.IsNullOrWhiteSpace(userName))
            SetString(settings, "username", userName);

        SetInteger(settings, "prompt for credentials", hasPassword ? 0 : 1);
        SetInteger(settings, "promptcredentialonce", hasPassword ? 1 : 0);
        SetInteger(settings, "screen mode id", configuration.Display.FullScreen ? 2 : 1);
        SetInteger(settings, "use multimon", useMultimon.ToInt());
        SetInteger(settings, "desktopwidth", GetDesktopWidth(configuration));
        SetInteger(settings, "desktopheight", GetDesktopHeight(configuration));
        SetInteger(settings, "session bpp", MapColorDepth(configuration.Display.ColorDepth));
        SetInteger(settings, "smart sizing", (configuration.Display.ResizeBehavior == ResizeBehavior.SmartSizing).ToInt());
        SetInteger(settings, "displayconnectionbar", configuration.Display.DisplayConnectionBar.ToInt());
        SetInteger(settings, "authentication level", (int)configuration.Security.AuthenticationLevel);
        SetInteger(settings, "administrative session", configuration.Security.ConnectToAdministerServer.ToInt());
        SetInteger(settings, "public mode", configuration.Security.PublicMode.ToInt());
        SetInteger(settings, "enablecredsspsupport", configuration.Credentials.NetworkLevelAuthentication.ToInt());
        SetInteger(settings, "compression", configuration.Connection.Compression.ToInt());
        SetInteger(settings, "autoreconnection enabled", configuration.Connection.EnableAutoReconnect.ToInt());
        SetInteger(settings, "autoreconnect max retries", configuration.Connection.MaxReconnectAttempts);
        SetInteger(settings, "enablerdsaadauth", configuration.Connection.EnableRdsAadAuth.ToInt());
        SetInteger(settings, "use redirection server name", configuration.Connection.UseRedirectionServerName.ToInt());
        SetInteger(settings, "networkautodetect", (configuration.Performance.NetworkConnectionType == NetworkConnectionType.Automatic).ToInt());
        SetInteger(settings, "bandwidthautodetect", (configuration.Performance.BandwidthDetection || configuration.Performance.NetworkConnectionType == NetworkConnectionType.Automatic).ToInt());
        SetInteger(settings, "connection type", MapConnectionType(configuration.Performance.NetworkConnectionType));
        SetInteger(settings, "bitmapcachepersistenable", configuration.Performance.BitmapCaching.ToInt());
        SetInteger(settings, "audiomode", (int)configuration.Redirection.AudioRedirectionMode);
        SetInteger(settings, "audioqualitymode", (int)configuration.Redirection.AudioQualityMode);
        SetInteger(settings, "audiocapturemode", configuration.Redirection.AudioCaptureRedirectionMode.ToInt());
        SetInteger(settings, "videoplaybackmode", configuration.Redirection.RedirectVideoRendering.ToInt());
        SetInteger(settings, "redirectclipboard", configuration.Redirection.RedirectClipboard.ToInt());
        SetInteger(settings, "redirectprinters", configuration.Redirection.RedirectPrinters.ToInt());
        SetInteger(settings, "redirectsmartcards", configuration.Redirection.RedirectSmartCards.ToInt());
        SetInteger(settings, "redirectcomports", configuration.Redirection.RedirectPorts.ToInt());
        SetInteger(settings, "redirectposdevices", configuration.Redirection.RedirectPointOfServiceDevices.ToInt());
        SetInteger(settings, "redirectdrives", configuration.Redirection.RedirectDrives.ToInt());
        SetInteger(settings, "redirectwebauthn", configuration.Security.RemoteCredentialGuard.ToInt());
        SetInteger(settings, "allow font smoothing", configuration.Performance.EnableFontSmoothing.ToInt());
        SetInteger(settings, "allow desktop composition", configuration.Performance.EnableDesktopComposition.ToInt());
        SetInteger(settings, "disable wallpaper", configuration.Performance.DisableWallpaper.ToInt());
        SetInteger(settings, "disable full window drag", configuration.Performance.DisableFullWindowDrag.ToInt());
        SetInteger(settings, "disable menu anims", configuration.Performance.DisableMenuAnimations.ToInt());
        SetInteger(settings, "disable themes", configuration.Performance.DisableTheming.ToInt());
        SetInteger(settings, "disable cursor setting", configuration.Performance.DisableCursorSettings.ToInt());
        SetInteger(settings, "disable cursor shadow", configuration.Performance.DisableCursorShadow.ToInt());
        SetInteger(settings, "keyboardhook", configuration.Input.KeyboardHookMode ? 1 : 0);

        if (!string.IsNullOrWhiteSpace(configuration.Input.KeyBoardLayoutStr))
            SetString(settings, "keyboardlayout", configuration.Input.KeyBoardLayoutStr);

        if (!string.IsNullOrWhiteSpace(configuration.External.SelectedMonitors))
            SetString(settings, "selectedmonitors", NormalizeSelectedMonitors(configuration.External.SelectedMonitors));

        if (connectionContext.EffectiveRemoteApp.Enabled)
        {
            AppendRemoteAppSettings(settings, connectionContext.EffectiveRemoteApp);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(configuration.Program.StartProgram))
                SetString(settings, "alternate shell", configuration.Program.StartProgram);

            if (!string.IsNullOrWhiteSpace(configuration.Program.WorkDir))
                SetString(settings, "shell working directory", configuration.Program.WorkDir);
        }

        if (!string.IsNullOrWhiteSpace(configuration.Connection.LoadBalanceInfo))
            SetString(settings, "loadbalanceinfo", configuration.Connection.LoadBalanceInfo);

        if (configuration.Redirection.RedirectDrives)
            SetString(settings, "drivestoredirect", BuildDriveRedirectionValue(configuration.Redirection.RedirectDriveLetters));

        if (configuration.Redirection.RedirectCameras)
            SetString(settings, "camerastoredirect", "*");

        if (ShouldWriteGatewaySettings(configuration))
        {
            SetInteger(settings, "gatewayusagemethod", (int)configuration.Gateway.GatewayUsageMethod);
            SetInteger(settings, "gatewayprofileusagemethod", (int)configuration.Gateway.GatewayProfileUsageMethod);
            SetInteger(settings, "gatewaycredentialssource", (int)configuration.Gateway.GatewayCredsSource);
            SetInteger(settings, "promptcredentialonce", configuration.Gateway.GatewayCredSharing.ToInt());
            SetInteger(settings, "gatewayuserselectedcredentialssource", (int)configuration.Gateway.GatewayUserSelectedCredsSource);

            if (!string.IsNullOrWhiteSpace(configuration.Gateway.GatewayHostname))
                SetString(settings, "gatewayhostname", configuration.Gateway.GatewayHostname);

            if (!string.IsNullOrWhiteSpace(configuration.Gateway.GatewayUsername))
                SetString(settings, "gatewayusername", configuration.Gateway.GatewayUsername);

            if (!string.IsNullOrWhiteSpace(configuration.Gateway.GatewayDomain))
                SetString(settings, "gatewaydomain", configuration.Gateway.GatewayDomain);
        }

        if (!string.IsNullOrWhiteSpace(configuration.HyperV.Instance))
        {
            SetInteger(settings, "server port", configuration.HyperV.HyperVPort);
            SetString(
                settings,
                "pcb",
                configuration.HyperV.EnhancedSessionMode
                    ? $"{configuration.HyperV.Instance};EnhancedMode=1"
                    : configuration.HyperV.Instance);
        }

        foreach (var additionalSetting in configuration.External.AdditionalRdpSettings)
            Upsert(settings, additionalSetting);

        if (configuration.External.UseMsRdpExHooks)
        {
            AppendTypedMsRdpExSettings(settings, configuration, connectionContext.EffectiveSecurity);
            foreach (var additionalSetting in configuration.External.AdditionalMsRdpExSettings)
                Upsert(settings, additionalSetting);
        }

        var builder = new StringBuilder();
        foreach (var setting in settings)
            builder.Append(setting.ToRdpLine()).Append("\r\n");

        return builder.ToString();
    }

    private static int GetDesktopHeight(RdpClientConfiguration configuration)
    {
        return configuration.Display.DesktopHeight > 0
            ? configuration.Display.DesktopHeight
            : 1080;
    }

    private static int GetDesktopWidth(RdpClientConfiguration configuration)
    {
        return configuration.Display.DesktopWidth > 0
            ? configuration.Display.DesktopWidth
            : 1920;
    }

    private static string BuildDriveRedirectionValue(string? driveLetters)
    {
        if (string.IsNullOrWhiteSpace(driveLetters))
            return "*";

        var drives = driveLetters
            .Trim()
            .Where(char.IsLetter)
            .Select(char.ToUpperInvariant)
            .Distinct()
            .Select(letter => $"{letter}:");

        return string.Join(",", drives);
    }

    private static string NormalizeSelectedMonitors(string selectedMonitors)
    {
        var monitorIds = selectedMonitors
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Distinct(StringComparer.Ordinal);

        return string.Join(",", monitorIds);
    }

    private static bool ShouldWriteGatewaySettings(RdpClientConfiguration configuration)
    {
        return !string.IsNullOrWhiteSpace(configuration.Gateway.GatewayHostname) ||
               !string.IsNullOrWhiteSpace(configuration.Gateway.GatewayUsername) ||
               !string.IsNullOrWhiteSpace(configuration.Gateway.GatewayDomain) ||
               configuration.Gateway.GatewayUsageMethod != GatewayUsageMethod.Never ||
               configuration.Gateway.GatewayProfileUsageMethod != GatewayProfileUsageMethod.Default ||
               configuration.Gateway.GatewayCredsSource != GatewayCredentialSource.UsernameAndPassword ||
               configuration.Gateway.GatewayUserSelectedCredsSource != GatewayCredentialSource.UsernameAndPassword ||
               configuration.Gateway.GatewayCredSharing;
    }

    private static string BuildFullAddress(RdpClientConfiguration configuration)
    {
        var server = configuration.Server?.Trim();
        ArgumentException.ThrowIfNullOrWhiteSpace(server, nameof(configuration.Server));
        return configuration.Port is > 0 and not 3389
            ? $"{server}:{configuration.Port}"
            : server;
    }

    private static int MapColorDepth(ColorDepth colorDepth)
    {
        return colorDepth switch
        {
            ColorDepth.ColorDepth8Bpp => 8,
            ColorDepth.ColorDepth15Bpp => 15,
            ColorDepth.ColorDepth16Bpp => 16,
            ColorDepth.ColorDepth24Bpp => 24,
            _ => 32
        };
    }

    private static int MapConnectionType(NetworkConnectionType connectionType)
    {
        return connectionType switch
        {
            NetworkConnectionType.Modem => 1,
            NetworkConnectionType.BroadbandLow => 2,
            NetworkConnectionType.Satellite => 3,
            NetworkConnectionType.BroadbandHigh => 4,
            NetworkConnectionType.WAN => 5,
            _ => 6
        };
    }

    private static void AppendRemoteAppSettings(ICollection<RdpFileSetting> settings, EffectiveRemoteAppSettings remoteApp)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(remoteApp.Program);

        SetInteger(settings, "remoteapplicationmode", 1);
        SetString(settings, "remoteapplicationprogram", remoteApp.Program);
        SetInteger(settings, "remoteapplicationexpandcmdline", remoteApp.ExpandCommandLine.ToInt());
        SetInteger(settings, "remoteapplicationexpandworkingdir", remoteApp.ExpandWorkingDirectory.ToInt());

        if (!string.IsNullOrWhiteSpace(remoteApp.Name))
            SetString(settings, "remoteapplicationname", remoteApp.Name);

        if (!string.IsNullOrWhiteSpace(remoteApp.CommandLine))
            SetString(settings, "remoteapplicationcmdline", remoteApp.CommandLine);

        if (!string.IsNullOrWhiteSpace(remoteApp.WorkingDirectory))
            SetString(settings, "shell working directory", remoteApp.WorkingDirectory);

        if (!string.IsNullOrWhiteSpace(remoteApp.File))
            SetString(settings, "remoteapplicationfile", remoteApp.File);

        if (!string.IsNullOrWhiteSpace(remoteApp.Icon))
            SetString(settings, "remoteapplicationicon", remoteApp.Icon);

        if (remoteApp.DisableCapabilitiesCheck)
            SetInteger(settings, "disableremoteappcapscheck", 1);
    }

    private static void AppendTypedMsRdpExSettings(
        ICollection<RdpFileSetting> settings,
        RdpClientConfiguration configuration,
        EffectiveSecuritySettings effectiveSecuritySettings)
    {
        SetInteger(settings, "DisableCredentialsDelegation", effectiveSecuritySettings.DisableCredentialsDelegation.ToInt());
        SetInteger(settings, "RedirectedAuthentication", effectiveSecuritySettings.RedirectedAuthentication.ToInt());
        SetInteger(settings, "RestrictedLogon", effectiveSecuritySettings.RestrictedLogon.ToInt());
        SetInteger(settings, "DisableUDPTransport", configuration.Connection.DisableUdpTransport.ToInt());
        SetInteger(settings, "AllowBackgroundInput", configuration.Input.AllowBackgroundInput.ToInt());
        SetInteger(settings, "RelativeMouseMode", configuration.Input.RelativeMouseMode.ToInt());
        SetInteger(settings, "EnableHardwareMode", configuration.Performance.EnableHardwareMode.ToInt());
        SetInteger(settings, "EnableMouseJiggler", configuration.External.MsRdpEx.EnableMouseJiggler.ToInt());

        if (configuration.External.MsRdpEx.EnableMouseJiggler)
        {
            SetInteger(settings, "MouseJigglerMethod", (int)configuration.External.MsRdpEx.MouseJigglerMethod);

            if (configuration.External.MsRdpEx.MouseJigglerInterval > 0)
                SetInteger(settings, "MouseJigglerInterval", configuration.External.MsRdpEx.MouseJigglerInterval);
        }

        if (!string.IsNullOrWhiteSpace(configuration.External.MsRdpEx.KdcProxyUrl))
            SetString(settings, "KDCProxyURL", configuration.External.MsRdpEx.KdcProxyUrl);

        if (!string.IsNullOrWhiteSpace(configuration.External.MsRdpEx.UserSpecifiedServerName))
            SetString(settings, "UserSpecifiedServerName", configuration.External.MsRdpEx.UserSpecifiedServerName);
    }

    private static void SetInteger(ICollection<RdpFileSetting> settings, string name, int value)
    {
        Upsert(settings, new RdpFileSetting(name, RdpFileSettingType.Integer, value.ToString()));
    }

    private static void SetString(ICollection<RdpFileSetting> settings, string name, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        Upsert(settings, new RdpFileSetting(name, RdpFileSettingType.String, value));
    }

    private static void Upsert(ICollection<RdpFileSetting> settings, RdpFileSetting setting)
    {
        var existing = settings.FirstOrDefault(candidate => string.Equals(candidate.Name, setting.Name, StringComparison.OrdinalIgnoreCase));
        if (existing is null)
        {
            settings.Add(setting);
            return;
        }

        existing.Type = setting.Type;
        existing.Value = setting.Value;
    }
}

internal static class BooleanExtensions
{
    public static int ToInt(this bool value)
    {
        return value ? 1 : 0;
    }
}
