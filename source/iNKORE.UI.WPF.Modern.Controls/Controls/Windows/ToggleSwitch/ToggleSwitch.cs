﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using iNKORE.UI.WPF.Modern.Automation.Peers;
using iNKORE.UI.WPF.Modern.Common;
using iNKORE.UI.WPF.Modern.Controls.Helpers;
using iNKORE.UI.WPF.Modern.Controls.Primitives;

namespace iNKORE.UI.WPF.Modern.Controls
{
    [ContentProperty(nameof(Header))]
    [TemplatePart(Name = nameof(HeaderContentPresenter), Type = typeof(ContentPresenter))]
    [TemplatePart(Name = nameof(SwitchKnobBounds), Type = typeof(FrameworkElement))]
    [TemplatePart(Name = nameof(SwitchKnob), Type = typeof(FrameworkElement))]
    [TemplatePart(Name = nameof(KnobTranslateTransform), Type = typeof(TranslateTransform))]
    [TemplatePart(Name = nameof(SwitchThumb), Type = typeof(Thumb))]
    [TemplateVisualState(GroupName = VisualStates.GroupCommon, Name = VisualStates.StateNormal)]
    [TemplateVisualState(GroupName = VisualStates.GroupCommon, Name = VisualStates.StateMouseOver)]
    [TemplateVisualState(GroupName = VisualStates.GroupCommon, Name = VisualStates.StatePressed)]
    [TemplateVisualState(GroupName = VisualStates.GroupCommon, Name = VisualStates.StateDisabled)]
    [TemplateVisualState(GroupName = ContentStatesGroup, Name = OffContentState)]
    [TemplateVisualState(GroupName = ContentStatesGroup, Name = OnContentState)]
    [TemplateVisualState(GroupName = ToggleStatesGroup, Name = DraggingOnState)]
    [TemplateVisualState(GroupName = ToggleStatesGroup, Name = DraggingOffState)]
    [TemplateVisualState(GroupName = ToggleStatesGroup, Name = OffState)]
    [TemplateVisualState(GroupName = ToggleStatesGroup, Name = OnState)]
    public class ToggleSwitch : Control
    {
        private const string ContentStatesGroup = "ContentStates";
        private const string OffContentState = "OffContent";
        private const string OnContentState = "OnContent";
        private const string ToggleStatesGroup = "ToggleStates";
        private const string DraggingOnState = "DraggingOn";
        private const string DraggingOffState = "DraggingOff";
        private const string OffState = "Off";
        private const string OnState = "On";

        private const double _offTranslation = 0;
        private double _onTranslation;
        private double _startTranslation;
        private bool _wasDragged;

        private BitmapCache _bitmapCache;

