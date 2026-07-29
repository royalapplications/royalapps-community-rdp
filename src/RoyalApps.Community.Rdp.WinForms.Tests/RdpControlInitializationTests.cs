using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Connection;
using RoyalApps.Community.Rdp.WinForms.Controls.Clients;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class RdpControlInitializationTests
{
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
