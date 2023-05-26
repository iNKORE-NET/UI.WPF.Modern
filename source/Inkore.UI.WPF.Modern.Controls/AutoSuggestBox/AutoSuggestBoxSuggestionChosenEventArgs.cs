using System;
using System.Windows;

namespace Inkore.UI.WPF.Modern.Controls
{
    public sealed class AutoSuggestBoxSuggestionChosenEventArgs : EventArgs
    {
        public AutoSuggestBoxSuggestionChosenEventArgs()
        {
        }

        public object SelectedItem { get; internal set; }
    }
}
