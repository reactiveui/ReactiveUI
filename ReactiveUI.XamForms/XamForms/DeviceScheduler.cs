using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using ReactiveUI;
using Xamarin.Forms;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Scheduler that uses the Device static class to schedule items to the 
    /// UI thread
    /// </summary>
    public class DeviceScheduler : IScheduler, IEnableLogger
    {
        public DateTimeOffset Now {
            get { return DateTimeOffset.Now; }
        }
        
        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            IDisposable innerDisp = Disposable.Empty;
            Device.BeginInvokeOnMainThread(new Action(() => innerDisp = action(this, state)));
            
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
            IDisposable disp = Disposable.Empty;

            Device.StartTimer(dueTime, () => {
                if (!isCancelled) { 
                    Device.BeginInvokeOnMainThread(() => {
                        if (!isCancelled) disp = action(this, state);
                    });
                }

                return false;
            });

            return Disposable.Create(() => {
                isCancelled = true;
                disp.Dispose();
            });
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
