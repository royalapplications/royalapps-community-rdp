# RDP Control

[![NuGet Version](https://img.shields.io/nuget/v/RoyalApps.Community.Rdp.WinForms.svg?style=flat)](https://www.nuget.org/packages/RoyalApps.Community.Rdp.WinForms)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RoyalApps.Community.Rdp.WinForms.svg?color=green)](https://www.nuget.org/packages/RoyalApps.Community.Rdp.WinForms)
[![.NET](https://img.shields.io/badge/.NET-%3E%3D%20%208.0-blueviolet)](https://dotnet.microsoft.com/download)

RoyalApps.Community.RDP contains projects/packages to easily embed/use Microsoft RDP ActiveX wrapped in [MsRdpEx](https://github.com/Devolutions/MsRdpEx) in a Windows (WinForms) application.
![Screenshot](https://raw.githubusercontent.com/royalapplications/royalapps-community-rdp/main/docs/assets/Screenshot.png)

## Getting Started
### Installation
You should install the RoyalApps.Community.RDP.WinForms with NuGet:
```
Install-Package RoyalApps.Community.RDP.WinForms
```
or via the command line interface:
```
dotnet add package RoyalApps.Community.RDP.WinForms
```

### Using the RdpControl
#### Add Control
Place the `RdpControl` on a form or in a container control (user control, tab control, etc.) and set the `Dock` property to `DockStyle.Fill`

#### Set Properties
To configure all RDP relevant settings, use the properties of the `RdpClientConfiguration` class which is accessible through the `RdpControl.RdpConfiguration` property.

#### Connect and Disconnect
Once the configuration is set, call:
```csharp
RdpControl.Connect();
```
to start a connection.

To disconnect, simply call:
```csharp
RdpControl.Disconnect();
```
#### Subscribe to Events
When the connection has been established, the `Connected` event is raised.

The `Disconnected` event is raised when:
* the connection couldn't be established (server not reachable, incorrect credentials)
* the connection has been interrupted (network failure)
* the connection was closed by the user (logoff or disconnect)

The `DisconnectedEventArgs` may have an error code or error message for more information.

## Exploring the Demo Application
The demo application is quite simple. The `Remote Desktop` menu has the following items:
### Connect
Starts the remote desktop connection.

### Disconnect
Stops the remote desktop connection.

### Settings
Shows a window with all the settings from the `RdpClientConfiguration` class. Edit/change the settings before you click on `Connect`.

## Notable Features

### Use Microsoft Remote Desktop Client
One of the most interesting possibilities of this package is to use the Microsoft's modern Remote Desktop Client (RDC) instead of the Terminal Services Client (MSTSC) which ships with Windows. Just set `RdpClientConfiguration.UseMsRdc` to true and ensure that the [Remote Desktop Client](https://www.microsoft.com/store/productId/9WZDNCRFJ3PS) is installed from the Microsoft Store.

### Auto Expand Desktop Size
If `DesktopWidth` and `DesktopHeight` properties are set to `0` (default), the remote desktop size is determined by the container size the control is placed on.

### Resize Behavior
You can set the `RdpClientConfiguration.Display.ResizeBehavior` property to one of the following:
- Scrollbars: displays scrollbars when the container (form) size decreases.
- SmartSizing: scales down the remote desktop while preserving the original desktop size.
- SmartReconnect: adapts the remote desktop size to accordingly when the container (form) size changes.

Call the `RdpControl.SetResizeBehavior(ResizeBehavior resizeBehavior)` method to change the behavior while connected.

#### Scaling and Zoom
##### Auto Scaling
Setting the `RdpClientConfiguration.Display.AutoScaling` to true will automatically adapt the scaling factor to system's the DPI scaling during connect.

Alternatively you can provide the `RdpClientConfiguration.Display.InitialZoomLevel` to set a custom zoom level.

##### Changing Zoom Level
While connected you can use the methods `RdpControl.ZoomIn()` and `RdpControl.ZoomOut()` to change the zoom level.

##### Local Scaling
Setting `RdpClientConfiguration.Display.UseLocalScaling` to true will scale on the client side instead of settings the remote zoom level (DPI settings). With this enabled, `SmartSizing` as `ResizeBehavior` will not be possible.

## Acknowledgements
Special thanks to [Marc-André Moreau](https://github.com/awakecoding) / Devolutions for providing the [MsRdpEx](https://github.com/Devolutions/MsRdpEx) package.
