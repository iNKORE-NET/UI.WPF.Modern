// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using iNKORE.UI.WPF.Modern.Controls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using iNKORE.UI.WPF.Modern.Gallery.Helpers;
using iNKORE.UI.WPF.Modern;

namespace iNKORE.UI.WPF.Modern.Gallery.Pages.Controls.Foundation
{
    /// <summary>
    /// Spacing page showcasing Windows spacing values and layout examples.
    /// </summary>
    public partial class SpacingPage : Page
    {
        private DispatcherTimer _themeMonitorTimer;
        private ElementTheme _lastKnownTheme = ElementTheme.Default;

        public SpacingPage()
        {
            this.InitializeComponent();
            Loaded += SpacingPage_Loaded;

            ThemeManager.Current.ActualApplicationThemeChanged += OnThemeChanged;
            ThemeManager.AddActualThemeChangedHandler(this, OnElementThemeChanged);

            _themeMonitorTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            _themeMonitorTimer.Tick += ThemeMonitorTimer_Tick;
            _themeMonitorTimer.Start();
        }

        private void SpacingPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationRootPage.Current?.NavigationView != null)
            {
                NavigationRootPage.Current.NavigationView.Header = "Spacing";
            }
            UpdateSpacingImages();
        }

        private void UpdateSpacingImages()
        {
            if (CardsImage == null || DialogImage == null) return;

            var pageTheme = ThemeManager.GetActualTheme(this);
            var parentTheme = ElementTheme.Default;

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

            var effectiveTheme = pageTheme != ElementTheme.Default ? pageTheme : parentTheme;
            var isDarkTheme = effectiveTheme == ElementTheme.Dark ||
                             (effectiveTheme == ElementTheme.Default && ThemeHelper.IsDarkTheme());

            var cardsImageName = isDarkTheme ? "Cards.dark.png" : "Cards.light.png";
            var dialogImageName = isDarkTheme ? "Dialog.dark.png" : "Dialog.light.png";

            var cardsUri = new Uri($"pack://application:,,,/iNKORE.UI.WPF.Modern.Gallery;component/Assets/Design/{cardsImageName}");
            var dialogUri = new Uri($"pack://application:,,,/iNKORE.UI.WPF.Modern.Gallery;component/Assets/Design/{dialogImageName}");

            try
            {
                var cardsBitmap = new BitmapImage();
                cardsBitmap.BeginInit();
                cardsBitmap.UriSource = cardsUri;
                cardsBitmap.CacheOption = BitmapCacheOption.OnLoad;
                cardsBitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                cardsBitmap.EndInit();
                cardsBitmap.Freeze();
                CardsImage.Source = cardsBitmap;

                var dialogBitmap = new BitmapImage();
                dialogBitmap.BeginInit();
                dialogBitmap.UriSource = dialogUri;
                dialogBitmap.CacheOption = BitmapCacheOption.OnLoad;
                dialogBitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                dialogBitmap.EndInit();
                dialogBitmap.Freeze();
                DialogImage.Source = dialogBitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load spacing images: {ex.Message}");
                // Fallback to dark images if there's an issue
                CardsImage.Source = new BitmapImage(new Uri("pack://application:,,,/iNKORE.UI.WPF.Modern.Gallery;component/Assets/Design/Cards.dark.png"));
                DialogImage.Source = new BitmapImage(new Uri("pack://application:,,,/iNKORE.UI.WPF.Modern.Gallery;component/Assets/Design/Dialog.dark.png"));
            }
        }

        private void OnThemeChanged(ThemeManager sender, object args)
        {
            UpdateSpacingImages();
        }

        private void OnElementThemeChanged(object sender, RoutedEventArgs e)
        {
            UpdateSpacingImages();
        }

        private void ThemeMonitorTimer_Tick(object sender, EventArgs e)
        {
            var currentTheme = ThemeManager.GetActualTheme(this);
            if (currentTheme != _lastKnownTheme)
            {
                _lastKnownTheme = currentTheme;
                UpdateSpacingImages();
                Debug.WriteLine($"Theme change detected: {currentTheme}");
            }
        }
    }
}
