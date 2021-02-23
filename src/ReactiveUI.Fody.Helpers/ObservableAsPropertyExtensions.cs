// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reflection;

namespace ReactiveUI.Fody.Helpers
{
    /// <summary>
    /// Extension methods for observable as property helpers.
    /// </summary>
    public static class ObservableAsPropertyExtensions
    {
        /// <summary>
        /// To the property execute.
        /// </summary>
        /// <typeparam name="TObj">The type of the object.</typeparam>
        /// <typeparam name="TRet">The type of the ret.</typeparam>
        /// <param name="item">The observable with the return value.</param>
        /// <param name="source">The source.</param>
        /// <param name="property">The property.</param>
        /// <param name="deferSubscription">if set to <c>true</c> [defer subscription].</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns>An observable property helper with the specified return value.</returns>
        /// <exception cref="Exception">
        /// Could not resolve expression " + property + " into a property.
        /// or
        /// Backing field not found for " + propertyInfo.
        /// </exception>
        public static ObservableAsPropertyHelper<TRet> ToPropertyEx<TObj, TRet>(this IObservable<TRet> item, TObj source, Expression<Func<TObj, TRet>> property, bool deferSubscription = false, IScheduler? scheduler = null)
            where TObj : ReactiveObject
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var result = item.ToProperty(source, property, deferSubscription, scheduler);

            // Now assign the field via reflection.
            var propertyInfo = property.GetPropertyInfo();
            if (propertyInfo is null)
            {
                throw new Exception("Could not resolve expression " + property + " into a property.");
            }

            var field = propertyInfo.DeclaringType?.GetTypeInfo().GetDeclaredField("$" + propertyInfo.Name);
            if (field is null)
            {
                throw new Exception("Backing field not found for " + propertyInfo);
            }

            field.SetValue(source, result);

            return result;
        }

        /// <summary>
        /// To the property execute.
        /// </summary>
        /// <typeparam name="TObj">The type of the object.</typeparam>
        /// <typeparam name="TRet">The type of the ret.</typeparam>
        /// <param name="item">The observable with the return value.</param>
        /// <param name="source">The source.</param>
        /// <param name="property">The property.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="deferSubscription">if set to <c>true</c> [defer subscription].</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns>An observable property helper with the specified return value.</returns>
        /// <exception cref="Exception">
        /// Could not resolve expression " + property + " into a property.
        /// or
        /// Backing field not found for " + propertyInfo.
        /// </exception>
        public static ObservableAsPropertyHelper<TRet> ToPropertyEx<TObj, TRet>(this IObservable<TRet> item, TObj source, Expression<Func<TObj, TRet>> property, TRet initialValue, bool deferSubscription = false, IScheduler? scheduler = null)
            where TObj : ReactiveObject
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var result = item.ToProperty(source, property, initialValue, deferSubscription, scheduler);

            // Now assign the field via reflection.
            var propertyInfo = property.GetPropertyInfo();
            if (propertyInfo is null)
            {
                throw new Exception("Could not resolve expression " + property + " into a property.");
            }

            var field = propertyInfo.DeclaringType?.GetTypeInfo().GetDeclaredField("$" + propertyInfo.Name);
            if (field is null)
            {
                throw new Exception("Backing field not found for " + propertyInfo);
            }

            field.SetValue(source, result);

            return result;
        }

        private static PropertyInfo GetPropertyInfo(this LambdaExpression expression)
        {
            var current = expression.Body;
            if (current is UnaryExpression unary)
            {
                current = unary.Operand;
            }

            var call = (MemberExpression)current;
            return (PropertyInfo)call.Member;
        }
    }
}
