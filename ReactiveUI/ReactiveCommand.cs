using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using Splat;

namespace ReactiveUI
{
    public static class ReactiveCommand
    {
        public static ReactiveCommand<object> Create(IObservable<bool> canExecute = null, IScheduler scheduler = null)
        {
            canExecute = canExecute ?? Observable.Return(true);
            return new ReactiveCommand<object>(canExecute, x => Observable.Return(x), scheduler);
        }

        public static ReactiveCommand<Unit> Create(IObservable<bool> canExecute, Action<object> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<Unit>(canExecute, x => Observable.Start(() => executeAsync(x), RxApp.TaskpoolScheduler), scheduler);
        }

        public static ReactiveCommand<T> Create<T>(IObservable<bool> canExecute, Func<object, IObservable<T>> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<T>(canExecute, executeAsync, scheduler);
        }

        public static ReactiveCommand<T> CreateWithFunction<T>(IObservable<bool> canExecute, Func<object, T> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<T>(canExecute, x => Observable.Start(() => executeAsync(x), RxApp.TaskpoolScheduler), scheduler);
        }

        public static ReactiveCommand<T> CreateAsync<T>(IObservable<bool> canExecute, Func<object, Task<T>> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<T>(canExecute, x => executeAsync(x).ToObservable(), scheduler);
        }

        public static ReactiveCommand<Unit> Create(Action<object> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<Unit>(Observable.Return(true), x => Observable.Start(() => executeAsync(x), RxApp.TaskpoolScheduler), scheduler);
        }

        public static ReactiveCommand<T> Create<T>(Func<object, IObservable<T>> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<T>(Observable.Return(true), executeAsync, scheduler);
        }

        public static ReactiveCommand<T> CreateWithFunction<T>(Func<object, T> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<T>(Observable.Return(true), x => Observable.Start(() => executeAsync(x), RxApp.TaskpoolScheduler), scheduler);
        }

        public static ReactiveCommand<T> CreateAsync<T>(Func<object, Task<T>> executeAsync, IScheduler scheduler = null)
        {
            return new ReactiveCommand<T>(Observable.Return(true), x => executeAsync(x).ToObservable(), scheduler);
        }

        /// <summary>
        /// This creates a ReactiveCommand that calls several child 
        /// ReactiveCommands when invoked. Its CanExecute will match the
        /// combined result of the child CanExecutes (i.e. if any child
        /// commands cannot execute, neither can the parent)
        /// </summary>
        /// <param name="canExecute">An Observable that determines whether the 
        /// parent command can execute</param>
        /// <param name="commands">The commands to combine.</param>
        public static ReactiveCommand<object> CreateCombined(IObservable<bool> canExecute, params IReactiveCommand[] commands)
        {
            var childrenCanExecute = commands
                .Select(x => x.CanExecuteObservable)
                .CombineLatest(latestCanExecute => latestCanExecute.All(x => x != false));

            var canExecuteSum = Observable.CombineLatest(
                canExecute.StartWith(true),
                childrenCanExecute,
                (parent, child) => parent && child);

            var ret = ReactiveCommand.Create(canExecuteSum);
            ret.Subscribe(x => commands.ForEach(cmd => cmd.Execute(x)));
            return ret;
        }

        public static ReactiveCommand<object> CreateCombined(params IReactiveCommand[] commands)
        {
            return CreateCombined(Observable.Return(true), commands);
        }
    }

    public class ReactiveCommand<T> : IReactiveCommand<T>
    {
        readonly Subject<T> executeResults = new Subject<T>();
        readonly Subject<bool> isExecuting = new Subject<bool>();
        readonly Func<object, IObservable<T>> executeAsync;
        readonly IScheduler scheduler;
        readonly ScheduledSubject<Exception> exceptions;

        IConnectableObservable<bool> canExecute;
        bool canExecuteLatest = false;
        IDisposable canExecuteDisp;
        int inflightCount = 0;

        public ReactiveCommand(IObservable<bool> canExecute, Func<object, IObservable<T>> executeAsync, IScheduler scheduler = null)
        {
            this.scheduler = scheduler ?? RxApp.MainThreadScheduler;
            this.executeAsync = executeAsync;

            this.canExecute = canExecute.CombineLatest(isExecuting.StartWith(false), (ce, ie) => ce && !ie)
                .Catch<bool, Exception>(ex => {
                    exceptions.OnNext(ex);
                    return Observable.Return(false);
                })
                .Do(x => {
                    var fireCanExecuteChanged = (canExecuteLatest != x);
                    canExecuteLatest = x;

                    if (fireCanExecuteChanged) {
                        CanExecuteChangedEventManager.DeliverEvent(this, EventArgs.Empty);
                    }
                })
                .Publish();

            ThrownExceptions = exceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);
        }

