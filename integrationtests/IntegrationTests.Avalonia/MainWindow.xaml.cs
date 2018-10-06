using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IntegrationTests.Avalonia
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            Content = new LoginControl();
#if DEBUG
            this.AttachDevTools();
#endif
        }
    }
}
