using DebrisToys.Toys.NoTaskbar;
using DebrisToys.ToysManager.Base;
using ShimizuToolkit.HotkeyWinUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Power;
using static System.Net.Mime.MediaTypeNames;

namespace DebrisToys.Toys.SmartPaste
{
    public class SmartPasteToy : ToyBase
    {
        private const string _hotkey_ReplaceAction = $"{nameof(SmartPasteToy)}.RunReplaceAction";

        public SmartPasteToy()
        {
            HotkeyNameList = [_hotkey_ReplaceAction];
            Clipboard.ContentChanged += Clipboard_ContentChanged;
        }

        private void Clipboard_ContentChanged(object? sender, object e)
        {
            if (!SmartPasteConfig.Current.IsEnabled)
                return;
            if (!SmartPasteConfig.Current.IsAutoReplaceEnabled)
                return;
            ReplaceStrings();
        }

        public static SmartPasteToy Current => LazyInitializer.Instance;
        private static class LazyInitializer
        {
            public static readonly SmartPasteToy Instance = new();
        }

        private static bool _replacing = false;

        public void RunReplaceAction()
        {
            if (!SmartPasteConfig.Current.IsEnabled)
                return;
            ReplaceStrings();
        }

        private async void ReplaceStrings()
        {
            try
            {
                if (_replacing)
                    return;
                _replacing = true;
                DataPackageView dview = Clipboard.GetContent();
                if (dview.Contains(StandardDataFormats.Text))
                {
                    string text = await dview.GetTextAsync();

                    string pattern = string.Join("|", SmartPasteConfig.Current.ReplaceValues.Keys.Select(Regex.Escape));
                    string newText = Regex.Replace(text, pattern, match =>
                    {
                        return SmartPasteConfig.Current.ReplaceValues[match.Value];
                    });

                    DataPackage newPackage = new DataPackage();
                    newPackage.SetText(newText);
                    Clipboard.SetContent(newPackage);
                }
                _replacing = false;
            }
            catch
            {
                _replacing = false;
            }
        }

        public override async void AutoStart()
        {
            base.AutoStart();
            ApplyActions();

            await SmartPasteConfig.Current.ApplyConfig();
        }

        public override void RecoverStatus()
        {
            Clipboard.ContentChanged -= Clipboard_ContentChanged;
        }

        public override void ApplyActions()
        {
            HotKeyInfo hotkey = HotKeyManager.Current.GetHotKey(_hotkey_ReplaceAction);
            hotkey.ActionCallback = () => RunReplaceAction();
        }
    }
}
