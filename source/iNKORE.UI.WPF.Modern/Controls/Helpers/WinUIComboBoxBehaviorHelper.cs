using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using iNKORE.UI.WPF.Converters;
using iNKORE.UI.WPF.Modern.Controls.Primitives;

namespace iNKORE.UI.WPF.Modern.Controls.Helpers
{
    public sealed class WinUIComboBoxBehaviorHelper
    {
        static WinUIComboBoxBehaviorHelper()
        {
            EventManager.RegisterClassHandler(typeof(ComboBoxItem), Mouse.MouseEnterEvent,
                new MouseEventHandler(OnOverrideMouseEnter), false);
        }

        private static void OnOverrideMouseEnter(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(WinUIComboBoxBehaviorHelper),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(ComboBox comboBox) => (bool)comboBox.GetValue(IsEnabledProperty);

        public static void SetIsEnabled(ComboBox comboBox, bool value) => comboBox.SetValue(IsEnabledProperty, value);

        private static void OnIsEnabledChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs args)
        {
            if (sender is not ComboBox comboBox)
            {
                return;
            }

            if (args.NewValue is true)
            {
                comboBox.DropDownOpened += OnDropDownOpened;
                comboBox.DropDownClosed += OnDropDownClosed;
            }
            else
            {
                comboBox.DropDownOpened -= OnDropDownOpened;
                comboBox.DropDownClosed -= OnDropDownClosed;
            }
        }

        private static void OnDropDownOpened(object sender, EventArgs e)
        {
            var comboBox = (ComboBox)sender;
            // We need to know whether the dropDown opens above or below the ComboBox in order to update corner radius correctly.
            // Sometimes TransformToPoint value is incorrect because popup is not fully opened when this function gets called.
            // Use dispatcher to make sure we get correct VerticalOffset.
            comboBox.Dispatcher.BeginInvoke(() => { PopupArrangementHelperBase.GetForCombobox(comboBox).Arrange(); });
        }

        private static void OnDropDownClosed(object sender, object args)
        {
            var comboBox = (ComboBox)sender;
            UpdateCornerRadiusForClosedDropDown(comboBox);
        }

        private static void UpdateCornerRadiusForClosedDropDown(ComboBox comboBox)
        {
            var textBoxRadius = ControlHelper.GetCornerRadius(comboBox);
            var popupRadius = (CornerRadius)ResourceLookup(comboBox, "OverlayCornerRadius");

            if (GetTemplateChild<Border>("PopupBorder", comboBox) is { } popupBorder)
            {
                popupBorder.CornerRadius = popupRadius;
            }

            if (comboBox.IsEditable)
            {
                if (GetTemplateChild<TextBox>("PART_EditableTextBox", comboBox) is { } textBox)
                {
                    ControlHelper.SetCornerRadius(textBox, textBoxRadius);
                }
            }
            else
            {
                if (GetTemplateChild<Border>("Background", comboBox) is { } background)
                {
                    background.CornerRadius = textBoxRadius;
                }
            }
        }

        private static object ResourceLookup(Control control, object key) => control.TryFindResource(key);

        private static T GetTemplateChild<T>(string childName, Control control) where T : DependencyObject =>
            control.Template?.FindName(childName, control) as T;

        #region RestrictToTaskbar

        public static readonly DependencyProperty RestrictToTaskbarProperty =
            DependencyProperty.RegisterAttached(
                "RestrictToTaskbar",
                typeof(bool),
                typeof(WinUIComboBoxBehaviorHelper),
                new PropertyMetadata(false));

        public static bool GetRestrictToTaskbar(ComboBox comboBox) =>
            (bool)comboBox.GetValue(RestrictToTaskbarProperty);

        public static void SetRestrictToTaskbar(ComboBox comboBox, bool value) =>
            comboBox.SetValue(RestrictToTaskbarProperty, value);

        #endregion

        #region AdjustScroll

