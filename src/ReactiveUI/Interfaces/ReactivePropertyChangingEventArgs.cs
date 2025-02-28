// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Event arguments for when a property is changing.
/// </summary>
/// <typeparam name="TSender">The sender type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ReactivePropertyChangingEventArgs{TSender}"/> class.
/// </remarks>
/// <param name="sender">The sender.</param>
/// <param name="propertyName">Name of the property.</param>
public class ReactivePropertyChangingEventArgs<TSender>(TSender sender, string? propertyName) : PropertyChangingEventArgs(propertyName), IReactivePropertyChangedEventArgs<TSender>
{
    /// <summary>
    /// Gets the sender which triggered the Reactive property changed event.
    /// </summary>
    /// <inheritdoc/>
    public TSender Sender { get; } = sender;
}
