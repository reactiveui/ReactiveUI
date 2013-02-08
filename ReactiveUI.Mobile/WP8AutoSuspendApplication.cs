using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ReactiveUI.Mobile
{
    public class WP8SuspensionHost : ISuspensionHost
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
        internal static SuspensionHost SuspensionHost;

        readonly Subject<IApplicationRootState> _viewModelChanged = new Subject<IApplicationRootState>();
        IApplicationRootState _ViewModel;

        public IApplicationRootState ViewModel {
            get { return _ViewModel; }
            set { _ViewModel = value; _viewModelChanged.OnNext(value); }
        }

        protected AutoSuspendApplication()
        {
            var host = new SuspensionHost();

            host.IsLaunchingNew =
                Observable.FromEventPattern<LaunchingEventArgs>(
                    x => PhoneApplicationService.Current.Launching += x, x => PhoneApplicationService.Current.Launching -= x)
                    .Select(_ => Unit.Default);

            host.IsUnpausing =
                Observable.FromEventPattern<ActivatedEventArgs>(
                    x => PhoneApplicationService.Current.Activated += x, x => PhoneApplicationService.Current.Activated -= x)
                    .Where(x => x.EventArgs.IsApplicationInstancePreserved)
                    .Select(_ => Unit.Default);

            // NB: "Applications should not perform resource-intensive tasks 
            // such as loading from isolated storage or a network resource 
            // during the Activated event handler because it increase the time 
            // it takes for the application to resume"
            host.IsResuming =
                Observable.FromEventPattern<ActivatedEventArgs>(
                    x => PhoneApplicationService.Current.Activated += x, x => PhoneApplicationService.Current.Activated -= x)
                    .Where(x => !x.EventArgs.IsApplicationInstancePreserved)
                    .Select(_ => Unit.Default)
                    .ObserveOn(RxApp.TaskpoolScheduler);

            // NB: No way to tell OS that we need time to suspend, we have to
            // do it in-process
            host.ShouldPersistState = Observable.Merge(
                Observable.FromEventPattern<DeactivatedEventArgs>(
                    x => PhoneApplicationService.Current.Deactivated += x, x => PhoneApplicationService.Current.Deactivated -= x)
                    .Select(_ => Disposable.Empty),
                Observable.FromEventPattern<ClosingEventArgs>(
                    x => PhoneApplicationService.Current.Closing += x, x => PhoneApplicationService.Current.Closing -= x)
                    .Select(_ => Disposable.Empty));

            host.ShouldInvalidateState =
                Observable.FromEventPattern<ApplicationUnhandledExceptionEventArgs>(x => UnhandledException += x, x => UnhandledException -= x)
                    .Select(_ => Unit.Default);

            SuspensionHost = host;
        }

        internal void setupDefaultSuspendResume(ISuspensionDriver driver)
        {
            driver = driver ?? RxApp.GetService<ISuspensionDriver>();

            _viewModelChanged.Subscribe(vm => {
                var page = default(IViewFor);
                var frame = RootVisual as PhoneApplicationFrame;

                if (frame == null) {
                    page = RxApp.GetService<IViewFor>("InitialPage");

                    frame = new PhoneApplicationFrame() {
                        Content = page,
                    };

                    RootVisual = frame;
                }

                page.ViewModel = vm;
                var pg = page as PhoneApplicationPage;
                if (pg != null) {
                    pg.BackKeyPress += (o, e) => {
                        if (ViewModel.Router.NavigationStack.Count <= 1 ||
                            ViewModel.Router.NavigateBack.CanExecute(null)) {
                            return;
                        }

                        e.Cancel = true;
                        ViewModel.Router.NavigateBack.Execute(null);
                    };
                }
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
                    Observable.Defer(() => Observable.Return(RxApp.GetService<IApplicationRootState>())),
                    "Failed to restore app state from storage, creating from scratch")
                .ObserveOn(RxApp.DeferredScheduler)
                .Subscribe(x => {
                    ViewModel = x;
                });

            SuspensionHost.IsLaunchingNew.Subscribe(_ => {
                ViewModel = RxApp.GetService<IApplicationRootState>();
            });
        }
    }
}