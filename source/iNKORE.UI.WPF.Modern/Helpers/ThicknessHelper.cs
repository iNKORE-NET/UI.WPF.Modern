using System.Windows;

namespace iNKORE.UI.WPF.Modern.Helpers
{
    internal static class ThicknessHelper
    {
        public static Thickness FromLengths(double left, double top, double right, double bottom)
        {
            return new Thickness(left, top, right, bottom);
        }

        public static Thickness FromUniformLength(double uniformLength)
        {
            return new Thickness(uniformLength);
        }
    }
}
