using iNKORE.UI.WPF.Modern.Controls.Primitives;
using iNKORE.UI.WPF.Modern.Helpers;
using iNKORE.UI.WPF.Helpers;
using iNKORE.UI.WPF.Modern.Helpers.Styles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace iNKORE.UI.WPF.Modern.Controls.Helpers
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
            if (!((BackdropType)e.NewValue).IsSupported())
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

        #region CornerStyle

        public static readonly DependencyProperty CornerStyleProperty =
            DependencyProperty.RegisterAttached(
                "CornerStyle",
                typeof(WindowCornerStyle),
                typeof(WindowHelper),
                new PropertyMetadata(OnCornerStyleChanged));

        private static void OnCornerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window)
                CornerHelper.SetWindowCorners((Window)d, (WindowCornerStyle)e.NewValue);
        }

        public static WindowCornerStyle GetCornerStyle(Window window)
        {
            return (WindowCornerStyle)window.GetValue(CornerStyleProperty);
        }

        public static void SetCornerStyle(Window window, WindowCornerStyle value)
        {
            window.SetValue(CornerStyleProperty, value);
        }


        #endregion

        #region ApplyBackground

        public static readonly DependencyProperty ApplyBackgroundProperty =
            DependencyProperty.RegisterAttached(
                "ApplyBackground",
                typeof(bool),
                typeof(WindowHelper),
                new PropertyMetadata(true));

        public static bool GetApplyBackground(Window window)
        {
            return (bool)window.GetValue(ApplyBackgroundProperty);
        }

        public static void SetApplyBackground(Window window, bool value)
        {
            window.SetValue(ApplyBackgroundProperty, value);
        }


        #endregion

        #region ApplyNoise

        public static readonly DependencyProperty ApplyNoiseProperty =
            DependencyProperty.RegisterAttached(
                "ApplyNoise",
                typeof(bool),
                typeof(WindowHelper),
                new PropertyMetadata(false));

        public static bool GetApplyNoise(Window window)
        {
            return (bool)window.GetValue(ApplyNoiseProperty);
        }

        public static void SetApplyNoise(Window window, bool value)
        {
            window.SetValue(ApplyNoiseProperty, value);
        }


        #endregion


        #region FixMaximizedWindow

        public static readonly DependencyProperty FixMaximizedWindowProperty =
            DependencyProperty.RegisterAttached(
                "FixMaximizedWindow",
                typeof(bool),
                typeof(WindowHelper),
                new PropertyMetadata(false, OnFixMaximizedWindowChanged));

        public static bool GetFixMaximizedWindow(Window window)
        {
            return (bool)window.GetValue(FixMaximizedWindowProperty);
        }

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

            void ApplyDarkMode()
            {
                var theme = ThemeManager.GetActualTheme(window);

                bool IsDark(ElementTheme theme)
                {
                    return theme == ElementTheme.Default
                        ? ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark
                        : theme == ElementTheme.Dark;
                }

                try
                {
                    if (IsDark(theme))
                    {
                        window.ApplyDarkMode();
                    }
                    else
                    {
                        window.RemoveDarkMode();
                    }
                }
                catch { }
            }

            var handler = new RoutedEventHandler((sender, e) => ApplyDarkMode());

            if (isModern)
            {
                ApplyDarkMode();

                if (window.IsLoaded)
                {
                    window.RemoveTitleBar();
                }
                else
                {
                    void RemoveTitleBar(object sender, RoutedEventArgs e)
                    {
                        window.RemoveTitleBar();
                    }

                    window.Loaded -= RemoveTitleBar;
                    window.Loaded += RemoveTitleBar;
                }

                ThemeManager.RemoveActualThemeChangedHandler(window, handler);
                ThemeManager.AddActualThemeChangedHandler(window, handler);

                if (isUseMica)
                {
                    var type = GetSystemBackdropType(window);
                    if (type.IsSupported())
                    {
                        isSetMica = true;
                        window.SetResourceReference(FrameworkElement.StyleProperty, MicaWindowStyleKey);
                    }
                }

                if (!isSetMica && isUseAcrylic)
                {
                    if (AcrylicHelper.IsAcrylicSupported())
                    {
                        isSetAcrylic = true;
                        window.SetResourceReference(FrameworkElement.StyleProperty, AcrylicWindowStyleKey);
                    }
                    else if (AcrylicHelper.IsSupported())
                    {
                        isSetAcrylic = true;
                        window.SetResourceReference(FrameworkElement.StyleProperty, AeroWindowStyleKey);
                    }
                }

                if (!isSetMica && !isSetAcrylic && isUseAero)
                {
                    if (new Version(6, 0) <= OSVersionHelper.OSVersion && OSVersionHelper.OSVersion < new Version(6, 2, 8824))
                    {
                        isSetAero = true;
                        window.SetResourceReference(FrameworkElement.StyleProperty, AeroWindowStyleKey);
                    }
                }

                if (!isSetMica && !isSetAcrylic && !isSetAero)
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
            }
            else
            {
                window.ClearValue(FrameworkElement.StyleProperty);
                window.RemoveDarkMode();
                ThemeManager.RemoveActualThemeChangedHandler(window, handler);
            }
        }
    }
}