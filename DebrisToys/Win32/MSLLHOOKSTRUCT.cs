using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DebrisToys.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}
