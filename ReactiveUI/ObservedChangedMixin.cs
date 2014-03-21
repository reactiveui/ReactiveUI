using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;

namespace ReactiveUI
{
    public static class ObservedChangedMixin
    {
        /// <summary>
        /// Returns the current value of a property given a notification that
        /// it has changed.
        /// </summary>
        /// <returns>The current value of the property</returns>
        public static TValue GetValue<TSender, TValue>(this IObservedChange<TSender, TValue> This)
        {
            TValue ret;
            if (!This.TryGetValue(out ret)) {
                throw new Exception(String.Format("One of the properties in the expression '{0}' was null", This.PropertyName));
            }
            return ret;
        }

        /// <summary>
        /// Attempts to return the current value of a property given a 
        /// notification that it has changed. If any property in the
        /// property expression is null, false is returned.
        /// </summary>
        /// <param name="changeValue">The value of the property
        /// expression.</param>
        /// <returns>True if the entire expression was able to be followed,
        /// false otherwise</returns>
        internal static bool TryGetValue<TSender, TValue>(this IObservedChange<TSender, TValue> This, out TValue changeValue)
        {
            if (!Equals(This.Value, default(TValue))) {
                changeValue = This.Value;
                return true;
            }

            object current = This.Sender;
            string fullPropName = This.PropertyName;

            return Reflection.TryGetValueForPropertyChain(out changeValue, current, fullPropName.Split('.'));
        }

        /// <summary>
        /// Given a fully filled-out IObservedChange object, SetValueToProperty
        /// will apply it to the specified object (i.e. it will ensure that
        /// target.property == This.GetValue() and "replay" the observed change
        /// onto another object)
        /// </summary>
        /// <param name="target">The target object to apply the change to.</param>
        /// <param name="property">The target property to apply the change to.</param>
        internal static void SetValueToProperty<TSender, TValue, TTarget>(
            this IObservedChange<TSender, TValue> This, 
            TTarget target,
            Expression<Func<TTarget, TValue>> property)
        {
            Reflection.SetValueToPropertyChain(target, Reflection.ExpressionToPropertyNames(property), This.GetValue());
        }

        /// <summary>
        /// Given a stream of notification changes, this method will convert 
        /// the property changes to the current value of the property.
        /// </summary>
        /// <returns>An Observable representing the stream of current values of
        /// the given change notification stream.</returns>
        public static IObservable<TValue> Value<TSender, TValue>(
		    this IObservable<IObservedChange<TSender, TValue>> This)
        {
            return This.Select(GetValue);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