        internal static readonly DependencyProperty AdjustScrollProperty =
            DependencyProperty.RegisterAttached(
                "AdjustScroll",
                typeof(bool),
                typeof(WinUIComboBoxBehaviorHelper),
                new PropertyMetadata(true));

        internal static bool GetAdjustScroll(ComboBox comboBox) =>
            (bool)comboBox.GetValue(AdjustScrollProperty);

        internal static void SetAdjustScroll(ComboBox comboBox, bool value) =>
            comboBox.SetValue(AdjustScrollProperty, value);

        #endregion
    }

    internal interface IPopupArrangementHelper
    {
        void Arrange();

    }

    internal abstract class PopupArrangementHelperBase : IPopupArrangementHelper
    {
        public static IPopupArrangementHelper GetForCombobox(ComboBox comboBox) => comboBox.IsEditable
            ? new EditableComboboxPopupArrangementHelper(comboBox)
            : new DefaultPopupArrangementHelper(comboBox);

        public PopupArrangementHelperBase(ComboBox comboBox)
        {
            _comboBox = comboBox;

            _popup = GetTemplateChild<Popup>(c_popupName);
            _popupChild = _popup.Child as ThemeShadowChrome;
            _innerBorder1 = _popupChild?.Child as Border;
            innerBorder2 = _innerBorder1?.Child as Border;

            _background = GetTemplateChild<FrameworkElement>(c_backgroundName);
            _scroller = GetTemplateChild<ScrollViewer>(c_scrollerName);
            _presenter = _scroller.Content as ItemsPresenter;

            _itemsCount = _comboBox.Items.Count;
            _selectedIndex = _comboBox.SelectedIndex;

            MarginBetweenPresenterAndPopup = new Thickness(
                _popupChild.Margin.Left + _innerBorder1.Margin.Left + innerBorder2.Margin.Left + _presenter.Margin.Left,
                _popupChild.Margin.Top + _innerBorder1.Margin.Top + innerBorder2.Margin.Top + _presenter.Margin.Top,
                _popupChild.Margin.Right + _innerBorder1.Margin.Right + innerBorder2.Margin.Right +
                _presenter.Margin.Right,
                _popupChild.Margin.Bottom + _innerBorder1.Margin.Bottom + innerBorder2.Margin.Bottom +
                _presenter.Margin.Bottom);

            PopupRadius = (CornerRadius)ResourceLookup(c_overlayCornerRadiusKey);

            WorkAreaSize = new Size(SystemParameters.MaximizedPrimaryScreenWidth,
                SystemParameters.MaximizedPrimaryScreenHeight);
        }

        public void Arrange()
        {
            if (!_comboBox.IsDropDownOpen || _popupChild is null || !UpdateMeasurements())
            {
                return;
            }

            var (popupY, finalPopupHeight, offset) = ArrangeItems();
            UpdatePopupPlacement(popupY, finalPopupHeight);
        }

        protected readonly double MaxScreenExtent = SystemParameters.MaximizedPrimaryScreenHeight;
        protected static readonly Point P00 = new(0, 0);

        private const string c_backgroundName = "Background";
        private const string c_popupName = "PART_Popup";
        private const string c_scrollerName = "ScrollViewer";

        private const string c_overlayCornerRadiusKey = "OverlayCornerRadius";
        private const string c_maxItemOnOneSide = "ComboBoxPopupMaxNumberOfItemsThatCanBeShownOnOneSide";
        private const int defaultMaxItemsOnOneSide = 7;
        private const string c_maxItems = "ComboBoxPopupMaxNumberOfItems";
        private const int defaultMaxItems = 15;

        protected readonly ComboBox _comboBox;
        protected readonly Popup _popup;
        protected readonly ThemeShadowChrome _popupChild;
        protected readonly Border _innerBorder1;
        protected readonly Border innerBorder2;
        protected readonly FrameworkElement _background;
        protected readonly ScrollViewer _scroller;
        protected readonly ItemsPresenter _presenter;

        protected Thickness MarginBetweenPresenterAndPopup { get; }
        protected CornerRadius PopupRadius { get; }
        protected Size WorkAreaSize { get; }

