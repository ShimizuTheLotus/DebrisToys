using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DebrisToys.Toys.NoTaskbar
{
    public static class TaskbarInteraction
    {
        public static bool IsHideLoop = false;
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string? lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public struct Win32API_RECT
        {
            public int Left, Top, Right, Bottom;

            public Win32API_RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public Win32API_RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

            public int X
            {
                get { return Left; }
                set { Right -= (Left - value); Left = value; }
            }

            public int Y
            {
                get { return Top; }
                set { Bottom -= (Top - value); Top = value; }
            }

            public int Height
            {
                get { return Bottom - Top; }
                set { Bottom = value + Top; }
            }

            public int Width
            {
                get { return Right - Left; }
                set { Right = value + Left; }
            }

            public System.Drawing.Point Location
            {
                get { return new System.Drawing.Point(Left, Top); }
                set { X = value.X; Y = value.Y; }
            }

            public System.Drawing.Size Size
            {
                get { return new System.Drawing.Size(Width, Height); }
                set { Width = value.Width; Height = value.Height; }
            }

            public static implicit operator System.Drawing.Rectangle(Win32API_RECT r)
            {
                return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
            }

            public static implicit operator Win32API_RECT(System.Drawing.Rectangle r)
            {
                return new Win32API_RECT(r);
            }

            public static bool operator ==(Win32API_RECT r1, Win32API_RECT r2)
            {
                return r1.Equals(r2);
            }

            public static bool operator !=(Win32API_RECT r1, Win32API_RECT r2)
            {
                return !r1.Equals(r2);
            }

            public bool Equals(Win32API_RECT r)
            {
                return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            }

            public override bool Equals(object? obj)
            {
                if (obj == null) return false;
                if (obj is Win32API_RECT)
                    return Equals((Win32API_RECT)obj);
                else if (obj is System.Drawing.Rectangle)
                    return Equals(new Win32API_RECT((System.Drawing.Rectangle)obj));
                return false;
            }

            public override int GetHashCode()
            {
                return ((System.Drawing.Rectangle)this).GetHashCode();
            }

            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct Win32API_APPBARDATA
        {
            public int cbSize; // initialize this field using: Marshal.SizeOf(typeof(APPBARDATA));
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public Win32API_RECT rc;
            public int lParam;
        }
        public enum Win32API_AppBarStates
        {
            AlwaysOnTop = 0x00,
            AutoHide = 0x01
        }
        public enum Win32API_AppBarMessages
        {
            New = 0x00,
            Remove = 0x01,
            QueryPos = 0x02,
            SetPos = 0x03,
            GetState = 0x04,
            GetTaskBarPos = 0x05,
            Activate = 0x06,
            GetAutoHideBar = 0x07,
            SetAutoHideBar = 0x08,
            WindowPosChanged = 0x09,
            SetState = 0x0a
        }
        [DllImport("shell32.dll")]
        public static extern UInt32 SHAppBarMessage(UInt32 dwMessage, ref Win32API_APPBARDATA pData);
        public static void Win32API_SetTaskbarState(Win32API_AppBarStates option)
        {
            Win32API_APPBARDATA msgData = new Win32API_APPBARDATA();
            msgData.cbSize = Marshal.SizeOf(msgData);
            msgData.hWnd = FindWindow("System_TrayWnd", null);
            msgData.lParam = (int)option;
            SHAppBarMessage((UInt32)Win32API_AppBarMessages.SetState, ref msgData);
        }

        // Taskbar state consts

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;


        // Taskbar state methods

        public static async void HideTaskbar(NoTaskbarConfig config)
        {
            var handle = FindWindow("Shell_TrayWnd", null);
            if (handle != IntPtr.Zero)
            {
                ShowWindow(handle, SW_HIDE);
                Win32API_SetTaskbarState(Win32API_AppBarStates.AutoHide);
                ShowWindow(handle, SW_HIDE);
                IsHideLoop = true;
                StartHideBarLoop();
                config.IsEnabled = true;
            }
        }

        public static void ShowTaskbar(NoTaskbarConfig config)
        {
            var handle = FindWindow("Shell_TrayWnd", null);
            if (handle != IntPtr.Zero)
            {
                Win32API_SetTaskbarState(Win32API_AppBarStates.AlwaysOnTop);
                ShowWindow(handle, SW_SHOW);
                IsHideLoop = false;
                config.IsEnabled = false;
            }
        }

        public static async void StartHideBarLoop()
        {
            while (IsHideLoop)
            {
                var handle = FindWindow("Shell_TrayWnd", null);
                ShowWindow(handle, SW_HIDE);
                await Task.Delay(100);
            }
        }
    }

}
