using System.Windows.Forms;
using Windows.Win32;
using RoyalApps.Community.Rdp.WinForms.Interfaces;

namespace RoyalApps.Community.Rdp.WinForms.Controls;

internal static class MessageFilter
{
    public static bool Filter(IRdpClient control, ref Message m)
    {
        // processing base.WndProc(m) when WM_DESTROY is received while the control has already been disposed
        if (m.Msg == PInvoke.WM_DESTROY && control is {IsDisposed: true, IsHandleCreated: false})
            return true;

        if (control.DisableClickDetection ||
            (m.Msg != PInvoke.WM_MOUSEACTIVATE &&
             m.Msg != PInvoke.WM_LBUTTONDOWN &&
             m.Msg != PInvoke.WM_MDIACTIVATE)) 
            return false;
        
        // notify host application that the RDP client area has been clicked
        control.RaiseClientAreaClicked();
        if (!control.ContainsFocus)
            control.Focus();
        return true;
    }
}