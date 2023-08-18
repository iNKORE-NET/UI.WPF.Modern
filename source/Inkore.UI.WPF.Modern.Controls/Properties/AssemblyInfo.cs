using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]


[assembly: AssemblyTitle("Inkore.UI.WPF.Modern.Controls")]
[assembly: AssemblyDescription("Modern (Fluent 2) styles and controls for your WPF applications")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("iNKORE! Studios")]
[assembly: AssemblyProduct("Inkore.UI.WPF.Modern")]
[assembly: AssemblyCopyright("Copyright © iNKORE! Studios 2023")]
[assembly: AssemblyTrademark("iNKORE! Studios")]
[assembly: AssemblyVersion("0.9.16.0")]

//[assembly: InternalsVisibleTo("MUXControlsTestApp")]

[assembly: XmlnsDefinition("http://schemas.inkore.net/lib/ui/wpf/modern", "Inkore.UI.WPF.Modern.Controls")]
[assembly: XmlnsDefinition("http://schemas.inkore.net/lib/ui/wpf/modern", "Inkore.UI.WPF.Modern.Controls.Primitives")]
