using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using ShimizuToolkit.HotkeyWinUI;
using ShimizuToolkit.HotkeyWinUI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys.UI.Control
{
    public sealed partial class HotkeyCardEditEntryButton : Microsoft.UI.Xaml.Controls.Control
    {
        public ShimizuToolkit.HotkeyWinUI.HotKeyInfo HotkeyInfo
        {
            get => field;
            set
            {
                if (field != value)
                {
                    field = value;
                    UpdateUI();
                }
            }
        } = new();

        public string HotkeyIdentifier
        {
            get; set;
        } = string.Empty;

        private Button? _baseButton;
        private StackPanel? _hotkeyNotEmptyPresenter;
        private KeyBlockPanel? _keyBlockPanel;
        private StackPanel? _hotkeyEmptyPresenter;
        private HotkeySettingContentDialog? _hotkeySettingContentDialog;

        public HotkeyCardEditEntryButton()
        {
            DefaultStyleKey = typeof(HotkeyCardEditEntryButton);

            this.Loaded += HotkeyCardEditEntryButton_Loaded;
            this.Unloaded += HotkeyCardEditEntryButton_Unloaded;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _baseButton = GetTemplateChild("PART_BaseButton") as Button;
            _hotkeyNotEmptyPresenter = GetTemplateChild("PART_HotkeyNotEmptyPresenter") as StackPanel;
            _keyBlockPanel = GetTemplateChild("PART_KeyBlockPresenter") as KeyBlockPanel;
            _hotkeyEmptyPresenter = GetTemplateChild("PART_HotkeyEmptyPresenter") as StackPanel;
            _baseButton?.Click += _baseButton_Click;
            if (!string.IsNullOrWhiteSpace(HotkeyIdentifier))
            {
                HotkeyInfo = HotKeyManager.Current.GetHotKey(HotkeyIdentifier);
            }
            UpdateUI();
        }

        private async void _baseButton_Click(object sender, RoutedEventArgs e)
        {
            _hotkeySettingContentDialog = new(HotkeyInfo)
            {
                XamlRoot = this.XamlRoot
            };
            var result = await _hotkeySettingContentDialog.ShowAsync();
            UpdateUI();
        }

        private void HotkeyCardEditEntryButton_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= HotkeyCardEditEntryButton_Loaded;
        }

        private void HotkeyCardEditEntryButton_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= HotkeyCardEditEntryButton_Loaded;
            this.Unloaded -= HotkeyCardEditEntryButton_Unloaded;
            _baseButton?.Click -= _baseButton_Click;
            UpdateUI();
        }

        private void UpdateUI()
        {
            bool hotkeyNotEmpty = HotkeyInfo.Keys.Count > 0;
            _keyBlockPanel?.VirtualKeys = HotkeyInfo.Keys.Select(k => (uint)k);
            _hotkeyNotEmptyPresenter?.Visibility = hotkeyNotEmpty ? Visibility.Visible : Visibility.Collapsed;
            _hotkeyEmptyPresenter?.Visibility = hotkeyNotEmpty ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
