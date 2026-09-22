---
layout: home

hero:
  name: "RoyalApps RDP"
  text: "WinForms Remote Desktop sessions"
  tagline: "Embedded ActiveX hosting and external mstsc.exe sessions in one library."
  image:
    src: /assets/RoyalApps_1024.png
    alt: RoyalApps RDP
  actions:
    - theme: brand
      text: 🚀 Getting Started
      link: /articles/getting-started
    - theme: alt
      text: 💡 API Reference
      link: /api/
    - theme: alt
      text: 🔗 GitHub
      link: https://github.com/royalapplications/royalapps-community-rdp

features:
  - title: Embedded Sessions
    details: Host the Microsoft Remote Desktop ActiveX control directly inside `RdpControl` with display, input, resizing, and zoom support.
  - title: External Sessions
    details: Launch `mstsc.exe` or `mstscex.exe` with generated `.rdp` files, process tracking, temporary credential staging, and window activation support.
  - title: MsRdpEx Integration
    details: Use typed MsRdpEx settings, bundled launcher fallback for `x64` and `arm64`, and app-local probing without hand-authoring raw `.rdp` entries.
---

## Version 2.0.4

`RoyalApps.Community.Rdp.WinForms` provides a WinForms `RdpControl` for Microsoft Remote Desktop sessions.

Highlights:

- embedded ActiveX-hosted RDP sessions
- automatic MsRdpEx hooks for embedded RD Gateway sessions, enabling gateway connection isolation without logging or capture
- external `mstsc.exe` sessions driven by generated `.rdp` files
- optional `mstscex.exe` launch when MsRdpEx hooks are available
- unified configuration with mode-aware validation

See [Getting Started](/articles/getting-started#embedded-rd-gateway-isolation) for gateway isolation and process-wide logging configuration.

![RDP screenshot](/assets/Screenshot.png)

## Start Here

- [Getting Started](/articles/getting-started)
- [Support Matrix](/articles/support-matrix)
- [External Mode](/articles/external-mode)
- [API Reference](/api/)
