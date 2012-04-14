﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using NLog;
using ReactiveUI;

#if !WINDOWS_PHONE
using System.Threading.Tasks;
#endif

namespace ReactiveUI.Xaml
{
    /// <summary>
    /// IReactiveCommand is an Rx-enabled version of ICommand that is also an
    /// Observable. Its Observable fires once for each invocation of
    /// ICommand.Execute and its value is the CommandParameter that was
    /// provided.
    /// </summary>
    public class ReactiveCommand : IReactiveCommand, IDisposable, IEnableLogger
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

            var exSubject = new ScheduledSubject<Exception>(RxApp.DeferredScheduler, RxApp.DefaultExceptionHandler);

            _inner = canExecute.Subscribe(
                canExecuteSubject.OnNext, 
                exSubject.OnNext);

            ThrownExceptions = exSubject;
        }

        #if !WINDOWS_PHONE
        protected ReactiveCommand(Func<object, Task<bool>> canExecuteFunc, IScheduler scheduler = null)
        {
            var canExecute = canExecuteProbed.SelectAsync(canExecuteFunc);

            commonCtor(scheduler);

            var exSubject = new ScheduledSubject<Exception>(RxApp.DeferredScheduler, RxApp.DefaultExceptionHandler);

            _inner = canExecute.Subscribe(
                canExecuteSubject.OnNext, 
                exSubject.OnNext);

            ThrownExceptions = exSubject;
        }
#endif

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

#if !WINDOWS_PHONE
        /// <summary>
        /// Creates a new ReactiveCommand object in an imperative, non-Rx way,
        /// similar to RelayCommand, only via a TPL Async method
        /// </summary>
        /// <param name="canExecute">A function that determines when the Command
        /// can execute.</param>
        /// <param name="executed">A method that will be invoked when the
        /// Execute method is invoked.</param>
        /// <param name="scheduler">The scheduler to publish events on - default
        /// is RxApp.DeferredScheduler.</param>
        /// <returns>A new ReactiveCommand object.</returns>
        public static ReactiveCommand Create(
            Func<object, Task<bool>> canExecute, 
            Action<object> executed = null, 
            IScheduler scheduler = null)
        {
            var ret = new ReactiveCommand(canExecute, scheduler);
            if (executed != null) {
                ret.Subscribe(executed);
            }

            return ret;
        }
#endif

        public IObservable<Exception> ThrownExceptions { get; protected set; }

        void commonCtor(IScheduler scheduler)
        {
            this.scheduler = scheduler ?? RxApp.DeferredScheduler;

            canExecuteSubject = new ScheduledSubject<bool>(RxApp.DeferredScheduler);
            canExecuteLatest = new ObservableAsPropertyHelper<bool>(canExecuteSubject,
                b => { if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty); },
                true, scheduler);

            canExecuteProbed = new Subject<object>();
            executeSubject = new Subject<object>();
        }

        Func<object, bool> canExecuteExplicitFunc;
        protected ISubject<bool> canExecuteSubject;
        protected Subject<object> canExecuteProbed;
        IDisposable _inner = null;

    
        /// <summary>
        /// Fires whenever the CanExecute of the ICommand changes. 
        /// </summary>
        public IObservable<bool> CanExecuteObservable {
            get { return canExecuteSubject.DistinctUntilChanged(); }
        }

        ObservableAsPropertyHelper<bool> canExecuteLatest;
        public virtual bool CanExecute(object parameter)
        {
            canExecuteProbed.OnNext(parameter);
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
            this.Log().Info("{0:X}: Executed", this.GetHashCode());
            executeSubject.OnNext(parameter);
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return executeSubject.ObserveOn(scheduler).Subscribe(observer);
        }

        public void Dispose()
        {
            if (_inner != null) {
                _inner.Dispose();
            }
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

        /// <summary>
        /// A utility method that will pipe an Observable to an ICommand (i.e.
        /// it will first call its CanExecute with the provided value, then if
        /// the command can be executed, Execute() will be called)
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <returns>An object that when disposes, disconnects the Observable
        /// from the command.</returns>
        public static IDisposable InvokeCommand<T>(this IObservable<T> This, ICommand command)
        {
            return This.ObserveOn(RxApp.DeferredScheduler).Subscribe(x => {
                if (!command.CanExecute(x)) {
                    return;
                }
                command.Execute(x);
            });
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
