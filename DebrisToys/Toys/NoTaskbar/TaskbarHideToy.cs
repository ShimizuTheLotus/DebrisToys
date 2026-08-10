using DebrisToys.ToysManager.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static DebrisToys.Toys.NoTaskbar.Win32;

namespace DebrisToys.Toys.NoTaskbar
{
    public class TaskbarHideToy : ToyBase
    {
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
            await NoTaskbarConfig.Current.ApplyConfig();
            if (NoTaskbarConfig.Current.IsEnabled)
            {
                TaskbarHideToy.Current.HideTaskbar();
            }
        }

        public override void RecoverStatus()    
        {
            TaskbarHideToy.Current.ShowTaskbar();
        }
    }
}
