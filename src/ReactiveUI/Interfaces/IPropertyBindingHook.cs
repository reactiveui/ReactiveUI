// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Implement this as a way to intercept bindings at the time that they are
/// created and execute an additional action (or to cancel the binding).
/// </summary>
public interface IPropertyBindingHook
{
    /// <summary>
    /// Called when any binding is set up.
    /// </summary>
    /// <returns>If false, the binding is cancelled.</returns>
    /// <param name="source">The source ViewModel.</param>
    /// <param name="target">The target View (not the actual control).</param>
    /// <param name="getCurrentViewModelProperties">Get current view model properties.</param>
    /// <param name="getCurrentViewProperties">Get current view properties.</param>
    /// <param name="direction">The Binding direction.</param>
    bool ExecuteHook(
        object? source,
        object target,
        Func<IObservedChange<object, object>[]> getCurrentViewModelProperties,
        Func<IObservedChange<object, object>[]> getCurrentViewProperties,
        BindingDirection direction);
}
