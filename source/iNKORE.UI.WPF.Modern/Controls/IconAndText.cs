using iNKORE.UI.WPF.Controls;
using iNKORE.UI.WPF.Modern.Common.IconKeys;
using System.Windows;
using System.Windows.Controls;

namespace iNKORE.UI.WPF.Modern.Controls
{
    public class IconAndText : ContentControl
    {
        private FontIcon _fontIcon;

        static IconAndText()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconAndText), new FrameworkPropertyMetadata(typeof(IconAndText)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _fontIcon = GetTemplateChild("Icon") as FontIcon;
            UpdateIconElement();
        }

        #region Properties

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(FontIconData?),
                typeof(IconAndText),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                    OnIconChanged
                )
            );

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconAndText iconAndText)
            {
                iconAndText.UpdateIconElement();
            }
        }

        private void UpdateIconElement()
        {
            if (_fontIcon != null)
            {
                _fontIcon.Icon = Icon;
            }
        }

        public FontIconData? Icon
        {
            get { return (FontIconData?)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty SpacingProperty = SimpleStackPanel.SpacingProperty.AddOwner(typeof(IconAndText), new PropertyMetadata(6d));
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty = SimpleStackPanel.OrientationProperty.AddOwner(typeof(IconAndText), new PropertyMetadata(Orientation.Horizontal));
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(IconAndText), new PropertyMetadata(16d));
        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }


        #endregion
    }
}