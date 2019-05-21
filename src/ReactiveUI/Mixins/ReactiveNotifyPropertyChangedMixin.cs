﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Extension methods associated with the Observable Changes and the
    /// Reactive Notify Property Changed based events.
    /// </summary>
    public static class ReactiveNotifyPropertyChangedMixin
    {
        private static readonly MemoizingMRUCache<Tuple<Type, string, bool>, ICreatesObservableForProperty> notifyFactoryCache =
            new MemoizingMRUCache<Tuple<Type, string, bool>, ICreatesObservableForProperty>(
                (t, _) =>
                {
                    return Locator.Current.GetServices<ICreatesObservableForProperty>()
                                  .Aggregate(Tuple.Create(0, (ICreatesObservableForProperty)null), (acc, x) =>
                                  {
                                      int score = x.GetAffinityForObject(t.Item1, t.Item2, t.Item3);
                                      return score > acc.Item1 ? Tuple.Create(score, x) : acc;
                                  }).Item2;
                }, RxApp.BigCacheLimit);

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
        /// <typeparam name="TSender">The sender type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="this">The source object to observe properties of.</param>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty.SomeOtherProperty'.</param>
        /// <param name="beforeChange">If True, the Observable will notify
        /// immediately before a property is going to change.</param>
        /// <param name="skipInitial">If true, the Observable will not notify
        /// with the initial value.</param>
        /// <returns>An Observable representing the property change
        /// notifications for the given property.</returns>
        public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(
                this TSender @this,
                Expression<Func<TSender, TValue>> property,
                bool beforeChange = false,
                bool skipInitial = true)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
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
                @this,
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
        /// <typeparam name="TSender">The sender type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <typeparam name="TRet">The return value type.</typeparam>
        /// <param name="this">The source object to observe properties of.</param>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty'.</param>
        /// <param name="selector">A Select function that will be run on each
        /// item.</param>
        /// <param name="beforeChange">If True, the Observable will notify
        /// immediately before a property is going to change.</param>
        /// <returns>An Observable representing the property change
        /// notifications for the given property.</returns>
        public static IObservable<TRet> ObservableForProperty<TSender, TValue, TRet>(
                this TSender @this,
                Expression<Func<TSender, TValue>> property,
                Func<TValue, TRet> selector,
                bool beforeChange = false)
            where TSender : class
        {
            Contract.Requires(selector != null);
            return @this.ObservableForProperty(property, beforeChange).Select(x => selector(x.Value));
        }

        /// <summary>
        /// Creates a observable which will subscribe to the each property and sub property
        /// specified in the Expression. eg It will subscribe to x => x.Property1.Property2.Property3
        /// each property in the lambda expression. It will then provide updates to the last value in the chain.
        /// </summary>
        /// <param name="source">The object where we start the chain.</param>
        /// <param name="expression">A expression which will point towards the property.</param>
        /// <param name="beforeChange">If we are interested in notifications before the property value is changed.</param>
        /// <param name="skipInitial">If we don't want to get a notification about the default value of the property.</param>
        /// <param name="suppressWarnings">If true, no warnings should be logged.</param>
        /// <typeparam name="TSender">The type of the origin of the expression chain.</typeparam>
        /// <typeparam name="TValue">The end value we want to subscribe to.</typeparam>
        /// <returns>A observable which notifies about observed changes.</returns>
        /// <exception cref="InvalidCastException">If we cannot cast from the target value from the specified last property.</exception>
        public static IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TSender, TValue>(
            this TSender source,
            Expression expression,
            bool beforeChange = false,
            bool skipInitial = true,
            bool suppressWarnings = false)
        {
            IObservable<IObservedChange<object, object>> notifier =
                Observable.Return(new ObservedChange<object, object>(null, null, source));

            IEnumerable<Expression> chain = Reflection.Rewrite(expression).GetExpressionChain();
            notifier = chain.Aggregate(notifier, (n, expr) => n
                .Select(y => NestedObservedChanges(expr, y, beforeChange, suppressWarnings))
                .Switch());

            if (skipInitial)
            {
                notifier = notifier.Skip(1);
            }

            notifier = notifier.Where(x => x.Sender != null);

            var r = notifier.Select(x =>
            {
                // ensure cast to TValue will succeed, throw useful exception otherwise
                var val = x.GetValue();
                if (val != null && !(val is TValue))
                {
                    throw new InvalidCastException($"Unable to cast from {val.GetType()} to {typeof(TValue)}.");
                }

                return new ObservedChange<TSender, TValue>(source, expression, (TValue)val);
            });

            return r.DistinctUntilChanged(x => x.Value);
        }

        private static IObservedChange<object, object> ObservedChangeFor(Expression expression, IObservedChange<object, object> sourceChange)
        {
            var propertyName = expression.GetMemberInfo().Name;
            if (sourceChange.Value == null)
            {
                return new ObservedChange<object, object>(sourceChange.Value, expression);
            }

            // expression is always a simple expression
            Reflection.TryGetValueForPropertyChain(out object value, sourceChange.Value, new[] { expression });

            return new ObservedChange<object, object>(sourceChange.Value, expression, value);
        }

        private static IObservable<IObservedChange<object, object>> NestedObservedChanges(Expression expression, IObservedChange<object, object> sourceChange, bool beforeChange, bool suppressWarnings)
        {
            // Make sure a change at a root node propogates events down
            var kicker = ObservedChangeFor(expression, sourceChange);

            // Handle null values in the chain
            if (sourceChange.Value == null)
            {
                return Observable.Return(kicker);
            }

            // Handle non null values in the chain
            return NotifyForProperty(sourceChange.Value, expression, beforeChange, suppressWarnings)
                .Select(x => new ObservedChange<object, object>(x.Sender, expression, x.GetValue()))
                .StartWith(kicker);
        }

        private static IObservable<IObservedChange<object, object>> NotifyForProperty(object sender, Expression expression, bool beforeChange, bool suppressWarnings)
        {
            var propertyName = expression.GetMemberInfo().Name;
            var result = notifyFactoryCache.Get(Tuple.Create(sender.GetType(), propertyName, beforeChange));

            if (result == null)
            {
                throw new Exception($"Could not find a ICreatesObservableForProperty for {sender.GetType()} property {propertyName}. This should never happen, your service locator is probably broken. Please make sure you have installed the latest version of the ReactiveUI packages for your platform. See https://reactiveui.net/docs/getting-started/installation/nuget-packages for guidance.");
            }

            return result.GetNotificationForProperty(sender, expression, propertyName, beforeChange, suppressWarnings);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
