using Avalonia.Controls;
using Avalonia.Threading;

namespace ReactiveUI.Avalonia
{
    public static class AppBuilderExtensions
    {
        public static TAppBuilder UseReactiveUI<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new() => 
            builder.AfterSetup(_ => 
            {
                RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
            });
    }
}
