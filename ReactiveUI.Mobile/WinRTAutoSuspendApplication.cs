﻿using System;
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
        public IObservable<Unit> IsLaunchingNew { get { return AutoSuspendApplication.SuspensionHost.IsLaunchingNew; } }
        public IObservable<Unit> IsResuming { get { return AutoSuspendApplication.SuspensionHost.IsResuming; } }
        public IObservable<Unit> IsUnpausing { get { return AutoSuspendApplication.SuspensionHost.IsUnpausing; } }
        public IObservable<IDisposable> ShouldPersistState { get { return AutoSuspendApplication.SuspensionHost.ShouldPersistState; } }
        public IObservable<Unit> ShouldInvalidateState { get { return AutoSuspendApplication.SuspensionHost.ShouldInvalidateState; } }

        public void SetupDefaultSuspendResume(ISuspensionDriver driver = null)
        {
            var app = (AutoSuspendApplication) Application.Current;
            app.setupDefaultSuspendResume(driver);
        }
    }

    public abstract class AutoSuspendApplication : Application, IEnableLogger
    {
        readonly ReplaySubject<LaunchActivatedEventArgs> _launched = new ReplaySubject<LaunchActivatedEventArgs>();
        internal static SuspensionHost SuspensionHost;

        readonly Subject<IApplicationRootState> _viewModelChanged = new Subject<IApplicationRootState>();
        IApplicationRootState _ViewModel;

        public IApplicationRootState ViewModel {
            get { return _ViewModel; }
            set {
                if (_ViewModel == value) return;
                _ViewModel = value; 
                _viewModelChanged.OnNext(value);
            }
        }

        protected AutoSuspendApplication()
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

            var shouldPersistState = new Subject<SuspendingEventArgs>();
            Suspending += (o, e) => shouldPersistState.OnNext(e);
            host.ShouldPersistState =
                shouldPersistState.Select(x => {
                    var deferral = x.SuspendingOperation.GetDeferral();
                    return Disposable.Create(deferral.Complete);
                });

            var shouldInvalidateState = new Subject<Unit>();
            UnhandledException += (o, e) => shouldInvalidateState.OnNext(Unit.Default);
            host.ShouldInvalidateState = shouldInvalidateState;

            SuspensionHost = host;
        }

        internal void setupDefaultSuspendResume(ISuspensionDriver driver)
        {
            driver = driver ?? RxApp.DependencyResolver.GetService<ISuspensionDriver>();

            _viewModelChanged.Subscribe(vm => {
                var page = default(IViewFor);
                var frame = Window.Current.Content as Frame;

                if (frame == null) {
                    frame = new Frame();
                    Window.Current.Content = frame;
                }

                page = Window.Current.Content as IViewFor;
                if (page == null) {
                    page = RxApp.DependencyResolver.GetService<IViewFor>("InitialPage");
                    frame.Content = (UIElement)page;
                }

                page.ViewModel = vm;
                Window.Current.Activate();
            });

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
                    Observable.Defer(() => Observable.Return(RxApp.DependencyResolver.GetService<IApplicationRootState>())),
                    "Failed to restore app state from storage, creating from scratch")
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => ViewModel = x);

            SuspensionHost.IsLaunchingNew.Subscribe(_ => {
                ViewModel = RxApp.DependencyResolver.GetService<IApplicationRootState>();
            });
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            _launched.OnNext(args);
        }
    }
}