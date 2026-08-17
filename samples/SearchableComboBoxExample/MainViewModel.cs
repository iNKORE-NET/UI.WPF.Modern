using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using iNKORE.UI.WPF.Modern.Controls;

namespace SearchableComboBoxExample
{
    /// <summary>
    /// Everything the MVVM drop-down needs, exposed as bindable properties so the window needs no
    /// code-behind for it.
    /// </summary>
    public class CountryPicker : INotifyPropertyChanged
    {
        public ObservableCollection<Country> Items { get; } = new()
        {
            new Country("Brazil", "+55"),
            new Country("Canada", "+1"),
            new Country("Germany", "+49"),
            new Country("Iceland", "+354"),
            new Country("Japan", "+81"),
            new Country("Norway", "+47"),
            new Country("Poland", "+48"),
            new Country("Türkiye", "+90"),
            new Country("Vietnam", "+84"),
        };

        /// <summary>
        /// Bound to <see cref="SearchableComboBox.ItemFilter"/>. Without it the control would match
        /// the displayed name only; this way the dialling code is searchable too.
        /// </summary>
        public SearchableComboBoxFilterCallback Filter { get; } = (item, searchText) =>
        {
            var country = (Country)item;

            return country.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                || country.DiallingCode.Contains(searchText, StringComparison.OrdinalIgnoreCase);
        };

        private Country? _selected;

        public Country? Selected
        {
            get => _selected;
            set
            {
                if (!Equals(_selected, value))
                {
                    _selected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class MainViewModel
    {
        public CountryPicker Countries { get; } = new();
    }

    public record Country(string Name, string DiallingCode)
    {
        public override string ToString() => $"{Name} ({DiallingCode})";
    }

    public record Product(string Code, string Name)
    {
        public static List<Product> CreateMany(int count)
        {
            var products = new List<Product>(count);

            for (int i = 0; i < count; i++)
            {
                products.Add(new Product($"A-{1000 + i:0000}", $"Product part {i + 1}"));
            }

            return products;
        }

        public override string ToString() => $"{Name} ({Code})";
    }
}
