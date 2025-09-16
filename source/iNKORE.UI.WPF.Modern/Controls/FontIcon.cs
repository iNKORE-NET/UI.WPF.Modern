using iNKORE.UI.WPF.Modern.Common;
using iNKORE.UI.WPF.Modern.Common.IconKeys;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace iNKORE.UI.WPF.Modern.Controls
{
    /// <summary>
    /// Represents an icon that uses a glyph from the specified font.
    /// </summary>
    public class FontIcon : IconElement, IFontIconClass
    {
        public const string SegoeIconsFontFamilyName = "Segoe Fluent Icons,Segoe MDL2 Assets,Segoe UI Symbol";


        /// <summary>
        /// Initializes a new instance of the FontIcon class.F
        /// </summary>
        public FontIcon()
        {
        }

        public FontIcon(FontIconData icon) : this()
        {
            Icon = icon;
        }

        public FontIcon(string glyph, FontFamily fontFamily) : this()
        {
            Glyph = glyph;
            if(fontFamily != null)
            {
                FontFamily = fontFamily;
            }
        }


        /// <summary>
        /// The identifier for the FontFamily dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(
                nameof(FontFamily),
                typeof(FontFamily),
                typeof(FontIcon),
                new FrameworkPropertyMetadata(
                    new FontFamily(SegoeIconsFontFamilyName),
                    OnFontFamilyChanged));

        /// <summary>
        /// Gets or sets the font used to display the icon glyph.
        /// </summary>
        /// <returns>The font used to display the icon glyph.</returns>
        [Bindable(true), Category("Appearance")]
        [Localizability(LocalizationCategory.Font)]
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fontIcon = (FontIcon)d;
            if (fontIcon._textBlock != null)
            {
                fontIcon._textBlock.FontFamily = (FontFamily)e.NewValue;
            }

            FontIconSource.UpdateIconData(fontIcon, false);
        }

        /// <summary>
        /// The identifier for the FontSize dependency property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(
                nameof(FontSize),
                typeof(double),
                typeof(FontIcon),
                new FrameworkPropertyMetadata(16d, OnFontSizeChanged));

        /// <summary>
        /// Gets or sets the size of the icon glyph.
        /// </summary>
        /// <returns>A non-negative value that specifies the font size, measured in pixels.</returns>
        [TypeConverter(typeof(FontSizeConverter))]
        [Bindable(true), Category("Appearance")]
        [Localizability(LocalizationCategory.None)]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fontIcon = (FontIcon)d;
            if (fontIcon._textBlock != null)
            {
                fontIcon._textBlock.FontSize = (double)e.NewValue;
            }
        }

        /// <summary>
        /// The identifier for the FontStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.Register(
                nameof(FontStyle),
                typeof(FontStyle),
                typeof(FontIcon),
                new FrameworkPropertyMetadata(FontStyles.Normal, OnFontStyleChanged));

        /// <summary>
        /// Gets or sets the font style for the icon glyph.
        /// </summary>
        /// <returns>
        /// A named constant of the enumeration that specifies the style in which the icon
        /// glyph is rendered. The default is **Normal**.
        /// </returns>
        [Bindable(true), Category("Appearance")]
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        private static void OnFontStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fontIcon = (FontIcon)d;
            if (fontIcon._textBlock != null)
            {
                fontIcon._textBlock.FontStyle = (FontStyle)e.NewValue;
            }
        }

        /// <summary>
        /// The identifier for the FontWeight dependency property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register(
                nameof(FontWeight),
                typeof(FontWeight),
                typeof(FontIcon),
                new FrameworkPropertyMetadata(FontWeights.Normal, OnFontWeightChanged));

        /// <summary>
        /// Gets or sets the thickness of the icon glyph.
        /// </summary>
        /// <returns>
        /// A value that specifies the thickness of the icon glyph. The default is **Normal**.
        /// </returns>
        [Bindable(true), Category("Appearance")]
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        private static void OnFontWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fontIcon = (FontIcon)d;
            if (fontIcon._textBlock != null)
            {
                fontIcon._textBlock.FontWeight = (FontWeight)e.NewValue;
            }
        }

        /// <summary>
        /// The identifier for the Glyph dependency property.
        /// </summary>
        public static readonly DependencyProperty GlyphProperty =
            DependencyProperty.Register(
                nameof(Glyph),
                typeof(string),
                typeof(FontIcon),
                new FrameworkPropertyMetadata(string.Empty, OnGlyphChanged));

        /// <summary>
        /// Gets or sets the character code that identifies the icon glyph.
        /// </summary>
        /// <returns>The hexadecimal character code for the icon glyph.</returns>
        public string Glyph
        {
            get => (string)GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Icon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(FontIconData?),
                typeof(FontIcon),
                new PropertyMetadata(null, (d, e) => FontIconSource.UpdateIconData(d as IFontIconClass, true)));

        /// <summary>
        /// Gets or sets the wrapped icon, which includes <see cref="Glyph"/> and <see cref="FontFamily"/>. You can get these instances from <see cref="iNKORE.UI.WPF.Modern.Common.IconKeys"/> namespace.
        /// If you are using Glyph and FontFamily property, this can be ignored.
        /// </summary>
        public FontIconData? Icon
        {
            get => (FontIconData?)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }


        private static void OnGlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fontIcon = (FontIcon)d;
            if (fontIcon._textBlock != null)
            {
                fontIcon._textBlock.Text = (string)e.NewValue;
            }

            FontIconSource.UpdateIconData(fontIcon, false);
        }

        private protected override void InitializeChildren()
        {
            _textBlock = new TextBlock
            {
                Style = null,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                FontFamily = FontFamily,
                FontSize = FontSize,
                FontStyle = FontStyle,
                FontWeight = FontWeight,
                Text = Glyph
            };
            // Instead of manually copying brushes, just bind. WPF inheritance will supply parent Foreground
            // when our own Foreground is default; explicit user-set Foreground flows through.
            _textBlock.SetBinding(TextBlock.ForegroundProperty, new System.Windows.Data.Binding
            {
                Path = new PropertyPath(nameof(Foreground)),
                Source = this
            });

            Children.Add(_textBlock);
        }

        // Foreground propagation now handled purely via binding; overrides no longer necessary.
        // Legacy foreground inheritance logic (kept for reference only):
        //
        // private protected override void OnShouldInheritForegroundFromVisualParentChanged()
        // {
        //     if (_textBlock != null)
        //     {
        //         if (ShouldInheritForegroundFromVisualParent)
        //         {
        //             _textBlock.Foreground = VisualParentForeground;
        //         }
        //         else
        //         {
        //             _textBlock.ClearValue(TextBlock.ForegroundProperty);
        //             // Apply our own Foreground if explicitly set.
        //             if (Foreground != null)
        //             {
        //                 _textBlock.Foreground = Foreground;
        //             }
        //         }
        //     }
        // }
        //
        // private protected override void OnVisualParentForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
        // {
        //     if (ShouldInheritForegroundFromVisualParent && _textBlock != null)
        //     {
        //         _textBlock.Foreground = (Brush)args.NewValue;
        //     }
        // }
        //
        // private protected override void OnOwnForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
        // {
        //     if (!ShouldInheritForegroundFromVisualParent && _textBlock != null)
        //     {
        //         // When we're not inheriting from visual parent, push our Foreground to the text block.
        //         _textBlock.Foreground = (Brush)args.NewValue;
        //     }
        // }
        //

        private TextBlock _textBlock;


        protected override IconSource CreateIconSourceCore()
        {
            var iconSource = new FontIconSource();

            iconSource.Glyph = Glyph;
            iconSource.FontSize = FontSize;
            var newForeground = Foreground;
            if (newForeground != null)
            {
                iconSource.Foreground = newForeground;
            }

            if (FontFamily == null)
            {
                FontFamily = new FontFamily(SegoeIconsFontFamilyName);
            }
            iconSource.FontFamily = FontFamily;

            iconSource.FontWeight = FontWeight;
            iconSource.FontStyle = FontStyle;

            return iconSource;

        }
    }
}
