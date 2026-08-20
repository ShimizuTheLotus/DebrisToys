using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys.Toys.SmartPaste;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SmartPastePage : Page
{
    public SmartPastePage()
    {
        InitializeComponent();

        IsEnabledToggleSwitch.IsOn = SmartPasteConfig.Current.IsEnabled;
        IsAutoReplaceEnabledToggleSwitch.IsOn = SmartPasteConfig.Current.IsAutoReplaceEnabled;
    }

    private void IsEnabledToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        SmartPasteConfig.Current.IsEnabled = IsEnabledToggleSwitch.IsOn;
    }

    private void IsAutoReplaceEnabledToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        SmartPasteConfig.Current.IsAutoReplaceEnabled = IsAutoReplaceEnabledToggleSwitch.IsOn;
    }
}
