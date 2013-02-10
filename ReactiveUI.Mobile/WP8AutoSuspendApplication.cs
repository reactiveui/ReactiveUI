using System;
using System.ComponentModel;
using System.Diagnostics;
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
    class WP8SuspensionHost : ISuspensionHost
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
            set {
                if (_ViewModel == value) return;
                _ViewModel = value; 
                _viewModelChanged.OnNext(value);
            }
        }

        public static PhoneApplicationFrame RootFrame { get; protected set; }

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

            //
            // Do the equivalent steps that the boilerplate code does for WP8 apps
            //

            if (RootFrame != null) return;
            RootFrame = new PhoneApplicationFrame();

            var currentBackHook = default(IDisposable);
            var currentViewFor = default(WeakReference<IViewFor>);

            RootFrame.Navigated += (o, e) => {
                // Always clear the WP8 Back Stack, we're using our own
                while (RootFrame.RemoveBackEntry() != null) {}

                if (currentBackHook != null) currentBackHook.Dispose();
                var page = RootFrame.Content as PhoneApplicationPage;
                if (page != null) {
                    currentBackHook = Observable.FromEventPattern<CancelEventArgs>(x => page.BackKeyPress += x, x => page.BackKeyPress -= x)
                        .Where(_ => ViewModel != null)
                        .Subscribe(x => {
                            if (!ViewModel.Router.NavigateBack.CanExecute(null)) return;

                            x.EventArgs.Cancel = true;
                            ViewModel.Router.NavigateBack.Execute(null);
                        });

                    var viewFor = page as IViewFor;
                    if (viewFor == null) {
                        throw new Exception("Your Main Page (i.e. the one that is pointed to by WMAppManifest) must implement IViewFor<YourAppBootstrapperClass>");
                    }

                    currentViewFor = new WeakReference<IViewFor>(viewFor);
                    viewFor.ViewModel = ViewModel;
                }

                // Finally make it live
                RootVisual = RootFrame;
            };

            _viewModelChanged.StartWith(ViewModel).Where(x => x != null).Subscribe(vm => {
                var viewFor = default(IViewFor);
                if (currentViewFor.TryGetTarget(out viewFor)) {
                    viewFor.ViewModel = vm;
                }
            });

            UnhandledException += (o, e) => {
                if (Debugger.IsAttached) Debugger.Break();
            };

            RootFrame.NavigationFailed += (o, e) => {
                if (Debugger.IsAttached) Debugger.Break();
            };
        }

        internal void setupDefaultSuspendResume(ISuspensionDriver driver)
        {
            driver = driver ?? RxApp.GetService<ISuspensionDriver>();

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

            SuspensionHost.IsLaunchingNew.Subscribe(_ => {
                ViewModel = RxApp.GetService<IApplicationRootState>();
            });
        }
    }
}