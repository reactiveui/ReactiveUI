using System;
using Xamarin.Forms;
using Splat;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Diagnostics;
using System.Reactive;

namespace ReactiveUI.XamForms
{
    public class RoutedViewHost : NavigationPage, IActivatable
    {
        public static readonly BindableProperty RouterProperty = BindableProperty.Create(
            nameof(Router),
            typeof(RoutingState),
            typeof(RoutedViewHost),
            default(RoutingState),
            BindingMode.OneWay);

        public RoutingState Router {
            get { return (RoutingState)GetValue(RouterProperty); }
            set { SetValue(RouterProperty, value); }
        }

        public RoutedViewHost()
        {
            this.WhenActivated(new Action<Action<IDisposable>>(d => {
                bool currentlyPopping = false;
                bool popToRootPending = false;
                bool userInstigated = false;

                d (this.WhenAnyObservable (x => x.Router.NavigationStack.Changed)
                    .Where(_ => Router.NavigationStack.IsEmpty)
                    .Select(x => {
                        // Xamarin Forms does not let us completely clear down the navigation stack
                        // instead, we have to delay this request momentarily until we receive the new root view
                        // then, we can insert the new root view first, and then pop to it
                        popToRootPending = true;
                        return x;
                    })
                    .Subscribe ());

                var previousCount = this.WhenAnyObservable(x => x.Router.NavigationStack.CountChanged).StartWith(this.Router.NavigationStack.Count);
                var currentCount = previousCount.Skip(1);

                d (Observable.Zip(previousCount, currentCount, (previous, current) => new { Delta = previous - current, Current = current })
                    .Where(_ => !userInstigated)
                    .Where(x => x.Delta > 0)
                    .SelectMany(
                        async x =>
                        {
                            // XF doesn't provide a means of navigating back more than one screen at a time apart from navigating right back to the root page
                            // since we want as sensible an animation as possible, we pop to root if that makes sense. Otherwise, we pop each individual
                            // screen until the delta is made up, animating only the last one
                            var popToRoot = x.Current == 1;
                            currentlyPopping = true;

                            try
                            {
                                if (popToRoot)
                                {
                                    await this.PopToRootAsync(true);
                                }
                                else
                                {
                                    for (var i = 0; i < x.Delta; ++i)
                                    {
                                        await this.PopAsync(i == x.Delta - 1);
                                    }
                                }
                            }
                            finally
                            {
                                currentlyPopping = false;
                                ((IViewFor)this.CurrentPage).ViewModel = Router.GetCurrentViewModel();
                            }

                            return Unit.Default;
                        })
                    .Subscribe());

                d(this.WhenAnyObservable(x => x.Router.Navigate)
                    .SelectMany(_ => PageForViewModel(Router.GetCurrentViewModel()))
                    .SelectMany(async x => {
                        if (popToRootPending && this.Navigation.NavigationStack.Count > 0)
                        {
                            this.Navigation.InsertPageBefore(x, this.Navigation.NavigationStack[0]);
                            await this.PopToRootAsync();
                        }
                        else
                        {
                            await this.PushAsync(x);
                        }

                        popToRootPending = false;
                        return x;
                    })
                    .Subscribe());

                var poppingEvent = Observable.FromEventPattern<NavigationEventArgs>(x => this.Popped += x, x => this.Popped -= x);

                // NB: Catch when the user hit back as opposed to the application
                // requesting Back via NavigateBack
                d(poppingEvent
                    .Where(_ => !currentlyPopping && Router != null)
                    .Subscribe(_ => {
                        userInstigated = true;

                        try {
                            Router.NavigationStack.RemoveAt(Router.NavigationStack.Count - 1);
                        } finally {
                            userInstigated = false;
                        }

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

        protected IObservable<Page> PageForViewModel(IRoutableViewModel vm)
        {
            if (vm == null) return Observable<Page>.Empty;

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

