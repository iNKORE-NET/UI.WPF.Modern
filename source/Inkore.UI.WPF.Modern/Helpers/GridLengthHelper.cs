using System.Windows;

namespace Inkore.UI.WPF.Modern
{
    internal static class GridLengthHelper
    {
        public static GridLength FromPixels(double pixels)
        {
            return new GridLength(pixels);
        }

        public static GridLength FromValueAndType(double value, GridUnitType type)
        {
            return new GridLength(value, type);
        }
    }
}
