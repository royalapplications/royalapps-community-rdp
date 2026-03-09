# Migrate from v1

Version `2.0.0-beta.1` keeps `RdpControl` as the main entry point, but the configuration model now distinguishes between embedded and external sessions.

Breaking changes and additions:

- `RdpClientConfiguration.SessionMode` selects embedded vs external hosting.
- `RdpClientConfiguration.External` contains launcher, credential, temp-file, and cleanup settings.
- `RdpClientConfiguration.RemoteApp` is the new dedicated model for RemoteApp publishing. Do not overload `Program` for that purpose.
- `ConnectedEventArgs` and `DisconnectedEventArgs` now expose `SessionMode`.
- External mode does not expose an `IRdpClient` instance because the session is hosted by a separate process.
- Security configuration now includes both high-level modes (`RemoteCredentialGuard`, `RestrictedAdminMode`) and low-level flags (`DisableCredentialsDelegation`, `RedirectedAuthentication`, `RestrictedLogon`).
- Invalid mixed-mode combinations are now rejected during validation instead of being silently ignored.

See [Support Matrix](./support-matrix.md) for the supported feature split between embedded and external sessions.
