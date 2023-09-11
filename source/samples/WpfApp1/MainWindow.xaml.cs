using Inkore.UI.WPF.Modern;
using Inkore.UI.WPF.Modern.Controls;
using Inkore.UI.WPF.Modern.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            selec();
        }

        private async void selec()
        {
            ContentDialog dialog = new ContentDialog();
            dialog.PrimaryButtonText = "OK";
            dialog.SecondaryButtonText = "Cancel";
            await dialog.ShowAsync();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selec();

        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (ThemeManager.Current.ApplicationTheme != ApplicationTheme.Dark)
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            else
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            }

            //WindowHelper.ApplyDarkMode(this);
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //naview.SelectedItem = sgsac;
        }
    }
}
