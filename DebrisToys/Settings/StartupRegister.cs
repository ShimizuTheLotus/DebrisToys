using Microsoft.Win32;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace DebrisToys.Settings
{
    public class StartupRegister
    {
        private const string StartupKeyName = "DebrisToys";

        public static async void EnableStartupTask()
        {
            var task = await StartupTask.GetAsync(StartupKeyName);
            await task.RequestEnableAsync();
        }

        public static async void DisableStartupTask()
        {
            var task = await StartupTask.GetAsync(StartupKeyName);
            task.Disable();
        }

        public static async Task<bool> IsStartUpEnabled()
        {
            var task = await StartupTask.GetAsync(StartupKeyName);
            switch (task.State)
            {
                case StartupTaskState.Disabled:
                    return false;
                case StartupTaskState.DisabledByPolicy:
                    return false;
                case StartupTaskState.DisabledByUser:
                    return false;
                case StartupTaskState.Enabled:
                    return true;
                default:
                    return false;
            }
        }
    }

}