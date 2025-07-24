using iNKORE.UI.WPF.Modern.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Page = iNKORE.UI.WPF.Modern.Controls.Page;

namespace iNKORE.UI.WPF.Modern.Gallery.Pages.Controls.Windows
{
    public partial class ExpanderPage : Page
    {
        public ExpanderPage()
        {
            InitializeComponent();
        }

        private void ExpandDirectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string expandDirection = e.AddedItems[0].ToString();

            switch (expandDirection)
            {
                case "Down":
                default:
                    Expander1.ExpandDirection = ExpandDirection.Down;
                    Expander1.VerticalAlignment = VerticalAlignment.Top;
                    break;

                case "Up":
                    Expander1.ExpandDirection = ExpandDirection.Up;
                    Expander1.VerticalAlignment = VerticalAlignment.Bottom;
                    break;

                case "Left":
                    Expander1.ExpandDirection = ExpandDirection.Left;
                    Expander1.HorizontalAlignment = HorizontalAlignment.Right;
                    break;

                case "Right":
                    Expander1.ExpandDirection = ExpandDirection.Right;
                    Expander1.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
            }

            if (this.IsLoaded)
            {
                UpdateExampleCode();
            }
        }

        private void ExpandDirectionComboBox5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string expandDirection = e.AddedItems[0].ToString();

            switch (expandDirection)
            {
                case "Down":
                default:
                    Expander5.ExpandDirection = ExpandDirection.Down;
                    Expander5.VerticalAlignment = VerticalAlignment.Top;
                    break;

                case "Up":
                    Expander5.ExpandDirection = ExpandDirection.Up;
                    Expander5.VerticalAlignment = VerticalAlignment.Bottom;
                    break;

                case "Left":
                    Expander5.ExpandDirection = ExpandDirection.Left;
                    Expander5.HorizontalAlignment = HorizontalAlignment.Right;
                    break;

                case "Right":
                    Expander5.ExpandDirection = ExpandDirection.Right;
                    Expander5.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
            }

            if (this.IsLoaded)
            {
                UpdateExampleCode();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Setup bindings for Example 1
            ControlExampleSubstitution Substitution1 = new ControlExampleSubstitution
            {
                Key = "IsExpanded",
            };
            BindingOperations.SetBinding(Substitution1, ControlExampleSubstitution.ValueProperty, new Binding
            {
                Source = Expander1,
                Path = new PropertyPath("IsExpanded"),
            });

            ControlExampleSubstitution Substitution2 = new ControlExampleSubstitution
            {
                Key = "ExpandDirection",
            };
            BindingOperations.SetBinding(Substitution2, ControlExampleSubstitution.ValueProperty, new Binding
            {
                Source = ExpandDirectionComboBox,
                Path = new PropertyPath("SelectedValue"),
            });

            ControlExampleSubstitution Substitution3 = new ControlExampleSubstitution
            {
                Key = "VerticalAlignment",
            };
            BindingOperations.SetBinding(Substitution3, ControlExampleSubstitution.ValueProperty, new Binding
            {
                Source = Expander1,
                Path = new PropertyPath("VerticalAlignment"),
            });

            ControlExampleSubstitution Substitution4 = new ControlExampleSubstitution
            {
                Key = "HorizontalAlignment",
            };
            BindingOperations.SetBinding(Substitution4, ControlExampleSubstitution.ValueProperty, new Binding
            {
                Source = Expander1,
                Path = new PropertyPath("HorizontalAlignment"),
            });

            ObservableCollection<ControlExampleSubstitution> Substitutions = new ObservableCollection<ControlExampleSubstitution> { Substitution1, Substitution2, Substitution3, Substitution4 };
            Example1.Substitutions = Substitutions;

            // Setup bindings for Example 5
            ControlExampleSubstitution Substitution5 = new ControlExampleSubstitution
            {
                Key = "IsExpanded",
            };
            BindingOperations.SetBinding(Substitution5, ControlExampleSubstitution.ValueProperty, new Binding
            {
                Source = Expander5,
                Path = new PropertyPath("IsExpanded"),
            });

            ControlExampleSubstitution Substitution6 = new ControlExampleSubstitution
            {
                Key = "ExpandDirection",
            };
            BindingOperations.SetBinding(Substitution6, ControlExampleSubstitution.ValueProperty, new Binding
            {
                Source = ExpandDirectionComboBox5,
                Path = new PropertyPath("SelectedValue"),
            });

            ControlExampleSubstitution Substitution7 = new ControlExampleSubstitution
            {
                Key = "VerticalAlignment",
            };
            BindingOperations.SetBinding(Substitution7, ControlExampleSubstitution.ValueProperty, new Binding
            {
                Source = Expander5,
                Path = new PropertyPath("VerticalAlignment"),
            });

            ControlExampleSubstitution Substitution8 = new ControlExampleSubstitution
            {
                Key = "HorizontalAlignment",
            };
            BindingOperations.SetBinding(Substitution8, ControlExampleSubstitution.ValueProperty, new Binding
            {
                Source = Expander5,
                Path = new PropertyPath("HorizontalAlignment"),
            });

            ObservableCollection<ControlExampleSubstitution> Substitutions5 = new ObservableCollection<ControlExampleSubstitution> { Substitution5, Substitution6, Substitution7, Substitution8 };
            Example5.Substitutions = Substitutions5;

            UpdateExampleCode();
        }

