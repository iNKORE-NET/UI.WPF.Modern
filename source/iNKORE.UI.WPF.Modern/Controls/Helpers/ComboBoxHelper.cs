// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using iNKORE.UI.WPF.Converters;
using iNKORE.UI.WPF.Helpers;
using iNKORE.UI.WPF.Modern.Common.Converters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace iNKORE.UI.WPF.Modern.Controls.Helpers
{
    public sealed class ComboBoxHelper
    {
        private const string c_popupBorderName = "PopupBorder";
        private const string c_editableTextName = "PART_EditableTextBox";
        //private const string c_editableTextBorderName = "BorderElement";
        private const string c_backgroundName = "Background";
        private const string c_highlightBackgroundName = "HighlightBackground";
        //private const string c_controlCornerRadiusKey = "ControlCornerRadius";
        private const string c_overlayCornerRadiusKey = "OverlayCornerRadius";

        internal ComboBoxHelper()
        {
        }

        /// <summary>
        /// Identifies the TextBoxStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty TextBoxStyleProperty =
            DependencyProperty.RegisterAttached(
                "TextBoxStyle",
                typeof(Style),
                typeof(ComboBoxHelper),
                null);

        /// <summary>
        /// Gets the style of the TextBox in the ComboBox when the ComboBox is editable.
        /// </summary>
        /// <param name="comboBox">The element from which to read the property value.</param>
        /// <returns>The style of the TextBox in the ComboBox when the ComboBox is editable.</returns>
        public static Style GetTextBoxStyle(ComboBox comboBox)
        {
            return (Style)comboBox.GetValue(TextBoxStyleProperty);
        }

        /// <summary>
        /// Sets the style of the TextBox in the ComboBox when the ComboBox is editable.
        /// </summary>
        /// <param name="comboBox">The element on which to set the attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetTextBoxStyle(ComboBox comboBox, Style value)
        {
            comboBox.SetValue(TextBoxStyleProperty, value);
        }

        public static readonly DependencyProperty KeepInteriorCornersSquareProperty =
            DependencyProperty.RegisterAttached(
                "KeepInteriorCornersSquare",
                typeof(bool),
                typeof(ComboBoxHelper),
                new PropertyMetadata(false, OnKeepInteriorCornersSquareChanged));

        public static bool GetKeepInteriorCornersSquare(ComboBox comboBox)
        {
            return (bool)comboBox.GetValue(KeepInteriorCornersSquareProperty);
        }

        public static void SetKeepInteriorCornersSquare(ComboBox comboBox, bool value)
        {
            comboBox.SetValue(KeepInteriorCornersSquareProperty, value);
        }

        private static void OnKeepInteriorCornersSquareChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
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

        private static void OnDropDownOpened(object sender, object args)
        {
            var comboBox = (ComboBox)sender;
            // We need to know whether the dropDown opens above or below the ComboBox in order to update corner radius correctly.
            // Sometimes TransformToPoint value is incorrect because popup is not fully opened when this function gets called.
            // Use dispatcher to make sure we get correct VerticalOffset.
            comboBox.Dispatcher.BeginInvoke(() =>
                {
                    UpdateCornerRadius(comboBox, /*IsDropDownOpen=*/true);
                });
        }

        private static void OnDropDownClosed(object sender, object args)
        {
            var comboBox = (ComboBox)sender;
            UpdateCornerRadius(comboBox, /*IsDropDownOpen=*/false);
        }

        private static void UpdateCornerRadius(ComboBox comboBox, bool isDropDownOpen)
        {
            var textBoxRadius = ControlHelper.GetCornerRadius(comboBox);
            var popupRadius = (CornerRadius)ResourceLookup(comboBox, c_overlayCornerRadiusKey);

            if (isDropDownOpen)
            {
                var popup = GetTemplateChild<System.Windows.Controls.Primitives.Popup>("PART_Popup", comboBox);
                bool isOpenDown = IsPopupOpenDown(comboBox, popup.VerticalOffset);

                if (isOpenDown &&
                    GetToAlignContainer(comboBox) is { } itemContainer &&
                    itemContainer.TranslatePoint(new Point(0, -itemContainer.ActualHeight + comboBox.Padding.Top), comboBox) is { Y: not 0 } itemTop)
                {
                    popup.VerticalOffset -= itemTop.Y;

                    if (itemContainer.ActualHeight - comboBox.ActualHeight > 0)
                    {
                        popup.VerticalOffset -= comboBox.ActualHeight;
                    }
                }

                /*popup.HorizontalOffset = popup.Child switch
                {
                    FrameworkElement fe when fe.ActualWidth > comboBox.ActualWidth =>
                        -(fe.ActualWidth - comboBox.ActualWidth) / 2,
                    _ => 0
                };*/

                if (popup.VerticalOffset is 0)
                {
                    popupRadius = GetFilteredPopupRadius(popupRadius, isOpenDown);
                }

                var textBoxRadiusFilter = isOpenDown ? CornerRadiusFilterKind.Top : CornerRadiusFilterKind.Bottom;
                textBoxRadius = CornerRadiusFilterConverter.Convert(textBoxRadius, textBoxRadiusFilter);
            }

            if (GetTemplateChild<Border>(c_popupBorderName, comboBox) is Border popupBorder)
            {
                popupBorder.CornerRadius = popupRadius;
            }

            if (comboBox.IsEditable)
            {
                if (GetTemplateChild<TextBox>(c_editableTextName, comboBox) is TextBox textBox)
                {
                    ControlHelper.SetCornerRadius(textBox, textBoxRadius);
                }
            }
            else
            {
                if (GetTemplateChild<Border>(c_backgroundName, comboBox) is Border background)
                {
                    background.CornerRadius = textBoxRadius;
                }

                if (GetTemplateChild<Border>(c_highlightBackgroundName, comboBox) is Border highlightBackground)
                {
                    highlightBackground.CornerRadius = textBoxRadius;
                }
            }

            static FrameworkElement? GetToAlignContainer(ComboBox comboBox)
            {
                DependencyObject container;
                if (comboBox.SelectedItem is null)
                {
                    container = comboBox.ItemContainerGenerator.ContainerFromIndex(
                        (int)Math.Ceiling(comboBox.Items.Count / 2.0));

                    if (comboBox.ItemContainerGenerator.ContainerFromIndex(0) is ComboBoxItem item)
                    {
                        //item.IsSelected = true;
                        //item.IsPseudoSelected = true;
                    }
                }
                else
                {
                    container = comboBox.ItemContainerGenerator.ContainerFromItem(comboBox.SelectedItem);
                }

                return container as  FrameworkElement;
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
            if (GetTemplateChild<Border>(c_popupBorderName, comboBox) is Border popupBorder)
            {
                if (GetTemplateChild<TextBox>(c_editableTextName, comboBox) is TextBox textBox)
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
