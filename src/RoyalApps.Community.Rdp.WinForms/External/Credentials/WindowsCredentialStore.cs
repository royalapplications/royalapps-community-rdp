namespace RoyalApps.Community.Rdp.WinForms.External.Credentials;

internal sealed class WindowsCredentialStore : ICredentialStore
{
    public static WindowsCredentialStore Instance { get; } = new();

    private WindowsCredentialStore()
    {
    }

    public bool TryRead(string targetName, out StoredCredentialSnapshot? credential)
    {
        return WindowsCredentialManager.TryRead(targetName, out credential);
    }

    public void Write(StoredCredentialSnapshot credential)
    {
        WindowsCredentialManager.Write(credential);
    }

    public void Write(CredentialType type, string targetName, string userName, string password)
    {
        WindowsCredentialManager.Write(type, targetName, userName, password);
    }

    public void Delete(string targetName, CredentialType type)
    {
        WindowsCredentialManager.Delete(targetName, type);
    }
}
