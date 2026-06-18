// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

namespace ReactiveUI;

/// <summary>DynamicData change-set projections of a <see cref="RoutingState"/> navigation stack.</summary>
public static class RoutingStateDynamicDataMixins
{
    /// <summary>Provides DynamicData navigation-stack change-set extension members for <see cref="RoutingState"/>.</summary>
    /// <param name="routingState">The routing state whose navigation stack is observed.</param>
    extension(RoutingState routingState)
    {
        /// <summary>
        /// Gets an observable that signals detailed DynamicData change sets for the navigation stack, enabling reactive
        /// views to animate push/pop operations.
        /// </summary>
        /// <returns>A change-set stream describing each navigation-stack mutation.</returns>
        public IObservable<IChangeSet<IRoutableViewModel>> NavigationChanged()
        {
            ArgumentExceptionHelper.ThrowIfNull(routingState);

            return routingState.NavigationChanges.ToDynamicDataChangeSet();
        }
    }
}
