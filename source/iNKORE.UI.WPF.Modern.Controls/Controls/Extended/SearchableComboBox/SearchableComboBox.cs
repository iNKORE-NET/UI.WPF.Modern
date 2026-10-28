using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using static iNKORE.UI.WPF.Modern.Common.ResourceAccessor;

namespace iNKORE.UI.WPF.Modern.Controls
{
    /// <summary>
    /// Represents a <see cref="ComboBox"/> that shows a search box at the top of its drop-down so
    /// that long lists can be narrowed down by typing. An item matches when its text contains the
    /// search text, ignoring case.
    /// </summary>
    /// <remarks>
    /// <para><b>Filtering is local to the control.</b> When <see cref="ItemsControl.ItemsSource"/>
    /// is a plain collection, the control wraps it in its own <see cref="ICollectionView"/>, so
    /// typing in the search box never filters other controls bound to the same collection. If you
    /// assign an <see cref="ICollectionView"/> yourself, that view is used as-is and its
    /// <see cref="ICollectionView.Filter"/> is set while searching.</para>
    /// <para>The selected item is never filtered out, so binding a selection two-way is safe while
    /// the user types.</para>
    /// </remarks>
    /// <example>
    /// <code lang="xaml">
    /// &lt;ui:SearchableComboBox ItemsSource="{Binding Departments}"
    ///                       SelectedItem="{Binding SelectedDepartment}"
    ///                       DisplayMemberPath="Name"
    ///                       ui:ControlHelper.PlaceholderText="Pick a department"
    ///                       SearchBoxPlaceholderText="Type to search..." /&gt;
    /// </code>
    /// </example>
    [TemplatePart(Name = SearchBoxTemplateName, Type = typeof(TextBox))]
    [TemplatePart(Name = ScrollViewerTemplateName, Type = typeof(ScrollViewer))]
    [StyleTypedProperty(Property = nameof(SearchBoxStyle), StyleTargetType = typeof(TextBox))]
    public partial class SearchableComboBox : ComboBox
    {
        private const string SearchBoxTemplateName = "PART_SearchBox";
        private const string ScrollViewerTemplateName = "PART_ScrollViewer";

        /// <summary>Cache of resolved property chains, keyed by declaring type and path.</summary>
        private static readonly Dictionary<string, PropertyInfo[]> PropertyPathCache = new Dictionary<string, PropertyInfo[]>();

        private TextBox _searchBox;
        private ScrollViewer _scrollViewer;

        /// <summary>
        /// The raw value assigned to <see cref="ItemsControl.ItemsSource"/>, kept so that the
        /// wrapper view is rebuilt only when the source really changes.
        /// </summary>
        private IEnumerable _rawItemsSource;

        /// <summary>The private view the control filters, or the caller's own view.</summary>
        private ICollectionView _itemsView;

        /// <summary>Search text after case folding, computed once per pass.</summary>
        private string _normalizedSearchText = string.Empty;

