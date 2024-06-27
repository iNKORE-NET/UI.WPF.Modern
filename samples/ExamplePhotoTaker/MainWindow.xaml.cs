using ExamplePhotoTaker.Pages;
using iNKORE.UI.WPF.Modern.Media.Animation;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExamplePhotoTaker
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Do something when the button is clicked!
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog()
        {
            Filter = "PNG Picture|*.png",
            AddExtension = true,
        };


        private void Button_SaveScreenshot_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap targetBitmap = new RenderTargetBitmap(
             (int)Viewbox_Viewport.ActualWidth,
             (int)Viewbox_Viewport.ActualHeight,
             96d,
             96d,
             PixelFormats.Default);

            targetBitmap.Render(Viewbox_Viewport);


            if (saveFileDialog.ShowDialog() == true)
            {
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(targetBitmap));
                // save file to disk
                using (FileStream fs = File.Open(saveFileDialog.FileName, FileMode.OpenOrCreate))
                {
                    encoder.Save(fs);
                }
            }
        }

        Page1 Page_1 = new Page1();
        Page2 Page_2 = new Page2();

        private void Button_NavigateToPage1_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo(Page_1);
        }

        private void Button_NavigateToPage2_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo(Page_2);
        }

        WeakReference __Frame_Main_NavigatingTarget = new WeakReference(null);
        private void Frame_Main_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Content != __Frame_Main_NavigatingTarget.Target)
            {
                e.Cancel = true;
            }
        }

        public void NavigateTo(object content, object? extraData = null, NavigationTransitionInfo? infoOverride = null)
        {
            __Frame_Main_NavigatingTarget.Target = content;
            Frame_Main.Navigate(content, extraData, infoOverride);
        }
    }

    class VacationSpots : ObservableCollection<string>
    {
        public VacationSpots()
        {

            Add("Spain");
            Add("France");
            Add("Peru");
            Add("Mexico");
            Add("Italy");
        }
    }
}