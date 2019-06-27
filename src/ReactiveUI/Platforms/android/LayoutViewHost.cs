﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Android.Content;
using Android.Views;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// A class that implements the Android ViewHolder pattern. Use it along
    /// with GetViewHost.
    /// </summary>
    public abstract class LayoutViewHost : ILayoutViewHost, IEnableLogger
    {
        private View _view;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutViewHost"/> class.
        /// </summary>
        protected LayoutViewHost()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutViewHost"/> class.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="layoutId">The layout identifier.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="attachToRoot">if set to <c>true</c> [attach to root].</param>
        /// <param name="performAutoWireup">if set to <c>true</c> [perform automatic wireup].</param>
        protected LayoutViewHost(Context ctx, int layoutId, ViewGroup parent, bool attachToRoot = false, bool performAutoWireup = true)
        {
            var inflater = LayoutInflater.FromContext(ctx);
            View = inflater.Inflate(layoutId, parent, attachToRoot);

            if (performAutoWireup)
            {
                this.WireUpControls();
            }
        }

        /// <inheritdoc/>
        public View View
        {
            get => _view;

            set
            {
                if (_view == value)
                {
                    return;
                }

                _view = value;
                _view?.SetTag(ViewMixins.ViewHostTag, this.ToJavaObject());
            }
        }

        /// <summary>
        /// Casts the LayoutViewHost to a View.
        /// </summary>
        /// <param name="layoutViewHost">The LayoutViewHost to cast.</param>
        [SuppressMessage("Usage", "CA2225: Provide a method named ToView", Justification = "A property is already provided.")]
        public static implicit operator View(LayoutViewHost layoutViewHost) => layoutViewHost?.View;
    }
}
