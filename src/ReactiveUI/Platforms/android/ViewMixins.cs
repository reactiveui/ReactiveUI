// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Android.Views;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Provides extension methods for retrieving the view host associated with a given view.</summary>
/// <remarks>The methods in this class enable access to the view host instance that is stored as a tag on a view.
/// These extension methods are intended for use with views that participate in a layout hosting mechanism. This class
/// is static and cannot be instantiated.</remarks>
public static class ViewMixins
{
    /// <summary>The tag key used to store the view host instance on a view.</summary>
    internal const int ViewHostTag = -4_222;

    /// <summary>Extends a <see cref="View"/> with helpers for accessing its associated view host.</summary>
    /// <param name="item">The view whose associated view host is retrieved. Cannot be null.</param>
    extension(View item)
    {
        /// <summary>Retrieves the view host of the specified type associated with the given view.</summary>
        /// <typeparam name="T">The type of view host to retrieve. Must implement <see cref="ILayoutViewHost"/>.</typeparam>
        /// <returns>An instance of <typeparamref name="T"/> if a view host of the specified type is associated with the view;
        /// otherwise, the default value for <typeparamref name="T"/>.</returns>
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public T GetViewHost<T>()
            where T : ILayoutViewHost
        {
            var tagData = item?.GetTag(ViewHostTag);
            if (tagData is not null)
            {
                return tagData.ToNetObject<T>();
            }

            return default!;
        }

        /// <summary>Retrieves the layout view host associated with the specified view, if one exists.</summary>
        /// <returns>An object that implements <see cref="ILayoutViewHost"/> if the view has an associated host; otherwise, <see
        /// langword="null"/>.</returns>
        public ILayoutViewHost? GetViewHost() => item?.GetTag(ViewHostTag)?.ToNetObject<ILayoutViewHost>();
    }
}
