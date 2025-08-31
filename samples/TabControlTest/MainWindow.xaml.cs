using iNKORE.UI.WPF.Modern.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace TabControlTest;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = this;
        AddTestingMenuItems();

        tabControl.RegisterTabClosingEvent((menuControl, e) => 
        {
            if (e.Tab.Tag is not "ConfirmClose")
            {
                TabItems.Remove(e.Tab);
                return;
            }

            var msgResult = MessageBox.Show("Confirm closing?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (msgResult != MessageBoxResult.OK)
            {
                e.Cancel = true;
                return;
            }

            // when using ItemsSource, manually remove tab item.
            TabItems.Remove(e.Tab);
        });
    }

    public ObservableCollection<TabItem> TabItems { get; } = [];

    private void AddTestingMenuItems()
    {
        var item1 = new TabItem()
        {
            Header = "Closeable"
        };

        var item2 = new TabItem()
        {
            Header = "ConfirmToClose",
            Tag = "ConfirmClose"
        };

        TabItems.Add(item1);
        TabItems.Add(item2);
    }
}