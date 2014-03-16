using System;
using ReactiveUI;
using ReactiveUI.Mobile;

namespace AndroidPlayground
{
    public class AppBootstrapper : IApplicationRootState
    {
        public RoutingState Router { get; protected set; }

        public AppBootstrapper()
        {
            Router = new RoutingState();
            Router.Navigate.Execute(new MainViewModel(this));

            App.Current.Locator.Register(() => this, typeof(IScreen));
            App.Current.Locator.Register(() => this, typeof(IApplicationRootState));
        }
    }
}

