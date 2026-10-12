using System.Collections.Generic;
using Page = iNKORE.UI.WPF.Modern.Controls.Page;

namespace iNKORE.UI.WPF.Modern.Gallery.Pages.Controls.Windows
{
    public partial class SearchableComboBoxPage : Page
    {
        public SearchableComboBoxPage()
        {
            InitializeComponent();
            UpdateExampleCode();
        }

        public List<EmployeeInfo> Employees { get; } = new List<EmployeeInfo>
        {
            new EmployeeInfo("Ada Lovelace", "Research"),
            new EmployeeInfo("Alan Turing", "Research"),
            new EmployeeInfo("Barbara Liskov", "Engineering"),
            new EmployeeInfo("Edsger Dijkstra", "Engineering"),
            new EmployeeInfo("Grace Hopper", "Engineering"),
            new EmployeeInfo("Katherine Johnson", "Operations"),
            new EmployeeInfo("Margaret Hamilton", "Operations"),
            new EmployeeInfo("Radia Perlman", "Networking"),
            new EmployeeInfo("Tim Berners-Lee", "Networking")
        };

        #region Example Code

        public void UpdateExampleCode()
        {
            Example1.Xaml = Example1Xaml;
            Example2.Xaml = Example2Xaml;
            Example2.CSharp = Example2CS;
        }

        public string Example1Xaml => $@"
<ui:SearchableComboBox x:Name=""Combo1""
    ui:ControlHelper.Header=""Colors""
    ui:ControlHelper.PlaceholderText=""Pick a color""
    SearchBoxPlaceholderText=""Type to filter colors"">
    <sys:String>Blue</sys:String>
    <sys:String>Green</sys:String>
    <sys:String>Orange</sys:String>
    <sys:String>Red</sys:String>
    <sys:String>Violet</sys:String>
    <sys:String>Yellow</sys:String>
</ui:SearchableComboBox>
";

        public string Example2Xaml => $@"
<ui:SearchableComboBox x:Name=""Combo2""
    ui:ControlHelper.Header=""Employee""
    ui:ControlHelper.PlaceholderText=""Pick an employee""
    DisplayMemberPath=""Summary""
    ItemsSource=""{{Binding Employees}}""
    SearchBoxPlaceholderText=""Type to search...""
    NoResultsText=""Nobody matches that""
    MaxDropDownHeight=""240""
    ClearSearchTextOnClose=""True""
    IsSearchEnabled=""True"" />
";

        public string Example2CS => $@"
public class EmployeeInfo
{{
    public EmployeeInfo(string displayName, string department)
    {{
        DisplayName = displayName;
        Department = department;
    }}

    public string DisplayName {{ get; }}
    public string Department {{ get; }}
    public string Summary => $""{{DisplayName}} — {{Department}}"";
}}

public List<EmployeeInfo> Employees {{ get; }} = new List<EmployeeInfo>
{{
    new EmployeeInfo(""Ada Lovelace"", ""Research""),
    new EmployeeInfo(""Grace Hopper"", ""Engineering""),
    new EmployeeInfo(""Radia Perlman"", ""Networking""),
    // ...
}};
";

        #endregion
    }

    public class EmployeeInfo
    {
        public EmployeeInfo(string displayName, string department)
        {
            DisplayName = displayName;
            Department = department;
        }

        public string DisplayName { get; }

        public string Department { get; }

        /// <summary>Shown in the list so the effect of switching SearchMemberPath is visible.</summary>
        public string Summary => $"{DisplayName} — {Department}";
    }
}
