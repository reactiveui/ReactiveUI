using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReactiveUI
{
    // static factory methods
    public abstract partial class ReactiveCommand
    {
        public static ReactiveCommand<Unit, Unit> Create(
            Action execute,
            IObservable<bool> canExecute = null,
            IScheduler scheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return new ReactiveCommand<Unit, Unit>(
                canExecute ?? Observable.Return(true),
                _ =>
                {
                    execute();
                    return Observable.Return(Unit.Default);
                },
                scheduler ?? RxApp.MainThreadScheduler);
        }

        public static ReactiveCommand<TParam, Unit> Create<TParam>(
            Action<TParam> execute,
            IObservable<bool> canExecute = null,
            IScheduler scheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return new ReactiveCommand<TParam, Unit>(
                canExecute ?? Observable.Return(true),
                param =>
                {
                    execute(param);
                    return Observable.Return(Unit.Default);
                },
                scheduler ?? RxApp.MainThreadScheduler);
        }

        public static ReactiveCommand<Unit, TResult> Create<TResult>(
            Func<IObservable<TResult>> executeAsync,
            IObservable<bool> canExecute = null,
            IScheduler scheduler = null)
        {
            if (executeAsync == null)
            {
                throw new ArgumentNullException(nameof(executeAsync));
            }

            return new ReactiveCommand<Unit, TResult>(canExecute ?? Observable.Return(true), _ => executeAsync(), scheduler ?? RxApp.MainThreadScheduler);
        }

        public static ReactiveCommand<Unit, TResult> CreateTask<TResult>(
            Func<Task<TResult>> executeAsync,
            IObservable<bool> canExecute = null,
            IScheduler scheduler = null)
        {
            return Create(
                () => executeAsync().ToObservable(),
                canExecute,
                scheduler);
        }

        public static ReactiveCommand<TParam, TResult> Create<TParam, TResult>(
                Func<TParam, IObservable<TResult>> executeAsync,
                IObservable<bool> canExecute = null,
                IScheduler scheduler = null) =>
            new ReactiveCommand<TParam, TResult>(canExecute ?? Observable.Return(true), executeAsync, scheduler ?? RxApp.MainThreadScheduler);

        public static ReactiveCommand<TParam, TResult> CreateTask<TParam, TResult>(
                Func<TParam, Task<TResult>> executeAsync,
                IObservable<bool> canExecute = null,
                IScheduler scheduler = null) =>
            Create<TParam, TResult>(
                param => executeAsync(param).ToObservable(),
                canExecute,
                scheduler);

        public static CombinedReactiveCommand<TParam, TResult> CreateCombined<TParam, TResult>(
                IEnumerable<ReactiveCommandBase<TParam, TResult>> childCommands,
                IObservable<bool> canExecute = null,
                IScheduler scheduler = null) =>
            new CombinedReactiveCommand<TParam, TResult>(childCommands, canExecute ?? Observable.Return(true), scheduler ?? RxApp.MainThreadScheduler);
    }

    // non-generic reactive command functionality
    public abstract partial class ReactiveCommand : IDisposable, ICommand
    {
        private EventHandler canExecuteChanged;

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

        event EventHandler ICommand.CanExecuteChanged
        {
            add { this.canExecuteChanged += value; }
            remove { this.canExecuteChanged -= value; }
        }

        bool ICommand.CanExecute(object parameter) =>
            this.ICommandCanExecute(parameter);

        void ICommand.Execute(object parameter) =>
            this.ICommandExecute(parameter);

        protected abstract bool ICommandCanExecute(object parameter);

        protected abstract void ICommandExecute(object parameter);

        protected void OnCanExecuteChanged()
        {
            var handler = this.canExecuteChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    // common functionality to all reactive commands that return a value of type TResult
    public abstract class ReactiveCommandBase<TParam, TResult> : ReactiveCommand, IObservable<TResult>
    {
        public abstract IDisposable Subscribe(IObserver<TResult> observer);

        public abstract IObservable<TResult> ExecuteAsync(TParam parameter = default(TParam));

        protected override bool ICommandCanExecute(object parameter) =>
            this.CanExecute.First();

        protected override void ICommandExecute(object parameter)
        {
            // ensure that null is coerced to default(TParam) so that commands taking value types will use a sensible default if no parameter is supplied
            if (parameter == null)
            {
                parameter = default(TParam);
            }

            if (!(parameter is TParam))
            {
                throw new InvalidOperationException(
                    String.Format(
                        "Command requires parameters of type {0}, but received parameter of type {1}.",
                        typeof(TParam).FullName,
                        parameter.GetType().FullName));
            }

            this.ExecuteAsync((TParam)parameter);
        }
    }

    // a reactive command that executes asynchronously
    public class ReactiveCommand<TParam, TResult> : ReactiveCommandBase<TParam, TResult>
    {
        private readonly Func<TParam, IObservable<TResult>> executeAsync;
        private readonly IScheduler scheduler;
        private readonly Subject<ExecutionInfo> executionInfo;
        private readonly ISubject<ExecutionInfo, ExecutionInfo> synchronizedExecutionInfo;
        private readonly IObservable<bool> isExecuting;
        private readonly IObservable<bool> canExecute;
        private readonly IObservable<TResult> results;
        private readonly ScheduledSubject<Exception> exceptions;
        private readonly IDisposable canExecuteSubscription;

        internal protected ReactiveCommand(
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
            this.synchronizedExecutionInfo = Subject.Synchronize(this.executionInfo, scheduler);
            this.isExecuting = this
                .synchronizedExecutionInfo
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
                .synchronizedExecutionInfo
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
            this.synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateBegin());

            return this
                .executeAsync(parameter)
                .Do(result => this.synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateResult(result)))
                .Catch<TResult, Exception>(
                    ex =>
                    {
                        this.synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateFail());
                        exceptions.OnNext(ex);
                        return Observable.Empty<TResult>();
                    })
                .FirstOrDefaultAsync()
                .RunAsync(CancellationToken.None);
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
    public class CombinedReactiveCommand<TParam, TResult> : ReactiveCommandBase<TParam, IList<TResult>>
    {
        private readonly ReactiveCommand<TParam, IList<TResult>> innerCommand;
        private readonly ScheduledSubject<Exception> exceptions;
        private readonly IDisposable exceptionsSubscription;

        internal protected CombinedReactiveCommand(
            IEnumerable<ReactiveCommandBase<TParam, TResult>> childCommands,
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

            this.innerCommand = new ReactiveCommand<TParam, IList<TResult>>(
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