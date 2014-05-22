using System;
using System.Reactive;
using System.Reactive.Linq;
using Splat;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace ReactiveUI.Mobile
{
    internal class SuspensionHost : ReactiveObject, ISuspensionHost
    {
        readonly Subject<Unit> isLaunchingNewProxy = new Subject<Unit>();
        readonly SerialDisposable isLaunchingNewSub = new SerialDisposable();
        public IObservable<Unit> IsLaunchingNew {
            get { return isLaunchingNewProxy; }
            set { isLaunchingNewSub.Disposable = value.Subscribe(isLaunchingNewProxy); }
        }

        readonly Subject<Unit> isResumingProxy = new Subject<Unit>();
        readonly SerialDisposable isResumingSub = new SerialDisposable();
        public IObservable<Unit> IsResuming {
            get { return isResumingProxy; }
            set { isResumingSub.Disposable = value.Subscribe(isResumingProxy); }
        }

        readonly Subject<Unit> isUnpausingProxy = new Subject<Unit>();
        readonly SerialDisposable isUnpausingSub = new SerialDisposable();
        public IObservable<Unit> IsUnpausing {
            get { return isUnpausingProxy; }
            set { isLaunchingNewSub.Disposable = value.Subscribe(isUnpausingProxy); }
        }

        readonly Subject<IDisposable> shouldPersistStateProxy = new Subject<IDisposable>();
        readonly SerialDisposable shouldPersistStateSub = new SerialDisposable();
        public IObservable<IDisposable> ShouldPersistState {
            get { return shouldPersistStateProxy; }
            set { shouldPersistStateSub.Disposable = value.Subscribe(shouldPersistStateProxy); }
        }

        readonly Subject<Unit> shouldInvalidateStateProxy = new Subject<Unit>();
        readonly SerialDisposable shouldInvalidateStateSub = new SerialDisposable();
        public IObservable<Unit> ShouldInvalidateState {
            get { return shouldInvalidateStateProxy; }
            set { shouldInvalidateStateSub.Disposable = value.Subscribe(shouldInvalidateStateProxy); }
        }

        public Func<IApplicationRootState> CreateNewAppState { get; set; }

        IApplicationRootState appState;
        public IApplicationRootState AppState {
            get { return appState; }
            set { this.RaiseAndSetIfChanged(ref appState, value); }
        }

        public SuspensionHost()
        {
#if COCOA
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

        public IObservable<T> ObserveAppState<T>()
        {
            return this.WhenAny(x => x.AppState, x => (T)x.Value);
        }

        public T GetAppState<T>()
        {
            return (T)AppState;
        }
                
        public IDisposable SetupDefaultSuspendResume(ISuspensionDriver driver = null)
        {
            var ret = new CompositeDisposable();
            driver = driver ?? Locator.Current.GetService<ISuspensionDriver>();

            ret.Add(this.ShouldInvalidateState
                .SelectMany(_ => driver.InvalidateState())
                .LoggedCatch(this, Observable.Return(Unit.Default), "Tried to invalidate app state")
                .Subscribe(_ => this.Log().Info("Invalidated app state")));

            ret.Add(this.ShouldPersistState
                .SelectMany(x => driver.SaveState(AppState).Finally(x.Dispose))
                .LoggedCatch(this, Observable.Return(Unit.Default), "Tried to persist app state")
                .Subscribe(_ => this.Log().Info("Persisted application state")));

            ret.Add(this.IsResuming
                .SelectMany(x => driver.LoadState<IApplicationRootState>())
                .LoggedCatch(this,
                    Observable.Defer(() => Observable.Return(CreateNewAppState())),
                    "Failed to restore app state from storage, creating from scratch")
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => AppState = x));

            ret.Add(this.IsLaunchingNew.Subscribe(_ => {
                AppState = CreateNewAppState();
            }));

            return ret;
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
