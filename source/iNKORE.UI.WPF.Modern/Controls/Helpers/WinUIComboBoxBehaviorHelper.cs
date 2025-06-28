using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using iNKORE.UI.WPF.Converters;

namespace iNKORE.UI.WPF.Modern.Controls.Helpers
{
    public sealed class WinUIComboBoxBehaviorHelper
    {
        private const string c_popupBorderName = "PopupBorder";

        private const string c_editableTextName = "PART_EditableTextBox";

        //private const string c_editableTextBorderName = "BorderElement";
        private const string c_backgroundName = "Background";

        private const string c_highlightBackgroundName = "HighlightBackground";

        //private const string c_controlCornerRadiusKey = "ControlCornerRadius";
        private const string c_overlayCornerRadiusKey = "OverlayCornerRadius";

        /// <summary>
        /// Adds WinUI behaviors
        /// 1. align selected container in popup to combobox.
        /// 2. in case of no selection, first in popup is highlighted (done)
        /// 3. mouse hovering shouldn't trigger focus ?! (done)
        /// 4. persist selected item only when drop down closes?? (not done)
        /// 5. KeepInteriorCornersSquare (already done)
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(WinUIComboBoxBehaviorHelper),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(ComboBox comboBox)
        {
            return (bool)comboBox.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(ComboBox comboBox, bool value)
        {
            comboBox.SetValue(IsEnabledProperty, value);
        }

        private static void OnIsEnabledChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs args)
        {
            if (sender is ComboBox comboBox)
            {
                bool shouldMonitorDropDownState = (bool)args.NewValue;
                if (shouldMonitorDropDownState)
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
        }

        private static void OnOverrideMouseEnter(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }

        private static void OnDropDownClosed(object sender, object args)
        {
            var comboBox = (ComboBox)sender;
            UpdateCornerRadius(comboBox, null, /*IsDropDownOpen=*/false, false);
        }

        private static void OnDropDownOpened(object sender, object args)
        {
            var comboBox = (ComboBox)sender;
            comboBox.Dispatcher.BeginInvoke(() =>
            {
                var popup = GetTemplateChild<Popup>("PART_Popup", comboBox);
                var isOpenDown = IsPopupOpenDown(comboBox, popup.VerticalOffset);

                AlignSelectedContainer(comboBox, popup, isOpenDown);
                UpdateCornerRadius(comboBox, popup, true, isOpenDown);
            });
        }

        private static void AlignSelectedContainer(ComboBox comboBox, Popup popup, bool isOpenDown)
        {
            if (!isOpenDown ||
                GetToAlignContainer(comboBox) is not { } itemContainer ||
                itemContainer.TranslatePoint(new Point(0, -itemContainer.ActualHeight + comboBox.Padding.Top),
                    comboBox) is not { Y: not 0 } itemTop)
            {
                return;
            }

            popup.VerticalOffset -= itemTop.Y;

            if (itemContainer.ActualHeight - comboBox.ActualHeight > 0)
            {
                popup.VerticalOffset -= comboBox.ActualHeight;
            }
        }

        private static FrameworkElement GetToAlignContainer(ComboBox comboBox)
        {
            DependencyObject container;
            if (comboBox.SelectedItem is null)
            {
                container = comboBox.ItemContainerGenerator.ContainerFromIndex(
                    (int)Math.Ceiling(comboBox.Items.Count / 2.0));

                if (comboBox.ItemContainerGenerator.ContainerFromIndex(0) is ComboBoxItem item)
                {
                    var highlightedInfoProperty = typeof(ComboBox).GetProperty("HighlightedInfo",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                    var setter = highlightedInfoProperty.SetMethod;

                    var itemInfo = typeof(ComboBox)
                        .GetMethod("ItemInfoFromContainer", BindingFlags.Instance | BindingFlags.NonPublic)?
                        .Invoke(comboBox, [item]);

                    setter?.Invoke(comboBox, [itemInfo]);
                }
            }
            else
            {
                container = comboBox.ItemContainerGenerator.ContainerFromItem(comboBox.SelectedItem);
            }

            return container as FrameworkElement;
        }

        private static void UpdateCornerRadius(ComboBox comboBox, Popup? popup, bool isDropDownOpen, bool isOpenDown)
        {
            var textBoxRadius = ControlHelper.GetCornerRadius(comboBox);
            var popupRadius = (CornerRadius)ResourceLookup(comboBox, c_overlayCornerRadiusKey);

            if (isDropDownOpen)
            {
                if (popup?.VerticalOffset is 0)
                {
                    popupRadius = GetFilteredPopupRadius(popupRadius, isOpenDown);
                }

                var textBoxRadiusFilter = isOpenDown ? CornerRadiusFilterKind.Top : CornerRadiusFilterKind.Bottom;
                textBoxRadius = CornerRadiusFilterConverter.Convert(textBoxRadius, textBoxRadiusFilter);
            }

            if (GetTemplateChild<Border>(c_popupBorderName, comboBox) is { } popupBorder)
            {
                popupBorder.CornerRadius = popupRadius;
            }

            if (comboBox.IsEditable)
            {
                if (GetTemplateChild<TextBox>(c_editableTextName, comboBox) is { } textBox)
                {
                    ControlHelper.SetCornerRadius(textBox, textBoxRadius);
                }
            }
            else
            {
                if (GetTemplateChild<Border>(c_backgroundName, comboBox) is { } background)
                {
                    background.CornerRadius = textBoxRadius;
                }

                if (GetTemplateChild<Border>(c_highlightBackgroundName, comboBox) is { } highlightBackground)
                {
                    highlightBackground.CornerRadius = textBoxRadius;
                }
            }
        }

        private static CornerRadius GetFilteredPopupRadius(CornerRadius popupRadius, bool isOpenDown)
        {
            var popupRadiusFilter = isOpenDown ? CornerRadiusFilterKind.Bottom : CornerRadiusFilterKind.Top;
            return CornerRadiusFilterConverter.Convert(popupRadius, popupRadiusFilter);
        }

        private static bool IsPopupOpenDown(ComboBox comboBox, double popupVerticalOffset)
        {
            double verticalOffset = popupVerticalOffset;
            if (GetTemplateChild<Border>(c_popupBorderName, comboBox) is { } popupBorder)
            {
                if (GetTemplateChild<TextBox>(c_editableTextName, comboBox) is { } textBox)
                {
                    var popupTop = popupBorder.TranslatePoint(new Point(0, 0), textBox);
                    verticalOffset = popupTop.Y;
                }
            }

            return verticalOffset > popupVerticalOffset;
        }

        private static object ResourceLookup(Control control, object key)
        {
            return control.TryFindResource(key);
        }

        private static T GetTemplateChild<T>(string childName, Control control) where T : DependencyObject
        {
            return control.Template?.FindName(childName, control) as T;
        }
    }
}