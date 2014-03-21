using System;
using ReactiveUI;
using ReactiveUI.Mobile;
using Splat;

namespace AndroidPlayground
{
    public class AppBootstrapper : IApplicationRootState
    {
        public RoutingState Router { get; protected set; }

        public AppBootstrapper()
        {
            Locator.CurrentMutable.Register(() => this, typeof(IScreen));
            Locator.CurrentMutable.Register(() => this, typeof(IApplicationRootState));

            Router = new RoutingState();
            Router.Navigate.Execute(new MainViewModel(this));
        }
    }
}

