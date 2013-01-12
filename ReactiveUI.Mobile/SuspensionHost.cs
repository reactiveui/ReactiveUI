using System;
using System.Reactive;

namespace ReactiveUI.Mobile
{
    public class SuspensionHost : ISuspensionHost
    {
        public IObservable<Unit> IsLaunchingNew { get; set; }
        public IObservable<Unit> IsResuming { get; set; }
        public IObservable<Unit> IsUnpausing { get; set; }
        public IObservable<Unit> ShouldPersistState { get; set; }
        public IObservable<Unit> ShouldInvalidateState { get; set; }
    }
}