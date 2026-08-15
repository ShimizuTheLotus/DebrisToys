using DebrisToys.Toys.NoTaskbar;
using DebrisToys.ToysManager;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys.Toys.IMEBlocker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IMEBlockerPage : Page
    {

        public IMEBlockerConfig Config { get; set; } = IMEBlockerConfig.Current;
        public IMEBlocker IMEBlocker { get; set; } = IMEBlocker.Current;
        private void RegisterConfigPropertyChanged()
        {
            IMEBlockerConfig.PropertyChanged += IMEBlockerConfig_PropertyChanged;
            ;
        }
        private void UnregisterConfigPropertyChanged()
        {
            IMEBlockerConfig.PropertyChanged -= IMEBlockerConfig_PropertyChanged;
        }

        private void IMEBlockerConfig_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IMEBlockerConfig.IsEnabled))
            {
                IMEBlockerToggleSwitch.IsOn = Config.IsEnabled;
                if (Config.IsEnabled)
                {
                    IMEBlocker.Start();
                }
                else
                {
                    IMEBlocker.Stop();
                }
            }
        }

        public IMEBlockerPage()
        {
            InitializeComponent();

            this.Unloaded += IMEBlockerPage_Unloaded;
            RegisterConfigPropertyChanged();
            TargetAppListCard.SetValue(Config.TargetAppList);

            UpdateUIFromConfig();
        }

        public void UpdateUIFromConfig()
        {
            IMEBlockerToggleSwitch.IsOn = Config.IsEnabled;
        }

        private void IMEBlockerPage_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= IMEBlockerPage_Unloaded;
            UnregisterConfigPropertyChanged();
        }

        private void IMEBlockerToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            Config.IsEnabled = IMEBlockerToggleSwitch.IsOn;
        }

        private void AddAppNameButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewBlockedApp();
        }

        private void AddOptionAppNameTextBlock_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                AddNewBlockedApp();
            }
        }

        private void AddNewBlockedApp()
        {
            if (string.IsNullOrWhiteSpace(AddOptionAppNameTextBlock.Text))
            {
                return;
            }
            Config.TargetAppList.Add(new()
            {
                AppName = NormalizeName(AddOptionAppNameTextBlock.Text),
                OnChangedAcion = () => Config.SaveTargetAppConfig()
            });
            AddOptionAppNameTextBlock.Text = string.Empty;
            Config.SaveTargetAppConfig();
        }

        private static string NormalizeName(string name)
        {
            name = name.Trim();
            if (!string.IsNullOrWhiteSpace(name))
                if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    return name;
            return name + ".exe";
        }

        private void RemoveAppNameButton_Click(object sender, RoutedEventArgs e)
        {
            TargetAppListCard.RemoveSelection();
            Config.SaveTargetAppConfig();
        }
    }
}
