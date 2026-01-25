// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace ReactiveUI;

/// <summary>
/// Extension methods associated with the ReactiveCommand class.
/// </summary>
/// <remarks>
/// <para>
/// <c>InvokeCommand</c> is typically chained after an <see cref="IObservable{T}"/> that represents user intent. It
/// forwards each value into an <see cref="ICommand"/> once <see cref="ICommand.CanExecute(object?)"/> returns true,
/// keeping the observable and command lifetimes aligned via the returned disposable.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// this.WhenAnyValue(x => x.ViewModel.SaveCommand)
///     .Select(_ => Unit.Default)
///     .InvokeCommand(ViewModel.SaveCommand)
///     .DisposeWith(disposables);
/// ]]>
/// </code>
/// </example>
public static class ReactiveCommandMixins
{
    /// <summary>
    /// Subscribes to the observable sequence and invokes the specified command for each element, if the command can
    /// execute with the element as its parameter.
    /// </summary>
    /// <remarks>The command's CanExecuteChanged event is monitored to ensure that the command is only
    /// executed when it is able to do so. If the command is null, no action is taken for elements in the sequence.
    /// Disposing the returned IDisposable will unsubscribe from the sequence and stop further command
    /// invocations.</remarks>
    /// <typeparam name="T">The type of the elements in the observable sequence and the parameter type for the command.</typeparam>
    /// <param name="item">The observable sequence whose elements are passed to the command as parameters.</param>
    /// <param name="command">The command to invoke for each element in the sequence. The command is executed only if its CanExecute method
    /// returns true for the element. This parameter can be null.</param>
    /// <returns>An IDisposable object that can be used to unsubscribe from the observable sequence and stop invoking the
    /// command.</returns>
    public static IDisposable InvokeCommand<T>(this IObservable<T> item, ICommand? command)
    {
        var canExecuteChanged = Observable.FromEvent<EventHandler, Unit>(
                                            eventHandler =>
                                            {
                                                void Handler(object? sender, EventArgs e) => eventHandler(Unit.Default);
                                                return Handler;
                                            },
                                            h => command!.CanExecuteChanged += h,
                                            h => command!.CanExecuteChanged -= h)
                                          .StartWith(Unit.Default);

        return WithLatestFromFixed(item, canExecuteChanged, (value, _) => new InvokeCommandInfo<ICommand?, T>(command, command!.CanExecute(value), value))
               .Where(ii => ii.CanExecute)
               .Do(ii => command?.Execute(ii.Value))
               .Subscribe();
    }

    /// <summary>
    /// Subscribes to the observable sequence and invokes the specified reactive command for each element, if the
    /// command can execute.
    /// </summary>
    /// <remarks>The command is only executed for elements where its CanExecute observable returns true. If
    /// the command's execution results in an error, the error is suppressed and processing continues with subsequent
    /// elements.</remarks>
    /// <typeparam name="T">The type of the elements in the source observable sequence.</typeparam>
    /// <typeparam name="TResult">The type of the result produced by the reactive command.</typeparam>
    /// <param name="item">The source observable sequence whose elements are used as input to the command.</param>
    /// <param name="command">The reactive command to invoke for each element in the sequence. Cannot be null.</param>
    /// <returns>An IDisposable that can be disposed to unsubscribe from the sequence and stop invoking the command.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the command parameter is null.</exception>
    public static IDisposable InvokeCommand<T, TResult>(this IObservable<T> item, ReactiveCommandBase<T, TResult>? command) =>
        command is null
            ? throw new ArgumentNullException(nameof(command))
            : WithLatestFromFixed(item, command.CanExecute, (value, canExecute) => new InvokeCommandInfo<ReactiveCommandBase<T, TResult>, T>(command, canExecute, value))
              .Where(ii => ii.CanExecute)
              .SelectMany(ii => command.Execute(ii.Value).Catch(Observable<TResult>.Empty))
              .Subscribe();

