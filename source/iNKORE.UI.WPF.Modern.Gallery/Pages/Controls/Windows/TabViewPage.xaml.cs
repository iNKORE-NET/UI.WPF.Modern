using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using iNKORE.UI.WPF.Modern.Common.IconKeys;
using iNKORE.UI.WPF.Modern.Controls;
using iNKORE.UI.WPF.Modern.Controls.Helpers;
using iNKORE.UI.WPF.Modern.Controls.Primitives;
using SamplesCommon.SamplePages;
using Frame = iNKORE.UI.WPF.Modern.Controls.Frame;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;

namespace iNKORE.UI.WPF.Modern.Gallery.Pages.Controls.Windows
{
    public partial class TabViewPage
    {
        public TabViewPage()
        {
            InitializeComponent();

            for (int i = 0; i < 3; i++)
            {
                tabControl.Items.Add(CreateNewTab(i));
                tabControl2.Items.Add(CreateNewTab(i));
                tabControl3.Items.Add(CreateNewTab(i));
            }

            InitializeExample6();
            UpdateExampleCode();
        }

        private void TabView_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                (sender as TabControl).Items.Add(CreateNewTab(i));
            }
        }

        private TabItem CreateNewTab(int index)
        {
            TabItem newItem = new TabItem();

            newItem.Header = $"Document {index}";
            TabItemHelper.SetIcon(newItem, new FontIcon(SegoeFluentIcons.Document));

            // The content of the tab is often a frame that contains a page, though it could be any UIElement.
            Frame frame = new Frame();

            frame.Navigated += (s, e) =>
            {
                ((FrameworkElement)frame.Content).Margin = new Thickness(-18, 0, -18, 0);
            };

            switch (index % 3)
            {
                case 0:
                    frame.Navigate(typeof(SamplePage1));
                    break;
                case 1:
                    frame.Navigate(typeof(SamplePage2));
                    break;
                case 2:
                    frame.Navigate(typeof(SamplePage3));
                    break;
            }

            newItem.Content = frame;

            return newItem;
        }

        private void ShowHeaderAndFooterCheckBox_Click(object sender, RoutedEventArgs e)
        {
            UpdateExampleCode();
        }

        private void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateExampleCode();
        }

        #region Example 6

        private const string NiceTry = "Nice try!";

        private void InitializeExample6()
        {
            ResetExample6Tabs();

            tabControl.RegisterTabClosingEvent((menuControl, e) =>
            {
                if (e.Tab.Tag is "ConfirmClose")
                {
                    var msgResult = MessageBox.Show("Do you want to close this tab?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (msgResult != MessageBoxResult.OK)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (e.Tab.Tag is "NiceTry")
                {
                    e.Cancel = true;

                    if ((e.Tab.Header as string) != NiceTry)
                        e.Tab.Header = NiceTry;
                    else e.Tab.Header = "You can't close me!";

                    return;
                }
            });
        }

        private void ResetExample6Tabs()
        {
            tabControl6.Items.Clear();
            tabControl6.Items.Add(TabItem_Example6_Tab1);
            tabControl6.Items.Add(TabItem_Example6_Tab2);
            tabControl6.Items.Add(TabItem_Example6_Tab3);

            TabItem_Example6_Tab3.Header = "DON'T TOUCH ME!";
        }

        private void Button_Example6_ResetTabs_Click(object sender, RoutedEventArgs e)
        {
            ResetExample6Tabs();
        }


        #endregion


        #region Example Code

        public void UpdateExampleCode()
        {
            if (!this.IsInitialized) return;

            Example1.Xaml = Example1Xaml;
            Example2.Xaml = Example2Xaml;
            Example3.Xaml = Example3Xaml;
            Example4.Xaml = Example4Xaml;
            Example5.Xaml = Example5Xaml;
            Example6.Xaml = Example6Xaml;
            Example6.CSharp = Example6CS;
        }

        public string Example1Xaml => $@"
<TabControl x:Name=""tabControl""
    TabStripPlacement=""{tabControl.TabStripPlacement}"">
    <ui:TabControlHelper.TabStripHeader>
        <Button
            HorizontalAlignment=""Stretch""
            VerticalAlignment=""Stretch""
            ui:FocusVisualHelper.FocusVisualMargin=""0""
            Content=""Header""
            Visibility=""{(ShowHeaderAndFooterCheckBox.IsChecked == true ? Visibility.Visible : Visibility.Collapsed)}"" />
    </ui:TabControlHelper.TabStripHeader>
    <ui:TabControlHelper.TabStripFooter>
        <Button
            HorizontalAlignment=""Stretch""
            VerticalAlignment=""Stretch""
            ui:FocusVisualHelper.FocusVisualMargin=""0""
            Content=""Footer""
            Visibility=""{(ShowHeaderAndFooterCheckBox.IsChecked == true ? Visibility.Visible : Visibility.Collapsed)}"" />
    </ui:TabControlHelper.TabStripFooter>
</TabControl>
";

        public string Example2Xaml => $@"
<TabControl x:Name=""TabView4""
    SelectedIndex=""0"">
    <TabControl.Items>
        <TabItem Header=""CMD Prompt"">
            <ui:TabItemHelper.Icon>
                <ui:BitmapIcon ShowAsMonochrome=""False"" UriSource=""/Assets/TabViewIcons/cmd.png"" />
            </ui:TabItemHelper.Icon>
        </TabItem>
        <TabItem Header=""Powershell"">
            <ui:TabItemHelper.Icon>
                <ui:BitmapIcon ShowAsMonochrome=""False"" UriSource=""/Assets/TabViewIcons/powershell.png"" />
            </ui:TabItemHelper.Icon>
        </TabItem>
        <TabItem Header=""Windows Subsystem for Linux"">
            <ui:TabItemHelper.Icon>
                <ui:BitmapIcon ShowAsMonochrome=""False"" UriSource=""/Assets/TabViewIcons/linux.png"" />
            </ui:TabItemHelper.Icon>
        </TabItem>
    </TabControl.Items>
</TabControl>
";

        public string Example3Xaml => $@"
<TabControl SelectedIndex=""0"">
    <ui:TabControlHelper.TabStripHeader>
        <TextBlock
            Margin=""8,6""
            VerticalAlignment=""Center""
            Style=""{{DynamicResource BaseTextBlockStyle}}""
            Text=""TabStripHeader Content"" />
    </ui:TabControlHelper.TabStripHeader>
    <ui:TabControlHelper.TabStripFooter>
        <TextBlock
            Margin=""6""
            HorizontalAlignment=""Right""
            VerticalAlignment=""Center""
            Style=""{{DynamicResource BaseTextBlockStyle}}""
            Text=""TabStripFooter Content"" />
    </ui:TabControlHelper.TabStripFooter>
</TabControl>
";

        public string Example4Xaml => $@"
<TabControl x:Name=""tabControl3"" ui:ThemeManager.HasThemeResources=""True"">
    <TabControl.Resources>
        <ui:ResourceDictionaryEx>
            <ui:ResourceDictionaryEx.ThemeDictionaries>
                <ResourceDictionary x:Key=""Light"">
                    <SolidColorBrush x:Key=""TabViewBackground"" Color=""{{DynamicResource SystemAccentColorLight2}}"" />
                </ResourceDictionary>
                <ResourceDictionary x:Key=""Dark"">
                    <SolidColorBrush x:Key=""TabViewBackground"" Color=""{{DynamicResource SystemAccentColorDark2}}"" />
                </ResourceDictionary>
            </ui:ResourceDictionaryEx.ThemeDictionaries>
        </ui:ResourceDictionaryEx>
    </TabControl.Resources>
</TabControl>
";

        public string Example5Xaml => $@"
<TabControl x:Name=""tabControl2"">
    <TabControl.Resources>
        <sys:Double x:Key=""TabViewItemHeaderFontSize"">24</sys:Double>
        <sys:Double x:Key=""TabViewItemHeaderIconSize"">32</sys:Double>
    </TabControl.Resources>
    <TabControl.ItemContainerStyle>
        <Style BasedOn=""{{StaticResource DefaultTabItemStyle}}"" TargetType=""TabItem"">
            <Setter Property=""FontFamily"" Value=""Courier New"" />
        </Style>
    </TabControl.ItemContainerStyle>
</TabControl>
";

        public string Example6Xaml => $@"
<TabControl x:Name=""tabControl6"">
    <TabItem x:Name=""TabItem_Example6_Tab1"" Header=""Closable""/>
    <TabItem x:Name=""TabItem_Example6_Tab2"" Header=""Confirm To Close"" Tag=""ConfirmClose""/>
    <TabItem x:Name=""TabItem_Example6_Tab3"" Header=""DON'T TOUCH ME!"" Tag=""NiceTry""/>
</TabControl>
";

        public string Example6CS => $@"
// Invoked during the constructor after InitializeComponent or at any later time
private void InitializeExample6()
{{
    ResetExample6Tabs();

    tabControl.RegisterTabClosingEvent((menuControl, e) =>
    {{
        if (e.Tab.Tag is ""ConfirmClose"")
        {{
            var msgResult = MessageBox.Show(""Do you want to close this tab?"", ""Confirm"", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (msgResult != MessageBoxResult.OK)
            {{
                e.Cancel = true;
                return;
            }}
        }}
        else if (e.Tab.Tag is ""NiceTry"")
        {{
            e.Cancel = true;

            if ((e.Tab.Header as string) != NiceTry)
                e.Tab.Header = NiceTry;
            else e.Tab.Header = ""You can't close me!"";

            return;
        }}
    }});
}}
";


        #endregion
    }
}
