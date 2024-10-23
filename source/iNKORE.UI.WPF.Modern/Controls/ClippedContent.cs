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
using iNKORE.UI.WPF.Modern.Controls.Helpers;
using iNKORE.UI.WPF.Modern.Common;

namespace iNKORE.UI.WPF.Modern.Controls
{
    /// <summary>
    /// This is similar to a Border control, but it clips the content to the border's rounded corners.
    /// This operation is more expensive than a regular Border because a normal Decorator can't do this, so two nested Borders are used.
    /// </summary>
    [TemplatePart(Name = "PART_OuterBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_InnerBorder", Type = typeof(Border))]
    public class ClippedContent : ContentControl
    {
        public static readonly DependencyProperty CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(ClippedContent), new PropertyMetadata(new CornerRadius(0), CornerRadiusProperty_ValueChanged));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        static ClippedContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClippedContent), new FrameworkPropertyMetadata(typeof(ClippedContent)));
            BorderThicknessProperty.OverrideMetadata(typeof(ClippedContent), new FrameworkPropertyMetadata(new Thickness(0), BorderThicknessProperty_ValueChanged));
        }

        private static void BorderThicknessProperty_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ClippedContent)?.ApplyClip();
        }

        private static void CornerRadiusProperty_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ClippedContent)?.ApplyClip();
        }

        public ClippedContent()
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
            if (PART_InnerBorder == null)
                return;

            var innerRadius = new CornerRadiusEx
            (
                CornerRadius.TopLeft - BorderThickness.Left / 2, CornerRadius.TopLeft - BorderThickness.Top / 2,
                CornerRadius.TopRight - BorderThickness.Right / 2, CornerRadius.TopRight - BorderThickness.Top / 2,
                CornerRadius.BottomLeft - BorderThickness.Left / 2, CornerRadius.BottomLeft - BorderThickness.Bottom / 2,
                CornerRadius.BottomRight - BorderThickness.Right / 2, CornerRadius.BottomRight - BorderThickness.Bottom / 2
            );

            innerRadius = new CornerRadiusEx
            (
                Math.Max(0, innerRadius.TopLeftX), Math.Max(0, innerRadius.TopLeftY),
                Math.Max(0, innerRadius.TopRightX), Math.Max(0, innerRadius.TopRightY),
                Math.Max(0, innerRadius.BottomLeftX), Math.Max(0, innerRadius.BottomLeftY),
                Math.Max(0, innerRadius.BottomRightX), Math.Max(0, innerRadius.BottomRightY)
            );

            _clipRect = CreateRoundedRectangleGeometry(new Rect(0, 0, PART_InnerBorder.ActualWidth, PART_InnerBorder.ActualHeight), innerRadius);

            if (_clipRect != null)
            {
                PART_InnerBorder.Clip = _clipRect;
            }
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_OuterBorder = this.Template.FindName(nameof(PART_OuterBorder), this) as Border;
            PART_InnerBorder = this.Template.FindName(nameof(PART_InnerBorder), this) as Border;

            if (PART_InnerBorder != null)
            {
                PART_InnerBorder.SizeChanged += PART_InnerBorder_SizeChanged;
            }
            else
            {
                throw new Exception("PART_InnerBorder not found in template of ClippedContent");
            }

            ApplyClip();
        }

        private void PART_InnerBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplyClip();
        }

        private StreamGeometry _clipRect;
        private Border PART_OuterBorder;
        private Border PART_InnerBorder;

        /// <summary>
        /// Draws a rounded rectangle with four individual corner radius
        /// </summary>
        public StreamGeometry CreateRoundedRectangleGeometry(Rect rect, CornerRadiusEx cornerRadii)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                // The AI did really a great job! Prompt as follows:
                // c# wpf，我有一个 rect 和 CornerRadiusEx（这个CornerRadiusEx包含 TopLeftX, TopLeftY...每个角都可以自定义 X 和 Y），
                // 现在请你编写一个方法，生成一个 StreamGeometry，其内容就是指定的 rect 加上这个圆角

                context.BeginFigure(new Point(rect.Left + cornerRadii.TopLeftX, rect.Top), true, true);

                // Top edge
                context.LineTo(new Point(rect.Right - cornerRadii.TopRightX, rect.Top), true, false);
                context.ArcTo(new Point(rect.Right, rect.Top + cornerRadii.TopRightY),
                              new Size(cornerRadii.TopRightX, cornerRadii.TopRightY),
                              0, false, SweepDirection.Clockwise, true, false);

                // Right edge
                context.LineTo(new Point(rect.Right, rect.Bottom - cornerRadii.BottomRightY), true, false);
                context.ArcTo(new Point(rect.Right - cornerRadii.BottomRightX, rect.Bottom),
                              new Size(cornerRadii.BottomRightX, cornerRadii.BottomRightY),
                              0, false, SweepDirection.Clockwise, true, false);

                // Bottom edge
                context.LineTo(new Point(rect.Left + cornerRadii.BottomLeftX, rect.Bottom), true, false);
                context.ArcTo(new Point(rect.Left, rect.Bottom - cornerRadii.BottomLeftY),
                              new Size(cornerRadii.BottomLeftX, cornerRadii.BottomLeftY),
                              0, false, SweepDirection.Clockwise, true, false);

                // Left edge
                context.LineTo(new Point(rect.Left, rect.Top + cornerRadii.TopLeftY), true, false);
                context.ArcTo(new Point(rect.Left + cornerRadii.TopLeftX, rect.Top),
                              new Size(cornerRadii.TopLeftX, cornerRadii.TopLeftY),
                              0, false, SweepDirection.Clockwise, true, false);
            }

            geometry.Freeze(); // 让几何体不可变，提高性能
            return geometry;
        }
    }
}
