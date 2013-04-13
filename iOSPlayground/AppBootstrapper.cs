using System;
using ReactiveUI;
using ReactiveUI.Mobile;
using ReactiveUI.Routing;
using System.Runtime.Serialization;
using TinyIoC;
using MonoTouch.UIKit;
using System.Linq;
using System.Collections.Generic;

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

            var toRegister = new Dictionary<Tuple<Type, String>, List<Type>>();

            RxApp.ConfigureServiceLocator(
                (t, s) => s != null ? Kernel.Resolve(t, s) : Kernel.Resolve(t),
                (t, s) => Kernel.ResolveAll(t, true),
                (c, t, s) => { 
                    if (toRegister != null) {
                        var pair = Tuple.Create(t,s);
                        if (!toRegister.ContainsKey(pair)) {
                            toRegister[pair] = new List<Type>();
                        }
                        toRegister[pair].Add(c);
                        return;
                    }

                    if (s != null) { 
                        Kernel.Register(t, c, s); 
                    } else { 
                        Kernel.Register(t, c); 
                    } 
                });

            foreach(var key in toRegister.Keys) {
                var val = toRegister[key].Distinct();
                Kernel.RegisterMultiple(key.Item1, val);
            }

            toRegister = null;

            var items = Kernel.ResolveAll(typeof(ICreatesObservableForProperty), true).ToArray();

            Router.Navigate.Go<iOSPlaygroundViewModel>();
        }
    }
}