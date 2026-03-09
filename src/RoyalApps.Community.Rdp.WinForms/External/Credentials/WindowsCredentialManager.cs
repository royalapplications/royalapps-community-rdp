using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Security.Credentials;

namespace RoyalApps.Community.Rdp.WinForms.External.Credentials;

internal static class WindowsCredentialManager
{
    public static bool TryRead(string targetName, out StoredCredentialSnapshot? credential)
    {
        foreach (var type in new[] { CredentialType.Generic, CredentialType.DomainPassword })
        {
            unsafe
            {
                fixed (char* targetNamePointer = targetName)
                {
                    CREDENTIALW* credentialPointer;
                    if (!PInvoke.CredRead(new PCWSTR(targetNamePointer), ToNative(type), 0, &credentialPointer))
                        continue;

                    try
                    {
                        credential = CreateSnapshot(credentialPointer);
                        return true;
                    }
                    finally
                    {
                        PInvoke.CredFree(credentialPointer);
                    }
                }
            }
        }

        credential = null;
        return false;
    }

    public static void Write(StoredCredentialSnapshot credential)
    {
        ArgumentNullException.ThrowIfNull(credential);
        WriteInternal(
            credential.Type,
            credential.TargetName,
            credential.UserName,
            credential.CredentialBlob,
            credential.Comment,
            credential.TargetAlias,
            credential.Persistence);
    }

    public static void Write(CredentialType type, string targetName, string userName, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetName);
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);

        var bytes = Encoding.Unicode.GetBytes(password);
        WriteInternal(type, targetName, userName, bytes, null, null, CredentialPersistence.Session);
    }

    public static void Delete(string targetName, CredentialType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetName);

        unsafe
        {
            fixed (char* targetNamePointer = targetName)
            {
                if (!PInvoke.CredDelete(new PCWSTR(targetNamePointer), ToNative(type)))
                {
                    var error = Marshal.GetLastWin32Error();
                    if (error != ErrorNotFound)
                        throw new Win32Exception(error);
                }
            }
        }
    }

    private static unsafe StoredCredentialSnapshot CreateSnapshot(CREDENTIALW* credentialPointer)
    {
        var blob = credentialPointer->CredentialBlobSize == 0
            ? []
            : CopyBytes((IntPtr)credentialPointer->CredentialBlob, checked((int)credentialPointer->CredentialBlobSize));

        return new StoredCredentialSnapshot
        {
            Type = FromNative(credentialPointer->Type),
            TargetName = credentialPointer->TargetName.ToString() ?? string.Empty,
            Comment = credentialPointer->Comment.ToString(),
            TargetAlias = credentialPointer->TargetAlias.ToString(),
            Persistence = FromNative(credentialPointer->Persist),
            CredentialBlob = blob,
            UserName = credentialPointer->UserName.ToString()
        };
    }

    private static void WriteInternal(
        CredentialType type,
        string targetName,
        string? userName,
        byte[] credentialBlob,
        string? comment,
        string? targetAlias,
        CredentialPersistence persistence)
    {
        unsafe
        {
            fixed (char* targetNamePointer = targetName)
            fixed (char* userNamePointer = userName)
            fixed (char* commentPointer = comment)
            fixed (char* targetAliasPointer = targetAlias)
            fixed (byte* credentialBlobPointer = credentialBlob)
            {
                var credential = new CREDENTIALW
                {
                    Type = ToNative(type),
                    TargetName = targetNamePointer,
                    Comment = commentPointer,
                    CredentialBlobSize = (uint)credentialBlob.Length,
                    CredentialBlob = credentialBlobPointer,
                    Persist = ToNative(persistence),
                    AttributeCount = 0,
                    Attributes = null,
                    TargetAlias = targetAliasPointer,
                    UserName = userNamePointer
                };

                if (!PInvoke.CredWrite(credential, 0))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }

    private static byte[] CopyBytes(IntPtr source, int length)
    {
        var bytes = new byte[length];
        Marshal.Copy(source, bytes, 0, length);
        return bytes;
    }

    private static CRED_TYPE ToNative(CredentialType type)
    {
        return type switch
        {
            CredentialType.DomainPassword => CRED_TYPE.CRED_TYPE_DOMAIN_PASSWORD,
            _ => CRED_TYPE.CRED_TYPE_GENERIC
        };
    }

    private static CredentialType FromNative(CRED_TYPE type)
    {
        return type switch
        {
            CRED_TYPE.CRED_TYPE_DOMAIN_PASSWORD => CredentialType.DomainPassword,
            _ => CredentialType.Generic
        };
    }

    private static CRED_PERSIST ToNative(CredentialPersistence persistence)
    {
        return persistence switch
        {
            CredentialPersistence.LocalMachine => CRED_PERSIST.CRED_PERSIST_LOCAL_MACHINE,
            CredentialPersistence.Enterprise => CRED_PERSIST.CRED_PERSIST_ENTERPRISE,
            _ => CRED_PERSIST.CRED_PERSIST_SESSION
        };
    }

    private static CredentialPersistence FromNative(CRED_PERSIST persistence)
    {
        return persistence switch
        {
            CRED_PERSIST.CRED_PERSIST_LOCAL_MACHINE => CredentialPersistence.LocalMachine,
            CRED_PERSIST.CRED_PERSIST_ENTERPRISE => CredentialPersistence.Enterprise,
            _ => CredentialPersistence.Session
        };
    }

    private const int ErrorNotFound = 1168;
}

internal enum CredentialType : uint
{
    Generic = 1,
    DomainPassword = 2
}

internal enum CredentialPersistence : uint
{
    Session = 1,
    LocalMachine = 2,
    Enterprise = 3
}
