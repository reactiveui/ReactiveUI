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
        public static readonly BindableProperty RouterProperty = BindableProperty.Create<RoutedViewHost, RoutingState>(
            x => x.Router, null, BindingMode.OneWay);

        public RoutingState Router {
            get { return (RoutingState)GetValue(RouterProperty); }
            set { SetValue(RouterProperty, value); }
        }

        public RoutedViewHost()
        {
            this.WhenActivated(new Action<Action<IDisposable>>(d => {
                bool currentlyPopping = false;

                d (this.WhenAnyObservable (x => x.Router.NavigationStack.Changed)
                    .Where(_ => Router.NavigationStack.IsEmpty)
                    .SelectMany (_ => pageForViewModel (Router.GetCurrentViewModel ()))
                    .SelectMany (async x => {
                        currentlyPopping = true;
                        await this.PopToRootAsync ();
                        currentlyPopping = false;

                        return x;
                    })
                    .Subscribe ());

                d(this.WhenAnyObservable(x => x.Router.Navigate)
                    .SelectMany(_ => pageForViewModel(Router.GetCurrentViewModel()))
                    .SelectMany(x => this.PushAsync(x).ToObservable())
                    .Subscribe());

                d(this.WhenAnyObservable(x => x.Router.NavigateBack)
                    .SelectMany(async x => {
                        currentlyPopping = true;
                        await this.PopAsync();
                        currentlyPopping = false;

                        return x;
                    })
                    .Do(_ => ((IViewFor)this.CurrentPage).ViewModel = Router.GetCurrentViewModel())
                    .Subscribe());

                var poppingEvent = Observable.FromEventPattern<NavigationEventArgs>(x => this.Popped += x, x => this.Popped -= x);

                // NB: Catch when the user hit back as opposed to the application
                // requesting Back via NavigateBack
                d(poppingEvent
                    .Where(_ => !currentlyPopping && Router != null)
                    .Subscribe(_ => {
                        Router.NavigationStack.RemoveAt(Router.NavigationStack.Count - 1);
                        ((IViewFor)this.CurrentPage).ViewModel = Router.GetCurrentViewModel();
                    }));
            }));

            var screen = Locator.Current.GetService<IScreen>();
            if (screen == null) throw new Exception("You *must* register an IScreen class representing your App's main Screen");

            Router = screen.Router;

            this.WhenAnyValue(x => x.Router)
                .SelectMany(router => {
                    return router.NavigationStack.ToObservable()
                            .Select(x => (Page)ViewLocator.Current.ResolveView(x))
                            .SelectMany(x => this.PushAsync(x).ToObservable())
                            .Finally(() => {

                        var vm = router.GetCurrentViewModel();
                        if (vm == null) return;

                        ((IViewFor)this.CurrentPage).ViewModel = vm;
                        this.CurrentPage.Title = vm.UrlPathSegment;
                    });
                })
                .Subscribe();
        }

        IObservable<Page> pageForViewModel(IRoutableViewModel vm) 
        {
            if (vm == null) return Observable.Empty<Page>();

            var ret = ViewLocator.Current.ResolveView(vm);
            if (ret == null) {
                var msg = String.Format(
                    "Couldn't find a View for ViewModel. You probably need to register an IViewFor<{0}>",
                    vm.GetType().Name);

                return Observable.Throw<Page>(new Exception(msg));
            }

            ret.ViewModel = vm;

            var pg = (Page)ret;
            pg.Title = vm.UrlPathSegment;
            return Observable.Return(pg);
        }
    }
}