        #region Example Code

        public void UpdateExampleCode()
        {
            Example1.Xaml = Example1Xaml;
            Example2.Xaml = Example2Xaml;
            Example3.Xaml = Example3Xaml;
            // Example4.Xaml = Example4Xaml;
            Example5.Xaml = Example5Xaml;
            Example6.Xaml = Example6Xaml;
            Example7.Xaml = Example7Xaml;
        }

        public string Example1Xaml => $@"
<Expander x:Name=""Expander1""
    Style=""{{StaticResource {{x:Static ui:ThemeKeys.ExpanderCardStyleKey}}}}""
    Content=""This is in the content""
    ExpandDirection=""{Expander1.ExpandDirection.ToString()}""
    Header=""This text is in the header"" />
";

        public string Example2Xaml => $@"
<Expander x:Name=""Expander2"" Style=""{{StaticResource {{x:Static ui:ThemeKeys.ExpanderCardStyleKey}}}}"">
    <Expander.Header>
        <ToggleButton Content=""This is a ToggleButton in the header"" />
    </Expander.Header>
    <Expander.Content>
        <Grid>
            <Button Margin=""15"" Content=""This is a Button in the content"" />
        </Grid>
    </Expander.Content>
</Expander>
";

            public string Example3Xaml => $@"
<Expander Style=""{{StaticResource {{x:Static ui:ThemeKeys.ExpanderCardStyleKey}}}}"">
    <Expander.Header>
        <ToggleButton HorizontalAlignment=""Center"" Content=""This ToggleButton is centered"" />
    </Expander.Header>
    <Expander.Content>
        <Button Margin=""4"" Content=""This Button is left aligned"" />
    </Expander.Content>
</Expander>
";

//         public string Example4Xaml => $@"
// <Expander Width=""500""
//     Content=""This is in the content""
//     ExpandDirection=""Down""
//     Header=""This text is in the header""
//     HorizontalContentAlignment=""Left"" />
// ";

        public string Example5Xaml => $@"
<Expander x:Name=""Expander5""
    VerticalAlignment=""Top""
    Content=""This is in the content""
    ExpandDirection=""{Expander5.ExpandDirection.ToString()}""
    Header=""This text is in the header""
    IsExpanded=""False"" />
";

        public string Example6Xaml => $@"
<Expander x:Name=""Expander6"">
    <Expander.Header>
        <ToggleButton Content=""This is a ToggleButton in the header"" />
    </Expander.Header>
    <Expander.Content>
        <Grid>
            <Button Margin=""15"" Content=""This is a Button in the content"" />
        </Grid>
    </Expander.Content>
</Expander>
";

        public string Example7Xaml => $@"
<Expander Width=""500""
    Padding=""0""
    HorizontalContentAlignment=""Left"">
    <Expander.Header>
        <ToggleButton HorizontalAlignment=""Center"" Content=""This ToggleButton is centered"" />
    </Expander.Header>
    <Expander.Content>
        <Button Margin=""4"" Content=""This Button is left aligned"" />
    </Expander.Content>
</Expander>
";

        #endregion

    }
}
