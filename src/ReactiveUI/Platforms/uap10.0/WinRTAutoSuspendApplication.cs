using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Splat;

namespace ReactiveUI
{
    public class AutoSuspendHelper : IEnableLogger
    {
        readonly ReplaySubject<IActivatedEventArgs> _activated = new ReplaySubject<IActivatedEventArgs>(1);

        public AutoSuspendHelper(Application app)
        {
            Reflection.ThrowIfMethodsNotOverloaded("AutoSuspendHelper", app, "OnLaunched");

            var launchNew = new[] { ApplicationExecutionState.ClosedByUser, ApplicationExecutionState.NotRunning, };
            RxApp.SuspensionHost.IsLaunchingNew = _activated
                .Where(x => launchNew.Contains(x.PreviousExecutionState))
                .Select(_ => Unit.Default);

            RxApp.SuspensionHost.IsResuming = _activated
                .Where(x => x.PreviousExecutionState == ApplicationExecutionState.Terminated)
                .Select(_ => Unit.Default);

            var unpausing = new[] { ApplicationExecutionState.Suspended, ApplicationExecutionState.Running, };
            RxApp.SuspensionHost.IsUnpausing = _activated
                .Where(x => unpausing.Contains(x.PreviousExecutionState))
                .Select(_ => Unit.Default);

            var shouldPersistState = new Subject<SuspendingEventArgs>();
            app.Suspending += (o, e) => shouldPersistState.OnNext(e);
            RxApp.SuspensionHost.ShouldPersistState =
                shouldPersistState.Select(x => {
                    var deferral = x.SuspendingOperation.GetDeferral();
                    return Disposable.Create(deferral.Complete);
                });

            var shouldInvalidateState = new Subject<Unit>();
            app.UnhandledException += (o, e) => shouldInvalidateState.OnNext(Unit.Default);
            RxApp.SuspensionHost.ShouldInvalidateState = shouldInvalidateState;
        }

        public void OnLaunched(IActivatedEventArgs args)
        {
            _activated.OnNext(args);
        }
    }
}