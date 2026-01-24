// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides extension methods for querying and retrieving view models from a routing state navigation stack.
/// </summary>
public static class RoutingStateMixins
{
    /// <summary>
    /// Searches the navigation stack in reverse order and returns the first view model of the specified type, if found.
    /// </summary>
    /// <typeparam name="T">The type of view model to search for. Must implement <see cref="IRoutableViewModel"/>.</typeparam>
    /// <param name="item">The <see cref="RoutingState"/> instance whose navigation stack is searched.</param>
    /// <returns>The first view model of type <typeparamref name="T"/> found in the navigation stack, or <see langword="null"/>
    /// if no such view model exists.</returns>
    public static T? FindViewModelInStack<T>(this RoutingState item)
        where T : IRoutableViewModel
    {
        ArgumentExceptionHelper.ThrowIfNull(item);

        return item.NavigationStack.Reverse().OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// Gets the current view model from the top of the navigation stack.
    /// </summary>
    /// <param name="item">The routing state instance from which to retrieve the current view model. Cannot be null.</param>
    /// <returns>The view model at the top of the navigation stack, or null if the navigation stack is empty.</returns>
    public static IRoutableViewModel? GetCurrentViewModel(this RoutingState item)
    {
        ArgumentExceptionHelper.ThrowIfNull(item);

        return item.NavigationStack.LastOrDefault();
    }
}
