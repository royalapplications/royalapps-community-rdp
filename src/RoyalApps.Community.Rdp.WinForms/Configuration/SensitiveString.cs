namespace RoyalApps.Community.Rdp.WinForms.Configuration;

/// <summary>
/// Represents a sensitive string and prevents accidental leaking.
/// </summary>
public class SensitiveString
{
    private string? _value;

    /// <summary>
    /// Creates a sensitive string instance.
    /// </summary>
    /// <param name="value"></param>
    public SensitiveString(string? value)
    {
        _value = value;
    }

    /// <summary>
    /// Sets the sensitive string value.
    /// </summary>
    /// <param name="value">Sensitive string.</param>
    public void SetValue(string value) => _value = value;
    
    /// <summary>
    /// Retrieves the sensitive string value.
    /// </summary>
    /// <returns>Sensitive string.</returns>
    public string? GetValue() => _value;

    /// <summary>
    /// Overrides ToString()
    /// </summary>
    /// <returns>Returns nothing useful.</returns>
    public override string ToString() => "****";
}