using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace iNKORE.UI.WPF.Modern.Controls.Helpers;

public static class ExpanderAnimationsHelper
{
    #region ToAnimateControlName
    public static string GetToAnimateControlName(Expander element) => (string)element.GetValue(ToAnimateControlNameProperty);

    public static void SetToAnimateControlName(Expander element, string value) => element.SetValue(ToAnimateControlNameProperty, value);

    public static readonly DependencyProperty ToAnimateControlNameProperty = DependencyProperty.RegisterAttached(
        "ToAnimateControlName",
        typeof(string),
        typeof(ExpanderAnimationsHelper),
        new PropertyMetadata("ExpanderContent"));
    #endregion
    
    #region ExpandAnimationDuration
    public static Duration GetExpandAnimationDuration(Expander element) => (Duration)element.GetValue(ExpandAnimationDurationProperty);

    public static void SetExpandAnimationDuration(Expander element, Duration value) => element.SetValue(ExpandAnimationDurationProperty, value);

    public static readonly DependencyProperty ExpandAnimationDurationProperty = DependencyProperty.RegisterAttached(
        "ExpandAnimationDuration",
        typeof(Duration),
        typeof(ExpanderAnimationsHelper),
        new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(333))));
    #endregion
    
    #region CollapseAnimationDuration
    public static Duration GetCollapseAnimationDuration(Expander element) => (Duration)element.GetValue(CollapseAnimationDurationProperty);

    public static void SetCollapseAnimationDuration(Expander element, Duration value) => element.SetValue(CollapseAnimationDurationProperty, value);

    public static readonly DependencyProperty CollapseAnimationDurationProperty = DependencyProperty.RegisterAttached(
        "CollapseAnimationDuration",
        typeof(Duration),
        typeof(ExpanderAnimationsHelper),
        new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(167))));
    #endregion

    #region IsEnabled
    public static bool GetIsEnabled(Expander element) => (bool)element.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(Expander element, bool value) => element.SetValue(IsEnabledProperty, value);

    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled",
        typeof(bool),
        typeof(ExpanderAnimationsHelper),
        new PropertyMetadata(OnIsEnabledChanged));

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Expander expander)
        {
            return;
        }
        
        var expandDirectionDescriptor = DependencyPropertyDescriptor.FromName(
            nameof(Expander.ExpandDirection),
            typeof(Expander),
            typeof(Expander));

        if (e.NewValue is not true)
        {
            expandDirectionDescriptor?.RemoveValueChanged(expander, OnExpandDirectionChanged);
            expander.Expanded -= OnExpanderExpanded;
            expander.Collapsed -= OnExpanderCollapsed;
            expander.ClearValue(ExpansionHandlerProperty);
            return;
        }

        OnExpandDirectionChanged(expander, null);
        expandDirectionDescriptor?.AddValueChanged(expander, OnExpandDirectionChanged);
        expander.Expanded += OnExpanderExpanded;
        expander.Collapsed += OnExpanderCollapsed;
    }

    private static void OnExpanderExpanded(object sender, RoutedEventArgs e)
    {
        if (sender is not Expander expander)
        {
            return;
        }

        if (GetExpansionHandler(expander) is not { } handler)
        {
            return;
        }

        var animationDuration = GetExpandAnimationDuration(expander);
        handler.Handle(animationDuration);
    }

    private static void OnExpanderCollapsed(object sender, RoutedEventArgs e)
    {
        if (sender is not Expander expander)
        {
            return;
        }

        if (GetExpansionHandler(expander) is not { } handler)
        {
            return;
        }

        var animationDuration = GetCollapseAnimationDuration(expander);
        handler.Handle(animationDuration);
    }

    private static void OnExpandDirectionChanged(object sender, EventArgs e)
    {
        if (sender is not Expander expander)
        {
            return;
        }

        var toAnimateControlName = GetToAnimateControlName(expander);
        ExpanderExpansionBaseHandler expansionHandler = expander.ExpandDirection switch
        {
            ExpandDirection.Up or ExpandDirection.Down => new ExpanderVerticalExpansionHandler(expander, toAnimateControlName),
            _ => new ExpanderHorizontalExpansionHandler(expander, toAnimateControlName)
        };

        expander.SetValue(ExpansionHandlerProperty, expansionHandler);
    }
    #endregion

    public static ExpanderExpansionBaseHandler GetExpansionHandler(Expander element) => (ExpanderExpansionBaseHandler)element.GetValue(ExpansionHandlerProperty);
    public static readonly DependencyProperty ExpansionHandlerProperty = DependencyProperty.RegisterAttached(
        "ExpansionHandler",
        typeof(ExpanderExpansionBaseHandler),
        typeof(ExpanderAnimationsHelper));
}

