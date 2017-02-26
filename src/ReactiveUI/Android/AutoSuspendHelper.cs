using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.App;
using Android.OS;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Auto Suspend Helper
    /// </summary>
    /// <seealso cref="Splat.IEnableLogger"/>
    public class AutoSuspendHelper : IEnableLogger
    {
        /// <summary>
        /// The un-timely demise
        /// </summary>
        public static readonly Subject<Unit> untimelyDemise = new Subject<Unit>();

        private readonly Subject<Bundle> onCreate = new Subject<Bundle>();
        private readonly Subject<Unit> onPause = new Subject<Unit>();
        private readonly Subject<Unit> onRestart = new Subject<Unit>();
        private readonly Subject<Bundle> onSaveInstanceState = new Subject<Bundle>();

        static AutoSuspendHelper()
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) => untimelyDemise.OnNext(Unit.Default);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoSuspendHelper"/> class.
        /// </summary>
        /// <param name="hostApplication">The host application.</param>
        public AutoSuspendHelper(Application hostApplication)
        {
            hostApplication.RegisterActivityLifecycleCallbacks(new ObservableLifecycle(this));

            Observable.Merge(this.onCreate, this.onSaveInstanceState).Subscribe(x => LatestBundle = x);

            RxApp.SuspensionHost.IsLaunchingNew = this.onCreate.Where(x => x == null).Select(_ => Unit.Default);
            RxApp.SuspensionHost.IsResuming = this.onCreate.Where(x => x != null).Select(_ => Unit.Default);
            RxApp.SuspensionHost.IsUnpausing = this.onRestart;

            RxApp.SuspensionHost.ShouldPersistState = Observable.Merge(
                this.onPause.Select(_ => Disposable.Empty), this.onSaveInstanceState.Select(_ => Disposable.Empty));

            RxApp.SuspensionHost.ShouldInvalidateState = untimelyDemise;
        }

        /// <summary>
        /// Gets or sets the latest bundle.
        /// </summary>
        /// <value>The latest bundle.</value>
        public static Bundle LatestBundle { get; set; }

        private class ObservableLifecycle : Java.Lang.Object, Application.IActivityLifecycleCallbacks
        {
            private readonly AutoSuspendHelper This;

            public ObservableLifecycle(AutoSuspendHelper This)
            {
                this.This = This;
            }

            public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
            {
                this.This.onCreate.OnNext(savedInstanceState);
            }

            public void OnActivityDestroyed(Activity activity)
            {
            }

            public void OnActivityPaused(Activity activity)
            {
                this.This.onPause.OnNext(Unit.Default);
            }

            public void OnActivityResumed(Activity activity)
            {
                this.This.onRestart.OnNext(Unit.Default);
            }

            public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
            {
                // NB: This is so that we always have a bundle on OnCreate, so that we can tell the
                // difference between created from scratch and resume.
                outState.PutString("___dummy_value_please_create_a_bundle", "VeryYes");
                this.This.onSaveInstanceState.OnNext(outState);
            }

            public void OnActivityStarted(Activity activity)
            {
            }

            public void OnActivityStopped(Activity activity)
            {
            }
        }
    }
}