        protected double scaledCBY { get; set; }

        protected readonly int _itemsCount;
        protected readonly int _selectedIndex;

        protected int _maxNumberOfItemsAllowedOnOneSide;
        protected int _maxNumberOfItemsAllowed;
        protected double _actualHeight;
        protected double _popupMaxHeight;

        private bool UpdateMeasurements()
        {
            var p00S = _background.PointToScreen(P00);
            var cbY = p00S.Y;

            var source = PresentationSource.FromVisual(_comboBox);
            if (source?.CompositionTarget is null)
            {
                scaledCBY = 0;
                return false;
            }

            var transformToRootMatrix = source.CompositionTarget.TransformToDevice;

            var scaleY = transformToRootMatrix.M22 != 0
                ? Math.Sqrt(transformToRootMatrix.M21 * transformToRootMatrix.M21 +
                            transformToRootMatrix.M22 * transformToRootMatrix.M22)
                : 1.0;

            scaledCBY = cbY / scaleY;

            _actualHeight = _background.ActualHeight;
            _popupChild.MinWidth = _background.ActualWidth;
            _popupChild.MinHeight = _actualHeight;

            _popupMaxHeight = _comboBox.MaxDropDownHeight;
            if (double.IsInfinity(_popupMaxHeight) || double.IsNaN(_popupMaxHeight))
            {
                _popupMaxHeight = WorkAreaSize.Height;
            }

            return true;
        }

        protected abstract (double popupY, double finalPopupHeight, double offset) ArrangeItems();
        protected abstract void UpdatePopupPlacement(double popupY, double finalPopupHeight);

        protected virtual void UpdateLayoutResources()
        {
            var maxNumberOfItemsThatCanBeShownOnOneSide =
                ResourceLookup(c_maxItemOnOneSide) as int? ?? defaultMaxItemsOnOneSide;
            var maxNumberOfItemsThatCanBeShown = ResourceLookup(c_maxItems) as int? ?? defaultMaxItems;

            _maxNumberOfItemsAllowedOnOneSide = Math.Min(maxNumberOfItemsThatCanBeShownOnOneSide, _itemsCount);
            _maxNumberOfItemsAllowed = Math.Min(maxNumberOfItemsThatCanBeShown, _itemsCount);
        }

        protected double GetItemLayoutHeight(int index)
        {
            if (_comboBox.ItemContainerGenerator.ContainerFromIndex(index) is not UIElement container)
            {
                container = GetRealizedFirstChildFromPanel();
                if (container is null)
                {
                    return 0;
                }
            }

            if (container.Visibility is not Visibility.Visible)
            {
                return 0;
            }

            if (container is ComboBoxItem comboBoxItem)
            {
                comboBoxItem.ApplyTemplate(); // Ensures the visual tree is loaded
            }

            if (double.IsNaN(container.DesiredSize.Height) || container.DesiredSize.Height is 0)
            {
                container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return container.DesiredSize.Height;
        }

        protected UIElement GetRealizedFirstChildFromPanel()
        {
            if (_presenter is null)
            {
                return null;
            }

            return VisualTreeHelper.GetChild(_presenter, 0) is Panel { Children.Count: > 0 } itemsHostPanel
                ? itemsHostPanel.Children[0]
                : null;
        }

        protected void UpdatePopupBorderCornerRadius(CornerRadiusFilterKind popupBorderRadiusFilter)
        {
            var popupBorderRadius =
                CornerRadiusFilterConverter.Convert(PopupRadius, popupBorderRadiusFilter);
            _innerBorder1.CornerRadius = popupBorderRadius;
        }

        private object ResourceLookup(object key) => _comboBox.TryFindResource(key);

        protected T GetTemplateChild<T>(string childName) where T : DependencyObject =>
            _comboBox.Template?.FindName(childName, _comboBox) as T;
    }

