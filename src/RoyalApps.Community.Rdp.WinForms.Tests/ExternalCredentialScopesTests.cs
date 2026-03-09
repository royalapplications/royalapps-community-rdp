using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.External.Credentials;
using Xunit;

namespace RoyalApps.Community.Rdp.WinForms.Tests;

public class ExternalCredentialScopesTests
{
    [Fact]
    public void Create_StagesServerCredentialOnly_WhenGatewayIsNotConfigured()
    {
        var configuration = CreateBaseConfiguration();
        var store = new FakeCredentialStore();

        using var scopes = ExternalCredentialScopes.Create(configuration, NullLogger.Instance, store);

        Assert.NotNull(scopes);
        Assert.Collection(
            store.Operations,
            operation =>
            {
                Assert.Equal("WritePassword", operation.Kind);
                Assert.Equal("TERMSRV/rdp.example.test", operation.TargetName);
                Assert.Equal(CredentialType.Generic, operation.CredentialType);
                Assert.Equal(@"LAB\alice", operation.UserName);
            });

        Assert.Equal("TERMSRV/rdp.example.test", store.StoredCredentials.Keys.Single());
    }

    [Fact]
    public void Create_StagesGatewayCredentialOnly_WhenServerCredentialIsMissing()
    {
        var configuration = new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.External
        };
        configuration.Gateway.GatewayHostname = "48.209.10.183";
        configuration.Gateway.GatewayUsername = "radmin";
        configuration.Gateway.GatewayPassword = new SensitiveString("gateway-secret");

        var store = new FakeCredentialStore();

        using var scopes = ExternalCredentialScopes.Create(configuration, NullLogger.Instance, store);

