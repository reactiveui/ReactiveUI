// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// ICreatesObservableForProperty represents an object that knows how to
/// create notifications for a given type of object. Implement this if you
/// are porting RxUI to a new UI toolkit, or generally want to enable WhenAny
/// for another type of object that can be observed in a unique way.
/// </summary>
public interface ICreatesObservableForProperty : IEnableLogger
{
    /// <summary>
    /// Returns a positive integer when this class supports
    /// GetNotificationForProperty for this particular Type. If the method
    /// isn't supported at all, return a non-positive integer. When multiple
    /// implementations return a positive value, the host will use the one
    /// which returns the highest value. When in doubt, return '2' or '0'.
    /// </summary>
    /// <param name="type">The type to query for.</param>
    /// <param name="propertyName">The property of the type to query for.</param>
    /// <param name="beforeChanged">If true, returns whether GNFP is supported before a change occurs.</param>
    /// <returns>A positive integer if GNFP is supported, zero or a negative
    /// value otherwise.</returns>
    int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false);

    /// <summary>
    /// Subscribe to notifications on the specified property, given an
    /// object and a property name.
    /// </summary>
    /// <param name="sender">The object to observe.</param>
    /// <param name="expression">The expression on the object to observe.
    ///     This will be either a MemberExpression or an IndexExpression
    ///     depending on the property.
    /// </param>
    /// <param name="propertyName">The property of the type to query for.</param>
    /// <param name="beforeChanged">If true, signal just before the
    ///     property value actually changes. If false, signal after the
    ///     property changes.</param>
    /// <param name="suppressWarnings">If true, no warnings should be logged.</param>
    /// <returns>An IObservable which is signaled whenever the specified
    /// property on the object changes. If this cannot be done for a
    /// specified value of beforeChanged, return Observable.Never.</returns>
    IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false);
}