using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys.UI.Control
{
    public sealed partial class TargetAppListCardItem : Microsoft.UI.Xaml.Controls.Control, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty AppNameProperty =
            DependencyProperty.Register(
            nameof(AppName),
            typeof(string),
            typeof(TargetAppListCardItem),
            new PropertyMetadata(string.Empty, OnTitleChanged));
        public static new readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
            nameof(IsEnabled),
            typeof(bool),
            typeof(TargetAppListCardItem),
            new PropertyMetadata(true, OnIsEnabledChanged));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cardItem = d as TargetAppListCardItem;
            var newValue = e.NewValue as string;
            cardItem?._appNameTextBlock?.Text = newValue;
        }
        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cardItem = d as TargetAppListCardItem;
            var newValue = (bool)e.NewValue;
            cardItem?._toggleSwitch?.IsOn = newValue;
        }

        public string AppName
        {
            get => (string)GetValue(AppNameProperty);
            set
            {
                SetValue(AppNameProperty, value);
                OnPropertyChanged();
            }
        }

        public new bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set
            {
                SetValue(IsEnabledProperty, value);
                OnPropertyChanged();
            }
        }


        private TextBlock? _appNameTextBlock;
        private ToggleSwitch? _toggleSwitch;


        public TargetAppListCardItem()
        {
            DefaultStyleKey = typeof(TargetAppListCardItem);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _appNameTextBlock = GetTemplateChild("PART_AppNameTextBlock") as TextBlock;
            _toggleSwitch = GetTemplateChild("PART_EnableStatusToggleSwitch") as ToggleSwitch;

            _appNameTextBlock?.Text = AppName;
            _toggleSwitch?.IsOn = IsEnabled;
        }
    }
}
