using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class MsRdpExRegressionTests
{
    [Theory]
    [InlineData("initial-enabled")]
    [InlineData("disabled-then-enabled")]
    [InlineData("explicit-disable")]
    [InlineData("runtime-reconfigure")]
    [InlineData("gateway-default")]
    [InlineData("gateway-disabled")]
    public async Task NativeScenario_RunsInFreshProcess(string scenario)
    {
        var directory = Path.Combine(Path.GetTempPath(), "royal-rdp-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var start = new ProcessStartInfo("dotnet")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            start.ArgumentList.Add(typeof(MsRdpExRegressionTests).Assembly.Location);
            start.ArgumentList.Add(scenario);
            start.ArgumentList.Add(directory);
            foreach (var variable in new[] { "MSRDPEX_GATEWAY_UNIQUE_BINDING", "MSRDPEX_LOG_ENABLED", "MSRDPEX_LOG_LEVEL", "MSRDPEX_LOG_FILE_PATH", "MSRDPEX_DLL_PATH" })
                start.Environment.Remove(variable);
            if (scenario == "gateway-disabled")
                start.Environment["MSRDPEX_GATEWAY_UNIQUE_BINDING"] = "0";

            using var process = Process.Start(start)!;
            var stdout = process.StandardOutput.ReadToEndAsync();
            var stderr = process.StandardError.ReadToEndAsync();
            var exited = process.WaitForExitAsync();
            if (await Task.WhenAny(exited, Task.Delay(TimeSpan.FromSeconds(30))) != exited)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync();
                Assert.Fail("Native scenario timed out: " + scenario);
            }
            Assert.True(process.ExitCode == 0, await stdout + await stderr);

            // Inspect logs only after exit, when all native handles have been released.
            if (!scenario.StartsWith("gateway-", StringComparison.Ordinal))
                Assert.NotEmpty(File.ReadAllText(Path.Combine(directory, "first.log")));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
