using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactiveUI
{
    internal static class CompatMixins
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
    }

    // according to spouliot, this is just a string match, and will cause the
    // linker to be ok with everything.
    internal class PreserveAttribute : Attribute
    {
        public bool AllMembers { get; set; }
    }
}
