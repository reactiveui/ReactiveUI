using Avalonia;
using Avalonia.Logging.Serilog;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace IntegrationTests.Avalonia
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>();
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug();
    }
}
