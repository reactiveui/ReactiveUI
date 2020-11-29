// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using UIKit;

namespace ReactiveUI
{
    /// <summary>
    /// UI Kit command binder platform registrations.
    /// </summary>
    /// <seealso cref="ReactiveUI.ICreatesCommandBinding" />
    public class UIKitCommandBinders : FlexibleCommandBinder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIKitCommandBinders"/> class.
        /// </summary>
        public UIKitCommandBinders()
        {
            Register(typeof(UIControl), 9, (cmd, t, cp) => ForTargetAction(cmd, t, cp, typeof(UIControl).GetRuntimeProperty("Enabled")));
            Register(typeof(UIRefreshControl), 10, (cmd, t, cp) => ForEvent(cmd, t, cp, "ValueChanged", typeof(UIRefreshControl).GetRuntimeProperty("Enabled")));
            Register(typeof(UIBarButtonItem), 10, (cmd, t, cp) => ForEvent(cmd, t, cp, "Clicked", typeof(UIBarButtonItem).GetRuntimeProperty("Enabled")));
        }

        /// <summary>
        /// Gets the UIKitCommandBinders instance.
        /// </summary>
        public static Lazy<UIKitCommandBinders> Instance { get; } = new ();
    }
}
