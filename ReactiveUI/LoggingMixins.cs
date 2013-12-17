using System;
using System.Reactive.Linq;
using Splat;

namespace ReactiveUI
{
    public static class ObservableLoggingMixin
    {
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
    }
}