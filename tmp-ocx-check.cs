using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using AxMSTSCLib;
using MSTSCLib;

var client = new AxMsRdpClient10NotSafeForScripting();
((ISupportInitialize)client).BeginInit();
var ocx = client.GetOcx();
Console.WriteLine($"Before CreateControl: {(ocx == null ? "null" : ocx.GetType().FullName)}");
try
{
    var handle = client.Handle;
    Console.WriteLine($"Handle created: {handle}");
}
catch (Exception ex)
{
    Console.WriteLine($"Handle error: {ex.GetType().Name}: {ex.Message}");
}
ocx = client.GetOcx();
Console.WriteLine($"After Handle: {(ocx == null ? "null" : ocx.GetType().FullName)}");
if (ocx != null)
{
    Console.WriteLine($"is ITSRemoteProgram: {ocx is ITSRemoteProgram}");
    Console.WriteLine($"is ITSRemoteProgram2: {ocx is ITSRemoteProgram2}");
    Console.WriteLine($"is IMsRdpClientShell: {ocx is IMsRdpClientShell}");
    Console.WriteLine($"is IMsRdpClientNonScriptable5: {ocx is IMsRdpClientNonScriptable5}");
    var unk = Marshal.GetIUnknownForObject(ocx);
    try
    {
        var iid1 = typeof(ITSRemoteProgram).GUID;
        var iid2 = typeof(ITSRemoteProgram2).GUID;
        var iid3 = typeof(IMsRdpClientShell).GUID;
        Console.WriteLine($"ITSRemoteProgram GUID: {iid1}");
        Console.WriteLine($"ITSRemoteProgram2 GUID: {iid2}");
        Console.WriteLine($"IMsRdpClientShell GUID: {iid3}");
        Console.WriteLine($"QI ITSRemoteProgram: {Marshal.QueryInterface(unk, ref iid1, out var p1)} ptr={p1}");
        if (p1 != IntPtr.Zero) Marshal.Release(p1);
        Console.WriteLine($"QI ITSRemoteProgram2: {Marshal.QueryInterface(unk, ref iid2, out var p2)} ptr={p2}");
        if (p2 != IntPtr.Zero) Marshal.Release(p2);
        Console.WriteLine($"QI IMsRdpClientShell: {Marshal.QueryInterface(unk, ref iid3, out var p3)} ptr={p3}");
        if (p3 != IntPtr.Zero) Marshal.Release(p3);
    }
    finally
    {
        Marshal.Release(unk);
    }
}
((ISupportInitialize)client).EndInit();
