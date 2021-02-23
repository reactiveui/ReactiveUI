// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;

namespace ReactiveUI
{
    /// <summary>
    /// Default methods for <see cref="ILayoutViewHost"/>.
    /// </summary>
    public static class ViewMixins
    {
        internal const int ViewHostTag = -4222;

        /// <summary>
        /// Gets the ViewHost associated with a given View by accessing the
        /// Tag of the View.
        /// </summary>
        /// <typeparam name="T">The layout view host type.</typeparam>
        /// <param name="item">The view.</param>
        /// <returns>The view host.</returns>
        public static T GetViewHost<T>(this View item)
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
        /// Gets the ViewHost associated with a given View by accessing the
        /// Tag of the View.
        /// </summary>
        /// <param name="item">The view.</param>
        /// <returns>The view host.</returns>
        public static ILayoutViewHost? GetViewHost(this View item) => item?.GetTag(ViewHostTag)?.ToNetObject<ILayoutViewHost>();
    }
}
