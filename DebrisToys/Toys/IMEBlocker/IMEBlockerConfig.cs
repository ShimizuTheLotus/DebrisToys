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
                        item.OnChangedAcion = () => SaveConfig();
                    }
                }
            }
        } = [];

        public readonly string ConfigBasePath = "IMEBlocker";
        public string TargetAppConfigPath => Path.Combine(ConfigBasePath, "targetApp");

        public IMEBlockerConfig()
        {
            RelativePathApplyActionPair = new()
            {
                {TargetAppConfigPath, ApplyTargetAppConfig}
            };
        }
        public async void ApplyTargetAppConfig()
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
        public override void ApplyConfig()
        {
            ApplyTargetAppConfig();
        }

        public override void SaveConfig()
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

        public override List<HotKeyInfo> CheckConflicts()
        {
            throw new NotImplementedException();
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
    }
}
