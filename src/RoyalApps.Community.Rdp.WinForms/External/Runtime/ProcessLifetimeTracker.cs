using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace RoyalApps.Community.Rdp.WinForms.External.Runtime;

internal static class ProcessLifetimeTracker
{
    private static readonly Lazy<ProcessJobTracker> Tracker = new(() => new ProcessJobTracker("ExternalRdp"));

    public static void Track(Process process, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(process);
        ArgumentNullException.ThrowIfNull(logger);

        try
        {
            Tracker.Value.AddProcess(process);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to add external RDP process {ProcessId} to the Windows job object.", process.Id);
        }
    }
}
