using DebrisToys.Global.Helper;
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
using System.Text.Json.Serialization;
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
                    SaveIsEnabledConfig();
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
            string? json = await ToysConfigManager.Current.GetConfig(IsEnabledConfigPath);
            if (string.IsNullOrWhiteSpace(json))
                return;
            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = DebrisToys.Global.Helper.AppJsonContext.Default
            };
            IsEnabled = JsonSerializer.Deserialize<bool>(json, options);
        }

        private async Task ApplyTargetAppConfig()
        {
            string? json = await ToysConfigManager.Current.GetConfig(TargetAppConfigPath);
            if (string.IsNullOrWhiteSpace(json))
                return;
            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = DebrisToys.Global.Helper.AppJsonContext.Default
            };
            TargetAppList = JsonSerializer.Deserialize<ObservableCollection<TargetAppListCardItemDTO>>(json, options) ?? [];
        }
        public override async Task ApplyConfig()
        {
            await ApplyIsEnabledConfig();
            await ApplyTargetAppConfig();
        }

        public void SaveIsEnabledConfig()
        {
            string json = JsonSerializer.Serialize(IsEnabled, AppJsonContext.Default.Boolean);
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

            string json = JsonSerializer.Serialize(targetConfig, AppJsonContext.Default.ListTargetAppListCardItemDTO);
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
