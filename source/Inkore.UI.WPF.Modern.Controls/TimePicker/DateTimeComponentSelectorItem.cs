using System.Windows;
using System.Windows.Controls;

namespace Inkore.UI.WPF.Modern.Controls
{
    public class DateTimeComponentSelectorItem : ListBoxItem
    {
        static DateTimeComponentSelectorItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimeComponentSelectorItem), new FrameworkPropertyMetadata(typeof(DateTimeComponentSelectorItem)));
        }
    }
}
