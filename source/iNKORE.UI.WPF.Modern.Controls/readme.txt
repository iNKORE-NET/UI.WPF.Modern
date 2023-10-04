Thanks for installing the iNKORE.UI.WPF.Modern UI NuGet package!

Don't forget to add the theme resources to your Application resources in App.xaml:

    <Application
        ...
        xmlns:ui="http://schemas.inkore.net/lib/ui/wpf/modern">
        <Application.Resources>
            <ResourceDictionary>
                <ResourceDictionary.MergedDictionaries>
                    <ui:ThemeResources />
                    <ui:XamlControlsResources />
                    <!-- Other merged dictionaries here -->
                </ResourceDictionary.MergedDictionaries>
                <!-- Other app resources here -->
            </ResourceDictionary>
        </Application.Resources>
    </Application>

To enable themed style for a window, set WindowHelper.UseModernWindowStyle to true:

    <Window
        ...
        xmlns:ui="http://schemas.inkore.net/lib/ui/wpf/modern"
        ui:WindowHelper.UseModernWindowStyle="True">
        <!-- Window content here -->
    </Window>

See https://github.com/Kinnara/iNKORE.UI.WPF.Modern for more information.
