using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace ReactiveUI
{
    /// <summary>
    /// Observed Changed Mixin
    /// </summary>
    public static class ObservedChangedMixin
    {
        /// <summary>
        /// Returns the name of a property which has been changed.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="This">The this.</param>
        /// <returns>The name of the property which has change</returns>
        public static string GetPropertyName<TSender, TValue>(this IObservedChange<TSender, TValue> This)
        {
            return Reflection.ExpressionToPropertyNames(This.Expression);
        }

        /// <summary>
        /// Returns the current value of a property given a notification that it has changed.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="This">The this.</param>
        /// <returns>The current value of the property</returns>
        /// <exception cref="System.Exception"></exception>
        public static TValue GetValue<TSender, TValue>(this IObservedChange<TSender, TValue> This)
        {
            TValue ret;
            if (!This.TryGetValue(out ret)) {
                throw new Exception(string.Format("One of the properties in the expression '{0}' was null", This.GetPropertyName()));
            }
            return ret;
        }

        /// <summary>
        /// Given a stream of notification changes, this method will convert the property changes to
        /// the current value of the property.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="This">The this.</param>
        /// <returns>
        /// An Observable representing the stream of current values of the given change notification stream.
        /// </returns>
        public static IObservable<TValue> Value<TSender, TValue>(
            this IObservable<IObservedChange<TSender, TValue>> This)
        {
            return This.Select(GetValue);
        }

        /// <summary>
        /// Given a fully filled-out IObservedChange object, SetValueToProperty will apply it to the
        /// specified object (i.e. it will ensure that target.property == This.GetValue() and
        /// "replay" the observed change onto another object)
        /// </summary>
        /// <typeparam name="TSender">The type of the sender.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="This">The this.</param>
        /// <param name="target">The target object to apply the change to.</param>
        /// <param name="property">The target property to apply the change to.</param>
        internal static void SetValueToProperty<TSender, TValue, TTarget>(
            this IObservedChange<TSender, TValue> This,
            TTarget target,
            Expression<Func<TTarget, TValue>> property)
        {
            Reflection.TrySetValueToPropertyChain(target, Reflection.Rewrite(property.Body).GetExpressionChain(), This.GetValue());
        }

        /// <summary>
        /// Attempts to return the current value of a property given a notification that it has
        /// changed. If any property in the property expression is null, false is returned.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="This">The this.</param>
        /// <param name="changeValue">The value of the property expression.</param>
        /// <returns>True if the entire expression was able to be followed, false otherwise</returns>
        internal static bool TryGetValue<TSender, TValue>(this IObservedChange<TSender, TValue> This, out TValue changeValue)
        {
            if (!Equals(This.Value, default(TValue))) {
                changeValue = This.Value;
                return true;
            }

            return Reflection.TryGetValueForPropertyChain(out changeValue, This.Sender, This.Expression.GetExpressionChain());
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :