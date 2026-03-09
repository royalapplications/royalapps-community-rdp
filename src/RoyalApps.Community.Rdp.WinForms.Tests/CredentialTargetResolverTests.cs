using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Connection;
using RoyalApps.Community.Rdp.WinForms.External.Credentials;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class CredentialTargetResolverTests
{
    [Fact]
    public void Resolve_UsesDefaultTermsrvTarget_WhenPortIsDefault()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test"
        };

        var target = CredentialTargetResolver.Resolve(configuration);

        Assert.Equal("TERMSRV/rdp.example.test", target);
    }

    [Fact]
    public void Resolve_IncludesPort_WhenNonDefaultPortIsUsed()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            Port = 3390
        };

        var target = CredentialTargetResolver.Resolve(configuration);

        Assert.Equal("TERMSRV/rdp.example.test:3390", target);
    }

    [Fact]
    public void FormatUserName_CombinesDomainAndUser()
    {
        var credentials = new CredentialConfiguration
        {
            Domain = "LAB",
            Username = "alice"
        };

        var value = CredentialTargetResolver.FormatUserName(credentials);

        Assert.Equal(@"LAB\alice", value);
    }

    [Fact]
    public void ResolveGateway_UsesPlainGatewayHost()
    {
        var configuration = new RdpClientConfiguration();
        configuration.Gateway.GatewayHostname = "gateway.example.test";

        var target = CredentialTargetResolver.ResolveGateway(configuration);

        Assert.Equal("gateway.example.test", target);
    }

    [Fact]
    public void FormatGatewayUserName_CombinesDomainAndUser()
    {
        var gateway = new GatewayConfiguration
        {
            GatewayDomain = "LAB",
            GatewayUsername = "gw-user"
        };

        var value = CredentialTargetResolver.FormatGatewayUserName(gateway);

        Assert.Equal(@"LAB\gw-user", value);
    }
}
