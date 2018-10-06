using Avalonia;
using Avalonia.Markup.Xaml;

namespace IntegrationTests.Avalonia
{
    public class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);
    }
}
