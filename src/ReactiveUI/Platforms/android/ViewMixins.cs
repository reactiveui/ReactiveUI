// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        /// <param name="this">The view.</param>
        /// <returns>The view host.</returns>
        public static T GetViewHost<T>(this View @this)
            where T : ILayoutViewHost
        {
            var tagData = @this.GetTag(ViewHostTag);
            if (tagData != null)
            {
                return tagData.ToNetObject<T>();
            }

            return default(T);
        }

        /// <summary>
        /// Gets the ViewHost associated with a given View by accessing the
        /// Tag of the View.
        /// </summary>
        /// <param name="this">The view.</param>
        /// <returns>The view host.</returns>
        public static ILayoutViewHost GetViewHost(this View @this)
        {
            var tagData = @this.GetTag(ViewHostTag);
            if (tagData != null)
            {
                return tagData.ToNetObject<ILayoutViewHost>();
            }

            return null;
        }
    }
}
