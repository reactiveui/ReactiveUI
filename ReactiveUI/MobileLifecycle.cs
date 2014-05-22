using System;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveUI.Mobile
{
    public class SuspensionHost : ISuspensionHost
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