    /// <summary>
    /// Subscribes to the observable sequence and invokes the specified command on the target object whenever a new
    /// value is emitted, if the command can execute with that value.
    /// </summary>
    /// <remarks>The command is only executed if it is not null and its CanExecute method returns true for the
    /// emitted value. The subscription listens for changes to the command property and to the command's
    /// CanExecuteChanged event. This method uses reflection to evaluate the command property expression, which may be
    /// affected by trimming in some deployment scenarios.</remarks>
    /// <typeparam name="T">The type of the values emitted by the observable sequence.</typeparam>
    /// <typeparam name="TTarget">The type of the target object that contains the command property. Must be a reference type.</typeparam>
    /// <param name="item">The observable sequence whose emitted values will be passed to the command as parameters.</param>
    /// <param name="target">The target object that contains the command property. Can be null.</param>
    /// <param name="commandProperty">An expression that identifies the command property on the target object to be invoked. The expression should
    /// return an object implementing ICommand.</param>
    /// <returns>An IDisposable that can be used to unsubscribe from the observable sequence and stop invoking the command.</returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IDisposable InvokeCommand<T, TTarget>(this IObservable<T> item, TTarget? target, Expression<Func<TTarget, ICommand?>> commandProperty)
        where TTarget : class
    {
        var commandObs = target.WhenAnyValue(commandProperty);
        var commandCanExecuteChanged = commandObs
                                       .Select(command => command is null ?
                                            Observable<ICommand>.Empty :
                                            Observable.FromEvent<EventHandler, ICommand>(
                                                eventHandler => (_, _) => eventHandler(command),
                                                h => command.CanExecuteChanged += h,
                                                h => command.CanExecuteChanged -= h)
                                                .StartWith(command))
                                       .Switch();

        return WithLatestFromFixed(item, commandCanExecuteChanged, (value, cmd) => new InvokeCommandInfo<ICommand, T>(cmd, cmd.CanExecute(value), value))
               .Where(ii => ii.CanExecute)
               .Do(ii => ii.Command.Execute(ii.Value))
               .Subscribe();
    }

    /// <summary>
    /// Subscribes to the specified observable and invokes a reactive command on the target object whenever a new value
    /// is emitted.
    /// </summary>
    /// <remarks>The command is only executed if it is not null and its CanExecute observable returns <see
    /// langword="true"/> for the current value. If the command is null or cannot execute, no action is taken for that
    /// value. This method uses reflection to evaluate the command property expression, which may be affected by
    /// trimming in some deployment scenarios.</remarks>
    /// <typeparam name="T">The type of the values emitted by the source observable.</typeparam>
    /// <typeparam name="TResult">The type of the result produced by the reactive command.</typeparam>
    /// <typeparam name="TTarget">The type of the target object that contains the reactive command.</typeparam>
    /// <param name="item">The observable sequence whose emitted values will be passed to the command for execution.</param>
    /// <param name="target">The target object that contains the reactive command to be invoked. Can be null.</param>
    /// <param name="commandProperty">An expression that identifies the reactive command property on the target object to be invoked.</param>
    /// <returns>An IDisposable that can be disposed to unsubscribe from the observable and stop invoking the command.</returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IDisposable InvokeCommand<T, TResult, TTarget>(this IObservable<T> item, TTarget? target, Expression<Func<TTarget, ReactiveCommandBase<T, TResult>?>> commandProperty)
        where TTarget : class
    {
        var command = target.WhenAnyValue(commandProperty);
        var invocationInfo = command
                             .Select(cmd => cmd is null ?
                                Observable<InvokeCommandInfo<ReactiveCommandBase<T, TResult>, T>>.Empty :
                                cmd
                                    .CanExecute
                                    .Select(canExecute => new InvokeCommandInfo<ReactiveCommandBase<T, TResult>, T>(cmd, canExecute)))
                             .Switch();

        return WithLatestFromFixed(item, invocationInfo, (value, ii) => ii.WithValue(value))
               .Where(ii => ii.CanExecute)
               .SelectMany(ii => ii.Command.Execute(ii.Value).Catch(Observable<TResult>.Empty))
               .Subscribe();
    }

    // See https://github.com/Reactive-Extensions/Rx.NET/issues/444
    private static IObservable<TResult> WithLatestFromFixed<TLeft, TRight, TResult>(
        IObservable<TLeft> item,
        IObservable<TRight> other,
        Func<TLeft, TRight, TResult> resultSelector) =>
        item
            .Publish(
                     os =>
                         other
                             .Select(
                                     a =>
                                         os
                                             .Select(b => resultSelector(b, a)))
                             .Switch());

    /// <summary>
    /// Represents the result of invoking a command, including the command instance, whether it can be executed, and an
    /// associated value.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command being invoked.</typeparam>
    /// <typeparam name="TValue">The type of the value associated with the command invocation.</typeparam>
    private readonly struct InvokeCommandInfo<TCommand, TValue>
    {
        public InvokeCommandInfo(TCommand command, bool canExecute, TValue value)
        {
            Command = command;
            CanExecute = canExecute;
            Value = value!;
        }

        public InvokeCommandInfo(TCommand command, bool canExecute)
        {
            Command = command;
            CanExecute = canExecute;
            Value = default!;
        }

        public TCommand Command { get; }

        public bool CanExecute { get; }

        public TValue Value { get; }

        public InvokeCommandInfo<TCommand, TValue> WithValue(TValue value) =>
            new(Command, CanExecute, value);
    }
}
