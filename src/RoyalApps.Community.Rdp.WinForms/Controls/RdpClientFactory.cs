using System;
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
    /// <returns>An RdpClient instance</returns>
    /// <exception cref="Exception">Throws an exception if no RDP client </exception>
    public static IRdpClient Create(int rdpClientVersion = 0)
    {
        if (rdpClientVersion is 0 or >= 10 && Create<RdpClient10>(out var rdpClient, out var exception))
            return rdpClient!;

        if (rdpClientVersion is 0 or >= 9 && Create<RdpClient9>(out rdpClient, out exception))
            return rdpClient!;
        
        if (rdpClientVersion is 0 or >= 8 && Create<RdpClient8>(out rdpClient, out exception))
            return rdpClient!;
        
        if (rdpClientVersion is 0 or >= 7 && Create<RdpClient7>(out rdpClient, out exception))
            return rdpClient!;
        
        if (Create<RdpClient2>(out rdpClient, out exception))
            return rdpClient!;
        
        throw new Exception("Failed to create RDP client instance.", exception);
    }

    private static bool Create<T>(out IRdpClient? rdpClient, out Exception? exception) where T : IRdpClient
    {
        rdpClient = null;
        exception = null;

        try
        {
            rdpClient = Activator.CreateInstance<T>();
            return true;
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
    }
}