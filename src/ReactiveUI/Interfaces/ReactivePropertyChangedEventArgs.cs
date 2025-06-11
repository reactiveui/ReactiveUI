// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Event arguments for when a property has changed.
/// Expands on the PropertyChangedEventArgs to add the Sender.
/// </summary>
/// <typeparam name="TSender">The sender type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ReactivePropertyChangedEventArgs{TSender}"/> class.
/// </remarks>
/// <param name="sender">The sender.</param>
/// <param name="propertyName">Name of the property.</param>
public class ReactivePropertyChangedEventArgs<TSender>(TSender sender, string propertyName) : PropertyChangedEventArgs(propertyName), IReactivePropertyChangedEventArgs<TSender>
{
    /// <summary>
    /// Gets the sender which triggered the property changed event.
    /// </summary>
    /// <inheritdoc/>
    public TSender Sender { get; } = sender;
}
