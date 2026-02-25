using System;
using System.Windows.Forms;
using Windows.Win32;
using RoyalApps.Community.Rdp.WinForms.Interfaces;

namespace RoyalApps.Community.Rdp.WinForms.Controls;

internal static class MessageFilter
{
    [ThreadStatic]
    private static bool _isProcessingClickMessage;

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

        if (_isProcessingClickMessage)
            return true;

        // Avoid synchronous callbacks/focus changes in hook/WndProc paths, because
        // they can trigger re-entrant COM calls on the UI STA and freeze the app.
        if (control is Control winFormsControl && control.IsHandleCreated && !control.IsDisposed)
        {
            _isProcessingClickMessage = true;
            try
            {
                winFormsControl.BeginInvoke((MethodInvoker)(() =>
                {
                    try
                    {
                        if (control is {IsDisposed: true})
                            return;

                        control.RaiseClientAreaClicked();

                        if (!control.ContainsFocus)
                            control.Focus();
                    }
                    finally
                    {
                        _isProcessingClickMessage = false;
                    }
                }));
            }
            catch
            {
                _isProcessingClickMessage = false;
            }
        }
        else
        {
            control.RaiseClientAreaClicked();
            if (!control.ContainsFocus)
                control.Focus();
        }

        return true;
    }
}
