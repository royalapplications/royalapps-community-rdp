using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace RoyalApps.Community.Rdp.WinForms.Logging;

internal class DebugLogger : ILogger
{
    private static readonly Lazy<DebugLogger> _instance = new Lazy<DebugLogger>(() => new DebugLogger(), LazyThreadSafetyMode.ExecutionAndPublication);
    public static DebugLogger Instance => _instance.Value;

    private DebugLogger() { }
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Debug.WriteLine($"[{eventId.Id, 2}: {logLevel, -12}]");
        Debug.Write($"    DEBUG - ");
        Debug.WriteLine($"{formatter(state, exception)}");
        Debug.WriteLine("");
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;
}