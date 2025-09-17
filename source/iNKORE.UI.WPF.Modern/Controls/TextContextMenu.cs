using iNKORE.UI.WPF.Modern.Common;
using iNKORE.UI.WPF.Modern.Common.IconKeys;
using iNKORE.UI.WPF.Modern.Controls.Helpers;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using static iNKORE.UI.WPF.Modern.Common.ResourceAccessor;

namespace iNKORE.UI.WPF.Modern.Controls
{
    /// <summary>
    /// Represents a specialized context menu that contains commands for editing text.
    /// </summary>
    public class TextContextMenu : ContextMenu
    {
        private static readonly ResourceAccessor ResourceAccessor = new ResourceAccessor(typeof(TextContextMenu));

        private static readonly CommandBinding _selectAllBinding;
        private static readonly CommandBinding _undoBinding;
        private static readonly CommandBinding _redoBinding;

        private readonly MenuItem _proofingMenuItem;

        static TextContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextContextMenu), new FrameworkPropertyMetadata(typeof(TextContextMenu)));

            _selectAllBinding = new CommandBinding(ApplicationCommands.SelectAll);
            _selectAllBinding.PreviewCanExecute += OnSelectAllPreviewCanExecute;

            _undoBinding = new CommandBinding(ApplicationCommands.Undo);
            _undoBinding.PreviewCanExecute += OnUndoRedoPreviewCanExecute;

            _redoBinding = new CommandBinding(ApplicationCommands.Redo);
            _redoBinding.PreviewCanExecute += OnUndoRedoPreviewCanExecute;
        }

        /// <summary>
        /// Initializes a new instance of the TextContextMenu class.
        /// </summary>
        public TextContextMenu()
        {
            _proofingMenuItem = new MenuItem();
            Items.Add(_proofingMenuItem);
            Items.Add(new MenuItem
            {
                Command = ApplicationCommands.Cut,
                Icon = new FontIcon(SegoeFluentIcons.Cut)
            });
            Items.Add(new MenuItem
            {
                Command = ApplicationCommands.Copy,
                Icon = new FontIcon(SegoeFluentIcons.Copy)
            });
            Items.Add(new MenuItem
            {
                Command = ApplicationCommands.Paste,
                Icon = new FontIcon(SegoeFluentIcons.Paste)
            });
            Items.Add(new MenuItem
            {
                Command = ApplicationCommands.Undo,
                Icon = new FontIcon(SegoeFluentIcons.Undo)
            });
            Items.Add(new MenuItem
            {
                Command = ApplicationCommands.Redo,
                Icon = new FontIcon(SegoeFluentIcons.Redo)
            });
            Items.Add(new MenuItem
            {
                Command = ApplicationCommands.SelectAll
            });
        }

        #region UsingTextContextMenu

        public static readonly DependencyProperty UsingTextContextMenuProperty =
            DependencyProperty.RegisterAttached(
                "UsingTextContextMenu",
                typeof(bool),
                typeof(TextContextMenu),
                new PropertyMetadata(false, OnUsingTextContextMenuChanged));

        public static bool GetUsingTextContextMenu(Control textControl)
        {
            return (bool)textControl.GetValue(UsingTextContextMenuProperty);
        }

        public static void SetUsingTextContextMenu(Control textControl, bool value)
        {
            textControl.SetValue(UsingTextContextMenuProperty, value);
        }

        private static void OnUsingTextContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textControl = (Control)d;
            if ((bool)e.NewValue)
            {
                textControl.CommandBindings.Add(_selectAllBinding);
                textControl.CommandBindings.Add(_undoBinding);
                textControl.CommandBindings.Add(_redoBinding);
                textControl.ContextMenuOpening += OnContextMenuOpening;
            }
            else
            {
                textControl.CommandBindings.Remove(_selectAllBinding);
                textControl.CommandBindings.Remove(_undoBinding);
                textControl.CommandBindings.Remove(_redoBinding);
                textControl.ContextMenuOpening -= OnContextMenuOpening;
            }
        }

        #endregion

        protected override void OnOpened(RoutedEventArgs e)
        {
            base.OnOpened(e);

            if (_proofingMenuItem.IsVisible)
            {
                _proofingMenuItem.IsSubmenuOpen = true;
            }
        }

        protected override void OnClosed(RoutedEventArgs e)
        {
            base.OnClosed(e);

            if (!IsOpen)
            {
                _proofingMenuItem.Items.Clear();

                foreach (MenuItem menuItem in Items)
                {
                    menuItem.ClearValue(MenuItem.CommandTargetProperty);
                }
            }
        }

        private static void OnSelectAllPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
            {
                e.CanExecute = false;
                e.Handled = true;
            }
            else if (sender is PasswordBox passwordBox && string.IsNullOrEmpty(passwordBox.Password))
            {
                e.CanExecute = false;
                e.Handled = true;
            }
        }

        private static void OnUndoRedoPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is TextBoxBase textBoxBase && textBoxBase.IsReadOnly)
            {
                e.CanExecute = false;
                e.Handled = true;
            }
        }

        private static void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var textControl = (Control)sender;
            if (textControl.ContextMenu is TextContextMenu contextMenu)
            {
                Control target;
                if (textControl is PasswordBox passwordBox &&
                    PasswordBoxHelper.GetPasswordRevealMode(passwordBox) == PasswordRevealMode.Visible &&
                    e.Source is TextBox)
                {
                    target = (Control)e.Source;
                }
                else
                {
                    target = textControl;
                }

                contextMenu.UpdateItems(target);

                bool hasVisibleItems = contextMenu.Items.OfType<MenuItem>().Any(mi => mi.Visibility == Visibility.Visible);
                if (!hasVisibleItems)
                {
                    e.Handled = true;
                }
            }
        }

        private void UpdateProofingMenuItem(Control target)
        {
            _proofingMenuItem.Header = ResourceAccessor.GetLocalizedStringResource(SR_ProofingMenuItemLabel);
            _proofingMenuItem.Items.Clear();

            SpellingError spellingError = null;

            if (target is TextBox textBox)
            {
                spellingError = textBox.GetSpellingError(textBox.CaretIndex);
            }
            else if (target is RichTextBox richTextBox)
            {
                spellingError = richTextBox.GetSpellingError(richTextBox.CaretPosition);
            }

            if (spellingError != null)
            {
                foreach (string suggestion in spellingError.Suggestions)
                {
                    var menuItem = new MenuItem
                    {
                        Header = suggestion,
                        Command = EditingCommands.CorrectSpellingError,
                        CommandParameter = suggestion,
                        CommandTarget = target
                    };
                    _proofingMenuItem.Items.Add(menuItem);
                }

                if (_proofingMenuItem.HasItems)
                {
                    _proofingMenuItem.Items.Add(new Separator());
                }

                _proofingMenuItem.Items.Add(new MenuItem
                {
                    Header = Strings.IgnoreMenuItemLabel,
                    Command = EditingCommands.IgnoreSpellingError,
                    CommandTarget = target
                });

                _proofingMenuItem.Visibility = Visibility.Visible;
            }
            else
            {
                _proofingMenuItem.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateItems(Control target)
        {
            UpdateProofingMenuItem(target);

            foreach (MenuItem menuItem in Items)
            {
                if (menuItem.Command is RoutedUICommand command)
                {
                    // Local helper to build a tooltip that centers horizontally above the mouse cursor
                    ToolTip BuildCenteredMouseToolTip(string text)
                    {
                        var tt = new ToolTip
                        {
                            Content = text,
                            Placement = PlacementMode.Mouse
                        };
                        tt.Opened += (s, e) =>
                        {
                            if (s is ToolTip t)
                            {
                                t.HorizontalOffset = 0;
                                t.VerticalOffset = 0;
                                void ApplyOffsets()
                                {
                                    const double gap = 32;
                                    t.HorizontalOffset = -t.ActualWidth / 2.0;
                                    t.VerticalOffset = -(t.ActualHeight + gap);
                                }

                                if (t.ActualWidth <= 0 || t.ActualHeight <= 0)
                                {
                                    t.Dispatcher.BeginInvoke((System.Action)(() =>
                                    {
                                        ApplyOffsets();
                                    }));
                                }
                                else
                                {
                                    ApplyOffsets();
                                }
                            }
                        };
                        return tt;
                    }

                    if (command == ApplicationCommands.Cut)
                    {
                        menuItem.Header = ResourceAccessor.GetLocalizedStringResource(SR_TextCommandLabelCut);
                        var cutDescription = ResourceAccessor.GetLocalizedStringResource(SR_TextCommandDescriptionCut);
                        menuItem.ToolTip = BuildCenteredMouseToolTip(cutDescription);
                        ToolTipService.SetInitialShowDelay(menuItem, 700);
                        ToolTipService.SetShowDuration(menuItem, 10000);
                    }
                    else if (command == ApplicationCommands.Copy)
                    {
                        menuItem.Header = ResourceAccessor.GetLocalizedStringResource(SR_TextCommandLabelCopy);
                        var copyDescription = ResourceAccessor.GetLocalizedStringResource(SR_TextCommandDescriptionCopy);
                        menuItem.ToolTip = BuildCenteredMouseToolTip(copyDescription);
                        ToolTipService.SetInitialShowDelay(menuItem, 700);
                        ToolTipService.SetShowDuration(menuItem, 10000);
                    }
                    else if (command == ApplicationCommands.Paste)
                    {
                        menuItem.Header = ResourceAccessor.GetLocalizedStringResource(SR_TextCommandLabelPaste);
                        var pasteDescription = ResourceAccessor.GetLocalizedStringResource(SR_TextCommandDescriptionPaste);
                        menuItem.ToolTip = BuildCenteredMouseToolTip(pasteDescription);
                        ToolTipService.SetInitialShowDelay(menuItem, 700);
                        ToolTipService.SetShowDuration(menuItem, 10000);
                    }
                    else if (command == ApplicationCommands.Undo)
                    {
                        menuItem.Header = ResourceAccessor.GetLocalizedStringResource(SR_TextCommandLabelUndo);
                        var undoDescription = ResourceAccessor.GetLocalizedStringResource(SR_TextCommandDescriptionUndo);
                        menuItem.ToolTip = BuildCenteredMouseToolTip(undoDescription);
                        ToolTipService.SetInitialShowDelay(menuItem, 700);
                        ToolTipService.SetShowDuration(menuItem, 10000);
                    }
                    else if (command == ApplicationCommands.Redo)
                    {
                        menuItem.Header = ResourceAccessor.GetLocalizedStringResource(SR_TextCommandLabelRedo);
                        var redoDescription = ResourceAccessor.GetLocalizedStringResource(SR_TextCommandDescriptionRedo);
                        menuItem.ToolTip = BuildCenteredMouseToolTip(redoDescription);
                        ToolTipService.SetInitialShowDelay(menuItem, 700);
                        ToolTipService.SetShowDuration(menuItem, 10000);
                    }
                    else if (command == ApplicationCommands.SelectAll)
                    {
                        menuItem.Header = ResourceAccessor.GetLocalizedStringResource(SR_TextCommandLabelSelectAll);
                        var selectAllDescription = ResourceAccessor.GetLocalizedStringResource(SR_TextCommandDescriptionSelectAll);
                        menuItem.ToolTip = BuildCenteredMouseToolTip(selectAllDescription);
                        ToolTipService.SetInitialShowDelay(menuItem, 700);
                        ToolTipService.SetShowDuration(menuItem, 10000);
                    }

                    menuItem.CommandTarget = target;
                    menuItem.Visibility = command.CanExecute(null, target) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
    }
}
