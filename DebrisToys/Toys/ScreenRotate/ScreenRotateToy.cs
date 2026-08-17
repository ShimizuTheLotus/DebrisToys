using DebrisToys.Toys.NoTaskbar;
using DebrisToys.ToysManager.Base;
using ShimizuToolkit.HotkeyWinUI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DebrisToys.Toys.ScreenRotate
{
    public class ScreenRotateToy : ToyBase
    {
        private const string _hotkey_to_landscape = $"{nameof(ScreenRotateToy)}.ToLandscape";
        private const string _hotkey_to_landscape_flipped = $"{nameof(ScreenRotateToy)}.ToLandscapeFlipped";
        private const string _hotkey_to_portrait = $"{nameof(ScreenRotateToy)}.ToPortrait";
        private const string _hotkey_to_portrait_flipped = $"{nameof(ScreenRotateToy)}.ToPortraitFlipped";

        public ScreenRotateToy()
        {
            HotkeyNameList = [_hotkey_to_landscape, _hotkey_to_landscape_flipped, _hotkey_to_portrait, _hotkey_to_portrait_flipped];
        }

        public static ScreenRotateToy Current => LazyInitializer.Instance;
        private static class LazyInitializer
        {
            public static readonly ScreenRotateToy Instance = new();
        }

        public static void RotateTo(int orientation)
        {
            if (!ScreenRotateToyConfig.Current.IsEnabled)
                return;
            ChangeDisplayOrientation(orientation);
        }
        private static void ChangeDisplayOrientation(int displayOrientation)
        {
            DEVMODE devmode = new DEVMODE();
            devmode.dmDeviceName = new String(new char[32]);
            devmode.dmFormName = new String(new char[32]);
            devmode.dmSize = (short)Marshal.SizeOf(devmode);

            if (0 != Win32.EnumDisplaySettings(null, Win32.ENUM_CURRENT_SETTINGS, ref devmode))
            {
                int height = 0;
                int width = 0;
                switch (devmode.dmDisplayOrientation)
                {
                    case Win32.DMDO_DEFAULT:
                        height = devmode.dmPelsHeight;
                        width = devmode.dmPelsWidth;
                        break;
                    case Win32.DMDO_270:
                        width = devmode.dmPelsHeight;
                        height = devmode.dmPelsWidth;
                        break;
                    case Win32.DMDO_180:
                        height = devmode.dmPelsHeight;
                        width = devmode.dmPelsWidth;
                        break;
                    case Win32.DMDO_90:
                        width = devmode.dmPelsHeight;
                        height = devmode.dmPelsWidth;
                        break;
                    default:
                        // unknown orientation value
                        break;
                }

                int temp = devmode.dmPelsHeight;
                devmode.dmPelsHeight = devmode.dmPelsWidth;
                devmode.dmPelsWidth = temp;
                if (devmode.dmDisplayOrientation != displayOrientation)
                {
                    switch (displayOrientation)
                    {
                        case Win32.DMDO_DEFAULT:
                            devmode.dmPelsHeight = height;
                            devmode.dmPelsWidth = width;
                            devmode.dmDisplayOrientation = Win32.DMDO_DEFAULT;
                            break;
                        case Win32.DMDO_270:
                            devmode.dmPelsHeight = width;
                            devmode.dmPelsWidth = height;
                            devmode.dmDisplayOrientation = Win32.DMDO_270;
                            break;
                        case Win32.DMDO_180:
                            devmode.dmPelsHeight = height;
                            devmode.dmPelsWidth = width;
                            devmode.dmDisplayOrientation = Win32.DMDO_180;
                            break;
                        case Win32.DMDO_90:
                            devmode.dmPelsHeight = width;
                            devmode.dmPelsWidth = height;
                            devmode.dmDisplayOrientation = Win32.DMDO_90;
                            break;
                        default:
                            // unknown orientation value
                            break;
                    }
                }
                else
                {
                    return;
                }

                int iRet = Win32.ChangeDisplaySettings(ref devmode, 0);
                if (iRet == Win32.DISP_CHANGE_FAILED)
                {
                    return;
                }
                else
                {
                    iRet = Win32.ChangeDisplaySettings(ref devmode, Win32.CDS_UPDATEREGISTRY);

                    switch (iRet)
                    {
                        case Win32.DISP_CHANGE_SUCCESSFUL:
                            {
                                break;
                            }
                        case Win32.DISP_CHANGE_RESTART:
                            {
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
        }

        public override async void AutoStart()
        {
            base.AutoStart();
            ApplyActions();

            await ScreenRotateToyConfig.Current.ApplyConfig();
        }

        public override void RecoverStatus()
        {
            ScreenRotateToy.RotateTo(Win32.DMDO_DEFAULT);
        }

        public override void ApplyActions()
        {
            HotKeyInfo hotkey = HotKeyManager.Current.GetHotKey(_hotkey_to_landscape);
            hotkey.ActionCallback = () => RotateTo(Win32.DMDO_DEFAULT);
            hotkey = HotKeyManager.Current.GetHotKey(_hotkey_to_landscape_flipped);
            hotkey.ActionCallback = () => RotateTo(Win32.DMDO_180);
            hotkey = HotKeyManager.Current.GetHotKey(_hotkey_to_portrait);
            hotkey.ActionCallback = () => RotateTo(Win32.DMDO_90);
            hotkey = HotKeyManager.Current.GetHotKey(_hotkey_to_portrait_flipped);
            hotkey.ActionCallback = () => RotateTo(Win32.DMDO_270);
        }
    }
}
