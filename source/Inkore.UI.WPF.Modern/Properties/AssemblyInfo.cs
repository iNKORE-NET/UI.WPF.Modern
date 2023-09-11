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


[assembly: AssemblyTitle("Inkore.UI.WPF.Modern")]

[assembly: InternalsVisibleTo("Inkore.UI.WPF.Modern.Controls")]
//[assembly: InternalsVisibleTo("Inkore.UI.WPF.Modern.MahApps")]
[assembly: InternalsVisibleTo("MUXControlsTestApp")]

[assembly: XmlnsPrefix("http://schemas.inkore.net/lib/ui/wpf/modern", "modern")]
[assembly: XmlnsDefinition("http://schemas.inkore.net/lib/ui/wpf/modern", "Inkore.UI.WPF.Modern")]
[assembly: XmlnsDefinition("http://schemas.inkore.net/lib/ui/wpf/modern", "Inkore.UI.WPF.Modern.Controls")]
[assembly: XmlnsDefinition("http://schemas.inkore.net/lib/ui/wpf/modern", "Inkore.UI.WPF.Modern.Controls.Primitives")]
[assembly: XmlnsDefinition("http://schemas.inkore.net/lib/ui/wpf/modern", "Inkore.UI.WPF.Modern.DesignTime")]
[assembly: XmlnsDefinition("http://schemas.inkore.net/lib/ui/wpf/modern", "Inkore.UI.WPF.Modern.Markup")]
[assembly: XmlnsDefinition("http://schemas.inkore.net/lib/ui/wpf/modern", "Inkore.UI.WPF.Modern.Media")]
[assembly: XmlnsDefinition("http://schemas.inkore.net/lib/ui/wpf/modern", "Inkore.UI.WPF.Modern.Media.Animation")]
