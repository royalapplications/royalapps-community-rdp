using System.Linq;
using System.Text;

namespace RoyalApps.Community.Rdp.WinForms.External.Credentials;

internal sealed class StoredCredentialSnapshot
{
    public required CredentialType Type { get; init; }

    public required string TargetName { get; init; }

    public string? Comment { get; init; }

    public string? TargetAlias { get; init; }

    public required CredentialPersistence Persistence { get; init; }

    public required byte[] CredentialBlob { get; init; }

    public string? UserName { get; init; }

    public bool MatchesPassword(string password)
    {
        var bytes = Encoding.Unicode.GetBytes(password);
        return CredentialBlob.SequenceEqual(bytes);
    }
}
