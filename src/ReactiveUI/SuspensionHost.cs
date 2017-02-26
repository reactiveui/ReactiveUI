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
        private readonly ReplaySubject<IObservable<Unit>> isLaunchingNew = new ReplaySubject<IObservable<Unit>>(1);

        public IObservable<Unit> IsLaunchingNew
        {
            get { return this.isLaunchingNew.Switch(); }
            set { this.isLaunchingNew.OnNext(value); }
        }

        private readonly ReplaySubject<IObservable<Unit>> isResuming = new ReplaySubject<IObservable<Unit>>(1);

        public IObservable<Unit> IsResuming
        {
            get { return this.isResuming.Switch(); }
            set { this.isResuming.OnNext(value); }
        }

        private readonly ReplaySubject<IObservable<Unit>> isUnpausing = new ReplaySubject<IObservable<Unit>>(1);

        public IObservable<Unit> IsUnpausing
        {
            get { return this.isUnpausing.Switch(); }
            set { this.isUnpausing.OnNext(value); }
        }

        private readonly ReplaySubject<IObservable<IDisposable>> shouldPersistState = new ReplaySubject<IObservable<IDisposable>>(1);

        public IObservable<IDisposable> ShouldPersistState
        {
            get { return this.shouldPersistState.Switch(); }
            set { this.shouldPersistState.OnNext(value); }
        }

        private readonly ReplaySubject<IObservable<Unit>> shouldInvalidateState = new ReplaySubject<IObservable<Unit>>(1);

        public IObservable<Unit> ShouldInvalidateState
        {
            get { return this.shouldInvalidateState.Switch(); }
            set { this.shouldInvalidateState.OnNext(value); }
        }

        public Func<object> CreateNewAppState { get; set; }

        private object appState;

        public object AppState
        {
            get { return this.appState; }
            set { this.RaiseAndSetIfChanged(ref this.appState, value); }
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

            this.IsLaunchingNew = this.IsResuming = this.IsUnpausing = this.ShouldInvalidateState =
                Observable.Throw<Unit>(new Exception(message));

            this.ShouldPersistState = Observable.Throw<IDisposable>(new Exception(message));
        }
    }

    /// <summary>
    /// Suspension Host Extensions
    /// </summary>
    public static class SuspensionHostExtensions
    {
        /// <summary>
        /// Observes the state of the application.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="This">The this.</param>
        /// <returns></returns>
        public static IObservable<T> ObserveAppState<T>(this ISuspensionHost This)
        {
            return This.WhenAny(x => x.AppState, x => (T)x.Value)
                .Where(x => x != null);
        }

        /// <summary>
        /// Gets the state of the application.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="This">The this.</param>
        /// <returns></returns>
        public static T GetAppState<T>(this ISuspensionHost This)
        {
            return (T)This.AppState;
        }

        /// <summary>
        /// Setups the default suspend resume.
        /// </summary>
        /// <param name="This">The this.</param>
        /// <param name="driver">The driver.</param>
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
    /// Dummy Suspension Driver
    /// </summary>
    public class DummySuspensionDriver : ISuspensionDriver
    {
        /// <summary>
        /// Loads the application state from persistent storage
        /// </summary>
        /// <returns></returns>
        public IObservable<object> LoadState()
        {
            return Observable<object>.Default;
        }

        /// <summary>
        /// Saves the application state to disk.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public IObservable<Unit> SaveState(object state)
        {
            return Observables.Unit;
        }

        /// <summary>
        /// Invalidates the application state (i.e. deletes it from disk)
        /// </summary>
        /// <returns></returns>
        public IObservable<Unit> InvalidateState()
        {
            return Observables.Unit;
        }
    }
}