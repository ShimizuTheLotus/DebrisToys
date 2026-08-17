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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys.Toys.ScreenRotate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScreenRotateToyPage : Page
    {
        public ScreenRotateToyConfig Config { get; set; } = ScreenRotateToyConfig.Current;

        public ScreenRotateToyPage()
        {
            InitializeComponent();

            IsEnabledToggleSwitch.IsOn = Config.IsEnabled;
        }

        private void IsEnabledToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                Config.IsEnabled = toggleSwitch.IsOn;
            }
        }
    }
}
