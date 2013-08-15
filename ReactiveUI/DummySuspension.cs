using System;
using ReactiveUI.Mobile;
using System.Reactive.Linq;
using System.Reactive;

namespace ReactiveUI.Mobile
{
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