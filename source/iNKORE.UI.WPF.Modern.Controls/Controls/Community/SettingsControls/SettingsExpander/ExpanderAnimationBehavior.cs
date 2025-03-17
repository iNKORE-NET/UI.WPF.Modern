using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Behaviors;

namespace iNKORE.UI.WPF.Modern.Controls;

public sealed class ExpanderAnimationBehavior : Behavior<FrameworkElement>
{
    private static readonly Duration s_expandAnimationDuration = new(TimeSpan.FromMilliseconds(333));
    private static readonly Duration s_collapseAnimationDuration = new(TimeSpan.FromMilliseconds(167));

    public FrameworkElement AnimationTarget
    {
        get => (FrameworkElement)GetValue(AnimationTargetProperty);
        set => SetValue(AnimationTargetProperty, value);
    }

    public static readonly DependencyProperty AnimationTargetProperty = DependencyProperty.Register(
        nameof(AnimationTarget),
        typeof(FrameworkElement),
        typeof(ExpanderAnimationBehavior),
        new PropertyMetadata(null));

    public FrameworkElement ContentControl
    {
        get => (FrameworkElement)GetValue(ContentControlProperty);
        set => SetValue(ContentControlProperty, value);
    }

    public static readonly DependencyProperty ContentControlProperty = DependencyProperty.Register(
        nameof(ContentControl),
        typeof(FrameworkElement),
        typeof(ExpanderAnimationBehavior),
        new PropertyMetadata(null));

    public ExpandDirection ExpandDirection
    {
        get => (ExpandDirection)GetValue(ExpandDirectionProperty);
        set => SetValue(ExpandDirectionProperty, value);
    }

    public static readonly DependencyProperty ExpandDirectionProperty = DependencyProperty.Register(
        nameof(ExpandDirection),
        typeof(ExpandDirection),
        typeof(ExpanderAnimationBehavior),
        new PropertyMetadata(default(ExpandDirection)));

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded),
        typeof(bool),
        typeof(ExpanderAnimationBehavior),
        new PropertyMetadata(false, OnIsExpandedChanged));

    private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ExpanderAnimationBehavior { ContentControl: { } contentControl, AnimationTarget: { } toAnimate } behavior)
        {
            return;
        }

        if (behavior.ExpandDirection is ExpandDirection.Up or ExpandDirection.Down)
        {
            HandleHeightExpansion(e, contentControl, toAnimate);
        }
        else
        {
            HandleWidthExpansion(e, contentControl, toAnimate);
        }
    }

    private static void HandleHeightExpansion(DependencyPropertyChangedEventArgs e, FrameworkElement contentControl, FrameworkElement toAnimate)
    {
        var to = 0.0; // default value, used in case of collapse

        //need to get it before layout cycle updates.
        var from = toAnimate.ActualHeight;

        var animationDuration = s_collapseAnimationDuration;

        if (e.NewValue is true)
        {
            animationDuration = s_expandAnimationDuration;

            //this clears previous animation, as it seems to interfere with layout calculation?
            toAnimate.BeginAnimation(FrameworkElement.HeightProperty, null);
            toAnimate.Height = double.NaN;
            UpdateLayout(contentControl);

            to = contentControl.DesiredSize.Height;
        }

        var animation = new DoubleAnimation(to, animationDuration)
        {
            RepeatBehavior = new RepeatBehavior(1),
        };

        if (double.IsNaN(toAnimate.Height))
        {
            animation.From = from;
        }

        toAnimate.BeginAnimation(FrameworkElement.HeightProperty, animation);
    }

    private static void HandleWidthExpansion(DependencyPropertyChangedEventArgs e, FrameworkElement contentControl, FrameworkElement toAnimate)
    {
        var to = 0.0; // default value, used in case of collapse

        //need to get it before layout cycle updates.
        var from = toAnimate.ActualWidth;

        var animationDuration = s_collapseAnimationDuration;

        if (e.NewValue is true)
        {
            animationDuration = s_expandAnimationDuration;

            //this clears previous animation, as it seems to interfere with layout calculation?
            toAnimate.BeginAnimation(FrameworkElement.WidthProperty, null);
            toAnimate.Width = double.NaN;
            UpdateLayout(contentControl);

            to = contentControl.DesiredSize.Width;
        }

        var animation = new DoubleAnimation(to, animationDuration)
        {
            RepeatBehavior = new RepeatBehavior(1),
        };

        if (double.IsNaN(toAnimate.Width))
        {
            animation.From = from;
        }

        toAnimate.BeginAnimation(FrameworkElement.WidthProperty, animation);
    }

    private static void UpdateLayout(FrameworkElement contentControl)
    {
        //update content measures
        contentControl.Measure(new Size(contentControl.MaxWidth, contentControl.MaxHeight));
        contentControl.UpdateLayout();
    }
}