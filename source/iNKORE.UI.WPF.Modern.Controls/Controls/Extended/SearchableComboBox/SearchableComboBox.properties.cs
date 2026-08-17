using System.Windows;
using iNKORE.UI.WPF.Modern.Common;
using static iNKORE.UI.WPF.Modern.Common.ResourceAccessor;

namespace iNKORE.UI.WPF.Modern.Controls
{
    public partial class SearchableComboBox
    {
        private static ControlStrings _resourceAccessor;

        internal static ControlStrings ResourceAccessor =>
            _resourceAccessor ?? (_resourceAccessor = new ControlStrings(typeof(SearchableComboBox), ModernControlCategory.Extended));

        private static string GetString(string key, string fallback)
        {
            var value = ResourceAccessor.GetLocalizedStringResource(key);
            return string.IsNullOrEmpty(value) || value == key ? fallback : value;
        }

        #region IsSearchEnabled

        public static readonly DependencyProperty IsSearchEnabledProperty =
            DependencyProperty.Register(
                nameof(IsSearchEnabled),
                typeof(bool),
                typeof(SearchableComboBox),
                new PropertyMetadata(true, OnIsSearchEnabledChanged));

        /// <summary>
        /// Gets or sets a value that indicates whether the search box is shown in the drop-down.
        /// When <see langword="false"/> the control behaves like a regular
        /// <see cref="System.Windows.Controls.ComboBox"/>. The default is <see langword="true"/>.
        /// </summary>
        public bool IsSearchEnabled
        {
            get => (bool)GetValue(IsSearchEnabledProperty);
            set => SetValue(IsSearchEnabledProperty, value);
        }

        private static void OnIsSearchEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = (SearchableComboBox)d;

            if (!(bool)e.NewValue)
            {
                owner.SearchText = string.Empty;
            }

            owner.UpdateFilter();
        }

        #endregion

        #region SearchText

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(
                nameof(SearchText),
                typeof(string),
                typeof(SearchableComboBox),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSearchTextChanged));

        /// <summary>
        /// Gets or sets the text currently typed in the search box. Bind this property to run the
        /// search against a server or another source; combine it with <see cref="ItemFilter"/> to
        /// take over matching completely.
        /// </summary>
        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((SearchableComboBox)d).UpdateFilter();

        #endregion

        #region SearchBoxPlaceholderText

        public static readonly DependencyProperty SearchBoxPlaceholderTextProperty =
            DependencyProperty.Register(
                nameof(SearchBoxPlaceholderText),
                typeof(string),
                typeof(SearchableComboBox),
                new PropertyMetadata(GetString(SR_SearchableComboBoxSearchPlaceholder, "Search")));

        /// <summary>
        /// Gets or sets the placeholder text of the search box.
        /// </summary>
        public string SearchBoxPlaceholderText
        {
            get => (string)GetValue(SearchBoxPlaceholderTextProperty);
            set => SetValue(SearchBoxPlaceholderTextProperty, value);
        }

        #endregion

        #region NoResultsText

        public static readonly DependencyProperty NoResultsTextProperty =
            DependencyProperty.Register(
                nameof(NoResultsText),
                typeof(string),
                typeof(SearchableComboBox),
                new PropertyMetadata(GetString(SR_SearchableComboBoxNoResults, "No results found")));

        /// <summary>
        /// Gets or sets the message that is shown in place of the list when the search text does
        /// not match any item.
        /// </summary>
        public string NoResultsText
        {
            get => (string)GetValue(NoResultsTextProperty);
            set => SetValue(NoResultsTextProperty, value);
        }

        #endregion

        #region SearchMemberPath

        public static readonly DependencyProperty SearchMemberPathProperty =
            DependencyProperty.Register(
                nameof(SearchMemberPath),
                typeof(string),
                typeof(SearchableComboBox),
                new PropertyMetadata(null, OnSearchDefinitionChanged));

        /// <summary>
        /// Gets or sets the path to the property that is matched against the search text.
        /// Supports nested paths such as <c>Department.Name</c>.
        /// <para>When it is not set the control falls back to
        /// <see cref="System.Windows.Controls.ItemsControl.DisplayMemberPath"/>, then to the
        /// attached <c>TextSearch.TextPath</c>, and finally to <see cref="object.ToString"/>.</para>
        /// </summary>
        public string SearchMemberPath
        {
            get => (string)GetValue(SearchMemberPathProperty);
            set => SetValue(SearchMemberPathProperty, value);
        }

        #endregion

        #region ItemFilter

        public static readonly DependencyProperty ItemFilterProperty =
            DependencyProperty.Register(
                nameof(ItemFilter),
                typeof(SearchableComboBoxFilterCallback),
                typeof(SearchableComboBox),
                new PropertyMetadata(null, OnSearchDefinitionChanged));

        /// <summary>
        /// Gets or sets a callback that replaces the built-in matching logic, for cases the plain
        /// case-insensitive substring test cannot express - matching several members at once,
        /// ignoring accents, or filtering on something other than text. When it is set,
        /// <see cref="SearchMemberPath"/> is not used.
        /// </summary>
        public SearchableComboBoxFilterCallback ItemFilter
        {
            get => (SearchableComboBoxFilterCallback)GetValue(ItemFilterProperty);
            set => SetValue(ItemFilterProperty, value);
        }

        #endregion

        #region ClearSearchTextOnClose

        public static readonly DependencyProperty ClearSearchTextOnCloseProperty =
            DependencyProperty.Register(
                nameof(ClearSearchTextOnClose),
                typeof(bool),
                typeof(SearchableComboBox),
                new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value that indicates whether the search text is cleared every time the
        /// drop-down closes, so the next open starts from the full list.
        /// The default is <see langword="true"/>.
        /// </summary>
        public bool ClearSearchTextOnClose
        {
            get => (bool)GetValue(ClearSearchTextOnCloseProperty);
            set => SetValue(ClearSearchTextOnCloseProperty, value);
        }

        #endregion

        #region SearchBoxStyle

        public static readonly DependencyProperty SearchBoxStyleProperty =
            DependencyProperty.Register(
                nameof(SearchBoxStyle),
                typeof(Style),
                typeof(SearchableComboBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style applied to the <see cref="System.Windows.Controls.TextBox"/>
        /// used as the search box.
        /// </summary>
        public Style SearchBoxStyle
        {
            get => (Style)GetValue(SearchBoxStyleProperty);
            set => SetValue(SearchBoxStyleProperty, value);
        }

        #endregion

        #region HasNoResults

        private static readonly DependencyPropertyKey HasNoResultsPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(HasNoResults),
                typeof(bool),
                typeof(SearchableComboBox),
                new PropertyMetadata(false));

        public static readonly DependencyProperty HasNoResultsProperty = HasNoResultsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates that the current search text matches no item. The default
        /// template hides the list and shows <see cref="NoResultsText"/> while this is
        /// <see langword="true"/>.
        /// </summary>
        public bool HasNoResults
        {
            get => (bool)GetValue(HasNoResultsProperty);
            private set => SetValue(HasNoResultsPropertyKey, value);
        }

        #endregion

        private static void OnSearchDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((SearchableComboBox)d).UpdateFilter();
    }
}
