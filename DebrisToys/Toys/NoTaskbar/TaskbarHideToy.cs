using DebrisToys.ToysManager.Base;
using ShimizuToolkit.HotkeyWinUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static DebrisToys.Toys.NoTaskbar.Win32;

namespace DebrisToys.Toys.NoTaskbar
{
    public class TaskbarHideToy : ToyBase
    {
        private const string _hotkey_Toggle = $"{nameof(TaskbarHideToy)}.Toggle";
        public TaskbarHideToy()
        {
            HotkeyNameList = [_hotkey_Toggle];
        }
        public static TaskbarHideToy Current => LazyInitializer.Instance;
        private static class LazyInitializer
        {
            public static readonly TaskbarHideToy Instance = new();
        }

        public async void HideTaskbar()
        {
            var handle = FindWindow("Shell_TrayWnd", null);
            if (handle != IntPtr.Zero)
            {
                ShowWindow(handle, SW_HIDE);
                Win32API_SetTaskbarState(Win32API_AppBarStates.AutoHide);
                ShowWindow(handle, SW_HIDE);
                IsHideLoop = true;
                StartHideBarLoop();
            }
        }

        public async void ShowTaskbar()
        {
            var handle = FindWindow("Shell_TrayWnd", null);
            if (handle != IntPtr.Zero)
            {
                Win32API_SetTaskbarState(Win32API_AppBarStates.AlwaysOnTop);
                ShowWindow(handle, SW_SHOW);
                IsHideLoop = false;
            }
        }

        private void ToggleTaskBar()
        {
            NoTaskbarConfig.Current.IsEnabled = !NoTaskbarConfig.Current.IsEnabled;
            if (NoTaskbarConfig.Current.IsEnabled)
            {
                HideTaskbar();
            }
            else
            {
                ShowTaskbar();
            }
        }

        private async void StartHideBarLoop()
        {
            while (IsHideLoop)
            {
                var handle = FindWindow("Shell_TrayWnd", null);
                ShowWindow(handle, SW_HIDE);
                await Task.Delay(100);
            }
        }

        public override async void AutoStart()
        {
            base.AutoStart();
            ApplyActions();

            await NoTaskbarConfig.Current.ApplyConfig();

            if (NoTaskbarConfig.Current.IsStartupEnabled)
            {
                TaskbarHideToy.Current.HideTaskbar();
                if (!NoTaskbarConfig.Current.IsEnabled)
                {
                    NoTaskbarConfig.Current.IsEnabled = true;
                }
            }
            else
            {
                if (NoTaskbarConfig.Current.IsEnabled)
                {
                    NoTaskbarConfig.Current.IsEnabled = false;
                }
            }
        }

        public override void RecoverStatus()
        {
            TaskbarHideToy.Current.ShowTaskbar();
        }

        public override void ApplyActions()
        {
            HotKeyInfo hotkey = HotKeyManager.Current.GetHotKey(_hotkey_Toggle);
            hotkey.ActionCallback = () => ToggleTaskBar();
        }
    }
}