    internal sealed class DefaultPopupArrangementHelper(ComboBox comboBox) : PopupArrangementHelperBase(comboBox)
    {
        protected override (double popupY, double finalPopupHeight, double offset) ArrangeItems()
        {
            if (_itemsCount is 0)
            {
                var popupYForEmpty = scaledCBY;
                return (popupYForEmpty, _actualHeight, 0);
            }

            var centerItemIndex = GetCenterIndex();

            var currentItemHeight = GetItemLayoutHeight(centerItemIndex);
            var adjustment = currentItemHeight - _comboBox.ActualHeight;

            UpdateLayoutResources(centerItemIndex, currentItemHeight);

            LayItemsAboveCenter(ref currentItemHeight);
            LayItemsBelowCenter(ref currentItemHeight);
            LayRemainingItems(ref currentItemHeight);

            var popupY = _layoutLocationAbove;
            if (adjustment > 0)
            {
                popupY += _comboBox.ActualHeight - _presenter.Margin.Bottom;
            }

            var finalPopupHeight =
                Math.Max(_actualHeight,
                    Math.Min(
                        _layoutLocationBelow - _layoutLocationAbove + _presenter.Margin.Bottom,
                        _popupMaxHeight));

            var offset = _itemIndexAbove + 1;

            MakeAdjustmentsForNoSelection();

            return (popupY, finalPopupHeight, offset);
        }

        protected override void UpdatePopupPlacement(double popupY, double finalPopupHeight)
        {
            _popup.Placement = PlacementMode.Relative;
            var backgroundVisual = _background.TransformToVisual(_comboBox);
            var backgroundP00Cb = backgroundVisual.Transform(P00);

            var popupTop = popupY - scaledCBY + backgroundP00Cb.Y;

            var overflow = popupY + finalPopupHeight - WorkAreaSize.Height;
            if (overflow > 0 && popupTop > -overflow)
            {
                popupTop = -overflow;
            }

            _popup.VerticalOffset = popupTop;
            UpdatePopupBorderCornerRadius(CornerRadiusFilterKind.All);
        }

        private void MakeAdjustmentsForNoSelection()
        {
            if (_selectedIndex >= 0 || _comboBox.ItemContainerGenerator.ContainerFromIndex(0) is not ComboBoxItem item)
            {
                return;
            }

            if (WinUIComboBoxBehaviorHelper.GetAdjustScroll(_comboBox))
            {
                BringIndexToView(_itemIndexBelow + 1);
                WinUIComboBoxBehaviorHelper.SetAdjustScroll(_comboBox, false);
            }
            HighlightItem(item);
        }

        private void HighlightItem(ComboBoxItem item)
        {
            var highlightedInfoProperty = typeof(ComboBox).GetProperty("HighlightedInfo",
                BindingFlags.Instance | BindingFlags.NonPublic);

            var setter = highlightedInfoProperty?.SetMethod;

            var itemInfo = typeof(ComboBox)
                .GetMethod("ItemInfoFromContainer", BindingFlags.Instance | BindingFlags.NonPublic)?
                .Invoke(_comboBox, [item]);

            setter?.Invoke(_comboBox, [itemInfo]);
        }

        private void BringIndexToView(int index)
        {
            var navigateToItemMethod = typeof(ItemsControl)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .First(method => method.Name == "NavigateToItem" &&
                                 method.GetParameters().Length is 4 &&
                                 method.GetParameters()[0].ParameterType == typeof(object) &&
                                 method.GetParameters()[1].ParameterType == typeof(int) &&
                                 method.GetParameters()[3].ParameterType == typeof(bool));

            var arg2 = navigateToItemMethod.GetParameters()[2].ParameterType.GetProperty("Empty").GetMethod
                .Invoke(null, []);
            navigateToItemMethod?.Invoke(_comboBox, [null, index, arg2, false]);
        }

        private int GetCenterIndex()
        {
            var centerItemIndex = _selectedIndex;

            if (centerItemIndex >= _itemsCount || centerItemIndex < 0)
            {
                centerItemIndex = _itemsCount / 2;
            }

            return centerItemIndex;
        }

