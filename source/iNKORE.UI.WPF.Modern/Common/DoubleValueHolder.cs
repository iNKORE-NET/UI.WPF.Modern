using System.Windows;

namespace iNKORE.UI.WPF.Modern.Common
{
    public class DoubleValueHolder : UIElement
    {
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register(nameof(X), typeof(double), typeof(DoubleValueHolder), new PropertyMetadata(0.0));

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register(nameof(Y), typeof(double), typeof(DoubleValueHolder), new PropertyMetadata(0.0));

        public static readonly DependencyProperty ZProperty =
            DependencyProperty.Register(nameof(Z), typeof(double), typeof(DoubleValueHolder), new PropertyMetadata(0.0));

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public double Z
        {
            get { return (double)GetValue(ZProperty); }
            set { SetValue(ZProperty, value); }
        }
    }
}
