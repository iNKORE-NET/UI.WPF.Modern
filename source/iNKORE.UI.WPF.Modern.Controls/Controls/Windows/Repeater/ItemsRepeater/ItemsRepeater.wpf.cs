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

        // Default alignment for BringIntoView: 0.0 means align to top/left edge of viewport
        private const double DefaultBringIntoViewAlignment = 0.0;
        
        // Default offset for BringIntoView: no additional offset
        private const double DefaultBringIntoViewOffset = 0.0;
        
        // Default animation for BringIntoView: false for immediate scrolling
        private const bool DefaultBringIntoViewAnimate = false;

        protected override void OnRequestBringIntoView(RequestBringIntoViewEventArgs e)
        {
            // Forward the BringIntoView request to the ItemsRepeaterScrollHost if one exists.
            // This enables BringIntoView to work properly with virtualized layouts like UniformGridLayout.
            var targetObject = e.TargetObject;
            if (targetObject != this && targetObject is UIElement targetElement)
            {
                // Verify the target element is actually a descendant of this ItemsRepeater
                if (!IsAncestorOf(targetElement))
                {
                    // Not our element, let the base class handle it
                    base.OnRequestBringIntoView(e);
                    return;
                }

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
                            alignmentX: DefaultBringIntoViewAlignment,
                            alignmentY: DefaultBringIntoViewAlignment,
                            offsetX: DefaultBringIntoViewOffset,
                            offsetY: DefaultBringIntoViewOffset,
                            animate: DefaultBringIntoViewAnimate);
                        
                        // Mark the event as handled so it doesn't bubble up further
                        e.Handled = true;
                        return;
                    }
                }
            }
            
            // No scroll host found, use default behavior
            base.OnRequestBringIntoView(e);
        }

        private bool IsAncestorOf(DependencyObject descendant)
        {
            if (descendant == null)
                return false;

            DependencyObject current = descendant;
            while (current != null)
            {
                if (current == this)
                    return true;
                
                current = System.Windows.Media.VisualTreeHelper.GetParent(current);
            }
            
            return false;
        }

        protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            return new RepeaterUIElementCollection(this, logicalParent);
        }

        private static readonly Size s_zeroSize = new Size();
    }
}
