// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

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
///
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
public static class ReactiveCommand
{
    /// <summary>
    /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}" /> with synchronous execution logic.
    /// </summary>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>
    /// The <c>ReactiveCommand</c> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, Unit> Create(
        Action execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new ReactiveCommand<Unit, Unit>(
                _ => Observable.Create<Unit>(
                observer =>
                {
                    execute();
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }),
                canExecute,
                outputScheduler);
    }

    /// <summary>
    /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}" /> with asynchronous execution logic.
    /// </summary>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <param name="backgroundScheduler">The background scheduler.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>
    /// The <c>ReactiveCommand</c> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, Unit> CreateRunInBackground(
        Action execute,
        IObservable<bool>? canExecute = null,
        IScheduler? backgroundScheduler = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservable(() => Observable.Start(execute, backgroundScheduler ?? RxSchedulers.TaskpoolScheduler), canExecute, outputScheduler);
    }

    /// <summary>
    /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}" /> with synchronous execution logic that returns a value
    /// of type <typeparamref name="TResult" />.
    /// </summary>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>
    /// The <c>ReactiveCommand</c> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, TResult> Create<TResult>(
        Func<TResult> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new ReactiveCommand<Unit, TResult>(
                _ => Observable.Create<TResult>(
                observer =>
                {
                    observer.OnNext(execute());
                    observer.OnCompleted();
                    return Disposable.Empty;
                }),
                canExecute,
                outputScheduler);
    }

    /// <summary>
    /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}" /> with asynchronous execution logic that returns a value
    /// of type <typeparamref name="TResult" />.
    /// </summary>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <param name="backgroundScheduler">The background scheduler.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>
    /// The <c>ReactiveCommand</c> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, TResult> CreateRunInBackground<TResult>(
        Func<TResult> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? backgroundScheduler = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservable(() => Observable.Start(execute, backgroundScheduler ?? RxSchedulers.TaskpoolScheduler), canExecute, outputScheduler);
    }

    /// <summary>
    /// Creates a <see cref="ReactiveCommand{TParam, TResult}" /> with synchronous execution logic that takes a parameter of type <typeparamref name="TParam" />.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>
    /// The <c>ReactiveCommand</c> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, Unit> Create<TParam>(
        Action<TParam> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new ReactiveCommand<TParam, Unit>(
                param => Observable.Create<Unit>(
                observer =>
                {
                    execute(param);
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }),
                canExecute,
                outputScheduler);
    }

    /// <summary>
    /// Creates a <see cref="ReactiveCommand{TParam, TResult}" /> with asynchronous execution logic that takes a parameter of type <typeparamref name="TParam" />.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <param name="backgroundScheduler">The background scheduler.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>
    /// The <c>ReactiveCommand</c> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, Unit> CreateRunInBackground<TParam>(
        Action<TParam> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? backgroundScheduler = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservable<TParam, Unit>(p => Observable.Start(() => execute(p), backgroundScheduler ?? RxSchedulers.TaskpoolScheduler), canExecute, outputScheduler);
    }

    /// <summary>
    /// Creates a <see cref="ReactiveCommand{TParam, TResult}" /> with synchronous execution logic that takes a parameter of type <typeparamref name="TParam" />
    /// and returns a value of type <typeparamref name="TResult" />.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>
    /// The <c>ReactiveCommand</c> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, TResult> Create<TParam, TResult>(
        Func<TParam, TResult> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new ReactiveCommand<TParam, TResult>(
                param => Observable.Create<TResult>(
                    observer =>
                    {
                        observer.OnNext(execute(param));
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }),
                canExecute,
                outputScheduler);
    }

    /// <summary>
    /// Creates a <see cref="ReactiveCommand{TParam, TResult}" /> with asynchronous execution logic that takes a parameter of type <typeparamref name="TParam" />
    /// and returns a value of type <typeparamref name="TResult" />.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <param name="backgroundScheduler">The background scheduler.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>
    /// The <c>ReactiveCommand</c> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, TResult> CreateRunInBackground<TParam, TResult>(
        Func<TParam, TResult> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? backgroundScheduler = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservable<TParam, TResult>(p => Observable.Start(() => execute(p), backgroundScheduler ?? RxSchedulers.TaskpoolScheduler), canExecute, outputScheduler);
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
    /// An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.
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
        ArgumentExceptionHelper.ThrowIfNull(childCommands);

        return new CombinedReactiveCommand<TParam, TResult>(childCommands, canExecute, outputScheduler);
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
    /// An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.
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
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new ReactiveCommand<Unit, TResult>(
                                                  _ => execute(),
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
    /// An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.
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
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new ReactiveCommand<TParam, TResult>(
                                                    execute,
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
    /// An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.
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
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservable(() => execute().ToObservable(), canExecute, outputScheduler);
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
    /// An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.
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
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservableCancellable<Unit, TResult>(() => ObservableMixins.FromAsyncWithAllNotifications(execute), canExecute, outputScheduler);
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
    /// An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.
    /// </param>
    /// <returns>
    /// The <c>ReactiveCommand</c> instance.
    /// </returns>
    public static ReactiveCommand<Unit, Unit> CreateFromTask(
        Func<Task> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservable(() => execute().ToObservable(), canExecute, outputScheduler);
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
    /// An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.
    /// </param>
    /// <returns>
    /// The <c>ReactiveCommand</c> instance.
    /// </returns>
    public static ReactiveCommand<Unit, Unit> CreateFromTask(
        Func<CancellationToken, Task> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservableCancellable<Unit, Unit>(() => ObservableMixins.FromAsyncWithAllNotifications(execute), canExecute, outputScheduler);
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
    /// An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.
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
        ArgumentExceptionHelper.ThrowIfNull(execute);

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
    /// An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.
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
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservableCancellable<TParam, TResult>(
                                                     param => ObservableMixins.FromAsyncWithAllNotifications(ct => execute(param, ct)),
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
    /// An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.
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
        ArgumentExceptionHelper.ThrowIfNull(execute);

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
    /// An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.
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
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservableCancellable<TParam, Unit>(
                                                  param => ObservableMixins.FromAsyncWithAllNotifications(ct => execute(param, ct)),
                                                  canExecute,
                                                  outputScheduler);
    }

    /// <summary>
    /// Creates a parameterless <see cref="ReactiveCommand{TParam, TResult}" /> with asynchronous execution logic.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides an observable representing the command's asynchronous execution logic.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>
    /// The <c>ReactiveCommand</c> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    internal static ReactiveCommand<TParam, TResult> CreateFromObservableCancellable<TParam, TResult>(
        Func<IObservable<(IObservable<TResult> Result, Action Cancel)>> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new ReactiveCommand<TParam, TResult>(
            _ => execute(),
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
    /// An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.
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
    internal static ReactiveCommand<TParam, TResult> CreateFromObservableCancellable<TParam, TResult>(
        Func<TParam, IObservable<(IObservable<TResult> Result, Action Cancel)>> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new ReactiveCommand<TParam, TResult>(
            execute,
            canExecute,
            outputScheduler);
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
[SuppressMessage(
                    "StyleCop.CSharp.MaintainabilityRules",
                    "SA1402:FileMayOnlyContainASingleType",
                    Justification = "Same class just generic.")]
public class ReactiveCommand<TParam, TResult> : ReactiveCommandBase<TParam, TResult>
{
    private readonly IObservable<bool> _canExecute;
    private readonly IDisposable _canExecuteSubscription;
    [SuppressMessage("Design", "CA2213: Dispose member", Justification = "Internal use only")]
    private readonly ScheduledSubject<Exception> _exceptions;
    private readonly Func<TParam, IObservable<(IObservable<TResult> Result, Action Cancel)>> _execute;
    [SuppressMessage("Design", "CA2213: Dispose member", Justification = "Internal use only")]
    private readonly Subject<ExecutionInfo> _executionInfo;
    private readonly IObservable<bool> _isExecuting;
    private readonly IScheduler _outputScheduler;
    private readonly IObservable<TResult> _results;
    private readonly ISubject<ExecutionInfo, ExecutionInfo> _synchronizedExecutionInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCommand{TParam, TResult}" /> class for work
    /// that signals cancellation through a separate callback (as opposed to cancelling by
    /// unsubscribing).
    /// </summary>
    /// <param name="execute">The Func to perform when the command is executed.</param>
    /// <param name="canExecute">A observable which has a value if the command can execute.</param>
    /// <param name="outputScheduler">The scheduler where to send output after the main execution.</param>
    /// <exception cref="ArgumentNullException">
    /// execute.
    /// </exception>
    /// <exception cref="ArgumentNullException">Thrown if any dependent parameters are null.</exception>
    protected internal ReactiveCommand(
        Func<TParam, IObservable<(IObservable<TResult> Result, Action Cancel)>> execute,
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _outputScheduler = outputScheduler ?? RxSchedulers.MainThreadScheduler;
        _exceptions = new ScheduledSubject<Exception>(_outputScheduler, RxState.DefaultExceptionHandler);
        _executionInfo = new Subject<ExecutionInfo>();
        _synchronizedExecutionInfo = Subject.Synchronize(_executionInfo, _outputScheduler);
        _isExecuting = _synchronizedExecutionInfo
            .Scan(
                0,
                (acc, next) => next.Demarcation switch
                {
                    ExecutionDemarcation.Begin => acc + 1,
                    ExecutionDemarcation.End => acc > 0 ? acc - 1 : acc = 0,
                    _ => acc
                })
            .Select(inFlightCount => inFlightCount > 0)
            .StartWith(false)
            .DistinctUntilChanged()
            .Replay(1)
            .RefCount();

        _canExecute = (canExecute ?? Observables.True)
                                .Catch<bool, Exception>(
                                    ex =>
                                    {
                                        _exceptions.OnNext(ex);
                                        return Observables.False;
                                    }).StartWith(false)
                                .CombineLatest(_isExecuting, (canEx, isEx) => canEx && !isEx)
                                .DistinctUntilChanged()
                                .Replay(1)
                                .RefCount();

        _results = _synchronizedExecutionInfo.Where(x => x.Demarcation == ExecutionDemarcation.Result)
                                             .Select(x => x.Result);

        _canExecuteSubscription = _canExecute
                                  .Subscribe(OnCanExecuteChanged);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCommand{TParam, TResult}" /> class.
    /// </summary>
    /// <param name="execute">The Func to perform when the command is executed.</param>
    /// <param name="canExecute">A observable which has a value if the command can execute.</param>
    /// <param name="outputScheduler">The scheduler where to send output after the main execution.</param>
    /// <exception cref="ArgumentNullException">
    /// execute.
    /// </exception>
    /// <exception cref="ArgumentNullException">Thrown if any dependent parameters are null.</exception>
    protected internal ReactiveCommand(
        Func<TParam, IObservable<TResult>> execute,
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
        : this(
            p =>
            {
                var resultObservable = execute(p);
                return Observable.Defer(
                    () =>
                    {
                        var cancelationSubject = new Subject<Unit>();
                        void Cancel() => cancelationSubject.OnNext(Unit.Default);
                        return Observable
                            .Return((resultObservable.TakeUntil(cancelationSubject), (Action)Cancel));
                    });
            },
            canExecute,
            outputScheduler ?? RxSchedulers.MainThreadScheduler)
    {
    }

    /// <summary>
    /// Specifies markers used to indicate the boundaries and result point of an execution process.
    /// </summary>
    /// <remarks>Use this enumeration to identify the start, result, or end of an execution sequence when
    /// processing or tracking execution flow.</remarks>
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
    public override IObservable<TResult> Execute(TParam parameter)
    {
        try
        {
            return Observable.Defer(
                    () =>
                    {
                        _executionInfo.OnNext(ExecutionInfo.CreateBegin());
                        return Observable<(IObservable<TResult>, Action)>.Empty;
                    })
                .Concat(_execute(parameter))
                .SelectMany(sourceAndCancellation =>
                {
                    var (sourceObservable, cancelCallback) = sourceAndCancellation;
                    var sharedSource = sourceObservable.Publish().RefCount(2);

                    // This is the subscription that survives for however long sourceObservable takes to complete (or fail).
                    sharedSource
                        .Do(result => _synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateResult(result)))
                        .Catch<TResult, Exception>(
                            ex =>
                            {
                                _exceptions.OnNext(ex);
                                return Observable.Empty<TResult>();
                            })
                        .Finally(() => _synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateEnd()))
                        .Subscribe();

                    // TODO: Check if it is a problem that we always cancel, even on normal completion!!!
                    return sharedSource.Finally(() => cancelCallback());
                });
        }
        catch (Exception ex)
        {
            _synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateEnd());
            _exceptions.OnNext(ex);
            return Observable.Throw<TResult>(ex);
        }
    }

    /// <inheritdoc/>
    public override IObservable<TResult> Execute() => Execute(default!);

    /// <inheritdoc/>
    public override IDisposable Subscribe(IObserver<TResult> observer) =>
        _results.Subscribe(observer);

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _canExecuteSubscription.Dispose();
    }

    /// <summary>
    /// Represents information about a specific stage and result of an execution process.
    /// </summary>
    /// <remarks>The ExecutionInfo struct encapsulates both the demarcation point within an execution flow and
    /// the associated result value. It is typically used to track or communicate the current execution state and its
    /// outcome in scenarios such as workflow processing or state machines.</remarks>
    private readonly struct ExecutionInfo
    {
        private ExecutionInfo(ExecutionDemarcation demarcation, TResult result)
        {
            Demarcation = demarcation;
            Result = result;
        }

        public ExecutionDemarcation Demarcation { get; }

        public TResult Result { get; }

        public static ExecutionInfo CreateBegin() =>
            new(ExecutionDemarcation.Begin, default!);

        public static ExecutionInfo CreateResult(TResult result) =>
            new(ExecutionDemarcation.Result, result);

        public static ExecutionInfo CreateEnd() =>
            new(ExecutionDemarcation.End, default!);
    }
}
