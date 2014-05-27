using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.App;
using Android.OS;
using System.Reflection;
using Splat;
using System.Reactive.Disposables;

namespace ReactiveUI.Mobile
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

        public AutoSuspendHelper(Activity hostActivity)
        {
            Reflection.ThrowIfMethodsNotOverloaded("AutoSuspendHelper", hostActivity,
                "OnRestart", "OnSaveInstanceState", "OnCreate");

            Observable.Merge(onCreate, onSaveInstanceState).Subscribe(x => LatestBundle = x);

            RxApp.SuspensionHost.IsLaunchingNew = onCreate.Where(x => x == null).Select(_ => Unit.Default);
            RxApp.SuspensionHost.IsResuming = onCreate.Where(x => x != null).Select(_ => Unit.Default);
            RxApp.SuspensionHost.IsUnpausing = onRestart;

            RxApp.SuspensionHost.ShouldPersistState = Observable.Merge(
                onPause.Select(_ => Disposable.Empty), onSaveInstanceState.Select(_ => Disposable.Empty));

            RxApp.SuspensionHost.ShouldInvalidateState = untimelyDemise;
        }

        public void OnCreate(Bundle bundle)
        {
            onCreate.OnNext(bundle);
        }

        public void OnRestart()
        {
            onRestart.OnNext(Unit.Default);
        }

        public void OnSaveInstanceState(Bundle outState)
        {
            // NB: This is so that we always have a bundle on OnCreate, so that
            // we can tell the difference between created from scratch and resume.
            outState.PutString("___dummy_value_please_create_a_bundle", "VeryYes");
            onSaveInstanceState.OnNext(outState);
        }
    }
}