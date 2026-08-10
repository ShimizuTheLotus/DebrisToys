using DebrisToys.ToysManager;
using DebrisToys.ToysManager.Base;
using DebrisToys.UI.Control;
using ShimizuToolkit.HotkeyWinUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DebrisToys.Toys.IMEBlocker
{
    public class IMEBlockerConfig : ToyConfigBase<IMEBlockerConfig>
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
                }
            }
        }
        public ObservableCollection<TargetAppListCardItemDTO> TargetAppList
        {
            get => field;
            set
            {
                if (field != value)
                {
                    field = value;
                    foreach (var item in field)
                    {
                        item.OnChangedAcion = () => SaveTargetAppConfig();
                    }
                }
            }
        } = [];

        public readonly string ConfigBasePath = "IMEBlocker";
        public string IsEnabledConfigPath => Path.Combine(ConfigBasePath, "isEnabled");
        public string TargetAppConfigPath => Path.Combine(ConfigBasePath, "targetApp");

        public IMEBlockerConfig()
        {
            RelativePathApplyActionPair = new()
            {
                {TargetAppConfigPath, () => ApplyTargetAppConfig().GetAwaiter().GetResult()}
            };
        }

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

        private async Task ApplyTargetAppConfig()
        {
            try
            {
                string? json = await ToysConfigManager.Current.GetConfig(TargetAppConfigPath);
                TargetAppList = JsonSerializer.Deserialize<ObservableCollection<TargetAppListCardItemDTO>>(json) ?? [];
            }
            catch
            {
            }
        }
        public override async Task ApplyConfig()
        {
            await ApplyIsEnabledConfig();
            await ApplyTargetAppConfig();
        }

        public void SaveIsEnabledConfig()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(IsEnabled, options);
            ToysConfigManager.Current.SaveConfig(IsEnabledConfigPath, json);
        }
        public void SaveTargetAppConfig()
        {
            List<TargetAppListCardItemDTO> targetConfig = [];
            foreach (var item in TargetAppList)
            {
                targetConfig.Add(new()
                {
                    AppName = item.AppName,
                    IsEnabled = item.IsEnabled,
                });
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(targetConfig, options);
            ToysConfigManager.Current.SaveConfig(TargetAppConfigPath, json);
        }
        public override void SaveConfig()
        {
            SaveTargetAppConfig();
        }

        public override List<HotKeyInfo> CheckConflicts()
        {
            throw new NotImplementedException();
        }
    }
}
