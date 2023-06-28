using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Inkore.UI.WPF.Modern.Controls.Common
{
    public class CornerRadiusHalfConverter : IValueConverter
    {
        public CornerRadiusHalfConverterMode Mode { get; set; } = CornerRadiusHalfConverterMode.UpperHalf;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                CornerRadius val = (CornerRadius)value;
                switch (Mode)
                {
                    case CornerRadiusHalfConverterMode.UpperHalf:
                        return new CornerRadius(val.TopLeft, val.TopRight, 0, 0);
                    case CornerRadiusHalfConverterMode.LowerHalf:
                        return new CornerRadius(0, 0, val.BottomRight, val.BottomLeft);
                    case CornerRadiusHalfConverterMode.LeftHalf:
                        return new CornerRadius(val.TopLeft, 0, 0, val.BottomLeft);
                    case CornerRadiusHalfConverterMode.RightHalf:
                        return new CornerRadius(0, val.TopRight, val.BottomRight, 0);
                    default:
                        return val;
                }
            }
            catch { return value; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public enum CornerRadiusHalfConverterMode
    {
        UpperHalf,
        LowerHalf,
        LeftHalf,
        RightHalf
    }
}
