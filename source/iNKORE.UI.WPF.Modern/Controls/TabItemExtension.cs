using System.Windows.Controls;
using iNKORE.UI.WPF.Modern.Common;
using iNKORE.UI.WPF.Modern.Controls.Helpers;

namespace iNKORE.UI.WPF.Modern.Controls;

public static class TabItemExtension
{
    public static void RegisterTabClosingEvent(this TabControl tabControl, TypedEventHandler<TabControl, TabViewTabCloseRequestedEventArgs> handler)
    {
        if (tabControl.GetValue(TabControlHelper.TabControlHelperEventsProperty) is not TabControlHelperEvents eventHelper)
        {
            return;
        }

        eventHelper.TabCloseRequested += handler;
    }

    public static void UnregisterTabClosingEvent(this TabControl tabControl, TypedEventHandler<TabControl, TabViewTabCloseRequestedEventArgs> handler)
    {
        if (tabControl.GetValue(TabControlHelper.TabControlHelperEventsProperty) is not TabControlHelperEvents eventHelper)
        {
            return;
        }

        eventHelper.TabCloseRequested -= handler;
    }
}
