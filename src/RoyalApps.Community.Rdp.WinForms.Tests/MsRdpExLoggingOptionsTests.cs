using System;
using System.IO;
using RoyalApps.Community.Rdp.WinForms.Controls.ActiveX;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class MsRdpExLoggingOptionsTests
{
    [Theory]
    [InlineData("TRACE")]
    [InlineData("debug")]
    [InlineData("Info")]
    [InlineData("WARN")]
    [InlineData("ERROR")]
    [InlineData("FATAL")]
    public void TryCreate_AcceptsSupportedLevelsCaseInsensitively(string level)
    {
        var result = MsRdpExLoggingOptions.TryCreate(level, @"%TEMP%\MsRdpEx.log", out var options, out var error);

        Assert.True(result, error);
        Assert.Equal(Path.GetFullPath(Environment.ExpandEnvironmentVariables(@"%TEMP%\MsRdpEx.log")), options.FilePath);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("OFF")]
    [InlineData("VERBOSE")]
    public void TryCreate_RejectsUnsupportedLevels(string? level)
    {
        var result = MsRdpExLoggingOptions.TryCreate(level, @"%TEMP%\MsRdpEx.log", out _, out var error);

        Assert.False(result);
        Assert.NotNull(error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryCreate_RejectsEmptyFilePath(string? filePath)
    {
        var result = MsRdpExLoggingOptions.TryCreate("TRACE", filePath, out _, out var error);

        Assert.False(result);
        Assert.NotNull(error);
    }
}
