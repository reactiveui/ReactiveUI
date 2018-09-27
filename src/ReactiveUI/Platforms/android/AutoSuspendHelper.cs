// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Android.App;
using Android.OS;
using Java.Lang;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Helps manage android application lifecycle events.
    /// </summary>
    public class AutoSuspendHelper : IEnableLogger
    {
        private readonly Subject<Bundle> _onCreate = new Subject<Bundle>();
        private readonly Subject<Unit> _onRestart = new Subject<Unit>();
        private readonly Subject<Unit> _onPause = new Subject<Unit>();
        private readonly Subject<Bundle> _onSaveInstanceState = new Subject<Bundle>();

        /// <summary>
        /// The untimely demise of an application.
        /// </summary>
        public static readonly Subject<Unit> UntimelyDemise = new Subject<Unit>();

        /// <summary>
        /// Gets or sets the latest bundle.
        /// </summary>
        /// <value>
        /// The latest bundle.
        /// </value>
        public static Bundle LatestBundle { get; set; }

        /// <summary>
        /// Initializes the <see cref="AutoSuspendHelper"/> class.
        /// </summary>
        static AutoSuspendHelper()
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) => UntimelyDemise.OnNext(Unit.Default);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoSuspendHelper"/> class.
        /// </summary>
        /// <param name="hostApplication">The host application.</param>
        public AutoSuspendHelper(Application hostApplication)
        {
            hostApplication.RegisterActivityLifecycleCallbacks(new ObservableLifecycle(this));

            Observable.Merge(_onCreate, _onSaveInstanceState).Subscribe(x => LatestBundle = x);

            RxApp.SuspensionHost.IsLaunchingNew = _onCreate.Where(x => x == null).Select(_ => Unit.Default);
            RxApp.SuspensionHost.IsResuming = _onCreate.Where(x => x != null).Select(_ => Unit.Default);
            RxApp.SuspensionHost.IsUnpausing = _onRestart;

            RxApp.SuspensionHost.ShouldPersistState = Observable.Merge(
                _onPause.Select(_ => Disposable.Empty), _onSaveInstanceState.Select(_ => Disposable.Empty));

            RxApp.SuspensionHost.ShouldInvalidateState = UntimelyDemise;
        }

        private class ObservableLifecycle : Java.Lang.Object, Application.IActivityLifecycleCallbacks
        {
            private readonly AutoSuspendHelper _this;

            public ObservableLifecycle(AutoSuspendHelper @this)
            {
                _this = @this;
            }

            public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
            {
                _this._onCreate.OnNext(savedInstanceState);
            }

            public void OnActivityResumed(Activity activity)
            {
                _this._onRestart.OnNext(Unit.Default);
            }

            public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
            {
                // NB: This is so that we always have a bundle on OnCreate, so that
                // we can tell the difference between created from scratch and resume.
                outState.PutString("___dummy_value_please_create_a_bundle", "VeryYes");
                _this._onSaveInstanceState.OnNext(outState);
            }

            public void OnActivityPaused(Activity activity)
            {
                _this._onPause.OnNext(Unit.Default);
            }

            public void OnActivityDestroyed(Activity activity)
            {
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
