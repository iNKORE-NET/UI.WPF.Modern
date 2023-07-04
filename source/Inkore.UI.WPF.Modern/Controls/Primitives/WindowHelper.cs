using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Inkore.UI.WPF.Modern.Controls.Primitives
{
    public static class WindowHelper
    {
        private const string DefaultWindowStyleKey = "DefaultWindowStyle";
        private const string AeroWindowStyleKey = "AeroWindowStyle";
        private const string AcrylicWindowStyleKey = "AcrylicWindowStyle";
        private const string MicaWindowStyleKey = "MicaWindowStyle";
        private const string SnapWindowStyleKey = "SnapWindowStyle";

        #region UseModernWindowStyle

        public static readonly DependencyProperty UseModernWindowStyleProperty =
            DependencyProperty.RegisterAttached(
                "UseModernWindowStyle",
                typeof(bool),
                typeof(WindowHelper),
                new PropertyMetadata(OnUseModernWindowStyleChanged));

        public static bool GetUseModernWindowStyle(Window window)
        {
            return (bool)window.GetValue(UseModernWindowStyleProperty);
        }

        public static void SetUseModernWindowStyle(Window window, bool value)
        {
            window.SetValue(UseModernWindowStyleProperty, value);
        }

        private static void OnUseModernWindowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;

            if (DesignerProperties.GetIsInDesignMode(d))
            {
                if (d is Control control)
                {
                    if (newValue)
                    {
                        if (control.TryFindResource(DefaultWindowStyleKey) is Style style)
                        {
                            var dStyle = new Style();

                            foreach (Setter setter in style.Setters)
                            {
                                if (setter.Property == Control.BackgroundProperty ||
                                    setter.Property == Control.ForegroundProperty)
                                {
                                    dStyle.Setters.Add(setter);
                                }
                            }

                            control.Style = dStyle;
                        }
                    }
                    else
                    {
                        control.ClearValue(FrameworkElement.StyleProperty);
                    }
                }
            }
            else
            {
                var window = (Window)d;
                SetWindowStyle(window);
            }
        }

        #endregion

        #region UseAeroBackdrop

        public static readonly DependencyProperty UseAeroBackdropProperty =
            DependencyProperty.RegisterAttached(
                "UseAeroBackdrop",
                typeof(bool),
                typeof(WindowHelper),
                new PropertyMetadata(OnUseAeroBackdropChanged));

        public static bool GetUseAeroBackdrop(Window window)
        {
            return (bool)window.GetValue(UseAeroBackdropProperty);
        }

        public static void SetUseAeroBackdrop(Window window, bool value)
        {
            window.SetValue(UseAeroBackdropProperty, value);
        }

        private static void OnUseAeroBackdropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (OSVersionHelper.OSVersion < new Version(6, 0) || new Version(6, 2, 8824) < OSVersionHelper.OSVersion)
            {
                return;
            }

            if (d is Window window)
            {
                SetWindowStyle(window);
            }
        }

        #endregion

        #region UseAcrylicBackdrop

        public static readonly DependencyProperty UseAcrylicBackdropProperty =
            DependencyProperty.RegisterAttached(
                "UseAcrylicBackdrop",
                typeof(bool),
                typeof(WindowHelper),
                new PropertyMetadata(OnUseAcrylicBackdropChanged));

        public static bool GetUseAcrylicBackdrop(Window window)
        {
            return (bool)window.GetValue(UseAcrylicBackdropProperty);
        }

        public static void SetUseAcrylicBackdrop(Window window, bool value)
        {
            window.SetValue(UseAcrylicBackdropProperty, value);
        }

        private static void OnUseAcrylicBackdropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!AcrylicHelper.IsSupported())
            {
                return;
            }

            if (d is Window window)
            {
                var handler = new RoutedEventHandler(async (sender, e) =>
                {
                    await Task.Delay(1);
                    AcrylicHelper.Apply(window);
                });

                SetWindowStyle(window);

                if ((bool)e.NewValue)
                {
                    AcrylicHelper.Apply(window);

                    if (!window.IsLoaded)
                    {
                        window.Loaded += (sender, e) => AcrylicHelper.Apply(window);
                    }

                    if (AcrylicHelper.IsAcrylicSupported())
                    {
                        ThemeManager.RemoveActualThemeChangedHandler(window, handler);
                        ThemeManager.AddActualThemeChangedHandler(window, handler);
                    }
                }
                else
                {
                    AcrylicHelper.Remove(window);
                    ThemeManager.RemoveActualThemeChangedHandler(window, handler);
                }
            }
        }

        #endregion

        #region UseMicaBackdrop
        public static class Methods
        {
            [Flags]
            public enum DWMWINDOWATTRIBUTE
            {
                DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
                DWMWA_SYSTEMBACKDROP_TYPE = 38
            }
            [DllImport("dwmapi.dll")]
            static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref int pvAttribute, int cbAttribute);

            public static int SetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, int parameter)
                => DwmSetWindowAttribute(hwnd, attribute, ref parameter, Marshal.SizeOf<int>());
        }
        public static void SetUseMicaBackdrop(Window window)
        {
            Methods.SetWindowAttribute(
                new WindowInteropHelper(window).Handle,
                Methods.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
                2);
            RefreshDarkMode(window);
            ThemeManager.Current.ActualApplicationThemeChanged += (s, ev) => RefreshDarkMode(window);
        }
        private static void RefreshDarkMode(Window window)
        {
            var isDark = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark;
            int flag = isDark ? 1 : 0;
            Methods.SetWindowAttribute(
                new WindowInteropHelper(window).Handle,
                Methods.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
                flag);
        }
        #endregion

        #region SystemBackdropType

        public static readonly DependencyProperty SystemBackdropTypeProperty =
            DependencyProperty.RegisterAttached(
                "SystemBackdropType",
                typeof(BackdropType),
                typeof(WindowHelper),
                new PropertyMetadata(OnSystemBackdropTypeChanged));

        public static BackdropType GetSystemBackdropType(Window window)
        {
            return (BackdropType)window.GetValue(SystemBackdropTypeProperty);
        }

        public static void SetSystemBackdropType(Window window, BackdropType value)
        {
            window.SetValue(SystemBackdropTypeProperty, value);
        }

        private static void OnSystemBackdropTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!MicaHelper.IsSupported((BackdropType)e.NewValue))
            {
                return;
            }

            if (d is Window window)
            {
                SetWindowStyle(window);
                MicaHelper.Apply(window, (BackdropType)e.NewValue);
            }
        }

        #endregion

        #region FixMaximizedWindow

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DependencyProperty FixMaximizedWindowProperty =
            DependencyProperty.RegisterAttached(
                "FixMaximizedWindow",
                typeof(bool),
                typeof(WindowHelper),
                new PropertyMetadata(false, OnFixMaximizedWindowChanged));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool GetFixMaximizedWindow(Window window)
        {
            return (bool)window.GetValue(FixMaximizedWindowProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetFixMaximizedWindow(Window window, bool value)
        {
            window.SetValue(FixMaximizedWindowProperty, value);
        }

        private static void OnFixMaximizedWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
            {
                if ((bool)e.NewValue)
                {
                    MaximizedWindowFixer.SetMaximizedWindowFixer(window, new MaximizedWindowFixer());
                }
                else
                {
                    window.ClearValue(MaximizedWindowFixer.MaximizedWindowFixerProperty);
                }
            }
        }

        #endregion

        public static void SetWindowStyle(Window window)
        {
            bool isModern = DependencyPropertyHelper.GetValueSource(window, UseModernWindowStyleProperty).BaseValueSource != BaseValueSource.Default && GetUseModernWindowStyle(window);
            bool isUseMica = DependencyPropertyHelper.GetValueSource(window, SystemBackdropTypeProperty).BaseValueSource != BaseValueSource.Default;
            bool isUseAcrylic = DependencyPropertyHelper.GetValueSource(window, UseAcrylicBackdropProperty).BaseValueSource != BaseValueSource.Default && GetUseAcrylicBackdrop(window);
            bool isUseAero = DependencyPropertyHelper.GetValueSource(window, UseAeroBackdropProperty).BaseValueSource != BaseValueSource.Default && GetUseAeroBackdrop(window);

            bool isSetMica = false;
            bool isSetAcrylic = false;
            bool isSetAero = false;

            if (isModern)
            {
                ApplyDarkMode(window);
                HandleTitleBar(window);

                if (isUseMica)
                {
                    isSetMica = SetMicaStyle(window);
                }

                if (!isSetMica && isUseAcrylic)
                {
                    isSetAcrylic = SetAcrylicStyle(window);
                }

                if (!isSetMica && !isSetAcrylic && isUseAero)
                {
                    isSetAero = SetAeroStyle(window);
                }

                if (!isSetMica && !isSetAcrylic && !isSetAero)
                {
                    SetDefaultStyle(window);
                }
            }
            else
            {
                ClearWindowStyle(window);
                RemoveDarkMode(window);
            }
        }

        private static void ApplyDarkMode(Window window)
        {
            var theme = ThemeManager.GetActualTheme(window);

            bool IsDark(ElementTheme theme)
            {
                return theme == ElementTheme.Default
                    ? ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark
                    : theme == ElementTheme.Dark;
            }

            if (IsDark(theme))
            {
                window.ApplyDarkMode();
            }
            else
            {
                window.RemoveDarkMode();
            }
        }

        private static void HandleTitleBar(Window window)
        {
            if (window.IsLoaded)
            {
                window.RemoveTitleBar();
            }
            else
            {
                window.Loaded -= RemoveTitleBar;
                window.Loaded += RemoveTitleBar;
            }
        }

        private static void RemoveTitleBar(object sender, RoutedEventArgs e)
        {
            var window = (Window)sender;
            window.RemoveTitleBar();
        }

        private static bool SetMicaStyle(Window window)
        {
            var type = GetSystemBackdropType(window);
            if (MicaHelper.IsSupported(type))
            {
                window.SetResourceReference(FrameworkElement.StyleProperty, MicaWindowStyleKey);
                return true;
            }
            return false;
        }

        private static bool SetAcrylicStyle(Window window)
        {
            if (AcrylicHelper.IsAcrylicSupported())
            {
                window.SetResourceReference(FrameworkElement.StyleProperty, AcrylicWindowStyleKey);
                return true;
            }
            else if (AcrylicHelper.IsSupported())
            {
                window.SetResourceReference(FrameworkElement.StyleProperty, AeroWindowStyleKey);
                return true;
            }
            return false;
        }

        private static bool SetAeroStyle(Window window)
        {
            if (new Version(6, 0) <= OSVersionHelper.OSVersion && OSVersionHelper.OSVersion < new Version(6, 2, 8824))
            {
                window.SetResourceReference(FrameworkElement.StyleProperty, AeroWindowStyleKey);
                return true;
            }
            return false;
        }

        private static void SetDefaultStyle(Window window)
        {
            if (OSVersionHelper.IsWindows11OrGreater)
            {
                window.SetResourceReference(FrameworkElement.StyleProperty, SnapWindowStyleKey);
            }
            else
            {
                window.SetResourceReference(FrameworkElement.StyleProperty, DefaultWindowStyleKey);
            }
        }

        private static void ClearWindowStyle(Window window)
        {
            window.ClearValue(FrameworkElement.StyleProperty);
        }

        private static void RemoveDarkMode(Window window)
        {
            window.RemoveDarkMode();
        }
    }
}
