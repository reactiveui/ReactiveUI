using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

#if UNIFIED
using CoreFoundation;
using Foundation;
using NSAction = System.Action;
#elif UIKIT
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
#else
using MonoMac.Foundation;
using MonoMac.CoreFoundation;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// Provides a scheduler which will use the Cocoa main loop to schedule
    /// work on. This is the Cocoa equivalent of DispatcherScheduler.
    /// </summary>
    public class NSRunloopScheduler : IScheduler
    {
        public DateTimeOffset Now {
            get { return DateTimeOffset.Now; }
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            var innerDisp = new SingleAssignmentDisposable();

            DispatchQueue.MainQueue.DispatchAsync(new NSAction(() => {
                if (!innerDisp.IsDisposed) innerDisp.Disposable = action(this, state);
            }));
            
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

#if UNIFIED
            var timer = NSTimer.CreateScheduledTimer(dueTime, _ => {
#else
            var timer = NSTimer.CreateScheduledTimer(dueTime, () => {
#endif
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
