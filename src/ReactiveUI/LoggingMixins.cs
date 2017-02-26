using System;
using System.Reactive.Linq;
using Splat;

namespace ReactiveUI
{
    public static class ObservableLoggingMixin
    {
        /// <summary>
        /// Logs an Observable to Splat's Logger
        /// </summary>
        /// <param name="klass">The hosting class, usually 'this'</param>
        /// <param name="message">An optional method</param>
        /// <param name="stringifier">An optional Func to convert Ts to strings.</param>
        /// <returns>The same Observable</returns>
        public static IObservable<T> Log<T, TObj>(this IObservable<T> This,
                TObj klass,
                string message = null,
                Func<T, string> stringifier = null)
            where TObj : IEnableLogger
        {
            message = message ?? "";

            if (stringifier != null) {
                return This.Do(
                    x => klass.Log().Info("{0} OnNext: {1}", message, stringifier(x)),
                    ex => klass.Log().WarnException(message + " " + "OnError", ex),
                    () => klass.Log().Info("{0} OnCompleted", message));
            } else {
                return This.Do(
                    x => klass.Log().Info("{0} OnNext: {1}", message, x),
                    ex => klass.Log().WarnException(message + " " + "OnError", ex),
                    () => klass.Log().Info("{0} OnCompleted", message));
            }
        }

        /// <summary>
        /// Like Catch, but also prints a message and the error to the log.
        /// </summary>
        /// <param name="klass">The hosting class, usually 'this'</param>
        /// <param name="next">The Observable to replace the current one OnError.</param>
        /// <param name="message">An error message to print.</param>
        /// <returns>The same Observable</returns>
        public static IObservable<T> LoggedCatch<T, TObj>(this IObservable<T> This, TObj klass, IObservable<T> next = null, string message = null)
            where TObj : IEnableLogger
        {
            next = next ?? Observable<T>.Default;
            return This.Catch<T, Exception>(ex => {
                klass.Log().WarnException(message ?? "", ex);
                return next;
            });
        }

        /// <summary>
        /// Like Catch, but also prints a message and the error to the log.
        /// </summary>
        /// <param name="klass">The hosting class, usually 'this'</param>
        /// <param name="next">A Func to create an Observable to replace the
        /// current one OnError.</param>
        /// <param name="message">An error message to print.</param>
        /// <returns>The same Observable</returns>
        public static IObservable<T> LoggedCatch<T, TObj, TException>(this IObservable<T> This, TObj klass, Func<TException, IObservable<T>> next, string message = null)
            where TObj : IEnableLogger
            where TException : Exception
        {
            return This.Catch<T, TException>(ex => {
                klass.Log().WarnException(message ?? "", ex);
                return next(ex);
            });
        }
    }
}
