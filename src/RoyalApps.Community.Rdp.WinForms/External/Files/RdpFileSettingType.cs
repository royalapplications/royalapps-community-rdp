namespace RoyalApps.Community.Rdp.WinForms.External.Files;

/// <summary>
/// Specifies the serialized value type used in an RDP file line.
/// </summary>
public enum RdpFileSettingType
{
    /// <summary>
    /// A string value.
    /// </summary>
    String,

    /// <summary>
    /// A 32-bit integer value.
    /// </summary>
    Integer,

    /// <summary>
    /// A binary value.
    /// </summary>
    Binary
}

internal static class RdpFileSettingTypeExtensions
{
    public static string ToToken(this RdpFileSettingType type)
    {
        return type switch
        {
            RdpFileSettingType.Integer => "i",
            RdpFileSettingType.Binary => "b",
            _ => "s"
        };
    }
}
