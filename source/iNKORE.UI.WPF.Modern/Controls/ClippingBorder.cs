using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using iNKORE.UI.WPF.Modern.Common.Converters;
using iNKORE.UI.WPF.Converters;

namespace iNKORE.UI.WPF.Modern.Controls
{
    /// <Remarks>
    ///     As a side effect ClippingBorder will surpress any databinding or animation of 
    ///         its childs UIElement.Clip property until the child is removed from ClippingBorder
    /// </Remarks>
    public class ClippingBorder : Border
    {
        static ClippingBorder()
        {
            CornerRadiusProperty.OverrideMetadata(typeof(ClippingBorder), new FrameworkPropertyMetadata(new CornerRadius(0), CornerRadiusProperty_ValueChanged));
        }

        private static void CornerRadiusProperty_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ClippingBorder)?.ApplyClip();
        }

        public ClippingBorder()
        {
            this.SizeChanged += ClippingBorder_SizeChanged;
            ApplyClip();
        }

        private void ClippingBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplyClip();
        }

        public void ApplyClip()
        {
            try
            {
                _clipRect = ClipInnerRectConverter.DrawRoundedRectangle(new Rect(this.RenderSize), CornerRadius);
                if (_clipRect != null)
                {
                    this.Clip = _clipRect;
                }
            }
            catch { }
        }

        private StreamGeometry _clipRect;
    }
}
