using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DebrisToys.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
