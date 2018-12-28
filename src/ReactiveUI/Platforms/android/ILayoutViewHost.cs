﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Android.Views;

namespace ReactiveUI
{
    /// <summary>
    /// Interface that defines a layout view host.
    /// </summary>
    public interface ILayoutViewHost
    {
        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        View View { get; }
    }
}
