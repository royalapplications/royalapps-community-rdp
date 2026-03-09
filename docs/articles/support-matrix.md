# Support Matrix

`RoyalApps.Community.Rdp.WinForms` uses one shared `RdpClientConfiguration` model, but not every option is supported by both hosting modes.

Use this page as the contract for which features are valid in `Embedded` and `External` mode.

## Feature Matrix

| Feature / setting | Embedded | External | Notes |
| --- | --- | --- | --- |
| Full desktop session | Yes | Yes | The baseline session model in both modes. |
| `Program` alternate shell | Yes | Yes | Desktop-session only. Cannot be combined with `RemoteApp`. |
| `RemoteApp` | No | Yes | Embedded mode rejects it during validation. |
| `Display.UseMultimon` | Yes | Yes | Embedded uses the ActiveX client property; external writes `use multimon`. |
| `External.SelectedMonitors` | No | Yes | External-only monitor targeting via `selectedmonitors`. |
| `Connection.EnableRdsAadAuth` | Yes | Yes | External writes `enablerdsaadauth`. |
| `Security.AuthenticationServiceClass` | Yes | No | Embedded-only ActiveX setting. It is not written to generated external `.rdp` files. |
| `Security.DisableCredentialsDelegation` | Yes | Limited | External support requires `External.UseMsRdpExHooks`. |
| `Security.RedirectedAuthentication` | Yes | Limited | External support requires `External.UseMsRdpExHooks`. |
| `Security.RestrictedLogon` | Yes | Limited | External support requires `External.UseMsRdpExHooks`. |
| `Security.RemoteCredentialGuard` | Yes | Limited | External support is only effective when the low-level flags are written through MsRdpEx hooks. |
| `Security.RestrictedAdminMode` | Yes | Limited | External support is only effective when the low-level flags are written through MsRdpEx hooks. |
| Zoom / smart sizing / direct `IRdpClient` access | Yes | No | External mode treats these as no-ops because the session runs in another process. |
| Temp `.rdp` file, launcher path, credential staging, process lifetime | No | Yes | These live under `RdpConfiguration.External`. |

## Validation Rules

The library validates unsupported combinations before connect instead of silently ignoring them.

- `RemoteApp` is external-only.
- `Program` and `RemoteApp` cannot be combined in one connection.
- `External.SelectedMonitors` is external-only.
