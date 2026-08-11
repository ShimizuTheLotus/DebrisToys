using DebrisToys.ToysManager;
using DebrisToys.ToysManager.Base;
using ShimizuToolkit.HotkeyWinUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
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
                    SaveIsEnabledConfig();
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
                IsEnabled = JsonSerializer.Deserialize<bool>(json);
            }
            catch
            {
            }
        }

        public override async Task ApplyConfig()
        {
            await ApplyIsEnabledConfig();
        }

        public void SaveIsEnabledConfig()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(IsEnabled, options);
            ToysConfigManager.Current.SaveConfig(IsEnabledConfigPath, json);
        }
        public void SaveIsStartupEnabledConfig()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(IsStartupEnabled, options);
            ToysConfigManager.Current.SaveConfig(IsEnabledConfigPath, json);
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
