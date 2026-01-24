// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;

namespace ReactiveUI;

/// <summary>
/// Provides extension methods for retrieving the view host associated with a given view.
/// </summary>
/// <remarks>The methods in this class enable access to the view host instance that is stored as a tag on a view.
/// These extension methods are intended for use with views that participate in a layout hosting mechanism. This class
/// is static and cannot be instantiated.</remarks>
public static class ViewMixins
{
    internal const int ViewHostTag = -4222;

    /// <summary>
    /// Retrieves the view host of the specified type associated with the given view.
    /// </summary>
    /// <typeparam name="T">The type of view host to retrieve. Must implement <see cref="ILayoutViewHost"/>.</typeparam>
    /// <param name="item">The view from which to retrieve the associated view host. Cannot be null.</param>
    /// <returns>An instance of <typeparamref name="T"/> if a view host of the specified type is associated with the view;
    /// otherwise, the default value for <typeparamref name="T"/>.</returns>
    public static T GetViewHost<T>(this View item) // TODO: Create Test
        where T : ILayoutViewHost
    {
        var tagData = item?.GetTag(ViewHostTag);
        if (tagData is not null)
        {
            return tagData.ToNetObject<T>();
        }

        return default!;
    }

    /// <summary>
    /// Retrieves the layout view host associated with the specified view, if one exists.
    /// </summary>
    /// <param name="item">The view from which to retrieve the associated layout view host. Cannot be null.</param>
    /// <returns>An object that implements <see cref="ILayoutViewHost"/> if the view has an associated host; otherwise, <see
    /// langword="null"/>.</returns>
    public static ILayoutViewHost? GetViewHost(this View item) => // TODO: Create Test
        item?.GetTag(ViewHostTag)?.ToNetObject<ILayoutViewHost>();
}
