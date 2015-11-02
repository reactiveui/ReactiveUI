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
            IScheduler scheduler = null)
        {
            if (executeAsync == null)
            {
                throw new ArgumentNullException(nameof(executeAsync));
            }

            return new NewReactiveCommand<Unit, TResult>(canExecute ?? Observable.Return(true), _ => executeAsync(), scheduler ?? RxApp.MainThreadScheduler);
        }

        public static NewReactiveCommand<TParam, TResult> Create<TParam, TResult>(
                Func<TParam, IObservable<TResult>> executeAsync,
                IObservable<bool> canExecute = null,
                IScheduler scheduler = null) =>
            new NewReactiveCommand<TParam, TResult>(canExecute ?? Observable.Return(true), executeAsync, scheduler ?? RxApp.MainThreadScheduler);

        public static CombinedReactiveCommand<TParam, TResult> CreateCombined<TParam, TResult>(
                IEnumerable<NewReactiveCommandBase<TParam, TResult>> childCommands,
                IObservable<bool> canExecute = null,
                IScheduler scheduler = null) =>
            new CombinedReactiveCommand<TParam, TResult>(childCommands, canExecute ?? Observable.Return(true), scheduler ?? RxApp.MainThreadScheduler);
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
    public abstract class NewReactiveCommandBase<TParam, TResult> : NewReactiveCommand, IObservable<TResult>, ICommand
    {
        private EventHandler canExecuteChanged;

        event EventHandler ICommand.CanExecuteChanged
        {
            add { this.canExecuteChanged += value; }
            remove { this.canExecuteChanged -= value; }
        }

        public abstract IDisposable Subscribe(IObserver<TResult> observer);

        public abstract IObservable<TResult> ExecuteAsync(TParam parameter = default(TParam));

        bool ICommand.CanExecute(object parameter) =>
            this.CanExecute.First();

        void ICommand.Execute(object parameter)
        {
            if (parameter == null)
            {
                parameter = default(TParam);
            }

            this.ExecuteAsync((TParam)parameter);
        }

        protected void OnCanExecuteChanged()
        {
            var handler = this.canExecuteChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    // a reactive command that executes asynchronously
    public class NewReactiveCommand<TParam, TResult> : NewReactiveCommandBase<TParam, TResult>
    {
        private readonly Func<TParam, IObservable<TResult>> executeAsync;
        private readonly IScheduler scheduler;
        private readonly Subject<ExecutionInfo> executionInfo;
        private readonly IObservable<bool> isExecuting;
        private readonly IObservable<bool> canExecute;
        private readonly IObservable<TResult> results;
        private readonly ScheduledSubject<Exception> exceptions;
        private readonly IDisposable canExecuteSubscription;

        internal protected NewReactiveCommand(
            IObservable<bool> canExecute,
            Func<TParam, IObservable<TResult>> executeAsync,
            IScheduler scheduler)
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

            this.executeAsync = executeAsync;
            this.scheduler = scheduler;
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
                .Select(x => x.Result);

            this.exceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);

            this
                .canExecute
                .Subscribe(_ => this.OnCanExecuteChanged());

            this.canExecuteSubscription = this.canExecute.Subscribe();
        }

        public override IObservable<bool> CanExecute => this.canExecute;
        
        public override IObservable<bool> IsExecuting => this.isExecuting;

        public override IObservable<Exception> ThrownExceptions => this.exceptions;

        public override IDisposable Subscribe(IObserver<TResult> observer) =>
            results.Subscribe(observer);

        public override IObservable<TResult> ExecuteAsync(TParam parameter = default(TParam))
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
    public class CombinedReactiveCommand<TParam, TResult> : NewReactiveCommandBase<TParam, IList<TResult>>
    {
        private readonly NewReactiveCommand<TParam, IList<TResult>> innerCommand;
        private readonly ScheduledSubject<Exception> exceptions;
        private readonly IDisposable exceptionsSubscription;

        internal protected CombinedReactiveCommand(
            IEnumerable<NewReactiveCommandBase<TParam, TResult>> childCommands,
            IObservable<bool> canExecute,
            IScheduler scheduler)
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
                scheduler);

            this.exceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);

            this
                .CanExecute
                .Subscribe(_ => this.OnCanExecuteChanged());
        }

        public override IObservable<bool> CanExecute => this.innerCommand.CanExecute;

        public override IObservable<bool> IsExecuting => this.innerCommand.IsExecuting;

        public override IObservable<Exception> ThrownExceptions => this.exceptions;

        public override IDisposable Subscribe(IObserver<IList<TResult>> observer) =>
            innerCommand.Subscribe(observer);

        public override IObservable<IList<TResult>> ExecuteAsync(TParam parameter = default(TParam)) =>
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
}