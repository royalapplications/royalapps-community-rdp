using System;
using Microsoft.Extensions.Logging;

namespace RoyalApps.Community.Rdp.WinForms.External.Credentials;

internal sealed class RdpCredentialScope : IDisposable
{
    private readonly ILogger _logger;
    private readonly string _targetName;
    private readonly CredentialType _credentialType;
    private readonly StoredCredentialSnapshot? _originalCredential;
    private readonly bool _hasTemporaryCredential;

    private RdpCredentialScope(
        string targetName,
        CredentialType credentialType,
        StoredCredentialSnapshot? originalCredential,
        bool hasTemporaryCredential,
        ILogger logger)
    {
        _targetName = targetName;
        _credentialType = credentialType;
        _originalCredential = originalCredential;
        _hasTemporaryCredential = hasTemporaryCredential;
        _logger = logger;
    }

    public static RdpCredentialScope Create(
        string targetName,
        CredentialType defaultCredentialType,
        string userName,
        string password,
        ILogger logger,
        ICredentialStore? credentialStore = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetName);
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(logger);

        credentialStore ??= WindowsCredentialStore.Instance;
        credentialStore.TryRead(targetName, out var originalCredential);

        if (originalCredential is not null &&
            string.Equals(originalCredential.UserName, userName, StringComparison.OrdinalIgnoreCase) &&
            originalCredential.MatchesPassword(password))
        {
            logger.LogDebug("Using existing Windows credential for {TargetName}.", targetName);
            return new RdpCredentialScope(targetName, originalCredential.Type, originalCredential, hasTemporaryCredential: false, logger);
        }

        var credentialType = originalCredential?.Type ?? defaultCredentialType;
        credentialStore.Write(credentialType, targetName, userName, password);
        logger.LogDebug("Stored temporary Windows credential for {TargetName}.", targetName);

        return new RdpCredentialScope(targetName, credentialType, originalCredential, hasTemporaryCredential: true, logger)
        {
            _credentialStore = credentialStore
        };
    }

    private ICredentialStore _credentialStore = WindowsCredentialStore.Instance;

    public void Dispose()
    {
        if (!_hasTemporaryCredential)
            return;

        try
        {
            if (_originalCredential is not null)
            {
                _credentialStore.Write(_originalCredential);
                _logger.LogDebug("Restored original Windows credential for {TargetName}.", _targetName);
                return;
            }

            _credentialStore.Delete(_targetName, _credentialType);
            _logger.LogDebug("Removed temporary Windows credential for {TargetName}.", _targetName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to restore or remove the Windows credential for {TargetName}.", _targetName);
        }
    }
}
