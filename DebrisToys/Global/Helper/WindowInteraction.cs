using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DebrisToys.Global.Helper
{
    public static class WindowInteraction
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        public static void SetForegroundWindowAndSetFocus(Window? window)
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            SetForegroundWindow(hWnd);
            SetFocus(hWnd);
        }
    }
}
