using System;
using System.Diagnostics;

namespace RoyalApps.Community.Rdp.WinForms.External.Runtime;

internal interface IExternalProcess : IDisposable
{
    Process? UnderlyingProcess { get; }

    int Id { get; }

    bool HasExited { get; }

    int ExitCode { get; }

    bool EnableRaisingEvents { get; set; }

    event EventHandler? Exited;

    bool CloseMainWindow();

    bool WaitForExit(TimeSpan timeout);

    void Kill(bool entireProcessTree);
}
