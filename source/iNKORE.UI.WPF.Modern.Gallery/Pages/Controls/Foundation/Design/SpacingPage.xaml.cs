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
        public SpacingPage()
        {
            this.InitializeComponent();
            Loaded += SpacingPage_Loaded;
        }

        private void SpacingPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationRootPage.Current?.NavigationView != null)
            {
                NavigationRootPage.Current.NavigationView.Header = "Spacing";
            }
        }
    }
}
