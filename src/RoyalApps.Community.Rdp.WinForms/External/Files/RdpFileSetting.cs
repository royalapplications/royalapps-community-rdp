using System;

namespace RoyalApps.Community.Rdp.WinForms.External.Files;

/// <summary>
/// Represents a single line in an RDP file.
/// </summary>
public sealed class RdpFileSetting
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RdpFileSetting"/> class.
    /// </summary>
    public RdpFileSetting()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RdpFileSetting"/> class.
    /// </summary>
    /// <param name="name">The RDP property name.</param>
    /// <param name="type">The RDP property value type.</param>
    /// <param name="value">The serialized property value.</param>
    public RdpFileSetting(string name, RdpFileSettingType type, string value)
    {
        Name = name;
        Type = type;
        Value = value;
    }

    /// <summary>
    /// Gets or sets the RDP property name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the RDP value type.
    /// </summary>
    public RdpFileSettingType Type { get; set; } = RdpFileSettingType.String;

    /// <summary>
    /// Gets or sets the serialized property value.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Converts this setting to an RDP file line.
    /// </summary>
    /// <returns>The serialized line.</returns>
    public string ToRdpLine()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new InvalidOperationException("The RDP setting name cannot be empty.");

        return $"{Name}:{Type.ToToken()}:{Value}";
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Name)
            ? string.Empty
            : ToRdpLine();
    }
}
