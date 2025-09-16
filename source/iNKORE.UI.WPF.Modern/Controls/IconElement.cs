using iNKORE.UI.WPF.Modern.Common;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace iNKORE.UI.WPF.Modern.Controls
{
    /// <summary>
    /// Represents the base class for an icon UI element.
    /// </summary>
    // [TypeConverter(typeof(IconElementConverter))]
    public abstract class IconElement : FrameworkElement
    {
        private protected IconElement()
        {
        }

        #region Foreground

        /// <summary>
        /// Identifies the Foreground dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
                TextElement.ForegroundProperty.AddOwner(
                        typeof(IconElement),
                        new FrameworkPropertyMetadata(SystemColors.ControlTextBrush,
                            FrameworkPropertyMetadataOptions.Inherits,
                            OnForegroundPropertyChanged));

        private static void OnForegroundPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((IconElement)sender).OnForegroundPropertyChanged(args);
        }

        private void OnForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            var baseValueSource = DependencyPropertyHelper.GetValueSource(this, args.Property).BaseValueSource;
            _isForegroundDefaultOrInherited = baseValueSource <= BaseValueSource.Inherited;
            UpdateShouldInheritForegroundFromVisualParent();
            // Notify derived classes so they can push Foreground to internal visuals when not inheriting.
            OnOwnForegroundPropertyChanged(args);
        }

        /// <summary>
        /// Gets or sets a brush that describes the foreground color.
        /// </summary>
        /// <value>The brush that paints the foreground of the control.</value>
        [Bindable(true), Category("Appearance")]
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        #endregion

        #region VisualParentForeground

        private static readonly DependencyProperty VisualParentForegroundProperty =
            DependencyProperty.Register(
                nameof(VisualParentForeground),
                typeof(Brush),
                typeof(IconElement),
                new PropertyMetadata(null, OnVisualParentForegroundPropertyChanged));

        private protected Brush VisualParentForeground
        {
            get => (Brush)GetValue(VisualParentForegroundProperty);
            set => SetValue(VisualParentForegroundProperty, value);
        }

        private static void OnVisualParentForegroundPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((IconElement)sender).OnVisualParentForegroundPropertyChanged(args);
        }

        private protected virtual void OnVisualParentForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
        }

        #endregion

        private protected bool ShouldInheritForegroundFromVisualParent
        {
            get => _shouldInheritForegroundFromVisualParent;
            private set
            {
                if (_shouldInheritForegroundFromVisualParent != value)
                {
                    _shouldInheritForegroundFromVisualParent = value;

                    if (_shouldInheritForegroundFromVisualParent)
                    {
                        SetBinding(VisualParentForegroundProperty,
                            new Binding
                            {
                                Path = new PropertyPath(TextElement.ForegroundProperty),
                                Source = VisualParent
                            });
                    }
                    else
                    {
                        ClearValue(VisualParentForegroundProperty);
                    }

                    OnShouldInheritForegroundFromVisualParentChanged();
                }
            }
        }

        private protected virtual void OnShouldInheritForegroundFromVisualParentChanged()
        {
        }

        private void UpdateShouldInheritForegroundFromVisualParent()
        {
            // Legacy logic (before simplification) kept for reference:
            // We originally required that the element had both a logical Parent and a VisualParent and that they differed.
            // This proved too restrictive in scenarios where the logical and visual tree relationships caused FontIcon
            // to miss theme brush updates (e.g., in the iconography gallery) leaving glyphs with a stale/dark brush.
            //
            // ShouldInheritForegroundFromVisualParent =
            //     _isForegroundDefaultOrInherited &&
            //     Parent != null &&
            //     VisualParent != null &&
            //     Parent != VisualParent;
            
            // if our Foreground is default/inherited and we have a visual parent, bind to it.
            ShouldInheritForegroundFromVisualParent =
                _isForegroundDefaultOrInherited &&
                VisualParent != null; // Always inherit from visual parent when our Foreground is default/inherited.
        }

        /// <summary>
        /// Called whenever this element's own Foreground property changes (after inheritance logic updated).
        /// Allows derived classes to propagate the Foreground to visual children when not inheriting directly from visual parent.
        /// </summary>
        /// <param name="args">Change args.</param>
        private protected virtual void OnOwnForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
        }

        private protected UIElementCollection Children
        {
            get
            {
                EnsureLayoutRoot();
                return _layoutRoot.Children;
            }
        }

        private protected abstract void InitializeChildren();

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
            {
                EnsureLayoutRoot();
                return _layoutRoot;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            EnsureLayoutRoot();
            _layoutRoot.Measure(availableSize);
            return _layoutRoot.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            EnsureLayoutRoot();
            _layoutRoot.Arrange(new Rect(new Point(), finalSize));
            return finalSize;
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            UpdateShouldInheritForegroundFromVisualParent();
        }

        private void EnsureLayoutRoot()
        {
            if (_layoutRoot != null)
                return;

            _layoutRoot = new Grid
            {
                Background = Brushes.Transparent,
                SnapsToDevicePixels = true,
            };
            InitializeChildren();

            AddVisualChild(_layoutRoot);
        }

        private Grid _layoutRoot;
        private bool _isForegroundDefaultOrInherited = true;
        private bool _shouldInheritForegroundFromVisualParent;

        /// <summary>
        /// Creates an icon source.
        /// </summary>
        /// <returns>An icon source.</returns>
        public IconSource CreateIconSource()
        {
            var element = CreateIconSourceCore();
            return element;
        }

        /// <summary>
        /// Creates an icon source.
        /// </summary>
        /// <returns>An icon source.</returns>
        protected virtual IconSource CreateIconSourceCore() => null;

    }
}
