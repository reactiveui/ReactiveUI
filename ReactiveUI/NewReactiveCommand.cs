using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace ReactiveUI
{
    // static factory methods
    public abstract partial class NewReactiveCommand
    {
        public static NewReactiveCommand<Unit, TResult> Create<TResult>(
            Func<IObservable<TResult>> executeAsync,
            IObservable<bool> canExecute = null,
            IScheduler scheduler = null,
            int maxInFlightExecutions = 1)
        {
            if (executeAsync == null)
            {
                throw new ArgumentNullException(nameof(executeAsync));
            }

            return new NewReactiveCommand<Unit, TResult>(canExecute ?? Observable.Return(true), _ => executeAsync(), scheduler ?? RxApp.MainThreadScheduler, maxInFlightExecutions);
        }

        public static NewReactiveCommand<TParam, TResult> Create<TParam, TResult>(
                Func<TParam, IObservable<TResult>> executeAsync,
                IObservable<bool> canExecute = null,
                IScheduler scheduler = null,
                int maxInFlightExecutions = 1) =>
            new NewReactiveCommand<TParam, TResult>(canExecute ?? Observable.Return(true), executeAsync, scheduler ?? RxApp.MainThreadScheduler, maxInFlightExecutions);

        public static CombinedReactiveCommand<TParam, TResult> CreateCombined<TParam, TResult>(
                IEnumerable<NewReactiveCommand<TParam, TResult>> childCommands,
                IObservable<bool> canExecute = null,
                IScheduler scheduler = null,
                int maxInFlightExecutions = 1) =>
            new CombinedReactiveCommand<TParam, TResult>(childCommands, canExecute ?? Observable.Return(true), scheduler ?? RxApp.MainThreadScheduler, maxInFlightExecutions);
    }

    // non-generic reactive command functionality
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

    // a reactive command that executes asynchronously
    public class NewReactiveCommand<TParam, TResult> : NewReactiveCommand<TResult>
    {
        private readonly Func<TParam, IObservable<TResult>> executeAsync;
        private readonly IScheduler scheduler;
        private readonly int maxInFlightExecutions;
        private readonly Subject<ExecutionInfo> executionInfo;
        private readonly IObservable<int> inFlightExecutions;
        private readonly IObservable<bool> isExecuting;
        private readonly IObservable<bool> canExecute;
        private readonly IObservable<TResult> results;
        private readonly ScheduledSubject<Exception> exceptions;
        private readonly IDisposable canExecuteSubscription;

        internal protected NewReactiveCommand(
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

            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            if (maxInFlightExecutions < 1)
            {
                throw new ArgumentException("maxInFlightExecutions must be greater than zero.", nameof(maxInFlightExecutions));
            }

            this.executeAsync = executeAsync;
            this.scheduler = scheduler;
            this.maxInFlightExecutions = maxInFlightExecutions;
            this.executionInfo = new Subject<ExecutionInfo>();
            this.inFlightExecutions = this
                .executionInfo
                .Scan(0, (running, next) => running + (next.Demarcation == ExecutionDemarcation.Begin ? 1 : -1))
                .StartWith(0)
                .Replay(1)
                .RefCount();
            this.isExecuting = this
                .inFlightExecutions
                .Select(x => x > 0)
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
                .CombineLatest(this.inFlightExecutions, (canEx, inFlight) => canEx && inFlight < this.maxInFlightExecutions)
                .DistinctUntilChanged()
                .Replay(1)
                .RefCount();
            this.results = this
                .executionInfo
                .Where(x => x.Demarcation == ExecutionDemarcation.EndWithResult)
                .Select(x => x.Result);

            this.exceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);

            this.canExecuteSubscription = this.canExecute.Subscribe();
        }

        public int MaxInFlightExecutions => this.maxInFlightExecutions;

        public override IObservable<bool> CanExecute => this.canExecute;

        public IObservable<int> InFlightExecutions => this.inFlightExecutions;

        public override IObservable<bool> IsExecuting => this.isExecuting;

        public override IObservable<Exception> ThrownExceptions => this.exceptions;

        public override IDisposable Subscribe(IObserver<TResult> observer) =>
            results.Subscribe(observer);

        public IObservable<TResult> ExecuteAsync(TParam parameter = default(TParam))
        {
            var execution = Observable
                .Start(
                    () =>
                    {
                        if (!this.canExecute.First())
                        {
                            return Observable.Throw<TResult>(
                                new InvalidOperationException("Command cannot currently execute."));
                        }

                        this.executionInfo.OnNext(ExecutionInfo.CreateBegin());
                        return this.executeAsync(parameter);
                    },
                    this.scheduler)
                .Switch()
                .Do(result => this.executionInfo.OnNext(ExecutionInfo.CreateResult(result)))
                .Catch<TResult, Exception>(
                    ex =>
                    {
                        this.executionInfo.OnNext(ExecutionInfo.CreateFail());
                        exceptions.OnNext(ex);
                        return Observable.Empty<TResult>();
                    })
                .Publish()
                .RefCount();

            execution.Subscribe();

            return execution;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.executionInfo.Dispose();
                this.exceptions.Dispose();
                this.canExecuteSubscription.Dispose();
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

    // a reactive command that combines the execution of multiple child commands
    public class CombinedReactiveCommand<TParam, TResult> : NewReactiveCommand<IList<TResult>>
    {
        private readonly int maxInFlightExecutions;
        private readonly NewReactiveCommand<TParam, IList<TResult>> innerCommand;
        private readonly ScheduledSubject<Exception> exceptions;
        private readonly IDisposable exceptionsSubscription;

        internal protected CombinedReactiveCommand(
            IEnumerable<NewReactiveCommand<TParam, TResult>> childCommands,
            IObservable<bool> canExecute,
            IScheduler scheduler,
            int maxInFlightExecutions)
        {
            if (childCommands == null)
            {
                throw new ArgumentNullException(nameof(childCommands));
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException(nameof(canExecute));
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            var childCommandsArray = childCommands.ToArray();

            if (childCommandsArray.Length == 0)
            {
                throw new ArgumentException("No child commands provided.", nameof(childCommands));
            }

            if (childCommandsArray.Any(x => maxInFlightExecutions > x.MaxInFlightExecutions))
            {
                throw new ArgumentException("All child commands must have equal or higher MaxInFlightExecutions."); ;
            }

            this.maxInFlightExecutions = maxInFlightExecutions;

            var canChildrenExecute = Observable
                .CombineLatest(childCommandsArray.Select(x => x.CanExecute))
                .Select(x => x.All(y => y));
            var combinedCanExecute = canExecute
                .Catch<bool, Exception>(
                    ex =>
                    {
                        this.exceptions.OnNext(ex);
                        return Observable.Return(false);
                    })
                .StartWith(true)
                .CombineLatest(canChildrenExecute, (ce, cce) => ce && cce)
                .DistinctUntilChanged()
                .Replay(1)
                .RefCount();
            this.exceptionsSubscription= Observable
                .Merge(childCommandsArray.Select(x => x.ThrownExceptions))
                .Subscribe(ex => this.exceptions.OnNext(ex));

            this.innerCommand = new NewReactiveCommand<TParam, IList<TResult>>(
                combinedCanExecute,
                param =>
                    Observable
                        .CombineLatest(
                            childCommandsArray
                                .Select(x => x.ExecuteAsync(param))),
                scheduler,
                maxInFlightExecutions);

            this.exceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);
        }

        public int MaxInFlightExecutions => this.maxInFlightExecutions;

        public override IObservable<bool> CanExecute => this.innerCommand.CanExecute;

        public IObservable<int> InFlightExecutions => this.innerCommand.InFlightExecutions;

        public override IObservable<bool> IsExecuting => this.innerCommand.IsExecuting;

        public override IObservable<Exception> ThrownExceptions => this.exceptions;

        public override IDisposable Subscribe(IObserver<IList<TResult>> observer) =>
            innerCommand.Subscribe(observer);

        public IObservable<IList<TResult>> ExecuteAsync(TParam parameter = default(TParam)) =>
            this.innerCommand.ExecuteAsync(parameter);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.innerCommand.Dispose();
                this.exceptions.Dispose();
                this.exceptionsSubscription.Dispose();
            }
        }
    }

    public static class NewReactiveCommandMixins
    {
        // TODO: might want to rename "platform commands" to something else?
        public static ICommand ToPlatform<TParam, TResult>(this NewReactiveCommand<TParam, TResult> @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            return new PlatformCommand<TParam>(@this.CanExecute, param => @this.ExecuteAsync(param));
        }

        public static ICommand ToPlatform<TParam, TResult>(this CombinedReactiveCommand<TParam, TResult> @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            return new PlatformCommand<TParam>(@this.CanExecute, param => @this.ExecuteAsync(param));
        }

        // TODO: other mixins that are deemed useful
    }

    public sealed class PlatformCommand<TParam> : ICommand, IDisposable
    {
        private readonly Action<TParam> execute;
        private readonly IDisposable canExecuteSubscription;
        private bool latestCanExecute;

        public PlatformCommand(
            IObservable<bool> canExecute,
            Action<TParam> execute)
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
            this.canExecuteSubscription = canExecute
                .Subscribe(x => this.LatestCanExecute = x);
        }

        public event EventHandler CanExecuteChanged;

        private bool LatestCanExecute
        {
            get { return this.latestCanExecute; }
            set
            {
                if (this.latestCanExecute == value)
                {
                    return;
                }

                this.latestCanExecute = value;
                this.OnCanExecuteChanged();
            }
        }

        public bool CanExecute(object parameter) =>
            this.latestCanExecute;

        public void Execute(object parameter)
        {
            // if TParam is a value type, we need to make sure it defaults to its default value because otherwise casting from null to TParam below will fail
            if (parameter == null)
            {
                parameter = default(TParam);
            }

            this.execute((TParam)parameter);
        }

        private void OnCanExecuteChanged() =>
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public void Dispose() =>
            this.canExecuteSubscription.Dispose();
    }
}