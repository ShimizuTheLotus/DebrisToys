using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DebrisToys.Win32
{
    internal class Mouse
    {
        public const int WH_MOUSE_LL = 14;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_MBUTTONDOWN = 0x0207;

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);
    }
}
