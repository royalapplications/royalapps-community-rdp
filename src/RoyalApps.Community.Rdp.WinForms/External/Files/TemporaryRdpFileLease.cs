using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using RoyalApps.Community.Rdp.WinForms.Configuration.Internal;

namespace RoyalApps.Community.Rdp.WinForms.External.Files;

internal sealed class TemporaryRdpFileLease : ITemporaryRdpFileLease
{
    private readonly bool _keepFile;
    private readonly ILogger _logger;

    private TemporaryRdpFileLease(string filePath, bool keepFile, ILogger logger)
    {
        FilePath = filePath;
        _keepFile = keepFile;
        _logger = logger;
    }

    public string FilePath { get; }

    public static TemporaryRdpFileLease Create(RdpConnectionContext connectionContext, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(connectionContext);
        ArgumentNullException.ThrowIfNull(logger);
        var configuration = connectionContext.Configuration;

        var directory = string.IsNullOrWhiteSpace(configuration.External.TemporaryDirectory)
            ? Path.GetTempPath()
            : Environment.ExpandEnvironmentVariables(configuration.External.TemporaryDirectory);

        Directory.CreateDirectory(directory);

        var filePath = Path.Combine(directory, $"RoyalApps-Rdp-{Guid.NewGuid():N}.rdp");
        var content = ExternalRdpFileMapper.Build(connectionContext);
        File.WriteAllText(filePath, content, Encoding.Unicode);
        logger.LogDebug("Wrote temporary RDP file to {Path}", filePath);

        return new TemporaryRdpFileLease(filePath, configuration.External.KeepTemporaryRdpFile, logger);
    }

    public void Dispose()
    {
        if (_keepFile)
            return;

        try
        {
            if (!File.Exists(FilePath))
                return;

            File.Delete(FilePath);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to delete temporary RDP file {Path}", FilePath);
        }
    }
}
