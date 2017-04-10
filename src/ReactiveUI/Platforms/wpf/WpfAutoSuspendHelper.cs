using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Splat;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace ReactiveUI
{
    public class AutoSuspendHelper : IEnableLogger
    {
        public TimeSpan IdleTimeout { get; set; }

        public AutoSuspendHelper(Application app)
        {
            IdleTimeout = TimeSpan.FromSeconds(15.0);

            RxApp.SuspensionHost.IsLaunchingNew =
                Observable.FromEventPattern<StartupEventHandler, StartupEventArgs>(
                    x => app.Startup += x, x => app.Startup -= x)
                    .Select(_ => Unit.Default);

            RxApp.SuspensionHost.IsUnpausing =
                Observable.FromEventPattern<EventHandler, EventArgs>(
                    x => app.Activated += x, x => app.Activated -= x)
                    .Select(_ => Unit.Default);

            RxApp.SuspensionHost.IsResuming = Observable<Unit>.Never;

            // NB: No way to tell OS that we need time to suspend, we have to
            // do it in-process
            var deactivated = Observable.FromEventPattern<EventHandler, EventArgs>(
                x => app.Deactivated += x, x => app.Deactivated -= x);

            var exit = Observable.FromEventPattern<ExitEventHandler, ExitEventArgs>(
                x => app.Exit += x, x => app.Exit -= x);

            RxApp.SuspensionHost.ShouldPersistState = Observable.Merge(
                exit.Select(_ => Disposable.Empty),
                deactivated
                    .SelectMany(_ => Observable.Timer(IdleTimeout, RxApp.TaskpoolScheduler))
                    .TakeUntil(RxApp.SuspensionHost.IsUnpausing)
                    .Repeat()
                    .Select(_ => Disposable.Empty)
                );

            var untimelyDeath = new Subject<Unit>();
            AppDomain.CurrentDomain.UnhandledException += (o,e) => untimelyDeath.OnNext(Unit.Default);
            RxApp.SuspensionHost.ShouldInvalidateState = untimelyDeath;
        }
    }
}
