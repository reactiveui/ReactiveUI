using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using ReactiveUI;

using Xamarin.Forms;

namespace PlaygroundXamForms.iOS
{
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        UIWindow window;
        AutoSuspendHelper suspendHelper;
        UIViewController vc;

        public AppDelegate()
        {
            RxApp.SuspensionHost.CreateNewAppState = () => new AppBootstrapper();
        }

        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {
            Forms.Init ();
            RxApp.SuspensionHost.SetupDefaultSuspendResume();

            suspendHelper = new AutoSuspendHelper(this);
            suspendHelper.FinishedLaunching(app, options);

            window = new UIWindow (UIScreen.MainScreen.Bounds);
            var vc = RxApp.SuspensionHost.GetAppState<AppBootstrapper>().CreateMainView().CreateViewController();

            window.RootViewController = vc;
            window.MakeKeyAndVisible ();

            return true;
        }

        public override void DidEnterBackground(UIApplication application)
        {
            suspendHelper.DidEnterBackground(application);
        }

        public override void OnActivated(UIApplication application)
        {
            suspendHelper.OnActivated(application);
        }
    }
}

