using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using Splat;

#if UNIFIED
using Foundation;
using UIKit;
using NSAction = System.Action;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// AutoSuspend-based App Delegate. To use AutoSuspend with iOS, change your
    /// AppDelegate to inherit from this class, then call:
    /// 
    /// Locator.Current.GetService<ISuspensionHost>().SetupDefaultSuspendResume();
    /// </summary>
    public class AutoSuspendHelper : IEnableLogger
    {
        readonly Subject<UIApplication> _finishedLaunching = new Subject<UIApplication>();
        readonly Subject<UIApplication> _activated = new Subject<UIApplication>();
        readonly Subject<UIApplication> _backgrounded = new Subject<UIApplication>();

        public IDictionary<string, string> LaunchOptions { get; protected set; }

        public AutoSuspendHelper(UIApplicationDelegate appDelegate)
        {
            Reflection.ThrowIfMethodsNotOverloaded("AutoSuspendHelper", appDelegate,
                "FinishedLaunching", "OnActivated", "DidEnterBackground");

            RxApp.SuspensionHost.IsLaunchingNew = Observable<Unit>.Never;
            RxApp.SuspensionHost.IsResuming = _finishedLaunching.Select(_ => Unit.Default);
            RxApp.SuspensionHost.IsUnpausing = _activated.Select(_ => Unit.Default);

            var untimelyDeath = new Subject<Unit>();
            AppDomain.CurrentDomain.UnhandledException += (o,e) => untimelyDeath.OnNext(Unit.Default);

            RxApp.SuspensionHost.ShouldInvalidateState = untimelyDeath;

            RxApp.SuspensionHost.ShouldPersistState = _backgrounded.SelectMany(app => {
                var taskId = app.BeginBackgroundTask(new NSAction(() => untimelyDeath.OnNext(Unit.Default)));

                // NB: We're being force-killed, signal invalidate instead
                if (taskId == UIApplication.BackgroundTaskInvalid) {
                    untimelyDeath.OnNext(Unit.Default);
                    return Observable<IDisposable>.Empty;
                }

                return Observable.Return(Disposable.Create(() => app.EndBackgroundTask(taskId)));
            });
        }

        public void FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            if (launchOptions != null) {
                LaunchOptions = launchOptions.Keys.ToDictionary(k => k.ToString(), v => launchOptions[v].ToString());
            } else {
                LaunchOptions = new Dictionary<string, string>();
            }

            // NB: This is run in-context (i.e. not scheduled), so by the time this
            // statement returns, UIWindow should be created already
            _finishedLaunching.OnNext(application);
        }

        public void OnActivated(UIApplication application)
        {
            _activated.OnNext(application);
        }

        public void DidEnterBackground(UIApplication application)
        {
            _backgrounded.OnNext(application);
        }
    }
}

