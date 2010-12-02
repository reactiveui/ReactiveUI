using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Concurrency;
using System.Windows.Threading;
using System.Diagnostics.Contracts;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace ReactiveXaml
{
    public class ReactiveCommand : IReactiveCommand, IEnableLogger
    {
        public ReactiveCommand(Action<object> executed = null, IScheduler scheduler = null)
            : this((IObservable<bool>)null, executed, scheduler) { }

        public ReactiveCommand(IObservable<bool> can_execute = null, Action<object> executed = null, IScheduler scheduler = null)
        {
            can_execute = can_execute ?? Observable.Return(true).Concat(Observable.Never<bool>());
            commonCtor(executed, scheduler);
            can_execute.Subscribe(canExecuteSubject.OnNext, canExecuteSubject.OnError, canExecuteSubject.OnCompleted);
        }

        public ReactiveCommand(Func<object, bool> can_execute, Action<object> executed = null, IScheduler scheduler = null)
        {
            canExecuteExplicitFunc = can_execute;
            commonCtor(executed, scheduler);
        }

        private void commonCtor(Action<object> executed, IScheduler scheduler)
        {
            this.scheduler = scheduler ?? RxApp.DeferredScheduler;

            canExecuteSubject = new Subject<bool>();
            canExecuteLatest = new ObservableAsPropertyHelper<bool>(canExecuteSubject,
                b => { if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty); },
                true, scheduler);

            executeSubject = new Subject<object>();

            if (executed != null) {
                executeExplicitFunc = executed;
                executeSubject.Subscribe(executed);
            }
        }

        Func<object, bool> canExecuteExplicitFunc;
        protected Subject<bool> canExecuteSubject;

        public IObservable<bool> CanExecuteObservable {
            get { return canExecuteSubject; }
        }

        ObservableAsPropertyHelper<bool> canExecuteLatest;
        public virtual bool CanExecute(object parameter)
        {
            if (canExecuteExplicitFunc != null)
                canExecuteSubject.OnNext(canExecuteExplicitFunc(parameter));
                
            return canExecuteLatest.Value;
        }

        public event EventHandler CanExecuteChanged;

        Action<object> executeExplicitFunc; 
        IScheduler scheduler;
        Subject<object> executeSubject;

        public void Execute(object parameter)
        {
            this.Log().DebugFormat("{0:X}: Executed", this.GetHashCode());
            executeSubject.OnNext(parameter);
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return executeSubject.ObserveOn(scheduler).Subscribe(observer);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :