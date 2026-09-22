using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MsRdpEx;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Controls;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

// Each test launches this entry point in a fresh process because native hooks and logging are global.
internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            Run(args[0], args[1]);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void Run(string scenario, string directory)
    {
        var first = Path.Combine(directory, "first.log");
        var logger = new InitializationLogger();

        if (scenario is "gateway-default" or "gateway-disabled")
        {
            Initialize(false, first, logger);
            var core = new RdpCoreApi();
            Require(core.GatewayIsolationEnabled == (scenario == "gateway-default"), "Gateway startup override was not respected.");
            return;
        }

        if (scenario == "disabled-then-enabled")
        {
            Initialize(false, first, logger);
            Require(logger.Configurations == 0 && !File.Exists(first), "Disabled initialization configured logging.");
        }
        else if (scenario == "explicit-disable")
        {
            RdpControl.ConfigureProcessWideMsRdpExLogging(false, null, null, logger);
            Initialize(true, first, logger);
            Require(logger.Configurations == 0 && !File.Exists(first), "Connection overrode explicit disable.");
            RdpControl.ConfigureProcessWideMsRdpExLogging(true, "TRACE", first, logger);
        }

        if (scenario != "explicit-disable")
            Initialize(true, first, logger);
        Require(logger.Configurations == 1, "Logging was not configured exactly once.");

        // Load emits native diagnostics without connecting to an RDP server.
        var api = new RdpCoreApi();
        api.Load();
        RdpControl.ConfigureProcessWideMsRdpExLogging(false, null, null, logger);

        if (scenario == "runtime-reconfigure")
        {
            var length = new FileInfo(first).Length;
            api.Load();
            Require(new FileInfo(first).Length == length, "Disabled logging still wrote diagnostics.");
            RdpControl.ConfigureProcessWideMsRdpExLogging(true, "FATAL", first, logger);
            api.Load();
            Require(new FileInfo(first).Length == length, "Runtime log level did not filter diagnostics.");
            RdpControl.ConfigureProcessWideMsRdpExLogging(true, "TRACE", first, logger);
            api.Load();
            Require(new FileInfo(first).Length > length, "Re-enabled logging did not resume diagnostics.");
            RdpControl.ConfigureProcessWideMsRdpExLogging(false, null, null, logger);
        }
    }

    private static void Initialize(bool logging, string path, InitializationLogger logger)
    {
        using var control = new RdpControl { Logger = logger };
        control.RdpConfiguration.Server = "rdp.example.test";
        control.RdpConfiguration.ClientVersion = 8;
        control.RdpConfiguration.Gateway.GatewayUsageMethod = GatewayUsageMethod.Always;
        control.RdpConfiguration.LogEnabled = logging;
        control.RdpConfiguration.LogLevel = "TRACE";
        control.RdpConfiguration.LogFilePath = path;
        try
        {
            control.Connect();
            throw new InvalidOperationException("Initialization did not stop before network connection.");
        }
        catch (InitializationCompleteException)
        {
            Require(!string.IsNullOrEmpty(control.RdpClient!.RdpExDll), "Hooks were not enabled.");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private sealed class InitializationCompleteException : Exception;

    private sealed class InitializationLogger : ILogger
    {
        public int Configurations { get; private set; }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            if (message.StartsWith("Configured process-wide MsRdpEx logging:", StringComparison.Ordinal))
                Configurations++;
            // Stop after native ActiveX creation and before control setup or network activity.
            if (message.StartsWith("Created embedded RDP client:", StringComparison.Ordinal))
                throw new InitializationCompleteException();
        }
    }
}
