namespace RoyalApps.Community.Rdp.WinForms.Configuration.Internal;

internal readonly record struct EffectiveSecuritySettings(
    string? AuthenticationServiceClass,
    bool DisableCredentialsDelegation,
    bool RedirectedAuthentication,
    bool RestrictedLogon);
