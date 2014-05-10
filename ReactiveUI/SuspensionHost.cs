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
}