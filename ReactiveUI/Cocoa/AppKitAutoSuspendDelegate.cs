using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Reactive.Disposables;

namespace ReactiveUI
{
    public class AutoSuspendAppDelegate : NSApplicationDelegate
    {
        readonly Subject<IDisposable> shouldPersistState = new Subject<IDisposable>();
        readonly Subject<Unit> isResuming = new Subject<Unit>();
        readonly Subject<Unit> isUnpausing = new Subject<Unit>();

        public AutoSuspendAppDelegate()
        {
            RxApp.SuspensionHost.IsLaunchingNew = Observable.Never<Unit>();
            RxApp.SuspensionHost.IsResuming = isResuming;
            RxApp.SuspensionHost.IsUnpausing = isUnpausing;
            RxApp.SuspensionHost.ShouldPersistState = shouldPersistState;

            var untimelyDemise = new Subject<Unit>();
            AppDomain.CurrentDomain.UnhandledException += (o, e) => 
                untimelyDemise.OnNext(Unit.Default);

            RxApp.SuspensionHost.ShouldInvalidateState = untimelyDemise;
        }

        public override NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
                shouldPersistState.OnNext(Disposable.Create(() =>
                    sender.ReplyToApplicationShouldTerminate(true))));

            return NSApplicationTerminateReply.Later;
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            isResuming.OnNext(Unit.Default);
        }

        public override void DidResignActive(NSNotification notification)
        {
            shouldPersistState.OnNext(Disposable.Empty);
        }

        public override void DidBecomeActive(NSNotification notification)
        {
            isUnpausing.OnNext(Unit.Default);
        }

        public override void DidHide(NSNotification notification)
        {
            shouldPersistState.OnNext(Disposable.Empty);
        }
    }
}