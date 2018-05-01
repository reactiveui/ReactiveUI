// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using UIKit;

namespace ReactiveUI
{
    public class UIKitCommandBinders : FlexibleCommandBinder
    {
        public static Lazy<UIKitCommandBinders> Instance = new Lazy<UIKitCommandBinders>();

        public UIKitCommandBinders()
        {
            Register(typeof(UIControl), 9, (cmd, t, cp) => ForTargetAction(cmd, t, cp, typeof(UIControl).GetRuntimeProperty("Enabled")));
            Register(typeof(UIRefreshControl), 10, (cmd, t, cp) => ForEvent(cmd, t, cp, "ValueChanged", typeof(UIRefreshControl).GetRuntimeProperty("Enabled")));
            Register(typeof(UIBarButtonItem), 10, (cmd, t, cp) => ForEvent(cmd, t, cp, "Clicked", typeof(UIBarButtonItem).GetRuntimeProperty("Enabled")));
        }
    }
}

