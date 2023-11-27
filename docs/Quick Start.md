
# Quick Start

1. Create a new WPF app.

2. Add reference to `Inkore.UI.WPF.Modern.dll` and `Inkore.UI.WPF.Modern.Controls.dll` or install the NuGet package

   ```
   dotnet add package Inkore.UI.WPF.Modern --version 0.9.15
   ```

3. Add XML Namespace in `App.xaml` and `MainWindow.xaml`

    ```xaml
    xmlns:ui="http://schemas.inkore.net/lib/ui/wpf/modern"
    ```

4. Add theme resources in the `Application.Resources` section of `App.xaml`

   Minimum Sample:

   ```xaml
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ui:ThemeResources/>
                <ui:XamlControlsResources />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
   ```

      Customizable Sample:

   ```xaml
       <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ui:ThemeResources CanBeAccessedAcrossThreads="False">

                    <ui:ThemeResources.ThemeDictionaries>
                        <ResourceDictionary x:Key="Light">
                            <SolidColorBrush x:Key="NavigationViewExpandedPaneBackground" Color="#FFEDEDED" />
                        </ResourceDictionary>
                        <ResourceDictionary x:Key="Dark">
                            <SolidColorBrush x:Key="NavigationViewExpandedPaneBackground" Color="#FF202020" />
                        </ResourceDictionary>
                    </ui:ThemeResources.ThemeDictionaries>

                </ui:ThemeResources>
                <ui:XamlControlsResources />

            </ResourceDictionary.MergedDictionaries>


            <Style x:Key="OptionsPanelStyle" TargetType="ui:SimpleStackPanel">
                <Setter Property="Spacing" Value="12" />
                <Style.Resources>
                    <Style BasedOn="{StaticResource {x:Type ComboBox}}" TargetType="ComboBox">
                        <Setter Property="MinWidth" Value="200" />
                    </Style>
                    <Style BasedOn="{StaticResource {x:Type TextBox}}" TargetType="TextBox">
                        <Setter Property="MinWidth" Value="200" />
                    </Style>
                    <Style BasedOn="{StaticResource {x:Type DatePicker}}" TargetType="DatePicker">
                        <Setter Property="MinWidth" Value="200" />
                    </Style>
                </Style.Resources>
            </Style>

            <Thickness x:Key="ControlPageContentMargin">24,0,24,20</Thickness>

            <Style x:Key="ControlPageContentPanelStyle" TargetType="ui:SimpleStackPanel">
                <Setter Property="Margin" Value="{StaticResource ControlPageContentMargin}" />
                <Setter Property="Spacing" Value="16" />
            </Style>

            <Style x:Key="ScrollableContentDialogStyle" TargetType="ui:ContentDialog">
                <Setter Property="ScrollViewer.VerticalScrollBarVisibility" Value="Auto" />
                <Style.Resources>
                    <Thickness x:Key="ContentDialogContentMargin">24,0,24,0</Thickness>
                    <Thickness x:Key="ContentDialogContentScrollViewerMargin">-24,0,-24,0</Thickness>
                    <Thickness x:Key="ContentDialogTitleMargin">24,0,24,12</Thickness>
                </Style.Resources>
            </Style>

            <Style
                x:Key="RichTextBlockStyle"
                BasedOn="{StaticResource DefaultRichTextBoxStyle}"
                TargetType="RichTextBox">
                <Setter Property="Padding" Value="0" />
                <Setter Property="IsReadOnly" Value="True" />
                <Setter Property="IsTabStop" Value="False" />
                <Setter Property="Template">
                    <Setter.Value>
                        <ControlTemplate TargetType="RichTextBox">
                            <ui:ScrollViewerEx x:Name="PART_ContentHost" />
                        </ControlTemplate>
                    </Setter.Value>
                </Setter>
                <Style.Resources>
                    <Style TargetType="Paragraph">
                        <Setter Property="Margin" Value="0" />
                    </Style>
                </Style.Resources>
            </Style>

            <CornerRadius x:Key="ControlCornerRadius">4</CornerRadius>

            <LinearGradientBrush x:Key="HeroImageGradientBrush" StartPoint="0.5,0" EndPoint="0.5,1.5">
                <GradientStop Offset="0" Color="Transparent" />
                <GradientStop Offset="0.5" Color="{DynamicResource LayerFillColorDefault}" />
                <GradientStop Offset="1" Color="{DynamicResource LayerFillColorDefault}" />
            </LinearGradientBrush>

            <!--  Breakpoints  -->
            <sys:Double x:Key="Breakpoint640Plus">641</sys:Double>

            <Thickness x:Key="PageHeaderDefaultPadding">0</Thickness>
            <Thickness x:Key="PageHeaderMinimalPadding">-4,0,12,0</Thickness>

            <Thickness x:Key="ControlElementScreenshotModePadding">67</Thickness>

            <!--  L-Pattern Overwriting resources  -->
            <Thickness x:Key="NavigationViewContentMargin">0,48,0,0</Thickness>
            <Thickness x:Key="NavigationViewContentGridBorderThickness">1,1,0,0</Thickness>
            <CornerRadius x:Key="NavigationViewContentGridCornerRadius">8,0,0,0</CornerRadius>
            <Thickness x:Key="NavigationViewHeaderMargin">56,34,0,0</Thickness>

            <SolidColorBrush x:Key="GridViewHeaderItemDividerStroke" Color="Transparent" />

            <sys:String x:Key="ControlsName">All controls</sys:String>
            <sys:String x:Key="AppTitleName">ModernWPF Gallery</sys:String>
            <sys:String x:Key="WinUIVersion">SDK 0.10.0</sys:String>

            <sys:String x:Key="NewControlsName">What's New</sys:String>

            <Style x:Key="ControlPageScrollStyle" TargetType="ScrollViewer">
                <Setter Property="VerticalScrollBarVisibility" Value="Auto" />
            </Style>

            <Style x:Key="GridViewItemStyle" TargetType="ui:GridViewItem">
                <Setter Property="Margin" Value="0,0,12,12" />
            </Style>

            <Style x:Key="IndentedGridViewItemStyle" TargetType="ui:GridViewItem">
                <Setter Property="Margin" Value="12,0,0,12" />
            </Style>

            <Style x:Key="GridViewItemStyleSmall" TargetType="ui:GridViewItem">
                <Setter Property="Margin" Value="0,0,0,12" />
                <Setter Property="HorizontalContentAlignment" Value="Stretch" />
            </Style>

            <Style x:Key="IndentedGridViewItemStyleSmall" TargetType="ui:GridViewItem">
                <Setter Property="Margin" Value="12,0,12,12" />
                <Setter Property="HorizontalContentAlignment" Value="Stretch" />
            </Style>

            <sys:Double x:Key="TeachingTipMinWidth">50</sys:Double>
        </ResourceDictionary>
    </Application.Resources>
   ```

5. Enable Modern style (and backdrop if you want) for MainWindow in `MainWindow.xaml`

   ```xaml
        ui:ThemeManager.IsThemeAware="True"
        ui:TitleBar.ExtendViewIntoTitleBar="True"    
        ui:WindowHelper.SystemBackdropType="Mica"
        ui:TitleBar.IsBackButtonVisible="False"
        ui:WindowHelper.UseModernWindowStyle="True"
   ```