        private void LayItemsAboveCenter(ref double currentItemHeight)
        {
            if (_itemIndexAbove < 0)
            {
                return;
            }

            currentItemHeight = GetItemLayoutHeight(_itemIndexAbove);

            while (_itemIndexAbove >= 0 &&
                   _layoutLocationAbove - currentItemHeight >= _upperLimit &&
                   _totalItemsLayed < _maxNumberOfItemsAllowedOnOneSide)
            {
                _layoutLocationAbove -= currentItemHeight;
                _totalItemsLayed++;
                _itemIndexAbove--;
                if (_itemIndexAbove >= 0)
                {
                    currentItemHeight = GetItemLayoutHeight(_itemIndexAbove);
                }
            }
        }

        private void LayItemsBelowCenter(ref double currentItemHeight)
        {
            if (_itemIndexBelow >= _itemsCount)
            {
                return;
            }

            currentItemHeight = GetItemLayoutHeight(_itemIndexBelow);

            while (_itemIndexBelow < _itemsCount &&
                   _layoutLocationBelow + currentItemHeight < _lowerLimit &&
                   _layoutLocationBelow - _layoutLocationAbove < _popupMaxHeight &&
                   _totalItemsLayed < _maxNumberOfItemsAllowed)
            {
                _layoutLocationBelow += currentItemHeight;
                _totalItemsLayed++;
                _itemIndexBelow++;
                if (_itemIndexBelow < _itemsCount)
                {
                    currentItemHeight = GetItemLayoutHeight(_itemIndexBelow);
                }
            }
        }

        private void LayRemainingItems(ref double currentItemHeight)
        {
            if (_itemIndexAbove < 0 && _itemIndexBelow >= _itemsCount)
            {
                return;
            }

            var isAbove = _itemIndexAbove >= 0;
            var currentItemIndex = isAbove ? _itemIndexAbove : _itemIndexBelow;
            currentItemHeight = GetItemLayoutHeight(currentItemIndex);

            while (_layoutLocationBelow - _layoutLocationAbove + currentItemHeight <= _popupMaxHeight &&
                   (_layoutLocationBelow + currentItemHeight < WorkAreaSize.Height ||
                    _layoutLocationAbove - currentItemHeight >= 0) &&
                   _totalItemsLayed < _maxNumberOfItemsAllowed)
            {
                if (isAbove)
                {
                    _itemIndexAbove--;
                }
                else
                {
                    _itemIndexBelow++;
                }

                if (_layoutLocationAbove - currentItemHeight <= 0)
                {
                    _layoutLocationBelow += currentItemHeight;
                }
                else
                {
                    _layoutLocationAbove -= currentItemHeight;
                }

                _totalItemsLayed++;
                if (_itemIndexAbove < 0 && _itemIndexBelow >= _itemsCount)
                {
                    continue;
                }

                isAbove = _itemIndexAbove >= 0;
                currentItemIndex = isAbove ? _itemIndexAbove : _itemIndexBelow;
                currentItemHeight = GetItemLayoutHeight(currentItemIndex);
            }
        }

        private void UpdateLayoutResources(
            int centerItemIndex,
            double currentItemHeight)
        {
            UpdateLayoutResources();

            _calculatedLayoutLocationAbove =
                scaledCBY + _actualHeight / 2 - currentItemHeight / 2 - MarginBetweenPresenterAndPopup.Top;
            _layoutLocationAbove = Math.Max(_calculatedLayoutLocationAbove, 0);
            _upperLimit = scaledCBY + _actualHeight / 2 - currentItemHeight - _popupMaxHeight / 2;

            _calculatedLayoutLocationBelow = _layoutLocationAbove + currentItemHeight + MarginBetweenPresenterAndPopup.Top +
                                             MarginBetweenPresenterAndPopup.Bottom;
            _layoutLocationBelow = _calculatedLayoutLocationBelow;
            _lowerLimit = Math.Min(_upperLimit + _popupMaxHeight + currentItemHeight,
                WorkAreaSize.Height + MarginBetweenPresenterAndPopup.Bottom);

            _itemIndexAbove = centerItemIndex - 1;
            _itemIndexBelow = centerItemIndex + 1;
            _totalItemsLayed = 1;
        }

