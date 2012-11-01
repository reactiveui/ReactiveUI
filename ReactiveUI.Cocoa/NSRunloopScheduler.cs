using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using ReactiveUI;

#if UIKIT
using MonoTouch.UIKit;
using MonoTouch.Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif

namespace ReactiveUI.Cocoa
{
    public class NSRunloopScheduler : IScheduler, IEnableLogger
    {
        NSObject theApp;

#if UIKIT
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
        
        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            IDisposable innerDisp = Disposable.Empty;
            theApp.BeginInvokeOnMainThread(new NSAction(() => innerDisp = action(this, state)));
            
            return innerDisp;
        }
        
        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            if (dueTime <= Now) {
                return Schedule(state, action);
            }
            
            return Schedule(state, dueTime - Now, action);
        }
        
        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var innerDisp = Disposable.Empty;
            bool isCancelled = false;
            
            var timer = NSTimer.CreateScheduledTimer(dueTime, () => {
                if (!isCancelled) innerDisp = action(this, state);
            });
            
            return Disposable.Create(() => {
                isCancelled = true;
                timer.Invalidate();
                innerDisp.Dispose();
            });
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :