// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// A data-only version of IObservedChange.
/// </summary>
/// <typeparam name="TSender">The sender type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ObservedChange{TSender, TValue}"/> class.
/// </remarks>
/// <param name="sender">The sender.</param>
/// <param name="expression">Expression describing the member.</param>
/// <param name="value">The value.</param>
public class ObservedChange<TSender, TValue>(TSender sender, Expression? expression, TValue value) : IObservedChange<TSender, TValue>
{
    /// <inheritdoc/>
    public TSender Sender { get; } = sender;

    /// <inheritdoc/>
    public Expression? Expression { get; } = expression;

    /// <inheritdoc/>
    public TValue Value { get; } = value;
}
