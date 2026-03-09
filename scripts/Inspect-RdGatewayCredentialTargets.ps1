[CmdletBinding()]
param(
    [string]$Filter,
    [switch]$LaunchMstsc,
    [string]$BeforeSnapshotPath,
    [string]$AfterSnapshotPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if (-not ("CredManNative" -as [type])) {
    Add-Type -TypeDefinition @"
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class CredManNative
{
    [DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CredEnumerateW(string Filter, int Flags, out int Count, out IntPtr Credentials);

    [DllImport("Advapi32.dll", SetLastError = true)]
    private static extern void CredFree(IntPtr Buffer);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CREDENTIAL
    {
        public int Flags;
        public int Type;
        public string TargetName;
        public string Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public int CredentialBlobSize;
        public IntPtr CredentialBlob;
        public int Persist;
        public int AttributeCount;
        public IntPtr Attributes;
        public string TargetAlias;
        public string UserName;
    }

    public sealed class CredentialInfo
    {
        public CredentialInfo()
        {
            TargetName = string.Empty;
            UserName = string.Empty;
            Type = string.Empty;
            Persist = string.Empty;
            Comment = string.Empty;
            TargetAlias = string.Empty;
        }

        public string TargetName { get; set; }
        public string UserName { get; set; }
        public string Type { get; set; }
        public string Persist { get; set; }
        public string Comment { get; set; }
        public string TargetAlias { get; set; }
        public long LastWrittenUtcTicks { get; set; }
    }

    public static List<CredentialInfo> Enumerate()
    {
        List<CredentialInfo> results = new List<CredentialInfo>();
        int count;
        IntPtr credentials;
        if (!CredEnumerateW(null, 0, out count, out credentials))
        {
            int error = Marshal.GetLastWin32Error();
            if (error == 1168)
                return results;

            throw new InvalidOperationException("CredEnumerateW failed with error " + error + ".");
        }

        try
        {
            for (int i = 0; i < count; i++)
            {
                IntPtr credentialPointer = Marshal.ReadIntPtr(credentials, i * IntPtr.Size);
                CREDENTIAL credential = (CREDENTIAL)Marshal.PtrToStructure(credentialPointer, typeof(CREDENTIAL));
                CredentialInfo info = new CredentialInfo();
                info.TargetName = credential.TargetName ?? string.Empty;
                info.UserName = credential.UserName ?? string.Empty;
                info.Type = MapType(credential.Type);
                info.Persist = MapPersist(credential.Persist);
                info.Comment = credential.Comment ?? string.Empty;
                info.TargetAlias = credential.TargetAlias ?? string.Empty;
                info.LastWrittenUtcTicks = ToDateTimeUtcTicks(credential.LastWritten);
                results.Add(info);
            }
        }
        finally
        {
            CredFree(credentials);
        }

        return results;
    }

    private static long ToDateTimeUtcTicks(System.Runtime.InteropServices.ComTypes.FILETIME fileTime)
    {
        long high = ((long)fileTime.dwHighDateTime) << 32;
        long combined = high | (uint)fileTime.dwLowDateTime;
        return DateTime.FromFileTimeUtc(combined).Ticks;
    }

    private static string MapType(int type)
    {
        switch (type)
        {
            case 1:
                return "Generic";
            case 2:
                return "DomainPassword";
            case 3:
                return "DomainCertificate";
            case 4:
                return "DomainVisiblePassword";
            case 5:
                return "GenericCertificate";
            case 6:
                return "DomainExtended";
            default:
                return type.ToString();
        }
    }

    private static string MapPersist(int persist)
    {
        switch (persist)
        {
            case 1:
                return "Session";
            case 2:
                return "LocalMachine";
            case 3:
                return "Enterprise";
            default:
                return persist.ToString();
        }
    }
}
"@
}

function Get-CredentialEntries {
    $entries = [CredManNative]::Enumerate() |
        Sort-Object TargetName, Type, UserName

    if ([string]::IsNullOrWhiteSpace($Filter)) {
        return $entries
    }

    return $entries | Where-Object {
        $_.TargetName -like "*$Filter*" -or
        $_.UserName -like "*$Filter*" -or
        $_.TargetAlias -like "*$Filter*"
    }
}

function Save-Snapshot {
    param(
        [Parameter(Mandatory = $true)]
        [object[]]$Entries,

        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $directory = Split-Path -Path $Path -Parent
    if (-not [string]::IsNullOrWhiteSpace($directory)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }

    $Entries | ConvertTo-Json -Depth 4 | Set-Content -Path $Path -Encoding UTF8
}

function Get-DefaultSnapshotPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Label
    )

    $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
    return Join-Path $env:TEMP "royalapps-rdgateway-$Label-$timestamp.json"
}

function Format-Entry {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Entry
    )

    $userName = if ([string]::IsNullOrEmpty($Entry.UserName)) { "<empty>" } else { $Entry.UserName }
    return "{0} [{1}] User={2} Persist={3}" -f $Entry.TargetName, $Entry.Type, $userName, $Entry.Persist
}

