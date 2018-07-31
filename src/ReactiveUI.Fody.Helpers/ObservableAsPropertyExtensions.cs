// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reflection;

namespace ReactiveUI.Fody.Helpers
{
    public static class ObservableAsPropertyExtensions
    {
        public static ObservableAsPropertyHelper<TRet> ToPropertyEx<TObj, TRet>(this IObservable<TRet> @this, TObj source, Expression<Func<TObj, TRet>> property, TRet initialValue = default(TRet), bool deferSubscription = false, IScheduler scheduler = null) where TObj : ReactiveObject
        {
            var result = @this.ToProperty(source, property, initialValue, deferSubscription, scheduler);

            // Now assign the field via reflection.
            var propertyInfo = property.GetPropertyInfo();
            if (propertyInfo == null)
                throw new Exception("Could not resolve expression " + property + " into a property.");

            var field = propertyInfo.DeclaringType.GetTypeInfo().GetDeclaredField("$" + propertyInfo.Name);
            if (field == null)
                throw new Exception("Backing field not found for " + propertyInfo);
            field.SetValue(source, result);

            return result;
        }

        static PropertyInfo GetPropertyInfo(this LambdaExpression expression)
        {
            var current = expression.Body;
            var unary = current as UnaryExpression;
            if (unary != null)
                current = unary.Operand;
            var call = (MemberExpression)current;
            return (PropertyInfo)call.Member;
        }
    }
}
