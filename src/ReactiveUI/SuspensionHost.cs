using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Splat;

namespace ReactiveUI
{
    internal class SuspensionHost : ReactiveObject, ISuspensionHost
    {
        readonly ReplaySubject<IObservable<Unit>> isLaunchingNew = new ReplaySubject<IObservable<Unit>>(1);
        public IObservable<Unit> IsLaunchingNew
        {
            get { return isLaunchingNew.Switch(); }
            set { isLaunchingNew.OnNext(value); }
        }

        readonly ReplaySubject<IObservable<Unit>> isResuming = new ReplaySubject<IObservable<Unit>>(1);
        public IObservable<Unit> IsResuming
        {
            get { return isResuming.Switch(); }
            set { isResuming.OnNext(value); }
        }

        readonly ReplaySubject<IObservable<Unit>> isUnpausing = new ReplaySubject<IObservable<Unit>>(1);
        public IObservable<Unit> IsUnpausing
        {
            get { return isUnpausing.Switch(); }
            set { isUnpausing.OnNext(value); }
        }

        readonly ReplaySubject<IObservable<IDisposable>> shouldPersistState = new ReplaySubject<IObservable<IDisposable>>(1);
        public IObservable<IDisposable> ShouldPersistState
        {
            get { return shouldPersistState.Switch(); }
            set { shouldPersistState.OnNext(value); }
        }

        readonly ReplaySubject<IObservable<Unit>> shouldInvalidateState = new ReplaySubject<IObservable<Unit>>(1);
        public IObservable<Unit> ShouldInvalidateState
        {
            get { return shouldInvalidateState.Switch(); }
            set { shouldInvalidateState.OnNext(value); }
        }

        /// <summary>
        ///
        /// </summary>
        public Func<object> CreateNewAppState { get; set; }

        object appState;

        /// <summary>
        ///
        /// </summary>
        public object AppState
        {
            get { return appState; }
            set { this.RaiseAndSetIfChanged(ref appState, value); }
        }

        public SuspensionHost()
        {
#if COCOA
            var message = "Your AppDelegate class needs to use AutoSuspendHelper";
#elif ANDROID
            var message = "You need to create an App class and use AutoSuspendHelper";
#else
            var message = "Your App class needs to use AutoSuspendHelper";
#endif

            IsLaunchingNew = IsResuming = IsUnpausing = ShouldInvalidateState =
                Observable.Throw<Unit>(new Exception(message));

            ShouldPersistState = Observable.Throw<IDisposable>(new Exception(message));
        }
    }

    public static class SuspensionHostExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static IObservable<T> ObserveAppState<T>(this ISuspensionHost This)
        {
            return This.WhenAny(x => x.AppState, x => (T)x.Value)
                .Where(x => x != null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static T GetAppState<T>(this ISuspensionHost This)
        {
            return (T)This.AppState;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="This"></param>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static IDisposable SetupDefaultSuspendResume(this ISuspensionHost This, ISuspensionDriver driver = null)
        {
            var ret = new CompositeDisposable();
            driver = driver ?? Locator.Current.GetService<ISuspensionDriver>();

            ret.Add(This.ShouldInvalidateState
                .SelectMany(_ => driver.InvalidateState())
                .LoggedCatch(This, Observables.Unit, "Tried to invalidate app state")
                .Subscribe(_ => This.Log().Info("Invalidated app state")));

            ret.Add(This.ShouldPersistState
                .SelectMany(x => driver.SaveState(This.AppState).Finally(x.Dispose))
                .LoggedCatch(This, Observables.Unit, "Tried to persist app state")
                .Subscribe(_ => This.Log().Info("Persisted application state")));

            ret.Add(Observable.Merge(This.IsResuming, This.IsLaunchingNew)
                .SelectMany(x => driver.LoadState())
                .LoggedCatch(This,
                    Observable.Defer(() => Observable.Return(This.CreateNewAppState())),
                    "Failed to restore app state from storage, creating from scratch")
                .Subscribe(x => This.AppState = x ?? This.CreateNewAppState()));

            return ret;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class DummySuspensionDriver : ISuspensionDriver
    {
        public IObservable<object> LoadState()
        {
            return Observable<object>.Default;
        }

        public IObservable<Unit> SaveState(object state)
        {
            return Observables.Unit;
        }

        public IObservable<Unit> InvalidateState()
        {
            return Observables.Unit;
        }
    }
}
