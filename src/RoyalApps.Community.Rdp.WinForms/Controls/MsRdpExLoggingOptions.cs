using System;
using System.IO;
using MsRdpEx;

namespace RoyalApps.Community.Rdp.WinForms.Controls;

internal sealed record MsRdpExLoggingOptions(MsRdpEx_LogLevel Level, string FilePath)
{
    public static bool TryCreate(string? level, string? filePath, out MsRdpExLoggingOptions options, out string? error)
    {
        options = null!;
        error = null;

        if (!Enum.TryParse(level, true, out MsRdpEx_LogLevel parsedLevel) || parsedLevel == MsRdpEx_LogLevel.Off)
        {
            error = $"The log level '{level}' is invalid.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(filePath))
        {
            error = "The log file path is empty.";
            return false;
        }

        try
        {
            var expandedPath = Environment.ExpandEnvironmentVariables(filePath);
            var fullPath = Path.GetFullPath(expandedPath);
            options = new MsRdpExLoggingOptions(parsedLevel, fullPath);
            return true;
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            error = $"The log file path is invalid: {ex.Message}";
            return false;
        }
    }
}