        static ToggleSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch), new FrameworkPropertyMetadata(typeof(ToggleSwitch)));

            EventManager.RegisterClassHandler(typeof(ToggleSwitch), MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
        }

        public ToggleSwitch()
        {
            SetCurrentValue(OffContentProperty, Strings.ToggleSwitchOff);
            SetCurrentValue(OnContentProperty, Strings.ToggleSwitchOn);

            IsEnabledChanged += OnIsEnabledChanged;
        }

        public static readonly RoutedEvent ToggledEvent = EventManager.RegisterRoutedEvent(nameof(Toggled), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToggleSwitch));

        public event RoutedEventHandler Toggled
        {
            add { AddHandler(ToggledEvent, value); }
            remove { RemoveHandler(ToggledEvent, value); }
        }

        #region Header

        
        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty =
            ControlHelper.HeaderProperty.AddOwner(
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(OnHeaderChanged));

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ToggleSwitch)d;
            control.UpdateHeaderContentPresenterVisibility();
            control.OnHeaderChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnHeaderChanged(object oldContent, object newContent)
        {
        }

        #endregion

        #region HeaderTemplate

        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public static readonly DependencyProperty HeaderTemplateProperty =
            ControlHelper.HeaderTemplateProperty.AddOwner(
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(OnHeaderTemplateChanged));

        private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToggleSwitch)d).UpdateHeaderContentPresenterVisibility();
        }

        #endregion

        #region IsOn

        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }

        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register(
                nameof(IsOn),
                typeof(bool),
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                    OnIsOnChanged));

        private static void OnIsOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ToggleSwitch)d;
            control.OnToggled();
            control.UpdateVisualStates(true);
        }

        #endregion

        #region OffContent

        public object OffContent
        {
            get => GetValue(OffContentProperty);
            set => SetValue(OffContentProperty, value);
        }

        public static readonly DependencyProperty OffContentProperty =
            DependencyProperty.Register(
                nameof(OffContent),
                typeof(object),
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(OnOffContentChanged));

        private static void OnOffContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToggleSwitch)d).OnOffContentChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnOffContentChanged(object oldContent, object newContent)
        {
        }

        #endregion

        #region OffContentTemplate

        public DataTemplate OffContentTemplate
        {
            get => (DataTemplate)GetValue(OffContentTemplateProperty);
            set => SetValue(OffContentTemplateProperty, value);
        }

        public static readonly DependencyProperty OffContentTemplateProperty =
            DependencyProperty.Register(
                nameof(OffContentTemplate),
                typeof(DataTemplate),
                typeof(ToggleSwitch),
                null);

        #endregion

        #region OnContent

        public object OnContent
        {
            get => GetValue(OnContentProperty);
            set => SetValue(OnContentProperty, value);
        }

        public static readonly DependencyProperty OnContentProperty =
            DependencyProperty.Register(
                nameof(OnContent),
                typeof(object),
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(OnOnContentChanged));

        private static void OnOnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToggleSwitch)d).OnOffContentChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnOnContentChanged(object oldContent, object newContent)
        {
        }

        #endregion

        #region OnContentTemplate

        public DataTemplate OnContentTemplate
        {
            get => (DataTemplate)GetValue(OnContentTemplateProperty);
            set => SetValue(OnContentTemplateProperty, value);
        }

        public static readonly DependencyProperty OnContentTemplateProperty =
            DependencyProperty.Register(
                nameof(OnContentTemplate),
                typeof(DataTemplate),
                typeof(ToggleSwitch),
                null);

        #endregion

        #region UseSystemFocusVisuals

        public static readonly DependencyProperty UseSystemFocusVisualsProperty =
            FocusVisualHelper.UseSystemFocusVisualsProperty.AddOwner(typeof(ToggleSwitch));

        public bool UseSystemFocusVisuals
        {
            get => (bool)GetValue(UseSystemFocusVisualsProperty);
            set => SetValue(UseSystemFocusVisualsProperty, value);
        }

        #endregion

        #region FocusVisualMargin

        public static readonly DependencyProperty FocusVisualMarginProperty =
            FocusVisualHelper.FocusVisualMarginProperty.AddOwner(typeof(ToggleSwitch));

        public Thickness FocusVisualMargin
        {
            get => (Thickness)GetValue(FocusVisualMarginProperty);
            set => SetValue(FocusVisualMarginProperty, value);
        }

        #endregion

        #region CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty =
            ControlHelper.CornerRadiusProperty.AddOwner(typeof(ToggleSwitch));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region IsRightAlignedCompact

        public static readonly DependencyProperty IsRightAlignedCompactProperty =
            DependencyProperty.Register(nameof(IsRightAlignedCompact), typeof(bool), typeof(ToggleSwitch), new PropertyMetadata(false));

        public bool IsRightAlignedCompact
        {
            get => (bool)GetValue(IsRightAlignedCompactProperty);
            set => SetValue(IsRightAlignedCompactProperty, value);
        }

        #endregion

        private ContentPresenter HeaderContentPresenter { get; set; }

        private FrameworkElement SwitchKnobBounds { get; set; }

        private FrameworkElement SwitchKnob { get; set; }

        private TranslateTransform KnobTranslateTransform { get; set; }

        private Thumb SwitchThumb { get; set; }

        public override void OnApplyTemplate()
        {
            if (SwitchKnobBounds != null &&
                SwitchKnob != null &&
                KnobTranslateTransform != null &&
                SwitchThumb != null)
            {
                SwitchThumb.DragStarted -= OnSwitchThumbDragStarted;
                SwitchThumb.DragDelta -= OnSwitchThumbDragDelta;
                SwitchThumb.DragCompleted -= OnSwitchThumbDragCompleted;
                if (ShadowAssist.UseBitmapCache)
                {
                    SwitchThumb.ClearValue(CacheModeProperty);
                }
            }

            base.OnApplyTemplate();

            HeaderContentPresenter = GetTemplateChild(nameof(HeaderContentPresenter)) as ContentPresenter;
            SwitchKnobBounds = GetTemplateChild(nameof(SwitchKnobBounds)) as FrameworkElement;
            SwitchKnob = GetTemplateChild(nameof(SwitchKnob)) as FrameworkElement;
            KnobTranslateTransform = GetTemplateChild(nameof(KnobTranslateTransform)) as TranslateTransform;
            SwitchThumb = GetTemplateChild(nameof(SwitchThumb)) as Thumb;

            if (SwitchKnobBounds != null &&
                SwitchKnob != null &&
                KnobTranslateTransform != null &&
                SwitchThumb != null)
            {
                SwitchThumb.DragStarted += OnSwitchThumbDragStarted;
                SwitchThumb.DragDelta += OnSwitchThumbDragDelta;
                SwitchThumb.DragCompleted += OnSwitchThumbDragCompleted;

                if (ShadowAssist.UseBitmapCache)
                {
                    if (_bitmapCache == null)
                    {
#if NET462_OR_NEWER
                        _bitmapCache = new BitmapCache(VisualTreeHelper.GetDpi(this).PixelsPerDip);
#else
                        _bitmapCache = new BitmapCache(2);
#endif
                    }

                    SwitchThumb.CacheMode = _bitmapCache;
                }
            }

            UpdateHeaderContentPresenterVisibility();
            UpdateVisualStates(false);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ToggleSwitchAutomationPeer(this);
        }

        protected virtual void OnToggled()
        {
            RaiseEvent(new RoutedEventArgs(ToggledEvent));

            if (UIElementAutomationPeer.FromElement(this) is { } peer)
            {
                var newValue = IsOn ? ToggleState.On : ToggleState.Off;
                var oldValue = (newValue == ToggleState.On) ? ToggleState.Off : ToggleState.On;
                peer.RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, oldValue, newValue);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == IsMouseOverProperty)
            {
                UpdateVisualStates(true);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (SwitchKnobBounds != null && SwitchKnob != null)
            {
                _onTranslation = SwitchKnobBounds.ActualWidth - SwitchKnob.ActualWidth - SwitchKnob.Margin.Left - SwitchKnob.Margin.Right;
            }
        }

