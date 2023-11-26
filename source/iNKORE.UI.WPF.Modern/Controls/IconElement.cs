
/* 项目“iNKORE.UI.WPF.Modern (net452)”的未合并的更改
在此之前:
using System;
在此之后:
using iNKORE;
using iNKORE.UI;
using iNKORE.UI.WPF;
using iNKORE.UI.WPF.Modern;
using iNKORE.UI.WPF.Modern.Controls;
using iNKORE.UI.WPF.Modern.Controls.IconElement;
using iNKORE.UI.WPF.Modern.Controls.IconElement.IconElement;
using System;
*/
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

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
using iNKORE.UI.WPF.Modern.Controls.IconElement;
*/
using System.Windows.Media;

namespace iNKORE.UI.WPF.Modern.Controls
{
    /// <summary>
    /// Represents the base class for an icon UI element.
    /// </summary>
    [TypeConverter(typeof(IconElementConverter))]
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
            ShouldInheritForegroundFromVisualParent =
                _isForegroundDefaultOrInherited &&
                Parent != null &&
                VisualParent != null &&
                Parent != VisualParent;
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
    }
}
