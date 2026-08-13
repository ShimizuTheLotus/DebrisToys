using DebrisToys.ToysManager;
using DebrisToys.ToysManager.Interface;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using ShimizuToolkit.HotkeyWinUI;
using ShimizuToolkit.TrayIconWinUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Microsoft.UI.Xaml.Application
    {
        public static Window? MainWindow;
        public static Window? TrayWindow;

        private readonly string hotkeyConfigPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "Hotkeys");

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            SetUpTrayIcon();
        }

        private void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            ToysConfigManager.Current.RecoverStatus();
            HotKeyManager.Current.Dispose();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainWindow = new MainWindow();
            MainWindow.Activate();

            var activatedArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
            if (activatedArgs.Kind == Microsoft.Windows.AppLifecycle.ExtendedActivationKind.StartupTask)
            {
                // Is auto startup
                MainWindow.AppWindow.Hide();
            }

            HotKeyManager.Current.HotkeyConfigPath = hotkeyConfigPath;
            await HotKeyManager.Current.LoadHotkeyConfig();

            ToysConfigManager.Current.Initialize();
        }

        private async void SetUpTrayIcon()
        {
            Uri fileUri = new("ms-appx:///Images/Icon/ico.ico");
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(fileUri);

            string iconPath = file.Path;

            if (System.IO.Path.Exists(file.Path))
            {
                NotifyIcon notifyIcon = new()
                {
                    Icon = new System.Drawing.Icon(iconPath),
                    Visible = true
                };
                ShimizuToolkit.TrayIconWinUI.TrayIconManager.Current.SetNotifyIcon(notifyIcon);
            }
            TrayWindow = new();
            ShimizuToolkit.TrayIconWinUI.TrayIconManager.Current.LeftClickAction += ShowTrayWindow;
            ShimizuToolkit.TrayIconWinUI.TrayIconManager.Current.RightMenuWindow = new ShimizuToolkit.TrayIconWinUI.UI.TrayFlyoutBaseWindow(() =>
            {
                return new DebrisToys.UI.Tray.RightTrayMenu();
            });
        }

        public void ShowTrayWindow()
        {
            TrayWindow = new DebrisToys.UI.Window.TrayWindow();
            // Move window to right bottom corner
            var (screenWidth, screenHeight) = Global.Helper.ScreenInteraction.GetMonitorWorkArea();
            // Window size consts
            int windowWidth = 600;
            int windowHeight = 800;

            int x = screenWidth - windowWidth;
            int y = screenHeight - windowHeight;
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(TrayWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            appWindow.MoveAndResize(new Windows.Graphics.RectInt32(x, y, windowWidth, windowHeight));

            TrayWindow.Activate();
        }

        public static void RequestExitApp()
        {
            App.Current.Exit();
        }
    }
}
