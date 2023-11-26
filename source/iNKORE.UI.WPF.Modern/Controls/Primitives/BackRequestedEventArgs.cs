
/* 项目“iNKORE.UI.WPF.Modern (net452)”的未合并的更改
在此之前:
using System.Windows;
在此之后:
using iNKORE.UI.WPF.Modern.Controls.TitleBar;
using System.Windows;
*/
using System.Windows;

namespace iNKORE.UI.WPF.Modern.Controls.Primitives
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
