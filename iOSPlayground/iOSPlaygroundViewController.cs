using System;
using System.Drawing;
using System.Reactive.Concurrency;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;
using ReactiveUI.Routing;
using System.ComponentModel;

namespace iOSPlayground
{
    public partial class iOSPlaygroundViewController : UIViewController, IViewFor<iOSPlaygroundViewModel>, INotifyPropertyChanged
    {
        #region Boring copy-paste code I want to die
        iOSPlaygroundViewModel _ViewModel;
        public iOSPlaygroundViewModel ViewModel {
            get { return _ViewModel; }
            set {
                if (_ViewModel == value) return;
                _ViewModel = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ViewModel"));
            }
        }
        
        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (iOSPlaygroundViewModel)value; }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public iOSPlaygroundViewController() : base ("iOSPlaygroundViewController", null)
        {
        }
        
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            
            // Release any cached data, images, etc that aren't in use.
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            RxApp.DeferredScheduler.Schedule(() => Console.WriteLine("Bar"));
        }
        
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            // Return true for supported orientations
            return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
        }
    }

    public class iOSPlaygroundViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment {
            get { return "Initial View"; }
        }

        public IScreen HostScreen { get; protected set; }

        public iOSPlaygroundViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen ?? RxApp.GetService<IScreen>();
        }
    }
}

