using DebrisToys.ToysManager;
using DebrisToys.ToysManager.Base;
using DebrisToys.ToysManager.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DebrisToys.Toys.IMEBlocker
{
    public class IMEBlocker : ToyBase
    {
        private IntPtr _foregroundHook = IntPtr.Zero;
        private Win32.WinEventDelegate? _foregroundDelegate;
        private readonly object _hookSync = new();

        private IntPtr _englishHkl = IntPtr.Zero;

        // Stat
        private bool _isEnabled = false;

        // IME settings
        private bool _originalPerAppSetting = false;

        public static IMEBlocker Current => LazyInitializer.Instance;
        private static class LazyInitializer
        {
            public static readonly IMEBlocker Instance = new();
        }

        public void Start()
        {
            if (_isEnabled)
                return;

            // Save original settings.
            _originalPerAppSetting = Win32.IsPerAppInputMethodEnabled();

            // Enable if not enabled
            if (!_originalPerAppSetting)
            {
                Win32.SetPerAppInputMethodEnabled(true);
            }

            // Load English layout
            _englishHkl = Win32.LoadKeyboardLayout("00000409", Win32.KLF_ACTIVATE);

            // Hook
            StartWatcher();

            _isEnabled = true;
        }

        public void Stop()
        {
            if (!_isEnabled)
                return;

            // Stop hook
            StopWatcher();

            // Restore IME switching settings
            if (!_originalPerAppSetting)
            {
                Win32.SetPerAppInputMethodEnabled(false);
            }

            _isEnabled = false;
        }

        private void StartWatcher()
        {
            lock (_hookSync)
            {
                if (_foregroundHook != IntPtr.Zero)
                    return;

                _foregroundDelegate = ForegroundWinEventProc;
                _foregroundHook = Win32.SetWinEventHook(
                    Win32.EVENT_SYSTEM_FOREGROUND,
                    Win32.EVENT_SYSTEM_FOREGROUND,
                    IntPtr.Zero,
                    _foregroundDelegate,
                    0, 0,
                    Win32.WINEVENT_OUTOFCONTEXT  // Remove SKIPOWNPROCESS
                );
            }
        }

        private void StopWatcher()
        {
            lock (_hookSync)
            {
                if (_foregroundHook != IntPtr.Zero)
                {
                    Win32.UnhookWinEvent(_foregroundHook);
                    _foregroundHook = IntPtr.Zero;
                }
                _foregroundDelegate = null;
            }
        }

        private void ForegroundWinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd,
            int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // Reget actual foreground window
            IntPtr actualHwnd = Win32.GetForegroundWindow();
            if (actualHwnd == IntPtr.Zero)
                return;

            HandleForegroundChange(actualHwnd);
        }

        private bool IsBlockedProcess(uint processId)
        {
            try
            {
                var proc = Process.GetProcessById((int)processId);
                var exe = proc.ProcessName + ".exe";
                bool result = IMEBlockerConfig.Current.TargetAppList.Any(x => x.IsEnabled && x.AppName.ToLower() == exe.ToLower());
                return result;
            }
            catch
            {
                return false;
            }
        }

        private void HandleForegroundChange(IntPtr hwnd)
        {
            try
            {
                lock (_hookSync)
                {
                    if (!Win32.IsWindow(hwnd))
                        return;
                    uint threadId = Win32.GetWindowThreadProcessId(hwnd, out uint processId);
                    bool isBlocked = IsBlockedProcess(processId);

                    // Enter blocked app
                    if (isBlocked)
                    {
                        RequestEnglish(hwnd);
                    }
                }
            }
            catch
            {
            }
        }

        private void RequestEnglish(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero || _englishHkl == IntPtr.Zero)
                return;

            try
            {
                // Send request
                Win32.SendMessage(hwnd, Win32.WM_INPUTLANGCHANGEREQUEST, (IntPtr)1, _englishHkl);
            }
            catch
            {
            }
        }

        public override void AutoStart()
        {
            IMEBlockerConfig.Current.ApplyConfig();
            if (IMEBlockerConfig.Current.IsEnabled)
            {
                IMEBlocker.Current.Start();
            }
        }

        public override void RecoverStatus()
        {
            IMEBlocker.Current.Stop();
        }
    }
}
