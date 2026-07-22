// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Provides extension methods for querying and retrieving view models from a routing state navigation stack.</summary>
public static class RoutingStateMixins
{
    /// <summary>Provides view model query extension members for <see cref="RoutingState"/>.</summary>
    /// <param name="item">The <see cref="RoutingState"/> instance whose navigation stack is searched.</param>
    extension(RoutingState item)
    {
        /// <summary>Searches the navigation stack in reverse order and returns the first view model of the specified type, if found.</summary>
        /// <typeparam name="T">The type of view model to search for. Must implement <see cref="IRoutableViewModel"/>.</typeparam>
        /// <returns>The first view model of type <typeparamref name="T"/> found in the navigation stack, or <see langword="null"/>
        /// if no such view model exists.</returns>
        [SuppressMessage(
            "Design",
            "SST2307:Generic method type parameters should be inferable from the parameters",
            Justification = "The view model type to search for is supplied explicitly by the caller by design; it selects the match type and cannot be inferred from the parameters.")]
        public T? FindViewModelInStack<T>()
            where T : IRoutableViewModel
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            var stack = item.NavigationStack;
            for (var i = stack.Count - 1; i >= 0; i--)
            {
                if (stack[i] is T match)
                {
                    return match;
                }
            }

            return default;
        }

        /// <summary>Gets the current view model from the top of the navigation stack.</summary>
        /// <returns>The view model at the top of the navigation stack, or null if the navigation stack is empty.</returns>
        public IRoutableViewModel? GetCurrentViewModel()
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            var stack = item.NavigationStack;
            return stack.Count > 0 ? stack[stack.Count - 1] : null;
        }
    }
}
