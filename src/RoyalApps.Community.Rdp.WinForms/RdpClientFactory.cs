using System;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Interfaces;

namespace RoyalApps.Community.Rdp.WinForms;

/// <summary>
/// RdpClient factory class.
/// </summary>
public class RdpClientFactory
{
    /// <summary>
    /// Creates an RdpClient instance
    /// </summary>
    /// <param name="rdpClientVersion">The RDP client version. Default = 0 which creates the highest possible version.</param>
    /// <param name="rdpClientConfiguration"></param>
    /// <returns>An RdpClient instance</returns>
    /// <exception cref="Exception">Throws an exception if no RDP client </exception>
    public static IRdpClient Create(int rdpClientVersion = 0, RdpClientConfiguration? rdpClientConfiguration = null)
    {
        if (rdpClientVersion is 0 or >= 10 && Create<RdpClient10>(rdpClientConfiguration, out var rdpClient, out var exception))
            return rdpClient!;

        if (rdpClientVersion is 0 or >= 9 && Create<RdpClient9>(rdpClientConfiguration, out rdpClient, out exception))
            return rdpClient!;
        
        if (rdpClientVersion is 0 or >= 8 && Create<RdpClient8>(rdpClientConfiguration, out rdpClient, out exception))
            return rdpClient!;
        
        if (rdpClientVersion is 0 or >= 7 && Create<RdpClient7>(rdpClientConfiguration, out rdpClient, out exception))
            return rdpClient!;
        
        if (Create<RdpClient2>(rdpClientConfiguration, out rdpClient, out exception))
            return rdpClient!;
        
        throw new Exception("Failed to create RDP client instance.", exception);
    }

    private static bool Create<T>(RdpClientConfiguration? rdpClientConfiguration, out IRdpClient? rdpClient, out Exception? exception) where T : IRdpClient
    {
        rdpClient = null;
        exception = null;

        try
        {
            if (rdpClientConfiguration == null)
                rdpClient = Activator.CreateInstance<T>();
            else
                rdpClient = (T)Activator.CreateInstance(typeof(T), rdpClientConfiguration)!;
            return true;
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
    }
}