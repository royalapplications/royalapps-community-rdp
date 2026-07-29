using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class RdpDisconnectPolicyTests
{
    [Theory]
    [InlineData(4360, true, true)]
    [InlineData(4360, false, false)]
    [InlineData(2825, true, false)]
    [InlineData(0, true, false)]
    public void ShouldScheduleSmartReconnectFallback_OnlySchedulesFailedActiveReconnect(
        int disconnectReason,
        bool smartReconnectInProgress,
        bool expected)
    {
        Assert.Equal(
            expected,
            RdpControl.ShouldScheduleSmartReconnectFallback(disconnectReason, smartReconnectInProgress));
    }
}
