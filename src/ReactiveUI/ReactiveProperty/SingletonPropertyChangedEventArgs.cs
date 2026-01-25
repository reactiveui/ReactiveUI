// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides cached instances of commonly used <see cref="PropertyChangedEventArgs"/> objects for singleton property
/// change notifications.
/// </summary>
/// <remarks>This class is intended to reduce allocations by reusing <see cref="PropertyChangedEventArgs"/>
/// instances for frequently raised property changes. It is typically used in scenarios where property change
/// notifications are raised repeatedly for the same property names, such as in data binding or validation
/// frameworks.</remarks>
internal static class SingletonPropertyChangedEventArgs
{
    /// <summary>
    /// Provides a cached instance of PropertyChangedEventArgs for the Value property.
    /// </summary>
    /// <remarks>Using a cached instance can improve performance by reducing allocations when raising the
    /// PropertyChanged event for the Value property. This field is intended for use when notifying listeners that the
    /// Value property has changed.</remarks>
    public static readonly PropertyChangedEventArgs Value = new(nameof(Value));

    /// <summary>
    /// Provides a static instance of <see cref="PropertyChangedEventArgs"/> for the <see
    /// cref="INotifyDataErrorInfo.HasErrors"/> property.
    /// </summary>
    /// <remarks>This instance can be used to raise the <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// event when the <see cref="INotifyDataErrorInfo.HasErrors"/> property value changes, avoiding the need to create
    /// a new <see cref="PropertyChangedEventArgs"/> object each time.</remarks>
    public static readonly PropertyChangedEventArgs HasErrors = new(nameof(INotifyDataErrorInfo.HasErrors));

    /// <summary>
    /// Provides a PropertyChangedEventArgs instance for the ErrorMessage property.
    /// </summary>
    public static readonly PropertyChangedEventArgs ErrorMessage = new(nameof(ErrorMessage));
}
