// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using iNKORE.UI.WPF.Modern.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using iNKORE.UI.WPF.Modern.Gallery.Helpers;
using iNKORE.UI.WPF.Modern;

namespace iNKORE.UI.WPF.Modern.Gallery.Pages.Controls.Foundation
{
    /// <summary>
    /// Typography page showcasing Windows typography styles and system fonts.
    /// </summary>
    public sealed partial class TypographyPage : Page
    {
        public TypographyPage()
        {
            this.InitializeComponent();
            Loaded += TypographyPage_Loaded;

            // The header image sits inside the ControlExample, whose theme the gallery's toggle theme button
            // flips on its own, so watching the theme of the page is not enough. ActualTheme is kept in sync on
            // every element of the tree, so watching the image itself catches the toggle and application level
            // theme changes alike. No need to detach: the handler belongs to the page the image is part of
            ThemeManager.AddActualThemeChangedHandler(TypographyHeaderImage, OnImageActualThemeChanged);
        }

        private void TypographyPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationRootPage.Current?.NavigationView != null)
            {
                NavigationRootPage.Current.NavigationView.Header = "Typography";
            }

            UpdateTypographyImage();

            UpdateExampleCode();
        }

        private void OnImageActualThemeChanged(object sender, RoutedEventArgs e)
        {
            UpdateTypographyImage();
        }

        private void UpdateTypographyImage()
        {
            if (TypographyHeaderImage == null) return;

            var isDarkTheme = ThemeManager.GetActualTheme(TypographyHeaderImage) == ElementTheme.Dark;

            var imageName = isDarkTheme ? "Typography.dark.png" : "Typography.light.png";
            var uri = new System.Uri($"pack://application:,,,/iNKORE.UI.WPF.Modern.Gallery;component/Assets/Design/{imageName}");
            
            try
            {
                // Force refresh the BitmapImage to ensure it updates
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = uri;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                TypographyHeaderImage.Source = bitmapImage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load typography image: {ex.Message}");
                // Fallback to dark image if there's an issue
                var fallbackUri = new System.Uri("pack://application:,,,/iNKORE.UI.WPF.Modern.Gallery;component/Assets/Design/Typography.dark.png");
                TypographyHeaderImage.Source = new BitmapImage(fallbackUri);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        // Button click handlers for typography info buttons
        private void ShowTypographyButtonClick1(object sender, RoutedEventArgs e)
        {
            // Caption button clicked - could show teaching tip
            // TeachingTip not available yet, so using simple message for now
            System.Windows.MessageBox.Show("Caption: Small, Regular - 12/16 epx\nStyle: CaptionTextBlockStyle", "Typography Info", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void ShowTypographyButtonClick2(object sender, RoutedEventArgs e)
        {
            // Body button clicked
            System.Windows.MessageBox.Show("Body: Text, Regular - 14/20 epx\nStyle: BodyTextBlockStyle", "Typography Info", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void ShowTypographyButtonClick3(object sender, RoutedEventArgs e)
        {
            // Body Strong button clicked
            System.Windows.MessageBox.Show("Body Strong: Text, SemiBold - 14/20 epx\nStyle: BodyStrongTextBlockStyle", "Typography Info", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void ShowTypographyButtonClick4(object sender, RoutedEventArgs e)
        {
            // Title button clicked
            System.Windows.MessageBox.Show("Title: Display, SemiBold - 28/36 epx\nStyle: TitleTextBlockStyle", "Typography Info", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void ShowTypographyButtonClick5(object sender, RoutedEventArgs e)
        {
            // Display button clicked
            System.Windows.MessageBox.Show("Display: Display, SemiBold - 68/92 epx\nStyle: DisplayTextBlockStyle", "Typography Info", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void UpdateExampleCode()
        {
            Example1.Xaml = Example1Xaml;
        }

        string Example1Xaml => @"
<TextBlock Text=""Caption"" Style=""{StaticResource {x:Static ui:ThemeKeys.CaptionTextBlockStyleKey}}""/>
<TextBlock Text=""Body"" Style=""{StaticResource {x:Static ui:ThemeKeys.BodyTextBlockStyleKey}}""/>
<TextBlock Text=""Body Strong"" Style=""{StaticResource {x:Static ui:ThemeKeys.BodyStrongTextBlockStyleKey}}""/>
<TextBlock Text=""Subtitle"" Style=""{StaticResource {x:Static ui:ThemeKeys.SubtitleTextBlockStyleKey}}""/>
<TextBlock Text=""Title"" Style=""{StaticResource {x:Static ui:ThemeKeys.TitleTextBlockStyleKey}}""/>
<TextBlock Text=""Title Large"" Style=""{StaticResource {x:Static ui:ThemeKeys.TitleLargeTextBlockStyleKey}}""/>
<TextBlock Text=""Display"" Style=""{StaticResource {x:Static ui:ThemeKeys.DisplayTextBlockStyleKey}}""/>
";
    }
}