        Assert.NotNull(scopes);
        Assert.Collection(
            store.Operations,
            operation =>
            {
                Assert.Equal("WritePassword", operation.Kind);
                Assert.Equal("48.209.10.183", operation.TargetName);
                Assert.Equal(CredentialType.DomainPassword, operation.CredentialType);
                Assert.Equal("radmin", operation.UserName);
            });
    }

    [Fact]
    public void Create_StagesServerAndGatewayCredentials_AndDeletesBothOnDispose()
    {
        var configuration = CreateBaseConfiguration();
        configuration.Gateway.GatewayHostname = "48.209.10.183";
        configuration.Gateway.GatewayDomain = "LAB";
        configuration.Gateway.GatewayUsername = "gateway-user";
        configuration.Gateway.GatewayPassword = new SensitiveString("gateway-secret");

        var store = new FakeCredentialStore();

        using (ExternalCredentialScopes.Create(configuration, NullLogger.Instance, store))
        {
            Assert.Equal(2, store.StoredCredentials.Count);
        }

        Assert.Equal(
            new[]
            {
                "WritePassword:TERMSRV/rdp.example.test",
                "WritePassword:48.209.10.183",
                "Delete:48.209.10.183",
                "Delete:TERMSRV/rdp.example.test"
            },
            store.Operations.Select(operation => $"{operation.Kind}:{operation.TargetName}"));
    }

    [Fact]
    public void Create_DoesNotRewriteMatchingExistingCredential()
    {
        var configuration = CreateBaseConfiguration();
        var store = new FakeCredentialStore();
        store.StoredCredentials["TERMSRV/rdp.example.test"] = CreateStoredCredential(
            CredentialType.Generic,
            "TERMSRV/rdp.example.test",
            @"LAB\alice",
            "secret");

        using var scopes = ExternalCredentialScopes.Create(configuration, NullLogger.Instance, store);

        Assert.NotNull(scopes);
        Assert.Empty(store.Operations);
    }

    [Fact]
    public void Dispose_RestoresOriginalCredentials_WhenTemporaryCredentialsOverwriteExistingValues()
    {
        var configuration = CreateBaseConfiguration();
        configuration.Gateway.GatewayHostname = "48.209.10.183";
        configuration.Gateway.GatewayDomain = "LAB";
        configuration.Gateway.GatewayUsername = "gateway-user";
        configuration.Gateway.GatewayPassword = new SensitiveString("gateway-secret");

        var store = new FakeCredentialStore();
        store.StoredCredentials["TERMSRV/rdp.example.test"] = CreateStoredCredential(
            CredentialType.DomainPassword,
            "TERMSRV/rdp.example.test",
            @"LAB\alice",
            "old-secret");
        store.StoredCredentials["48.209.10.183"] = CreateStoredCredential(
            CredentialType.DomainPassword,
            "48.209.10.183",
            @"LAB\gateway-user",
            "old-gateway-secret");

        using (ExternalCredentialScopes.Create(configuration, NullLogger.Instance, store))
        {
            Assert.Equal(@"LAB\alice", store.StoredCredentials["TERMSRV/rdp.example.test"].UserName);
            Assert.Equal(@"LAB\gateway-user", store.StoredCredentials["48.209.10.183"].UserName);
        }

        Assert.Equal(
            new[]
            {
                "WritePassword:TERMSRV/rdp.example.test",
                "WritePassword:48.209.10.183",
                "WriteSnapshot:48.209.10.183",
                "WriteSnapshot:TERMSRV/rdp.example.test"
            },
            store.Operations.Select(operation => $"{operation.Kind}:{operation.TargetName}"));

        Assert.True(store.StoredCredentials["TERMSRV/rdp.example.test"].MatchesPassword("old-secret"));
        Assert.True(store.StoredCredentials["48.209.10.183"].MatchesPassword("old-gateway-secret"));
    }

    private static RdpClientConfiguration CreateBaseConfiguration()
    {
        return new RdpClientConfiguration
        {
            Server = "rdp.example.test",
            SessionMode = RdpSessionMode.External,
            Credentials =
            {
                Domain = "LAB",
                Username = "alice",
                Password = new SensitiveString("secret")
            }
        };
    }

    private static StoredCredentialSnapshot CreateStoredCredential(CredentialType credentialType, string targetName, string userName, string password)
    {
        return new StoredCredentialSnapshot
        {
            Type = credentialType,
            TargetName = targetName,
            Persistence = CredentialPersistence.Session,
            CredentialBlob = Encoding.Unicode.GetBytes(password),
            UserName = userName
        };
    }

    private sealed class FakeCredentialStore : ICredentialStore
    {
        public Dictionary<string, StoredCredentialSnapshot> StoredCredentials { get; } = new(StringComparer.OrdinalIgnoreCase);

        public List<CredentialOperation> Operations { get; } = [];

        public bool TryRead(string targetName, out StoredCredentialSnapshot? credential)
        {
            if (StoredCredentials.TryGetValue(targetName, out var storedCredential))
            {
                credential = Clone(storedCredential);
                return true;
            }

            credential = null;
            return false;
        }

        public void Write(StoredCredentialSnapshot credential)
        {
            Operations.Add(new CredentialOperation("WriteSnapshot", credential.TargetName, credential.Type, credential.UserName));
            StoredCredentials[credential.TargetName] = Clone(credential);
        }

        public void Write(CredentialType type, string targetName, string userName, string password)
        {
            Operations.Add(new CredentialOperation("WritePassword", targetName, type, userName));
            StoredCredentials[targetName] = new StoredCredentialSnapshot
            {
                Type = type,
                TargetName = targetName,
                Persistence = CredentialPersistence.Session,
                CredentialBlob = Encoding.Unicode.GetBytes(password),
                UserName = userName
            };
        }

        public void Delete(string targetName, CredentialType type)
        {
            Operations.Add(new CredentialOperation("Delete", targetName, type, null));
            StoredCredentials.Remove(targetName);
        }

        private static StoredCredentialSnapshot Clone(StoredCredentialSnapshot credential)
        {
            return new StoredCredentialSnapshot
            {
                Type = credential.Type,
                TargetName = credential.TargetName,
                Comment = credential.Comment,
                TargetAlias = credential.TargetAlias,
                Persistence = credential.Persistence,
                CredentialBlob = credential.CredentialBlob.ToArray(),
                UserName = credential.UserName
            };
        }
    }

    private sealed record CredentialOperation(string Kind, string TargetName, CredentialType CredentialType, string? UserName);
}
