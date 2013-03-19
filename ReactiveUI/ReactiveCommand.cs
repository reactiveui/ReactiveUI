using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

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
        /// <param name="initialCondition">Initial CanExecute state</param>
        public ReactiveCommand(IObservable<bool> canExecute = null, IScheduler scheduler = null, bool initialCondition = true)
        {
            canExecute = canExecute ?? Observable.Return(true).Concat(Observable.Never<bool>());
            canExecute = canExecute.ObserveOn(scheduler ?? RxApp.DeferredScheduler);
            commonCtor(scheduler, initialCondition);

            _inner = canExecute.Subscribe(
                _canExecuteSubject.OnNext, 
                _exSubject.OnNext);

            ThrownExceptions = _exSubject;
        }

        protected ReactiveCommand(Func<object, Task<bool>> canExecuteFunc, IScheduler scheduler = null)
        {
            var canExecute = _canExecuteProbed.SelectMany(x => canExecuteFunc(x).ToObservable());

            commonCtor(scheduler);

            _inner = canExecute.Subscribe(
                _canExecuteSubject.OnNext, 
                _exSubject.OnNext);
        }

        protected ReactiveCommand(Func<object, bool> canExecute, IScheduler scheduler = null)
        {
            _canExecuteExplicitFunc = canExecute;
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

        public IObservable<Exception> ThrownExceptions { get; protected set; }

        void commonCtor(IScheduler scheduler, bool initialCondition = true)
        {
            this.scheduler = scheduler ?? RxApp.DeferredScheduler;

            _canExecuteSubject = new ScheduledSubject<bool>(RxApp.DeferredScheduler);
            canExecuteLatest = new ObservableAsPropertyHelper<bool>(_canExecuteSubject,
                b => { if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty); },
                initialCondition, scheduler);

            _canExecuteProbed = new Subject<object>();
            executeSubject = new Subject<object>();

            _exSubject = new ScheduledSubject<Exception>(RxApp.DeferredScheduler, RxApp.DefaultExceptionHandler);
            ThrownExceptions = _exSubject;
        }

        Func<object, bool> _canExecuteExplicitFunc;
        protected ISubject<bool> _canExecuteSubject;
        protected Subject<object> _canExecuteProbed;
        IDisposable _inner = null;
        ScheduledSubject<Exception> _exSubject;
    
        /// <summary>
        /// Fires whenever the CanExecute of the ICommand changes. 
        /// </summary>
        public IObservable<bool> CanExecuteObservable {
            get { return _canExecuteSubject.DistinctUntilChanged(); }
        }

        ObservableAsPropertyHelper<bool> canExecuteLatest;
        public virtual bool CanExecute(object parameter)
        {
            _canExecuteProbed.OnNext(parameter);
            if (_canExecuteExplicitFunc != null) {
                bool ret = _canExecuteExplicitFunc(parameter);
                _canExecuteSubject.OnNext(ret);
                return ret;
            }

            return canExecuteLatest.Value;
        }

        public event EventHandler CanExecuteChanged;

        IScheduler scheduler;
        Subject<object> executeSubject;

        public void Execute(object parameter)
        {
            this.Log().Debug("{0:X}: Executed", this.GetHashCode());
            executeSubject.OnNext(parameter);
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return executeSubject.ObserveOn(scheduler).Subscribe(
                Observer.Create<object>(
                    x => marshalFailures(observer.OnNext, x),
                    ex => marshalFailures(observer.OnError, ex),
                    () => marshalFailures(observer.OnCompleted)));
        }

        public void Dispose()
        {
            if (_inner != null) {
                _inner.Dispose();
            }
        }

        void marshalFailures<T>(Action<T> block, T param)
        {
            try {
                block(param);
            } catch (Exception ex) {
                _exSubject.OnNext(ex);
            }
        }

        void marshalFailures(Action block)
        {
            marshalFailures(_ => block(), Unit.Default);
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
