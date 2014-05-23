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

        public Func<object> CreateNewAppState { get; set; }

        object appState;
        public object AppState {
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
    }

    public static class SuspensionHostExtensions
    {
        public static IObservable<T> ObserveAppState<T>(this ISuspensionHost This)
        {
            return This.WhenAny(x => x.AppState, x => (T)x.Value);
        }

        public static T GetAppState<T>(this ISuspensionHost This)
        {
            return (T)This.AppState;
        }
                
        public static IDisposable SetupDefaultSuspendResume(this ISuspensionHost This, ISuspensionDriver driver = null)
        {
            var ret = new CompositeDisposable();
            driver = driver ?? Locator.Current.GetService<ISuspensionDriver>();

            ret.Add(This.ShouldInvalidateState
                .SelectMany(_ => driver.InvalidateState())
                .LoggedCatch(This, Observable.Return(Unit.Default), "Tried to invalidate app state")
                .Subscribe(_ => This.Log().Info("Invalidated app state")));

            ret.Add(This.ShouldPersistState
                .SelectMany(x => driver.SaveState(This.AppState).Finally(x.Dispose))
                .LoggedCatch(This, Observable.Return(Unit.Default), "Tried to persist app state")
                .Subscribe(_ => This.Log().Info("Persisted application state")));

            ret.Add(This.IsResuming
                .SelectMany(x => driver.LoadState())
                .LoggedCatch(This,
                    Observable.Defer(() => Observable.Return(This.CreateNewAppState())),
                    "Failed to restore app state from storage, creating from scratch")
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => This.AppState = x));

            ret.Add(This.IsLaunchingNew.Subscribe(_ => {
                This.AppState = This.CreateNewAppState();
            }));

            return ret;
        }
    }

    public class DummySuspensionDriver : ISuspensionDriver
    {
        public IObservable<object> LoadState()
        {
            return Observable.Return(default(object));
        }

        public IObservable<Unit> SaveState(object state)
        {
            return Observable.Return(Unit.Default);
        }

        public IObservable<Unit> InvalidateState()
        {
            return Observable.Return(Unit.Default);
        }
    }
}
