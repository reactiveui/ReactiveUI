using System;
using MonoTouch.UIKit;
using ReactiveUI;
using ReactiveUI.Cocoa;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using MonoTouch.Foundation;
using System.ComponentModel;
using Splat;
using ReactiveUI;

namespace ReactiveUI.Cocoa
{
    public class RouterUINavigationController : UINavigationController
    {
        readonly RoutingState router;
        readonly Subject<Unit> orientationChanged = new Subject<Unit>();

        public RouterUINavigationController(RoutingState router, IViewLocator viewLocator = null)
        {
            this.router = router;
            viewLocator = viewLocator ?? ViewLocator.Current;

            var platform = Locator.Current.GetService<IPlatformOperations>();

            var vmAndContract = Observable.CombineLatest(
                router.Navigate,
                orientationChanged,
                (vm, _) => new { ViewModel = vm, Contract = platform.GetOrientation() });

            vmAndContract.Subscribe (x => {
                var view = viewLocator.ResolveView(x.ViewModel, x.Contract);
                view.ViewModel = x.ViewModel;

                this.PushViewController((UIViewController)view, true);
            });

            router.NavigateBack.Subscribe(_ => this.PopViewControllerAnimated(true));
            router.NavigateAndReset.Subscribe (x => {
                this.PopToRootViewController(false);
                router.Navigate.Execute(x);
            });

            this.Delegate = new RouterUINavigationControllerDelegate(this, router);
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            orientationChanged.OnNext(Unit.Default);
        }

        bool dontPopWhileYouPop = false;

        [Export("navigationBar:shouldPopItem:")]
        bool navigationBarShouldPopItem(UINavigationBar navigationBar, UINavigationItem item)
        {
            if (dontPopWhileYouPop) return true;

            RxApp.MainThreadScheduler.Schedule (() => {
                if (!router.NavigateBack.CanExecute(null) || dontPopWhileYouPop) return;
                dontPopWhileYouPop = true;
                router.NavigateBack.Execute(null);
            });

            return false;
        }
    
        class RouterUINavigationControllerDelegate : UINavigationControllerDelegate
        {
            RoutingState router;
            RouterUINavigationController parent;
            IDisposable prevBackWireup = Disposable.Empty;

            public RouterUINavigationControllerDelegate(RouterUINavigationController parent, RoutingState router)
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

