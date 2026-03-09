using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RoyalApps.Community.Rdp.WinForms.Configuration;

namespace RoyalApps.Community.Rdp.WinForms.External.Credentials;

internal sealed class ExternalCredentialScopes : IDisposable
{
    private readonly List<RdpCredentialScope> _scopes = [];

    private ExternalCredentialScopes()
    {
    }

    public static ExternalCredentialScopes? Create(RdpClientConfiguration configuration, ILogger logger, ICredentialStore? credentialStore = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);

        if (!configuration.External.UseCredentialManager)
            return null;

        credentialStore ??= WindowsCredentialStore.Instance;
        var credentialScopes = new ExternalCredentialScopes();
        credentialScopes.AddServerScope(configuration, logger, credentialStore);
        credentialScopes.AddGatewayScope(configuration, logger, credentialStore);

        return credentialScopes._scopes.Count == 0
            ? null
            : credentialScopes;
    }

    public void Dispose()
    {
        for (var index = _scopes.Count - 1; index >= 0; index--)
            _scopes[index].Dispose();

        _scopes.Clear();
    }

    private void AddServerScope(RdpClientConfiguration configuration, ILogger logger, ICredentialStore credentialStore)
    {
        var userName = CredentialTargetResolver.FormatUserName(configuration.Credentials);
        var password = configuration.Credentials.Password?.GetValue();
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrEmpty(password))
            return;

        var targetName = CredentialTargetResolver.Resolve(configuration);
        var scope = RdpCredentialScope.Create(
            targetName,
            CredentialType.Generic,
            userName,
            password,
            logger,
            credentialStore);

        _scopes.Add(scope);
    }

    private void AddGatewayScope(RdpClientConfiguration configuration, ILogger logger, ICredentialStore credentialStore)
    {
        var userName = CredentialTargetResolver.FormatGatewayUserName(configuration.Gateway);
        var password = configuration.Gateway.GatewayPassword?.GetValue();
        if (string.IsNullOrWhiteSpace(configuration.Gateway.GatewayHostname) ||
            string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrEmpty(password))
        {
            return;
        }

        var targetName = CredentialTargetResolver.ResolveGateway(configuration);
        var scope = RdpCredentialScope.Create(
            targetName,
            CredentialType.DomainPassword,
            userName,
            password,
            logger,
            credentialStore);

        _scopes.Add(scope);
    }
}
