using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using iNKORE.UI.WPF.Modern;
using iNKORE.UI.WPF.Modern.Controls;
using iNKORE.UI.WPF.Modern.Helpers;

namespace PackageConsumer
{
    public partial class MainWindow : Window
    {
        bool _failed;

        public MainWindow()
        {
            InitializeComponent();
            // ApplicationIdle comes after the first layout and render, and after the controls' own Loaded work.
            ContentRendered += (s, e) => Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(RunChecks));
        }

        void RunChecks()
        {
            // Both assemblies come from the package under test.
            var expected = typeof(MainWindow).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .Single(a => a.Key == "ModernPackageVersion").Value;
            foreach (var assembly in new[] { typeof(ThemeResources).Assembly, typeof(NavigationView).Assembly })
            {
                var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0];
                Expect(version == expected, $"{assembly.GetName().Name} {version} is the package under test");
            }

            var merged = Application.Current.Resources.MergedDictionaries;
            Expect(merged.OfType<ThemeResources>().Any(), "ui:ThemeResources is merged");
            Expect(merged.OfType<XamlControlsResources>().Any(), "ui:XamlControlsResources is merged");
            Expect(TryFindResource("AccentButtonStyle") is Style, "AccentButtonStyle resolves");

            foreach (var control in new Control[] { Navigation, Info, Number })
            {
                Expect(control.IsLoaded && VisualTreeHelper.GetChildrenCount(control) > 0 && control.ActualWidth > 0 && control.ActualHeight > 0,
                    $"{control.GetType().Name} is templated and rendered");
            }

            // Strings from the Controls translations, embedded in its DLL: 設定 is "Settings", 關閉 is "Close".
            Expect(CultureInfo.CurrentUICulture.Name == "zh-TW", "the UI culture is zh-TW");
            var settings = (Navigation.SettingsItem as NavigationViewItem)?.Content as string;
            Expect(settings == "設定", $"the NavigationView settings item reads \"{settings}\", expected \"設定\"");
            var closeButton = Info.Template.FindName("CloseButton", Info) as FrameworkElement;
            var closeTip = ((closeButton == null ? null : ToolTipService.GetToolTip(closeButton)) as ToolTip)?.Content as string;
            Expect(closeTip == "關閉", $"the InfoBar close button tooltip reads \"{closeTip}\", expected \"關閉\"");

#if NO_SYSTEM_COLORS
            App.Log("skip the system colours (known issue on lib/net6.0-windows7.0)");
#else
            Expect(ColorsHelper.Current.SystemTheme.HasValue, "the system theme was read through WinRT");
#endif

            if (_failed) App.Fail("See the failed checks above.");
            else App.Pass();
        }

        void Expect(bool passed, string check)
        {
            App.Log((passed ? "ok   " : "FAIL ") + check);
            _failed |= !passed;
        }
    }
}
