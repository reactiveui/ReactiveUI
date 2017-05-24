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

