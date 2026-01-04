// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
public sealed class IROObservableForProperty : ICreatesObservableForProperty
{
    /// <inheritdoc />
    /// <remarks>
    /// This implementation returns a higher affinity than the INPC-based implementation because every
    /// <see cref="IReactiveObject"/> also implements property change notification and should be preferred when available.
    /// </remarks>
    /// <param name="type">The runtime type to query.</param>
    /// <param name="propertyName">The property name to query.</param>
    /// <param name="beforeChanged">
    /// If <see langword="true"/>, indicates the caller requests notifications before the property value changes.
    /// If <see langword="false"/>, indicates after-change notifications.
    /// </param>
    /// <returns>
    /// A positive integer if supported; zero otherwise.
    /// </returns>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
    {
        ArgumentExceptionHelper.ThrowIfNull(type);
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        // NB: Since every IReactiveObject is also an INPC, we need to bind more tightly than INPCObservableForProperty.
        return typeof(IReactiveObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) ? 10 : 0;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="sender"/> does not implement <see cref="IReactiveObject"/>.</exception>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged = false,
        bool suppressWarnings = false)
    {
        ArgumentExceptionHelper.ThrowIfNull(sender);
        ArgumentExceptionHelper.ThrowIfNull(expression);
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        if (sender is not IReactiveObject iro)
        {
            throw new ArgumentException("Sender doesn't implement IReactiveObject", nameof(sender));
        }

        // For indexers, ReactiveObject reports "PropertyName[]".
        var observedName =
            expression.NodeType == ExpressionType.Index
                ? string.Concat(propertyName, "[]")
                : propertyName;

        // Preserve the original comparison semantics.
        const StringComparison comparison = StringComparison.InvariantCulture;

        var source = beforeChanged ? iro.GetChangingObservable() : iro.GetChangedObservable();

        // Keep the projection allocation-free; avoid repeating the same query shape.
        return source
            .Where(x => x.PropertyName is not null && x.PropertyName.Equals(observedName, comparison))
            .Select(static _ => default(object))
            .Select(_ => new ObservedChange<object, object?>(sender, expression, default));
    }
}
