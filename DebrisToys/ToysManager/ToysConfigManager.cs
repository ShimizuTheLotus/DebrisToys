using DebrisToys.Toys.IMEBlocker;
using DebrisToys.Toys.NoTaskbar;
using DebrisToys.Toys.ScreenRotate;
using DebrisToys.ToysManager.Base;
using DebrisToys.ToysManager.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebrisToys.ToysManager
{
    public partial class ToysConfigManager
    {
        public readonly string BaseConfigPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
        public HashSet<ToyConfigBase> ToyConfigs { get; private set; } = [];
        public HashSet<ToyBase> Toys { get; private set; } = [];
        public ToysConfigManager()
        {
        }
        public static ToysConfigManager Current
        {
            get => LazyInitializer.Instance;
        }
        private static class LazyInitializer
        {
            static LazyInitializer()
            {
            }
            public static readonly ToysConfigManager Instance = new();
        }

        public void Initialize()
        {
            ToyConfigs =
            [
                NoTaskbarConfig.Current,
                IMEBlockerConfig.Current,
                ScreenRotateToyConfig.Current,
            ];

            Toys =
            [
                TaskbarHideToy.Current,
                IMEBlocker.Current,
                ScreenRotateToy.Current
            ];

            RunToys();
        }

        public T? GetToyConfig<T>() where T : ToyConfigBase
        {
            var config = ToyConfigs.OfType<T>().FirstOrDefault();
            return config;
        }

        public void AddToyConfig(ToyConfigBase config)
        {
            if (!ToyConfigs.Contains(config))
            {
                ToyConfigs.Add(config);
            }
        }

        public void RunToys()
        {
            foreach (var toy in Toys)
            {
                toy.AutoStart();
            }
        }

        public void RecoverStatus()
        {
            foreach (var toy in Toys)
            {
                toy.RecoverStatus();
            }
        }

        public async Task<string> GetConfig(string path)
        {
            string fullPath = System.IO.Path.Combine(BaseConfigPath, path);
            if (File.Exists(fullPath))
            {
                try
                {
                    var text = await File.ReadAllTextAsync(fullPath);
                    return text;
                }
                catch(Exception ex)
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        public async void SaveConfig(string path, string content)
        {
            try
            {
                string fullPath = System.IO.Path.Combine(BaseConfigPath, path);
                string? directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }
                await File.WriteAllTextAsync(fullPath, content);
            }
            catch { }
        }
    }
}
