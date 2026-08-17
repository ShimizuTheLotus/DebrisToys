using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Text;

namespace DebrisToys.UI.Tray
{
    internal class RightTrayMenu
    {
        public static ShimizuToolkit.TrayIconWinUI.UI.TrayMenuFlyout CreateMenu()
        {
            var f = new ShimizuToolkit.TrayIconWinUI.UI.TrayMenuFlyout();
            f.AddMenuItem("Open MainWindow", ShowMainWindow);
            f.AddMenuItem("Exit", ExitApp);
            return f;
        }

        private static void ShowMainWindow()
        {
            App.MainWindow?.Activate();
        }

        private static void ExitApp()
        {
            App.RequestExitApp();
        }
    }
}
