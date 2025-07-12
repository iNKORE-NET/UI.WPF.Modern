using System;
using System.Globalization;
using System.Windows.Data;

namespace iNKORE.UI.WPF.Modern.Common.Converters
{
    public class MultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var first = values[0];

            if (first is not double result)
            {
                return values;
            }

            for (int index = 1; index < values.Length; index++)
            {
                if (values[index] is double current)
                {
                    result *= current;
                }
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}