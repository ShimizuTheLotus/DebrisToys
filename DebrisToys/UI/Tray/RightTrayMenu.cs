using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Text;

namespace DebrisToys.UI.Tray
{
    internal class RightTrayMenu : ShimizuToolkit.TrayIconWinUI.UI.TrayMenuFlyout
    {
        public RightTrayMenu()
        {
            AddMenuItems();
        }

        private void AddMenuItems()
        {
            AddMenuItem("Open MainWindow", ShowMainWindow);
            AddMenuItem("Exit", ExitApp);
        }

        private void ShowMainWindow()
        {
            App.MainWindow?.Activate();
        }

        public void ExitApp()
        {
            App.RequestExitApp();
        }
    }
}
