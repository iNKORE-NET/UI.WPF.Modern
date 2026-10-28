using System.Windows;
using System.Windows.Controls;
using iNKORE.UI.WPF.Modern.Controls;

namespace SearchableComboBoxExample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Approach 1 - MVVM: the view model is the only thing the window knows about.
            DataContext = new MainViewModel();

            // Approach 2 - code-behind: fill and configure the control directly.
            ProductCombo.ItemsSource = Product.CreateMany(5000);
            ProductCombo.ItemFilter = MatchNameOrCode;
        }

        /// <summary>
        /// The built-in rule is one case-insensitive substring test over the displayed text.
        /// ItemFilter replaces it - here to match the code as well as the name.
        /// </summary>
        private static bool MatchNameOrCode(object item, string searchText)
        {
            var product = (Product)item;

            return product.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                || product.Code.Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }

        private void ProductCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductOutput.Text = ProductCombo.SelectedItem is Product product
                ? $"Code-behind holds: {product}"
                : "Nothing selected.";
        }
    }
}
