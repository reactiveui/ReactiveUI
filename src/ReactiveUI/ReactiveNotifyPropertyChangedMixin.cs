using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;
using Splat;

namespace ReactiveUI
{
    public static class ReactiveNotifyPropertyChangedMixin
    {
        static ReactiveNotifyPropertyChangedMixin()
        {
            RxApp.EnsureInitialized();
        }

        /// <summary>
        /// ObservableForProperty returns an Observable representing the
        /// property change notifications for a specific property on a
        /// ReactiveObject. This method (unlike other Observables that return
        /// IObservedChange) guarantees that the Value property of
        /// the IObservedChange is set.
        /// </summary>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty.SomeOtherProperty'</param>
        /// <param name="beforeChange">If True, the Observable will notify
        /// immediately before a property is going to change.</param>
        /// <returns>An Observable representing the property change
        /// notifications for the given property.</returns>
        public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(
                this TSender This,
                Expression<Func<TSender, TValue>> property,
                bool beforeChange = false,
                bool skipInitial = true)
        {
            if (This == null) {
                throw new ArgumentNullException("Sender");
            }
            
            /* x => x.Foo.Bar.Baz;
             * 
             * Subscribe to This, look for Foo
             * Subscribe to Foo, look for Bar
             * Subscribe to Bar, look for Baz
             * Subscribe to Baz, publish to Subject
             * Return Subject
             * 
             * If Bar changes (notification fires on Foo), resubscribe to new Bar
             *  Resubscribe to new Baz, publish to Subject
             * 
             * If Baz changes (notification fires on Bar),
             *  Resubscribe to new Baz, publish to Subject
             */

            return SubscribeToExpressionChain<TSender, TValue>(
                This,
                property.Body,
                beforeChange,
                skipInitial);
        }

        /// <summary>
        /// ObservableForProperty returns an Observable representing the
        /// property change notifications for a specific property on a
        /// ReactiveObject, running the IObservedChange through a Selector
        /// function.
        /// </summary>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty'</param>
        /// <param name="selector">A Select function that will be run on each
        /// item.</param>
        /// <param name="beforeChange">If True, the Observable will notify
        /// immediately before a property is going to change.</param>
        /// <returns>An Observable representing the property change
        /// notifications for the given property.</returns>
        public static IObservable<TRet> ObservableForProperty<TSender, TValue, TRet>(
                this TSender This,
                Expression<Func<TSender, TValue>> property,
                Func<TValue, TRet> selector,
                bool beforeChange = false)
            where TSender : class
        {
            Contract.Requires(selector != null);
            return This.ObservableForProperty(property, beforeChange).Select(x => selector(x.Value));
        }

        public static IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TSender, TValue> ( 
            this TSender source,
            Expression expression, 
            bool beforeChange = false,
            bool skipInitial = true)
        {
            IObservable<IObservedChange<object, object>> notifier = 
                Observable.Return(new ObservedChange<object, object>(null, null, source));

            IEnumerable<Expression> chain = Reflection.Rewrite(expression).GetExpressionChain();
            notifier = chain.Aggregate(notifier, (n, expr) => n
                .Select(y => nestedObservedChanges(expr, y, beforeChange))
                .Switch());
            
            if (skipInitial) {
                notifier = notifier.Skip(1);
            }

            notifier = notifier.Where(x => x.Sender != null);

            var r = notifier.Select(x => {
                // ensure cast to TValue will succeed, throw useful exception otherwise
                var val = x.GetValue();
                if (val != null && ! (val is TValue)) {
                    throw new InvalidCastException(string.Format("Unable to cast from {0} to {1}.", val.GetType(), typeof(TValue)));
                }

                return new ObservedChange<TSender, TValue>(source, expression, (TValue) val);
            });

            return r.DistinctUntilChanged(x=>x.Value);
        }

        static IObservedChange<object, object> observedChangeFor(Expression expression, IObservedChange<object, object> sourceChange)
        {
            var propertyName = expression.GetMemberInfo().Name;
            if (sourceChange.Value == null) {
                return new ObservedChange<object, object>(sourceChange.Value, expression); ;
            } else {
                object value;
                // expression is always a simple expression
                Reflection.TryGetValueForPropertyChain(out value, sourceChange.Value, new[] { expression });
                return new ObservedChange<object, object>(sourceChange.Value, expression, value);
            }
        }

        static IObservable<IObservedChange<object, object>> nestedObservedChanges(Expression expression, IObservedChange<object, object> sourceChange, bool beforeChange)
        {
            // Make sure a change at a root node propogates events down
            var kicker = observedChangeFor(expression, sourceChange);

            // Handle null values in the chain
            if (sourceChange.Value == null) {
                return Observable.Return(kicker);
            }

            // Handle non null values in the chain
            return notifyForProperty(sourceChange.Value, expression, beforeChange)
                .Select(x => new ObservedChange<object, object>(x.Sender, expression, x.GetValue()))
                .StartWith(kicker);
        }

        static readonly MemoizingMRUCache<Tuple<Type, string, bool>, ICreatesObservableForProperty> notifyFactoryCache =
            new MemoizingMRUCache<Tuple<Type, string, bool>, ICreatesObservableForProperty>((t, _) => {
                return Locator.Current.GetServices<ICreatesObservableForProperty>()
                    .Aggregate(Tuple.Create(0, (ICreatesObservableForProperty)null), (acc, x) => {
                        int score = x.GetAffinityForObject(t.Item1, t.Item2, t.Item3);
                        return (score > acc.Item1) ? Tuple.Create(score, x) : acc;
                    }).Item2;
            }, RxApp.BigCacheLimit);

        static IObservable<IObservedChange<object, object>> notifyForProperty(object sender, Expression expression, bool beforeChange)
        {
            var result = default(ICreatesObservableForProperty);
            lock (notifyFactoryCache) {
                result = notifyFactoryCache.Get(Tuple.Create(sender.GetType(), expression.GetMemberInfo().Name, beforeChange));
            }

            if (result == null) {
                throw new Exception(
                    String.Format("Couldn't find a ICreatesObservableForProperty for {0}. This should never happen, your service locator is probably broken.", 
                    sender.GetType()));
            }
            
            return result.GetNotificationForProperty(sender, expression, beforeChange);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
