using System;
using System.Diagnostics;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace RoyalApps.Community.Rdp.WinForms.External.Runtime;

internal static class ExternalSessionWindowTracker
{
    public static unsafe bool TryActivateWindow(int processId, out nint windowHandle)
    {
        if (processId <= 0)
        {
            windowHandle = IntPtr.Zero;
            return false;
        }

        var hwnd = FindWindow(processId);
        if (hwnd == HWND.Null)
        {
            windowHandle = IntPtr.Zero;
            return false;
        }

        if (PInvoke.IsIconic(hwnd))
            PInvoke.ShowWindow(hwnd, SHOW_WINDOW_CMD.SW_RESTORE);

        windowHandle = (nint)hwnd.Value;
        return PInvoke.SetForegroundWindow(hwnd);
    }

    public static bool TryActivateWindow(Process process, out nint windowHandle)
    {
        ArgumentNullException.ThrowIfNull(process);

        if (process.HasExited)
        {
            windowHandle = IntPtr.Zero;
            return false;
        }

        process.Refresh();
        return TryActivateWindow(process.Id, out windowHandle);
    }

    public static unsafe bool TryGetWindowHandle(int processId, out nint windowHandle)
    {
        if (processId <= 0)
        {
            windowHandle = IntPtr.Zero;
            return false;
        }

        var hwnd = FindWindow(processId);
        windowHandle = (nint)hwnd.Value;
        return hwnd != HWND.Null;
    }

    public static bool TryGetWindowHandle(Process process, out nint windowHandle)
    {
        ArgumentNullException.ThrowIfNull(process);

        if (process.HasExited)
        {
            windowHandle = IntPtr.Zero;
            return false;
        }

        process.Refresh();
        return TryGetWindowHandle(process.Id, out windowHandle);
    }

    private static HWND FindWindow(int processId)
    {
        HWND foundWindow = HWND.Null;

        PInvoke.EnumWindows(
            (hwnd, _) =>
            {
                if (!PInvoke.IsWindowVisible(hwnd))
                    return true;

                PInvoke.GetWindowThreadProcessId(hwnd, out var ownerProcessId);
                if (ownerProcessId != processId)
                    return true;

                foundWindow = hwnd;
                return false;
            },
            IntPtr.Zero);

        return foundWindow;
    }
}