public abstract class ExpanderExpansionBaseHandler
{
    protected readonly Expander Expander;
    private readonly string _toAnimateTemplateControlName;
    protected FrameworkElement ToAnimateControl;
    protected FrameworkElement ContentControl;

    protected ExpanderExpansionBaseHandler(Expander expander,string toAnimateTemplateControlName)
    {
        Expander = expander;
        _toAnimateTemplateControlName = toAnimateTemplateControlName;
        expander.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ToAnimateControl = Expander.Template.FindName(_toAnimateTemplateControlName, Expander) as FrameworkElement;
        ContentControl = LogicalTreeHelper.GetChildren(Expander).OfType<FrameworkElement>().LastOrDefault();
        Expander.Loaded -= OnLoaded;
    }

    protected abstract DependencyProperty GetToAnimateProperty();
    protected abstract double GetAnimationFromValue();
    protected abstract double GetAnimationToValue();
    
    protected abstract void ValidateAnimationFromValue(FrameworkElement toAnimate, DoubleAnimation animation,
        double from);

    public void Handle(Duration animationDuration)
    {
        var toAnimateProperty = GetToAnimateProperty();
        if (toAnimateProperty?.PropertyType != typeof(double))
        {
            return;
        }
        
        //need to get it before layout cycle updates.
        var from = GetAnimationFromValue();
        var to = GetAnimationToValue(toAnimateProperty);

        var animation = new DoubleAnimation(to, animationDuration)
        {
            RepeatBehavior = new RepeatBehavior(1),
        };

        ValidateAnimationFromValue(ToAnimateControl, animation, from);

        ToAnimateControl.BeginAnimation(toAnimateProperty, animation);
    }

    private double GetAnimationToValue(DependencyProperty toAnimateProperty)
    {
        if (!Expander.IsExpanded)
        {
            return 0;
        }

        //this clears previous animation, as it seems to interfere with layout calculation?
        ToAnimateControl.BeginAnimation(toAnimateProperty, null);
        ToAnimateControl.SetValue(toAnimateProperty, double.NaN);
            
        UpdateLayout(ContentControl);
        return GetAnimationToValue();
    }

    private static void UpdateLayout(FrameworkElement contentControl)
    {
        //update content measures
        contentControl.Measure(new Size(contentControl.MaxWidth, contentControl.MaxHeight));
        contentControl.UpdateLayout();
    }
}

public sealed class ExpanderHorizontalExpansionHandler(Expander expander, string toAnimateTemplateControlName)
    : ExpanderExpansionBaseHandler(expander, toAnimateTemplateControlName)
{
    protected override DependencyProperty GetToAnimateProperty() => FrameworkElement.WidthProperty;
    protected override double GetAnimationFromValue() => ToAnimateControl.ActualWidth;

    protected override double GetAnimationToValue() => ContentControl.DesiredSize.Width;

    protected override void ValidateAnimationFromValue(FrameworkElement toAnimate, DoubleAnimation animation,
        double from)
    {
        if (!double.IsNaN(toAnimate.Width))
        {
            return;
        }

        animation.From = from;
    }
}

public sealed class ExpanderVerticalExpansionHandler(Expander expander, string toAnimateTemplateControlName)
    : ExpanderExpansionBaseHandler(expander, toAnimateTemplateControlName)
{
    protected override DependencyProperty GetToAnimateProperty() => FrameworkElement.HeightProperty;
    protected override double GetAnimationFromValue() => ToAnimateControl.ActualHeight;

    protected override double GetAnimationToValue() => ContentControl.DesiredSize.Height;

    protected override void ValidateAnimationFromValue(FrameworkElement toAnimate, DoubleAnimation animation,
        double from)
    {
        if (!double.IsNaN(toAnimate.Height))
        {
            return;
        }

        animation.From = from;
    }
}