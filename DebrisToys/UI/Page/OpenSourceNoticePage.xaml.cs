using DebrisToys.Class;
using DebrisToys.UI.Control;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys.UI.Page
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OpenSourceNoticePage : Microsoft.UI.Xaml.Controls.Page
    {
        List<OpenSourceNoticeInfo> OpenSourceNoticeInfo { get; set; } = [];
        public OpenSourceNoticePage()
        {
            InitializeComponent();

            OpenSourceNoticeInfo.Add(new()
            {
                PackageName = "Microsoft.WindowsAppSDK",
                Version = "2.3.1",
                License = "MICROSOFT SOFTWARE LICENSE TERMS",
                LicenseLink = "https://www.nuget.org/packages/Microsoft.WindowsAppSDK/2.3.1/License",
            });
            OpenSourceNoticeInfo.Add(new()
            {
                PackageName = "System.Windows.Forms",
                Version = "4.0.0.0",
            });

            this.Loaded += OpenSourceNoticePage_Loaded;
        }

        private void OpenSourceNoticePage_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var i in OpenSourceNoticeInfo)
            {
                var card = new Card()
                {
                    Title = i.PackageName + " " + i.Version,
                    Description = i.License,
                    Style = (Style)Application.Current.Resources["LinkCardStyle"],
                };
                if (i.License == "Unknown")
                {
                    card.ClearRightPartElement();
                }
                if (i.LicenseLink != null)
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = i.LicenseLink,
                        UseShellExecute = true
                    };
                    card.Click += (s, e) => System.Diagnostics.Process.Start(psi);
                }
                OpenSourceItemsStackPanel.Children.Add(card);
            }
        }
    }
}