        static SearchableComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchableComboBox),
                new FrameworkPropertyMetadata(typeof(SearchableComboBox)));

            // Wrapping the source in a view of our own is what keeps the filter from leaking into
            // every other control bound to the same collection.
            ItemsSourceProperty.OverrideMetadata(typeof(SearchableComboBox),
                new FrameworkPropertyMetadata(null, null, CoerceItemsSource));

            // Type-ahead selection would fight the search box over the keystrokes.
            IsTextSearchEnabledProperty.OverrideMetadata(typeof(SearchableComboBox),
                new FrameworkPropertyMetadata(false));

            // Left at its default of null, a Selector synchronises the selection with the view's
            // current item whenever ItemsSource is an ICollectionView - which, after the coercion
            // above, is always. That would select the first item on its own and move the selection
            // around as the user filters. Callers who want the synchronisation can still ask for it.
            IsSynchronizedWithCurrentItemProperty.OverrideMetadata(typeof(SearchableComboBox),
                new FrameworkPropertyMetadata(false));
        }

        #region Items source isolation

        private static object CoerceItemsSource(DependencyObject d, object baseValue)
        {
            var owner = (SearchableComboBox)d;
            var source = baseValue as IEnumerable;

            if (source == null)
            {
                owner._rawItemsSource = null;
                owner._itemsView = null;
                return null;
            }

            if (ReferenceEquals(owner._rawItemsSource, source))
            {
                return owner._itemsView ?? source;
            }

            owner._rawItemsSource = source;

            // A view supplied by the caller is honoured as-is: they already decided how the items
            // are sorted, grouped and shared.
            if (source is ICollectionView callerView)
            {
                owner._itemsView = callerView;
                return callerView;
            }

            var view = new CollectionViewSource { Source = source }.View;
            owner._itemsView = view;
            return (IEnumerable)view ?? source;
        }

        #endregion

        public override void OnApplyTemplate()
        {
            if (_searchBox != null)
            {
                _searchBox.PreviewKeyDown -= OnSearchBoxPreviewKeyDown;
                BindingOperations.ClearBinding(_searchBox, TextBox.TextProperty);
            }

            base.OnApplyTemplate();

            _searchBox = GetTemplateChild(SearchBoxTemplateName) as TextBox;
            _scrollViewer = GetTemplateChild(ScrollViewerTemplateName) as ScrollViewer;

            if (_searchBox != null)
            {
                // Enter and Down have to be caught before the TextBox and the base ComboBox see them.
                _searchBox.PreviewKeyDown += OnSearchBoxPreviewKeyDown;

                BindingOperations.SetBinding(_searchBox, TextBox.TextProperty, new Binding(nameof(SearchText))
                {
                    Source = this,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

                if (string.IsNullOrEmpty(AutomationProperties.GetName(_searchBox)))
                {
                    AutomationProperties.SetName(_searchBox,
                        GetString(SR_SearchableComboBoxSearchBoxName, "Search box"));
                }
            }
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);

            if (!IsSearchEnabled)
            {
                return;
            }

            // The popup is only realised after this call returns, so focusing has to wait a beat.
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                if (!IsDropDownOpen || _searchBox == null)
                {
                    return;
                }

                _searchBox.Focus();
                _searchBox.CaretIndex = _searchBox.Text?.Length ?? 0;
            }));
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);

            if (ClearSearchTextOnClose)
            {
                SearchText = string.Empty;
            }
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);

            // Typing while the drop-down is closed opens it and starts the search, the way
            // type-ahead does on a plain ComboBox.
            if (e.Handled || !IsSearchEnabled || IsDropDownOpen || IsEditable)
            {
                return;
            }

            var text = e.Text;

            if (string.IsNullOrEmpty(text) || char.IsControl(text[0]))
            {
                return;
            }

            SearchText = text;
            IsDropDownOpen = true;
            e.Handled = true;
        }

        private void OnSearchBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Up and Down are deliberately left alone: they bubble out of the search box and the
            // base ComboBox moves the highlight through the filtered list. Escape is likewise the
            // base class's job.
            if (e.Key == Key.Enter)
            {
                CommitFirstMatch();
                IsDropDownOpen = false;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Makes sure Enter picks something sensible: whatever the user highlighted with the arrow
        /// keys, or else the first item that matches what they typed.
        /// </summary>
        private void CommitFirstMatch()
        {
            if (!IsSearchEnabled || string.IsNullOrEmpty(SearchText))
            {
                return;
            }

            var selected = SelectedItem;

            if (selected != null && Matches(selected))
            {
                return;
            }

            foreach (var item in Items)
            {
                if (Matches(item))
                {
                    SelectedItem = item;
                    return;
                }
            }
        }

        #region Filtering

        private void UpdateFilter()
        {
            var searchText = IsSearchEnabled ? SearchText : null;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                _normalizedSearchText = string.Empty;
                ClearFilter();
                HasNoResults = false;
                return;
            }

            _normalizedSearchText = Normalize(searchText);

            if (!Items.CanFilter)
            {
                return;
            }

            // A fresh delegate every time: assigning Filter is what makes the view re-evaluate,
            // while Refresh() on its own leaves the previous result in place.
            Items.Filter = new Predicate<object>(PassesFilter);

            // The selected item is kept in the view even when it does not match, so it cannot be
            // counted as a result.
            HasNoResults = !Items.Cast<object>().Any(Matches);

            _scrollViewer?.ScrollToTop();
        }

        private void ClearFilter()
        {
            if (Items.CanFilter && Items.Filter != null)
            {
                Items.Filter = null;
            }
        }

        /// <summary>
        /// The predicate handed to the view. It keeps the selected item alive so that the base
        /// selector does not drop the selection while the user narrows the list.
        /// </summary>
        private bool PassesFilter(object item)
        {
            return Equals(item, SelectedItem) || Matches(item);
        }

        private bool Matches(object item)
        {
            var itemFilter = ItemFilter;

            if (itemFilter != null)
            {
                return itemFilter(item, SearchText);
            }

            if (_normalizedSearchText.Length == 0)
            {
                return true;
            }

            var text = Normalize(GetItemText(item));

            return text.Length != 0
                && text.IndexOf(_normalizedSearchText, StringComparison.Ordinal) >= 0;
        }

        #endregion

        #region Item text

        /// <summary>Text of an item, using the first path that is actually configured.</summary>
        private string GetItemText(object item)
        {
            if (item == null)
            {
                return string.Empty;
            }

            var path = SearchMemberPath;

            if (string.IsNullOrEmpty(path))
            {
                path = DisplayMemberPath;
            }

            if (string.IsNullOrEmpty(path))
            {
                path = TextSearch.GetTextPath(this);
            }

            if (string.IsNullOrEmpty(path))
            {
                return item.ToString() ?? string.Empty;
            }

            return GetPropertyValue(item, path)?.ToString() ?? string.Empty;
        }

        private static object GetPropertyValue(object item, string path)
        {
            var type = item.GetType();
            var key = type.AssemblyQualifiedName + " " + path;
            PropertyInfo[] chain;

            lock (PropertyPathCache)
            {
                if (!PropertyPathCache.TryGetValue(key, out chain))
                {
                    chain = ResolvePropertyPath(type, path);
                    PropertyPathCache[key] = chain;
                }
            }

            if (chain == null)
            {
                return null;
            }

            object current = item;

            foreach (var property in chain)
            {
                if (current == null)
                {
                    return null;
                }

                current = property.GetValue(current, null);
            }

            return current;
        }

        private static PropertyInfo[] ResolvePropertyPath(Type type, string path)
        {
            var names = path.Split('.');
            var chain = new PropertyInfo[names.Length];
            var current = type;

            for (int i = 0; i < names.Length; i++)
            {
                var property = current.GetProperty(names[i]);

                if (property == null)
                {
                    return null;
                }

                chain[i] = property;
                current = property.PropertyType;
            }

            return chain;
        }

        #endregion

        #region Text normalization

        /// <summary>Case folding, done once per side so matching can use an ordinal compare.</summary>
        private static string Normalize(string text)
            => string.IsNullOrEmpty(text) ? string.Empty : text.ToLower(CultureInfo.CurrentCulture);

        #endregion
    }
}
