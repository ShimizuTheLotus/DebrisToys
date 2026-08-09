using DebrisToys.ToysManager;
using DebrisToys.ToysManager.Base;
using ShimizuToolkit.HotkeyWinUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

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

        public override void ApplyConfig()
        {
            throw new NotImplementedException();
        }

        public override void SaveConfig()
        {

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
