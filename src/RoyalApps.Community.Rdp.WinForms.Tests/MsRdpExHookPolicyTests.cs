using RoyalApps.Community.Rdp.WinForms.Configuration.Connection;
using RoyalApps.Community.Rdp.WinForms.Controls.ActiveX;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class MsRdpExHookPolicyTests
{
    [Theory]
    [InlineData(GatewayUsageMethod.Never, false)]
    [InlineData(GatewayUsageMethod.Always, true)]
    [InlineData(GatewayUsageMethod.OnDemand, true)]
    [InlineData(GatewayUsageMethod.UseDefaultSettings, true)]
    [InlineData(GatewayUsageMethod.BypassLocalAddresses, true)]
    public void GatewayMode_SelectsHooks_WithoutHostnameLoggingOrCapture(
        GatewayUsageMethod gatewayUsageMethod,
        bool expected)
    {
        var gateway = new GatewayConfiguration { GatewayUsageMethod = gatewayUsageMethod };

        Assert.Null(gateway.GatewayHostname);
        Assert.Equal(expected, MsRdpExHookPolicy.ShouldEnable(false, false, false, gateway.GatewayUsageMethod));
    }

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public void ExistingTrigger_EnablesHooks_WithoutGateway(
        bool useMsRdc,
        bool enableSessionCapture,
        bool logEnabled)
    {
        Assert.True(MsRdpExHookPolicy.ShouldEnable(
            useMsRdc, enableSessionCapture, logEnabled, GatewayUsageMethod.Never));
    }
}
