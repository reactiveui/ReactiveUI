using System;
using System.Reactive;
using System.Reactive.Linq;
using Splat;
using System.Reactive.Disposables;

namespace ReactiveUI.Mobile
{
    public class SuspensionHost : ReactiveObject, ISuspensionHost
    {
        public IObservable<Unit> IsLaunchingNew { get; set; }
        public IObservable<Unit> IsResuming { get; set; }
        public IObservable<Unit> IsUnpausing { get; set; }
        public IObservable<IDisposable> ShouldPersistState { get; set; }
        public IObservable<Unit> ShouldInvalidateState { get; set; }
        public Action<ISuspensionDriver> SetupDefaultSuspendResumeFunc { get; set; }

        public SuspensionHost()
        {
#if UIKIT
            var message = "Your AppDelegate class needs to derive from AutoSuspendAppDelegate";
#elif ANDROID
            var message = "Your Activities need to instantiate AutoSuspendActivityHelper";
#else
            var message = "Your App class needs to derive from AutoSuspendApplication";
#endif

            IsLaunchingNew = IsResuming = IsUnpausing = ShouldInvalidateState =
                Observable.Throw<Unit>(new Exception(message));
            ShouldPersistState = Observable.Throw<IDisposable>(new Exception(message));
        }

        public void SetupDefaultSuspendResume(ISuspensionDriver driver = null)
        {
            SetupDefaultSuspendResumeFunc(driver);
        }
    }

    public static class SuspensionHostExtensions
    {
        public static IDisposable SetupDefaultSuspendResume(this ISuspensionHost This, ISuspensionDriver driver = null)
        {
            var ret = new CompositeDisposable();
            driver = driver ?? Locator.Current.GetService<ISuspensionDriver>();

            ret.Add(This.ShouldInvalidateState
                .SelectMany(_ => driver.InvalidateState())
                .LoggedCatch(this, Observable.Return(Unit.Default), "Tried to invalidate app state")
                .Subscribe(_ => This.Log().Info("Invalidated app state")));

            ret.Add(This.ShouldPersistState
                .SelectMany(x => driver.SaveState(viewModel).Finally(x.Dispose))
                .LoggedCatch(this, Observable.Return(Unit.Default), "Tried to persist app state")
                .Subscribe(_ => This.Log().Info("Persisted application state")));

            ret.Add(This.IsResuming
                .SelectMany(x => driver.LoadState<IApplicationRootState>())
                .LoggedCatch(this,
                    Observable.Defer(() => Observable.Return(Locator.Current.GetService<IApplicationRootState>())),
                    "Failed to restore app state from storage, creating from scratch")
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => viewModel = x));

            ret.Add(This.IsLaunchingNew.Subscribe(_ => {
                viewModel = Locator.Current.GetService<IApplicationRootState>();
            }));
        }
    }

    public class DummySuspensionHost : ISuspensionHost
    {
        public void SetupDefaultSuspendResume(ISuspensionDriver driver = null) { }

        public IObservable<Unit> IsLaunchingNew {
            get { return Observable.Return(Unit.Default); }
        }

        public IObservable<Unit> IsResuming {
            get { return Observable.Never<Unit>(); }
        }

        public IObservable<Unit> IsUnpausing {
            get { return Observable.Never<Unit>(); }
        }

        public IObservable<IDisposable> ShouldPersistState {
            get { return Observable.Never<IDisposable>(); }
        }

        public IObservable<Unit> ShouldInvalidateState {
            get { return Observable.Never<Unit>(); }
        }
    }

    public class DummySuspensionDriver : ISuspensionDriver
    {
        public IObservable<T> LoadState<T>() where T : class, IApplicationRootState
        {
            return Observable.Return(Activator.CreateInstance<T>());
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