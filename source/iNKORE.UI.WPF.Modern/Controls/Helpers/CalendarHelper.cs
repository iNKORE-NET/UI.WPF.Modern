using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Calendar = System.Windows.Controls.Calendar;

namespace iNKORE.UI.WPF.Modern.Controls.Helpers
{
    public static class CalendarHelper
    {
        #region AutoReleaseMouseCapture

        public static bool GetAutoReleaseMouseCapture(Calendar calendar)
        {
            return (bool)calendar.GetValue(AutoReleaseMouseCaptureProperty);
        }

        public static void SetAutoReleaseMouseCapture(Calendar calendar, bool value)
        {
            calendar.SetValue(AutoReleaseMouseCaptureProperty, value);
        }

        public static readonly DependencyProperty AutoReleaseMouseCaptureProperty = DependencyProperty.RegisterAttached(
            "AutoReleaseMouseCapture",
            typeof(bool),
            typeof(CalendarHelper),
            new PropertyMetadata(OnAutoReleaseMouseCaptureChanged));

        private static void OnAutoReleaseMouseCaptureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var calendar = (Calendar)d;
            if ((bool)e.NewValue)
            {
                calendar.GotMouseCapture += OnCalendarGotMouseCapture;
            }
            else
            {
                calendar.GotMouseCapture -= OnCalendarGotMouseCapture;
            }
        }

        #endregion

        private static void OnCalendarGotMouseCapture(object sender, MouseEventArgs e)
        {
            var calendar = (Calendar)sender;
            if (calendar.SelectionMode != CalendarSelectionMode.MultipleRange)
            {
                UIElement originalElement = e.OriginalSource as UIElement;
                if (originalElement is CalendarDayButton || originalElement is CalendarItem)
                {
                    originalElement.ReleaseMouseCapture();
                }
            }
        }
    
        #region CalendarKeyboardBehaviorOverride

        public static bool GetKeyboardBehaviorOverride(Calendar calendar) =>
            (bool)calendar.GetValue(KeyboardBehaviorOverrideProperty);

        public static void SetKeyboardBehaviorOverride(Calendar calendar, bool value) =>
            calendar.SetValue(KeyboardBehaviorOverrideProperty, value);

        public static readonly DependencyProperty KeyboardBehaviorOverrideProperty =
            DependencyProperty.RegisterAttached(
                "KeyboardBehaviorOverride",
                typeof(bool),
                typeof(CalendarHelper),
                new PropertyMetadata(OnKeyboardBehaviorOverrideChanged));

        private static void OnKeyboardBehaviorOverrideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Calendar calendar)
            {
                return;
            }

            if (e.NewValue is true)
            {
                calendar.PreviewKeyDown += OnHandleChangeFocusByKeys;
            }
            else
            {
                calendar.PreviewKeyDown -= OnHandleChangeFocusByKeys;
            }
        }

        private static void OnHandleChangeFocusByKeys(object sender, KeyEventArgs e)
        {
            if (sender is not Calendar calendar ||
                (Keyboard.Modifiers & ModifierKeys.Control) is ModifierKeys.Control)
            {
                return;
            }

            if ((Keyboard.Modifiers & ModifierKeys.Shift) is ModifierKeys.Shift && e.Key is not Key.Tab)
            {
                e.Handled = true;
                return;
            }

            DateTime? focusTarget;
            switch (calendar.DisplayMode)
            {
                case CalendarMode.Month:
                    int? change = e.Key switch
                    {
                        Key.Up => -7,
                        Key.Down => 7,
                        Key.Left => -1,
                        Key.Right => 1,
                        _ => null
                    };

                    if (change is null)
                    {
                        return;
                    }

                    var currentDate = typeof(Calendar).GetProperty("CurrentDate",
                                BindingFlags.NonPublic | BindingFlags.Instance)!
                            .GetValue(calendar)
                        as DateTime?;

                    focusTarget = GetNonBlackoutTarget(calendar, currentDate?.AddDays(change.Value));

                    break;

                case CalendarMode.Year:
                    if (GetChangeForYearOrDecadeView(e.Key) is not { } monthChange)
                    {
                        return;
                    }

                    focusTarget = calendar.DisplayDate.AddMonths(monthChange);
                    break;
                case CalendarMode.Decade:
                    if (GetChangeForYearOrDecadeView(e.Key) is not { } yearChange)
                    {
                        return;
                    }

                    focusTarget = calendar.DisplayDate.AddYears(yearChange);
                    break;
                default:
                    return;
            }

            typeof(Calendar).GetMethod("MoveDisplayTo", BindingFlags.NonPublic | BindingFlags.Instance)!
                .Invoke(calendar,
                    [focusTarget]);

            e.Handled = true;

            static int? GetChangeForYearOrDecadeView(Key key) => key switch
            {
                Key.Up => -4,
                Key.Down => 4,
                Key.Left => -1,
                Key.Right => 1,
                _ => null
            };

            static DateTime? GetNonBlackoutTarget(Calendar calendar, DateTime? targetFocus)
            {
                var blackoutDates =
                    typeof(Calendar).GetField("_blackoutDates", BindingFlags.NonPublic | BindingFlags.Instance)!
                            .GetValue(calendar)
                        as CalendarBlackoutDatesCollection;

                var toSelectDate =
                    typeof(CalendarBlackoutDatesCollection).GetMethod("GetNonBlackoutDate",
                            BindingFlags.NonPublic | BindingFlags.Instance)!
                        .Invoke(blackoutDates, [targetFocus, -1]) as DateTime?;
                return toSelectDate;
            }
        }

        private static DateTime? GetCurrentFocusedDate() =>
            (Keyboard.FocusedElement as FrameworkElement)?.DataContext as DateTime?;

        #endregion

        #region ContainsToday

        public static CalendarMode? GetCalendarDisplayMode(DependencyObject obj) => (CalendarMode?)obj.GetValue(CalendarDisplayModeProperty);

        public static void SetCalendarDisplayMode(DependencyObject obj, CalendarMode? value) => obj.SetValue(CalendarDisplayModeProperty, value);

        public static readonly DependencyProperty CalendarDisplayModeProperty =
            DependencyProperty.RegisterAttached(
                "CalendarDisplayMode",
                typeof(CalendarMode?),
                typeof(CalendarHelper),
                new PropertyMetadata(null, OnContainsTodayNeedsUpdate));

        public static object GetContextDate(DependencyObject obj) => obj.GetValue(ContextDateProperty);

        public static void SetContextDate(DependencyObject obj, object value) => obj.SetValue(ContextDateProperty, value);

        public static readonly DependencyProperty ContextDateProperty =
            DependencyProperty.RegisterAttached(
                "ContextDate",
                typeof(object),
                typeof(CalendarHelper),
                new PropertyMetadata(null, OnContainsTodayNeedsUpdate));

        public static bool GetIsContainsTodayActive(DependencyObject obj) => (bool)obj.GetValue(IsContainsTodayActiveProperty);

        public static void SetIsContainsTodayActive(DependencyObject obj, bool value) => obj.SetValue(IsContainsTodayActiveProperty, value);

        public static readonly DependencyProperty IsContainsTodayActiveProperty =
            DependencyProperty.RegisterAttached(
                "IsContainsTodayActive",
                typeof(bool),
                typeof(CalendarHelper),
                new PropertyMetadata(true, OnContainsTodayNeedsUpdate));

        private static void OnContainsTodayNeedsUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateContainsTodayActive(d);
        }

        private static void UpdateContainsTodayActive(DependencyObject d)
        {
            var mode = GetCalendarDisplayMode(d);
            var contextDate = GetContextDate(d) as DateTime?;

            if (!mode.HasValue || !contextDate.HasValue)
            {
                SetContainsToday(d, false);
                return;
            }

            var containsTodayActive = mode.Value switch
            {
                CalendarMode.Year => contextDate.Value.Year == DateTime.Today.Year &&
                                     contextDate.Value.Month == DateTime.Today.Month,
                CalendarMode.Decade => contextDate.Value.Year == DateTime.Today.Year,
                _ => false
            };

            SetContainsToday(d, containsTodayActive);
        }

        public static bool GetContainsToday(DependencyObject obj) =>
            (bool)obj.GetValue(ContainsTodayProperty);

        public static void SetContainsToday(DependencyObject obj, bool value) =>
            obj.SetValue(ContainsTodayProperty, value);

        public static readonly DependencyProperty ContainsTodayProperty =
            DependencyProperty.RegisterAttached(
                "ContainsToday",
                typeof(bool),
                typeof(CalendarHelper),
                new PropertyMetadata(false));

        #endregion
    }
}
