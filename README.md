# RDP Control

[![NuGet Version](https://img.shields.io/nuget/v/RoyalApps.Community.Rdp.WinForms.svg?style=flat)](https://www.nuget.org/packages/RoyalApps.Community.Rdp.WinForms)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RoyalApps.Community.Rdp.WinForms.svg?color=green)](https://www.nuget.org/packages/RoyalApps.Community.Rdp.WinForms)
[![.NET](https://img.shields.io/badge/.NET-net10.0--windows-blueviolet)](https://dotnet.microsoft.com/download)

`RoyalApps.Community.Rdp.WinForms` provides a WinForms `RdpControl` for Microsoft Remote Desktop sessions.

Version `2.0.0-beta.1` supports both:

- embedded ActiveX-hosted RDP sessions
- external `mstsc.exe` sessions driven by a generated `.rdp` file

When available, external mode can also launch through `mstscex.exe` for MsRdpEx-based hooks.

## Documentation

Read the documentation first:

- [Documentation site](https://royalapplications.github.io/royalapps-community-rdp/)
- [Getting Started](https://royalapplications.github.io/royalapps-community-rdp/articles/getting-started)
- [Support Matrix](https://royalapplications.github.io/royalapps-community-rdp/articles/support-matrix)
- [External Mode](https://royalapplications.github.io/royalapps-community-rdp/articles/external-mode)
- [API Reference](https://royalapplications.github.io/royalapps-community-rdp/api/)

![Screenshot](https://raw.githubusercontent.com/royalapplications/royalapps-community-rdp/main/docs/assets/Screenshot.png)

## Installation

```powershell
Install-Package RoyalApps.Community.Rdp.WinForms
```

```cmd
dotnet add package RoyalApps.Community.Rdp.WinForms
```

## Quick Start

```csharp
using System.Windows.Forms;
using RoyalApps.Community.Rdp.WinForms;
using RoyalApps.Community.Rdp.WinForms.Configuration;

var control = new RdpControl
{
    Dock = DockStyle.Fill
};

control.RdpConfiguration.Server = "rdp.example.test";
control.RdpConfiguration.Credentials.Username = "alice";
control.RdpConfiguration.Credentials.Password = new SensitiveString("secret");
control.RdpConfiguration.SessionMode = RdpSessionMode.Embedded;

control.Connect();
```

Switch to external mode:

```csharp
control.RdpConfiguration.SessionMode = RdpSessionMode.External;
control.RdpConfiguration.External.UseCredentialManager = true;
control.RdpConfiguration.External.KillProcessOnHostExit = true;

control.Connect();
```

## API Model

`RdpControl` remains the main entry point, but `RdpClientConfiguration` now distinguishes between the hosting models:

- `SessionMode`: `Embedded` or `External`
- `External`: launcher path, temporary file, credential-manager, cleanup, and optional MsRdpEx hook settings
- `Display`, `Connection`, `Credentials`, `Gateway`, `Input`, `Performance`, `Program`, `RemoteApp`, and `Security`: shared connection settings used for both modes

In external mode:

- a temporary `.rdp` file is written with standard mapped settings plus any custom `RdpFileSetting` values
- `External.SelectedMonitors` can target a specific monitor set for `mstsc.exe` or `mstscex.exe`
- gateway settings and `Connection.UseRedirectionServerName` are written into the generated `.rdp` file
- a temporary Windows credential can be staged and later restored or removed
- the launched process can be assigned to a Windows job so it is terminated if the host process exits unexpectedly
- `ExternalSessionWindowChanged` and `ExternalSessionWindowHandle` allow the host to track the external top-level window
- `ConnectedEventArgs.SessionMode` and `DisconnectedEventArgs.SessionMode` report that the session ran externally

`IRdpClient` is only available in embedded mode.

See [`docs/articles/support-matrix.md`](docs/articles/support-matrix.md) for the supported feature split between embedded and external sessions.

## Notable Features

### External mstsc.exe Support

Set `RdpClientConfiguration.SessionMode` to `RdpSessionMode.External` to launch an external Remote Desktop client instead of embedding the ActiveX control. The library writes a temporary `.rdp` file, launches `mstsc.exe`, and tracks the process lifetime.

That generated `.rdp` file now includes the typed gateway configuration and `use redirection server name`, which makes AVD and brokered RDS scenarios closer to the embedded ActiveX path.

### Optional MsRdpEx Launcher

Set `RdpClientConfiguration.External.UseMsRdpExHooks` to `true` to launch through `mstscex.exe` when it is installed, explicitly configured via `External.MsRdpExLauncherPath`, found through `External.MsRdpExSearchPaths` for app-local deployments, or extracted together with the matching bundled `MsRdpEx.dll` for `x64` / `arm64` as a fallback.

The bundled fallback is only provided for `x64` and `arm64`. When it is used, the launcher files are extracted into `%LocalAppData%\RoyalApps.Community.Rdp.WinForms\MsRdpEx\<version>\<rid>`.

`External.MsRdpEx` now provides typed hook-specific settings for the common cases that previously required raw `AdditionalMsRdpExSettings`, including `KdcProxyUrl`, `UserSpecifiedServerName`, `EnableMouseJiggler`, `MouseJigglerInterval`, and `MouseJigglerMethod`.
The existing shared settings `Connection.DisableUdpTransport`, `Input.AllowBackgroundInput`, `Input.RelativeMouseMode`, `Performance.EnableHardwareMode`, and the security low-level flags are also emitted in MsRdpEx form when hooks are enabled.

### Temporary Credential Staging

When `External.UseCredentialManager` is enabled and a username/password is configured, the library stages the credential in the Windows credential manager before launch and restores or removes it after the external process exits.

If gateway-specific credentials are configured through `Gateway.GatewayHostname`, `Gateway.GatewayUsername`, and `Gateway.GatewayPassword`, the library stages a second temporary credential for the RD Gateway endpoint independently from the target server credential.

### RemoteApp

RemoteApp is now modeled through `RdpClientConfiguration.RemoteApp` instead of overloading `Program`.

- External mode writes standard RemoteApp `.rdp` properties such as `remoteapplicationmode` and `remoteapplicationprogram`.
- Embedded mode is intentionally unsupported for RemoteApp because the window is not truly hosted inside the control and behaves as a fragile pseudo-embedded top-level window.
- `Program` and `RemoteApp` cannot be combined in one connection attempt; conflicting intent is rejected during validation.

### Security Flag Precedence

`Security.RemoteCredentialGuard` and `Security.RestrictedAdminMode` are the high-level modes.

- `RemoteCredentialGuard` implies `DisableCredentialsDelegation=true` and `RedirectedAuthentication=true`.
- `RestrictedAdminMode` implies `DisableCredentialsDelegation=true` and `RestrictedLogon=true`.
- If both high-level modes are enabled, all three low-level flags become effective.
- `AuthenticationServiceClass` is an embedded-mode ActiveX setting used to override the SPN service class for authentication. It is mainly relevant for special targets such as Hyper-V console sessions.
- In external mode, the low-level flags are only written when `External.UseMsRdpExHooks` is enabled, because stock `mstsc.exe` does not consume them through the standard `.rdp` mapping used here.

### Validation

The connection pipeline now normalizes and validates configuration before either mode starts.

- Embedded mode rejects external-only features such as `External.SelectedMonitors`.
- Embedded mode rejects `RemoteApp.Enabled`.
- Conflicting intent such as `Program.StartProgram` together with `RemoteApp.Enabled` is rejected instead of silently ignored.

The full supported feature split is documented in [`docs/articles/support-matrix.md`](docs/articles/support-matrix.md).

### Resizing and Zoom

Embedded mode keeps the existing resizing and zoom features:

- `ResizeBehavior.Scrollbars`
- `ResizeBehavior.SmartSizing`
- `ResizeBehavior.SmartReconnect`
- `RdpControl.ZoomIn()`
- `RdpControl.ZoomOut()`

These operations are no-ops in external mode because the session is hosted by a separate process.

## Breaking Changes from v1

- `RdpClientConfiguration.SessionMode` selects embedded vs external hosting.
- `RdpClientConfiguration.External` contains the new external-launch configuration.
- `ConnectedEventArgs` and `DisconnectedEventArgs` now expose `SessionMode`.
- External mode does not expose an `IRdpClient` instance.

## Testing

Run the tests with:

```cmd
dotnet test src/RoyalApps.Community.Rdp.slnx
```

The current suite covers configuration validation, `.rdp` file serialization, credential-target resolution, credential-manager staging, external-session orchestration, launcher resolution, and bundled MsRdpEx fallback extraction.

## Documentation

The documentation site is built with VitePress and the API reference is generated from XML documentation comments.

Useful commands:

```cmd
npm install
npm run docs:api
npm run docs:build
```

Guide pages:

- `docs/articles/getting-started.md`
- `docs/articles/support-matrix.md`
- `docs/articles/external-mode.md`
- `docs/articles/migrating-from-v1.md`

Diagnostic helpers:

- `powershell -ExecutionPolicy Bypass -File .\scripts\Inspect-RdGatewayCredentialTargets.ps1 -LaunchMstsc`

## Acknowledgements

Special thanks to [Marc-André Moreau](https://github.com/awakecoding) / Devolutions for providing [MsRdpEx](https://github.com/Devolutions/MsRdpEx).
