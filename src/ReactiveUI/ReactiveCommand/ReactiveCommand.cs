// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUI
{
    /// <summary>
    /// Encapsulates a user action behind a reactive interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This non-generic base class defines the creation behavior of the ReactiveCommand's.
    /// </para>
    /// <para>
    /// <see cref="ReactiveCommand{TInput, Output}"/> adds the concept of Input and Output generic types.
    /// The Input is often passed in by the View and it's type is captured as TInput, and the Output is
    /// the result of executing the command which type is captured as TOutput.
    /// </para>
    /// <para>
    /// <see cref="ReactiveCommand{TInput, Output}"/> is <c>IObservable</c> which can be used like any other <c>IObservable</c>.
    /// For example, you can Subscribe() to it like any other observable, and add the output to a List on your view model.
    /// The Unit type is a functional programming construct analogous to void and can be used in cases where you don't
    /// care about either the input and/or output value.
    /// </para>
    /// <para>
    /// Creating synchronous reactive commands:
    /// <code>
    /// <![CDATA[
    /// // A synchronous command taking a parameter and returning nothing.
    /// ReactiveCommand<int, Unit> command = ReactiveCommand.Create<int>(x => Console.WriteLine(x));
    ///
    /// // This outputs 42 to console.
    /// command.Execute(42).Subscribe();
    ///
    /// // A better approach is to invoke a command in response to an Observable<T>.
    /// // InvokeCommand operator respects the command's executability. That is, if
    /// // the command's CanExecute method returns false, InvokeCommand will not
    /// // execute the command when the source observable ticks.
    /// Observable.Return(42).InvokeCommand(command);
    /// ]]>
    /// </code>
    /// </para>
    /// <para>
    /// Creating asynchronous reactive commands:
    /// <code>
    /// <![CDATA[
    /// // An asynchronous command that waits 2 seconds and returns 42.
    /// var command = ReactiveCommand.CreateFromObservable<Unit, int>(
    ///      _ => Observable.Return(42).Delay(TimeSpan.FromSeconds(2))
    /// );
    ///
    /// // Calling the asynchronous reactive command:
    /// // Observable.Return(Unit.Default).InvokeCommand(command);
    ///
    /// // Subscribing to values emitted by the command:
    /// command.Subscribe(Console.WriteLine);
    /// ]]>
    /// </code>
    /// </para>
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same class just generic.")]
    public static class ReactiveCommand
    {
        /// <summary>
        /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}"/> with synchronous execution logic.
        /// </summary>
        /// <param name="execute">
        /// The action to execute whenever the command is executed.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        public static ReactiveCommand<Unit, Unit> Create(
            Action execute,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return new ReactiveCommand<Unit, Unit>(
                _ => Observable.Create<Unit>(
                    observer =>
                    {
                        execute();
                        observer.OnNext(Unit.Default);
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}"/> with synchronous execution logic that returns a value
        /// of type <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="execute">
        /// The function to execute whenever the command is executed.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TResult">
        /// The type of value returned by command executions.
        /// </typeparam>
        public static ReactiveCommand<Unit, TResult> Create<TResult>(
            Func<TResult> execute,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return new ReactiveCommand<Unit, TResult>(
                _ => Observable.Create<TResult>(
                    observer =>
                    {
                        observer.OnNext(execute());
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with synchronous execution logic that takes a parameter of type <typeparamref name="TParam"/>.
        /// </summary>
        /// <param name="execute">
        /// The action to execute whenever the command is executed.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        public static ReactiveCommand<TParam, Unit> Create<TParam>(
            Action<TParam> execute,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return new ReactiveCommand<TParam, Unit>(
                param => Observable.Create<Unit>(
                    observer =>
                    {
                        execute(param);
                        observer.OnNext(Unit.Default);
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with synchronous execution logic that takes a parameter of type <typeparamref name="TParam"/>
        /// and returns a value of type <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="execute">
        /// The function to execute whenever the command is executed.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of value returned by command executions.
        /// </typeparam>
        public static ReactiveCommand<TParam, TResult> Create<TParam, TResult>(
            Func<TParam, TResult> execute,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return new ReactiveCommand<TParam, TResult>(
                param => Observable.Create<TResult>(
                    observer =>
                    {
                        observer.OnNext(execute(param));
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic.
        /// </summary>
        /// <param name="execute">
        /// Provides an observable representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static ReactiveCommand<Unit, TResult> CreateFromObservable<TResult>(
            Func<IObservable<TResult>> execute,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return new ReactiveCommand<Unit, TResult>(
                _ => execute(),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static ReactiveCommand<Unit, TResult> CreateFromTask<TResult>(
            Func<Task<TResult>> execute,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            return CreateFromObservable(
                () => execute().ToObservable(),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a parameterless, cancellable <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static ReactiveCommand<Unit, TResult> CreateFromTask<TResult>(
            Func<CancellationToken, Task<TResult>> execute,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            return CreateFromObservable(
                () => Observable.StartAsync(execute),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        public static ReactiveCommand<Unit, Unit> CreateFromTask(
            Func<Task> execute,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            return CreateFromObservable(
                () => execute().ToObservable(),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a parameterless, cancellable <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        public static ReactiveCommand<Unit, Unit> CreateFromTask(
            Func<CancellationToken, Task> execute,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            return CreateFromObservable(
                () => Observable.StartAsync(execute),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic that takes a parameter of type <typeparamref name="TParam"/>.
        /// </summary>
        /// <param name="execute">
        /// Provides an observable representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static ReactiveCommand<TParam, TResult> CreateFromObservable<TParam, TResult>(
            Func<TParam, IObservable<TResult>> execute,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            return new ReactiveCommand<TParam, TResult>(
                execute,
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic that takes a parameter of type <typeparamref name="TParam"/>.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static ReactiveCommand<TParam, TResult> CreateFromTask<TParam, TResult>(
            Func<TParam, Task<TResult>> execute,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            return CreateFromObservable<TParam, TResult>(
                param => execute(param).ToObservable(),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous, cancellable execution logic that takes a parameter of type <typeparamref name="TParam"/>.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static ReactiveCommand<TParam, TResult> CreateFromTask<TParam, TResult>(
            Func<TParam, CancellationToken, Task<TResult>> execute,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            return CreateFromObservable<TParam, TResult>(
                param => Observable.StartAsync(ct => execute(param, ct)),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous execution logic that takes a parameter of type <typeparamref name="TParam"/>.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        public static ReactiveCommand<TParam, Unit> CreateFromTask<TParam>(
            Func<TParam, Task> execute,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            return CreateFromObservable<TParam, Unit>(
                param => execute(param).ToObservable(),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a <see cref="ReactiveCommand{TParam, TResult}"/> with asynchronous, cancellable execution logic that takes a parameter of type <typeparamref name="TParam"/>.
        /// </summary>
        /// <param name="execute">
        /// Provides a <see cref="Task"/> representing the command's asynchronous execution logic.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution.
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>ReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        public static ReactiveCommand<TParam, Unit> CreateFromTask<TParam>(
            Func<TParam, CancellationToken, Task> execute,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            return CreateFromObservable<TParam, Unit>(
                param => Observable.StartAsync(ct => execute(param, ct)),
                canExecute,
                outputScheduler);
        }

        /// <summary>
        /// Creates a <see cref="CombinedReactiveCommand{TParam, TResult}"/> that composes all the provided child commands.
        /// </summary>
        /// <param name="childCommands">
        /// The child commands that the combined command will compose.
        /// </param>
        /// <param name="canExecute">
        /// An optional observable that dictates the availability of the command for execution (in addition to the availability specified
        /// by each individual child command).
        /// </param>
        /// <param name="outputScheduler">
        /// An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.
        /// </param>
        /// <returns>
        /// The <c>CombinedReactiveCommand</c> instance.
        /// </returns>
        /// <typeparam name="TParam">
        /// The type of the parameter passed through to command execution.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the command's result.
        /// </typeparam>
        public static CombinedReactiveCommand<TParam, TResult> CreateCombined<TParam, TResult>(
            IEnumerable<ReactiveCommandBase<TParam, TResult>> childCommands,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            return new CombinedReactiveCommand<TParam, TResult>(childCommands, canExecute ?? Observables.True, outputScheduler ?? RxApp.MainThreadScheduler);
        }
    }

    /// <summary>
    /// Encapsulates a user interaction behind a reactive interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides the bulk of the actual implementation for reactive commands. You should not create instances
    /// of this class directly, but rather via the static creation methods on the non-generic <see cref="ReactiveCommand"/>
    /// class.
    /// </para>
    /// </remarks>
    /// <typeparam name="TParam">
    /// The type of parameter values passed in during command execution.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the values that are the result of command execution.
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same class just generic.")]
    public class ReactiveCommand<TParam, TResult> : ReactiveCommandBase<TParam, TResult>
    {
        private readonly Func<TParam, IObservable<TResult>> _execute;
        private readonly IScheduler _outputScheduler;
        private readonly Subject<ExecutionInfo> _executionInfo;
        private readonly ISubject<ExecutionInfo, ExecutionInfo> _synchronizedExecutionInfo;
        private readonly IObservable<bool> _isExecuting;
        private readonly IObservable<bool> _canExecute;
        private readonly IObservable<TResult> _results;
        private readonly ScheduledSubject<Exception> _exceptions;
        private readonly IDisposable _canExecuteSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCommand{TParam, TResult}"/> class.
        /// </summary>
        /// <param name="execute">The Func to perform when the command is executed.</param>
        /// <param name="canExecute">A observable which has a value if the command can execute.</param>
        /// <param name="outputScheduler">The scheduler where to send output after the main execution.</param>
        /// <exception cref="ArgumentNullException">Thrown if any dependent parameters are null.</exception>
        protected internal ReactiveCommand(
            Func<TParam, IObservable<TResult>> execute,
            IObservable<bool>? canExecute,
            IScheduler? outputScheduler)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException(nameof(canExecute));
            }

            if (outputScheduler == null)
            {
                throw new ArgumentNullException(nameof(outputScheduler));
            }

            _execute = execute;
            _outputScheduler = outputScheduler;
            _executionInfo = new Subject<ExecutionInfo>();
            _synchronizedExecutionInfo = Subject.Synchronize(_executionInfo, outputScheduler);
            _isExecuting = _synchronizedExecutionInfo
                .Scan(
                    0,
                    (acc, next) =>
                    {
                        if (next.Demarcation == ExecutionDemarcation.Begin)
                        {
                            return acc + 1;
                        }

                        if (next.Demarcation == ExecutionDemarcation.End)
                        {
                            return acc - 1;
                        }

                        return acc;
                    })
                .Select(inFlightCount => inFlightCount > 0)
                .StartWith(false)
                .DistinctUntilChanged()
                .Replay(1)
                .RefCount();
            _canExecute = canExecute
                .Catch<bool, Exception>(ex =>
                {
                    _exceptions.OnNext(ex);
                    return Observables.False;
                })
                .StartWith(false)
                .CombineLatest(_isExecuting, (canEx, isEx) => canEx && !isEx)
                .DistinctUntilChanged()
                .Replay(1)
                .RefCount();
            _results = _synchronizedExecutionInfo
                .Where(x => x.Demarcation == ExecutionDemarcation.Result)
                .Select(x => x.Result);

            _exceptions = new ScheduledSubject<Exception>(outputScheduler, RxApp.DefaultExceptionHandler);

            _canExecuteSubscription = _canExecute.Subscribe(OnCanExecuteChanged);
        }

        private enum ExecutionDemarcation
        {
            Begin,
            Result,
            End
        }

        /// <inheritdoc/>
        public override IObservable<bool> CanExecute => _canExecute;

        /// <inheritdoc/>
        public override IObservable<bool> IsExecuting => _isExecuting;

        /// <inheritdoc/>
        public override IObservable<Exception> ThrownExceptions => _exceptions.AsObservable();

        /// <inheritdoc/>
        public override IDisposable Subscribe(IObserver<TResult> observer)
        {
            return _results.Subscribe(observer);
        }

        /// <inheritdoc/>
        public override IObservable<TResult> Execute(TParam parameter = default(TParam))
        {
            try
            {
                return Observable
                    .Defer(
                        () =>
                        {
                            _synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateBegin());
                            return Observable<TResult>.Empty;
                        })
                    .Concat(_execute(parameter))
                    .Do(result => _synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateResult(result)))
                    .Catch<TResult, Exception>(
                        ex =>
                        {
                            _exceptions.OnNext(ex);
                            return Observable.Throw<TResult>(ex);
                        })
                    .Finally(() => _synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateEnd()))
                    .PublishLast()
                    .RefCount()
                    .ObserveOn(_outputScheduler);
            }
            catch (Exception ex)
            {
                _exceptions.OnNext(ex);
                return Observable.Throw<TResult>(ex);
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _executionInfo?.Dispose();
                _exceptions?.Dispose();
                _canExecuteSubscription?.Dispose();
            }
        }

        private struct ExecutionInfo
        {
            private readonly ExecutionDemarcation _demarcation;
            private readonly TResult _result;

            private ExecutionInfo(ExecutionDemarcation demarcation, TResult result)
            {
                _demarcation = demarcation;
                _result = result;
            }

            public ExecutionDemarcation Demarcation => _demarcation;

            public TResult Result => _result;

            public static ExecutionInfo CreateBegin() =>
                new ExecutionInfo(ExecutionDemarcation.Begin, default!);

            public static ExecutionInfo CreateResult(TResult result) =>
                new ExecutionInfo(ExecutionDemarcation.Result, result);

            public static ExecutionInfo CreateEnd() =>
                new ExecutionInfo(ExecutionDemarcation.End, default!);
        }
    }
}
