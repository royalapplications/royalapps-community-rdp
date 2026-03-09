using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging.Abstractions;
using RoyalApps.Community.Rdp.WinForms.External.Runtime;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public sealed class BundledMsRdpExLauncherTests
{
    [Theory]
    [InlineData(Architecture.X64, "win-x64")]
    [InlineData(Architecture.Arm64, "win-arm64")]
    public void TryGetRuntimeIdentifier_ReturnsBundledRuntimeIdentifiers(Architecture architecture, string expectedRuntimeIdentifier)
    {
        var result = BundledMsRdpExLauncher.TryGetRuntimeIdentifier(architecture, out var runtimeIdentifier);

        Assert.True(result);
        Assert.Equal(expectedRuntimeIdentifier, runtimeIdentifier);
    }

    [Theory]
    [InlineData(Architecture.X86)]
    [InlineData(Architecture.Arm)]
    [InlineData(Architecture.Wasm)]
    [InlineData(Architecture.LoongArch64)]
    [InlineData(Architecture.S390x)]
    public void TryGetRuntimeIdentifier_ReturnsFalse_ForArchitecturesWithoutBundledLauncher(Architecture architecture)
    {
        var result = BundledMsRdpExLauncher.TryGetRuntimeIdentifier(architecture, out var runtimeIdentifier);

        Assert.False(result);
        Assert.Equal(string.Empty, runtimeIdentifier);
    }

    [Theory]
    [InlineData("win-x64")]
    [InlineData("win-arm64")]
    public void GetResourceName_UsesExpectedExecutableLogicalName(string runtimeIdentifier)
    {
        var resourceName = BundledMsRdpExLauncher.GetResourceName(runtimeIdentifier, BundledMsRdpExLauncher.ExecutableName);

        Assert.Equal(
            $"RoyalApps.Community.Rdp.WinForms.Resources.Tools.{runtimeIdentifier}.mstscex.exe",
            resourceName);
    }

    [Theory]
    [InlineData("win-x64")]
    [InlineData("win-arm64")]
    public void GetResourceName_UsesExpectedNativeLibraryLogicalName(string runtimeIdentifier)
    {
        var resourceName = BundledMsRdpExLauncher.GetResourceName(runtimeIdentifier, BundledMsRdpExLauncher.NativeLibraryName);

        Assert.Equal(
            $"RoyalApps.Community.Rdp.WinForms.Resources.Tools.{runtimeIdentifier}.MsRdpEx.dll",
            resourceName);
    }

    [Fact]
    public void TryResolve_ExtractsBundledFiles_AndReusesExistingCacheEntry()
    {
        var cacheRoot = Path.Combine(Path.GetTempPath(), "RoyalApps.BundledMsRdpExLauncher.Tests", Guid.NewGuid().ToString("N"));

        try
        {
            var executablePath = BundledMsRdpExLauncher.TryResolve(NullLogger.Instance, Architecture.X64, cacheRoot);

            Assert.NotNull(executablePath);
            Assert.EndsWith(BundledMsRdpExLauncher.ExecutableName, executablePath, StringComparison.OrdinalIgnoreCase);
            Assert.True(File.Exists(executablePath));

            var cacheDirectory = Path.GetDirectoryName(executablePath)!;
            var nativeLibraryPath = Path.Combine(cacheDirectory, BundledMsRdpExLauncher.NativeLibraryName);
            Assert.True(File.Exists(nativeLibraryPath));

            var executableWriteTime = File.GetLastWriteTimeUtc(executablePath);
            var nativeLibraryWriteTime = File.GetLastWriteTimeUtc(nativeLibraryPath);

            var secondExecutablePath = BundledMsRdpExLauncher.TryResolve(NullLogger.Instance, Architecture.X64, cacheRoot);

            Assert.Equal(executablePath, secondExecutablePath, StringComparer.OrdinalIgnoreCase);
            Assert.Equal(executableWriteTime, File.GetLastWriteTimeUtc(executablePath));
            Assert.Equal(nativeLibraryWriteTime, File.GetLastWriteTimeUtc(nativeLibraryPath));
        }
        finally
        {
            TryDeleteDirectory(cacheRoot);
        }
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
