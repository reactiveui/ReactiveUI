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
    public class UIKitCommandBinders : FlexibleCommandBinder
    {
        public static Lazy<UIKitCommandBinders> Instance = new Lazy<UIKitCommandBinders>();

        public UIKitCommandBinders ()
        {
            Register(typeof(UIControl), 9, (cmd, t, cp)=> ForTargetAction(cmd, t, cp, "Enabled"));
            Register(typeof(UIRefreshControl), 10, (cmd, t, cp)=> ForEvent(cmd, t, cp, "ValueChanged", "Enabled"));
            Register(typeof(UIBarButtonItem), 10, (cmd, t, cp)=> ForEvent(cmd, t, cp, "Clicked", "Enabled"));
        }
    }
}

