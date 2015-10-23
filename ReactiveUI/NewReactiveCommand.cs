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
        public static SynchronousReactiveCommand<Unit, TResult> CreateSynchronous<TResult>(Func<TResult> execute, IObservable<bool> canExecute = null, IScheduler scheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return new SynchronousReactiveCommand<Unit, TResult>(_ => execute(), canExecute ?? Observable.Return(true), scheduler ?? RxApp.MainThreadScheduler);
        }

        public static SynchronousReactiveCommand<TParam, TResult> CreateSynchronous<TParam, TResult>(Func<TParam, TResult> execute, IObservable<bool> canExecute = null, IScheduler scheduler = null) =>
            new SynchronousReactiveCommand<TParam, TResult>(execute, canExecute ?? Observable.Return(true), scheduler ?? RxApp.MainThreadScheduler);

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
        private readonly Subject<ExecutionInfo> executionInfo;
        private readonly IObservable<bool> isExecuting;
        private readonly IObservable<bool> canExecute;
        private readonly IObservable<TResult> results;
        private readonly ScheduledSubject<Exception> exceptions;
        private readonly IDisposable canExecuteSubscription;

        internal protected SynchronousReactiveCommand(
            Func<TParam, TResult> execute,
            IObservable<bool> canExecute,
            IScheduler scheduler)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException(nameof(canExecute));
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            this.execute = execute;
            this.executionInfo = new Subject<ExecutionInfo>();
            this.isExecuting = this
                .executionInfo
                .Select(x => x.Demarcation == ExecutionDemarcation.Begin)
                .StartWith(false)
                .DistinctUntilChanged()
                .Replay(1)
                .RefCount();
            this.canExecute = canExecute
                .Catch<bool, Exception>(
                    ex =>
                    {
                        this.exceptions.OnNext(ex);
                        return Observable.Return(false);
                    })
                .StartWith(true)
                .CombineLatest(this.isExecuting, (canEx, isEx) => canEx && !isEx)
                .DistinctUntilChanged()
                .Replay(1)
                .RefCount();
            this.results = this
                .executionInfo
                .Where(x => x.Demarcation == ExecutionDemarcation.EndWithResult)
                .Select(x => x.Result)
                .ObserveOn(scheduler);

            this.exceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);

            this.canExecuteSubscription = this.canExecute.Subscribe();
        }

        public override IObservable<bool> CanExecute => this.canExecute;

        public override IObservable<bool> IsExecuting => this.isExecuting;

        public override IObservable<Exception> ThrownExceptions => this.exceptions;

        public override IDisposable Subscribe(IObserver<TResult> observer) =>
            results.Subscribe(observer);

        public void Execute(TParam parameter = default(TParam))
        {
            // TODO: would be good if we could remove this obsolete synchronous call, but not really sure of a sensible way to do that
            if (!this.canExecute.First())
            {
                this.exceptions.OnNext(new InvalidOperationException("Command cannot currently execute."));
                return;
            }

            try
            {
                this.executionInfo.OnNext(ExecutionInfo.CreateBegin());
                var result = this.execute(parameter);
                this.executionInfo.OnNext(ExecutionInfo.CreateResult(result));
            }
            catch (Exception ex)
            {
                this.executionInfo.OnNext(ExecutionInfo.CreateFail());
                this.exceptions.OnNext(ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.executionInfo.Dispose();
                this.canExecuteSubscription.Dispose();
                this.exceptions.Dispose();
            }
        }

        private enum ExecutionDemarcation
        {
            Begin,
            EndWithResult,
            EndWithException
        }

        private struct ExecutionInfo
        {
            private readonly ExecutionDemarcation demarcation;
            private readonly TResult result;

            private ExecutionInfo(ExecutionDemarcation demarcation, TResult result)
            {
                this.demarcation = demarcation;
                this.result = result;
            }

            public ExecutionDemarcation Demarcation => this.demarcation;

            public TResult Result => this.result;

            public static ExecutionInfo CreateBegin() =>
                new ExecutionInfo(ExecutionDemarcation.Begin, default(TResult));

            public static ExecutionInfo CreateResult(TResult result) =>
                new ExecutionInfo(ExecutionDemarcation.EndWithResult, result);

            public static ExecutionInfo CreateFail() =>
                new ExecutionInfo(ExecutionDemarcation.EndWithException, default(TResult));
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
        private readonly Subject<TResult> results;
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
            this.exceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);

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
                this.canExecute.Dispose();
                this.inFlightExecutions.Dispose();
                this.results.Dispose();
                this.exceptions.Dispose();
            }
        }
    }
}