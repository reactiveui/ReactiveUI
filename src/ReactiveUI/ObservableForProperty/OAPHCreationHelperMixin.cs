// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reactive.Concurrency;

namespace ReactiveUI
{
    /// <summary>
    /// A collection of helpers to aid working with observable properties.
    /// </summary>
    public static class OAPHCreationHelperMixin
    {
        /// <summary>
        /// Converts an Observable to an ObservableAsPropertyHelper and
        /// automatically provides the onChanged method to raise the property
        /// changed notification.
        /// </summary>
        /// <typeparam name="TObj">The onject type.</typeparam>
        /// <typeparam name="TRet">The result type.</typeparam>
        /// <param name="target">
        /// The observable to convert to an ObservableAsPropertyHelper.
        /// </param>
        /// <param name="source">
        /// The ReactiveObject that has the property.
        /// </param>
        /// <param name="property">
        /// An Expression representing the property (i.e. <c>x => x.SomeProperty</c>).
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the property.
        /// </param>
        /// <param name="deferSubscription">
        /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
        /// should defer the subscription to the <paramref name="target"/> source
        /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
        /// or if it should immediately subscribe to the the <paramref name="target"/> source.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will be provided on - this should normally
        /// be a Dispatcher-based scheduler.
        /// </param>
        /// <returns>
        /// An initialized ObservableAsPropertyHelper; use this as the backing field
        /// for your property.
        /// </returns>
        public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
            this IObservable<TRet> target,
            TObj source,
            Expression<Func<TObj, TRet>> property,
            TRet initialValue = default(TRet),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            where TObj : class, IReactiveObject => source.ObservableToProperty(target, property, initialValue, deferSubscription, scheduler);

        /// <summary>
        /// Converts an Observable to an ObservableAsPropertyHelper and
        /// automatically provides the onChanged method to raise the property
        /// changed notification.
        /// </summary>
        /// <typeparam name="TObj">The onject type.</typeparam>
        /// <typeparam name="TRet">The result type.</typeparam>
        /// <param name="target">
        /// The observable to convert to an ObservableAsPropertyHelper.
        /// </param>
        /// <param name="source">
        /// The ReactiveObject that has the property.
        /// </param>
        /// <param name="property">
        /// An Expression representing the property (i.e. <c>x => x.SomeProperty</c>).
        /// </param>
        /// <param name="result">
        /// An out param matching the return value, provided for convenience.
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the property.
        /// </param>
        /// <param name="deferSubscription">
        /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
        /// should defer the subscription to the <paramref name="target"/> source
        /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
        /// or if it should immediately subscribe to the the <paramref name="target"/> source.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will be provided on - this should
        /// normally be a Dispatcher-based scheduler.
        /// </param>
        /// <returns>
        /// An initialized ObservableAsPropertyHelper; use this as the backing
        /// field for your property.
        /// </returns>
        public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
            this IObservable<TRet> target,
            TObj source,
            Expression<Func<TObj, TRet>> property,
            out ObservableAsPropertyHelper<TRet> result,
            TRet initialValue = default(TRet),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            where TObj : class, IReactiveObject
        {
            var ret = source.ObservableToProperty(target, property, initialValue, deferSubscription, scheduler);

            result = ret;
            return ret;
        }

        /// <summary>
        /// Converts an Observable to an ObservableAsPropertyHelper and
        /// automatically provides the onChanged method to raise the property
        /// changed notification.
        /// </summary>
        /// <typeparam name="TObj">The onject type.</typeparam>
        /// <typeparam name="TRet">The result type.</typeparam>
        /// <param name="target">
        /// The observable to convert to an ObservableAsPropertyHelper.
        /// </param>
        /// <param name="source">
        /// The ReactiveObject that has the property.
        /// </param>
        /// <param name="property">
        /// The name of the property that has changed. Recommended for use with nameof() or a FODY.
        /// or a fody.
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the property.
        /// </param>
        /// <param name="deferSubscription">
        /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
        /// should defer the subscription to the <paramref name="target"/> source
        /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
        /// or if it should immediately subscribe to the the <paramref name="target"/> source.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will be provided on - this should normally
        /// be a Dispatcher-based scheduler.
        /// </param>
        /// <returns>
        /// An initialized ObservableAsPropertyHelper; use this as the backing field
        /// for your property.
        /// </returns>
        public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
            this IObservable<TRet> target,
            TObj source,
            string property,
            TRet initialValue = default(TRet),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            where TObj : class, IReactiveObject => source.ObservableToProperty(target, property, initialValue, deferSubscription, scheduler);

        /// <summary>
        /// Converts an Observable to an ObservableAsPropertyHelper and
        /// automatically provides the onChanged method to raise the property
        /// changed notification.
        /// </summary>
        /// <typeparam name="TObj">The onject type.</typeparam>
        /// <typeparam name="TRet">The result type.</typeparam>
        /// <param name="target">
        /// The observable to convert to an ObservableAsPropertyHelper.
        /// </param>
        /// <param name="source">
        /// The ReactiveObject that has the property.
        /// </param>
        /// <param name="property">
        /// The name of the property that has changed. Recommended for use with nameof() or a FODY.
        /// </param>
        /// <param name="result">
        /// An out param matching the return value, provided for convenience.
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the property.
        /// </param>
        /// <param name="deferSubscription">
        /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
        /// should defer the subscription to the <paramref name="target"/> source
        /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
        /// or if it should immediately subscribe to the the <paramref name="target"/> source.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will be provided on - this should
        /// normally be a Dispatcher-based scheduler.
        /// </param>
        /// <returns>
        /// An initialized ObservableAsPropertyHelper; use this as the backing
        /// field for your property.
        /// </returns>
        public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
            this IObservable<TRet> target,
            TObj source,
            string property,
            out ObservableAsPropertyHelper<TRet> result,
            TRet initialValue = default(TRet),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            where TObj : class, IReactiveObject
        {
            result = source.ObservableToProperty(
                                                 target,
                                                 property,
                                                 initialValue,
                                                 deferSubscription,
                                                 scheduler);

            return result;
        }

        private static ObservableAsPropertyHelper<TRet> ObservableToProperty<TObj, TRet>(
            this TObj target,
            IObservable<TRet> observable,
            Expression<Func<TObj, TRet>> property,
            TRet initialValue = default(TRet),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            where TObj : class, IReactiveObject
        {
            Contract.Requires(target != null);
            Contract.Requires(observable != null);
            Contract.Requires(property != null);

            Expression expression = Reflection.Rewrite(property.Body);

            if (expression.GetParent().NodeType != ExpressionType.Parameter)
            {
                throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty'");
            }

            var name = expression.GetMemberInfo().Name;
            if (expression is IndexExpression)
            {
                name += "[]";
            }

            var ret = new ObservableAsPropertyHelper<TRet>(
                observable,
                _ => target.RaisingPropertyChanged(name),
                _ => target.RaisingPropertyChanging(name),
                initialValue,
                deferSubscription,
                scheduler);

            return ret;
        }

        private static ObservableAsPropertyHelper<TRet> ObservableToProperty<TObj, TRet>(
            this TObj target,
            IObservable<TRet> observable,
            string property,
            TRet initialValue = default(TRet),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            where TObj : class, IReactiveObject
        {
            Contract.Requires(target != null);
            Contract.Requires(observable != null);
            Contract.Requires(property != null);

            return new ObservableAsPropertyHelper<TRet>(
                                                        observable,
                                                        _ => target.RaisingPropertyChanged(property),
                                                        _ => target.RaisingPropertyChanging(property),
                                                        initialValue,
                                                        deferSubscription,
                                                        scheduler);
        }
    }
}
