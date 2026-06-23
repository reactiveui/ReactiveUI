// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Windows.Input;
using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Extension methods associated with the ReactiveCommand class.</summary>
/// <remarks>
/// <para>
/// <c>InvokeCommand</c> is typically chained after an <see cref="IObservable{T}"/> that represents user intent. It
/// forwards each value into an <see cref="ICommand"/> once <see cref="ICommand.CanExecute(object?)"/> returns true,
/// keeping the observable and command lifetimes aligned via the returned disposable. Each overload is implemented as a
/// small tailored sink that tracks the current command / can-execute state and invokes on each source value when the
/// command is currently able to execute; there is no operator chain.
/// </para>
/// </remarks>
public static class ReactiveCommandMixins
{
    /// <summary>Provides InvokeCommand extension members for <see cref="IObservable{T}"/> sequences.</summary>
    /// <typeparam name="T">The type of the elements in the observable sequence and the parameter type for the command.</typeparam>
    /// <param name="item">The observable sequence whose elements are passed to the command as parameters.</param>
    extension<T>(IObservable<T> item)
    {
        /// <summary>
        /// Subscribes to the observable sequence and invokes the specified command for each element, if the command can
        /// execute with the element as its parameter.
        /// </summary>
        /// <param name="command">The command to invoke for each element. Executed only if its <see cref="ICommand.CanExecute(object?)"/> returns true. May be null.</param>
        /// <returns>An <see cref="IDisposable"/> that unsubscribes from the sequence and stops invoking the command.</returns>
        public IDisposable InvokeCommand(ICommand? command) =>
            item.Subscribe(new DelegateObserver<T>(value =>
            {
                if (command?.CanExecute(value) != true)
                {
                    return;
                }

                command.Execute(value);
            }));

        /// <summary>
        /// Subscribes to the observable sequence and invokes the specified reactive command for each element, if the
        /// command can execute.
        /// </summary>
        /// <typeparam name="TResult">The type of the result produced by the reactive command.</typeparam>
        /// <param name="command">The reactive command to invoke for each element. Cannot be null.</param>
        /// <returns>An <see cref="IDisposable"/> that unsubscribes from the sequence and stops invoking the command.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="command"/> is null.</exception>
        public IDisposable InvokeCommand<TResult>(
            ReactiveCommandBase<T, TResult>? command) =>
            command is null
                ? throw new ArgumentNullException(nameof(command))
                : new ReactiveCommandInvoker<T, TResult>(command).Run(item);

        /// <summary>
        /// Subscribes to the observable sequence and invokes the command found on the target object for each element, if
        /// the command can execute with that element.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target object that contains the command property.</typeparam>
        /// <param name="target">The target object that contains the command property. Can be null.</param>
        /// <param name="commandProperty">An expression identifying the command property on the target object.</param>
        /// <returns>An <see cref="IDisposable"/> that unsubscribes from the sequence and stops invoking the command.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable InvokeCommand<TTarget>(
            TTarget? target,
            Expression<Func<TTarget, ICommand?>> commandProperty)
            where TTarget : class =>
            new CommandPropertyInvoker<T>(target.WhenAnyValue(commandProperty)).Run(item);

