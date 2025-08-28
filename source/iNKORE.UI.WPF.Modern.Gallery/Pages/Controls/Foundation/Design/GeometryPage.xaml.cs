// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using iNKORE.UI.WPF.Modern.Controls;
using System;
using System.Windows;
using iNKORE.UI.WPF.Modern.Gallery;
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
        public GeometryPage()
        {
            this.InitializeComponent();

            UpdateExampleCode();
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
        /// Helper method: if you want resource-copy buttons to also show the same animation,
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
