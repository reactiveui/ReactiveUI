﻿// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace ReactiveUI;

/// <summary>
/// A base class for generic reactive commands.
/// </summary>
/// <remarks>
/// <para>
/// This class extends <see cref="ReactiveCommand"/> and adds generic type parameters for the parameter values passed
/// into command execution, and the return values of command execution.
/// </para>
/// <para>
/// Because the result type is known by this class, it can implement <see cref="IObservable{T}"/>. However, the implementation
/// is defined as abstract, so subclasses must provide it.
/// </para>
/// <para>
/// Reactive commands encapsulate the behavior of running some execution logic and then surfacing the results on the UI
/// thread. Importantly, no scheduling is performed against input observables (the <c>canExecute</c> and execution pipelines).
/// </para>
/// <para>
/// To create an instance of <c>ReactiveCommand</c>, call one of the static creation methods defined by this class.
/// <see cref="ReactiveCommand.Create"/> can be used when your execution logic is synchronous.
/// ReactiveCommand.CreateFromObservable and
/// ReactiveCommand.CreateFromTask (and overloads) can be used for asynchronous
/// execution logic. Optionally, you can provide an observable that governs the availability of the command for execution,
/// as well as a scheduler to which events will be delivered.
/// </para>
/// <para>
/// The <see cref="CanExecute"/> property provides an observable that can be used to determine whether the command is
/// eligible for execution. The value of this observable is determined by both the <c>canExecute</c> observable provided
/// during command creation, and the current execution status of the command. A command that is already executing will
/// yield <c>false</c> from its <see cref="CanExecute"/> observable regardless of the <c>canExecute</c> observable provided
/// during command creation.
/// </para>
/// <para>
/// The <see cref="IsExecuting"/> property provides an observable whose value indicates whether the command is currently
/// executing. This can be a useful means of triggering UI, such as displaying an activity indicator whilst a command is
/// executing.
/// </para>
/// <para>
/// As discussed above, you are under no obligation to somehow incorporate this into your <c>canExecute</c> observable
/// because that is taken care of for you. That is, if the value of <c>IsExecuting</c> is <c>true</c>, the value of
/// <c>CanExecute</c> will be <c>false</c>. However, if the value of <c>CanExecute</c> is <c>false</c>, that does not imply
/// the value of <c>IsExecuting</c> is <c>true</c>.
/// </para>
/// <para>
/// Any errors in your command's execution logic (including any <c>canExecute</c> observable you choose to provide) will be
/// surfaced via the <see cref="ThrownExceptions"/> observable. This gives you the opportunity to handle the error before
/// it triggers a default handler that tears down the application. For example, you might use this as a means of alerting
/// the user that something has gone wrong executing the command.
/// </para>
/// <para>
/// For the sake of convenience, all <c>ReactiveCommand</c> instances are also implementations of <see cref="ICommand"/>.
/// This allows you to easily integrate instances of <c>ReactiveCommand</c> into platforms that understands <c>ICommand</c>
/// natively (such as WPF and UWP).
/// </para>
/// </remarks>
/// <typeparam name="TParam">
/// The type of parameter values passed in during command execution.
/// </typeparam>
/// <typeparam name="TResult">
/// The type of the values that are the result of command execution.
/// </typeparam>
public abstract class ReactiveCommandBase<TParam, TResult> : IReactiveCommand<TParam, TResult>, ICommand
{
    private EventHandler? _canExecuteChanged;
    private bool _canExecuteValue;

    /// <inheritdoc/>
    event EventHandler? ICommand.CanExecuteChanged
    {
        add => _canExecuteChanged += value;
        remove => _canExecuteChanged -= value;
    }

    /// <inheritdoc />
    public abstract IObservable<bool> CanExecute
    {
        get;
    }

    /// <inheritdoc />
    public abstract IObservable<bool> IsExecuting
    {
        get;
    }

    /// <inheritdoc />
    public abstract IObservable<Exception> ThrownExceptions
    {
        get;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    bool ICommand.CanExecute(object? parameter) => ICommandCanExecute(parameter);

    /// <inheritdoc/>
    void ICommand.Execute(object? parameter) => ICommandExecute(parameter);

    /// <summary>
    /// Subscribes to execution results from this command.
    /// </summary>
    /// <param name="observer">
    /// The observer.
    /// </param>
    /// <returns>
    /// An <see cref="IDisposable"/> that, when disposed, will unsubscribe the observer.
    /// </returns>
    public abstract IDisposable Subscribe(IObserver<TResult> observer);

    /// <inheritdoc/>
    public abstract IObservable<TResult> Execute(TParam parameter);

    /// <inheritdoc/>
    public abstract IObservable<TResult> Execute();

    /// <summary>
    /// Disposes of the managed resources.
    /// </summary>
    /// <param name="disposing">If its getting called by the Dispose() method.</param>
    protected abstract void Dispose(bool disposing);

    /// <summary>
    /// Will trigger a event when the CanExecute condition has changed.
    /// </summary>
    /// <param name="newValue">The new value of the execute.</param>
    protected void OnCanExecuteChanged(bool newValue)
    {
        _canExecuteValue = newValue;
        _canExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Will be called by the methods from the ICommand interface.
    /// This method is called when the Command should evaluate if it can execute.
    /// </summary>
    /// <param name="parameter">The parameter being passed to the ICommand.</param>
    /// <returns>If the command can be executed.</returns>
    protected virtual bool ICommandCanExecute(object? parameter) => _canExecuteValue;

    /// <summary>
    /// Will be called by the methods from the ICommand interface.
    /// This method is called when the Command should execute.
    /// </summary>
    /// <param name="parameter">The parameter being passed to the ICommand.</param>
    protected virtual void ICommandExecute(object? parameter)
    {
        // ensure that null is coerced to default(TParam) so that commands taking value types will use a sensible default if no parameter is supplied
        parameter ??= default(TParam);

        if (parameter is not null && parameter is not TParam)
        {
            throw new InvalidOperationException(
                                                $"Command requires parameters of type {typeof(TParam).FullName}, but received parameter of type {parameter.GetType().FullName}.");
        }

        var result = parameter is null ? Execute() : Execute((TParam)parameter);

        result
            .Catch(Observable<TResult>.Empty)
            .Subscribe();
    }
}
