using System;
using System.Diagnostics;

namespace RoyalApps.Community.Rdp.WinForms.External.Runtime;

internal sealed class ExternalProcessAdapter : IExternalProcess
{
    private readonly Process _process;

    public ExternalProcessAdapter(Process process)
    {
        _process = process ?? throw new ArgumentNullException(nameof(process));
    }

    public Process UnderlyingProcess => _process;

    public int Id => _process.Id;

    public bool HasExited => _process.HasExited;

    public int ExitCode => _process.ExitCode;

    public bool EnableRaisingEvents
    {
        get => _process.EnableRaisingEvents;
        set => _process.EnableRaisingEvents = value;
    }

    public event EventHandler? Exited
    {
        add => _process.Exited += value;
        remove => _process.Exited -= value;
    }

    public bool CloseMainWindow()
    {
        return _process.CloseMainWindow();
    }

    public bool WaitForExit(TimeSpan timeout)
    {
        return _process.WaitForExit(timeout);
    }

    public void Kill(bool entireProcessTree)
    {
        _process.Kill(entireProcessTree);
    }

    public void Dispose()
    {
        _process.Dispose();
    }
}
