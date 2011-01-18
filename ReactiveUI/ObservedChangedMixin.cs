using System;
using System.Collections.Generic;
using System.Disposables;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ReactiveUI
{
    public static class ObservedChangedMixin
    {
        static MemoizingMRUCache<string, string[]> propStringToNameCache = 
            new MemoizingMRUCache<string, string[]>((x,_) => x.Split('.'), 25);

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
                var pi = RxApp.getPropertyInfoOrThrow(current.GetType(), propName);
                current = pi.GetValue(current, null);
            }

            return (TValue)current;
        }

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

    public static class BindingMixins
    {
        public static IDisposable BindTo<TTarget, TValue>(
                this IObservable<TValue> This, 
                TTarget target,
                Expression<Func<TTarget, TValue>> property)
            where TTarget : IReactiveNotifyPropertyChanged
        {
            var sourceSub = new MutableDisposable();
            var source = This.Publish(new Subject<TValue>());

            var subscribify = new Action<TTarget, string[]>((tgt, propNames) => {
                if (sourceSub.Disposable != null) {
                    sourceSub.Disposable.Dispose();
                }

                object current = tgt;
                PropertyInfo pi = null;
                foreach(var propName in propNames.SkipLast(1)) {
                    pi = RxApp.getPropertyInfoOrThrow(current.GetType(), propName);
                    current = pi.GetValue(current, null);
                }

                pi = RxApp.getPropertyInfoOrThrow(current.GetType(), propNames.Last());
                sourceSub.Disposable = This.Subscribe(x => {
                    pi.SetValue(current, x, null);
                });
            });

            IDisposable[] toDispose = new IDisposable[] {sourceSub, null};
            string[] propertyNames = RxApp.expressionToPropertyNames(property);
            toDispose[1] = target.ObservableForProperty(property).Subscribe(_ => subscribify(target, propertyNames));

            subscribify(target, propertyNames);

            return Disposable.Create(() => { toDispose[0].Dispose(); toDispose[1].Dispose(); });
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
