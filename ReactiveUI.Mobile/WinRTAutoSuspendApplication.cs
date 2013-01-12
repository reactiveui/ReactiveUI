using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace ReactiveUI.Mobile
{
    class WinRTSuspensionHost : ISuspensionHost
    {
        public IObservable<Unit> IsLaunchingNew { get { return WinRTAutoSuspendApplication.SuspensionHost.IsLaunchingNew; } }
        public IObservable<Unit> IsResuming { get { return WinRTAutoSuspendApplication.SuspensionHost.IsResuming; } }
        public IObservable<Unit> IsUnpausing { get { return WinRTAutoSuspendApplication.SuspensionHost.IsUnpausing; } }
        public IObservable<Unit> ShouldPersistState { get { return WinRTAutoSuspendApplication.SuspensionHost.ShouldPersistState; } }
        public IObservable<Unit> ShouldInvalidateState { get { return WinRTAutoSuspendApplication.SuspensionHost.ShouldInvalidateState; } }
    }

    public abstract class WinRTAutoSuspendApplication : Application
    {
        readonly Subject<LaunchActivatedEventArgs> _onLaunched = new Subject<LaunchActivatedEventArgs>();
        internal static ISuspensionHost SuspensionHost;

        protected WinRTAutoSuspendApplication()
        {
            var host = new SuspensionHost();

            var launchNew = new[] { ApplicationExecutionState.ClosedByUser, ApplicationExecutionState.NotRunning, };
            host.IsLaunchingNew = _onLaunched
                .Where(x => launchNew.Contains(x.PreviousExecutionState))
                .Select(_ => Unit.Default);

            host.IsResuming = _onLaunched
                .Where(x => x.PreviousExecutionState == ApplicationExecutionState.Terminated)
                .Select(_ => Unit.Default);

            var unpausing = new[] { ApplicationExecutionState.Suspended, ApplicationExecutionState.Running, };
            host.IsUnpausing = _onLaunched
                .Where(x => unpausing.Contains(x.PreviousExecutionState))
                .Select(_ => Unit.Default);

            host.ShouldPersistState =
                Observable.FromEvent<SuspendingEventHandler, SuspendingEventArgs>(x => Suspending += x, x => Suspending -= x)
                    .Select(_ => Unit.Default);

            host.ShouldInvalidateState =
                Observable.FromEvent<UnhandledExceptionEventHandler, UnhandledExceptionEventArgs>(x => UnhandledException += x, x => UnhandledException -= x)
                    .Select(_ => Unit.Default);


            SuspensionHost = host;
            RxApp.Register(typeof(WinRTSuspensionHost), typeof(ISuspensionHost));
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            _onLaunched.OnNext(args);
        }
    }
}