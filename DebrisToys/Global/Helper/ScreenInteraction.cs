using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using WinRT.Interop;

namespace DebrisToys.Global.Helper
{
    public class ScreenInteraction
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;  // Full monitor
            public RECT rcWork;     // Monitor without taskbar
            public uint dwFlags;
        }

        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        // Get current work area pixel size
        public static (int width, int height) GetMonitorWorkArea()
        {
            IntPtr hwnd = WindowNative.GetWindowHandle(App.TrayWindow);
            IntPtr hMonitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            MONITORINFO monitorInfo = new MONITORINFO();
            monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            bool success = GetMonitorInfo(hMonitor, ref monitorInfo);
            if (!success)
            {
                return (0, 0);
            }

            int width = monitorInfo.rcWork.Right - monitorInfo.rcWork.Left;
            int height = monitorInfo.rcWork.Bottom - monitorInfo.rcWork.Top;

            return (width, height);
        }
    }
}
