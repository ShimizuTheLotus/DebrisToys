using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DebrisToys.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
