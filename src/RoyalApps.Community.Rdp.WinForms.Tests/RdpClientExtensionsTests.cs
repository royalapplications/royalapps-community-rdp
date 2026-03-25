using System;
using RoyalApps.Community.Rdp.WinForms.Controls.ActiveX;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class RdpClientExtensionsTests
{
    [Theory]
    [InlineData(RdpProperties.EnableMouseJiggler)]
    [InlineData(RdpProperties.KeyboardHookToggleShortcutEnabled)]
    [InlineData(RdpProperties.KeyboardHookToggleShortcutKey)]
    [InlineData(RdpProperties.MouseJigglerInterval)]
    [InlineData(RdpProperties.MouseJigglerMethod)]
    [InlineData(RdpProperties.ZoomLevel)]
    public void RequiresMsRdpExHook_ReturnsTrue_ForHookOnlyProperties(string propertyName)
    {
        Assert.True(RdpClientExtensions.RequiresMsRdpExHook(propertyName));
    }

    [Theory]
    [InlineData(RdpProperties.DisableUdpTransport)]
    [InlineData(RdpProperties.EnableRdsAadAuth)]
    [InlineData(RdpProperties.RestrictedLogon)]
    [InlineData(RdpProperties.UseRedirectionServerName)]
    public void RequiresMsRdpExHook_ReturnsFalse_ForBuiltInProperties(string propertyName)
    {
        Assert.False(RdpClientExtensions.RequiresMsRdpExHook(propertyName));
    }

    [Fact]
    public void IsUnsupportedWithoutMsRdpExHook_ReturnsTrue_ForHookOnlyPropertyWithoutHook()
    {
        var unsupported = RdpClientExtensions.IsUnsupportedWithoutMsRdpExHook(
            null,
            RdpProperties.KeyboardHookToggleShortcutEnabled,
            new NotSupportedException());

        Assert.True(unsupported);
    }

    [Fact]
    public void IsUnsupportedWithoutMsRdpExHook_ReturnsFalse_ForHookOnlyPropertyWhenHookIsAvailable()
    {
        var unsupported = RdpClientExtensions.IsUnsupportedWithoutMsRdpExHook(
            @"D:\tools\MsRdpEx.dll",
            RdpProperties.KeyboardHookToggleShortcutEnabled,
            new NotSupportedException());

        Assert.False(unsupported);
    }

    [Fact]
    public void IsUnsupportedWithoutMsRdpExHook_ReturnsFalse_ForBuiltInPropertyWithoutHook()
    {
        var unsupported = RdpClientExtensions.IsUnsupportedWithoutMsRdpExHook(
            @"D:\tools\MsRdpEx.dll",
            RdpProperties.EnableRdsAadAuth,
            new NotSupportedException());

        Assert.False(unsupported);
    }

    [Fact]
    public void IsUnsupportedWithoutMsRdpExHook_ReturnsTrue_ForUnexpectedComFailureWithoutHook()
    {
        var unsupported = RdpClientExtensions.IsUnsupportedWithoutMsRdpExHook(
            null,
            RdpProperties.EnableRdsAadAuth,
            new System.Runtime.InteropServices.COMException("Catastrophic failure", unchecked((int)0x8000FFFF)));

        Assert.True(unsupported);
    }
}
