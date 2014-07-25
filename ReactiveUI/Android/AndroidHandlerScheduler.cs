using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using Android.OS;

namespace ReactiveUI
{
    public sealed class AndroidHandlerScheduler : IScheduler
    {
        private readonly Handler handler;

        public AndroidHandlerScheduler(Handler handler)
        {
            this.handler = handler;
        }

        public AndroidHandlerScheduler(Looper looper) : this(new Handler(looper)) {}

        public AndroidHandlerScheduler() : this(new Handler()) {}

        public DateTimeOffset Now {
            get { return DateTimeOffset.Now; }
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            IDisposable innerDisp = Disposable.Empty;
            handler.Post (new Action (() => innerDisp = action (this, state)));
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

            var disp = TaskPoolScheduler.Default.Schedule(state, dueTime, (sched, st) => {
                IDisposable innerDisp = Disposable.Empty;

                if (!isCancelled) { 
                    handler.Post(() => {
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

