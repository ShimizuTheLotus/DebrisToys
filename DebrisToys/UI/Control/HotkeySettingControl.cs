using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using ShimizuToolkit.HotkeyWinUI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys.UI.Control
{
    public sealed partial class HotkeySettingControl : Microsoft.UI.Xaml.Controls.Control
    {
        private ShimizuToolkit.HotkeyWinUI.Controls.KeyCaptureControl? _keyCaptureControl;
        public static readonly DependencyProperty HotkeyInfoProperty =
            DependencyProperty.Register(
            nameof(HotkeyInfo),
            typeof(List<VirtualKey>),
            typeof(HotkeySettingControl),
            new PropertyMetadata(null, OnHotKeyInfoChanged));

        private static void OnHotKeyInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as HotkeySettingControl;
            var newValue = e.NewValue as string;
            c?.CheckHotkeyInfo();
        }

        private void CheckHotkeyInfo()
        {
            var a = (List<VirtualKey>)GetValue(HotkeyInfoProperty);
            _keyCaptureControl?.SetKeys(a);
        }

        public ShimizuToolkit.HotkeyWinUI.HotKeyInfo HotkeyInfo
        {
            get => (ShimizuToolkit.HotkeyWinUI.HotKeyInfo)GetValue(HotkeyInfoProperty);
            set => SetValue(HotkeyInfoProperty, value);
        }

        public HotkeySettingControl()
        {
            DefaultStyleKey = typeof(HotkeySettingControl);

            this.Loaded += HotkeySettingControl_Loaded;
            this.Unloaded += HotkeySettingControl_Unloaded;
        }

        private void HotkeySettingControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= HotkeySettingControl_Loaded;
            this.Unloaded -= HotkeySettingControl_Unloaded;

            _keyCaptureControl?.IsCapturing = true;
        }

        private void HotkeySettingControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= HotkeySettingControl_Loaded;
            _keyCaptureControl?.IsCapturing = false;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _keyCaptureControl = GetTemplateChild("PART_KeyCaptureControl") as ShimizuToolkit.HotkeyWinUI.Controls.KeyCaptureControl;
        }
    }
}