        private double _calculatedLayoutLocationAbove;
        private double _layoutLocationAbove;
        private double _upperLimit;
        private double _calculatedLayoutLocationBelow;
        private double _layoutLocationBelow;
        private double _lowerLimit;
        private int _itemIndexAbove;
        private int _itemIndexBelow;
        private int _totalItemsLayed;
    }

    internal sealed class EditableComboboxPopupArrangementHelper : PopupArrangementHelperBase
    {
        public EditableComboboxPopupArrangementHelper(ComboBox comboBox) : base(comboBox)
        {
            _textBox = GetTemplateChild<TextBox>(c_editableTextName);
        }

        protected override (double popupY, double finalPopupHeight, double offset) ArrangeItems()
        {
            if (_textBox is not null && _textBox.ActualHeight > _actualHeight)
            {
                _actualHeight = _textBox.ActualHeight;
            }

            if (_itemsCount is 0)
            {
                return (scaledCBY, _actualHeight, 0);
            }

            UpdateLayoutResources();
            LayItems();

            var popupY = _layoutLocationAbove;

            var finalPopupHeight =
                Math.Max(_actualHeight,
                    Math.Min(
                        _layoutLocationBelow - _layoutLocationAbove,
                        _popupMaxHeight));

            return (popupY, finalPopupHeight, 0);
        }

        protected override void UpdatePopupPlacement(double popupY, double finalPopupHeight)
        {
            var extensionBeyondLimit = popupY + finalPopupHeight - WorkAreaSize.Height;
            var (placement, verticalOffset, textboxCornerRadiusFilter, popupBorderRadiusFilter) =
                extensionBeyondLimit switch
                {
                    > 0 when scaledCBY > finalPopupHeight => (PlacementMode.Top, 0, CornerRadiusFilterKind.Bottom,
                        CornerRadiusFilterKind.Top),
                    <= 0 => (PlacementMode.Bottom, popupY - scaledCBY, CornerRadiusFilterKind.Top,
                        CornerRadiusFilterKind.Bottom),
                    _ => (PlacementMode.Bottom, popupY - scaledCBY - extensionBeyondLimit, CornerRadiusFilterKind.None,
                        CornerRadiusFilterKind.None),
                };

            var textboxRadius =
                CornerRadiusFilterConverter.Convert(ControlHelper.GetCornerRadius(_comboBox),
                    textboxCornerRadiusFilter);
            ControlHelper.SetCornerRadius(_textBox, textboxRadius);

            UpdatePopupBorderCornerRadius(popupBorderRadiusFilter);

            _popup.Placement = placement;
            _popup.VerticalOffset = verticalOffset;
        }

        private void LayItems()
        {
            var currentIndex = 0;
            var totalItemsLayed = 0;

            while (currentIndex < _itemsCount && totalItemsLayed < _maxNumberOfItemsAllowed)
            {
                var currentItemHeight = GetItemLayoutHeight(currentIndex);
                _layoutLocationBelow += currentItemHeight;
                totalItemsLayed++;
                currentIndex++;
            }
        }

        protected override void UpdateLayoutResources()
        {
            base.UpdateLayoutResources();

            _calculatedLayoutLocationAbove = scaledCBY;
            _layoutLocationAbove = Math.Max(_calculatedLayoutLocationAbove, 0);
            _layoutLocationBelow = _layoutLocationAbove + MarginBetweenPresenterAndPopup.Top;
        }

        private double _calculatedLayoutLocationAbove;
        private double _layoutLocationAbove;
        private double _layoutLocationBelow;

        private readonly TextBox _textBox;
        private const string c_editableTextName = "PART_EditableTextBox";
    }
}