using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace RoyalApps.Community.Rdp.WinForms.External.Runtime;

internal static class BundledMsRdpExLauncher
{
    internal const string ExecutableName = "mstscex.exe";
    internal const string NativeLibraryName = "MsRdpEx.dll";

    public static string? TryResolve(ILogger logger)
    {
        return TryResolve(logger, RuntimeInformation.ProcessArchitecture);
    }

    internal static string? TryResolve(ILogger logger, Architecture architecture, string? cacheRootOverride = null)
    {
        ArgumentNullException.ThrowIfNull(logger);

        if (!TryGetRuntimeIdentifier(architecture, out var runtimeIdentifier))
            return null;

        var assembly = typeof(BundledMsRdpExLauncher).Assembly;
        using var executableResourceStream = assembly.GetManifestResourceStream(GetResourceName(runtimeIdentifier, ExecutableName));
        using var nativeLibraryResourceStream = assembly.GetManifestResourceStream(GetResourceName(runtimeIdentifier, NativeLibraryName));
        if (executableResourceStream is null || nativeLibraryResourceStream is null)
        {
            logger.LogDebug(
                "Bundled MsRdpEx launcher resources for runtime {RuntimeIdentifier} were not found.",
                runtimeIdentifier);
            return null;
        }

        var cacheDirectory = GetCacheDirectory(runtimeIdentifier, cacheRootOverride);
        Directory.CreateDirectory(cacheDirectory);

        var executablePath = Path.Combine(cacheDirectory, ExecutableName);
        WriteFileIfMissingOrDifferent(executablePath, executableResourceStream);

        var extractedDllPath = Path.Combine(cacheDirectory, NativeLibraryName);
        WriteFileIfMissingOrDifferent(extractedDllPath, nativeLibraryResourceStream);

        logger.LogDebug("Resolved bundled MsRdpEx launcher path: {Path}", executablePath);
        return executablePath;
    }

    internal static bool TryGetRuntimeIdentifier(Architecture architecture, out string runtimeIdentifier)
    {
        switch (architecture)
        {
            case Architecture.X64:
                runtimeIdentifier = "win-x64";
                return true;
            case Architecture.Arm64:
                runtimeIdentifier = "win-arm64";
                return true;
            default:
                runtimeIdentifier = string.Empty;
                return false;
        }
    }

    internal static string GetResourceName(string runtimeIdentifier, string fileName)
    {
        return $"RoyalApps.Community.Rdp.WinForms.Resources.Tools.{runtimeIdentifier}.{fileName}";
    }

    internal static string GetCacheDirectory(string runtimeIdentifier, string? cacheRootOverride = null)
    {
        var version = typeof(BundledMsRdpExLauncher).Assembly.GetName().Version?.ToString() ?? "unknown";
        var rootDirectory = string.IsNullOrWhiteSpace(cacheRootOverride)
            ? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RoyalApps.Community.Rdp.WinForms",
                "MsRdpEx")
            : cacheRootOverride;

        return Path.Combine(
            rootDirectory,
            version,
            runtimeIdentifier);
    }

    private static void WriteFileIfMissingOrDifferent(string destinationPath, Stream sourceStream)
    {
        using var memoryStream = new MemoryStream();
        sourceStream.CopyTo(memoryStream);
        var bytes = memoryStream.ToArray();

        if (File.Exists(destinationPath))
        {
            var existingBytes = File.ReadAllBytes(destinationPath);
            if (existingBytes.AsSpan().SequenceEqual(bytes))
                return;
        }

        File.WriteAllBytes(destinationPath, bytes);
    }
}
