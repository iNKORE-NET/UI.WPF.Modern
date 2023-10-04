using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace iNKORE.UI.WPF.Modern.Controls.Common
{
    public class CornerRadiusHalfConverter : DependencyObject, IValueConverter
    {
        public static DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(CornerRadiusHalfConverterMode), typeof(CornerRadiusHalfConverter), new PropertyMetadata(CornerRadiusHalfConverterMode.All));
        public CornerRadiusHalfConverterMode Mode
        {
            get { return (CornerRadiusHalfConverterMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }
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
                    case CornerRadiusHalfConverterMode.All:
                        return val;
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
        RightHalf,
        All
    }
}
