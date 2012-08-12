using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;

namespace ReactiveUI
{
    public static class ObservedChangedMixin
    {
        static MemoizingMRUCache<string, string[]> propStringToNameCache = 
            new MemoizingMRUCache<string, string[]>((x,_) => x.Split('.'), 25);

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
        public static bool TryGetValue<TSender, TValue>(this IObservedChange<TSender, TValue> This, out TValue changeValue)
        {
            if (!Equals(This.Value, default(TValue))) {
                changeValue = This.Value;
                return true;
            }

            object current = This.Sender;
            string[] propNames = null;
            lock(propStringToNameCache) { propNames = propStringToNameCache.Get(This.PropertyName); }

            PropertyInfo pi;
            foreach(var propName in propNames.SkipLast(1)) {
                if (current == null) {
                    changeValue = default(TValue);
                    return false;
                }

                pi = RxApp.getPropertyInfoOrThrow(current.GetType(), propName);
                current = pi.GetValue(current, null);
            }

            if (current == null) {
                changeValue = default(TValue);
                return false;
            }

            pi = RxApp.getPropertyInfoOrThrow(current.GetType(), propNames.Last());
            changeValue = (TValue)pi.GetValue(current, null);
            return true;
        }

        internal static IObservedChange<TSender, TValue> fillInValue<TSender, TValue>(this IObservedChange<TSender, TValue> This)
        {
            // XXX: This is an internal method because I'm unsafely upcasting,
            // but in certain cases it's needed.
            var ret = (ObservedChange<TSender, TValue>)This;
            var val = default(TValue);
            This.TryGetValue(out val);
            ret.Value = val;
            return ret;
        }

        /// <summary>
        /// Given a fully filled-out IObservedChange object, SetValueToProperty
        /// will apply it to the specified object (i.e. it will ensure that
        /// target.property == This.GetValue() and "replay" the observed change
        /// onto another object)
        /// </summary>
        /// <param name="target">The target object to apply the change to.</param>
        /// <param name="property">The target property to apply the change to.</param>
        public static void SetValueToProperty<TSender, TValue, TTarget>(
            this IObservedChange<TSender, TValue> This, 
            TTarget target,
            Expression<Func<TTarget, TValue>> property)
        {
            object current = target;
            string[] propNames = RxApp.expressionToPropertyNames(property);

            PropertyInfo pi;
            foreach(var propName in propNames.SkipLast(1)) {
                pi = RxApp.getPropertyInfoOrThrow(current.GetType(), propName);
                current = pi.GetValue(current, null);
            }

            pi = RxApp.getPropertyInfoForProperty(current.GetType(), propNames.Last());
            pi.SetValue(current, This.GetValue(), null);
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

        /// <summary>
        /// ValueIfNotDefault is similar to Value(), but filters out null values
        /// from the stream.
        /// </summary>
        /// <returns>An Observable representing the stream of current values of
        /// the given change notification stream.</returns>
        public static IObservable<TValue> ValueIfNotDefault<TSender, TValue>(
		    this IObservable<IObservedChange<TSender, TValue>> This)
        {
            return This.Value().Where(x => EqualityComparer<TValue>.Default.Equals(x, default(TValue)) == false);
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

    public static class BindingMixins
    {
        /// <summary>
        /// BindTo takes an Observable stream and applies it to a target
        /// property. Conceptually it is similar to "Subscribe(x =&gt;
        /// target.property = x)", but allows you to use child properties
        /// without the null checks.
        /// </summary>
        /// <param name="target">The target object whose property will be set.</param>
        /// <param name="property">An expression representing the target
        /// property to set. This can be a child property (i.e. x.Foo.Bar.Baz).</param>
        /// <returns>An object that when disposed, disconnects the binding.</returns>
        public static IDisposable BindTo<TTarget, TValue>(
                this IObservable<TValue> This, 
                TTarget target,
                Expression<Func<TTarget, TValue>> property)
            where TTarget : class
        {
            var sourceSub = new MultipleAssignmentDisposable();
            var source = This;

            var subscribify = new Action<TTarget, string[]>((tgt, propNames) => {
                if (sourceSub.Disposable != null) {
                    sourceSub.Disposable.Dispose();
                }

                object current = tgt;
                PropertyInfo pi = null;
                foreach(var propName in propNames.SkipLast(1)) {
                    if (current == null) {
                        return;
                    }

                    pi = RxApp.getPropertyInfoOrThrow(current.GetType(), propName);
                    current = pi.GetValue(current, null);
                }
                if (current == null) {
                    return;
                }

                pi = RxApp.getPropertyInfoOrThrow(current.GetType(), propNames.Last());
                sourceSub.Disposable = This.Subscribe(x => pi.SetValue(current, x, null));
            });

            var toDispose = new IDisposable[] {sourceSub, null};
            var propertyNames = RxApp.expressionToPropertyNames(property);
            toDispose[1] = target.ObservableForProperty(property).Subscribe(_ => subscribify(target, propertyNames));

            subscribify(target, propertyNames);

            return Disposable.Create(() => { toDispose[0].Dispose(); toDispose[1].Dispose(); });
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
