using System;
using Xamarin.Forms;
using Splat;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Diagnostics;

namespace ReactiveUI.XamForms
{
    public class RoutedViewHost : NavigationPage, IActivatable
    {
        public RoutedViewHost()
        {
            var screen = Locator.Current.GetService<IScreen>();

            this.WhenActivated(d => {
                d(screen.Router.Navigate
                    .Select(_ => screen.Router.GetCurrentViewModel())
                    .Select(x => pageForViewModel(x))
                    .SelectMany(x => this.PushAsync(x).ToObservable())
                    .Subscribe());

                d(screen.Router.NavigateAndReset
                    .Select(_ => pageForViewModel(screen.Router.GetCurrentViewModel()))
                    .SelectMany(async x => {
                        await this.PopToRootAsync();
                        await this.PushAsync(x);
                        return x;
                    })
                    .Subscribe());

                bool popping = false;
                d(screen.Router.NavigateBack
                    .SelectMany(async x => {
                        popping = true;
                        await this.PopAsync();
                        popping = false;

                        return x;
                    })
                    .Do(_ => ((IViewFor)this.CurrentPage).ViewModel = screen.Router.GetCurrentViewModel())
                    .Subscribe());

                var poppingEvent = Observable.FromEventPattern<NavigationEventArgs>(x => this.Popped += x, x => this.Popped -= x);

                // NB: Catch when the user hit back as opposed to the application
                // requesting Back via NavigateBack
                d(poppingEvent
                    .Where(_ => !popping)
                    .Subscribe(_ => {
                        screen.Router.NavigationStack.RemoveAt(screen.Router.NavigationStack.Count - 1);
                        ((IViewFor)this.CurrentPage).ViewModel = screen.Router.GetCurrentViewModel();
                    }));
            });

            screen.Router.NavigationStack.ToObservable()
                .Select(x => (Page)ViewLocator.Current.ResolveView(x))
                .SelectMany(x => this.PushAsync(x).ToObservable())
                .Finally(() => ((IViewFor)this.CurrentPage).ViewModel = screen.Router.GetCurrentViewModel())
                .Subscribe();
        }

        Page pageForViewModel(IRoutableViewModel vm)
        {
            var ret = ViewLocator.Current.ResolveView(vm);
            ret.ViewModel = vm;

            this.Title = vm.UrlPathSegment;
            return (Page)ret;
        }
    }
}
