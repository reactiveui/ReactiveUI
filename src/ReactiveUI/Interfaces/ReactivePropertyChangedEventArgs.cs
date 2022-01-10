// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI;

/// <summary>
/// Event arguments for when a property has changed.
/// Expands on the PropertyChangedEventArgs to add the Sender.
/// </summary>
/// <typeparam name="TSender">The sender type.</typeparam>
public class ReactivePropertyChangedEventArgs<TSender> : PropertyChangedEventArgs, IReactivePropertyChangedEventArgs<TSender>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePropertyChangedEventArgs{TSender}"/> class.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="propertyName">Name of the property.</param>
    public ReactivePropertyChangedEventArgs(TSender sender, string propertyName)
        : base(propertyName) =>
        Sender = sender;

    /// <summary>
    /// Gets the sender which triggered the property changed event.
    /// </summary>
    /// <inheritdoc/>
    public TSender Sender { get; }
}