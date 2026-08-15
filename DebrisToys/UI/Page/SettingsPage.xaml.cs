using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys.UI.Page
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Microsoft.UI.Xaml.Controls.Page
    {
        public SettingsPage()
        {
            InitializeComponent();

            GetConfigAndUpdateUI();
        }

        private async void GetConfigAndUpdateUI()
        {
            Settings_LaunchOnStartup_ToggleSwitch.IsOn = await DebrisToys.Settings.StartupRegister.IsStartUpEnabled();
        }

        private void Settings_LaunchOnStartup_ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (Settings_LaunchOnStartup_ToggleSwitch.IsOn)
            {
                DebrisToys.Settings.StartupRegister.EnableStartupTask();
            }
            else
            {
                DebrisToys.Settings.StartupRegister.DisableStartupTask();
            }
        }

        private void SettingsPage_Button_Quit_Click(object sender, RoutedEventArgs e)
        {
            App.RequestExitApp();
        }
    }
}
