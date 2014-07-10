using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;

namespace PlaygroundiOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        public override UIWindow Window { get; set; }
        readonly AutoSuspendHelper autoSuspendHelper;

        public AppDelegate()
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            RxApp.SuspensionHost.CreateNewAppState = () => new AppState();
            RxApp.SuspensionHost.SetupDefaultSuspendResume();

            autoSuspendHelper = new AutoSuspendHelper(this);
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            autoSuspendHelper.FinishedLaunching(application, launchOptions);
            return true;
        }

        public override void OnActivated(UIApplication application)
        {
            autoSuspendHelper.OnActivated(application);
        }

        public override void DidEnterBackground(UIApplication application)
        {
            autoSuspendHelper.DidEnterBackground(application);
        }
    }
}

