using System;
using ReactiveUI.Routing;
using MonoTouch.UIKit;
using ReactiveUI;
using ReactiveUI.Cocoa;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using MonoTouch.Foundation;
using System.ComponentModel;

namespace ReactiveUI.Routing
{
    public class RouterUINavigationController : UINavigationController
    {
        readonly IRoutingState router;

        public RouterUINavigationController(IRoutingState router)
        {
            this.router = router;

            router.Navigate.Subscribe (x => {
                var view = RxRouting.ResolveView(x);
                view.ViewModel = x;

                this.PushViewController((UIViewController)view, true);
            });

            router.NavigateBack.Subscribe(_ => this.PopViewControllerAnimated(true));
            router.NavigateAndReset.Subscribe (x => {
                this.PopToRootViewController(false);
                router.Navigate.Execute(x);
            });

            this.Delegate = new RouterUINavigationControllerDelegate(this, router);
        }

        bool dontPopWhileYouPop = false;

        [Export("navigationBar:shouldPopItem:")]
        bool navigationBarShouldPopItem(UINavigationBar navigationBar, UINavigationItem item)
        {
            if (dontPopWhileYouPop) return true;

            RxApp.DeferredScheduler.Schedule (() => {
                if (!router.NavigateBack.CanExecute(null) || dontPopWhileYouPop) return;
                dontPopWhileYouPop = true;
                router.NavigateBack.Execute(null);
            });

            return false;
        }
    
        class RouterUINavigationControllerDelegate : UINavigationControllerDelegate
        {
            IRoutingState router;
            RouterUINavigationController parent;
            IDisposable prevBackWireup = Disposable.Empty;

            public RouterUINavigationControllerDelegate(RouterUINavigationController parent, IRoutingState router)
            {
                this.parent = parent;
                this.router = router;
            }

            public override void DidShowViewController (UINavigationController navigationController, UIViewController viewController, bool animated)
            {
                var viewFor = viewController as IViewFor;
                if (viewFor != null) {
                    var vm = router.GetCurrentViewModel();
                    viewFor.ViewModel = vm;
                    viewController.Title = vm.UrlPathSegment;
                }

                parent.dontPopWhileYouPop = false;
            }
        }
    }
}

