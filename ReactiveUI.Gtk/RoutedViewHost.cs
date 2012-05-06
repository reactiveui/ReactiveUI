using System;
using Gtk;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Routing;

namespace ReactiveUI.Gtk
{
    [System.ComponentModel.ToolboxItem(true)]
    public class RoutedViewHost : Bin
    {
        IRoutingState _Router;
        public IRoutingState Router {
            get { return _Router; }
            set {
                _Router = value;
                updateRouter();
            }
        }
        
        public Widget DefaultContent { get; set;}
    
        IDisposable currentRouterSub = null;
        void updateRouter()
        {
            if (currentRouterSub != null) {
                currentRouterSub.Dispose();
                currentRouterSub = null;
            }
            
            if (Router == null)
                return;
            
            currentRouterSub = Router.ViewModelObservable().Subscribe(vm => {
                if (vm == null) {
                    Child = DefaultContent;
                    return;
                }
                
                var view = RxRouting.ResolveView(vm);
                view.ViewModel = vm;
                Child = (Widget)view;
            });
        }
    }
}

