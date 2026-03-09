namespace RoyalApps.Community.Rdp.WinForms.External.Credentials;

internal interface ICredentialStore
{
    bool TryRead(string targetName, out StoredCredentialSnapshot? credential);

    void Write(StoredCredentialSnapshot credential);

    void Write(CredentialType type, string targetName, string userName, string password);

    void Delete(string targetName, CredentialType type);
}
