using DebrisToys.Toys.IMEBlocker;
using DebrisToys.Toys.NoTaskbar;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.WindowManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private Type? _currentPageType;
        public MainWindow()
        {
            InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            AppTitleBar.Loaded += AppTitleBar_Loaded;
            AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
        }

        private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ExtendsContentIntoTitleBar)
            {
                SetTitleBarRegion();
            }
        }

        private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            AppTitleBar.Focus(FocusState.Programmatic);
            if (ExtendsContentIntoTitleBar)
            {
                SetTitleBarRegion();
            }
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem == null)
                return;
            Type navigationTargetType = default!;
            if (args.IsSettingsInvoked)
            {
                navigationTargetType = typeof(DebrisToys.UI.Page.SettingsPage);
            }
            if (args.InvokedItem == NoTaskbarItem.Content)
            {
                navigationTargetType = typeof(NoTaskbarPage);
            }
            else if (args.InvokedItem == IMEBlockerItem.Content)
            {
                navigationTargetType = typeof(IMEBlockerPage);
            }

            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo;

            if (navigationTargetType == null)
                return;
            if (_currentPageType != navigationTargetType)
            {
                _currentPageType = navigationTargetType;
                NavigationFrame.NavigateToType(navigationTargetType, null, navOptions);
            }
        }

        private void NavigationViewPaneButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationView.IsPaneOpen = !NavigationView.IsPaneOpen;
        }

        private void SetTitleBarRegion()
        {
            // Specify the interactive regions of the title bar.

            double scaleAdjustment = AppTitleBar.XamlRoot.RasterizationScale;

            RightPaddingColumn.Width = new GridLength(AppWindow.TitleBar.RightInset / scaleAdjustment);
            LeftPaddingColumn.Width = new GridLength(AppWindow.TitleBar.LeftInset / scaleAdjustment);
            GeneralTransform transform = NavigationViewPaneButton.TransformToVisual(null);
            Rect bounds = transform.TransformBounds(new Rect(0, 0,
                                                        NavigationViewPaneButton.ActualWidth,
                                                        NavigationViewPaneButton.ActualHeight));
            Windows.Graphics.RectInt32 NavigationPaneButtonRect = GetRect(bounds, scaleAdjustment);

            var rectArray = new Windows.Graphics.RectInt32[] {  NavigationPaneButtonRect };

            InputNonClientPointerSource nonClientInputSrc =
                InputNonClientPointerSource.GetForWindowId(this.AppWindow.Id);
            nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rectArray);
        }

        private Windows.Graphics.RectInt32 GetRect(Rect bounds, double scale)
        {
            return new Windows.Graphics.RectInt32(
                _X: (int)Math.Round(bounds.X * scale),
                _Y: (int)Math.Round(bounds.Y * scale),
                _Width: (int)Math.Round(bounds.Width * scale),
                _Height: (int)Math.Round(bounds.Height * scale)
            );
        }
    }
}
