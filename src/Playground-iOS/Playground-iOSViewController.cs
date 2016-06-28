using System;
using System.Drawing;
using System.Reactive.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;

namespace PlaygroundiOS
{
    public partial class Playground_iOSViewController : UIViewController
    {
        public Playground_iOSViewController(IntPtr handle) : base(handle)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            
            // Release any cached data, images, etc that aren't in use.
        }

#region View lifecycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            RxApp.SuspensionHost.ObserveAppState<AppState>()
                .Select(x => x.SavedGuid)
                .BindTo(this, x => x.savedGuid.Text);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

#endregion
    }
}

