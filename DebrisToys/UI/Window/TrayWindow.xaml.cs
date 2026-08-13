using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys.UI.Window;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TrayWindow : Microsoft.UI.Xaml.Window
{
  
    private Global.Helper.WindowExternalClickDetector _clickDetector;
    public TrayWindow()
    {
        InitializeComponent();

        // Hide app bar
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

        Win32.Window.RemoveTaskbarIcon(hWnd);
        if (appWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
        {
            presenter.IsAlwaysOnTop = true;
            presenter.SetBorderAndTitleBar(true, false);
        }

        this.Closed += TrayWindow_Closed;

        _clickDetector = new Global.Helper.WindowExternalClickDetector(this, () =>
        {
            this.Close();
        });

        Global.Helper.WindowInteraction.SetForegroundWindowAndSetFocus(this);
    }

    private void TrayWindow_Closed(object sender, WindowEventArgs args)
    {
        this.Closed -= TrayWindow_Closed;
        _clickDetector.Dispose();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        App.MainWindow?.AppWindow.Show();
        App.MainWindow?.Activate();
        //Global.Helper.WindowInteraction.SetForegroundWindowAndSetFocus(App.MainWindow);
        this.Close();
    }
}
