using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.App;
using Android.OS;
using System.Reflection;

namespace ReactiveUI.Mobile
{
    class AndroidSuspensionHost : ISuspensionHost
    {
        internal static ISuspensionHost inner { get; set; }
        internal static Bundle latestBundle { get; set; }

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

    public class RootState : ReactiveObject
    {
        static RootState _Current = new RootState();
        public static RootState Current { get { return _Current; } }

        protected RootState() { }

        IApplicationRootState _ViewModel;
        public IApplicationRootState ViewModel {
            get { return _ViewModel; }
            set { this.RaiseAndSetIfChanged(ref _ViewModel, value); }
        }

        public static T As<T>() where T : IApplicationRootState
        {
            return (T)Current.ViewModel;
        }
    }

    public class AutoSuspendActivityHelper : IEnableLogger
    {
        readonly Subject<Bundle> onCreate = new Subject<Bundle>();
        readonly Subject<Unit> onResume = new Subject<Unit>();
        readonly Subject<Unit> onPause = new Subject<Unit>();
        readonly Subject<Bundle> onSaveInstanceState = new Subject<Bundle>();

        public ISuspensionHost SuspensionHost { get; set; }

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

            Observable.Merge(onCreate, onSaveInstanceState)
                .Subscribe(x => AndroidSuspensionHost.latestBundle = x);

            var host = new SuspensionHost();
            host.IsLaunchingNew = onCreate.Select(_ => Unit.Default);
            host.IsResuming = onResume;
            host.IsUnpausing = onResume;

            bool hasRecentlyCrashed = false;
            host.SetupDefaultSuspendResumeFunc = new Action<ISuspensionDriver>(driver => {
                driver = driver ?? RxApp.GetService<ISuspensionDriver>();

                // TODO: Handle crashes here

                host.ShouldInvalidateState
                    .SelectMany(_ => driver.InvalidateState())
                    .LoggedCatch(this, Observable.Return(Unit.Default), "Tried to invalidate app state")
                    .Subscribe(_ => this.Log().Info("Invalidated app state"));

                host.ShouldPersistState
                    .SelectMany(x => driver.SaveState(RootState.Current.ViewModel).Finally(x.Dispose))
                    .LoggedCatch(this, Observable.Return(Unit.Default), "Tried to persist app state")
                    .Subscribe(_ => this.Log().Info("Persisted application state"));

                host.IsResuming
                    .SelectMany(x => driver.LoadState<IApplicationRootState>())
                    .LoggedCatch(this,
                        Observable.Defer(() => Observable.Return(RxApp.GetService<IApplicationRootState>())),
                        "Failed to restore app state from storage, creating from scratch")
                    .ObserveOn(RxApp.DeferredScheduler)
                    .Subscribe(x => RootState.Current.ViewModel = x);

                host.IsLaunchingNew.Subscribe(_ => {
                    RootState.Current.ViewModel = RxApp.GetService<IApplicationRootState>();
                });
            });

            SuspensionHost = host;
            AndroidSuspensionHost.inner = host;
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

