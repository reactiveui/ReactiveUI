using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using ReactiveUI;
using Android.App;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// AndroidUIScheduler is a scheduler that schedules items on a running 
    /// Activity's main thread. This is the moral equivalent of 
    /// DispatcherScheduler, but since every Activity runs separately, you must
    /// assign RxApp.MainThreadScheduler to an instance of this at the start of
    /// every activity.
    /// </summary>
    public class AndroidUIScheduler : IScheduler, IEnableLogger
    {
        Activity activity;
        
        public AndroidUIScheduler(Activity activity)
        {
            this.activity = activity;
        }
    
        public DateTimeOffset Now {
            get { return DateTimeOffset.Now; }
        }
        
        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            IDisposable innerDisp = Disposable.Empty;
            activity.RunOnUiThread(new Action(() => innerDisp = action(this, state)));
            
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
            bool isCancelled = false;
            
            var disp = Scheduler.TaskPool.Schedule(state, dueTime, (sched, st) => {
                IDisposable innerDisp = Disposable.Empty;
                
                if (!isCancelled) { 
                    activity.RunOnUiThread(() => {
                        if (!isCancelled) innerDisp = action(this, state);
                    });
                }
               
                return Disposable.Create(() => {
                    isCancelled = true;
                    innerDisp.Dispose();
                });    
            });
            
            return Disposable.Create(() => {
                isCancelled = true;
                disp.Dispose();
            });
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :