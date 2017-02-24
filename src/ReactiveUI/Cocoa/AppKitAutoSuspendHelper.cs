using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Splat;

#if UNIFIED
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif

namespace ReactiveUI
{
    public class AutoSuspendHelper : IEnableLogger
    {
        readonly Subject<IDisposable> shouldPersistState = new Subject<IDisposable>();
        readonly Subject<Unit> isResuming = new Subject<Unit>();
        readonly Subject<Unit> isUnpausing = new Subject<Unit>();

        public AutoSuspendHelper(NSApplicationDelegate appDelegate)
        {
            Reflection.ThrowIfMethodsNotOverloaded("AutoSuspendHelper", appDelegate,
                "ApplicationShouldTerminate", "DidFinishLaunching", "DidResignActive", "DidBecomeActive", "DidHide");

            RxApp.SuspensionHost.IsLaunchingNew = Observable<Unit>.Never;
            RxApp.SuspensionHost.IsResuming = isResuming;
            RxApp.SuspensionHost.IsUnpausing = isUnpausing;
            RxApp.SuspensionHost.ShouldPersistState = shouldPersistState;

            var untimelyDemise = new Subject<Unit>();
            AppDomain.CurrentDomain.UnhandledException += (o, e) => 
                untimelyDemise.OnNext(Unit.Default);

            RxApp.SuspensionHost.ShouldInvalidateState = untimelyDemise;
        }

        public NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
                shouldPersistState.OnNext(Disposable.Create(() =>
                    sender.ReplyToApplicationShouldTerminate(true))));

            return NSApplicationTerminateReply.Later;
        }

        public void DidFinishLaunching(NSNotification notification)
        {
            isResuming.OnNext(Unit.Default);
        }

        public void DidResignActive(NSNotification notification)
        {
            shouldPersistState.OnNext(Disposable.Empty);
        }

        public void DidBecomeActive(NSNotification notification)
        {
            isUnpausing.OnNext(Unit.Default);
        }

        public void DidHide(NSNotification notification)
        {
            shouldPersistState.OnNext(Disposable.Empty);
        }
    }
}