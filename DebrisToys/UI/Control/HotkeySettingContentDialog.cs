using DebrisToys.UI.Page.ContentPresenter;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using ShimizuToolkit.HotkeyWinUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys.UI.Control
{
    public sealed partial class HotkeySettingContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        private HotkeySettingsControlContentPage page;
        public ShimizuToolkit.HotkeyWinUI.HotKeyInfo HotKeyInfo
        {
            get; set;
        }

        public HotkeySettingContentDialog(HotKeyInfo hotKeyInfo)
        {
            HotKeyInfo = hotKeyInfo;
            Title = DebrisToys.Global.Helper.LocalizedString.GetLocalizedString("Code.CS.Global.HotkeySettingContentDialog.Title");
            this.Content = page = new HotkeySettingsControlContentPage();
            page.KeyCaptureControl.SetKeys(HotKeyInfo.Keys.ToList());
            PrimaryButtonText = DebrisToys.Global.Helper.LocalizedString.GetLocalizedString("Code.CS.Global.HotkeySettingContentDialog.PrimaryButtonText");
            SecondaryButtonText = DebrisToys.Global.Helper.LocalizedString.GetLocalizedString("Code.CS.Global.HotkeySettingContentDialog.SecondaryButtonText");
            this.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            this.DefaultButton = ContentDialogButton.Primary;
            this.PrimaryButtonClick += HotkeySettingContentDialog_PrimaryButtonClick;
        }

        private void HotkeySettingContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.PrimaryButtonClick -= HotkeySettingContentDialog_PrimaryButtonClick;
            HotKeyInfo.Keys.Clear();
            page.KeyCaptureControl.CompleteCapture();
            foreach (var key in page.KeyCaptureControl.CapturedKeys)
            {
                HotKeyInfo.AddKeyAndNotify(key);
            }
            HotKeyInfo.UpdateModifierAndActionKey();
            HotKeyManager.Current.AddOrOverwriteHotKey(HotKeyInfo);
            HotKeyManager.Current.SaveHotkeyConfig();
        }
    }
}
