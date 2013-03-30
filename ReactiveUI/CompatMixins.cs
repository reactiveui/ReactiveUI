using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace ReactiveUI
{
    public static class CompatMixins
    {
        internal static void ForEach<T>(this IEnumerable<T> This, Action<T> block)
        {
            foreach (var v in This) {
                block(v); 
            }
        }

        internal static IEnumerable<T> SkipLast<T>(this IEnumerable<T> This, int count)
        {
            return This.Take(This.Count() - count);
        }

        public static IObservable<T> PermaRef<T>(this IConnectableObservable<T> This)
        {
            This.Connect();
            return This;
        }
    }
}