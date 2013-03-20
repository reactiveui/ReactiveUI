using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI.Mobile;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive;

namespace iOSPlayground
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register ("AppDelegate")]
    public partial class AppDelegate : AutoSuspendAppDelegate
    {
        public override bool WillFinishLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // NB: Hax
            (new ReactiveUI.Xaml.ServiceLocationRegistration()).Register();
            (new ReactiveUI.Routing.ServiceLocationRegistration()).Register();
            (new ReactiveUI.Cocoa.ServiceLocationRegistration()).Register();
            (new ReactiveUI.Mobile.ServiceLocationRegistration()).Register();

            RxApp.Register(typeof(AppBootstrapper), typeof(IApplicationRootState));
            RxApp.Register(typeof(DummySuspensionDriver), typeof(ISuspensionDriver));

            var host = RxApp.GetService<ISuspensionHost>();
            host.SetupDefaultSuspendResume();

            return true;
        }
    }

    public class DummySuspensionDriver : ISuspensionDriver
    {
        public IObservable<T> LoadState<T>() where T : class, IApplicationRootState
        {
            return Observable.Throw<T>(new Exception("Didn't work lol"));
        }

        public IObservable<Unit> SaveState<T>(T state) where T : class, IApplicationRootState
        {
            return Observable.Return(Unit.Default);
        }

        public IObservable<Unit> InvalidateState()
        {
            return Observable.Return(Unit.Default);
        }
    }
}

