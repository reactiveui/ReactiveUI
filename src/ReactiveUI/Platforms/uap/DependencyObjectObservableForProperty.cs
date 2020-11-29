// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using Splat;
using Windows.UI.Xaml;

#if HAS_UNO
namespace ReactiveUI.Uno
#else
namespace ReactiveUI
#endif
{
    /// <summary>
    /// Creates a observable for a property if available that is based on a DependencyProperty.
    /// </summary>
    public class DependencyObjectObservableForProperty : ICreatesObservableForProperty
    {
        /// <inheritdoc/>
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            if (!typeof(DependencyObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                return 0;
            }

            if (GetDependencyPropertyFetcher(type, propertyName) == null)
            {
                return 0;
            }

            return 6;
        }

        /// <inheritdoc/>
        public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            var depSender = sender as DependencyObject;

            if (depSender == null)
            {
                throw new ArgumentException("The sender must be a DependencyObject", nameof(sender));
            }

            var type = sender.GetType();

            if (beforeChanged)
            {
                this.Log().Warn(
                    CultureInfo.InvariantCulture,
                    "Tried to bind DO {0}.{1}, but DPs can't do beforeChanged. Binding as POCO object",
                    type.FullName,
                    propertyName);

                var ret = new POCOObservableForProperty();
                return ret.GetNotificationForProperty(sender, expression, propertyName, beforeChanged);
            }

            var dpFetcher = GetDependencyPropertyFetcher(type, propertyName);
            if (dpFetcher == null)
            {
                this.Log().Warn(
                    CultureInfo.InvariantCulture,
                    "Tried to bind DO {0}.{1}, but DP doesn't exist. Binding as POCO object",
                    type.FullName,
                    propertyName);

                var ret = new POCOObservableForProperty();
                return ret.GetNotificationForProperty(sender, expression, propertyName, beforeChanged);
            }

            return Observable.Create<IObservedChange<object, object?>>(subj =>
            {
                var handler = new DependencyPropertyChangedCallback((_, _) =>
                {
                    subj.OnNext(new ObservedChange<object, object?>(sender, expression, default));
                });
                var dependencyProperty = dpFetcher();
                var token = depSender.RegisterPropertyChangedCallback(dependencyProperty, handler);
                return Disposable.Create(() => depSender.UnregisterPropertyChangedCallback(dependencyProperty, token));
            });
        }

        private static PropertyInfo? ActuallyGetProperty(TypeInfo typeInfo, string propertyName)
        {
            var current = typeInfo;
            while (current != null)
            {
                var ret = current.GetDeclaredProperty(propertyName);
                if (ret != null && ret.IsStatic())
                {
                    return ret;
                }

                current = current.BaseType != null ? current.BaseType.GetTypeInfo() : null;
            }

            return null;
        }

        private static FieldInfo? ActuallyGetField(TypeInfo typeInfo, string propertyName)
        {
            var current = typeInfo;
            while (current != null)
            {
                var ret = current.GetDeclaredField(propertyName);
                if (ret != null && ret.IsStatic)
                {
                    return ret;
                }

                current = current.BaseType != null ? current.BaseType.GetTypeInfo() : null;
            }

            return null;
        }

        private static Func<DependencyProperty>? GetDependencyPropertyFetcher(Type type, string propertyName)
        {
            var typeInfo = type.GetTypeInfo();

            // Look for the DependencyProperty attached to this property name
            var pi = ActuallyGetProperty(typeInfo, propertyName + "Property");
            if (pi != null)
            {
                return () => (DependencyProperty)pi.GetValue(null);
            }

            var fi = ActuallyGetField(typeInfo, propertyName + "Property");
            if (fi != null)
            {
                return () => (DependencyProperty)fi.GetValue(null);
            }

            return null;
        }
    }
}
