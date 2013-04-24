using System;
using ReactiveUI.Routing;
using System.Linq;
using System.Reactive.Linq;

#if UIKIT
using MonoTouch.UIKit;

using NSView = MonoTouch.UIKit.UIView;
using NSViewController = MonoTouch.UIKit.UIViewController;
#else
using MonoMac.AppKit;
#endif

namespace ReactiveUI.Cocoa
{
    public class RoutedViewHost : ReactiveObject
    {
        public RoutedViewHost(NSView targetView)
        {
            NSView viewLastAdded = null;
            this.WhenAny(x => x.Router.NavigationStack, x => x.Value)
                .SelectMany(x => x.CollectionCountChanged.StartWith(x.Count).Select(_ => x.LastOrDefault()))
                .Subscribe(vm => {
                    if (viewLastAdded != null) viewLastAdded.RemoveFromSuperview();

                    if (vm == null) {
                        if (DefaultContent != null) targetView.AddSubview(DefaultContent.View);
                        return;
                    }
                    
                    var view = RxRouting.ResolveView(vm);
                    view.ViewModel = vm;

                    if (view is NSViewController) {
                        viewLastAdded = ((NSViewController)view).View;
                    } else if (view is NSView) { 
                        viewLastAdded = (NSView)view;
                    } else {
                        throw new Exception(String.Format("'{0}' must be an NSViewController or NSView", view.GetType().FullName));
                    }

                    targetView.AddSubview(viewLastAdded);           
                }, ex => RxApp.DefaultExceptionHandler.OnNext(ex));
        }

        IRoutingState _Router;
        public IRoutingState Router {
            get { return _Router; }
            set { this.RaiseAndSetIfChanged(x => x.Router, value); }
        }
        
        NSViewController _DefaultContent;
        public NSViewController DefaultContent {
            get { return _DefaultContent; }
            set { this.RaiseAndSetIfChanged(x => x.DefaultContent, value); }
        }
    }
}
