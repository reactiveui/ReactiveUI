using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using Splat;

namespace ReactiveUI.Avalonia
{
    public static class AppBuilderExtensions
    {
        public static TAppBuilder UseReactiveUI<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            Locator.CurrentMutable.Register<IActivationForViewFetcher>(() => new ActivationForViewFetcher());
            return builder.AfterSetup(_ =>
            {
                RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
            });
        }
    }
}