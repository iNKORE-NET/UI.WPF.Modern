using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Automation;
using ColorCode;
using ColorCode.Common;
using ColorCode.Styling;
using ColorCode.Wpf;
using SamplesCommon;
using iNKORE.UI.WPF.Modern;
using iNKORE.UI.WPF.Modern.Gallery.Helpers;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;

namespace iNKORE.UI.WPF.Modern.Gallery.Controls
{
    /// <summary>
    /// SampleCodePresenter.xaml 的交互逻辑
    /// </summary>
    public partial class SampleCodePresenter : UserControl
    {
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register(
                nameof(Code),
                typeof(string),
                typeof(SampleCodePresenter),
                new PropertyMetadata(string.Empty, OnCodeOrSampleChanged));

        public string Code
        {
            get => (string)GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
        }

        public static readonly DependencyProperty IsCSharpSampleProperty =
            DependencyProperty.Register(
                nameof(IsCSharpSample),
                typeof(bool),
                typeof(SampleCodePresenter),
                new PropertyMetadata(false, OnCodeOrSampleChanged));

        public bool IsCSharpSample
        {
            get => (bool)GetValue(IsCSharpSampleProperty);
            set => SetValue(IsCSharpSampleProperty, value);
        }

        public static readonly DependencyProperty SubstitutionsProperty =
            DependencyProperty.Register(
                nameof(Substitutions),
                typeof(ObservableCollection<ControlExampleSubstitution>),
                typeof(SampleCodePresenter),
                new PropertyMetadata(null, OnSubstitutionsChanged));

        public ObservableCollection<ControlExampleSubstitution> Substitutions
        {
            get => (ObservableCollection<ControlExampleSubstitution>)GetValue(SubstitutionsProperty);
            set
            {
                if (value is null)
                    ClearValue(SubstitutionsProperty);
                else
                    SetValue(SubstitutionsProperty, value);
            }
        }

        public bool IsEmpty => string.IsNullOrEmpty(Code);

        private static readonly Regex SubstitutionPattern =
            new Regex(@"\$\(([^)]+)\)", RegexOptions.Compiled);

        private readonly DispatcherTimer _resetTimer;
        private string _actualCode = string.Empty;
        private string _lastHighlightedCode = null;
        private bool _lastIsDark = false;

        public SampleCodePresenter()
        {
            InitializeComponent();

            // Timer to reset visual state after copy
            _resetTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _resetTimer.Tick += (s, e) =>
            {
                _resetTimer.Stop();
                VisualStateManager.GoToState(this, "ConfirmationDialogHidden", false);
            };

            // Hook context menu
            CodePresenter.ContextMenuOpening += CodePresenter_ContextMenuOpening;
        }

        private static void OnCodeOrSampleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (SampleCodePresenter)d;
            ctl.UpdateVisibilityAndHighlight();
        }