function Compare-Entries {
    param(
        [Parameter(Mandatory = $true)]
        [object[]]$Before,

        [Parameter(Mandatory = $true)]
        [object[]]$After
    )

    $beforeMap = @{}
    foreach ($entry in $Before) {
        $key = "{0}|{1}" -f $entry.TargetName, $entry.Type
        $beforeMap[$key] = $entry
    }

    $afterMap = @{}
    foreach ($entry in $After) {
        $key = "{0}|{1}" -f $entry.TargetName, $entry.Type
        $afterMap[$key] = $entry
    }

    $added = New-Object System.Collections.Generic.List[object]
    $removed = New-Object System.Collections.Generic.List[object]
    $changed = New-Object System.Collections.Generic.List[object]

    foreach ($key in $afterMap.Keys) {
        if (-not $beforeMap.ContainsKey($key)) {
            $added.Add($afterMap[$key])
            continue
        }

        $beforeEntry = $beforeMap[$key]
        $afterEntry = $afterMap[$key]
        if ($beforeEntry.UserName -ne $afterEntry.UserName -or
            $beforeEntry.Persist -ne $afterEntry.Persist -or
            $beforeEntry.Comment -ne $afterEntry.Comment -or
            $beforeEntry.TargetAlias -ne $afterEntry.TargetAlias -or
            $beforeEntry.LastWrittenUtcTicks -ne $afterEntry.LastWrittenUtcTicks) {
            $changed.Add([pscustomobject]@{
                    Before = $beforeEntry
                    After  = $afterEntry
                })
        }
    }

    foreach ($key in $beforeMap.Keys) {
        if (-not $afterMap.ContainsKey($key)) {
            $removed.Add($beforeMap[$key])
        }
    }

    return [pscustomobject]@{
        Added   = $added
        Removed = $removed
        Changed = $changed
    }
}

$beforePath = if ([string]::IsNullOrWhiteSpace($BeforeSnapshotPath)) { Get-DefaultSnapshotPath "before" } else { $BeforeSnapshotPath }
$afterPath = if ([string]::IsNullOrWhiteSpace($AfterSnapshotPath)) { Get-DefaultSnapshotPath "after" } else { $AfterSnapshotPath }

$beforeEntries = Get-CredentialEntries
Save-Snapshot -Entries $beforeEntries -Path $beforePath

Write-Host "Saved before snapshot to $beforePath"
Write-Host ("Before snapshot entries: {0}" -f $beforeEntries.Count)

if ($LaunchMstsc) {
    Write-Host "Launching mstsc.exe..."
    Start-Process mstsc.exe | Out-Null
}

Write-Host ""
Write-Host "Next step:"
Write-Host "1. In mstsc.exe, configure the RD Gateway manually."
Write-Host "2. Save the gateway credentials if prompted."
Write-Host "3. Complete one successful connection, then close mstsc.exe."
Write-Host "4. Return here and press Enter."
[void](Read-Host "Press Enter after the manual mstsc.exe gateway login test")

$afterEntries = Get-CredentialEntries
Save-Snapshot -Entries $afterEntries -Path $afterPath

$diff = Compare-Entries -Before $beforeEntries -After $afterEntries

Write-Host ""
Write-Host "Saved after snapshot to $afterPath"
Write-Host ("After snapshot entries: {0}" -f $afterEntries.Count)
Write-Host ""

Write-Host "Added credentials:"
if ($diff.Added.Count -eq 0) {
    Write-Host "  <none>"
}
else {
    foreach ($entry in $diff.Added) {
        Write-Host ("  + {0}" -f (Format-Entry -Entry $entry))
    }
}

Write-Host ""
Write-Host "Changed credentials:"
if ($diff.Changed.Count -eq 0) {
    Write-Host "  <none>"
}
else {
    foreach ($change in $diff.Changed) {
        $beforeUserName = if ([string]::IsNullOrEmpty($change.Before.UserName)) { "<empty>" } else { $change.Before.UserName }
        $afterUserName = if ([string]::IsNullOrEmpty($change.After.UserName)) { "<empty>" } else { $change.After.UserName }
        Write-Host ("  * {0}" -f (Format-Entry -Entry $change.After))
        Write-Host ("    BeforeUser={0} AfterUser={1}" -f $beforeUserName, $afterUserName)
    }
}

Write-Host ""
Write-Host "Removed credentials:"
if ($diff.Removed.Count -eq 0) {
    Write-Host "  <none>"
}
else {
    foreach ($entry in $diff.Removed) {
        Write-Host ("  - {0}" -f (Format-Entry -Entry $entry))
    }
}

Write-Host ""
Write-Host "Tip: the likely RD Gateway CredMan target is one of the added or changed entries associated with the gateway host."
