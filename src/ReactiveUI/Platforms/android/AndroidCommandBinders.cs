// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using Android.Views;

namespace ReactiveUI
{
    /// <summary>
    /// Android implementation that provides binding to an ICommand in the ViewModel to a Control
    /// in the View.
    /// </summary>
    public class AndroidCommandBinders : FlexibleCommandBinder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndroidCommandBinders"/> class.
        /// </summary>
        public AndroidCommandBinders()
        {
            Type view = typeof(View);
            Register(view, 9, (cmd, t, cp) => ForEvent(cmd, t, cp, "Click", view.GetRuntimeProperty("Enabled")));
        }

        /// <summary>
        /// Gets the static instance of <see cref="AndroidCommandBinders"/>.
        /// </summary>
        public static Lazy<AndroidCommandBinders> Instance { get; } = new();
    }
}
