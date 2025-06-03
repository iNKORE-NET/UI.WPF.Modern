using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace iNKORE.UI.WPF.Modern.Media.Animation
{
    internal class NavigationAnimation
    {
        static NavigationAnimation()
        {
            _defaultBitmapCache = new BitmapCache();
            _defaultBitmapCache.Freeze();
        }

        private readonly bool _useBitmapCache;

        public NavigationAnimation(FrameworkElement element, Storyboard storyboard, bool useBitmapCache)
        {
            _element = element;
            _storyboard = storyboard;
            _useBitmapCache = useBitmapCache;
            _storyboard.CurrentStateInvalidated += OnCurrentStateInvalidated;
            _storyboard.Completed += OnCompleted;
        }

        public event EventHandler Completed;

        public void Begin()
        {
            if (_useBitmapCache && _element.CacheMode is not BitmapCache)
            {
                _element.SetCurrentValue(UIElement.CacheModeProperty, GetBitmapCache());
            }
            _storyboard.Begin(_element, true);
        }

        public void Stop()
        {
            if (_currentState != ClockState.Stopped)
            {
                _storyboard.Stop(_element);
            }
            if (_useBitmapCache)
            {
                _element.InvalidateProperty(UIElement.CacheModeProperty);
            }
            _element.InvalidateProperty(UIElement.RenderTransformProperty);
            _element.InvalidateProperty(UIElement.RenderTransformOriginProperty);
        }

        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            if (sender is Clock clock)
            {
                _currentState = clock.CurrentState;
            }
        }

        private void OnCompleted(object sender, EventArgs e)
        {
            Completed?.Invoke(this, EventArgs.Empty);
        }

        private BitmapCache GetBitmapCache()
        {
#if NET462_OR_NEWER
            return new BitmapCache(VisualTreeHelper.GetDpi(_element).PixelsPerDip);
#else
            return _defaultBitmapCache;
#endif
        }

        private static readonly BitmapCache _defaultBitmapCache;

        private readonly FrameworkElement _element;
        private readonly Storyboard _storyboard;

        private ClockState _currentState = ClockState.Stopped;
    }
}
