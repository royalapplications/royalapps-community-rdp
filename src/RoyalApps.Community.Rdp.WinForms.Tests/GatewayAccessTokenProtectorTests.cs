using System;
using System.Security.Cryptography;
using System.Text;
using RoyalApps.Community.Rdp.WinForms.Controls.ActiveX;
using RoyalApps.Community.Rdp.WinForms.Controls.Clients;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class GatewayAccessTokenProtectorTests
{
    [Fact]
    public void Protect_ProducesCurrentUserDpapiCookie_WithUtf16Terminator()
    {
        const string accessToken = "secureaccess";

        var encryptedAuthCookie = GatewayAccessTokenProtector.Protect(accessToken);
        var protectedData = Convert.FromBase64String(encryptedAuthCookie);
        var clearText = ProtectedData.Unprotect(protectedData, null, DataProtectionScope.CurrentUser);

        Assert.Equal(accessToken + '\0', Encoding.Unicode.GetString(clearText));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Protect_RejectsMissingTokens(string? accessToken)
    {
        Assert.ThrowsAny<ArgumentException>(() => GatewayAccessTokenProtector.Protect(accessToken!));
    }

    [Theory]
    [InlineData(typeof(RdpClient9), true)]
    [InlineData(typeof(RdpClient10), true)]
    [InlineData(typeof(RdpClient11), true)]
    [InlineData(typeof(RdpClient12), true)]
    [InlineData(typeof(RdpClient8), false)]
    public void GatewayPaaCapability_MatchesActiveXTransportSettingsVersion(Type clientType, bool expected)
    {
        Assert.Equal(expected, typeof(IGatewayPaaClient).IsAssignableFrom(clientType));
    }
}