#if NET462_OR_NEWER
        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            base.OnDpiChanged(oldDpi, newDpi);

            if (ShadowAssist.UseBitmapCache && _bitmapCache != null)
            {
                _bitmapCache.RenderAtScale = newDpi.PixelsPerDip;
            }
        }
#endif

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                Toggle();
            }

            base.OnKeyUp(e);
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;

            if (!toggle.IsKeyboardFocusWithin)
            {
                e.Handled = toggle.Focus() || e.Handled;
            }
        }

        private void OnSwitchThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            e.Handled = true;
            _startTranslation = KnobTranslateTransform.X;
            UpdateVisualStates(true);
            KnobTranslateTransform.X = _startTranslation;
        }

        private void OnSwitchThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            e.Handled = true;
            if (e.HorizontalChange != 0)
            {
                _wasDragged = true;
                double dragTranslation = _startTranslation + e.HorizontalChange;
                KnobTranslateTransform.X = Math.Max(_offTranslation, Math.Min(_onTranslation, dragTranslation));
            }
            UpdateVisualStates(true);
        }

        private void OnSwitchThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            e.Handled = true;
            bool click = false;
            if (_wasDragged)
            {
                double edge = (_onTranslation + _offTranslation) / 2;
                if ((IsOn && KnobTranslateTransform.X <= edge) || (!IsOn && KnobTranslateTransform.X >= edge))
                {
                    click = true;
                }
            }
            else
            {
                click = true;
            }
            if (click)
            {
                Toggle();
            }

            _wasDragged = false;
            UpdateVisualStates(true);
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualStates(true);
        }

        private void UpdateHeaderContentPresenterVisibility()
        {
            if (HeaderContentPresenter != null)
            {
                bool showHeader = !ControlHelper.IsNullOrEmptyString(Header) || HeaderTemplate != null;
                HeaderContentPresenter.Visibility = showHeader ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void UpdateVisualStates(bool useTransitions)
        {
            string stateName;

            if (!IsEnabled)
            {
                stateName = VisualStates.StateDisabled;
            }
            else if (SwitchThumb != null && SwitchThumb.IsDragging)
            {
                stateName = VisualStates.StatePressed;
            }
            else if (IsMouseOver)
            {
                stateName = VisualStates.StateMouseOver;
            }
            else
            {
                stateName = VisualStates.StateNormal;
            }
            VisualStateManager.GoToState(this, stateName, useTransitions);

            if (SwitchThumb != null && SwitchThumb.IsDragging)
            {
                stateName = IsOn ? DraggingOnState : DraggingOffState;
            }
            else
            {
                stateName = IsOn ? OnState : OffState;
            }
            VisualStateManager.GoToState(this, stateName, useTransitions);

            VisualStateManager.GoToState(this, IsOn ? OnContentState : OffContentState, useTransitions);
        }

        internal void Toggle()
        {
            SetCurrentValue(IsOnProperty, !IsOn);
        }
    }
}
