using System;
using MonoMac.AppKit;
using ReactiveUI.Routing;
using MonoMac.Foundation;
using System.Reactive;

namespace ReactiveUI.Cocoa
{
    public class ViewModelViewHost : ReactiveObject 
    {
        public ViewModelViewHost(NSView targetView)
        {
            NSView viewLastAdded = null;
            this.WhenAny(x => x.DefaultContent, x => x.ViewModel, (c,v) => Unit.Default)
                .Subscribe(_ => {
                    if (viewLastAdded != null) viewLastAdded.RemoveFromSuperview();

                    if (ViewModel == null) {
                        if (DefaultContent != null) targetView.AddSubview(DefaultContent.View);
                        return;
                    }

                    var view = RxRouting.ResolveView(ViewModel);
                    view.ViewModel = ViewModel;

                    viewLastAdded = ((NSViewController)view).View;
                    targetView.AddSubview(viewLastAdded);           
                });
        }

        NSViewController _DefaultContent;
        public NSViewController DefaultContent {
            get { return _DefaultContent; }
            set { this.RaiseAndSetIfChanged(x => x.DefaultContent, value); }
        }

        IReactiveNotifyPropertyChanged _ViewModel;
        public IReactiveNotifyPropertyChanged ViewModel {
            get { return _ViewModel; }
            set { this.RaiseAndSetIfChanged(x => x.ViewModel, value); }
        }
    }
}
