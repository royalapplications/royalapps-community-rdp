using System;
using System.Threading;
using MsRdpEx;

// ReSharper disable InconsistentNaming

namespace RoyalApps.Community.Rdp.WinForms.Controls;

internal class MsRdpExManager
{
    private static readonly RdpCoreApi _coreApi;
    private static readonly bool _axHookEnabled = true;

    private static readonly Lazy<MsRdpExManager> _instance = new(() => new MsRdpExManager(), LazyThreadSafetyMode.ExecutionAndPublication);
    public static MsRdpExManager Instance => _instance.Value;

    public RdpCoreApi CoreApi => _coreApi;
    public bool AxHookEnabled => _axHookEnabled;

    static MsRdpExManager()
    {
        _coreApi = LoadCoreApi();
    }

    private static RdpCoreApi LoadCoreApi()
    {
        var coreApi = new RdpCoreApi();

        var logFilePath = Environment.ExpandEnvironmentVariables("%LocalAppData%\\MsRdpEx\\HostApp.log");
        var pcapFilePath = Environment.ExpandEnvironmentVariables("%LocalAppData%\\MsRdpEx\\capture.pcap");

        coreApi.LogEnabled = true;
        coreApi.LogLevel = MsRdpEx_LogLevel.Trace;
        coreApi.LogFilePath = logFilePath;
        coreApi.PcapEnabled = false;
        coreApi.PcapFilePath = pcapFilePath;
        coreApi.AxHookEnabled = _axHookEnabled;
        coreApi.Load();

        return coreApi;
    }
}
