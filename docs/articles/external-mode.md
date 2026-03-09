# External Mode

External mode writes a temporary `.rdp` file, optionally stages credentials in the Windows credential manager, and launches the Remote Desktop client as a separate process.

Key behaviors:

- The process is tracked with a Windows job object when `External.KillProcessOnHostExit` is enabled, so the external session is terminated if the host process is killed.
- Temporary credentials are restored or removed after the external client exits.
- Monitor selection can be written through `External.SelectedMonitors`. This is intentionally external-only because the embedded ActiveX host cannot reliably present arbitrary selected-monitor layouts.
- `RemoteApp` settings are written as standard `.rdp` properties when `RemoteApp.Enabled` is true.
- gateway settings and `Connection.UseRedirectionServerName` are written as standard `.rdp` properties for brokered RDS / AVD-style connections.
- when both target-server and RD Gateway credentials are configured, the credential manager staging path now handles them as two independent temporary credentials with separate restore/remove behavior.
- `RdpControl.ActivateExternalSessionWindow()` can be used as a best-effort helper to bring the external launcher window back to the foreground.
- `RdpControl.ExternalSessionWindowChanged` reports when the launcher window is discovered or lost, and `RdpControl.ExternalSessionWindowHandle` exposes the last known handle.
- Additional `.rdp` settings can be appended through `External.AdditionalRdpSettings`.
- `External.MsRdpExSearchPaths` can be used to probe app-local directories or full file paths for `mstscex.exe` before the built-in well-known installation folders.
- when no configured or installed `mstscex.exe` is found, the library can extract bundled `x64` or `arm64` copies of both `mstscex.exe` and `MsRdpEx.dll` into a per-user cache folder as a fallback.
- the bundled fallback cache location is `%LocalAppData%\RoyalApps.Community.Rdp.WinForms\MsRdpEx\<version>\<rid>`.
- only `win-x64` and `win-arm64` are bundled. `x86` still requires an explicit path, search path, or installed MsRdpEx launcher.
- `External.MsRdpEx` provides typed hook-specific settings for `KDCProxyURL`, `UserSpecifiedServerName`, `EnableMouseJiggler`, `MouseJigglerInterval`, and `MouseJigglerMethod`.
- shared settings such as `Connection.DisableUdpTransport`, `Input.AllowBackgroundInput`, `Input.RelativeMouseMode`, `Performance.EnableHardwareMode`, and the low-level security flags are also emitted in MsRdpEx form when hooks are enabled.
- raw MsRdpEx-specific settings can still be appended through `External.AdditionalMsRdpExSettings`, and those entries are emitted after the typed mapping so they can override it when necessary.

Security behavior:

- `Security.RemoteCredentialGuard` implies `DisableCredentialsDelegation` and `RedirectedAuthentication`.
- `Security.RestrictedAdminMode` implies `DisableCredentialsDelegation` and `RestrictedLogon`.
- The low-level security flags are only written into the external session when `External.UseMsRdpExHooks` is enabled.
- `AuthenticationServiceClass` is not written to the generated `.rdp` file and should be treated as an embedded-mode setting.

Example:

```csharp
control.RdpConfiguration.SessionMode = RdpSessionMode.External;
control.RdpConfiguration.External.UseMsRdpExHooks = true;
control.RdpConfiguration.External.MsRdpExSearchPaths.Add(
    Path.Combine(AppContext.BaseDirectory, "MsRdpEx"));
control.RdpConfiguration.External.SelectedMonitors = "0,2";
control.RdpConfiguration.External.AdditionalRdpSettings.Add(
    new RdpFileSetting("disableconnectionsharing", RdpFileSettingType.Integer, "1"));
```
