using System;
using System.Drawing;
using System.Reactive.Concurrency;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;

namespace iOSPlayground
{
    public partial class iOSPlaygroundViewController : UIViewController
    {
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
}

