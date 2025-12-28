// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// IObservedChange is a generic interface that is returned from WhenAny()
/// Note that it is used for both Changing (i.e.'before change')
/// and Changed Observables.
/// </summary>
/// <typeparam name="TSender">The sender type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public interface IObservedChange<out TSender, out TValue>
{
    /// <summary>
    /// Gets the object that has raised the change.
    /// </summary>
    TSender Sender { get; }

    /// <summary>
    /// Gets the expression of the member that has changed on Sender.
    /// </summary>
    Expression? Expression { get; }

    /// <summary>
    /// Gets the value of the property that has changed. IMPORTANT NOTE: This
    /// property is often not set for performance reasons, unless you have
    /// explicitly requested an Observable for a property via a method such
    /// as ObservableForProperty. To retrieve the value for the property,
    /// use the GetValue() extension method.
    /// </summary>
    TValue Value { get; }
}
