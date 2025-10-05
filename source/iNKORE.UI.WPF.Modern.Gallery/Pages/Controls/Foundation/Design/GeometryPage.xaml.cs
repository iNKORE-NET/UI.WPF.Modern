// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using iNKORE.UI.WPF.Modern.Controls;
using System;
using System.Windows;
using iNKORE.UI.WPF.Modern.Gallery;
using iNKORE.UI.WPF.Modern;
using System.Windows.Controls;
using Page = iNKORE.UI.WPF.Modern.Controls.Page;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;
using System.Threading.Tasks;

namespace iNKORE.UI.WPF.Modern.Gallery.Pages.Controls.Foundation
{
    /// <summary>
    /// Geometry page showcasing Windows geometry values and layout examples.
    /// </summary>
    public partial class GeometryPage : Page
    {
    private ElementTheme _lastKnownTheme = ElementTheme.Default;

        public GeometryPage()
        {
            this.InitializeComponent();

            Loaded += GeometryPage_Loaded;

            iNKORE.UI.WPF.Modern.ThemeManager.Current.ActualApplicationThemeChanged += OnThemeChanged;
            iNKORE.UI.WPF.Modern.ThemeManager.AddActualThemeChangedHandler(this, OnElementThemeChanged);

            System.ComponentModel.DependencyPropertyDescriptor.FromProperty(iNKORE.UI.WPF.Modern.ThemeManager.RequestedThemeProperty, typeof(FrameworkElement))
                ?.AddValueChanged(this, OnRequestedThemeChanged);

            var _themeMonitorTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            _themeMonitorTimer.Tick += ThemeMonitorTimer_Tick;
            _themeMonitorTimer.Start();

            UpdateExampleCode();
        }

        private void GeometryPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateGeometryImage();
        }

