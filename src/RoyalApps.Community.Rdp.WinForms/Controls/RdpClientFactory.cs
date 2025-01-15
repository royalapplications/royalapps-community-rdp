using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AxMSTSCLib;
using RoyalApps.Community.Rdp.WinForms.Interfaces;

namespace RoyalApps.Community.Rdp.WinForms.Controls;

/// <summary>
/// RdpClient factory class.
/// </summary>
public class RdpClientFactory
{
    /// <summary>
    /// Creates an RdpClient instance
    /// </summary>
    /// <param name="rdpClientVersion">The RDP client version. Default = 0 which creates the highest possible version.</param>
    /// <param name="dllPath">Path to the MsRdpEx.dll</param>
    /// <param name="useMsRdc">Should Microsoft Remote Desktop Client (rdclientax.dll) be used instead of MSTSC (mstscax.dll)</param>
    /// <returns>An RdpClient instance</returns>
    /// <exception cref="Exception">Throws an exception if no RDP client </exception>
    public static IRdpClient Create(int rdpClientVersion = 0, string? dllPath = null, bool useMsRdc = false)
    {
        if (rdpClientVersion is 0 or >= 12 && Create<RdpClient12>(dllPath, useMsRdc, out var rdpClient, out var exception))
            return rdpClient!;
        
        if (rdpClientVersion is 0 or >= 10 && Create<RdpClient10>(dllPath, useMsRdc, out rdpClient, out exception))
            return rdpClient!;

        if (rdpClientVersion is 0 or >= 9 && Create<RdpClient9>(dllPath, useMsRdc, out rdpClient, out exception))
            return rdpClient!;
        
        if (rdpClientVersion is 0 or >= 8 && Create<RdpClient8>(dllPath, useMsRdc, out rdpClient, out exception))
            return rdpClient!;
        
        if (rdpClientVersion is 0 or >= 7 && Create<RdpClient7>(dllPath, useMsRdc, out rdpClient, out exception))
            return rdpClient!;
        
        if (Create<RdpClient2>(dllPath, useMsRdc, out rdpClient, out exception))
            return rdpClient!;
        
        throw new Exception("Failed to create RDP client instance.", exception);
    }

    private static bool Create<T>(string? dllPath, bool useMsRdc, out IRdpClient? rdpClient, out Exception? exception) where T : AxHostEx, IRdpClient
    {
        rdpClient = null;
        exception = null;

        try
        {
            // Only calling the constructor isn't enough, as the native RDP control will be created at a later point
            var client = Activator.CreateInstance<T>();

            var axName = "mstsc";
            var hookingEnabled = !string.IsNullOrEmpty(dllPath); 
            if (hookingEnabled)
            {
                client.RdpExDll = dllPath!;
                
                if (useMsRdc)
                {
                    axName = "msrdc";
                }
            }

            client.AxName = axName;

            // Read the clsid from the AxHost, and manually create an instance of the RDP control
            var clsid = (Guid)typeof(AxHost).GetField("_clsid", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(client)!;
            var instance = client.RdpCreateInstance(clsid);
            Marshal.ReleaseComObject(instance);

            rdpClient = client;
            return true;
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
    }
}
