using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Internal;
using RoyalApps.Community.Rdp.WinForms.Configuration.Security;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class SecurityConfigurationExtensionsTests
{
    [Fact]
    public void GetEffectiveSettings_PreservesExplicitLowLevelFlags()
    {
        var configuration = new SecurityConfiguration
        {
            AuthenticationServiceClass = " TERMSRV ",
            DisableCredentialsDelegation = true,
            RedirectedAuthentication = true,
            RestrictedLogon = true
        };

        var effectiveSettings = configuration.GetEffectiveSettings();

        Assert.Equal("TERMSRV", effectiveSettings.AuthenticationServiceClass);
        Assert.True(effectiveSettings.DisableCredentialsDelegation);
        Assert.True(effectiveSettings.RedirectedAuthentication);
        Assert.True(effectiveSettings.RestrictedLogon);
    }

    [Fact]
    public void GetEffectiveSettings_RemoteCredentialGuard_EnablesItsRequiredLowLevelFlags()
    {
        var configuration = new SecurityConfiguration
        {
            RemoteCredentialGuard = true
        };

        var effectiveSettings = configuration.GetEffectiveSettings();

        Assert.True(effectiveSettings.DisableCredentialsDelegation);
        Assert.True(effectiveSettings.RedirectedAuthentication);
        Assert.False(effectiveSettings.RestrictedLogon);
    }

    [Fact]
    public void GetEffectiveSettings_RestrictedAdminMode_EnablesItsRequiredLowLevelFlags()
    {
        var configuration = new SecurityConfiguration
        {
            RestrictedAdminMode = true
        };

        var effectiveSettings = configuration.GetEffectiveSettings();

        Assert.True(effectiveSettings.DisableCredentialsDelegation);
        Assert.False(effectiveSettings.RedirectedAuthentication);
        Assert.True(effectiveSettings.RestrictedLogon);
    }

    [Fact]
    public void GetEffectiveSettings_BothHighLevelModes_EnableAllLowLevelFlags()
    {
        var configuration = new SecurityConfiguration
        {
            RemoteCredentialGuard = true,
            RestrictedAdminMode = true
        };

        var effectiveSettings = configuration.GetEffectiveSettings();

        Assert.True(effectiveSettings.DisableCredentialsDelegation);
        Assert.True(effectiveSettings.RedirectedAuthentication);
        Assert.True(effectiveSettings.RestrictedLogon);
    }
}
