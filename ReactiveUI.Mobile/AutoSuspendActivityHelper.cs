using System;
using System.Reactive;
using Android.App;

namespace ReactiveUI.Mobile
{
    class AndroidSuspensionHost : ISuspensionHost
    {
        internal static ISuspensionHost inner { get; set; }

        static AndroidSuspensionHost()
        {
            inner = new SuspensionHost();
        }

        public IObservable<Unit> IsLaunchingNew { get { return inner.IsLaunchingNew; } }
        public IObservable<Unit> IsResuming { get { return inner.IsResuming; } }
        public IObservable<Unit> IsUnpausing { get { return inner.IsUnpausing; } } 
        public IObservable<IDisposable> ShouldPersistState { get { return inner.ShouldPersistState; } }
        public IObservable<Unit> ShouldInvalidateState { get { return inner.ShouldInvalidateState; } }

        public void SetupDefaultSuspendResume(ISuspensionDriver driver = null)
        {
            inner.SetupDefaultSuspendResume(driver);
        }
    }

    public class AutoSuspendActivityHelper
    {
        public AutoSuspendActivityHelper(Activity hostActivity)
        {
        }
    }
}

