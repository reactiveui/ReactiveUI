// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;

namespace ReactiveUI;

/// <summary>
/// Represents a reactive property that supports value observation, change notification, validation, and cancellation.
/// </summary>
/// <remarks>Implementations of this interface provide a property that notifies observers of value changes,
/// supports error notification for data validation, and allows cancellation of ongoing operations. This interface is
/// commonly used in reactive programming scenarios to enable data binding and validation in UI frameworks.</remarks>
/// <typeparam name="T">The type of the value stored by the reactive property.</typeparam>
public interface IReactiveProperty<T> : IObservable<T?>, ICancelable, INotifyDataErrorInfo, INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the value contained in the current instance.
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// Gets an observable sequence that signals when the collection of errors changes.
    /// </summary>
    /// <remarks>Subscribers receive a notification each time the set of errors is updated. The sequence emits
    /// the current collection of errors, which may be null or empty if there are no errors present.</remarks>
    IObservable<IEnumerable?> ObserveErrorChanged { get; }

    /// <summary>
    /// Gets an observable sequence that signals whether the object currently has validation errors.
    /// </summary>
    /// <remarks>The observable emits a new value whenever the error state changes. Subscribers can use this
    /// to react to validation state updates in real time.</remarks>
    IObservable<bool> ObserveHasErrors { get; }

    /// <summary>
    /// Reloads the current state or data from the underlying source.
    /// </summary>
    void Refresh();
}
