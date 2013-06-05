using System;
using ReactiveUI;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using MonoTouch.Foundation;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Reactive.Disposables;

namespace ReactiveUI.Cocoa
{
    public class UIKitEventBasedCommandBinders : EventBasedCommandBinder
    {
        public static Lazy<UIKitEventBasedCommandBinders> Instance = new Lazy<UIKitEventBasedCommandBinders>();

        public UIKitEventBasedCommandBinders ()
        {
            Register(typeof(UIRefreshControl), 10, (cmd, t, cp)=> CommandBindingFromEvent(cmd, t, cp, "ValueChanged", "Enabled"));
            Register(typeof(UIBarButtonItem), 10, (cmd, t, cp)=> CommandBindingFromEvent(cmd, t, cp, "Clicked", "Enabled"));
        }
    }
}

