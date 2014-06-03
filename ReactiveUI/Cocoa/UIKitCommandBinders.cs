using System;
using System.Reflection;
using ReactiveUI;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using MonoTouch.Foundation;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Reactive.Disposables;

namespace ReactiveUI
{
    public class UIKitCommandBinders : FlexibleCommandBinder
    {
        public static Lazy<UIKitCommandBinders> Instance = new Lazy<UIKitCommandBinders>();

        public UIKitCommandBinders ()
        {
            Register(typeof(UIControl), 9, (cmd, t, cp)=> ForTargetAction(cmd, t, cp, typeof(UIControl).GetRuntimeProperty("Enabled")));
            Register(typeof(UIRefreshControl), 10, (cmd, t, cp)=> ForEvent(cmd, t, cp, "ValueChanged", typeof(UIRefreshControl).GetRuntimeProperty("Enabled")));
            Register(typeof(UIBarButtonItem), 10, (cmd, t, cp)=> ForEvent(cmd, t, cp, "Clicked", typeof(UIBarButtonItem).GetRuntimeProperty("Enabled")));
        }
    }
}

