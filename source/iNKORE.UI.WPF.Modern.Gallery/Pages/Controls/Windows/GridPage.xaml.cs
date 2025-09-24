using iNKORE.UI.WPF.Modern.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Page = iNKORE.UI.WPF.Modern.Controls.Page;

namespace iNKORE.UI.WPF.Modern.Gallery.Pages.Controls.Windows
{
    /// <summary>
    /// GridPage.xaml 的交互逻辑
    /// </summary>
    public partial class GridPage : Page
    {
        public GridPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ControlExampleSubstitution Substitution1 = new ControlExampleSubstitution
            {
                Key = "Column",
            };
            BindingOperations.SetBinding(Substitution1, ControlExampleSubstitution.ValueProperty, new Binding
            {
                Source = ColumnSlider,
                Path = new PropertyPath("Value"),
            });
            ControlExampleSubstitution Substitution2 = new ControlExampleSubstitution
            {
                Key = "Row",
            };
            BindingOperations.SetBinding(Substitution2, ControlExampleSubstitution.ValueProperty, new Binding
            {
                Source = RowSlider,
                Path = new PropertyPath("Value"),
            });
            ControlExampleSubstitution Substitution3 = new ControlExampleSubstitution
            {
                Key = "ColumnSpacing",
            };
            BindingOperations.SetBinding(Substitution3, ControlExampleSubstitution.ValueProperty, new Binding
            {
                Source = ColumnSpacingSlider,
                Path = new PropertyPath("Value"),
            });
            ControlExampleSubstitution Substitution4 = new ControlExampleSubstitution
            {
                Key = "RowSpacing",
            };
            BindingOperations.SetBinding(Substitution4, ControlExampleSubstitution.ValueProperty, new Binding
            {
                Source = RowSpacingSlider,
                Path = new PropertyPath("Value"),
            });
            Example1.Substitutions = new ObservableCollection<ControlExampleSubstitution> { Substitution1, Substitution2, Substitution3, Substitution4 };

            UpdateExampleCode();
            UpdateGridSpacing();
        }

        private void ColumnSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.IsLoaded)
            {
                UpdateExampleCode();
                UpdateGridSpacing();
            }
        }

        private void RowSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.IsLoaded)
            {
                UpdateExampleCode();
                UpdateGridSpacing();
            }
        }

        private void ColumnSpacingSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.IsLoaded)
            {
                UpdateExampleCode();
                UpdateGridSpacing();
            }
        }

        private void RowSpacingSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.IsLoaded)
            {
                UpdateExampleCode();
                UpdateGridSpacing();
            }
        }

                private void UpdateGridSpacing()
        {
            if (Control1 == null) return;

            int maxCol = Math.Max(0, Control1.ColumnDefinitions.Count - 1);
            int maxRow = Math.Max(0, Control1.RowDefinitions.Count - 1);
            double colSpace = ColumnSpacingSlider?.Value ?? 0d;
            double rowSpace = RowSpacingSlider?.Value ?? 0d;

            void Apply(FrameworkElement el)
            {
                if (el == null) return;
                // For Rectangle1, ensure we use current slider values explicitly
                int row = el == Rectangle1 ? (int)(RowSlider?.Value ?? Grid.GetRow(el)) : Grid.GetRow(el);
                int col = el == Rectangle1 ? (int)(ColumnSlider?.Value ?? Grid.GetColumn(el)) : Grid.GetColumn(el);
                double left = col > 0 ? colSpace / 2 : 0;
                double right = col < maxCol ? colSpace / 2 : 0;
                double top = row > 0 ? rowSpace / 2 : 0;
                double bottom = row < maxRow ? rowSpace / 2 : 0;
                el.Margin = new Thickness(left, top, right, bottom);
            }

            Apply(Rectangle1);
            Apply(Rectangle2);
            Apply(Rectangle3);
            Apply(Rectangle4);
        }

        #region Example Code

        public void UpdateExampleCode()
        {
            Example1.Xaml = Example1Xaml;
        }

        public string Example1Xaml => $@"
<Grid
    Width=""240""
    Height=""160""
    Background=""Gray""
    ColumnDefinitions=""50, 50, 50""
    RowDefinitions=""50, 50, 50""
    ColumnSpacing=""$(ColumnSpacing)""
    RowSpacing=""$(RowSpacing)"">
    <Rectangle Fill=""Red"" Grid.Column=""$(Column)"" Grid.Row=""$(Row)"" Width=""50"" Height=""50"" />
    <Rectangle Fill=""Blue"" Grid.Row=""1"" Width=""50"" Height=""50"" />
    <Rectangle Fill=""Green"" Grid.Column=""1"" Width=""50"" Height=""50"" />
    <Rectangle Fill=""Yellow"" Grid.Column=""1"" Grid.Row=""1"" Width=""50"" Height=""50"" />
</Grid>
";
        #endregion
    }
}
