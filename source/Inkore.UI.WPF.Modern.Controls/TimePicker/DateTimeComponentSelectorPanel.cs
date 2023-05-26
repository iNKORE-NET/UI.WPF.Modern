using System.Windows.Controls;

namespace Inkore.UI.WPF.Modern.Controls
{
    public class DateTimeComponentSelectorPanel : VirtualizingStackPanel
    {
        public override void MouseWheelUp()
        {
            LineUp();
        }

        public override void MouseWheelDown()
        {
            LineDown();
        }
    }
}
