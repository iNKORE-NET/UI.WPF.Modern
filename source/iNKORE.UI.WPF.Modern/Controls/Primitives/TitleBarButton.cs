using System.Windows;
using System.Windows.Controls;

/* 项目“iNKORE.UI.WPF.Modern (net452)”的未合并的更改
在此之前:
using System.Windows.Media;
在此之后:
using System.Windows.Media;
using iNKORE;
using iNKORE.UI;
using iNKORE.UI.WPF;
using iNKORE.UI.WPF.Modern;
using iNKORE.UI.WPF.Modern.Controls;
using iNKORE.UI.WPF.Modern.Controls;
using iNKORE.UI.WPF.Modern.Controls.TitleBar;
*/
using System.Windows.Media;

namespace iNKORE.UI.WPF.Modern.Controls.Primitives
{
    public class TitleBarButton : Button
    {
        static TitleBarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleBarButton),
                new FrameworkPropertyMetadata(typeof(TitleBarButton)));
        }

        #region IsActive

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(
                nameof(IsActive),
                typeof(bool),
                typeof(TitleBarButton),
                new PropertyMetadata(false));

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        #endregion

        #region InactiveBackground

        public static readonly DependencyProperty InactiveBackgroundProperty =
            DependencyProperty.Register(
                nameof(InactiveBackground),
                typeof(Brush),
                typeof(TitleBarButton),
                null);

        public Brush InactiveBackground
        {
            get => (Brush)GetValue(InactiveBackgroundProperty);
            set => SetValue(InactiveBackgroundProperty, value);
        }

        #endregion

        #region InactiveForeground

        public static readonly DependencyProperty InactiveForegroundProperty =
            DependencyProperty.Register(
                nameof(InactiveForeground),
                typeof(Brush),
                typeof(TitleBarButton),
                null);

        public Brush InactiveForeground
        {
            get => (Brush)GetValue(InactiveForegroundProperty);
            set => SetValue(InactiveForegroundProperty, value);
        }

        #endregion

        #region HoverBackground

        public Brush HoverBackground
        {
            get => (Brush)GetValue(HoverBackgroundProperty);
            set => SetValue(HoverBackgroundProperty, value);
        }

        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register(
            nameof(HoverBackground),
            typeof(Brush),
            typeof(TitleBarButton),
            null);

        #endregion

        #region HoverForeground

        public Brush HoverForeground
        {
            get => (Brush)GetValue(HoverForegroundProperty);
            set => SetValue(HoverForegroundProperty, value);
        }

        public static readonly DependencyProperty HoverForegroundProperty = DependencyProperty.Register(
            nameof(HoverForeground),
            typeof(Brush),
            typeof(TitleBarButton),
            null);

        #endregion

        #region PressedBackground

        public Brush PressedBackground
        {
            get => (Brush)GetValue(PressedBackgroundProperty);
            set => SetValue(PressedBackgroundProperty, value);
        }

        public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register(
            nameof(PressedBackground),
            typeof(Brush),
            typeof(TitleBarButton),
            null);

        #endregion

        #region PressedForeground

        public Brush PressedForeground
        {
            get => (Brush)GetValue(PressedForegroundProperty);
            set => SetValue(PressedForegroundProperty, value);
        }

        public static readonly DependencyProperty PressedForegroundProperty = DependencyProperty.Register(
            nameof(PressedForeground),
            typeof(Brush),
            typeof(TitleBarButton),
            null);

        #endregion
    }
}