        /// <summary>
        /// Subscribes to the observable sequence and invokes the reactive command found on the target object for each
        /// element, if the command can execute.
        /// </summary>
        /// <typeparam name="TResult">The type of the result produced by the reactive command.</typeparam>
        /// <typeparam name="TTarget">The type of the target object that contains the reactive command.</typeparam>
        /// <param name="target">The target object that contains the reactive command. Can be null.</param>
        /// <param name="commandProperty">An expression identifying the reactive command property on the target object.</param>
        /// <returns>An <see cref="IDisposable"/> that unsubscribes from the sequence and stops invoking the command.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IDisposable InvokeCommand<TResult, TTarget>(
            TTarget? target,
            Expression<Func<TTarget, ReactiveCommandBase<T, TResult>?>> commandProperty)
            where TTarget : class =>
            new ReactiveCommandPropertyInvoker<T, TResult>(target.WhenAnyValue(commandProperty)).Run(item);
    }

    /// <summary>Tracks the latest can-execute state of a reactive command and executes it for each source value while it can.</summary>
    /// <typeparam name="T">The command parameter type.</typeparam>
    /// <typeparam name="TResult">The command result type.</typeparam>
    /// <param name="command">The command to invoke.</param>
    private sealed class ReactiveCommandInvoker<T, TResult>(ReactiveCommandBase<T, TResult> command) : IDisposable
    {
        /// <summary>Guards the latest can-execute value.</summary>
#if NET9_0_OR_GREATER
        private readonly Lock _gate = new();
#else
        private readonly object _gate = new();
#endif

        /// <summary>Subscription to the command's can-execute observable.</summary>
        private IDisposable? _canExecuteSubscription;

        /// <summary>Subscription to the source sequence.</summary>
        private IDisposable? _itemSubscription;

        /// <summary>The latest can-execute value.</summary>
        private bool _canExecute;

        /// <summary>Starts tracking can-execute and the source sequence.</summary>
        /// <param name="item">The source sequence.</param>
        /// <returns>A disposable stopping both subscriptions.</returns>
        public ReactiveCommandInvoker<T, TResult> Run(IObservable<T> item)
        {
            _canExecuteSubscription = command.CanExecute.Subscribe(new DelegateObserver<bool>(OnCanExecute));
            _itemSubscription = item.Subscribe(new DelegateObserver<T>(OnItem));
            return this;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _canExecuteSubscription?.Dispose();
            _itemSubscription?.Dispose();
        }

        /// <summary>Records the latest can-execute value.</summary>
        /// <param name="value">The can-execute value.</param>
        private void OnCanExecute(bool value)
        {
            lock (_gate)
            {
                _canExecute = value;
            }
        }

        /// <summary>Executes the command for a source value when it can currently execute.</summary>
        /// <param name="value">The source value passed to the command.</param>
        private void OnItem(T value)
        {
            bool canExecute;
            lock (_gate)
            {
                canExecute = _canExecute;
            }

            if (!canExecute)
            {
                return;
            }

            _ = command.Execute(value).Subscribe(new DelegateObserver<TResult>(static _ => { }, static _ => { }));
        }
    }

    /// <summary>
    /// Tracks the latest <see cref="ICommand"/> exposed by a target property and invokes it for each source value
    /// when it can execute with that value.
    /// </summary>
    /// <typeparam name="T">The command parameter type.</typeparam>
    /// <param name="commands">The stream of current commands.</param>
    private sealed class CommandPropertyInvoker<T>(IObservable<ICommand?> commands) : IDisposable
    {
        /// <summary>Guards the latest command reference.</summary>
#if NET9_0_OR_GREATER
        private readonly Lock _gate = new();
#else
        private readonly object _gate = new();
#endif

        /// <summary>Subscription to the command-property stream.</summary>
        private IDisposable? _commandSubscription;

        /// <summary>Subscription to the source sequence.</summary>
        private IDisposable? _itemSubscription;

        /// <summary>The latest command exposed by the property.</summary>
        private ICommand? _command;

        /// <summary>Starts tracking the command property and the source sequence.</summary>
        /// <param name="item">The source sequence.</param>
        /// <returns>A disposable stopping both subscriptions.</returns>
        public CommandPropertyInvoker<T> Run(IObservable<T> item)
        {
            _commandSubscription = commands.Subscribe(new DelegateObserver<ICommand?>(OnCommand));
            _itemSubscription = item.Subscribe(new DelegateObserver<T>(OnItem));
            return this;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _commandSubscription?.Dispose();
            _itemSubscription?.Dispose();
        }

        /// <summary>Records the latest command exposed by the property.</summary>
        /// <param name="command">The current command.</param>
        private void OnCommand(ICommand? command)
        {
            lock (_gate)
            {
                _command = command;
            }
        }

        /// <summary>Executes the current command for a source value when it can execute with that value.</summary>
        /// <param name="value">The source value passed to the command.</param>
        private void OnItem(T value)
        {
            ICommand? command;
            lock (_gate)
            {
                command = _command;
            }

            if (command?.CanExecute(value) != true)
            {
                return;
            }

            command.Execute(value);
        }
    }

    /// <summary>
    /// Tracks the latest reactive command exposed by a target property and its can-execute state, invoking it for
    /// each source value while it can.
    /// </summary>
    /// <typeparam name="T">The command parameter type.</typeparam>
    /// <typeparam name="TResult">The command result type.</typeparam>
    /// <param name="commands">The stream of current commands.</param>
    private sealed class ReactiveCommandPropertyInvoker<T, TResult>(IObservable<ReactiveCommandBase<T, TResult>?> commands) : IDisposable
    {
        /// <summary>Guards the latest command reference and its can-execute value.</summary>
#if NET9_0_OR_GREATER
        private readonly Lock _gate = new();
#else
        private readonly object _gate = new();
#endif

        /// <summary>The current command's can-execute subscription; swapped when the command changes.</summary>
        private readonly SwapDisposable _canExecuteSubscription = new();

        /// <summary>Subscription to the command-property stream.</summary>
        private IDisposable? _commandSubscription;

        /// <summary>Subscription to the source sequence.</summary>
        private IDisposable? _itemSubscription;

        /// <summary>The latest command exposed by the property.</summary>
        private ReactiveCommandBase<T, TResult>? _command;

        /// <summary>The latest can-execute value of the current command.</summary>
        private bool _canExecute;

        /// <summary>Starts tracking the command property and the source sequence.</summary>
        /// <param name="item">The source sequence.</param>
        /// <returns>A disposable stopping every subscription.</returns>
        public ReactiveCommandPropertyInvoker<T, TResult> Run(IObservable<T> item)
        {
            _commandSubscription = commands.Subscribe(new DelegateObserver<ReactiveCommandBase<T, TResult>?>(OnCommand));
            _itemSubscription = item.Subscribe(new DelegateObserver<T>(OnItem));
            return this;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _commandSubscription?.Dispose();
            _itemSubscription?.Dispose();
            _canExecuteSubscription.Dispose();
        }

        /// <summary>Tracks the latest command and switches the can-execute subscription to it.</summary>
        /// <param name="command">The current command.</param>
        private void OnCommand(ReactiveCommandBase<T, TResult>? command)
        {
            lock (_gate)
            {
                _command = command;
                _canExecute = false;
            }

            _canExecuteSubscription.Disposable =
                command?.CanExecute.Subscribe(new DelegateObserver<bool>(OnCanExecute));
        }

        /// <summary>Records the latest can-execute value of the current command.</summary>
        /// <param name="value">The can-execute value.</param>
        private void OnCanExecute(bool value)
        {
            lock (_gate)
            {
                _canExecute = value;
            }
        }

        /// <summary>Executes the current command for a source value when it can currently execute.</summary>
        /// <param name="value">The source value passed to the command.</param>
        private void OnItem(T value)
        {
            ReactiveCommandBase<T, TResult>? command;
            bool canExecute;
            lock (_gate)
            {
                command = _command;
                canExecute = _canExecute;
            }

            if (command is null || !canExecute)
            {
                return;
            }

            _ = command.Execute(value).Subscribe(new DelegateObserver<TResult>(static _ => { }, static _ => { }));
        }
    }
}
