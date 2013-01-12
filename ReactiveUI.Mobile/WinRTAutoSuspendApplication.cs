using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ReactiveUI.Mobile
{
    class WinRTSuspensionHost : ISuspensionHost
    {
        public IObservable<Unit> IsLaunchingNew { get { return WinRTAutoSuspendApplication.SuspensionHost.IsLaunchingNew; } }
        public IObservable<Unit> IsResuming { get { return WinRTAutoSuspendApplication.SuspensionHost.IsResuming; } }
        public IObservable<Unit> IsUnpausing { get { return WinRTAutoSuspendApplication.SuspensionHost.IsUnpausing; } }
        public IObservable<IDisposable> ShouldPersistState { get { return WinRTAutoSuspendApplication.SuspensionHost.ShouldPersistState; } }
        public IObservable<Unit> ShouldInvalidateState { get { return WinRTAutoSuspendApplication.SuspensionHost.ShouldInvalidateState; } }
    }

    public abstract class WinRTAutoSuspendApplication : Application, IEnableLogger
    {
        readonly Subject<LaunchActivatedEventArgs> _launched = new Subject<LaunchActivatedEventArgs>();
        internal static ISuspensionHost SuspensionHost;

        readonly Subject<IApplicationRootState> _viewModelChanged = new Subject<IApplicationRootState>();
        IApplicationRootState _ViewModel;

        public IApplicationRootState ViewModel {
            get { return _ViewModel; }
            set { _ViewModel = value; _viewModelChanged.OnNext(value); }
        }

        protected WinRTAutoSuspendApplication()
        {
            var host = new SuspensionHost();

            var launchNew = new[] { ApplicationExecutionState.ClosedByUser, ApplicationExecutionState.NotRunning, };
            host.IsLaunchingNew = _launched
                .Where(x => launchNew.Contains(x.PreviousExecutionState))
                .Select(_ => Unit.Default);

            host.IsResuming = _launched
                .Where(x => x.PreviousExecutionState == ApplicationExecutionState.Terminated)
                .Select(_ => Unit.Default);

            var unpausing = new[] { ApplicationExecutionState.Suspended, ApplicationExecutionState.Running, };
            host.IsUnpausing = _launched
                .Where(x => unpausing.Contains(x.PreviousExecutionState))
                .Select(_ => Unit.Default);

            host.ShouldPersistState =
                Observable.FromEvent<SuspendingEventHandler, SuspendingEventArgs>(x => Suspending += x, x => Suspending -= x)
                    .Select(x => {
                        var deferral = x.SuspendingOperation.GetDeferral();
                        return Disposable.Create(deferral.Complete);
                    });

            host.ShouldInvalidateState =
                Observable.FromEvent<UnhandledExceptionEventHandler, UnhandledExceptionEventArgs>(x => UnhandledException += x, x => UnhandledException -= x)
                    .Select(_ => Unit.Default);

            SuspensionHost = host;
            RxApp.Register(typeof(WinRTSuspensionHost), typeof(ISuspensionHost));
        }

        protected void SetupDefaultSuspendResume()
        {
            var driver = RxApp.GetService<ISuspensionDriver>();

            SuspensionHost.ShouldInvalidateState
                .SelectMany(_ => driver.InvalidateState())
                .LoggedCatch(this, Observable.Return(Unit.Default), "Tried to invalidate app state")
                .Subscribe(_ => this.Log().Info("Invalidated app state"));

            SuspensionHost.ShouldPersistState
                .SelectMany(x => driver.SaveState(ViewModel).Finally(x.Dispose))
                .LoggedCatch(this, Observable.Return(Unit.Default), "Tried to persist app state")
                .Subscribe(_ => this.Log().Info("Persisted application state"));

            SuspensionHost.IsResuming
                .SelectMany(x => driver.LoadState<IApplicationRootState>())
                .LoggedCatch(this,
                    Observable.Defer(() => Observable.Return(RxApp.GetService<IApplicationRootState>())),
                    "Failed to restore app state from storage, creating from scratch")
                .ObserveOn(RxApp.DeferredScheduler)
                .Subscribe(x => ViewModel = x);

            SuspensionHost.IsLaunchingNew.Subscribe(_ => ViewModel = RxApp.GetService<IApplicationRootState>());

            _viewModelChanged.Subscribe(vm => {
                var page = default(IViewFor);
                var frame = Window.Current.Content as Frame;

                if (frame == null) {
                    page = RxApp.GetService<IViewFor>("InitialPage");

                    frame = new Frame() {
                        Content = page,
                    };

                    Window.Current.Content = frame;
                }

                page.ViewModel = vm;
                Window.Current.Activate();
            });
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            _launched.OnNext(args);
        }
    }
}