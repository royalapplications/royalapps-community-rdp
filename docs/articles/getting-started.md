# Getting Started

`RoyalApps.Community.Rdp.WinForms` can now run in two modes:

- `Embedded`: the existing ActiveX-hosted experience inside `RdpControl`.
- `External`: a generated `.rdp` file plus an external `mstsc.exe` or `mstscex.exe` process.

## Install the Package

Install the package from NuGet first:

```powershell
Install-Package RoyalApps.Community.Rdp.WinForms
```

Or with the .NET CLI:

```bash
dotnet add package RoyalApps.Community.Rdp.WinForms
```

## Basic Setup

Create the control and configure a basic embedded session:

```csharp
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

Use external session mode:

```csharp
control.RdpConfiguration.SessionMode = RdpSessionMode.External;
control.RdpConfiguration.External.UseCredentialManager = true;
control.RdpConfiguration.External.KillProcessOnHostExit = true;

control.Connect();
```

RemoteApp example:

```csharp
control.RdpConfiguration.SessionMode = RdpSessionMode.External;
control.RdpConfiguration.RemoteApp.Enabled = true;
control.RdpConfiguration.RemoteApp.Program = "EXCEL";
control.RdpConfiguration.RemoteApp.Name = "Microsoft Excel";
control.RdpConfiguration.RemoteApp.CommandLine = "\"C:\\Docs\\Budget.xlsx\"";

control.Connect();
```

`RemoteApp` is supported for external sessions. `Program` remains the alternate-shell model for full desktop sessions.

For a quick overview of which settings are valid in each hosting mode, see [Support Matrix](./support-matrix.md).

Validation notes:

- `RemoteApp` and `Program` cannot be combined in one connection attempt.
- `External.SelectedMonitors` is rejected in embedded mode instead of being ignored.
- `RemoteApp` is rejected in embedded mode because the RemoteApp window is not truly hosted inside the control.

Security configuration notes:

- `RemoteCredentialGuard` is the higher-level option for redirecting authentication back to the local device.
- `RestrictedAdminMode` is the higher-level option for “connect without sending reusable credentials”.
- `DisableCredentialsDelegation`, `RedirectedAuthentication`, and `RestrictedLogon` are the low-level building blocks behind those modes.
- `AuthenticationServiceClass` is mainly for embedded ActiveX sessions that need a non-default SPN service class.