        private void UpdateGeometryImage()
        {
            if (GeometryImage == null) return;

            var pageTheme = ThemeManager.GetActualTheme(this);
            var parentTheme = ElementTheme.Default;
            var controlExampleTheme = ElementTheme.Default;

            var parentElement = this.Parent as FrameworkElement;
            while (parentElement != null)
            {
                var currentParentTheme = ThemeManager.GetActualTheme(parentElement);
                if (currentParentTheme != ElementTheme.Default)
                {
                    parentTheme = currentParentTheme;
                    break;
                }
                parentElement = parentElement.Parent as FrameworkElement;
            }

            if (Example1 != null)
            {
                try
                {
                    var exampleTheme = ThemeManager.GetActualTheme(Example1);
                    if (exampleTheme != ElementTheme.Default)
                    {
                        controlExampleTheme = exampleTheme;
                    }
                    else if (Example1.ExampleContainer != null)
                    {
                        var containerTheme = ThemeManager.GetActualTheme(Example1.ExampleContainer);
                        if (containerTheme != ElementTheme.Default)
                        {
                            controlExampleTheme = containerTheme;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Theme detection error: {ex.Message}");
                }
            }

            var effectiveTheme = controlExampleTheme != ElementTheme.Default ? controlExampleTheme :
                                 pageTheme != ElementTheme.Default ? pageTheme : parentTheme;
            var isDarkTheme = effectiveTheme == ElementTheme.Dark ||
                             (effectiveTheme == ElementTheme.Default && iNKORE.UI.WPF.Modern.Gallery.Helpers.ThemeHelper.IsDarkTheme());

            var imageName = isDarkTheme ? "Geometry.dark.png" : "Geometry.light.png";
            var uri = new System.Uri($"pack://application:,,,/iNKORE.UI.WPF.Modern.Gallery;component/Assets/Design/{imageName}");

            try
            {
                var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = uri;
                bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmapImage.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.IgnoreImageCache;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                GeometryImage.Source = bitmapImage;
                System.Diagnostics.Debug.WriteLine($"Geometry image updated to: {imageName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load geometry image: {ex.Message}");
                var fallbackUri = new System.Uri("pack://application:,,,/iNKORE.UI.WPF.Modern.Gallery;component/Assets/Design/Geometry.dark.png");
                GeometryImage.Source = new System.Windows.Media.Imaging.BitmapImage(fallbackUri);
            }
        }

        private void OnThemeChanged(iNKORE.UI.WPF.Modern.ThemeManager sender, object args)
        {
            UpdateGeometryImage();
        }

        private void OnElementThemeChanged(object sender, RoutedEventArgs e)
        {
            UpdateGeometryImage();
        }

        private void ThemeMonitorTimer_Tick(object sender, EventArgs e)
        {
            var currentTheme = iNKORE.UI.WPF.Modern.ThemeManager.GetActualTheme(this);
            if (currentTheme != _lastKnownTheme)
            {
                _lastKnownTheme = currentTheme;
                UpdateGeometryImage();
                System.Diagnostics.Debug.WriteLine($"Theme change detected: {currentTheme}");
            }

            var parentElement = this.Parent as FrameworkElement;
            while (parentElement != null)
            {
                var parentTheme = ThemeManager.GetActualTheme(parentElement);
                if (parentTheme != currentTheme)
                {
                    UpdateGeometryImage();
                    System.Diagnostics.Debug.WriteLine($"Element-level theme change detected: Parent={parentTheme}, Current={currentTheme}");
                    break;
                }
                parentElement = parentElement.Parent as FrameworkElement;
            }

            if (Example1 != null)
            {
                try
                {
                    var controlExampleTheme = ThemeManager.GetActualTheme(Example1);
                    var containerTheme = ElementTheme.Default;

                    if (Example1.ExampleContainer != null)
                    {
                        containerTheme = ThemeManager.GetActualTheme(Example1.ExampleContainer);
                    }

                    if (controlExampleTheme != currentTheme || containerTheme != currentTheme)
                    {
                        UpdateGeometryImage();
                        System.Diagnostics.Debug.WriteLine($"ControlExample theme change detected: ControlExample={controlExampleTheme}, Container={containerTheme}, Page={currentTheme}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Theme monitor error: {ex.Message}");
                }
            }
        }

        private void OnRequestedThemeChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new System.Action(() => {
                UpdateGeometryImage();
            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);

            var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            timer.Tick += (s, args) => {
                timer.Stop();
                UpdateGeometryImage();
            };
            timer.Start();
        }

        // kept for backward compatibility if we want to programmatically set usage
        public string Usage { get; set; }

        /// <summary>
        /// Unified handler for resource-copy buttons that use the Tag property to store copy text.
        /// (OverlayCornerRadiusCopyButton and ControlCornerRadiusCopyButton call this.)
        /// </summary>
        private async void CopyControlCornerRadiusButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Use GoToElementState for FrameworkElement-based VSM.
                bool started = VisualStateManager.GoToElementState(LayoutRoot, "ControlCornerRadiusCopyButtonVisible", true);
                if (!started)
                {
                    // MessageBox.Show("Could not find visual state 'ControlCornerRadiusCopyButtonVisible' on LayoutRoot.", "VSM", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                if (sender is Button btn && btn.Tag is string tagText)
                {
                    Clipboard.SetText(tagText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Unable to Perform Copy", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            await Task.Delay(1000);

            try
            {
                VisualStateManager.GoToElementState(LayoutRoot, "ControlCornerRadiusCopyButtonHidden", true);
            }
            catch
            {
                // swallow
            }
        }

        /// <summary>
        /// Helper method: if we want resource-copy buttons to also show the same animation,
        /// call this. Left public for optional reuse.
        /// </summary>
        public async Task ShowCopyConfirmationAnimationAsync()
        {
            try
            {
                VisualStateManager.GoToElementState(LayoutRoot, "ConfirmationDialogVisible", true);
            }
            catch { }

            await Task.Delay(1000);

            try
            {
                VisualStateManager.GoToElementState(LayoutRoot, "ConfirmationDialogHidden", true);
            }
            catch { }
        }

        private async void OverlayCornerRadiusCopyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool started = VisualStateManager.GoToElementState(LayoutRoot, "OverlayCornerRadiusCopyButtonVisible", true);
                if (!started)
                {
                    // MessageBox.Show("Could not find visual state 'OverlayCornerRadiusCopyButtonVisible' on LayoutRoot.", "VSM", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                if (sender is Button btn && btn.Tag is string tagText)
                {
                    Clipboard.SetText(tagText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Unable to Perform Copy", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            await Task.Delay(1000);

            try
            {
                VisualStateManager.GoToElementState(LayoutRoot, "OverlayCornerRadiusCopyButtonHidden", true);
            }
            catch
            {
                // swallow
            }
        }

        private void ShowGeometryButtonClick1(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var flyout = FlyoutService.GetFlyout(button);
                if (flyout != null)
                {
                    flyout.ShowAt(button);
                }
            }
        }

        private void ShowGeometryButtonClick2(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var flyout = FlyoutService.GetFlyout(button);
                if (flyout != null)
                {
                    flyout.ShowAt(button);
                }
            }
        }

        private void ShowGeometryButtonClick3(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var flyout = FlyoutService.GetFlyout(button);
                if (flyout != null)
                {
                    flyout.ShowAt(button);
                }
            }
        }

        private void UpdateExampleCode()
        {
            Example1.Xaml = Example1Xaml;
        }
        
         string Example1Xaml => @"
<Grid CornerRadius=""{StaticResource OverlayCornerRadius}""/>
<Grid CornerRadius=""{StaticResource ControlCornerRadius}""/>";

    }
}
