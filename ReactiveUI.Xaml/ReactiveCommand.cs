using System;
using System.Collections.Generic;
using System.Linq;
using System.Concurrency;
using ReactiveUI;

namespace ReactiveUI.Xaml
{
    /// <summary>
    /// IReactiveCommand is an Rx-enabled version of ICommand that is also an
    /// Observable. Its Observable fires once for each invocation of
    /// ICommand.Execute and its value is the CommandParameter that was
    /// provided.
    /// </summary>
    public class ReactiveCommand : IReactiveCommand, IEnableLogger
    {
        /// <summary>
        /// Creates a new ReactiveCommand object.
        /// </summary>
        /// <param name="canExecute">An Observable, often obtained via
        /// ObservableFromProperty, that defines when the Command can
        /// execute.</param>
        /// <param name="scheduler">The scheduler to publish events on - default
        /// is RxApp.DeferredScheduler.</param>
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
        /// Creates a new ReactiveCommand object in an imperative, non-Rx way,
        /// similar to RelayCommand.
        /// </summary>
        /// <param name="canExecute">A function that determines when the Command
        /// can execute.</param>
        /// <param name="executed">A method that will be invoked when the
        /// Execute method is invoked.</param>
        /// <param name="scheduler">The scheduler to publish events on - default
        /// is RxApp.DeferredScheduler.</param>
        /// <returns>A new ReactiveCommand object.</returns>
        public static ReactiveCommand Create(
            Func<object, bool> canExecute, 
            Action<object> executed = null, 
            IScheduler scheduler = null)
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
        /// Fires whenever the CanExecute of the ICommand changes. 
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
        /// ToCommand is a convenience method for returning a new
        /// ReactiveCommand based on an existing Observable chain.
        /// </summary>
        /// <param name="scheduler">The scheduler to publish events on - default
        /// is RxApp.DeferredScheduler.</param>
        /// <returns>A new ReactiveCommand whose CanExecute Observable is the
        /// current object.</returns>
        public static ReactiveCommand ToCommand(this IObservable<bool> This, IScheduler scheduler = null)
        {
            return new ReactiveCommand(This, scheduler);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
