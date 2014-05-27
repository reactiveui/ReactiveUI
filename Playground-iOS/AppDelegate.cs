using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI.Mobile;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive;
using Splat;

namespace MobileSample_iOS
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
            var r = Locator.CurrentMutable;
            (new ReactiveUI.Registrations()).Register((f, t) => r.Register(f, t));
            (new ReactiveUI.Mobile.Registrations()).Register((f, t) => r.Register(f, t));

            r.Register(() => new AppBootstrapper(), typeof(IApplicationRootState));
            r.Register(() => new DummySuspensionDriver(), typeof(ISuspensionDriver));

            var host = r.GetService<ISuspensionHost>();
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

