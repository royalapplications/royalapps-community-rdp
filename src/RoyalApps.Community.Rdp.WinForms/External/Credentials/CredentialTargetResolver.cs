using System;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Connection;

namespace RoyalApps.Community.Rdp.WinForms.External.Credentials;

internal static class CredentialTargetResolver
{
    public static string Resolve(RdpClientConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (!string.IsNullOrWhiteSpace(configuration.External.CredentialTarget))
            return configuration.External.CredentialTarget.Trim();

        var server = configuration.Server?.Trim();
        ArgumentException.ThrowIfNullOrWhiteSpace(server, nameof(configuration.Server));

        return configuration.Port is > 0 and not 3389
            ? $"TERMSRV/{server}:{configuration.Port}"
            : $"TERMSRV/{server}";
    }

    public static string ResolveGateway(RdpClientConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var gatewayHost = configuration.Gateway.GatewayHostname?.Trim();
        ArgumentException.ThrowIfNullOrWhiteSpace(gatewayHost, nameof(configuration.Gateway.GatewayHostname));
        return gatewayHost;
    }

    public static string? FormatUserName(CredentialConfiguration credentials)
    {
        ArgumentNullException.ThrowIfNull(credentials);

        if (string.IsNullOrWhiteSpace(credentials.Username))
            return null;

        return string.IsNullOrWhiteSpace(credentials.Domain)
            ? credentials.Username.Trim()
            : $"{credentials.Domain.Trim()}\\{credentials.Username.Trim()}";
    }

    public static string? FormatGatewayUserName(GatewayConfiguration gateway)
    {
        ArgumentNullException.ThrowIfNull(gateway);

        if (string.IsNullOrWhiteSpace(gateway.GatewayUsername))
            return null;

        return string.IsNullOrWhiteSpace(gateway.GatewayDomain)
            ? gateway.GatewayUsername.Trim()
            : $"{gateway.GatewayDomain.Trim()}\\{gateway.GatewayUsername.Trim()}";
    }
}
