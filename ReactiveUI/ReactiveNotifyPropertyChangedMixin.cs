﻿using System;
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

            var propertyNames = Reflection.ExpressionToPropertyNames(property);

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
                propertyNames,
                beforeChange,
                skipInitial);
        }

        /// <summary>
        /// ObservableForProperty returns an Observable representing the
        /// property change notifications for a specific property on a
        /// ReactiveObject. This method (unlike other Observables that return
        /// IObservedChange) guarantees that the Value property of
        /// the IObservedChange is set.
        /// </summary>
        /// <param name="propertyName">A string containing the property name</param>
        /// <param name="beforeChange">If True, the Observable will notify
        /// immediately before a property is going to change.</param>
        /// <returns>An Observable representing the property change
        /// notifications for the given property.</returns>
        public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(
            this TSender This,
            string propertyName,
            bool beforeChange = false,
            bool skipInitial = true)
        {
            var values = notifyForProperty(This, propertyName, beforeChange);

            if (!skipInitial) {
                values = values.StartWith(new ObservedChange<object, object>(This, propertyName));
            }

            return values.Select(x => new ObservedChange<TSender, TValue>(This, propertyName, (TValue)x.GetValue()))
                 .DistinctUntilChanged(x => x.Value);
        }

        /// <summary>
        /// ObservableForPropertyDynamic returns an Observable representing the
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
        public static IObservable<IObservedChange<TSender, object>> ObservableForProperty<TSender>(
                this TSender This,
                string[] property,
                bool beforeChange = false,
                bool skipInitial = true)
        {
            var propertyNames = new LinkedList<string>(property);

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

            return SubscribeToExpressionChain<TSender, object>(
                This,
                propertyNames,
                beforeChange,
                skipInitial);
        }


        static IObservedChange<object, object> observedChangeFor(string propertyName, IObservedChange<object, object> sourceChange)
        {
            if (sourceChange.Value == null) {
                return new ObservedChange<object, object>(sourceChange.Value, propertyName);;
            }
            else
            {
                object value;
                Reflection.TryGetValueForPropertyChain(out value, sourceChange.Value, propertyName.Split('.'));
                return new ObservedChange<object, object>(sourceChange.Value, propertyName, value);
            }
        }

        static IObservable<IObservedChange<object, object>> nestedObservedChanges(string propertyName, IObservedChange<object, object> sourceChange, bool beforeChange)
        {
            // Make sure a change at a root node propogates events down
            var kicker = observedChangeFor(propertyName, sourceChange);

            // Handle null values in the chain
            if (sourceChange.Value == null) {
                return Observable.Return(kicker);
            }

            // Handle non null values in the chain
            return notifyForProperty(sourceChange.Value, propertyName, beforeChange)
                .Select(x => new ObservedChange<object, object>(x.Sender, x.PropertyName, x.GetValue()))
                .StartWith(kicker);
        }

        public static IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TSender, TValue> ( 
            this TSender source,
            IEnumerable<string> propertyNames, 
            bool beforeChange = false,
            bool skipInitial = true)
        {
            var path = String.Join(".", propertyNames);

            IObservable<IObservedChange<object, object>> notifier = 
                Observable.Return(new ObservedChange<object, object>(null, null, source));

            notifier = propertyNames.Aggregate(notifier, (n, name) => n
                .Select(y => nestedObservedChanges(name, y, beforeChange))
                .Switch());

            if (skipInitial) {
                notifier = notifier.Skip(1);
            }

            notifier = notifier.Where(x => x.Sender != null);

            var r = notifier.Select(x => new ObservedChange<TSender, TValue>(source, path, (TValue)x.GetValue()));

            return r.DistinctUntilChanged(x=>x.Value);
        }

        static readonly MemoizingMRUCache<Tuple<Type, string, bool>, ICreatesObservableForProperty> notifyFactoryCache =
            new MemoizingMRUCache<Tuple<Type, string, bool>, ICreatesObservableForProperty>((t, _) => {
                return Locator.Current.GetServices<ICreatesObservableForProperty>()
                    .Aggregate(Tuple.Create(0, (ICreatesObservableForProperty)null), (acc, x) => {
                        int score = x.GetAffinityForObject(t.Item1, t.Item2, t.Item3);
                        return (score > acc.Item1) ? Tuple.Create(score, x) : acc;
                    }).Item2;
            }, RxApp.BigCacheLimit);

        static IObservable<IObservedChange<object, object>> notifyForProperty(object sender, string propertyName, bool beforeChange)
        {
            var result = default(ICreatesObservableForProperty);
            lock (notifyFactoryCache) {
                result = notifyFactoryCache.Get(Tuple.Create(sender.GetType(), propertyName, beforeChange));
            }

            if (result == null) {
                throw new Exception(
                    String.Format("Couldn't find a ICreatesObservableForProperty for {0}. This should never happen, your service locator is probably broken.", 
                    sender.GetType()));
            }
            
            return result.GetNotificationForProperty(sender, propertyName, beforeChange);
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
    }
}

// vim: tw=120 ts=4 sw=4 et :
