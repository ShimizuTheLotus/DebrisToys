using DebrisToys.ToysManager;
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

namespace DebrisToys.Toys.NoTaskbar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NoTaskbarPage : Page
    {
        public NoTaskbarConfig Config { get; set; } = new();

        private void RegisterConfigPropertyChanged()
        {
            NoTaskbarConfig.PropertyChanged += NoTaskbarConfig_PropertyChanged;
        }
        private void UnregisterConfigPropertyChanged()
        {
            NoTaskbarConfig.PropertyChanged -= NoTaskbarConfig_PropertyChanged;
        }


        private void NoTaskbarConfig_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NoTaskbarConfig.IsEnabled))
            {
                if (TaskbarToggleSwitch.IsOn != Config.IsEnabled)
                {
                    TaskbarToggleSwitch.IsOn = Config.IsEnabled;
                }

                if (Config.IsEnabled)
                {
                    TaskbarHideToy.Current.HideTaskbar();
                }
                else
                {
                    TaskbarHideToy.Current.ShowTaskbar();
                }
            }
        }

        public NoTaskbarPage()
        {
            InitializeComponent();

            this.Unloaded += NoTaskbarPage_Unloaded;
            RegisterConfigPropertyChanged();


            var config = ToysConfigManager.Current.GetToyConfig<NoTaskbarConfig>();
            if (config != null)
            {
                Config = (NoTaskbarConfig)config;
            }
            else
            {
                ToysConfigManager.Current.AddToyConfig(Config);
            }
            UpdateUIFromConfig();
        }

        public void UpdateUIFromConfig()
        {
            TaskbarToggleSwitch.IsOn = Config.IsEnabled;
        }

        private void NoTaskbarPage_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= NoTaskbarPage_Unloaded;
            UnregisterConfigPropertyChanged();
        }

        private void TaskbarToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                bool isOn = toggleSwitch.IsOn;
                if (Config.IsEnabled != isOn)
                {
                    Config.IsEnabled = isOn;
                }
            }
        }
    }
}
