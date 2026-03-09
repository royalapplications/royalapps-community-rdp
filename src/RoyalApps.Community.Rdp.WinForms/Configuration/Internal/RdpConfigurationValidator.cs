using System;

namespace RoyalApps.Community.Rdp.WinForms.Configuration.Internal;

internal static class RdpConfigurationValidator
{
    public static void ValidateForConnect(RdpConnectionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        ArgumentException.ThrowIfNullOrWhiteSpace(context.Configuration.Server, nameof(context.Configuration.Server));

        if (context.EffectiveRemoteApp.Enabled)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(context.EffectiveRemoteApp.Program, "RemoteApp.Program");

            if (!string.IsNullOrWhiteSpace(context.Configuration.Program.StartProgram) ||
                !string.IsNullOrWhiteSpace(context.Configuration.Program.WorkDir))
            {
                throw new InvalidOperationException("Program configuration cannot be combined with RemoteApp mode. Use RdpConfiguration.RemoteApp instead.");
            }
        }

        if (!context.IsEmbeddedMode)
            return;

        if (context.EffectiveRemoteApp.Enabled)
            throw new NotSupportedException("RemoteApp is only supported for external sessions. Embedded hosting cannot provide a production-ready embedded RemoteApp window.");

        if (!string.IsNullOrWhiteSpace(context.Configuration.External.SelectedMonitors))
            throw new NotSupportedException("Selected monitors are only supported for external sessions.");
    }
}
