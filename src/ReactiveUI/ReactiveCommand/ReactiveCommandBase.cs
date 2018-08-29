// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive.Linq;

namespace ReactiveUI
{
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
    /// </remarks>
    /// <typeparam name="TParam">
    /// The type of parameter values passed in during command execution.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the values that are the result of command execution.
    /// </typeparam>
    public abstract class ReactiveCommandBase<TParam, TResult> : ReactiveCommand, IObservable<TResult>
    {
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

        /// <summary>
        /// Gets an observable that, when subscribed, executes this command.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Invoking this method will return a cold (lazy) observable that, when subscribed, will execute the logic
        /// encapsulated by the command. It is worth restating that the returned observable is lazy. Nothing will
        /// happen if you call <c>Execute</c> and neglect to subscribe (directly or indirectly) to the returned observable.
        /// </para>
        /// <para>
        /// If no parameter value is provided, a default value of type <typeparamref name="TParam"/> will be passed into
        /// the execution logic.
        /// </para>
        /// <para>
        /// Any number of subscribers can subscribe to a given execution observable and the execution logic will only
        /// run once. That is, the result is broadcast to those subscribers.
        /// </para>
        /// <para>
        /// In those cases where execution fails, there will be no result value. Instead, the failure will tick through the
        /// <see cref="ReactiveCommand.ThrownExceptions"/> observable.
        /// </para>
        /// </remarks>
        /// <param name="parameter">
        /// The parameter to pass into command execution.
        /// </param>
        /// <returns>
        /// An observable that will tick the single result value if and when it becomes available.
        /// </returns>
        public abstract IObservable<TResult> Execute(TParam parameter = default(TParam));

        /// <inheritdoc/>
        protected override bool ICommandCanExecute(object parameter)
        {
            return CanExecute.FirstAsync().Wait();
        }

        /// <inheritdoc/>
        protected override void ICommandExecute(object parameter)
        {
            // ensure that null is coerced to default(TParam) so that commands taking value types will use a sensible default if no parameter is supplied
            if (parameter == null)
            {
                parameter = default(TParam);
            }

            if (parameter != null && !(parameter is TParam))
            {
                throw new InvalidOperationException(
                    $"Command requires parameters of type {typeof(TParam).FullName}, but received parameter of type {parameter.GetType().FullName}.");
            }

            Execute((TParam)parameter)
                .Catch(Observable<TResult>.Empty)
                .Subscribe();
        }
    }
}
