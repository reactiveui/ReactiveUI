﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;

namespace ReactiveUI
{
    /// <summary>
    /// Generates Observables based on observing INotifyPropertyChanged objects.
    /// </summary>
    public class INPCObservableForProperty : ICreatesObservableForProperty
    {
        /// <inheritdoc/>
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged)
        {
            var target = beforeChanged ? typeof(INotifyPropertyChanging) : typeof(INotifyPropertyChanged);
            return target.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) ? 5 : 0;
        }

        /// <inheritdoc/>
        [SuppressMessage("Roslynator", "RCS1211", Justification = "Neater with else clause.")]
        public IObservable<IObservedChange<object, object>>? GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var before = sender as INotifyPropertyChanging;
            var after = sender as INotifyPropertyChanged;

            if (beforeChanged ? before == null : after == null)
            {
                return Observable<IObservedChange<object, object>>.Never;
            }

            if (beforeChanged)
            {
                var obs = Observable.FromEvent<PropertyChangingEventHandler, string>(
                    eventHandler =>
                    {
                        void Handler(object eventSender, PropertyChangingEventArgs e) => eventHandler(e.PropertyName);
                        return Handler;
                    },
                    x => before!.PropertyChanging += x,
                    x => before!.PropertyChanging -= x);

                if (expression.NodeType == ExpressionType.Index)
                {
                    return obs.Where(x => string.IsNullOrEmpty(x)
                        || x.Equals(propertyName + "[]", StringComparison.InvariantCulture))
                        .Select(x => new ObservedChange<object, object>(sender, expression));
                }

                return obs.Where(x => string.IsNullOrEmpty(x)
                    || x.Equals(propertyName, StringComparison.InvariantCulture))
                .Select(x => new ObservedChange<object, object>(sender, expression));
            }
            else
            {
                var obs = Observable.FromEvent<PropertyChangedEventHandler, string>(
                    eventHandler =>
                    {
                        void Handler(object eventSender, PropertyChangedEventArgs e) => eventHandler(e.PropertyName);
                        return Handler;
                    },
                    x => after!.PropertyChanged += x,
                    x => after!.PropertyChanged -= x);

                if (expression.NodeType == ExpressionType.Index)
                {
                    return obs.Where(x => string.IsNullOrEmpty(x)
                        || x.Equals(propertyName + "[]", StringComparison.InvariantCulture))
                    .Select(x => new ObservedChange<object, object>(sender, expression));
                }

                return obs.Where(x => string.IsNullOrEmpty(x)
                    || x.Equals(propertyName, StringComparison.InvariantCulture))
                .Select(x => new ObservedChange<object, object>(sender, expression));
            }
        }
    }
}
