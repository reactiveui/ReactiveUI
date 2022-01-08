// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq;

namespace ReactiveUI;

/// <summary>
/// Extension methods associated with the RoutingState class.
/// </summary>
public static class RoutingStateMixins
{
    /// <summary>
    /// Locate the first ViewModel in the stack that matches a certain Type.
    /// </summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="item">The routing state.</param>
    /// <returns>The matching ViewModel or null if none exists.</returns>
    public static T? FindViewModelInStack<T>(this RoutingState item)
        where T : IRoutableViewModel
    {
        if (item is null)
        {
            throw new System.ArgumentNullException(nameof(item));
        }

        return item.NavigationStack.Reverse().OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// Returns the currently visible ViewModel.
    /// </summary>
    /// <param name="item">The routing state.</param>
    /// <returns>The matching ViewModel or null if none exists.</returns>
    public static IRoutableViewModel? GetCurrentViewModel(this RoutingState item)
    {
        if (item is null)
        {
            throw new System.ArgumentNullException(nameof(item));
        }

        return item.NavigationStack.LastOrDefault();
    }
}