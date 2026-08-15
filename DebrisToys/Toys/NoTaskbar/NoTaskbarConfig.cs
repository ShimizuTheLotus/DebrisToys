using DebrisToys.Global.Helper;
using DebrisToys.ToysManager;
using DebrisToys.ToysManager.Base;
using ShimizuToolkit.HotkeyWinUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DebrisToys.Toys.NoTaskbar
{
    public class NoTaskbarConfig : ToyConfigBase<NoTaskbarConfig>
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
                    OnPropertyChanged();
                    SaveIsEnabledConfig();
                }
            }
        } = false;
        public bool IsStartupEnabled
        {
            get => field;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged();
                    SaveIsStartupEnabledConfig();
                }
            }
        }

        public readonly string ConfigBasePath = "NoTaskbar";
        public string IsEnabledConfigPath => System.IO.Path.Combine(ConfigBasePath, "isEnabled");
        public string IsStartupEnabledConfigPath => System.IO.Path.Combine(ConfigBasePath, "isStartupEnabled");

        private async Task ApplyIsEnabledConfig()
        {
            try
            {
                string? json = await ToysConfigManager.Current.GetConfig(IsEnabledConfigPath);
                var options = new JsonSerializerOptions
                {
                    TypeInfoResolver = DebrisToys.Global.Helper.AppJsonContext.Default
                };
                IsEnabled = JsonSerializer.Deserialize<bool>(json, options);
            }
            catch
            {
            }
        }

        private async Task ApplyIsStartupEnabledConfig()
        {
            try
            {
                string? json = await ToysConfigManager.Current.GetConfig(IsStartupEnabledConfigPath);
                var options = new JsonSerializerOptions
                {
                    TypeInfoResolver = DebrisToys.Global.Helper.AppJsonContext.Default
                };
                IsStartupEnabled = JsonSerializer.Deserialize<bool>(json, options);
            }
            catch
            {
            }
        }

        public override async Task ApplyConfig()
        {
            await ApplyIsEnabledConfig();
            await ApplyIsEnabledConfig();
            await ApplyIsStartupEnabledConfig();
        }

        public void SaveIsEnabledConfig()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(IsEnabled, AppJsonContext.Default.Boolean);
            ToysConfigManager.Current.SaveConfig(IsEnabledConfigPath, json);
        }
        public void SaveIsStartupEnabledConfig()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(IsStartupEnabled, AppJsonContext.Default.Boolean);
            ToysConfigManager.Current.SaveConfig(IsStartupEnabledConfigPath, json);
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
