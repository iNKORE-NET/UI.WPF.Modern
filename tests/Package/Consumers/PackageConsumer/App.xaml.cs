using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PackageConsumer
{
    /// <summary>
    /// A smoke test that checks itself; MainWindow runs the checks once it has rendered. Exits with 0 when they pass,
    /// 1 when one fails or anything throws, and 2 when the window doesn't render in time.
    /// </summary>
    public partial class App : Application
    {
        static readonly TextWriter Output = TextWriter.Synchronized(
            new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false)) { AutoFlush = true });
        static readonly Timer Watchdog;

        static App()
        {
            // Set before any UI exists, so the controls load their zh-TW strings.
            var culture = new CultureInfo("zh-TW");
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = culture;

            AppDomain.CurrentDomain.UnhandledException += (s, e) => Fail(e.ExceptionObject.ToString());
            TaskScheduler.UnobservedTaskException += (s, e) => Fail(e.Exception.ToString());
            Watchdog = new Timer(_ => Quit(2, "FAIL The window didn't render within 60 seconds."), null, 60000, Timeout.Infinite);
        }

        public App()
        {
            DispatcherUnhandledException += (s, e) => Fail(e.Exception.ToString());
        }

        public static void Log(string line) => Output.WriteLine(line);

        public static void Pass()
        {
            Watchdog.Dispose();
            Log("PASS");
            Current.Shutdown(0);
        }

        public static void Fail(string message) => Quit(1, "FAIL " + message);

        static void Quit(int code, string message)
        {
            Log(message);
            Environment.Exit(code);
        }
    }
}
