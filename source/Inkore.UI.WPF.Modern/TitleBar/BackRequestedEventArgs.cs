using System.Windows;

namespace Inkore.UI.WPF.Modern.Controls
{
    /// <summary>
    /// Provides event data for the BackRequested event.
    /// </summary>
    public sealed class BackRequestedEventArgs : RoutedEventArgs
    {
        internal BackRequestedEventArgs() : base(TitleBar.BackRequestedEvent)
        {
        }

        internal BackRequestedEventArgs(object source) : base(TitleBar.BackRequestedEvent, source)
        {
        }
    }
}
