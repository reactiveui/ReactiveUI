using System;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveUI.Mobile
{
    public class SuspensionHost
    {
        public IObservable<Unit> IsLaunchingNew { get; set; }
        public IObservable<Unit> IsResuming { get; set; }
        public IObservable<Unit> IsUnpausing { get; set; }
        public IObservable<IDisposable> ShouldPersistState { get; set; }
        public IObservable<Unit> ShouldInvalidateState { get; set; }

        public SuspensionHost()
        {
            IsLaunchingNew = IsResuming = IsUnpausing = ShouldInvalidateState =
                Observable.Throw<Unit>(new Exception("Your App class needs to derive from AutoSuspendApplication"));
            ShouldPersistState = Observable.Throw<IDisposable>(new Exception("Your App class needs to derive from AutoSuspendApplication"));
        }
    }
}