using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

namespace SettingsPageExample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var en = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = en;
            CultureInfo.CurrentUICulture = en;
        }
    }

}
