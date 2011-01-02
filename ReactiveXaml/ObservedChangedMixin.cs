using System;
using System.Linq;

namespace ReactiveXaml
{
    public static class ObservedChangedMixin
    {
        /// <summary>
        /// Returns the current value of a property given a notification that it has changed.
        /// </summary>
        /// <returns>The current value of the property</returns>
        public static TValue GetValue<TSender, TValue>(this IObservedChange<TSender, TValue> This)
        {
            if (!Equals(This.Value, default(TValue))) {
                return This.Value;
            }
            var pi = RxApp.getPropertyInfoForProperty(This.Sender.GetType(), This.PropertyName);
            return (TValue)pi.GetValue(This.Sender, null);
        }

        /// <summary>
        /// Given a stream of notification changes, this method will convert 
        /// the property changes to the current value of the property.
        /// </summary>
        public static IObservable<TValue> Value<TSender, TValue>(this IObservable<IObservedChange<TSender, TValue>> This)
        {
            return This.Select(GetValue);
        }

        /// <summary>
        /// Given a stream of notification changes, this method will convert 
        /// the property changes to the current value of the property.
        /// </summary>
        public static IObservable<TRet> Value<TSender, TValue, TRet>(this IObservable<IObservedChange<TSender, TValue>> This)
        {
            // XXX: There is almost certainly a non-retarded way to do this
            return This.Select(x => (TRet)((object)GetValue(x)));
        }
    }
}