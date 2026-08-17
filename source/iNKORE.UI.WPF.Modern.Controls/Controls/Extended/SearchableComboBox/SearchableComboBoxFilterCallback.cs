namespace iNKORE.UI.WPF.Modern.Controls
{
    /// <summary>
    /// Represents the method that decides whether an item of a <see cref="SearchableComboBox"/>
    /// matches the current search text.
    /// </summary>
    /// <param name="item">The item to test. Never <see langword="null"/> for a non-null source item.</param>
    /// <param name="searchText">The raw text currently typed in the search box.</param>
    /// <returns><see langword="true"/> to keep the item in the drop-down; otherwise <see langword="false"/>.</returns>
    public delegate bool SearchableComboBoxFilterCallback(object item, string searchText);
}
