using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Reactive.Subjects;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using Splat;

namespace ReactiveUI.Mobile
{
    class CocoaSuspensionHost : ISuspensionHost
    {
        public IObservable<Unit> IsLaunchingNew { get { return ((AutoSuspendAppDelegate)UIApplication.SharedApplication.Delegate).SuspensionHost.IsLaunchingNew; } }
        public IObservable<Unit> IsResuming { get { return ((AutoSuspendAppDelegate)UIApplication.SharedApplication.Delegate).SuspensionHost.IsResuming; } }
        public IObservable<Unit> IsUnpausing { get { return ((AutoSuspendAppDelegate)UIApplication.SharedApplication.Delegate).SuspensionHost.IsUnpausing; } }
        public IObservable<IDisposable> ShouldPersistState { get { return ((AutoSuspendAppDelegate)UIApplication.SharedApplication.Delegate).SuspensionHost.ShouldPersistState; } }
        public IObservable<Unit> ShouldInvalidateState { get { return ((AutoSuspendAppDelegate)UIApplication.SharedApplication.Delegate).SuspensionHost.ShouldInvalidateState; } }

        public void SetupDefaultSuspendResume(ISuspensionDriver driver = null)
        {
            var app = (AutoSuspendAppDelegate) UIApplication.SharedApplication.Delegate;
            app.setupDefaultSuspendResume(driver);
        }
    }

    /// <summary>
    /// AutoSuspend-based App Delegate. To use AutoSuspend with iOS, change your
    /// AppDelegate to inherit from this class, then call:
    /// 
    /// Locator.Current.GetService<ISuspensionHost>().SetupDefaultSuspendResume();
    /// </summary>
    public abstract class AutoSuspendAppDelegate : UIApplicationDelegate, IEnableLogger
    {
        readonly Subject<UIApplication> _finishedLaunching = new Subject<UIApplication>();
        readonly Subject<UIApplication> _activated = new Subject<UIApplication>();
        readonly Subject<UIApplication> _backgrounded = new Subject<UIApplication>();
        readonly Subject<UIApplication> _willTerminate = new Subject<UIApplication>();

        internal SuspensionHost SuspensionHost;
        protected IApplicationRootState viewModel { get ; set;}

        public IDictionary<string, string> LaunchOptions { get; protected set; }

        public AutoSuspendAppDelegate()
        {
            var host = new SuspensionHost();
            host.IsLaunchingNew = Observable.Never<Unit>();
            host.IsResuming = _finishedLaunching.Select(_ => Unit.Default);
            host.IsUnpausing = _activated.Select(_ => Unit.Default);

            var untimelyDeath = new Subject<Unit>();
            AppDomain.CurrentDomain.UnhandledException += (o,e) => untimelyDeath.OnNext(Unit.Default);

            host.ShouldInvalidateState = untimelyDeath;
            host.ShouldPersistState = _willTerminate.Merge(_backgrounded).SelectMany(app => {
                var taskId = app.BeginBackgroundTask(new NSAction(() => untimelyDeath.OnNext(Unit.Default)));

                // NB: We're being force-killed, signal invalidate instead
                if (taskId == UIApplication.BackgroundTaskInvalid) {
                    untimelyDeath.OnNext(Unit.Default);
                    return Observable.Empty<IDisposable>();
                }

                return Observable.Return(
                    Disposable.Create(() => app.EndBackgroundTask(taskId)));
            });

            SuspensionHost = host;

            Locator.RegisterResolverCallbackChanged(() => {
                if (Locator.CurrentMutable == null) return;
                Locator.CurrentMutable.Register(() => this.viewModel, typeof(IApplicationRootState), "CurrentState");
            });
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            if (launchOptions != null) {
                LaunchOptions = launchOptions.Keys.ToDictionary(k => k.ToString(), v => launchOptions[v].ToString());
            } else {
                LaunchOptions = new Dictionary<string, string>();
            }

            // NB: This is run in-context (i.e. not scheduled), so by the time this
            // statement returns, UIWindow should be created already
            _finishedLaunching.OnNext(application);

            return true;
        }

        public override void OnActivated(UIApplication application)
        {
            _activated.OnNext(application);
        }

        public override void DidEnterBackground(UIApplication application)
        {
            _backgrounded.OnNext(application);
        }

        public override void WillTerminate(UIApplication application)
        {
            _willTerminate.OnNext(application);
        }

        internal void setupDefaultSuspendResume(ISuspensionDriver driver)
        {
            driver = driver ?? Locator.Current.GetService<ISuspensionDriver>();

            SuspensionHost.ShouldInvalidateState
                .SelectMany(_ => driver.InvalidateState())
                .LoggedCatch(this, Observable.Return(Unit.Default), "Tried to invalidate app state")
                .Subscribe(_ => this.Log().Info("Invalidated app state"));

            SuspensionHost.ShouldPersistState
                .SelectMany(x => driver.SaveState(viewModel).Finally(x.Dispose))
                .LoggedCatch(this, Observable.Return(Unit.Default), "Tried to persist app state")
                .Subscribe(_ => this.Log().Info("Persisted application state"));

            SuspensionHost.IsResuming
                .SelectMany(x => driver.LoadState<IApplicationRootState>())
                .LoggedCatch(this,
                    Observable.Defer(() => Observable.Return(Locator.Current.GetService<IApplicationRootState>())),
                    "Failed to restore app state from storage, creating from scratch")
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => viewModel = x);

            SuspensionHost.IsLaunchingNew.Subscribe(_ => {
                viewModel = Locator.Current.GetService<IApplicationRootState>();
            });
        }
    }
}

