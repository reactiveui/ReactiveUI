using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using GLib;

namespace ReactiveUI.Gtk
{
    public class GtkRunloopScheduler : IScheduler, IDisposable
    {
        IdleHandler handler = null;
        readonly List<Action> dispatcherQueue = new List<Action>();
    
        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            Action toAdd = () => action(this, state);
            dispatcherQueue.Add(toAdd);
            initializeHandlerIfNecessary();
            
            return Disposable.Create(() => {
                var index = dispatcherQueue.IndexOf(toAdd);
                if (index >= 0) {
                    dispatcherQueue.RemoveAt(index);
                }
            });
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
            
            Timeout.Add((uint)dueTime.TotalMilliseconds, () => {
                if (!isCancelled)
                    action(this, state);
                return false;
            });
            
            return Disposable.Create(() => isCancelled = true);
        }
        
        public DateTimeOffset Now {
            get { return DateTimeOffset.Now; }
        }
        
        public void Dispose()
        {
            if (handler != null) {
                Idle.Remove(handler);
                handler = null;
            }
        }
        
        void initializeHandlerIfNecessary()
        {
            if (handler != null) {
                return;
            }
        
            handler = () => {
                if (dispatcherQueue.Count == 0) {
                    return true;
                }
                
                var item = dispatcherQueue [0];
                dispatcherQueue.RemoveAt(0);
                item();
                
                if (dispatcherQueue.Count == 0) {
                    handler = null;
                    return false;
                }
                
                return true;
            };
            
            Idle.Add(handler);      
        }
    }
}
