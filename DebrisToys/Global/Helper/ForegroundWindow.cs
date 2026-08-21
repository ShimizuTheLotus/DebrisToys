using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Accessibility;
using static Windows.Win32.PInvoke;

namespace DebrisToys.Global.Helper
{
    public class ForegroundWindow
    {
        public nint Handle => _hWnd;

        // Keep a strong reference to the delegate to prevent GC.
        private WINEVENTPROC _winEventProcDelegate;

        // Keep the handle in case you want to UnhookWinEvent later.
        private HWINEVENTHOOK _hook;

        public string ClassName
        {
            get
            {
                lock (_hwndLock)
                {
                    if (_hWnd == 0)
                        return string.Empty;
                    var sb = new StringBuilder(512);
                    GetClassNameW((IntPtr)_hWnd, sb, sb.Capacity);
                    return sb.ToString();
                }
            }
        }

        public string Title
        {
            get
            {
                lock (_hwndLock)
                {
                    if (_hWnd == 0)
                        return string.Empty;
                    var sb = new StringBuilder(512);
                    GetWindowTextW((IntPtr)_hWnd, sb, sb.Capacity);
                    return sb.ToString();
                }
            }
        }

        public uint ProcessId => GetProcessIdCore();

        public string ProcessName => Process.GetProcessById((int)ProcessId).ProcessName;

        public System.Drawing.Rectangle? VisibleRect
        {
            get
            {
                lock (_hwndLock)
                {
                    if (_hWnd == 0)
                        return System.Drawing.Rectangle.Empty;

                    RECT rect = default;
                    int hr = DwmGetWindowAttribute(
                        (IntPtr)_hWnd,
                        DWMWA_EXTENDED_FRAME_BOUNDS,
                        ref rect,
                        Marshal.SizeOf(typeof(RECT))
                    );

                    if (hr == 0)
                    {
                        return new System.Drawing.Rectangle(
                            rect.left, rect.top,
                            rect.right - rect.left,
                            rect.bottom - rect.top
                        );
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public HashSet<Action> CallbackActions = [];

        private readonly object _hwndLock = new();
        private bool _ismonitoring = false;

        public ForegroundWindow()
        {
            _winEventProcDelegate = new WINEVENTPROC(WinEventProc);
        }

        public static ForegroundWindow Current
        {
            get
            {
                return LazyInitializer.Instance;
            }
        }

        private static class LazyInitializer
        {
            static LazyInitializer()
            {
            }
            public static readonly ForegroundWindow Instance = new();
        }

        public void StartMonitoring()
        {
            if (_ismonitoring)
                return;
            _ismonitoring = true;
            _hook = SetWinEventHook(
                EVENT_SYSTEM_FOREGROUND,
                EVENT_SYSTEM_FOREGROUND,
                HMODULE.Null,
                _winEventProcDelegate,
                0,
                0,
                WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS
            );
        }

        // Callback
        private void WinEventProc(
            HWINEVENTHOOK hWinEventHook,
            uint eventType,
            HWND hwnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime)
        {
            var currentHwnd = GetForegroundWindow();
            lock (_hwndLock)
            {
                _hWnd = currentHwnd;
            }
            foreach (var action in CallbackActions)
            {
                action.Invoke();
            }
        }


        #region Win32
        private nint _hWnd;

        private unsafe uint GetProcessIdCore()
        {
            lock (_hwndLock)
            {
                if (_hWnd == 0)
                    return 0;
                GetWindowThreadProcessId((IntPtr)_hWnd, out uint pid);
                return pid;
            }
        }


        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextW(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetClassNameW(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmGetWindowAttribute(
            IntPtr hwnd,
            int dwAttribute,
            ref RECT pvAttribute,
            int cbAttribute
        );

        private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;
        #endregion
    }
}
