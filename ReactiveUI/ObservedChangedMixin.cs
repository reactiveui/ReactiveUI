using System;
using System.Linq;

namespace ReactiveUI
{
    public static class ObservedChangedMixin
    {
        static MemoizingMRUCache<string, string[]> propStringToNameCache = new MemoizingMRUCache<string, string[]>((x,_) => x.Split('.'), 25);

        /// <summary>
        /// Returns the current value of a property given a notification that it has changed.
        /// </summary>
        /// <returns>The current value of the property</returns>
        public static TValue GetValue<TSender, TValue>(this IObservedChange<TSender, TValue> This)
        {
            if (!Equals(This.Value, default(TValue))) {
                return This.Value;
            }

            object current = This.Sender;
            string[] propNames = null;;
            lock(propStringToNameCache) { propNames = propStringToNameCache.Get(This.PropertyName); }

            foreach(var propName in propNames) {
                var pi = RxApp.getPropertyInfoForProperty(current.GetType(), propName);
                current = pi.GetValue(current, null);
            }

            return (TValue)current;
        }

        /// <summary>
        /// Given a stream of notification changes, this method will convert 
        /// the property changes to the current value of the property.
        /// </summary>
        public static IObservable<TValue> Value<TSender, TValue>(
		    this IObservable<IObservedChange<TSender, TValue>> This)
        {
            return This.Select(GetValue);
        }

        /// <summary>
        /// Given a stream of notification changes, this method will convert 
        /// the property changes to the current value of the property.
        /// </summary>
        public static IObservable<TRet> Value<TSender, TValue, TRet>(
                this IObservable<IObservedChange<TSender, TValue>> This)
        {
            // XXX: There is almost certainly a non-retarded way to do this
            return This.Select(x => (TRet)((object)GetValue(x)));
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