        public IObservable<T> ExecuteAsync(object parameter = null)
        {
            var ret = Observable.Create<T>(subj => {
                if (Interlocked.Increment(ref inflightCount) == 1) {
                    isExecuting.OnNext(true);
                }

                var decrement = new SerialDisposable() { 
                    Disposable = Disposable.Create(() => {
                        if (Interlocked.Decrement(ref inflightCount) == 0) {
                            isExecuting.OnNext(false);
                        }
                    })
                };

                var disp = executeAsync(parameter)
                    .ObserveOn(scheduler)
                    .Finally(() => decrement.Disposable = Disposable.Empty)
                    .Do(x => executeResults.OnNext(x))
                    .Catch<T, Exception>(ex => {
                        exceptions.OnNext(ex);
                        return Observable.Empty<T>();
                    })
                    .Subscribe(subj);

                return new CompositeDisposable(disp, decrement);
            });

            return ret.Publish().RefCount();
        }

        /// <summary>
        /// Fires whenever an exception would normally terminate ReactiveUI 
        /// internal state.
        /// </summary>
        /// <value>The thrown exceptions.</value>
        public IObservable<Exception> ThrownExceptions { get; protected set; }

        public IObservable<bool> CanExecuteObservable {
            get {
                var ret = canExecute.StartWith(canExecuteLatest).DistinctUntilChanged();

                if (canExecuteDisp != null) return null;
                return Observable.Create<bool>(subj => {
                    var disp = ret.Subscribe(subj);

                    // NB: We intentionally leak the CanExecute disconnect, it's 
                    // cleaned up by the global Dispose. This is kind of a
                    // "Lazy Subscription" to CanExecute by the command itself.
                    canExecuteDisp = canExecute.Connect();
                    return disp;
                });
            }
        }

        public IObservable<bool> IsExecuting {
            get { return isExecuting.StartWith(inflightCount > 0); }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return executeResults.Subscribe(observer);
        }

        public bool CanExecute(object parameter)
        {
            if (canExecuteDisp == null) canExecuteDisp = canExecute.Connect();
            return canExecuteLatest;
        }

        public event EventHandler CanExecuteChanged
        {
            add { 
                if (canExecuteDisp == null) canExecuteDisp = canExecute.Connect();
                CanExecuteChangedEventManager.AddHandler(this, value); 
            }
            remove { CanExecuteChangedEventManager.RemoveHandler(this, value); }
        }

        public void Execute(object parameter)
        {
            ExecuteAsync(parameter).Subscribe();
        }

        public void Dispose()
        {
            var disp = Interlocked.Exchange(ref canExecuteDisp, null);
            if (disp != null) disp.Dispose();
        }
    }

    public static class ReactiveCommandMixins
    {
        /// <summary>
        /// ToCommand is a convenience method for returning a new
        /// ReactiveCommand based on an existing Observable chain.
        /// </summary>
        /// <param name="scheduler">The scheduler to publish events on - default
        /// is RxApp.MainThreadScheduler.</param>
        /// <returns>A new ReactiveCommand whose CanExecute Observable is the
        /// current object.</returns>
        public static ReactiveCommand<object> ToCommand(this IObservable<bool> This, IScheduler scheduler = null)
        {
            return ReactiveCommand.Create(This, scheduler);
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
            return This.ObserveOn(RxApp.MainThreadScheduler).Subscribe(x => {
                if (!command.CanExecute(x)) {
                    return;
                }
                command.Execute(x);
            });
        }

        /// <summary>
        /// A utility method that will pipe an Observable to an ICommand (i.e.
        /// it will first call its CanExecute with the provided value, then if
        /// the command can be executed, Execute() will be called)
        /// </summary>
        /// <param name="target">The root object which has the Command.</param>
        /// <param name="commandProperty">The expression to reference the Command.</param>
        /// <returns>An object that when disposes, disconnects the Observable
        /// from the command.</returns>
        public static IDisposable InvokeCommand<T, TTarget>(this IObservable<T> This, TTarget target, Expression<Func<TTarget, ICommand>> commandProperty)
        {
            return This.CombineLatest(target.WhenAnyValue(commandProperty), (val, cmd) => new { val, cmd })
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => {
                    if (!x.cmd.CanExecute(x.val)) {
                        return;
                    }

                    x.cmd.Execute(x.val);
                });
        }
    }
}