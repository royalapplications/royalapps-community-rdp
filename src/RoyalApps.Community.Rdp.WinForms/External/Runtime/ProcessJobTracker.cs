using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.JobObjects;

namespace RoyalApps.Community.Rdp.WinForms.External.Runtime;

internal sealed class ProcessJobTracker : IDisposable
{
    private readonly SafeFileHandle _jobHandle;

    public ProcessJobTracker(string namePrefix)
    {
        var jobName = namePrefix + nameof(ProcessJobTracker) + Process.GetCurrentProcess().Id;
        _jobHandle = PInvoke.CreateJobObject(null, jobName);

        if (_jobHandle.IsInvalid)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        var extendedInformation = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            BasicLimitInformation = new JOBOBJECT_BASIC_LIMIT_INFORMATION
            {
                LimitFlags = JOB_OBJECT_LIMIT.JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE
            }
        };

        unsafe
        {
            if (!PInvoke.SetInformationJobObject(
                    new HANDLE(_jobHandle.DangerousGetHandle()),
                    JOBOBJECTINFOCLASS.JobObjectExtendedLimitInformation,
                    &extendedInformation,
                    (uint)Marshal.SizeOf<JOBOBJECT_EXTENDED_LIMIT_INFORMATION>()))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }

    public void AddProcess(Process process)
    {
        ArgumentNullException.ThrowIfNull(process);

        if (!PInvoke.AssignProcessToJobObject(_jobHandle, process.SafeHandle) && !process.HasExited)
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    public void Dispose()
    {
        _jobHandle.Dispose();
    }
}
