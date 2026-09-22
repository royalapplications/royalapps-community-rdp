using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Connection;
using RoyalApps.Community.Rdp.WinForms.Controls.Clients;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class RdpControlInitializationTests
{
    [Fact]
    public void GatewayOnlyInitialization_AllowsLoggingOnReconnect_AndHonorsExplicitDisable()
    {
        Exception? testException = null;
        var thread = new Thread(() =>
        {
            try
            {
                var logger = new RecordingLogger();
                using var rdpControl = new RdpControl { Logger = logger };
                rdpControl.RdpConfiguration.Server = "rdp.example.test";
                rdpControl.RdpConfiguration.ClientVersion = 8;
                rdpControl.RdpConfiguration.Gateway.GatewayUsageMethod = GatewayUsageMethod.Always;
                rdpControl.RdpConfiguration.Gateway.GatewayHostname = "gateway.example.test";
                // Unsupported PAA stops initialization after hook/log setup, before any network connection.
                rdpControl.RdpConfiguration.Gateway.GatewayAccessToken = new SensitiveString("secureaccess");
                // Use the Windows null device: assert configuration without retaining native log files.
                rdpControl.RdpConfiguration.LogFilePath = Path.Combine(Path.GetTempPath(), "NUL");

                Assert.Contains("version 9 or later", Assert.Throws<NotSupportedException>(rdpControl.Connect).Message);
                Assert.Empty(logger.LoggingConfigurations);
                rdpControl.Disconnect();

                rdpControl.RdpConfiguration.LogEnabled = true;
                Assert.Contains("version 9 or later", Assert.Throws<NotSupportedException>(rdpControl.Connect).Message);
                rdpControl.Disconnect();
                Assert.Single(logger.LoggingConfigurations);

                // Explicit process-wide disable must remain authoritative on reconnect.
                RdpControl.ConfigureProcessWideMsRdpExLogging(false, null, null, NullLogger.Instance);

                Assert.Contains("version 9 or later", Assert.Throws<NotSupportedException>(rdpControl.Connect).Message);
                Assert.Single(logger.LoggingConfigurations);
            }
            catch (Exception ex)
            {
                testException = ex;
            }
            finally
            {
                try
                {
                    RdpControl.ConfigureProcessWideMsRdpExLogging(false, null, null, NullLogger.Instance);
                }
                catch (Exception ex)
                {
                    testException ??= ex;
                }
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        Assert.True(thread.Join(TimeSpan.FromSeconds(30)), "The STA logging regression test did not complete within 30 seconds.");
        if (testException is not null)
            ExceptionDispatchInfo.Capture(testException).Throw();
    }

    private sealed class RecordingLogger : ILogger
    {
        public List<string> LoggingConfigurations { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            if (message.StartsWith("Configured process-wide MsRdpEx logging:", StringComparison.Ordinal))
                LoggingConfigurations.Add(message);
        }
    }

    [Fact]
    public void Connect_CleansUpActiveXClient_WhenPaaIsUnsupported()
    {
        Exception? testException = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var rdpControl = new RdpControl();
                rdpControl.RdpConfiguration.Server = "rdp.example.test";
                rdpControl.RdpConfiguration.ClientVersion = 8;
                rdpControl.RdpConfiguration.Gateway.GatewayUsageMethod = GatewayUsageMethod.Always;
                rdpControl.RdpConfiguration.Gateway.GatewayHostname = "gateway.example.test";
                rdpControl.RdpConfiguration.Gateway.GatewayAccessToken = new SensitiveString("secureaccess");

                var exception = Assert.Throws<NotSupportedException>(rdpControl.Connect);

                Assert.Contains("version 9 or later", exception.Message, StringComparison.Ordinal);
                Assert.Null(rdpControl.RdpClient);
                Assert.DoesNotContain(
                    rdpControl.Controls.Cast<Control>(),
                    control => control is IRdpClient);
            }
            catch (Exception ex)
            {
                testException = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        Assert.True(thread.Join(TimeSpan.FromSeconds(30)), "The STA regression test did not complete within 30 seconds.");
        if (testException is not null)
            ExceptionDispatchInfo.Capture(testException).Throw();
    }
}
