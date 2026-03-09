using System;
using RoyalApps.Community.Rdp.WinForms.Configuration.Internal;

namespace RoyalApps.Community.Rdp.WinForms.External.Files;

internal static class ExternalRdpFileMapper
{
    public static string Build(RdpConnectionContext connectionContext)
    {
        ArgumentNullException.ThrowIfNull(connectionContext);

        return RdpFileBuilder.Build(connectionContext);
    }
}
