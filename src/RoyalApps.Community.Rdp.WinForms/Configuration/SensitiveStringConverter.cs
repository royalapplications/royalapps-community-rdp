using System;
using System.ComponentModel;
using System.Globalization;

namespace RoyalApps.Community.Rdp.WinForms.Configuration;

internal class SensitiveStringConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value) => new SensitiveString(value?.ToString());

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType == typeof(SensitiveString);

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) => value is SensitiveString sensitiveString ? sensitiveString.GetValue() : null;
}
