using System;
using System.Security.Cryptography;
using System.Text;

namespace RoyalApps.Community.Rdp.WinForms.Controls.ActiveX;

internal static class GatewayAccessTokenProtector
{
    public static string Protect(string accessToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(accessToken);

        var clearText = Encoding.Unicode.GetBytes(accessToken + '\0');
        var protectedData = ProtectedData.Protect(clearText, null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(protectedData);
    }
}
