using System.Windows;
using System.Windows.Controls;

namespace iNKORE.UI.WPF.Modern.Controls
{
    partial class ItemsRepeater
    {
        // WPF-specific workaround to avoid freezing and improve performance
        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            var virtInfo = TryGetVirtualizationInfo(child);
            if (virtInfo != null && virtInfo.IsRealized)
            {
                var oldDesiredSize = virtInfo.DesiredSize;
                if (!oldDesiredSize.IsEmpty)
                {
                    var newDesiredSize = child.DesiredSize;
                    var renderSize = child.RenderSize;

                    if (oldDesiredSize == s_zeroSize || newDesiredSize == s_zeroSize ||
                        newDesiredSize.Height != oldDesiredSize.Height && renderSize.Height == oldDesiredSize.Height ||
                        newDesiredSize.Width != oldDesiredSize.Width && renderSize.Width == oldDesiredSize.Width)
                    {
                        base.OnChildDesiredSizeChanged(child);
                    }
                }
            }
        }

        protected override void OnRequestBringIntoView(RequestBringIntoViewEventArgs e)
        {
            // Forward the BringIntoView request to the ItemsRepeaterScrollHost if one exists.
            // This enables BringIntoView to work properly with virtualized layouts like UniformGridLayout.
            var targetObject = e.TargetObject;
            if (targetObject != this && targetObject is UIElement targetElement)
            {
                // Walk up the visual tree to find an ItemsRepeaterScrollHost
                DependencyObject parent = this;
                while (parent != null)
                {
                    parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
                    if (parent is ItemsRepeaterScrollHost scrollHost)
                    {
                        // Found a scroll host - forward the request to it.
                        // Using alignment 0.0 means align to the top/left edge of the viewport.
                        // The scroll host will calculate the correct scroll position based on
                        // the element's layout bounds.
                        scrollHost.StartBringIntoView(
                            targetElement,
                            alignmentX: 0.0,
                            alignmentY: 0.0,
                            offsetX: 0.0,
                            offsetY: 0.0,
                            animate: false);
                        
                        // Mark the event as handled so it doesn't bubble up further
                        e.Handled = true;
                        return;
                    }
                }
            }
            
            // No scroll host found, use default behavior
            base.OnRequestBringIntoView(e);
        }

        protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            return new RepeaterUIElementCollection(this, logicalParent);
        }

        private static readonly Size s_zeroSize = new Size();
    }
}
