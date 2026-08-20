using DebrisToys.Global.Helper;
using DebrisToys.Toys.ScreenRotate;
using DebrisToys.ToysManager;
using DebrisToys.ToysManager.Base;
using ShimizuToolkit.HotkeyWinUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DebrisToys.Toys.SmartPaste
{
    public class SmartPasteConfig : ToyConfigBase<SmartPasteConfig>
    {
        public static event PropertyChangedEventHandler? PropertyChanged;

        public static void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(null, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool IsEnabled
        {
            get => field;
            set
            {
                if (field != value)
                {
                    field = value;
                    SaveIsEnabledConfig();
                    OnPropertyChanged();
                }
            }
        }

        public bool IsAutoReplaceEnabled
        {
            get => field;
            set
            {
                if (field != value)
                {
                    field = value;
                    SaveIsAutoReplaceEnabledConfig();
                    OnPropertyChanged();
                }
            }
        }

        public Dictionary<string, string> ReplaceValues = new() { { "A", "B" } };

        public readonly string ConfigBasePath = "SmartPaste";
        public string IsEnabledConfigPath => System.IO.Path.Combine(ConfigBasePath, "isEnabled");
        public string IsAutoReplaceEnabledConfigPath => System.IO.Path.Combine(ConfigBasePath, "isAutoReplacedEnabled");

        private async void SaveIsEnabledConfig()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(IsEnabled, AppJsonContext.Default.Boolean);
            ToysConfigManager.Current.SaveConfig(IsEnabledConfigPath, json);
        }

        private async void SaveIsAutoReplaceEnabledConfig()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(IsAutoReplaceEnabled, AppJsonContext.Default.Boolean);
            ToysConfigManager.Current.SaveConfig(IsAutoReplaceEnabledConfigPath, json);
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        private async Task ApplyIsEnabledConfig()
        {
            string? json = await ToysConfigManager.Current.GetConfig(IsEnabledConfigPath);
            if (string.IsNullOrWhiteSpace(json))
                return;
            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = DebrisToys.Global.Helper.AppJsonContext.Default
            };
            IsEnabled = JsonSerializer.Deserialize<bool>(json, options);
        }

        private async Task ApplyIsAutoReplaceEnabledConfig()
        {
            string? json = await ToysConfigManager.Current.GetConfig(IsAutoReplaceEnabledConfigPath);
            if (string.IsNullOrWhiteSpace(json))
                return;
            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = DebrisToys.Global.Helper.AppJsonContext.Default
            };
            IsAutoReplaceEnabled = JsonSerializer.Deserialize<bool>(json, options);
        }

        public override async Task ApplyConfig()
        {
            await ApplyIsEnabledConfig();
            await ApplyIsAutoReplaceEnabledConfig();
        }

        public override void SaveConfig()
        {
            SaveIsEnabledConfig();
        }

        public override List<HotKeyInfo> CheckConflicts()
        {
            throw new NotImplementedException();
        }
    }
}
