using System;

namespace RoyalApps.Community.Rdp.WinForms.External.Files;

internal interface ITemporaryRdpFileLease : IDisposable
{
    string FilePath { get; }
}
