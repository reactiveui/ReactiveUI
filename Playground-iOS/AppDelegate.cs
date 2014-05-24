using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI.Mobile;

namespace PlaygroundiOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : AutoSuspendAppDelegate
    {
        // class-level declarations
        public override UIWindow Window { get; set; }
    }
}

