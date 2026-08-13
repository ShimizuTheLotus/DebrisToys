using DebrisToys.ToysManager.Interface;
using ShimizuToolkit.HotkeyWinUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace DebrisToys.ToysManager.Base
{
    public abstract class ToyBase : IAutoStart, IRecoverStatus
    {
        public List<string> HotkeyNameList = [];
        public List<HotKeyInfo> HotKeys { get; set; } = [];

        public void InitializeHotkeys()
        {
            HotKeys.Clear();
            foreach (string name in HotkeyNameList)
            {
                HotKeys.Add(HotKeyManager.Current.GetHotKey(name));
            }
        }

        public abstract void ApplyActions();

        public virtual void AutoStart()
        {
            InitializeHotkeys();
        }

        public virtual void RecoverStatus()
        {

        }
    }
}
