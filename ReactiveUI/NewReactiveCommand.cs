using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveUI
{
    // static factory methods
    public abstract partial class NewReactiveCommand
    {
        public static SynchronousReactiveCommand<Unit, TResult> CreateSynchronous<TResult>(Func<TResult> execute, IObservable<bool> canExecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return new SynchronousReactiveCommand<Unit, TResult>(canExecute ?? Observable.Return(true), _ => execute());
        }

        public static SynchronousReactiveCommand<TParam, TResult> CreateSynchronous<TParam, TResult>(Func<TParam, TResult> execute, IObservable<bool> canExecute = null) =>
            new SynchronousReactiveCommand<TParam, TResult>(canExecute ?? Observable.Return(true), execute);

        public static AsynchronousReactiveCommand<Unit, TResult> CreateAsynchronous<TResult>(Func<IObservable<TResult>> executeAsync, IObservable<bool> canExecute = null, IScheduler scheduler = null, int maxInFlightExecutions = 1)
        {
            if (executeAsync == null)
            {
                throw new ArgumentNullException(nameof(executeAsync));
            }

            return new AsynchronousReactiveCommand<Unit, TResult>(canExecute ?? Observable.Return(true), _ => executeAsync(), scheduler, maxInFlightExecutions);
        }

        public static AsynchronousReactiveCommand<TParam, TResult> CreateAsynchronous<TParam, TResult>(Func<TParam, IObservable<TResult>> executeAsync, IObservable<bool> canExecute = null, IScheduler scheduler = null, int maxInFlightExecutions = 1) =>
            new AsynchronousReactiveCommand<TParam, TResult>(canExecute ?? Observable.Return(true), executeAsync, scheduler, maxInFlightExecutions);
    }

    // common functionality amongst all reactive commands
    public abstract partial class NewReactiveCommand : IDisposable
    {
        public abstract IObservable<bool> CanExecute
        {
            get;
        }

        public abstract IObservable<bool> IsExecuting
        {
            get;
        }

        public abstract IObservable<Exception> ThrownExceptions
        {
            get;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected abstract void Dispose(bool disposing);
    }

    // common functionality to all reactive commands that return a value of type TResult
    public abstract class NewReactiveCommand<TResult> : NewReactiveCommand, IObservable<TResult>
    {
        public abstract IDisposable Subscribe(IObserver<TResult> observer);
    }

    // a reactive command that executes synchronously
    public class SynchronousReactiveCommand<TParam, TResult> : NewReactiveCommand<TResult>
    {
        private readonly Func<TParam, TResult> execute;
        private readonly BehaviorSubject<bool> canExecute;
        private readonly BehaviorSubject<bool> isExecuting;
        private readonly Subject<TResult> results = new Subject<TResult>();
        private readonly ScheduledSubject<Exception> exceptions;
        private readonly IDisposable canExecuteSubscription;

        internal protected SynchronousReactiveCommand(
            IObservable<bool> canExecute,
            Func<TParam, TResult> execute)
        {
            if (canExecute == null)
            {
                throw new ArgumentNullException(nameof(canExecute));
            }

            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            this.execute = execute;
            this.canExecute = new BehaviorSubject<bool>(true);
            this.isExecuting = new BehaviorSubject<bool>(false);
            this.results = new Subject<TResult>();

            this.canExecuteSubscription = canExecute
                .CombineLatest(this.isExecuting, (canEx, isEx) => canEx && !isEx)
                .Catch<bool, Exception>(
                    ex =>
                    {
                        exceptions.OnNext(ex);
                        return Observable.Return(false);
                    })
                .DistinctUntilChanged()
                .Subscribe(x => this.canExecute.OnNext(x));

            this.exceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);
        }

        public override IObservable<bool> CanExecute => this.canExecute;

        public override IObservable<bool> IsExecuting => this.isExecuting;

        public override IObservable<Exception> ThrownExceptions => this.exceptions;

        public override IDisposable Subscribe(IObserver<TResult> observer) =>
            results.Subscribe(observer);

        public void Execute(TParam parameter = default(TParam))
        {
            if (!this.canExecute.Value)
            {
                this.exceptions.OnNext(new InvalidOperationException("Command cannot currently execute."));
                return;
            }

            try
            {
                this.isExecuting.OnNext(true);
                var result = this.execute(parameter);
                this.results.OnNext(result);
            }
            catch (Exception ex)
            {
                this.exceptions.OnNext(ex);
            }
            finally
            {
                this.isExecuting.OnNext(false);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.canExecuteSubscription.Dispose();
            }
        }
    }

    // a reactive command that executes asynchronously
    public class AsynchronousReactiveCommand<TParam, TResult> : NewReactiveCommand<TResult>
    {
        private readonly Func<TParam, IObservable<TResult>> executeAsync;
        private readonly IScheduler scheduler;
        private readonly int maxInFlightExecutions;
        private readonly BehaviorSubject<int> inFlightExecutions;
        private readonly BehaviorSubject<bool> canExecute;
        private readonly Subject<TResult> results = new Subject<TResult>();
        private readonly ScheduledSubject<Exception> exceptions;
        private readonly IDisposable canExecuteSubscription;

        internal protected AsynchronousReactiveCommand(
            IObservable<bool> canExecute,
            Func<TParam, IObservable<TResult>> executeAsync,
            IScheduler scheduler,
            int maxInFlightExecutions)
        {
            if (canExecute == null)
            {
                throw new ArgumentNullException(nameof(canExecute));
            }

            if (executeAsync == null)
            {
                throw new ArgumentNullException(nameof(executeAsync));
            }

            if (maxInFlightExecutions < 1)
            {
                throw new ArgumentException("maxInFlightExecutions must be greater than zero.", nameof(maxInFlightExecutions));
            }

            this.executeAsync = executeAsync;
            this.scheduler = scheduler ?? RxApp.MainThreadScheduler;
            this.maxInFlightExecutions = maxInFlightExecutions;
            this.inFlightExecutions = new BehaviorSubject<int>(0);
            this.canExecute = new BehaviorSubject<bool>(true);
            this.results = new Subject<TResult>();

            this.canExecuteSubscription = canExecute
                .CombineLatest(this.inFlightExecutions, (canEx, inFlight) => canEx && inFlight < this.maxInFlightExecutions)
                .Catch<bool, Exception>(
                    ex =>
                    {
                        exceptions.OnNext(ex);
                        return Observable.Return(false);
                    })
                .DistinctUntilChanged()
                .Subscribe(x => this.canExecute.OnNext(x));

            this.exceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);
        }

        public int MaxInFlightExecutions => this.maxInFlightExecutions;

        public override IObservable<bool> CanExecute => this.canExecute;

        public IObservable<int> InFlightExecutions => this.inFlightExecutions;

        public override IObservable<bool> IsExecuting => this.inFlightExecutions.Select(x => x > 0).DistinctUntilChanged();

        public override IObservable<Exception> ThrownExceptions => this.exceptions;

        public override IDisposable Subscribe(IObserver<TResult> observer) =>
            results.Subscribe(observer);

        public IObservable<TResult> ExecuteAsync(TParam parameter = default(TParam))
        {
            var execution = Observable
                .Start(
                    () =>
                    {
                        var inFlightExecutions = this.inFlightExecutions.Value;

                        if (inFlightExecutions >= this.maxInFlightExecutions)
                        {
                            return Observable.Throw<TResult>(
                                new InvalidOperationException(
                                    string.Format("No more executions can be performed because the maximum number of in-flight executions ({0}) has been reached.", this.maxInFlightExecutions)));
                        }

                        if (!this.canExecute.Value)
                        {
                            return Observable.Throw<TResult>(
                                new InvalidOperationException("Command cannot currently execute."));
                        }

                        ++inFlightExecutions;
                        this.inFlightExecutions.OnNext(inFlightExecutions);
                        return this.ExecuteCoreAsync(parameter);
                    },
                    this.scheduler)
                .Switch()
                .Publish()
                .RefCount();

            execution
                .Subscribe(
                    _ => { },
                    ex => exceptions.OnNext(ex));

            return execution;
        }

        private IObservable<TResult> ExecuteCoreAsync(TParam parameter) =>
            this
                .executeAsync(parameter)
                .Do(result => this.results.OnNext(result))
                .Finally(
                    () =>
                    {
                        var inFlightExecutions = this.inFlightExecutions.Value;
                        --inFlightExecutions;
                        this.inFlightExecutions.OnNext(inFlightExecutions);
                    });

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.canExecuteSubscription.Dispose();
            }
        }
    }
}