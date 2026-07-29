using System;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Connection;
using RoyalApps.Community.Rdp.WinForms.Configuration.Internal;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class RdpConfigurationValidatorTests
{
    [Fact]
    public void Create_Throws_WhenEmbeddedModeUsesSelectedMonitors()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.Embedded
        };
        configuration.External.SelectedMonitors = "0,1";

        Assert.Throws<NotSupportedException>(() => RdpConnectionContextFactory.Create(configuration));
    }

    [Fact]
    public void Create_Throws_WhenEmbeddedModeUsesRemoteApp()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.Embedded,
            RemoteApp =
            {
                Enabled = true,
                Program = "EXCEL"
            }
        };

        Assert.Throws<NotSupportedException>(() => RdpConnectionContextFactory.Create(configuration));
    }

    [Fact]
    public void Create_Throws_WhenProgramAndRemoteAppAreCombined()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.External,
            RemoteApp =
            {
                Enabled = true,
                Program = "EXCEL"
            },
            Program =
            {
                StartProgram = "pwsh.exe"
            }
        };

        Assert.Throws<InvalidOperationException>(() => RdpConnectionContextFactory.Create(configuration));
    }

    [Fact]
    public void Create_Allows_ExternalModeWithSelectedMonitors()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.External,
            Display =
            {
                UseMultimon = true
            }
        };
        configuration.External.SelectedMonitors = "0,1";

        var context = RdpConnectionContextFactory.Create(configuration);

        Assert.True(context.IsExternalMode);
        Assert.Equal("0,1", context.Configuration.External.SelectedMonitors);
    }

    [Fact]
    public void Create_Allows_ExternalModeWithRemoteApp()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.External,
            RemoteApp =
            {
                Enabled = true,
                Program = "EXCEL"
            }
        };

        var context = RdpConnectionContextFactory.Create(configuration);

        Assert.True(context.IsExternalMode);
        Assert.True(context.EffectiveRemoteApp.Enabled);
        Assert.Equal("EXCEL", context.EffectiveRemoteApp.Program);
    }

    [Fact]
    public void Create_Allows_EmbeddedModeWithProgram()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.Embedded,
            Program =
            {
                StartProgram = "pwsh.exe",
                WorkDir = "C:\\"
            }
        };

        var context = RdpConnectionContextFactory.Create(configuration);

        Assert.True(context.IsEmbeddedMode);
        Assert.Equal("pwsh.exe", context.Configuration.Program.StartProgram);
        Assert.Equal("C:\\", context.Configuration.Program.WorkDir);
    }

    [Fact]
    public void Create_Throws_WhenAccessTokenIsConfiguredWithoutGatewayUsage()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            Gateway =
            {
                GatewayHostname = "gateway.example.test",
                GatewayAccessToken = new SensitiveString("secureaccess")
            }
        };

        Assert.Throws<InvalidOperationException>(() => RdpConnectionContextFactory.Create(configuration));
    }

    [Fact]
    public void Create_Throws_WhenAccessTokenIsConfiguredWithoutGatewayHostname()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            Gateway =
            {
                GatewayUsageMethod = GatewayUsageMethod.Always,
                GatewayAccessToken = new SensitiveString("secureaccess")
            }
        };

        Assert.ThrowsAny<ArgumentException>(() => RdpConnectionContextFactory.Create(configuration));
    }

    [Fact]
    public void Create_AllowsEmptyAccessTokenWithoutGatewayConfiguration()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            Gateway =
            {
                GatewayAccessToken = new SensitiveString(string.Empty)
            }
        };

        var context = RdpConnectionContextFactory.Create(configuration);

        Assert.True(context.IsEmbeddedMode);
    }
}
