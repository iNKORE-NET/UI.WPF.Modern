using System.Windows;

namespace Inkore.UI.WPF.Modern.Controls
{
    public class GridViewHeaderItem : ListViewBaseHeaderItem
    {
        static GridViewHeaderItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridViewHeaderItem), new FrameworkPropertyMetadata(typeof(GridViewHeaderItem)));
        }

        public GridViewHeaderItem()
        {
        }
    }
}
