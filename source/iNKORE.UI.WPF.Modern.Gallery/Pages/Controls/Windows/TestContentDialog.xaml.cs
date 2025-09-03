using iNKORE.UI.WPF.Modern.Controls;
using System;
using System.Windows;

namespace iNKORE.UI.WPF.Modern.Gallery.Pages.Controls.Windows
{
    public partial class TestContentDialog
    {
        public TestContentDialog()
        {
            InitializeComponent();
        }

        private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void OnSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            deferral.Complete();
        }

        private async void TryOpenAnother(object sender, RoutedEventArgs e)
        {
            try
            {
                await new TestContentDialog { Owner = Owner }.ShowAsync();
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
                ErrorText.Visibility = Visibility.Visible;
            }
        }

        private void OnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            ErrorText.Text = string.Empty;
            ErrorText.Visibility = Visibility.Collapsed;
        }

        public static string CodeXaml => $@"
<ui:ContentDialog
    x:Class=""iNKORE.UI.WPF.Modern.Gallery.Pages.Controls.Windows.TestContentDialog""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:ikw=""http://schemas.inkore.net/lib/ui/wpf""
    xmlns:sc=""clr-namespace:SamplesCommon;assembly=SamplesCommon""
    xmlns:ui=""http://schemas.inkore.net/lib/ui/wpf/modern""
    x:Name=""dialog""
    Title=""Title""
    d:DesignHeight=""756""
    d:DesignWidth=""548""
    CloseButtonClick=""OnCloseButtonClick""
    CloseButtonText=""Cancel""
    Closed=""OnClosed""
    DefaultButton=""Primary""
    PrimaryButtonClick=""OnPrimaryButtonClick""
    PrimaryButtonText=""Yes""
    SecondaryButtonClick=""OnSecondaryButtonClick""
    SecondaryButtonText=""No""
    Style=""{{StaticResource ScrollableContentDialogStyle}}""
    mc:Ignorable=""d"">
    <ikw:SimpleStackPanel Spacing=""12"">
        <TextBox
            ui:ControlHelper.Header=""Title""
            AcceptsReturn=""True""
            Text=""{{Binding ElementName=dialog, Path=Title, UpdateSourceTrigger=PropertyChanged}}"" />
        <TextBox ui:ControlHelper.Header=""PrimaryButtonText"" Text=""{{Binding ElementName=dialog, Path=PrimaryButtonText, UpdateSourceTrigger=PropertyChanged}}"" />
        <TextBox ui:ControlHelper.Header=""SecondaryButtonText"" Text=""{{Binding ElementName=dialog, Path=SecondaryButtonText, UpdateSourceTrigger=PropertyChanged}}"" />
        <TextBox ui:ControlHelper.Header=""CloseButtonText"" Text=""{{Binding ElementName=dialog, Path=CloseButtonText, UpdateSourceTrigger=PropertyChanged}}"" />
        <ComboBox HorizontalAlignment=""Stretch""
            ui:ControlHelper.Header=""DefaultButton""
            ItemsSource=""{{Binding Source={{x:Type ui:ContentDialogButton}}, Converter={{StaticResource EnumValuesConverter}}}}""
            SelectedItem=""{{Binding ElementName=dialog, Path=DefaultButton}}"" />
        <CheckBox Content=""FullSizeDesired"" IsChecked=""{{Binding ElementName=dialog, Path=FullSizeDesired}}"" />
        <CheckBox Content=""IsShadowEnabled"" IsChecked=""{{Binding ElementName=dialog, Path=IsShadowEnabled}}"" />
        <StackPanel>
            <Button Click=""TryOpenAnother"" Content=""Try to open another ContentDialog"" />
            <TextBlock
                x:Name=""ErrorText""
                Margin=""0,8,0,0""
                Foreground=""{{DynamicResource SystemControlErrorTextForegroundBrush}}""
                Style=""{{StaticResource BodyTextBlockStyle}}""
                Visibility=""Collapsed"" />
        </StackPanel>
    </ikw:SimpleStackPanel>
</ui:ContentDialog>
";

    }
}
