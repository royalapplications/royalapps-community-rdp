using System;
using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.External.Runtime;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public sealed class MsRdpExLocatorTests
{
    [Fact]
    public void ResolveLauncherPath_ReturnsNull_WhenHooksAreDisabled()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.External
        };
        configuration.External.MsRdpExLauncherPath = @"C:\Tools\mstscex.exe";

        var launcherPath = MsRdpExLocator.ResolveLauncherPath(configuration, NullLogger.Instance);

        Assert.Null(launcherPath);
    }

    [Fact]
    public void ResolveLauncherPath_PrefersExplicitLauncherPath_OverSearchPaths()
    {
        var tempDirectory = CreateTemporaryDirectory();
        try
        {
            var explicitPath = CreateLauncherFile(tempDirectory, "explicit-mstscex.exe");
            var searchDirectory = Path.Combine(tempDirectory, "AppLocal");
            Directory.CreateDirectory(searchDirectory);
            CreateLauncherFile(searchDirectory, "mstscex.exe");

            var configuration = CreateExternalConfiguration();
            configuration.External.MsRdpExLauncherPath = explicitPath;
            configuration.External.MsRdpExSearchPaths.Add(searchDirectory);

            var launcherPath = MsRdpExLocator.ResolveLauncherPath(configuration, NullLogger.Instance);

            Assert.Equal(explicitPath, launcherPath, StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            TryDeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void ResolveLauncherPath_UsesAppLocalSearchPath_WhenExplicitLauncherPathIsMissing()
    {
        var tempDirectory = CreateTemporaryDirectory();
        try
        {
            var searchDirectory = Path.Combine(tempDirectory, "AppLocal");
            Directory.CreateDirectory(searchDirectory);
            var appLocalLauncher = CreateLauncherFile(searchDirectory, "mstscex.exe");

            var configuration = CreateExternalConfiguration();
            configuration.External.MsRdpExLauncherPath = Path.Combine(tempDirectory, "missing", "mstscex.exe");
            configuration.External.MsRdpExSearchPaths.Add(searchDirectory);

            var launcherPath = MsRdpExLocator.ResolveLauncherPath(configuration, NullLogger.Instance);

            Assert.Equal(appLocalLauncher, launcherPath, StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            TryDeleteDirectory(tempDirectory);
        }
    }

    [Fact]
    public void ResolveLauncherPath_UsesFullFilePathSearchEntry_WhenProvided()
    {
        var tempDirectory = CreateTemporaryDirectory();
        try
        {
            var appLocalLauncher = CreateLauncherFile(tempDirectory, "bundled-mstscex.exe");

            var configuration = CreateExternalConfiguration();
            configuration.External.MsRdpExSearchPaths.Add(appLocalLauncher);

            var launcherPath = MsRdpExLocator.ResolveLauncherPath(configuration, NullLogger.Instance);

            Assert.Equal(appLocalLauncher, launcherPath, StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            TryDeleteDirectory(tempDirectory);
        }
    }

    private static RdpClientConfiguration CreateExternalConfiguration()
    {
        return new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.External,
            External =
            {
                UseMsRdpExHooks = true
            }
        };
    }

    private static string CreateLauncherFile(string directory, string fileName)
    {
        Directory.CreateDirectory(directory);
        var filePath = Path.Combine(directory, fileName);
        File.WriteAllText(filePath, "stub");
        return filePath;
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "RoyalApps.MsRdpExLocator.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void TryDeleteDirectory(string path)
    {
        if (!Directory.Exists(path))
            return;

        try
        {
            Directory.Delete(path, recursive: true);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
