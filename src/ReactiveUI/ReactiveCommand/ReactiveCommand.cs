// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;

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
    /// Creates a parameterless reactive command with synchronous execution logic.
    /// </summary>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, Unit> Create(Action execute) =>
        Create(execute, null, null);

    /// <summary>
    /// Creates a parameterless reactive command with synchronous execution logic.
    /// </summary>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, Unit> Create(
        Action execute,
        IObservable<bool>? canExecute) =>
        Create(execute, canExecute, null);

    /// <summary>
    /// Creates a parameterless reactive command with synchronous execution logic.
    /// </summary>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, Unit> Create(
        Action execute,
        IScheduler? outputScheduler) =>
        Create(execute, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new(
            _ => new SyncExecuteObservable<Unit>(() =>
            {
                execute();
                return Unit.Default;
            }),
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a parameterless reactive command with synchronous execution logic that returns a value of type TResult.
    /// </summary>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, TResult> Create<TResult>(Func<TResult> execute) =>
        Create(execute, null, null);

    /// <summary>
    /// Creates a parameterless reactive command with synchronous execution logic that returns a value of type TResult.
    /// </summary>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, TResult> Create<TResult>(
        Func<TResult> execute,
        IObservable<bool>? canExecute) =>
        Create(execute, canExecute, null);

    /// <summary>
    /// Creates a parameterless reactive command with synchronous execution logic that returns a value of type TResult.
    /// </summary>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, TResult> Create<TResult>(
        Func<TResult> execute,
        IScheduler? outputScheduler) =>
        Create(execute, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new(
            _ => new SyncExecuteObservable<TResult>(execute),
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a reactive command with synchronous execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, Unit> Create<TParam>(Action<TParam> execute) =>
        Create(execute, null, null);

    /// <summary>
    /// Creates a reactive command with synchronous execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, Unit> Create<TParam>(
        Action<TParam> execute,
        IObservable<bool>? canExecute) =>
        Create(execute, canExecute, null);

    /// <summary>
    /// Creates a reactive command with synchronous execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, Unit> Create<TParam>(
        Action<TParam> execute,
        IScheduler? outputScheduler) =>
        Create(execute, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new(
            param => new SyncExecuteObservable<Unit>(() =>
            {
                execute(param);
                return Unit.Default;
            }),
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a reactive command with synchronous execution logic that takes a parameter of type TParam and returns a value of type TResult.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, TResult> Create<TParam, TResult>(Func<TParam, TResult> execute) =>
        Create(execute, null, null);

    /// <summary>
    /// Creates a reactive command with synchronous execution logic that takes a parameter of type TParam and returns a value of type TResult.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, TResult> Create<TParam, TResult>(
        Func<TParam, TResult> execute,
        IObservable<bool>? canExecute) =>
        Create(execute, canExecute, null);

    /// <summary>
    /// Creates a reactive command with synchronous execution logic that takes a parameter of type TParam and returns a value of type TResult.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, TResult> Create<TParam, TResult>(
        Func<TParam, TResult> execute,
        IScheduler? outputScheduler) =>
        Create(execute, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new(
            param => new SyncExecuteObservable<TResult>(() => execute(param)),
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous background execution logic.
    /// </summary>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, Unit> CreateRunInBackground(Action execute) =>
        CreateRunInBackground(execute, null, null, null);

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous background execution logic.
    /// </summary>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, Unit> CreateRunInBackground(
        Action execute,
        IObservable<bool>? canExecute) =>
        CreateRunInBackground(execute, canExecute, null, null);

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous background execution logic.
    /// </summary>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <param name="backgroundScheduler">The background scheduler.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, Unit> CreateRunInBackground(
        Action execute,
        IObservable<bool>? canExecute,
        IScheduler? backgroundScheduler) =>
        CreateRunInBackground(execute, canExecute, backgroundScheduler, null);

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous background execution logic.
    /// </summary>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <param name="backgroundScheduler">The background scheduler.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, Unit> CreateRunInBackground(
        Action execute,
        IScheduler? backgroundScheduler,
        IScheduler? outputScheduler) =>
        CreateRunInBackground(execute, null, backgroundScheduler, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? backgroundScheduler,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservable(
            () => new StartObservable<Unit>(
                () =>
                {
                    execute();
                    return Unit.Default;
                },
                backgroundScheduler ?? RxSchedulers.TaskpoolScheduler),
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous background execution logic that returns a value of type TResult.
    /// </summary>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, TResult> CreateRunInBackground<TResult>(Func<TResult> execute) =>
        CreateRunInBackground(execute, null, null, null);

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous background execution logic that returns a value of type TResult.
    /// </summary>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, TResult> CreateRunInBackground<TResult>(
        Func<TResult> execute,
        IObservable<bool>? canExecute) =>
        CreateRunInBackground(execute, canExecute, null, null);

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous background execution logic that returns a value of type TResult.
    /// </summary>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <param name="backgroundScheduler">The background scheduler.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, TResult> CreateRunInBackground<TResult>(
        Func<TResult> execute,
        IObservable<bool>? canExecute,
        IScheduler? backgroundScheduler) =>
        CreateRunInBackground(execute, canExecute, backgroundScheduler, null);

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous background execution logic that returns a value of type TResult.
    /// </summary>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <param name="backgroundScheduler">The background scheduler.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, TResult> CreateRunInBackground<TResult>(
        Func<TResult> execute,
        IScheduler? backgroundScheduler,
        IScheduler? outputScheduler) =>
        CreateRunInBackground(execute, null, backgroundScheduler, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? backgroundScheduler,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservable(
            () => new StartObservable<TResult>(execute, backgroundScheduler ?? RxSchedulers.TaskpoolScheduler),
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a reactive command with asynchronous background execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, Unit> CreateRunInBackground<TParam>(Action<TParam> execute) =>
        CreateRunInBackground(execute, null, null, null);

    /// <summary>
    /// Creates a reactive command with asynchronous background execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, Unit> CreateRunInBackground<TParam>(
        Action<TParam> execute,
        IObservable<bool>? canExecute) =>
        CreateRunInBackground(execute, canExecute, null, null);

    /// <summary>
    /// Creates a reactive command with asynchronous background execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <param name="backgroundScheduler">The background scheduler.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, Unit> CreateRunInBackground<TParam>(
        Action<TParam> execute,
        IObservable<bool>? canExecute,
        IScheduler? backgroundScheduler) =>
        CreateRunInBackground(execute, canExecute, backgroundScheduler, null);

    /// <summary>
    /// Creates a reactive command with asynchronous background execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">The action to execute whenever the command is executed.</param>
    /// <param name="backgroundScheduler">The background scheduler.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, Unit> CreateRunInBackground<TParam>(
        Action<TParam> execute,
        IScheduler? backgroundScheduler,
        IScheduler? outputScheduler) =>
        CreateRunInBackground(execute, null, backgroundScheduler, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? backgroundScheduler,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservable<TParam, Unit>(
            p =>
                new StartObservable<Unit>(
                    () =>
                    {
                        execute(p);
                        return Unit.Default;
                    },
                    backgroundScheduler ?? RxSchedulers.TaskpoolScheduler),
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a reactive command with asynchronous background execution logic that takes a parameter of type TParam and returns a value of type TResult.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, TResult> CreateRunInBackground<TParam, TResult>(Func<TParam, TResult> execute) =>
        CreateRunInBackground(execute, null, null, null);

    /// <summary>
    /// Creates a reactive command with asynchronous background execution logic that takes a parameter of type TParam and returns a value of type TResult.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, TResult> CreateRunInBackground<TParam, TResult>(
        Func<TParam, TResult> execute,
        IObservable<bool>? canExecute) =>
        CreateRunInBackground(execute, canExecute, null, null);

    /// <summary>
    /// Creates a reactive command with asynchronous background execution logic that takes a parameter of type TParam and returns a value of type TResult.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <param name="backgroundScheduler">The background scheduler.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, TResult> CreateRunInBackground<TParam, TResult>(
        Func<TParam, TResult> execute,
        IObservable<bool>? canExecute,
        IScheduler? backgroundScheduler) =>
        CreateRunInBackground(execute, canExecute, backgroundScheduler, null);

    /// <summary>
    /// Creates a reactive command with asynchronous background execution logic that takes a parameter of type TParam and returns a value of type TResult.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of value returned by command executions.</typeparam>
    /// <param name="execute">The function to execute whenever the command is executed.</param>
    /// <param name="backgroundScheduler">The background scheduler.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, TResult> CreateRunInBackground<TParam, TResult>(
        Func<TParam, TResult> execute,
        IScheduler? backgroundScheduler,
        IScheduler? outputScheduler) =>
        CreateRunInBackground(execute, null, backgroundScheduler, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? backgroundScheduler,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservable<TParam, TResult>(
            p => new StartObservable<TResult>(() => execute(p), backgroundScheduler ?? RxSchedulers.TaskpoolScheduler),
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a combined reactive command that composes all the provided child commands.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="childCommands">The child commands that the combined command will compose.</param>
    /// <returns>The CombinedReactiveCommand instance.</returns>
    public static CombinedReactiveCommand<TParam, TResult> CreateCombined<TParam, TResult>(
        IEnumerable<ReactiveCommandBase<TParam, TResult>> childCommands) =>
        CreateCombined(childCommands, null, null);

    /// <summary>
    /// Creates a combined reactive command that composes all the provided child commands.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="childCommands">The child commands that the combined command will compose.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The CombinedReactiveCommand instance.</returns>
    public static CombinedReactiveCommand<TParam, TResult> CreateCombined<TParam, TResult>(
        IEnumerable<ReactiveCommandBase<TParam, TResult>> childCommands,
        IObservable<bool>? canExecute) =>
        CreateCombined(childCommands, canExecute, null);

    /// <summary>
    /// Creates a combined reactive command that composes all the provided child commands.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="childCommands">The child commands that the combined command will compose.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The CombinedReactiveCommand instance.</returns>
    public static CombinedReactiveCommand<TParam, TResult> CreateCombined<TParam, TResult>(
        IEnumerable<ReactiveCommandBase<TParam, TResult>> childCommands,
        IScheduler? outputScheduler) =>
        CreateCombined(childCommands, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(childCommands);

        return new(childCommands, canExecute, outputScheduler);
    }

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous observable execution logic.
    /// </summary>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides an observable representing the command's asynchronous execution logic.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<Unit, TResult> CreateFromObservable<TResult>(
        Func<IObservable<TResult>> execute) =>
        CreateFromObservable(execute, null, null);

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous observable execution logic.
    /// </summary>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides an observable representing the command's asynchronous execution logic.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<Unit, TResult> CreateFromObservable<TResult>(
        Func<IObservable<TResult>> execute,
        IObservable<bool>? canExecute) =>
        CreateFromObservable(execute, canExecute, null);

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous observable execution logic.
    /// </summary>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides an observable representing the command's asynchronous execution logic.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, TResult> CreateFromObservable<TResult>(
        Func<IObservable<TResult>> execute,
        IScheduler? outputScheduler) =>
        CreateFromObservable(execute, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new(
            _ => execute(),
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a reactive command with asynchronous observable execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides an observable representing the command's asynchronous execution logic.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<TParam, TResult> CreateFromObservable<TParam, TResult>(
        Func<TParam, IObservable<TResult>> execute) =>
        CreateFromObservable(execute, null, null);

    /// <summary>
    /// Creates a reactive command with asynchronous observable execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides an observable representing the command's asynchronous execution logic.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<TParam, TResult> CreateFromObservable<TParam, TResult>(
        Func<TParam, IObservable<TResult>> execute,
        IObservable<bool>? canExecute) =>
        CreateFromObservable(execute, canExecute, null);

    /// <summary>
    /// Creates a reactive command with asynchronous observable execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides an observable representing the command's asynchronous execution logic.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, TResult> CreateFromObservable<TParam, TResult>(
        Func<TParam, IObservable<TResult>> execute,
        IScheduler? outputScheduler) =>
        CreateFromObservable(execute, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new(
            execute,
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous task-based execution logic returning TResult.
    /// </summary>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<Unit, TResult> CreateFromTask<TResult>(
        Func<Task<TResult>> execute) =>
        CreateFromTask(execute, null, null);

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous task-based execution logic returning TResult.
    /// </summary>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<Unit, TResult> CreateFromTask<TResult>(
        Func<Task<TResult>> execute,
        IObservable<bool>? canExecute) =>
        CreateFromTask(execute, canExecute, null);

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous task-based execution logic returning TResult.
    /// </summary>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, TResult> CreateFromTask<TResult>(
        Func<Task<TResult>> execute,
        IScheduler? outputScheduler) =>
        CreateFromTask(execute, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservable(() => new TaskObservable<TResult>(execute()), canExecute, outputScheduler);
    }

    /// <summary>
    /// Creates a parameterless, cancellable reactive command with asynchronous task-based execution logic returning TResult.
    /// </summary>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<Unit, TResult> CreateFromTask<TResult>(
        Func<CancellationToken, Task<TResult>> execute) =>
        CreateFromTask(execute, null, null);

    /// <summary>
    /// Creates a parameterless, cancellable reactive command with asynchronous task-based execution logic returning TResult.
    /// </summary>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<Unit, TResult> CreateFromTask<TResult>(
        Func<CancellationToken, Task<TResult>> execute,
        IObservable<bool>? canExecute) =>
        CreateFromTask(execute, canExecute, null);

    /// <summary>
    /// Creates a parameterless, cancellable reactive command with asynchronous task-based execution logic returning TResult.
    /// </summary>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, TResult> CreateFromTask<TResult>(
        Func<CancellationToken, Task<TResult>> execute,
        IScheduler? outputScheduler) =>
        CreateFromTask(execute, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservableCancellable<Unit, TResult>(
            () => ObservableMixins.FromAsyncWithAllNotifications(execute),
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous task-based execution logic.
    /// </summary>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<Unit, Unit> CreateFromTask(Func<Task> execute) =>
        CreateFromTask(execute, null, null);

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous task-based execution logic.
    /// </summary>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<Unit, Unit> CreateFromTask(
        Func<Task> execute,
        IObservable<bool>? canExecute) =>
        CreateFromTask(execute, canExecute, null);

    /// <summary>
    /// Creates a parameterless reactive command with asynchronous task-based execution logic.
    /// </summary>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, Unit> CreateFromTask(
        Func<Task> execute,
        IScheduler? outputScheduler) =>
        CreateFromTask(execute, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservable(() => new TaskUnitObservable(execute()), canExecute, outputScheduler);
    }

    /// <summary>
    /// Creates a parameterless, cancellable reactive command with asynchronous task-based execution logic.
    /// </summary>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<Unit, Unit> CreateFromTask(Func<CancellationToken, Task> execute) =>
        CreateFromTask(execute, null, null);

    /// <summary>
    /// Creates a parameterless, cancellable reactive command with asynchronous task-based execution logic.
    /// </summary>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<Unit, Unit> CreateFromTask(
        Func<CancellationToken, Task> execute,
        IObservable<bool>? canExecute) =>
        CreateFromTask(execute, canExecute, null);

    /// <summary>
    /// Creates a parameterless, cancellable reactive command with asynchronous task-based execution logic.
    /// </summary>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<Unit, Unit> CreateFromTask(
        Func<CancellationToken, Task> execute,
        IScheduler? outputScheduler) =>
        CreateFromTask(execute, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservableCancellable<Unit, Unit>(
            () => ObservableMixins.FromAsyncWithAllNotifications(execute),
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a reactive command with asynchronous task-based execution logic that takes a parameter of type TParam and returns TResult.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<TParam, TResult> CreateFromTask<TParam, TResult>(
        Func<TParam, Task<TResult>> execute) =>
        CreateFromTask(execute, null, null);

    /// <summary>
    /// Creates a reactive command with asynchronous task-based execution logic that takes a parameter of type TParam and returns TResult.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<TParam, TResult> CreateFromTask<TParam, TResult>(
        Func<TParam, Task<TResult>> execute,
        IObservable<bool>? canExecute) =>
        CreateFromTask(execute, canExecute, null);

    /// <summary>
    /// Creates a reactive command with asynchronous task-based execution logic that takes a parameter of type TParam and returns TResult.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, TResult> CreateFromTask<TParam, TResult>(
        Func<TParam, Task<TResult>> execute,
        IScheduler? outputScheduler) =>
        CreateFromTask(execute, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservable<TParam, TResult>(
            param => new TaskObservable<TResult>(execute(param)),
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a reactive command with asynchronous, cancellable task-based execution logic that takes TParam and returns TResult.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<TParam, TResult> CreateFromTask<TParam, TResult>(
        Func<TParam, CancellationToken, Task<TResult>> execute) =>
        CreateFromTask(execute, null, null);

    /// <summary>
    /// Creates a reactive command with asynchronous, cancellable task-based execution logic that takes TParam and returns TResult.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<TParam, TResult> CreateFromTask<TParam, TResult>(
        Func<TParam, CancellationToken, Task<TResult>> execute,
        IObservable<bool>? canExecute) =>
        CreateFromTask(execute, canExecute, null);

    /// <summary>
    /// Creates a reactive command with asynchronous, cancellable task-based execution logic that takes TParam and returns TResult.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <typeparam name="TResult">The type of the command's result.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, TResult> CreateFromTask<TParam, TResult>(
        Func<TParam, CancellationToken, Task<TResult>> execute,
        IScheduler? outputScheduler) =>
        CreateFromTask(execute, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservableCancellable<TParam, TResult>(
            param => ObservableMixins.FromAsyncWithAllNotifications(ct => execute(param, ct)),
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a reactive command with asynchronous task-based execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<TParam, Unit> CreateFromTask<TParam>(
        Func<TParam, Task> execute) =>
        CreateFromTask(execute, null, null);

    /// <summary>
    /// Creates a reactive command with asynchronous task-based execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<TParam, Unit> CreateFromTask<TParam>(
        Func<TParam, Task> execute,
        IObservable<bool>? canExecute) =>
        CreateFromTask(execute, canExecute, null);

    /// <summary>
    /// Creates a reactive command with asynchronous task-based execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, Unit> CreateFromTask<TParam>(
        Func<TParam, Task> execute,
        IScheduler? outputScheduler) =>
        CreateFromTask(execute, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return CreateFromObservable<TParam, Unit>(
            param => new TaskUnitObservable(execute(param)),
            canExecute,
            outputScheduler);
    }

    /// <summary>
    /// Creates a reactive command with asynchronous, cancellable task-based execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<TParam, Unit> CreateFromTask<TParam>(
        Func<TParam, CancellationToken, Task> execute) =>
        CreateFromTask(execute, null, null);

    /// <summary>
    /// Creates a reactive command with asynchronous, cancellable task-based execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="canExecute">An optional observable that dictates the availability of the command for execution.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    public static ReactiveCommand<TParam, Unit> CreateFromTask<TParam>(
        Func<TParam, CancellationToken, Task> execute,
        IObservable<bool>? canExecute) =>
        CreateFromTask(execute, canExecute, null);

    /// <summary>
    /// Creates a reactive command with asynchronous, cancellable task-based execution logic that takes a parameter of type TParam.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter passed through to command execution.</typeparam>
    /// <param name="execute">Provides a Task representing the command's asynchronous execution logic.</param>
    /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxSchedulers.MainThreadScheduler</c>.</param>
    /// <returns>The ReactiveCommand instance.</returns>
    /// <exception cref="ArgumentNullException">execute.</exception>
    public static ReactiveCommand<TParam, Unit> CreateFromTask<TParam>(
        Func<TParam, CancellationToken, Task> execute,
        IScheduler? outputScheduler) =>
        CreateFromTask(execute, null, outputScheduler);

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
        IObservable<bool>? canExecute,
        IScheduler? outputScheduler)
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
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    internal static ReactiveCommand<TParam, TResult> CreateFromObservableCancellable<TParam, TResult>(
        Func<IObservable<(IObservable<TResult> Result, Action Cancel)>> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(execute);

        return new(
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

        return new(
            execute,
            canExecute,
            outputScheduler);
    }
}