        private static void OnSubstitutionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (SampleCodePresenter)d;
            if (e.OldValue is ObservableCollection<ControlExampleSubstitution> old)
            {
                foreach (var sub in old)
                    sub.ValueChanged -= ctl.OnSubstitutionValueChanged;
            }
            if (e.NewValue is ObservableCollection<ControlExampleSubstitution> neu)
            {
                foreach (var sub in neu)
                    sub.ValueChanged += ctl.OnSubstitutionValueChanged;
            }
        }

        private void OnSubstitutionValueChanged(ControlExampleSubstitution sender, object newValue)
        {
            GenerateSyntaxHighlightedContent();
        }

        private void SampleCodePresenter_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateVisibilityAndHighlight();
        }

        private void SampleCodePresenter_ActualThemeChanged(object sender, RoutedEventArgs e)
        {
            GenerateSyntaxHighlightedContent();
        }

        private void UpdateVisibilityAndHighlight()
        {
            if (IsEmpty)
            {
                Visibility = Visibility.Collapsed;
                return;
            }

            Visibility = Visibility.Visible;
            SampleHeader.Text = IsCSharpSample ? "C#" : "XAML";
            GenerateSyntaxHighlightedContent();
        }

        private void GenerateSyntaxHighlightedContent()
        {
            // Prevent NullReferenceException if controls are not loaded yet
            if (CodePresenter == null || InlinePresenter == null || CopyCodeButton == null)
                return;

            // Determine language
            ILanguage lang = IsCSharpSample ? Languages.CSharp : Languages.Xml;
            string raw = Code ?? string.Empty;

            // Apply substitutions
            if (Substitutions != null && Substitutions.Count > 0)
            {
                raw = SubstitutionPattern.Replace(raw, match =>
                {
                    var key = match.Groups[1].Value;
                    var sub = Substitutions.FirstOrDefault(s => s.Key == key);
                    if (sub == null)
                        throw new KeyNotFoundException(key);

                    return sub.ValueAsString();
                });
            }

            _actualCode = raw.Trim('\n').TrimEnd();

            // Update automation name
            AutomationProperties.SetName(CopyCodeButton,
                $"Copy {(IsCSharpSample ? "C#" : "XAML")} Code");

            // Show inline or full
            bool isInline = !IsCSharpSample && raw.Length < 100; 
            if (isInline)
            {
                InlinePresenter.Visibility = Visibility.Visible;
                CodePresenter.Visibility = Visibility.Collapsed;

                var tb = new TextBox
                {
                    Text = _actualCode,
                    FontFamily = new FontFamily("Consolas"),
                    IsReadOnly = true,
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    //TextTrimming = TextTrimming.CharacterEllipsis
                };
                InlinePresenter.Content = tb;
            }
            else
            {
                InlinePresenter.Visibility = Visibility.Collapsed;
                CodePresenter.Visibility = Visibility.Visible;

                bool isDark = ThemeHelper.IsDarkTheme();

                // Only update if code or theme changed
                if (_actualCode == _lastHighlightedCode && isDark == _lastIsDark)
                    return;

                _lastHighlightedCode = _actualCode;
                _lastIsDark = isDark;

                // Use current theme for styling, do not set/change theme here
                var style = isDark
                    ? StyleDictionary.DefaultDark
                    : StyleDictionary.DefaultLight;
                var formatter = new RichTextBoxFormatter(style);

                // Dark theme tweaks
                if (isDark)
                {
                    // Remove old styles by key
                    var removeKeys = new[] {
                        ScopeName.XmlAttribute,
                        ScopeName.XmlAttributeQuotes,
                        ScopeName.XmlAttributeValue,
                        ScopeName.HtmlComment,
                        ScopeName.XmlDelimiter,
                        ScopeName.XmlName
                    };
                    foreach (var key in removeKeys)
                        formatter.Styles.Remove(key);

                    // Add custom styles
                    formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttribute)
                    {
                        Foreground = "#FF87CEFA",
                        ReferenceName = "xmlAttribute"
                    });
                    formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttributeQuotes)
                    {
                        Foreground = "#FFFFA07A",
                        ReferenceName = "xmlAttributeQuotes"
                    });
                    formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttributeValue)
                    {
                        Foreground = "#FFFFA07A",
                        ReferenceName = "xmlAttributeValue"
                    });
                    formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.HtmlComment)
                    {
                        Foreground = "#FF6B8E23",
                        ReferenceName = "htmlComment"
                    });
                    formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlDelimiter)
                    {
                        Foreground = "#FF808080",
                        ReferenceName = "xmlDelimiter"
                    });
                    formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlName)
                    {
                        Foreground = "#FF5F82E8",
                        ReferenceName = "xmlName"
                    });
                }

                // Clear the document before formatting
                CodePresenter.Document.Blocks.Clear();
                formatter.FormatRichTextBox(_actualCode, lang, CodePresenter);
            }
        }

        private void CopyCodeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(_actualCode);

                VisualStateManager.GoToState(this, "ConfirmationDialogVisible", false);
                _resetTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Unable to Perform Copy", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CodePresenter_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (CodePresenter.ContextMenu == null)
            {
                var menu = new ContextMenu();
                menu.Items.Add(new MenuItem { Header = "Copy", Command = ApplicationCommands.Copy });
                menu.Items.Add(new MenuItem { Header = "Select All", Command = ApplicationCommands.SelectAll });
                CodePresenter.ContextMenu = menu;
            }
        }
    }
}
