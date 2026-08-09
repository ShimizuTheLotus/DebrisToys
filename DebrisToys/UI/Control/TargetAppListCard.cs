using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys.UI.Control
{
    public sealed partial class TargetAppListCard : Microsoft.UI.Xaml.Controls.Control
    {
        public ObservableCollection<TargetAppListCardItemDTO> TargetAppListCardItems { get; private set; } = [];

        private ListView? _mainListView;
        public TargetAppListCard()
        {
            DefaultStyleKey = typeof(TargetAppListCard);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _mainListView = GetTemplateChild("PART_MainListView") as ListView;
            _mainListView?.ItemsSource = TargetAppListCardItems;
        }

        public void Add(TargetAppListCardItemDTO item)
        {
            if (!string.IsNullOrWhiteSpace(item.AppName)
                && !TargetAppListCardItems.Any(x => x.AppName == item.AppName))
            {
                TargetAppListCardItems.Add(item);
            }
        }

        public void Remove(TargetAppListCardItemDTO item)
        {
            TargetAppListCardItems.Remove(item);
        }

        public void RemoveSelection()
        {
            if (_mainListView == null)
            {
                return;
            }
            TargetAppListCardItems.Remove((TargetAppListCardItemDTO)_mainListView.SelectedItem);
        }

        public void SetValue(ObservableCollection<TargetAppListCardItemDTO> items)
        {
            TargetAppListCardItems = items;
            _mainListView?.ItemsSource = null;
            _mainListView?.ItemsSource = TargetAppListCardItems;
        }
    }
}
