using iNKORE.UI.WPF.Modern.Controls;
using System.Windows;
using System.Windows.Controls;
using Page = iNKORE.UI.WPF.Modern.Controls.Page;

namespace iNKORE.UI.WPF.Modern.Gallery.ControlPages
{
    public partial class ContentDialogPage : Page
    {
        public ContentDialogPage()
        {
            InitializeComponent();
        }

        private async void ShowDialog_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Save your work?";
            dialog.PrimaryButtonText = "Save";
            dialog.SecondaryButtonText = "Don't Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new ContentDialogContent();

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                DialogResult.Text = "User saved their work";
            }
            else if (result == ContentDialogResult.Secondary)
            {
                DialogResult.Text = "User did not save their work";
            }
            else
            {
                DialogResult.Text = "User cancelled the dialog";
            }
        }
    }
}
