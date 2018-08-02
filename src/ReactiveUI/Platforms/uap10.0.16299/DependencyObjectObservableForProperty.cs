// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Splat;
using System;
using System.Diagnostics.Contracts;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Windows.UI.Xaml;

namespace ReactiveUI
{
    public class DependencyObjectObservableForProperty : ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            if (!typeof(DependencyObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo())) return 0;
            if (getDependencyPropertyFetcher(type, propertyName) == null) return 0;

            return 4;
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, System.Linq.Expressions.Expression expression, string propertyName, bool beforeChanged = false)
        {
            Contract.Requires(sender != null && sender is DependencyObject);
            var type = sender.GetType();
            var depSender = sender as DependencyObject;

            if (depSender == null)
            {
                this.Log().Warn("Tried to bind DP on a non-DependencyObject. Binding as POCO object",
                    type.FullName, propertyName);

                var ret = new POCOObservableForProperty();
                return ret.GetNotificationForProperty(sender, expression, propertyName, beforeChanged);
            }

            if (beforeChanged == true) {
                this.Log().Warn("Tried to bind DO {0}.{1}, but DPs can't do beforeChanged. Binding as POCO object",
                    type.FullName, propertyName);

                var ret = new POCOObservableForProperty();
                return ret.GetNotificationForProperty(sender, expression, propertyName, beforeChanged);
            }

            var dpFetcher = getDependencyPropertyFetcher(type, propertyName);
            if (dpFetcher == null) {
                this.Log().Warn("Tried to bind DO {0}.{1}, but DP doesn't exist. Binding as POCO object",
                    type.FullName, propertyName);

                var ret = new POCOObservableForProperty();
                return ret.GetNotificationForProperty(sender, expression, propertyName, beforeChanged);
            }

            return Observable.Create<IObservedChange<object, object>>(subj => {
                var handler = new DependencyPropertyChangedCallback((o, e) => {
                    subj.OnNext(new ObservedChange<object, object>(sender, expression));
                });
                var dependencyProperty = dpFetcher();
                var token = depSender.RegisterPropertyChangedCallback(dependencyProperty, handler);
                return Disposable.Create(() => depSender.UnregisterPropertyChangedCallback(dependencyProperty, token));
            });
        }

        Func<DependencyProperty> getDependencyPropertyFetcher(Type type, string propertyName)
        {
            var typeInfo = type.GetTypeInfo();

            // Look for the DependencyProperty attached to this property name
            var pi = actuallyGetProperty(typeInfo, propertyName + "Property");
            if (pi != null) {
                return () => (DependencyProperty)pi.GetValue(null);
            }

            var fi = actuallyGetField(typeInfo, propertyName + "Property");
            if (fi != null) {
                return () => (DependencyProperty)fi.GetValue(null);
            }

            return null;
        }

        PropertyInfo actuallyGetProperty(TypeInfo typeInfo, string propertyName)
        {
            var current = typeInfo;
            while (current != null) {
                var ret = current.GetDeclaredProperty(propertyName);
                if (ret != null && ret.IsStatic()) return ret;

                current = current.BaseType != null ? current.BaseType.GetTypeInfo() : null;
            }

            return null;
        }

        FieldInfo actuallyGetField(TypeInfo typeInfo, string propertyName)
        {
            var current = typeInfo;
            while (current != null) {
                var ret = current.GetDeclaredField(propertyName);
                if (ret != null && ret.IsStatic) return ret;

                current = current.BaseType != null ? current.BaseType.GetTypeInfo() : null;
            }

            return null;
        }
    }
}
