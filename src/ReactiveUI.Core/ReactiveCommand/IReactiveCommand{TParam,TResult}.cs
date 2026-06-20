// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Encapsulates a user action behind a reactive interface.
/// This is for interop inside for the command binding.
/// Not meant for external use due to the fact it doesn't implement ICommand
/// to force the user to favor the Reactive style command execution.
/// </summary>
/// <typeparam name="TParam">
/// The type of parameter values passed in during command execution.
/// </typeparam>
/// <typeparam name="TResult">
/// The type of the values that are the result of command execution.
/// </typeparam>
/// <remarks>
/// <para>
/// This interface extends <see cref="IReactiveCommand"/> and adds generic type parameters for the parameter values passed
/// into command execution, and the return values of command execution.
/// </para>
/// </remarks>
public interface IReactiveCommand<in TParam, out TResult> : IObservable<TResult>, IReactiveCommand
{
    /// <summary>Gets an observable that, when subscribed, executes this command.</summary>
    /// <param name="parameter">The parameter to pass into command execution.</param>
    /// <returns>
    /// An observable that will tick the single result value if and when it becomes available.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Invoking this method will return a cold (lazy) observable that, when subscribed, will execute the logic
    /// encapsulated by the command. It is worth restating that the returned observable is lazy. Nothing will
    /// happen if you call <c>Execute</c> and neglect to subscribe (directly or indirectly) to the returned observable.
    /// </para>
    /// <para>
    /// If no parameter value is provided, a default value of type <typeparamref name="TParam" /> will be passed into
    /// the execution logic.
    /// </para>
    /// <para>
    /// Any number of subscribers can subscribe to a given execution observable and the execution logic will only
    /// run once. That is, the result is broadcast to those subscribers.
    /// </para>
    /// <para>
    /// In those cases where execution fails, there will be no result value. Instead, the failure will tick through the
    /// <see cref="IHandleObservableErrors.ThrownExceptions" /> observable.
    /// </para>
    /// </remarks>
    IObservable<TResult> Execute(TParam parameter);

    /// <summary>Gets an observable that, when subscribed, executes this command.</summary>
    /// <returns>
    /// An observable that will tick the single result value if and when it becomes available.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Invoking this method will return a cold (lazy) observable that, when subscribed, will execute the logic
    /// encapsulated by the command. It is worth restating that the returned observable is lazy. Nothing will
    /// happen if you call <c>Execute</c> and neglect to subscribe (directly or indirectly) to the returned observable.
    /// </para>
    /// <para>
    /// If no parameter value is provided, a default value of type <typeparamref name="TParam" /> will be passed into
    /// the execution logic.
    /// </para>
    /// <para>
    /// Any number of subscribers can subscribe to a given execution observable and the execution logic will only
    /// run once. That is, the result is broadcast to those subscribers.
    /// </para>
    /// <para>
    /// In those cases where execution fails, there will be no result value. Instead, the failure will tick through the
    /// <see cref="IHandleObservableErrors.ThrownExceptions" /> observable.
    /// </para>
    /// </remarks>
    IObservable<TResult> Execute();
}
