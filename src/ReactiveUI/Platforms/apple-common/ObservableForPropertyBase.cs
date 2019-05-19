﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Foundation;

#if UIKIT
using UIKit;
#else
using AppKit;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// ObservableForPropertyBase represents an object that knows how to
    /// create notifications for a given type of object. Implement this if you
    /// are porting RxUI to a new UI toolkit, or generally want to enable WhenAny
    /// for another type of object that can be observed in a unique way.
    /// </summary>
    [Preserve]
    public abstract class ObservableForPropertyBase : ICreatesObservableForProperty
    {
        /// <summary>
        /// Configuration map.
        /// </summary>
        private readonly Dictionary<Type, Dictionary<string, ObservablePropertyInfo>> _config =
            new Dictionary<Type, Dictionary<string, ObservablePropertyInfo>>();

        /// <inheritdoc/>
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            if (beforeChanged)
            {
                return 0;
            }

            var match = _config.Keys
                               .Where(x => x.IsAssignableFrom(type) && _config[x].Keys.Contains(propertyName))
                               .Select(x => _config[x][propertyName])
                               .OrderByDescending(x => x.Affinity)
                               .FirstOrDefault();

            if (match == null)
            {
                return 0;
            }

            return match.Affinity;
        }

        /// <inheritdoc/>
        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
        {
            if (beforeChanged)
            {
                return Observable<IObservedChange<object, object>>.Never;
            }

            var type = sender.GetType();

            var match = _config.Keys
                               .Where(x => x.IsAssignableFrom(type) && _config[x].Keys.Contains(propertyName))
                               .Select(x => _config[x][propertyName])
                               .OrderByDescending(x => x.Affinity)
                               .FirstOrDefault();

            if (match == null)
            {
                throw new NotSupportedException($"Notifications for {type.Name}.{propertyName} are not supported");
            }

            return match.CreateObservable((NSObject)sender, expression);
        }

#if UIKIT
        /// <summary>
        /// Creates an Observable for a UIControl Event.
        /// </summary>
        /// <returns>An observable.</returns>
        /// <param name="sender">The sender.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="evt">The control event to listen for.</param>
        protected static IObservable<IObservedChange<object, object>> ObservableFromUIControlEvent(NSObject sender, Expression expression, UIControlEvent evt)
        {
            return Observable.Create<IObservedChange<object, object>>(subj =>
            {
                var control = (UIControl)sender;

                EventHandler handler = (s, e) => subj.OnNext(new ObservedChange<object, object>(sender, expression));

                control.AddTarget(handler, evt);

                return Disposable.Create(() => control.RemoveTarget(handler, evt));
            });
        }
#endif

        /// <summary>
        /// Creates an Observable for a NSNotificationCenter notification.
        /// </summary>
        /// <returns>The from notification.</returns>
        /// <param name="sender">Sender.</param>
        /// <param name="expression">Expression.</param>
        /// <param name="notification">Notification.</param>
        protected static IObservable<IObservedChange<object, object>> ObservableFromNotification(NSObject sender, Expression expression, NSString notification)
        {
            return Observable.Create<IObservedChange<object, object>>(subj =>
            {
                var handle = NSNotificationCenter.DefaultCenter.AddObserver(
                    notification,
                    _ => subj.OnNext(new ObservedChange<object, object>(sender, expression)),
                    sender);

                return Disposable.Create(() =>
                {
                    NSNotificationCenter.DefaultCenter.RemoveObserver(handle);
                });
            });
        }

        /// <summary>
        /// Creates an Observable for a NSNotificationCenter notification.
        /// </summary>
        /// <returns>The from notification.</returns>
        /// <param name="sender">Sender.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="eventName">The event name.</param>
        protected static IObservable<IObservedChange<object, object>> ObservableFromEvent(NSObject sender, Expression expression, string eventName)
        {
            return Observable.Create<IObservedChange<object, object>>(subj =>
            {
                return Observable.FromEventPattern(sender, eventName).Subscribe((e) =>
                {
                    subj.OnNext(new ObservedChange<object, object>(sender, expression));
                });
            });
        }

        /// <summary>
        /// Registers an observable factory for the specified type and property.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="property">Property.</param>
        /// <param name="affinity">Affinity.</param>
        /// <param name="createObservable">Create observable.</param>
        protected void Register(Type type, string property, int affinity, Func<NSObject, Expression, IObservable<IObservedChange<object, object>>> createObservable)
        {
            Dictionary<string, ObservablePropertyInfo> typeProperties;
            if (!_config.TryGetValue(type, out typeProperties))
            {
                typeProperties = new Dictionary<string, ObservablePropertyInfo>();
                _config[type] = typeProperties;
            }

            typeProperties[property] = new ObservablePropertyInfo { Affinity = affinity, CreateObservable = createObservable };
        }

        internal class ObservablePropertyInfo
        {
            public int Affinity { get; set; }

            public Func<NSObject, Expression, IObservable<IObservedChange<object, object>>> CreateObservable { get; set; }
        }
    }
}
