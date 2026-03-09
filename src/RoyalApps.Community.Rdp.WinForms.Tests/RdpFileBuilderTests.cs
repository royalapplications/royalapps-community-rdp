using System;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Connection;
using RoyalApps.Community.Rdp.WinForms.Configuration.Input;
using RoyalApps.Community.Rdp.WinForms.Configuration.Internal;
using RoyalApps.Community.Rdp.WinForms.External.Files;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class RdpFileBuilderTests
{
    [Fact]
    public void Build_IncludesCoreExternalSessionSettings()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            Port = 3390,
            SessionMode = RdpSessionMode.External,
            Display =
            {
                DesktopWidth = 1600,
                DesktopHeight = 900,
                FullScreen = true,
                UseMultimon = true
            },
            Credentials =
            {
                Domain = "LAB",
                Username = "alice",
                Password = new SensitiveString("secret")
            },
            Program =
            {
                StartProgram = "pwsh.exe",
                WorkDir = @"C:\Temp"
            },
            Connection =
            {
                EnableRdsAadAuth = true
            },
            Security =
            {
                DisableCredentialsDelegation = true,
                RedirectedAuthentication = true
            }
        };
        configuration.External.SelectedMonitors = "0,2,2";
        configuration.External.UseMsRdpExHooks = true;
        configuration.External.AdditionalRdpSettings.Add(new RdpFileSetting("custom setting", RdpFileSettingType.String, "value"));

        var content = Build(configuration);

        Assert.Contains("full address:s:rdp.example.test:3390", content);
        Assert.Contains("server port:i:3390", content);
        Assert.Contains(@"username:s:LAB\alice", content);
        Assert.Contains("screen mode id:i:2", content);
        Assert.Contains("use multimon:i:1", content);
        Assert.Contains("selectedmonitors:s:0,2", content);
        Assert.Contains("desktopwidth:i:1600", content);
        Assert.Contains("desktopheight:i:900", content);
        Assert.Contains("enablerdsaadauth:i:1", content);
        Assert.Contains("DisableCredentialsDelegation:i:1", content);
        Assert.Contains("RedirectedAuthentication:i:1", content);
        Assert.Contains("RestrictedLogon:i:0", content);
        Assert.Contains("alternate shell:s:pwsh.exe", content);
        Assert.Contains(@"shell working directory:s:C:\Temp", content);
        Assert.Contains("custom setting:s:value", content);
    }

    [Fact]
    public void Build_WritesDriveSelection_WhenSpecificDrivesAreConfigured()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            Redirection =
            {
                RedirectDrives = true,
                RedirectDriveLetters = "ceh"
            }
        };

        var content = Build(configuration);

        Assert.Contains("drivestoredirect:s:C:,E:,H:", content);
    }

    [Fact]
    public void Build_EnablesMultimon_WhenSelectedMonitorsAreConfiguredForExternalMode()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.External
        };
        configuration.External.SelectedMonitors = "1,3";

        var content = Build(configuration);

        Assert.Contains("use multimon:i:1", content);
        Assert.Contains("selectedmonitors:s:1,3", content);
    }

    [Fact]
    public void Build_DoesNotWriteMsRdpExSecurityFlags_WhenHooksAreDisabled()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            Connection =
            {
                DisableUdpTransport = true
            },
            Input =
            {
                AllowBackgroundInput = true,
                RelativeMouseMode = false
            },
            Performance =
            {
                EnableHardwareMode = true
            },
            Security =
            {
                RemoteCredentialGuard = true,
                RestrictedAdminMode = true
            },
            External =
            {
                MsRdpEx =
                {
                    EnableMouseJiggler = true,
                    MouseJigglerInterval = 45,
                    MouseJigglerMethod = KeepAliveMethod.KeyboardInput,
                    KdcProxyUrl = "https://kdc.example.test/kdcproxy",
                    UserSpecifiedServerName = "server-alias.example.test"
                }
            }
        };

        var content = Build(configuration);

        Assert.DoesNotContain("DisableCredentialsDelegation:i:", content);
        Assert.DoesNotContain("RedirectedAuthentication:i:", content);
        Assert.DoesNotContain("RestrictedLogon:i:", content);
        Assert.DoesNotContain("DisableUDPTransport:i:", content);
        Assert.DoesNotContain("AllowBackgroundInput:i:", content);
        Assert.DoesNotContain("RelativeMouseMode:i:", content);
        Assert.DoesNotContain("EnableHardwareMode:i:", content);
        Assert.DoesNotContain("EnableMouseJiggler:i:", content);
        Assert.DoesNotContain("MouseJigglerInterval:i:", content);
        Assert.DoesNotContain("MouseJigglerMethod:i:", content);
        Assert.DoesNotContain("KDCProxyURL:s:", content);
        Assert.DoesNotContain("UserSpecifiedServerName:s:", content);
    }

    [Fact]
    public void Build_WritesTypedMsRdpExSettings_WhenHooksAreEnabled()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.External,
            Connection =
            {
                DisableUdpTransport = true
            },
            Input =
            {
                AllowBackgroundInput = true,
                RelativeMouseMode = false
            },
            Performance =
            {
                EnableHardwareMode = true
            },
            Security =
            {
                RestrictedLogon = true
            }
        };
        configuration.External.UseMsRdpExHooks = true;
        configuration.External.MsRdpEx.EnableMouseJiggler = true;
        configuration.External.MsRdpEx.MouseJigglerInterval = 45;
        configuration.External.MsRdpEx.MouseJigglerMethod = KeepAliveMethod.KeyboardInput;
        configuration.External.MsRdpEx.KdcProxyUrl = "https://kdc.example.test/kdcproxy";
        configuration.External.MsRdpEx.UserSpecifiedServerName = "server-alias.example.test";

        var content = Build(configuration);

        Assert.Contains("DisableUDPTransport:i:1", content);
        Assert.Contains("AllowBackgroundInput:i:1", content);
        Assert.Contains("RelativeMouseMode:i:0", content);
        Assert.Contains("EnableHardwareMode:i:1", content);
        Assert.Contains("EnableMouseJiggler:i:1", content);
        Assert.Contains("MouseJigglerInterval:i:45", content);
        Assert.Contains("MouseJigglerMethod:i:1", content);
        Assert.Contains("KDCProxyURL:s:https://kdc.example.test/kdcproxy", content);
        Assert.Contains("UserSpecifiedServerName:s:server-alias.example.test", content);
        Assert.Contains("RestrictedLogon:i:1", content);
    }

    [Fact]
    public void Build_WritesRemoteAppSettings_WhenRemoteAppIsEnabled()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            RemoteApp =
            {
                Enabled = true,
                Program = "EXCEL",
                Name = "Microsoft Excel",
                CommandLine = "\"C:\\Docs\\Budget.xlsx\"",
                WorkingDirectory = "%USERPROFILE%\\Documents",
                File = "C:\\Docs\\Budget.xlsx",
                Icon = "C:\\Icons\\Excel.ico",
                DisableCapabilitiesCheck = true
            }
        };

        configuration.SessionMode = RdpSessionMode.External;

        var content = Build(configuration);

        Assert.Contains("remoteapplicationmode:i:1", content);
        Assert.Contains("remoteapplicationprogram:s:EXCEL", content);
        Assert.Contains("remoteapplicationname:s:Microsoft Excel", content);
        Assert.Contains("remoteapplicationcmdline:s:\"C:\\Docs\\Budget.xlsx\"", content);
        Assert.Contains("remoteapplicationexpandcmdline:i:1", content);
        Assert.Contains("remoteapplicationexpandworkingdir:i:1", content);
        Assert.Contains("shell working directory:s:%USERPROFILE%\\Documents", content);
        Assert.Contains("remoteapplicationfile:s:C:\\Docs\\Budget.xlsx", content);
        Assert.Contains("remoteapplicationicon:s:C:\\Icons\\Excel.ico", content);
        Assert.Contains("disableremoteappcapscheck:i:1", content);
    }

    [Fact]
    public void Build_WritesGatewayAndRedirectionServerSettings_WhenConfigured()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.External,
            Connection =
            {
                UseRedirectionServerName = true
            },
            Gateway =
            {
                GatewayUsageMethod = GatewayUsageMethod.UseDefaultSettings,
                GatewayProfileUsageMethod = GatewayProfileUsageMethod.Explicit,
                GatewayCredsSource = GatewayCredentialSource.UseLoggedOnUserCredentials,
                GatewayUserSelectedCredsSource = GatewayCredentialSource.CookieBasedAuthentication,
                GatewayCredSharing = true,
                GatewayHostname = "gateway.example.test",
                GatewayUsername = "gw-user",
                GatewayDomain = "LAB",
                GatewayPassword = new SensitiveString("gateway-secret")
            }
        };

        var content = Build(configuration);

        Assert.Contains("use redirection server name:i:1", content);
        Assert.Contains("gatewayusagemethod:i:3", content);
        Assert.Contains("gatewayprofileusagemethod:i:1", content);
        Assert.Contains("gatewaycredentialssource:i:2", content);
        Assert.Contains("gatewayuserselectedcredentialssource:i:5", content);
        Assert.Contains("promptcredentialonce:i:1", content);
        Assert.Contains("gatewayhostname:s:gateway.example.test", content);
        Assert.Contains("gatewayusername:s:gw-user", content);
        Assert.Contains("gatewaydomain:s:LAB", content);
        Assert.DoesNotContain("gatewaypassword", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Build_WritesGatewayUsageWithoutHostname_WhenDefaultGatewaySettingsAreRequested()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.External,
            Gateway =
            {
                GatewayUsageMethod = GatewayUsageMethod.UseDefaultSettings
            }
        };

        var content = Build(configuration);

        Assert.Contains("gatewayusagemethod:i:3", content);
    }

    [Fact]
    public void Build_Throws_WhenRemoteAppIsEnabledWithoutProgram()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            RemoteApp =
            {
                Enabled = true
            }
        };

        configuration.SessionMode = RdpSessionMode.External;

        Assert.ThrowsAny<ArgumentException>(() => Build(configuration));
    }

    private static string Build(RdpClientConfiguration configuration)
    {
        return RdpFileBuilder.Build(RdpConnectionContextFactory.Create(configuration));
    }
}
