using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUI
{
    public static class TplMixins
    {
        /// <summary>
        /// Apply a TPL-async method to each item in an IObservable. Like
        /// Select but asynchronous via the TPL.
        /// </summary>
        /// <param name="selector">The selection method to use.</param>
        /// <returns>An Observable represented the mapped sequence.</returns>
        public static IObservable<TRet> SelectAsync<T,TRet>(this IObservable<T> This, Func<T, Task<TRet>> selector)
        {
            return This.SelectMany(x => selector(x).ToObservable());
        }

        /// <summary>
        /// Creates an IObservable(Unit) for the Task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns>An observable for the task.</returns>
        public static IObservable<Unit> ToObservable(this Task task)
        {
            if (task == null) throw new ArgumentNullException("task");
            return new TaskObservable<Unit>(task);
        }

        /// <summary>
        /// Creates an IObservable(TResult) for the Task(TResult).
        /// </summary>
        /// <typeparam name="TResult">Specifies the type of result returned by the task.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>An observable for the task.</returns>
        public static IObservable<TResult> ToObservable<TResult>(this Task<TResult> task)
        {
            if (task == null) throw new ArgumentNullException("task");
            return new TaskObservable<TResult>(task);
        }

        /// <summary>
        /// Returns a task that contains the last value of the observable sequence.
        /// </summary>
        /// <typeparam name="TResult">Specifies the type of elements in the sequence.</typeparam>
        /// <param name="observable">The observable sequence.</param>
        /// <returns>A task that contains the last value of the observable sequence.</returns>
        public static Task<TResult> ToTask<TResult>(this IObservable<TResult> observable)
        {
            if (observable == null)
            {
                throw new ArgumentNullException("observable");
            }
            bool hasValue = false;
            TResult lastValue = default(TResult);
            var tcs = new TaskCompletionSource<TResult>();
            observable.Subscribe(
                value =>
                    {
                        hasValue = true;
                        lastValue = value;
                    },
                ex =>
                    {
                        tcs.TrySetException(ex);
                    },
                () =>
                    {
                        if (hasValue)
                        {
                            tcs.TrySetResult(lastValue);
                        }
                        else
                        {
                            tcs.TrySetException(new InvalidOperationException("Sequence contains no elements."));
                        }
                    });
            return tcs.Task;

        }

        private sealed class TaskObservable<TResult> : IObservable<TResult>
        {
            private readonly Task _task;

            public TaskObservable(Task task)
            {
                _task = task;
            }

            public IDisposable Subscribe(IObserver<TResult> observer)
            {
                if (observer == null) throw new ArgumentNullException("observer");
                if(_task.IsCompleted)
                {
                    PublishTaskToObserver(_task, observer);
                    return Disposable.Empty;
                }
                var source = new CancellationTokenSource();
                var disposable = Disposable.Create(source.Cancel);
                Action<Task> continuationAction = t =>
                    {
                        try
                        {
                            PublishTaskToObserver(t, observer);
                        }
                        catch (Exception exception)
                        {
                            observer.OnError(exception);
                        }
                    };

                _task.ContinueWith(
                    continuationAction,
                    source.Token,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);

                return disposable;
            }

            private void PublishTaskToObserver(Task task, IObserver<TResult> observer)
            {
                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        {
                            var task2 = task as Task<TResult>;
                            observer.OnNext((task2 != null) ? task2.Result : default(TResult));
                            observer.OnCompleted();
                            return;
                        }
                    case TaskStatus.Canceled:
                        observer.OnCompleted();
                        return;

                    case TaskStatus.Faulted:
                        observer.OnError(task.Exception);
                        return;
                }

            }
        }
    }
}