// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides a singleton instance of the <see cref="DataErrorsChangedEventArgs"/> class for use with data error
/// notifications.
/// </summary>
/// <remarks>This class is intended to reduce allocations by reusing a single <see
/// cref="DataErrorsChangedEventArgs"/> instance when the property name is not relevant or can be standardized. It is
/// typically used in scenarios where frequent error change notifications are required and the property name is not
/// significant.</remarks>
internal static class SingletonDataErrorsChangedEventArgs
{
    /// <summary>
    /// Represents a static instance of the <see cref="DataErrorsChangedEventArgs"/> class for the 'Value' property.
    /// </summary>
    /// <remarks>This instance can be used when raising the <see cref="INotifyDataErrorInfo.ErrorsChanged"/>
    /// event for changes related to the 'Value' property, avoiding the need to create a new <see
    /// cref="DataErrorsChangedEventArgs"/> each time.</remarks>
    public static readonly DataErrorsChangedEventArgs Value = new(nameof(Value));
}
