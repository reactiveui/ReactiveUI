using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.App;
using Android.OS;
using System.Reflection;
using Splat;

namespace ReactiveUI.Mobile
{
    public class AutoSuspendActivityHelper : IEnableLogger
    {
        readonly Subject<Bundle> onCreate = new Subject<Bundle>();
        readonly Subject<Unit> onResume = new Subject<Unit>();
        readonly Subject<Unit> onPause = new Subject<Unit>();
        readonly Subject<Bundle> onSaveInstanceState = new Subject<Bundle>();

        public static Bundle LatestBundle { get; set; }

        public AutoSuspendActivityHelper(Activity hostActivity)
        {
            var methodsToCheck = new[] {
                "OnCreate", "OnResume", "OnPause", "OnSaveInstanceState",
            };

            var missingMethod = methodsToCheck
                .Select(x => {
                    var methods = hostActivity.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    return Tuple.Create(x, methods.FirstOrDefault(y => y.Name == x));
                })
                .FirstOrDefault(x => x.Item2 == null);

            if (missingMethod != null) {
                throw new Exception(String.Format("Your activity must implement {0} and call AutoSuspendActivityHelper.{0}", missingMethod.Item1));
            }

            Observable.Merge(onCreate, onSaveInstanceState).Subscribe(x => LatestBundle = x);

            RxApp.SuspensionHost.IsLaunchingNew = onCreate.Select(_ => Unit.Default);
            RxApp.SuspensionHost.IsResuming = onResume;
            RxApp.SuspensionHost.IsUnpausing = onResume;
        }

        public void OnCreate(Bundle bundle)
        {
            onCreate.OnNext(bundle);
        }

        public void OnResume()
        {
            onResume.OnNext(Unit.Default);
        }

        public void OnPause()
        {
            onResume.OnNext(Unit.Default);
        }

        public void OnSaveInstanceState(Bundle outState)
        {
            onSaveInstanceState.OnNext(outState);
        }
    }
}