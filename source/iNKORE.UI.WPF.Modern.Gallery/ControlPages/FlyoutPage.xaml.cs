using iNKORE.UI.WPF.Modern.Controls;
using iNKORE.UI.WPF.Modern.Controls.Primitives;
using System.Windows;

namespace iNKORE.UI.WPF.Modern.Gallery.ControlPages
{
    public partial class FlyoutPage : Page
    {
        public FlyoutPage()
        {
            InitializeComponent();
        }

        private void DeleteConfirmation_Click(object sender, RoutedEventArgs e)
        {
            Flyout f = FlyoutService.GetFlyout(Control1) as Flyout;
            if (f != null)
            {
                f.Hide();
            }
        }
    }
}
