using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveUI
{
    // DO AN IDEAL IMPLEMENTATION FIRST, THEN SCALE BACK AS REQUIRED
    // too much type explosion?

    public abstract class NewReactiveCommand
    {
    }

    public abstract class NewReactiveCommand<TResult> : NewReactiveCommand, IObservable<TResult>
    {
        public abstract IDisposable Subscribe(IObserver<TResult> observer);
    }

    public class SynchronousReactiveCommand<TParam, TResult> : NewReactiveCommand<TResult>
    {
        public override IDisposable Subscribe(IObserver<TResult> observer)
        {
            throw new NotImplementedException();
        }
    }

    // incorporates a given reactive pipeline into each call to ExecuteAsync
    // if the number of in-flight executions is maxInFlightExecutions, an exception is thrown instead
    // 
    // will have SynchronousReactiveCommand counterpart
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

        public AsynchronousReactiveCommand(
                Func<TParam, IObservable<TResult>> executeAsync,
                IScheduler scheduler = null,
                int maxInFlightExecutions = 1)
            : this(Observable.Return(true), executeAsync, scheduler, maxInFlightExecutions)
        {
        }

        public AsynchronousReactiveCommand(
            IObservable<bool> canExecute,
            Func<TParam, IObservable<TResult>> executeAsync,
            IScheduler scheduler = null,
            int maxInFlightExecutions = 1)
        {
            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            if (executeAsync == null)
            {
                throw new ArgumentNullException("executeAsync");
            }

            if (maxInFlightExecutions < 1)
            {
                throw new ArgumentException("maxInFlightExecutions must be greater than zero.");
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

            this.ThrownExceptions = this.exceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);
        }

        public int MaxInFlightExecutions => this.maxInFlightExecutions;

        public IObservable<bool> CanExecute => this.canExecute;

        public IObservable<int> InFlightExecutions => this.inFlightExecutions;

        public IObservable<bool> IsExecuting => this.inFlightExecutions.Select(x => x > 0).DistinctUntilChanged();

        public IObservable<Exception> ThrownExceptions
        {
            get;
            protected set;
        }

        public override IDisposable Subscribe(IObserver<TResult> observer) =>
            results.Subscribe(observer);

        public IObservable<TResult> ExecuteAsync(TParam parameter = default(TParam))
        {
            var execution = Observable
                .Start(
                    () =>
                    {
                        var inFlightExecutions = this.inFlightExecutions.Value;

                        if (inFlightExecutions < this.maxInFlightExecutions)
                        {
                            ++inFlightExecutions;
                            this.inFlightExecutions.OnNext(inFlightExecutions);
                            return this.ExecuteCoreAsync(parameter);
                        }

                        return Observable.Throw<TResult>(
                            new InvalidOperationException(
                                string.Format("No more executions can be performed because the maximum number of in-flight executions ({0}) has been reached.", this.maxInFlightExecutions)));
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
    }
}