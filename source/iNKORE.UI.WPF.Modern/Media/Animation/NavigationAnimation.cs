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
        }

        public NavigationAnimation(FrameworkElement element, Storyboard storyboard)
        {
            _element = element;
            _storyboard = storyboard;
            _storyboard.CurrentStateInvalidated += OnCurrentStateInvalidated;
            _storyboard.Completed += OnCompleted;
        }

        public event EventHandler Completed;

        public void Begin()
        {
            _storyboard.Begin(_element, true);
        }

        public void Stop()
        {
            if (_currentState != ClockState.Stopped)
            {
                _storyboard.Stop(_element);
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

        private readonly FrameworkElement _element;
        private readonly Storyboard _storyboard;

        private ClockState _currentState = ClockState.Stopped;
    }
}
