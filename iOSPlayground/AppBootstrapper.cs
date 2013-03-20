using System;
using ReactiveUI;
using ReactiveUI.Mobile;
using ReactiveUI.Routing;
using System.Runtime.Serialization;
using TinyIoC;
using MonoTouch.UIKit;

namespace iOSPlayground
{
    public class AppBootstrapper : ReactiveObject, IApplicationRootState
    {
        [IgnoreDataMember]
        public TinyIoCContainer Kernel { get; protected set; }
        
        [DataMember]
        public IRoutingState Router { get; protected set; }

        public AppBootstrapper()
        {
            Router = new RoutingState();

            Kernel = new TinyIoCContainer();

            // XXX: This is gross
            Kernel.Register<UIViewController>(new RouterUINavigationController(Router), "InitialPage");

            Kernel.Register(typeof(IViewFor<iOSPlaygroundViewModel>), typeof(iOSPlaygroundViewController));
            Kernel.Register<IScreen>(this);

            RxApp.ConfigureServiceLocator(
                (t, s) => s != null ? Kernel.Resolve(t, s) : Kernel.Resolve(t),
                (t, s) => Kernel.ResolveAll(t, true),
                (c, t, s) => { if (s != null) { Kernel.Register(t, c, s); } else { Kernel.Register(t, c); } } );

            Router.Navigate.Go<iOSPlaygroundViewModel>();
        }
    }
}