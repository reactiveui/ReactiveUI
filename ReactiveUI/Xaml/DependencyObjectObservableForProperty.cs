﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;
using Splat;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
#endif

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

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, System.Linq.Expressions.Expression expression, bool beforeChanged = false)
        {
            Contract.Requires(sender != null && sender is DependencyObject);
            var type = sender.GetType();
            var propertyName = expression.GetMemberInfo().Name;

            if (beforeChanged == true) {
                this.Log().Warn("Tried to bind DO {0}.{1}, but DPs can't do beforeChanged. Binding as POCO object",
                    type.FullName, propertyName);

                var ret = new POCOObservableForProperty();
                return ret.GetNotificationForProperty(sender, expression, beforeChanged);
            }

            var dpFetcher = getDependencyPropertyFetcher(type, propertyName);
            if (dpFetcher == null) {
                this.Log().Warn("Tried to bind DO {0}.{1}, but DP doesn't exist. Binding as POCO object",
                    type.FullName, propertyName);

                var ret = new POCOObservableForProperty();
                return ret.GetNotificationForProperty(sender, expression, beforeChanged);
            }

            var dpAndSubj = createAttachedProperty(type, propertyName);

            return Observable.Create<IObservedChange<object, object>>(obs => {
                BindingOperations.SetBinding(sender as DependencyObject, dpAndSubj.Item1,
                    new Binding() { Source = sender as DependencyObject, Path = new PropertyPath(propertyName) });

                var disp = dpAndSubj.Item2
                    .Where(x => x == sender)
                    .Select(x => new ObservedChange<object, object>(x, expression))
                    .Subscribe(obs);

                // ClearBinding calls ClearValue http://stackoverflow.com/questions/1639219/clear-binding-in-silverlight-remove-data-binding-from-setbinding
                return new CompositeDisposable(Disposable.Create(() => (sender as DependencyObject).ClearValue(dpAndSubj.Item1)), disp);
            });
        }

        Func<DependencyProperty> getDependencyPropertyFetcher(Type type, string propertyName)
        {
            var typeInfo = type.GetTypeInfo();
#if WINRT
            // Look for the DependencyProperty attached to this property name
            var pi = actuallyGetProperty(typeInfo, propertyName + "Property");
            if (pi != null) {
                return () => (DependencyProperty)pi.GetValue(null);
            }
#endif

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

        static readonly Dictionary<Tuple<Type, string>, Tuple<DependencyProperty, Subject<object>>> attachedListener =
            new Dictionary<Tuple<Type, string>, Tuple<DependencyProperty, Subject<object>>>();

        Tuple<DependencyProperty, Subject<object>> createAttachedProperty(Type type, string propertyName)
        {
            var pair = Tuple.Create(type, propertyName);
            if (attachedListener.ContainsKey(pair)) return attachedListener[pair];

            var subj = new Subject<object>();

            // NB: There is no way to unregister an attached property, 
            // we just have to leak it. Luckily it's per-type, so it's
            // not *that* bad.
            var dp = DependencyProperty.RegisterAttached(
                "ListenAttached" + propertyName + this.GetHashCode().ToString("{0:x}"),
                typeof(object), type,
                new PropertyMetadata(null, (o, e) => subj.OnNext(o)));

            var ret = Tuple.Create(dp, subj);
            attachedListener[pair] = ret;
            return ret;
        }
    }
}
