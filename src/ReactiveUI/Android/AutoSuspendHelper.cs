using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Java.Lang;
using Android.App;
using Android.OS;
using System.Reflection;
using Splat;
using System.Reactive.Disposables;

namespace ReactiveUI
{
    public class AutoSuspendHelper : IEnableLogger
    {
        readonly Subject<Bundle> onCreate = new Subject<Bundle>();
        readonly Subject<Unit> onRestart = new Subject<Unit>();
        readonly Subject<Unit> onPause = new Subject<Unit>();
        readonly Subject<Bundle> onSaveInstanceState = new Subject<Bundle>();

        public static Bundle LatestBundle { get; set; }

        public static readonly Subject<Unit> untimelyDemise = new Subject<Unit>();

        static AutoSuspendHelper()
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) => untimelyDemise.OnNext(Unit.Default);
        }

        public AutoSuspendHelper(Application hostApplication)
        {
            hostApplication.RegisterActivityLifecycleCallbacks(new ObservableLifecycle(this));

            Observable.Merge(onCreate, onSaveInstanceState).Subscribe(x => LatestBundle = x);

            RxApp.SuspensionHost.IsLaunchingNew = onCreate.Where(x => x == null).Select(_ => Unit.Default);
            RxApp.SuspensionHost.IsResuming = onCreate.Where(x => x != null).Select(_ => Unit.Default);
            RxApp.SuspensionHost.IsUnpausing = onRestart;

            RxApp.SuspensionHost.ShouldPersistState = Observable.Merge(
                onPause.Select(_ => Disposable.Empty), onSaveInstanceState.Select(_ => Disposable.Empty));

            RxApp.SuspensionHost.ShouldInvalidateState = untimelyDemise;
        }
            
        class ObservableLifecycle : Java.Lang.Object, Application.IActivityLifecycleCallbacks
        {
            readonly AutoSuspendHelper This;
            public ObservableLifecycle(AutoSuspendHelper This)
            {
                this.This = This;
            }

            public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
            {
                This.onCreate.OnNext(savedInstanceState);
            }

            public void OnActivityResumed(Activity activity)
            {
                This.onRestart.OnNext(Unit.Default);
            }

            public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
            {
                // NB: This is so that we always have a bundle on OnCreate, so that
                // we can tell the difference between created from scratch and resume.
                outState.PutString("___dummy_value_please_create_a_bundle", "VeryYes");
                This.onSaveInstanceState.OnNext(outState);
            }
                        
            public void OnActivityPaused(Activity activity) 
            { 
                This.onPause.OnNext(Unit.Default);
            }

            public void OnActivityDestroyed(Activity activity) { }
            public void OnActivityStarted(Activity activity) { }
            public void OnActivityStopped(Activity activity) { }
        }
    }
}