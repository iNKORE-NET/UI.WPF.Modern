using iNKORE.UI.WPF.Modern.Helpers;
using iNKORE.UI.WPF.Modern.Helpers.Styles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace iNKORE.UI.WPF.Modern.Controls.Primitives
{
    [TemplatePart(Name = BackButtonName, Type = typeof(Button))]
    [TemplatePart(Name = MaximizeButtonName, Type = typeof(TitleBarButton))]
    [TemplatePart(Name = LeftSystemOverlayName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = RightSystemOverlayName, Type = typeof(FrameworkElement))]
    [StyleTypedProperty(Property = nameof(ButtonStyle), StyleTargetType = typeof(TitleBarButton))]
    [StyleTypedProperty(Property = nameof(BackButtonStyle), StyleTargetType = typeof(TitleBarButton))]
    public class TitleBarControl : Control
    {
        private const string BackButtonName = "PART_BackButton";
        private const string MaximizeButtonName = "MaximizeRestoreButton";
        private const string LeftSystemOverlayName = "PART_LeftSystemOverlay";
        private const string RightSystemOverlayName = "PART_RightSystemOverlay";

        private Window _parentWindow;
        private SnapLayout _snapLayout;
        private KeyBinding _altLeftBinding;


        private static readonly DependencyPropertyDescriptor descriptor_WindowStyle = DependencyPropertyDescriptor.FromProperty(Window.WindowStyleProperty, typeof(Window));
        private static readonly DependencyPropertyDescriptor descriptor_ResizeMode = DependencyPropertyDescriptor.FromProperty(Window.ResizeModeProperty, typeof(Window));

        static TitleBarControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleBarControl),
                new FrameworkPropertyMetadata(typeof(TitleBarControl)));
        }

        public TitleBarControl()
        {
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));

            SetInsideTitleBar(this, true);
        }

        #region IsActive

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(
                nameof(IsActive),
                typeof(bool),
                typeof(TitleBarControl),
                new PropertyMetadata(false));

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        #endregion

        //#region MaximizeButtonTouchOptimize

        //public static readonly DependencyProperty MaximizeButtonTouchOptimizeProperty =
        //    DependencyProperty.Register(
        //        nameof(MaximizeButtonTouchOptimize),
        //        typeof(bool),
        //        typeof(TitleBarControl),
        //        new PropertyMetadata(false));

        //public bool MaximizeButtonTouchOptimize
        //{
        //    get => (bool)GetValue(MaximizeButtonTouchOptimizeProperty);
        //    set => SetValue(MaximizeButtonTouchOptimizeProperty, value);
        //}

        //#endregion


        #region InactiveBackground

        public static readonly DependencyProperty InactiveBackgroundProperty =
            TitleBar.InactiveBackgroundProperty.AddOwner(typeof(TitleBarControl));

        public Brush InactiveBackground
        {
            get => (Brush)GetValue(InactiveBackgroundProperty);
            set => SetValue(InactiveBackgroundProperty, value);
        }

        #endregion

        #region InactiveForeground

        public static readonly DependencyProperty InactiveForegroundProperty =
            TitleBar.InactiveForegroundProperty.AddOwner(typeof(TitleBarControl));

        public Brush InactiveForeground
        {
            get => (Brush)GetValue(InactiveForegroundProperty);
            set => SetValue(InactiveForegroundProperty, value);
        }

        #endregion

        #region ButtonStyle

        public static readonly DependencyProperty ButtonStyleProperty =
            TitleBar.ButtonStyleProperty.AddOwner(typeof(TitleBarControl));

        public Style ButtonStyle
        {
            get => (Style)GetValue(ButtonStyleProperty);
            set => SetValue(ButtonStyleProperty, value);
        }

        #endregion

        #region Title

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(TitleBarControl),
                new PropertyMetadata(string.Empty));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        #region Icon

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(ImageSource),
                typeof(TitleBarControl),
                new PropertyMetadata(OnIconChanged));

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TitleBarControl)d).UpdateActualIcon();
        }

        #endregion

        #region ActualIcon

        private static readonly DependencyPropertyKey ActualIconPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ActualIcon),
                typeof(ImageSource),
                typeof(TitleBarControl),
                null);

        public static readonly DependencyProperty ActualIconProperty =
            ActualIconPropertyKey.DependencyProperty;

        public ImageSource ActualIcon
        {
            get => (ImageSource)GetValue(ActualIconProperty);
            private set => SetValue(ActualIconPropertyKey, value);
        }

        private void UpdateActualIcon()
        {
            if (Icon != null)
            {
                ActualIcon = Icon;
            }
            else
            {
                ImageSource actualIcon = null;

                var smallIconHandle = new IntPtr[1];
                IconHelper.GetDefaultIconHandles(null, smallIconHandle);
                var smallIcon = smallIconHandle[0];
                if (smallIcon != IntPtr.Zero)
                {
                    try
                    {
                        actualIcon = Imaging.CreateBitmapSourceFromHIcon(smallIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    }
                    finally
                    {
                        IconHelper.DestroyIcon(smallIcon);
                    }
                }

                ActualIcon = actualIcon;
            }
        }

        #endregion

        #region IsIconVisible

        public static readonly DependencyProperty IsIconVisibleProperty =
            TitleBar.IsIconVisibleProperty.AddOwner(typeof(TitleBarControl));

        public bool IsIconVisible
        {
            get => (bool)GetValue(IsIconVisibleProperty);
            set => SetValue(IsIconVisibleProperty, value);
        }

        #endregion

        #region IsBackButtonVisible

        public static readonly DependencyProperty IsBackButtonVisibleProperty =
            TitleBar.IsBackButtonVisibleProperty.AddOwner(typeof(TitleBarControl));

        public bool IsBackButtonVisible
        {
            get => (bool)GetValue(IsBackButtonVisibleProperty);
            set => SetValue(IsBackButtonVisibleProperty, value);
        }

        #endregion

        #region IsBackEnabled

        /// <summary>
        /// Identifies the IsBackEnabled attached property.
        /// </summary>
        public static readonly DependencyProperty IsBackEnabledProperty =
            TitleBar.IsBackEnabledProperty.AddOwner(typeof(TitleBarControl));

        /// <summary>
        /// Gets or sets a value that indicates whether the back button is enabled or disabled.
        /// </summary>
        /// <returns>true if the back button is enabled; otherwise, false. The default is true.</returns>
        public bool IsBackEnabled
        {
            get => (bool)GetValue(IsBackEnabledProperty);
            set => SetValue(IsBackEnabledProperty, value);
        }

        #endregion

        #region BackButtonCommand

        public static readonly DependencyProperty BackButtonCommandProperty =
            TitleBar.BackButtonCommandProperty.AddOwner(typeof(TitleBarControl));

        public ICommand BackButtonCommand
        {
            get => (ICommand)GetValue(BackButtonCommandProperty);
            set => SetValue(BackButtonCommandProperty, value);
        }

        #endregion

        #region BackButtonCommandParameter

        public static readonly DependencyProperty BackButtonCommandParameterProperty =
            TitleBar.BackButtonCommandParameterProperty.AddOwner(typeof(TitleBarControl));

        public object BackButtonCommandParameter
        {
            get => GetValue(BackButtonCommandParameterProperty);
            set => SetValue(BackButtonCommandParameterProperty, value);
        }

        #endregion

        #region BackButtonCommandTarget

        public static readonly DependencyProperty BackButtonCommandTargetProperty =
            TitleBar.BackButtonCommandTargetProperty.AddOwner(typeof(TitleBarControl));

        public IInputElement BackButtonCommandTarget
        {
            get => (IInputElement)GetValue(BackButtonCommandTargetProperty);
            set => SetValue(BackButtonCommandTargetProperty, value);
        }

        #endregion

        #region BackButtonStyle

        public static readonly DependencyProperty BackButtonStyleProperty =
            TitleBar.BackButtonStyleProperty.AddOwner(typeof(TitleBarControl));

        public Style BackButtonStyle
        {
            get => (Style)GetValue(BackButtonStyleProperty);
            set => SetValue(BackButtonStyleProperty, value);
        }

        #endregion

        #region ExtendViewIntoTitleBar

        public static readonly DependencyProperty ExtendViewIntoTitleBarProperty =
            TitleBar.ExtendViewIntoTitleBarProperty.AddOwner(typeof(TitleBarControl));

        public bool ExtendViewIntoTitleBar
        {
            get => (bool)GetValue(ExtendViewIntoTitleBarProperty);
            set => SetValue(ExtendViewIntoTitleBarProperty, value);
        }

        #endregion

        #region InsideTitleBar

        internal static readonly DependencyProperty InsideTitleBarProperty =
            DependencyProperty.RegisterAttached(
                "InsideTitleBar",
                typeof(bool),
                typeof(TitleBarControl),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        internal static bool GetInsideTitleBar(UIElement element)
        {
            return (bool)element.GetValue(InsideTitleBarProperty);
        }

        internal static void SetInsideTitleBar(UIElement element, bool value)
        {
            element.SetValue(InsideTitleBarProperty, value);
        }

        #endregion

        #region Button Availability

        private static void ButtonAvailabilityProperty_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TitleBarControl)?.RefreshButtonActualAvailabilities();
        }

        public static readonly DependencyProperty ButtonCloseAvailabilityProperty =
            TitleBar.ButtonCloseAvailabilityProperty.AddOwner(typeof(TitleBarControl), new PropertyMetadata(TitleBarButtonAvailability.Auto, ButtonAvailabilityProperty_ValueChanged));

        public TitleBarButtonAvailability ButtonCloseAvailability
        {
            get => (TitleBarButtonAvailability)GetValue(ButtonCloseAvailabilityProperty);
            set => SetValue(ButtonCloseAvailabilityProperty, value);
        }

        // User-customized values

        public static readonly DependencyProperty ButtonMinimizeAvailabilityProperty =
            TitleBar.ButtonMinimizeAvailabilityProperty.AddOwner(typeof(TitleBarControl), new PropertyMetadata(TitleBarButtonAvailability.Auto, ButtonAvailabilityProperty_ValueChanged));

        public TitleBarButtonAvailability ButtonMinimizeAvailability
        {
            get => (TitleBarButtonAvailability)GetValue(ButtonMinimizeAvailabilityProperty);
            set => SetValue(ButtonMinimizeAvailabilityProperty, value);
        }

        public static readonly DependencyProperty ButtonMaximizeAvailabilityProperty =
            TitleBar.ButtonMaximizeAvailabilityProperty.AddOwner(typeof(TitleBarControl), new PropertyMetadata(TitleBarButtonAvailability.Auto, ButtonAvailabilityProperty_ValueChanged));

        public TitleBarButtonAvailability ButtonMaximizeAvailability
        {
            get => (TitleBarButtonAvailability)GetValue(ButtonMaximizeAvailabilityProperty);
            set => SetValue(ButtonMaximizeAvailabilityProperty, value);
        }

        // Actual values

        public static readonly DependencyPropertyKey ButtonCloseActualAvailabilityPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ButtonCloseActualAvailability), typeof(TitleBarButtonAvailability), typeof(TitleBarControl), new PropertyMetadata(TitleBarButtonAvailability.Enabled));
        public static readonly DependencyProperty ButtonCloseActualAvailabilityProperty = ButtonCloseActualAvailabilityPropertyKey.DependencyProperty;
        public TitleBarButtonAvailability ButtonCloseActualAvailability
        {
            get => (TitleBarButtonAvailability)GetValue(ButtonCloseActualAvailabilityProperty);
            private set => SetValue(ButtonCloseActualAvailabilityPropertyKey, value);
        }

        public static readonly DependencyPropertyKey ButtonMaximizeActualAvailabilityPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ButtonMaximizeActualAvailability), typeof(TitleBarButtonAvailability), typeof(TitleBarControl), new PropertyMetadata(TitleBarButtonAvailability.Enabled));
        public static readonly DependencyProperty ButtonMaximizeActualAvailabilityProperty = ButtonMaximizeActualAvailabilityPropertyKey.DependencyProperty;
        public TitleBarButtonAvailability ButtonMaximizeActualAvailability
        {
            get => (TitleBarButtonAvailability)GetValue(ButtonMaximizeActualAvailabilityProperty);
            private set => SetValue(ButtonMaximizeActualAvailabilityPropertyKey, value);
        }

        public static readonly DependencyPropertyKey ButtonMinimizeActualAvailabilityPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ButtonMinimizeActualAvailability), typeof(TitleBarButtonAvailability), typeof(TitleBarControl), new PropertyMetadata(TitleBarButtonAvailability.Enabled));
        public static readonly DependencyProperty ButtonMinimizeActualAvailabilityProperty = ButtonMinimizeActualAvailabilityPropertyKey.DependencyProperty;
        public TitleBarButtonAvailability ButtonMinimizeActualAvailability
        {
            get => (TitleBarButtonAvailability)GetValue(ButtonMinimizeActualAvailabilityProperty);
            private set => SetValue(ButtonMinimizeActualAvailabilityPropertyKey, value);
        }

        #endregion

        private Button BackButton { get; set; }

        private TitleBarButton MaximizeRestoreButton { get; set; }

        private FrameworkElement LeftSystemOverlay { get; set; }

        private FrameworkElement RightSystemOverlay { get; set; }

        public override void OnApplyTemplate()
        {
            if (BackButton != null)
            {
                BackButton.Click -= OnBackButtonClick;
            }

            if (MaximizeRestoreButton != null)
            {
                MaximizeRestoreButton.Loaded -= OnMaximizeRestoreButtonLoaded;
            }

            if (LeftSystemOverlay != null)
            {
                LeftSystemOverlay.SizeChanged -= OnLeftSystemOverlaySizeChanged;
            }

            if (RightSystemOverlay != null)
            {
                RightSystemOverlay.SizeChanged -= OnRightSystemOverlaySizeChanged;
            }

            base.OnApplyTemplate();

            BackButton = GetTemplateChild(BackButtonName) as Button;
            MaximizeRestoreButton = GetTemplateChild(MaximizeButtonName) as TitleBarButton;
            LeftSystemOverlay = GetTemplateChild(LeftSystemOverlayName) as FrameworkElement;
            RightSystemOverlay = GetTemplateChild(RightSystemOverlayName) as FrameworkElement;

            if (BackButton != null)
            {
                BackButton.Click += OnBackButtonClick;
            }

            if (MaximizeRestoreButton != null)
            {
                MaximizeRestoreButton.Loaded += OnMaximizeRestoreButtonLoaded;
            }

            if (LeftSystemOverlay != null)
            {
                LeftSystemOverlay.SizeChanged += OnLeftSystemOverlaySizeChanged;
                UpdateSystemOverlayLeftInset(LeftSystemOverlay.ActualWidth);
            }

            if (RightSystemOverlay != null)
            {
                RightSystemOverlay.SizeChanged += OnRightSystemOverlaySizeChanged;
                UpdateSystemOverlayRightInset(RightSystemOverlay.ActualWidth);
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            UpdateActualIcon();
            base.OnInitialized(e);
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            if (_parentWindow != null)
            {
                descriptor_ResizeMode.RemoveValueChanged(_parentWindow, _window_ButtonAvailabilityShouldRefresh);
                descriptor_WindowStyle.RemoveValueChanged(_parentWindow, _window_ButtonAvailabilityShouldRefresh);

                if (_altLeftBinding != null)
                {
                    _parentWindow.InputBindings.Remove(_altLeftBinding);
                    _altLeftBinding = null;
                }
            }

            base.OnVisualParentChanged(oldParent);

            _parentWindow = TemplatedParent as Window;

            if (_parentWindow != null)
            {
                _altLeftBinding = new KeyBinding(new GoBackCommand(this), Key.Left, ModifierKeys.Alt);
                _parentWindow.InputBindings.Add(_altLeftBinding);
                descriptor_ResizeMode.AddValueChanged(_parentWindow, _window_ButtonAvailabilityShouldRefresh);
                descriptor_WindowStyle.AddValueChanged(_parentWindow, _window_ButtonAvailabilityShouldRefresh);

                RefreshButtonActualAvailabilities();
            }
        }

        private void _window_ButtonAvailabilityShouldRefresh(object sender, EventArgs e)
        {
            if(sender == _parentWindow)
            {
                RefreshButtonActualAvailabilities();
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            Debug.Assert(TemplatedParent is Window);
            if (TemplatedParent is Window window)
            {
                TitleBar.SetHeight(window, sizeInfo.NewSize.Height);
            }
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            if (TemplatedParent is Window window)
            {
                TitleBar.RaiseBackRequested(window);
            }
        }

        public void RefreshButtonActualAvailabilities()
        {

            // Close button
            if (ButtonCloseAvailability != TitleBarButtonAvailability.Auto)
            {
                ButtonCloseActualAvailability = ButtonCloseAvailability;
            }
            else
            {
                if (_parentWindow.WindowStyle == WindowStyle.None)
                {
                    ButtonCloseActualAvailability = TitleBarButtonAvailability.Collapsed;
                }
                else
                {
                    ButtonCloseActualAvailability = TitleBarButtonAvailability.Enabled;
                }
            }

            // Maximize button
            if (ButtonMaximizeAvailability != TitleBarButtonAvailability.Auto)
            {
                ButtonMaximizeActualAvailability = ButtonMaximizeAvailability;
            }
            else
            {
                if(_parentWindow.WindowStyle == WindowStyle.ToolWindow)
                {
                    ButtonMaximizeActualAvailability = TitleBarButtonAvailability.Collapsed;
                }
                else if (_parentWindow.WindowStyle == WindowStyle.None)
                {
                    ButtonMaximizeActualAvailability = TitleBarButtonAvailability.Collapsed;
                }
                else
                {
                    switch (_parentWindow.ResizeMode)
                    {
                        case ResizeMode.CanMinimize:
                            ButtonMaximizeActualAvailability = TitleBarButtonAvailability.Disabled;
                            break;
                        case ResizeMode.CanResizeWithGrip:
                        case ResizeMode.CanResize:
                            ButtonMaximizeActualAvailability = TitleBarButtonAvailability.Enabled;
                            break;
                        case ResizeMode.NoResize:
                            ButtonMaximizeActualAvailability = TitleBarButtonAvailability.Collapsed;
                            break;
                    }
                }
            }

            // Minimize button
            if (ButtonMinimizeAvailability != TitleBarButtonAvailability.Auto)
            {
                ButtonMinimizeActualAvailability = ButtonMinimizeAvailability;
            }
            else
            {
                if (_parentWindow.WindowStyle == WindowStyle.ToolWindow)
                {
                    ButtonMinimizeActualAvailability = TitleBarButtonAvailability.Collapsed;
                }
                else if (_parentWindow.WindowStyle == WindowStyle.None)
                {
                    ButtonMinimizeActualAvailability = TitleBarButtonAvailability.Collapsed;
                }
                else
                {
                    switch (_parentWindow.ResizeMode)
                    {
                        case ResizeMode.CanMinimize:
                        case ResizeMode.CanResizeWithGrip:
                        case ResizeMode.CanResize:
                            ButtonMinimizeActualAvailability = TitleBarButtonAvailability.Enabled;
                            break;
                        case ResizeMode.NoResize:
                            ButtonMinimizeActualAvailability = TitleBarButtonAvailability.Collapsed;
                            break;
                    }
                }
            }

            InitializeSnapLayout();
        }


        private void OnLeftSystemOverlaySizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSystemOverlayLeftInset(e.NewSize.Width);
        }

        private void OnRightSystemOverlaySizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSystemOverlayRightInset(e.NewSize.Width);
        }

        private void OnMaximizeRestoreButtonLoaded(object sender, RoutedEventArgs e)
        {
            InitializeSnapLayout();
        }

        private void UpdateSystemOverlayLeftInset(double value)
        {
            Debug.Assert(TemplatedParent is Window);
            if (TemplatedParent is Window window)
            {
                TitleBar.SetSystemOverlayLeftInset(window, value);
            }
        }

        private void UpdateSystemOverlayRightInset(double value)
        {
            Debug.Assert(TemplatedParent is Window);
            if (TemplatedParent is Window window)
            {
                TitleBar.SetSystemOverlayRightInset(window, value);
            }
        }

        private void InitializeSnapLayout()
        {
            if(MaximizeRestoreButton != null)
            {
                InitializeSnapLayout(MaximizeRestoreButton);
            }
        }

        private void InitializeSnapLayout(TitleBarButton maximizeButton)
        {
            if (!SnapLayout.IsSupported) return;

            if (maximizeButton.IsEnabled && maximizeButton.Visibility == Visibility.Visible)
            {
                if(_snapLayout == null)
                {
                    _snapLayout = new SnapLayout();
                    _snapLayout.Register(maximizeButton);
                }
            }
            else
            {
                if(_snapLayout != null)
                {
                    _snapLayout.Unregister();
                    _snapLayout = null;
                }
            }
        }

        private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (TemplatedParent is Window window)
            {
                SystemCommands.MinimizeWindow(window);
            }
        }

        private void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (TemplatedParent is Window window)
            {
                SystemCommands.MaximizeWindow(window);
            }
        }

        private void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (TemplatedParent is Window window)
            {
                SystemCommands.RestoreWindow(window);
            }
        }

        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (TemplatedParent is Window window)
            {
                SystemCommands.CloseWindow(window);
            }
        }

        private void InvokeBack()
        {
            if (BackButton != null && BackButton.IsEnabled)
            {
                var peer = UIElementAutomationPeer.CreatePeerForElement(BackButton);
                (peer?.GetPattern(PatternInterface.Invoke) as IInvokeProvider)?.Invoke();
            }
        }

        private class GoBackCommand : ICommand
        {
            private readonly TitleBarControl _owner;

            public GoBackCommand(TitleBarControl owner)
            {
                _owner = owner;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                _owner.InvokeBack();
            }
        }
    }
}
