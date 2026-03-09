using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using RoyalApps.Community.Rdp.WinForms.Configuration;

namespace RoyalApps.Community.Rdp.WinForms.External.Runtime;

internal static class MsRdpExLocator
{
    private static readonly string[] DefaultSearchPaths =
    [
        @"%ProgramFiles%\Devolutions\MsRdpEx\mstscex.exe",
        @"%ProgramFiles(x86)%\Devolutions\MsRdpEx\mstscex.exe",
        @"%LocalAppData%\Programs\MsRdpEx\mstscex.exe"
    ];

    public static string? ResolveLauncherPath(RdpClientConfiguration configuration, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);

        if (!configuration.External.UseMsRdpExHooks)
            return null;

        if (!string.IsNullOrWhiteSpace(configuration.External.MsRdpExLauncherPath))
        {
            var configuredPath = Environment.ExpandEnvironmentVariables(configuration.External.MsRdpExLauncherPath);
            if (File.Exists(configuredPath))
                return configuredPath;

            logger.LogWarning("Configured MsRdpEx launcher path was not found: {Path}", configuredPath);
        }

        foreach (var candidate in EnumerateCandidates(configuration))
        {
            if (!File.Exists(candidate))
                continue;

            logger.LogDebug("Resolved MsRdpEx launcher path: {Path}", candidate);
            return candidate;
        }

        var bundledLauncherPath = BundledMsRdpExLauncher.TryResolve(logger);
        if (!string.IsNullOrWhiteSpace(bundledLauncherPath))
            return bundledLauncherPath;

        logger.LogWarning("MsRdpEx hooks were requested, but mstscex.exe was not found. Falling back to mstsc.exe.");
        return null;
    }

    private static IEnumerable<string> EnumerateCandidates(RdpClientConfiguration configuration)
    {
        foreach (var path in configuration.External.MsRdpExSearchPaths)
        {
            if (string.IsNullOrWhiteSpace(path))
                continue;

            var expandedPath = Environment.ExpandEnvironmentVariables(path);
            if (expandedPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                yield return expandedPath;
                continue;
            }

            yield return Path.Combine(expandedPath, "mstscex.exe");
        }

        foreach (var path in DefaultSearchPaths)
            yield return Environment.ExpandEnvironmentVariables(path);
    }
}
