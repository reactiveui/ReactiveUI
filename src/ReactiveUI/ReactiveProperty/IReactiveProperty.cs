// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;

namespace ReactiveUI;

/// <summary>
/// Reactive Property.
/// </summary>
/// <typeparam name="T">The type of the property.</typeparam>
/// <seealso cref="IObservable&lt;T&gt;" />
/// <seealso cref="ICancelable" />
public interface IReactiveProperty<T> : IObservable<T?>, ICancelable, INotifyDataErrorInfo, INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public T? Value { get; set; }

    /// <summary>
    /// Gets the observe error changed.
    /// </summary>
    /// <value>The observe error changed.</value>
    IObservable<IEnumerable?> ObserveErrorChanged { get; }

    /// <summary>
    /// Gets the observe has errors.
    /// </summary>
    /// <value>The observe has errors.</value>
    IObservable<bool> ObserveHasErrors { get; }

    /// <summary>
    /// Refreshes this instance.
    /// </summary>
    void Refresh();
}
