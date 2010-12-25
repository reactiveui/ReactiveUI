using System;
using System.Collections.Generic;
using System.Linq;
using System.Concurrency;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace ReactiveXaml
{
    /// <summary>
    /// 
    /// </summary>
    public class ReactiveCommand : IReactiveCommand, IEnableLogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canExecute"></param>
        /// <param name="scheduler"></param>
        public ReactiveCommand(IObservable<bool> canExecute = null, IScheduler scheduler = null)
        {
            canExecute = canExecute ?? Observable.Return(true).Concat(Observable.Never<bool>());
            commonCtor(scheduler);
            canExecute.Subscribe(canExecuteSubject.OnNext, canExecuteSubject.OnError, canExecuteSubject.OnCompleted);
        }

        protected ReactiveCommand(Func<object, bool> canExecute, IScheduler scheduler = null)
        {
            canExecuteExplicitFunc = canExecute;
            commonCtor(scheduler);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canExecute"></param>
        /// <param name="executed"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static ReactiveCommand Create(Func<object, bool> canExecute, Action<object> executed = null, IScheduler scheduler = null)
        {
            var ret = new ReactiveCommand(canExecute, scheduler);
            if (executed != null) {
                ret.Subscribe(executed);
            }

            return ret;
        }

        void commonCtor(IScheduler scheduler)
        {
            this.scheduler = scheduler ?? RxApp.DeferredScheduler;

            canExecuteSubject = new Subject<bool>();
            canExecuteLatest = new ObservableAsPropertyHelper<bool>(canExecuteSubject,
                b => { if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty); },
                true, scheduler);

            executeSubject = new Subject<object>();
        }

        Func<object, bool> canExecuteExplicitFunc;
        protected Subject<bool> canExecuteSubject;

        /// <summary>
        /// 
        /// </summary>
        public IObservable<bool> CanExecuteObservable {
            get { return canExecuteSubject; }
        }

        ObservableAsPropertyHelper<bool> canExecuteLatest;
        public virtual bool CanExecute(object parameter)
        {
            if (canExecuteExplicitFunc != null) {
                bool ret = canExecuteExplicitFunc(parameter);
                canExecuteSubject.OnNext(ret);
                return ret;
            }

            return canExecuteLatest.Value;
        }

        /// <summary>
        ///
        /// </summary>
        public event EventHandler CanExecuteChanged;

        IScheduler scheduler;
        Subject<object> executeSubject;

        public void Execute(object parameter)
        {
            this.Log().InfoFormat("{0:X}: Executed", this.GetHashCode());
            executeSubject.OnNext(parameter);
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return executeSubject.ObserveOn(scheduler).Subscribe(observer);
        }
    }

    public static class ReactiveCommandMixins
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="This"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static ReactiveCommand ToCommand(this IObservable<bool> This, IScheduler scheduler = null)
        {
            return new ReactiveCommand(This, scheduler);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :