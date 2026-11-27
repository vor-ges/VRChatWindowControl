using System;
using System.Runtime.InteropServices;

namespace VRChatWindowControl;

internal static class NativeMethods
{
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags
    );

    [DllImport("user32.dll")]
    internal static extern bool ShowWindowAsync(
        IntPtr hWnd,
        int nCmdShow
    );
}
