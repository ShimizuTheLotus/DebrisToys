using DebrisToys.Toys.NoTaskbar;
using DebrisToys.ToysManager;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys.Toys.IMEBlocker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IMEBlockerPage : Page
    {
        // Event hook
        private IntPtr _foregroundHook = IntPtr.Zero;
        private IntPtr _focusHook = IntPtr.Zero;
        private Win32.WinEventDelegate? _foregroundDelegate;
        private Win32.WinEventDelegate? _focusDelegate;
        private readonly object _hookSync = new();

        private IntPtr _englishHkl = IntPtr.Zero;

        // Saved IME stat
        private IntPtr _savedHkl = IntPtr.Zero;
        private IntPtr _savedHwnd = IntPtr.Zero;
        private bool? _savedImeOpen = null;
        private bool _isInBlockedApp = false;

        // Last window stat
        private IntPtr _prevHwnd = IntPtr.Zero;
        private uint _prevThreadId = 0;
        private bool _prevWasBlocked = false;

        // Queue
        private readonly ConcurrentQueue<Action> _actionQueue = new();
        private int _isProcessingQueue = 0;

        public IMEBlockerConfig Config { get; set; } = IMEBlockerConfig.Current;
        private void RegisterConfigPropertyChanged()
        {
            IMEBlockerConfig.PropertyChanged += IMEBlockerConfig_PropertyChanged;
            ;
        }
        private void UnregisterConfigPropertyChanged()
        {
            IMEBlockerConfig.PropertyChanged -= IMEBlockerConfig_PropertyChanged;
        }

        private void IMEBlockerConfig_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IMEBlockerConfig.IsEnabled))
            {
                IMEBlockerToggleSwitch.IsOn = Config.IsEnabled;
                if (Config.IsEnabled)
                {
                    StartWatcher();
                }
                else
                {
                    StopWatcher();
                }
            }
        }

        public IMEBlockerPage()
        {
            InitializeComponent();

            this.Unloaded += IMEBlockerPage_Unloaded;
            RegisterConfigPropertyChanged();
            TargetAppListCard.SetValue(Config.TargetAppList);
        }

        private void IMEBlockerPage_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= IMEBlockerPage_Unloaded;
            UnregisterConfigPropertyChanged();
        }

        private void IMEBlockerToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            Config.IsEnabled = IMEBlockerToggleSwitch.IsOn;
        }


        # region MainFunctionHelper
        private void StartWatcher()
        {
            lock (_hookSync)
            {
                if (_foregroundHook != IntPtr.Zero && _focusHook != IntPtr.Zero)
                    return;

                try
                {
                    _englishHkl = Win32.LoadKeyboardLayout("00000409", 0);
                }
                catch { _englishHkl = IntPtr.Zero; }

                _foregroundDelegate = ForegroundWinEventProc;
                _foregroundHook = Win32.SetWinEventHook(
                    Win32.EVENT_SYSTEM_FOREGROUND,
                    Win32.EVENT_SYSTEM_FOREGROUND,
                    IntPtr.Zero,
                    _foregroundDelegate,
                    0, 0,
                    Win32.WINEVENT_OUTOFCONTEXT | Win32.WINEVENT_SKIPOWNPROCESS
                );

                _focusDelegate = FocusWinEventProc;
                _focusHook = Win32.SetWinEventHook(
                    Win32.EVENT_OBJECT_FOCUS,
                    Win32.EVENT_OBJECT_FOCUS,
                    IntPtr.Zero,
                    _focusDelegate,
                    0, 0,
                    Win32.WINEVENT_OUTOFCONTEXT | Win32.WINEVENT_SKIPOWNPROCESS
                );

                _savedHkl = IntPtr.Zero;
                _savedImeOpen = null;
                _isInBlockedApp = false;
                _prevHwnd = IntPtr.Zero;
                _prevThreadId = 0;
                _prevWasBlocked = false;
            }
        }

        private void StopWatcher()
        {
            lock (_hookSync)
            {
                if (_foregroundHook != IntPtr.Zero)
                {
                    Win32.UnhookWinEvent(_foregroundHook);
                    _foregroundHook = IntPtr.Zero;
                }

                if (_focusHook != IntPtr.Zero)
                {
                    Win32.UnhookWinEvent(_focusHook);
                    _focusHook = IntPtr.Zero;
                }

                _foregroundDelegate = null;
                _focusDelegate = null;

                _savedHkl = IntPtr.Zero;
                _savedImeOpen = null;
                _isInBlockedApp = false;

                // Empty queue
                while (_actionQueue.TryDequeue(out _))
                {
                }
            }
        }
        private void ForegroundWinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd,
            int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (hwnd == IntPtr.Zero)
                return;

            EnqueueAction(() => HandleForegroundChange(hwnd));
        }

        private void FocusWinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd,
            int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (hwnd == IntPtr.Zero)
                return;

            EnqueueAction(() =>
            {
                try
                {
                    if (!_isInBlockedApp)
                        return;
                    if (!Win32.IsWindow(hwnd))
                        return;

                    if (IsBlockedWindow(hwnd))
                    {
                        ForceEnglish(hwnd);
                    }
                }
                catch (Exception ex)
                {
                }
            });
        }
        private void EnqueueAction(Action action)
        {
            if (action == null)
                return;

            _actionQueue.Enqueue(action);
            ProcessQueue();
        }

        private void ForceEnglish(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return;
            if (_englishHkl == IntPtr.Zero)
                return;

            try
            {
                Win32.SendMessage(hwnd, Win32.WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, _englishHkl);
            }
            catch (Exception ex)
            {

            }
        }

        private async void ProcessQueue()
        {
            if (Interlocked.CompareExchange(ref _isProcessingQueue, 1, 0) == 1)
                return;

            try
            {
                var actions = new List<Action>();
                while (_actionQueue.TryDequeue(out Action? action))
                {
                    actions.Add(action);
                }

                if (actions.Count == 0)
                    return;

                var lastAction = actions.Last();
                await Task.Run(() => lastAction());
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessingQueue, 0);

                if (!_actionQueue.IsEmpty)
                {
                    ProcessQueue();
                }
            }
        }

        private bool IsBlockedWindow(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return false;
            uint processId = Win32.GetWindowThreadProcessId(hwnd, out _);
            return IsBlockedProcess(processId);
        }

        private bool IsBlockedProcess(uint processId)
        {
            try
            {
                var proc = Process.GetProcessById((int)processId);
                var exe = proc.ProcessName + ".exe";
                return Config.TargetAppList.Any(x => x.AppName.ToLower() == exe.ToLower());
            }
            catch
            {
                return false;
            }
        }

        private void HandleForegroundChange(IntPtr hwnd)
        {
            try
            {
                lock (_hookSync)
                {
                    if (!Win32.IsWindow(hwnd))
                        return;

                    uint threadId = Win32.GetWindowThreadProcessId(hwnd, out uint processId);
                    bool isBlocked = IsBlockedProcess(processId);

                    if (isBlocked && !_isInBlockedApp)
                    {
                        if (_prevHwnd != IntPtr.Zero && !_prevWasBlocked)
                        {
                            SaveImeState(_prevHwnd);
                        }

                        ForceEnglish(hwnd);
                        _isInBlockedApp = true;
                    }

                    if (!isBlocked && _isInBlockedApp)
                    {
                        RestoreImeState(hwnd);
                        _isInBlockedApp = false;
                    }

                    if (isBlocked && _isInBlockedApp)
                    {
                        ForceEnglish(hwnd);
                    }

                    if (!isBlocked && !_isInBlockedApp && _savedHkl != IntPtr.Zero)
                    {
                        _savedHkl = IntPtr.Zero;
                        _savedImeOpen = null;
                    }

                    _prevHwnd = hwnd;
                    _prevThreadId = threadId;
                    _prevWasBlocked = isBlocked;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void SaveImeState(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return;

            try
            {
                var layout = GetWindowLayout(hwnd);
                if (layout != IntPtr.Zero)
                {
                    _savedHkl = layout;
                    _savedImeOpen = null;

                    var hImc = Win32.ImmGetContext(hwnd);
                    if (hImc != IntPtr.Zero)
                    {
                        _savedImeOpen = Win32.ImmGetOpenStatus(hImc);
                        Win32.ImmReleaseContext(hwnd, hImc);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void RestoreImeState(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return;
            if (_savedHkl == IntPtr.Zero)
                return;

            try
            {
                if (!Win32.IsWindow(hwnd))
                {
                    return;
                }

                Win32.SendMessage(hwnd, Win32.WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, _savedHkl);

                if (_savedImeOpen != null)
                {
                    var hImc = Win32.ImmGetContext(hwnd);
                    if (hImc != IntPtr.Zero)
                    {
                        Win32.ImmSetOpenStatus(hImc, _savedImeOpen.Value);
                        Win32.ImmReleaseContext(hwnd, hImc);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                _savedHkl = IntPtr.Zero;
                _savedImeOpen = null;
            }
        }
        private IntPtr GetWindowLayout(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return IntPtr.Zero;
            uint threadId = Win32.GetWindowThreadProcessId(hwnd, out _);
            return Win32.GetKeyboardLayout(threadId);
        }
        private static string NormalizeName(string name)
        {
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    return name;
            return name + ".exe";
        }
        #endregion

        private void AddAppNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AddOptionAppNameTextBlock.Text))
            {
                return;
            }
            Config.TargetAppList.Add(new()
            {
                AppName = NormalizeName(AddOptionAppNameTextBlock.Text)
            });
            AddOptionAppNameTextBlock.Text = string.Empty;
            Config.SaveConfig();
        }

        private void RemoveAppNameAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            TargetAppListCard.RemoveSelection();
        }
    }
}
