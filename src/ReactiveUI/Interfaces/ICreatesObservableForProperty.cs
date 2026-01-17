// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// <see cref="ICreatesObservableForProperty"/> represents a component that can produce change notifications for a
/// given property on a given object.
/// </summary>
/// <remarks>
/// Implementations are typically platform-specific (e.g., a UI toolkit) but this interface must remain platform-agnostic.
/// </remarks>
public interface ICreatesObservableForProperty : IEnableLogger
{
    /// <summary>
    /// Returns a positive integer when this instance supports <see cref="GetNotificationForProperty"/> for
    /// the specified <paramref name="type"/> and <paramref name="propertyName"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the method is not supported, return a non-positive integer.
    /// When multiple implementations return a positive value, the host selects the highest value.
    /// </para>
    /// <para>
    /// Implementations should avoid expensive work here; this is typically a hot-path query.
    /// </para>
    /// </remarks>
    /// <param name="type">The runtime type to query.</param>
    /// <param name="propertyName">The property name to query.</param>
    /// <param name="beforeChanged">
    /// If <see langword="true"/>, indicates the caller requests notifications before the property value changes.
    /// If <see langword="false"/>, indicates after-change notifications.
    /// </param>
    /// <returns>
    /// A positive integer if supported; zero or a negative value otherwise.
    /// </returns>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false);

    /// <summary>
    /// Subscribes to change notifications for the specified <paramref name="propertyName"/> on <paramref name="sender"/>.
    /// </summary>
    /// <param name="sender">The object to observe.</param>
    /// <param name="expression">
    /// The expression describing the observed member.
    /// This is typically a <c>MemberExpression</c> or an <c>IndexExpression</c>.
    /// </param>
    /// <param name="propertyName">The property name to observe.</param>
    /// <param name="beforeChanged">
    /// If <see langword="true"/>, signal before the property value changes; otherwise signal after the change.
    /// </param>
    /// <param name="suppressWarnings">If <see langword="true"/>, warnings should not be logged.</param>
    /// <returns>
    /// An observable that produces an <see cref="IObservedChange{TSender,TValue}"/> whenever the observed property changes.
    /// If observing is not possible for the specified <paramref name="beforeChanged"/> value, implementations should return
    /// an observable that never produces values.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="sender"/> is not compatible with the observing mechanism implemented by the instance.
    /// </exception>
    /// <remarks>
    /// The <paramref name="expression"/> describes the observed member and is used to populate
    /// <see cref="IObservedChange{TSender,TValue}"/> instances emitted by the returned observable.
    /// </remarks>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged = false,
        bool suppressWarnings = false);
}
