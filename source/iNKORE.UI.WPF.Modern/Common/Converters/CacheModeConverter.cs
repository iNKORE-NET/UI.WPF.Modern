using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace iNKORE.UI.WPF.Modern.Common.Converters
{
    /// <summary>
    /// Converter that returns null when ShadowAssist.UseBitmapCache is false.
    /// This prevents freezing issues in multi-window scenarios.
    /// </summary>
    public class CacheModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If UseBitmapCache is disabled, return null to prevent CacheMode from being set
            if (!ShadowAssist.UseBitmapCache)
            {
                return null;
            }

            // Otherwise, pass through the CacheMode value
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
