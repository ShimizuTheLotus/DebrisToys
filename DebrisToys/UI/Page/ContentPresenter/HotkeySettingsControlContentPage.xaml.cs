using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ShimizuToolkit.HotkeyWinUI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys.UI.Page.ContentPresenter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HotkeySettingsControlContentPage : Microsoft.UI.Xaml.Controls.Page
    {
        public KeyCaptureControl KeyCaptureControl => _keyCaptureControl;
        public HotkeySettingsControlContentPage()
        {
            InitializeComponent();

            this.Loaded += HotkeySettingsControlContentPage_Loaded;
        }

        private void HotkeySettingsControlContentPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= HotkeySettingsControlContentPage_Loaded;
            _keyCaptureControl.IsCapturing = true;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _keyCaptureControl.SetKeys(new());
        }
    }
}
