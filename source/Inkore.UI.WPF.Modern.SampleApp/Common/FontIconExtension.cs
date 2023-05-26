using Inkore.UI.WPF.Modern.Controls;
using System;
using System.Windows.Markup;

namespace Inkore.UI.WPF.Modern.SampleApp.Common
{
    [MarkupExtensionReturnType(typeof(FontIcon))]
    public class FontIconExtension : MarkupExtension
    {
        public FontIconExtension()
        {
        }

        public FontIconExtension(string glyph)
        {
            Glyph = glyph;
        }

        [ConstructorArgument("glyph")]
        public string Glyph { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new FontIcon
            {
                Glyph = Glyph
            };
        }
    }
}
