// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Reflection;

namespace ReactiveUI;

/// <summary>
/// Generates observables for <see cref="IReactiveObject"/> instances by subscribing to their change notifications.
/// </summary>
/// <remarks>
/// <para>
/// This implementation filters the change stream for a specific property name and projects each matching notification to
/// an <see cref="ObservedChange{TSender,TValue}"/>.
/// </para>
/// <para>
/// Trimming/AOT: <see cref="ICreatesObservableForProperty"/> is annotated for trimming/AOT in this codebase. This type
/// repeats the required annotations on its public members to satisfy the interface contract.
/// </para>
/// </remarks>
[SuppressMessage(
    "Minor Code Smell",
    "S101:Types should be named in PascalCase",
    Justification = "Established public API; renaming is breaking.")]
public sealed class IROObservableForProperty : ICreatesObservableForProperty
{
    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type type, string propertyName) =>
        GetAffinityForObject(type, propertyName, false);

    /// <inheritdoc />
    /// <remarks>
    /// This implementation returns a higher affinity than the INPC-based implementation because every
    /// IReactiveObject also implements property change notification and should be preferred when available.
    /// </remarks>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type? type, string propertyName, bool beforeChanged)
    {
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        if (type is null)
        {
            return 0;
        }

        return typeof(IReactiveObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) ? BindingAffinity.ExactType : 0;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="sender"/> does not implement <see cref="IReactiveObject"/>.</exception>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName) =>
        GetNotificationForProperty(sender, expression, propertyName, false, false);

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged) =>
        GetNotificationForProperty(sender, expression, propertyName, beforeChanged, false);

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged,
        bool suppressWarnings)
    {
        ArgumentExceptionHelper.ThrowIfNull(sender);
        ArgumentExceptionHelper.ThrowIfNull(expression);
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        if (sender is not IReactiveObject iro)
        {
            throw new ArgumentException("Sender doesn't implement IReactiveObject", nameof(sender));
        }

        var observedName =
            expression.NodeType == ExpressionType.Index
                ? $"{propertyName}[]"
                : propertyName;

        var source = beforeChanged ? iro.GetChangingObservable() : iro.GetChangedObservable();

        return new FilteredChange(source, sender, expression, observedName);
    }

    /// <summary>
    /// A single-layer observable that filters a reactive object's change stream to the observed property name and
    /// projects each matching notification into an observed change, with no intermediate operators.
    /// </summary>
    /// <param name="source">The reactive object's change notifications.</param>
    /// <param name="sender">The object surfaced on the observed change.</param>
    /// <param name="expression">The expression surfaced on the observed change.</param>
    /// <param name="observedName">The observed property name.</param>
    private sealed class FilteredChange(
        IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> source,
        object sender,
        Expression expression,
        string observedName) : IObservable<IObservedChange<object, object?>>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IObservedChange<object, object?>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Observer(observer, sender, expression, observedName));
        }

        /// <summary>Filters change-args by name and forwards a projected observed change.</summary>
        /// <param name="downstream">The observer receiving observed changes.</param>
        /// <param name="sender">The object surfaced on the observed change.</param>
        /// <param name="expression">The expression surfaced on the observed change.</param>
        /// <param name="observedName">The observed property name.</param>
        private sealed class Observer(
            IObserver<IObservedChange<object, object?>> downstream,
            object sender,
            Expression expression,
            string observedName) : IObserver<IReactivePropertyChangedEventArgs<IReactiveObject>>
        {
            /// <inheritdoc/>
            public void OnNext(IReactivePropertyChangedEventArgs<IReactiveObject> value)
            {
                if (!string.Equals(value.PropertyName, observedName, StringComparison.InvariantCulture))
                {
                    return;
                }

                downstream.OnNext(new ObservedChange<object, object?>(sender, expression, null));
            }

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }
}
