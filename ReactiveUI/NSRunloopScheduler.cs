using System;
using System.Linq;
using System.Collections.Generic;
using System.Concurrency;
using System.Disposables;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace ReactiveUI
{
    public class NSRunloopScheduler : IScheduler, IEnableLogger
    {
        NSObject theApp;

#if IOS
        public NSRunloopScheduler (UIApplication app)
        {
            theApp = app;
        }
#else
        public NSRunloopScheduler (NSApplication app)
        {
            theApp = app;
        }
#endif

        public DateTimeOffset Now {
            get { return DateTimeOffset.Now; }
        }

        public IDisposable Schedule(Action action)
        {
            this.Log().Debug("Scheduling on Runloop");
            theApp.BeginInvokeOnMainThread(new NSAction(action));
            return Disposable.Empty;
        }

        public IDisposable Schedule(Action action, TimeSpan dueTime)
        {
            this.Log().Debug("Scheduling on Runloop");
            var timer = NSTimer.CreateScheduledTimer(dueTime, new NSAction(action));
            return Disposable.Create(() => timer.Invalidate());
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
