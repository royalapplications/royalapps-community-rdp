using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Connection;
using RoyalApps.Community.Rdp.WinForms.Configuration.Internal;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class GatewayAuthenticationPolicyTests
{
    [Fact]
    public void AccessToken_DerivesPaaSettings_WithoutMutatingConfiguration()
    {
        var gateway = new GatewayConfiguration
        {
            GatewayProfileUsageMethod = GatewayProfileUsageMethod.Default,
            GatewayCredsSource = GatewayCredentialSource.UsernameAndPassword,
            GatewayAccessToken = new SensitiveString("secureaccess")
        };

        Assert.True(GatewayAuthenticationPolicy.UsesPluggableAuthentication(gateway));
        Assert.Equal(GatewayProfileUsageMethod.Explicit, GatewayAuthenticationPolicy.GetEffectiveProfileUsageMethod(gateway));
        Assert.Equal(GatewayCredentialSource.CookieBasedAuthentication, GatewayAuthenticationPolicy.GetEffectiveCredentialSource(gateway));
        Assert.Equal(GatewayProfileUsageMethod.Default, gateway.GatewayProfileUsageMethod);
        Assert.Equal(GatewayCredentialSource.UsernameAndPassword, gateway.GatewayCredsSource);
        Assert.Equal("****", gateway.GatewayAccessToken!.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void MissingAccessToken_PreservesConfiguredSettings(string? accessToken)
    {
        var gateway = new GatewayConfiguration
        {
            GatewayProfileUsageMethod = GatewayProfileUsageMethod.Default,
            GatewayCredsSource = GatewayCredentialSource.UseLoggedOnUserCredentials,
            GatewayAccessToken = accessToken is null ? null : new SensitiveString(accessToken)
        };

        Assert.False(GatewayAuthenticationPolicy.UsesPluggableAuthentication(gateway));
        Assert.Equal(GatewayProfileUsageMethod.Default, GatewayAuthenticationPolicy.GetEffectiveProfileUsageMethod(gateway));
        Assert.Equal(GatewayCredentialSource.UseLoggedOnUserCredentials, GatewayAuthenticationPolicy.GetEffectiveCredentialSource(gateway));
    }
}
