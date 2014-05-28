using System;
using System.Reactive.Linq;
using ReactiveUI;

#if UIKIT
using MonoTouch.UIKit;
using NSView = MonoTouch.UIKit.UIView;
using NSViewController = MonoTouch.UIKit.UIViewController;
#else
using MonoMac.AppKit;
#endif

namespace ReactiveUI.Cocoa
{
    /// <summary>
    /// ViewModelViewHost is a helper class that will connect a ViewModel
    /// to an arbitrary NSView and attempt to load the View for the current
    /// ViewModel as a child view of the target. 
    /// 
    /// This is a bit different than the XAML's ViewModelViewHost in the sense
    /// that this isn't a Control itself, it only manipulates other Views.
    /// </summary>
    public class ViewModelViewHost : ReactiveObject 
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ReactiveUI.Cocoa.ViewModelViewHost"/>
        /// will automatically create Auto Layout constraints tying the sub view to the parent view.
        /// </summary>
        /// <value><c>true</c> if add layout contraints to sub view; otherwise, <c>false</c>.</value>
        public bool AddAutoLayoutConstraintsToSubView { get; set; } 

        public ViewModelViewHost(NSView targetView)
        {
            if (targetView == null) throw new ArgumentNullException("targetView");

            NSView viewLastAdded = null;
            ViewContractObservable = Observable.Return(default(string));

            var vmAndContract = Observable.CombineLatest(
                this.WhenAny(x => x.ViewModel, x => x.Value),
                this.WhenAnyObservable(x => x.ViewContractObservable),
                (vm, contract) => new { ViewModel = vm, Contract = contract, });

            vmAndContract.Subscribe(x => {
                if (viewLastAdded != null) viewLastAdded.RemoveFromSuperview();

                if (ViewModel == null) {
                    if (DefaultContent != null) targetView.AddSubview(DefaultContent.View);
                    return;
                }

                // get an instance of the view controller for the supplied VM + Contract
                var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                var viewController = viewLocator.ResolveView(x.ViewModel, x.Contract);

                // if not found, throw
                if (viewController == null) {
                    var message = String.Format("Unable to resolve view for \"{0}\"", x.ViewModel.GetType());

                    if (x.Contract != null) {
                        message += String.Format(" and contract \"{1}\"", x.Contract.GetType());
                    }

                    message += ".";
                    throw new Exception(message);
                }

                // set the VM on the controller and stash a copy of the added view
                viewController.ViewModel = x.ViewModel;
                viewLastAdded = ((NSViewController)viewController).View;

                // sanity check, view controllers are expect to have a view
                if (viewLastAdded == null) {
                    var message = string.Format("No view associated with view controller {0}.", viewController.GetType());
                    throw new Exception(message);
                }

                if (AddAutoLayoutConstraintsToSubView) {
                    // see https://developer.apple.com/library/ios/documentation/userexperience/conceptual/AutolayoutPG/AdoptingAutoLayout/AdoptingAutoLayout.html
                    viewLastAdded.TranslatesAutoresizingMaskIntoConstraints = false;
                }

                targetView.AddSubview(viewLastAdded);

                if (AddAutoLayoutConstraintsToSubView) {
                    // Add edge constraints so that subview trails changes in parent
                    addEdgeConstraint(NSLayoutAttribute.Left,  targetView, viewLastAdded);
                    addEdgeConstraint(NSLayoutAttribute.Right, targetView, viewLastAdded);
                    addEdgeConstraint(NSLayoutAttribute.Top, targetView, viewLastAdded);
                    addEdgeConstraint(NSLayoutAttribute.Bottom,  targetView, viewLastAdded);
                }
            });
        }

        void addEdgeConstraint(NSLayoutAttribute edge, NSView parentView, NSView subView)
        {
            var constraint = NSLayoutConstraint.Create(subView, edge, NSLayoutRelation.Equal, parentView, edge, 1, 0);
            parentView.AddConstraint(constraint);
        }

        NSViewController _DefaultContent;
        public NSViewController DefaultContent {
            get { return _DefaultContent; }
            set { this.RaiseAndSetIfChanged(ref _DefaultContent, value); }
        }

        IReactiveObject _ViewModel;
        public IReactiveObject ViewModel {
            get { return _ViewModel; }
            set { this.RaiseAndSetIfChanged(ref _ViewModel, value); }
        }
        
        IObservable<string> _ViewContractObservable;
        public IObservable<string> ViewContractObservable {
            get { return _ViewContractObservable; }
            set { this.RaiseAndSetIfChanged(ref _ViewContractObservable, value); }
        }
       
        public IViewLocator ViewLocator { get; set; }
    }
}
