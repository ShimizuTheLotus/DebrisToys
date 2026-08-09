using ShimizuToolkit.HotkeyWinUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace DebrisToys.ToysManager.Base
{
    public abstract class ToyConfigBase
    {
        public Dictionary<string, Action> RelativePathApplyActionPair { get; set; } = [];
        public List<ShimizuToolkit.HotkeyWinUI.HotKeyInfo> HotkeyInfos { get; private set; } = [];
        public abstract void ApplyConfig();
        public abstract void SaveConfig();
        public abstract List<ShimizuToolkit.HotkeyWinUI.HotKeyInfo> CheckConflicts();
    }
    public abstract class ToyConfigBase<T> : ToyConfigBase where T : ToyConfigBase<T>, new ()
    {
        private static readonly Lazy<T> _lazyInstance = new Lazy<T>(() => new T());
        public static T Current => _lazyInstance.Value;
    }
}
