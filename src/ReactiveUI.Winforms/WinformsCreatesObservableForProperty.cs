// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Forms;

using Splat;

namespace ReactiveUI.Winforms
{
    /// <summary>
    /// WinForm view objects are not Generally Observable™, so hard-code some
    /// particularly useful types.
    /// </summary>
    /// <seealso cref="ReactiveUI.ICreatesObservableForProperty" />
    public class WinformsCreatesObservableForProperty : ICreatesObservableForProperty
    {
        private static readonly MemoizingMRUCache<(Type type, string name), EventInfo?> EventInfoCache = new(
            (pair, _) => pair.type.GetEvent(pair.name + "Changed", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public),
            RxApp.SmallCacheLimit);

        /// <inheritdoc/>
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            var supportsTypeBinding = typeof(Component).IsAssignableFrom(type);
            if (!supportsTypeBinding)
            {
                return 0;
            }

            var ei = EventInfoCache.Get((type, propertyName));
            return !beforeChanged && ei != null ? 8 : 0;
        }

        /// <inheritdoc/>
        public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            var ei = EventInfoCache.Get((sender.GetType(), propertyName));

            if (ei is null)
            {
                throw new InvalidOperationException("Could not find a valid event for expression.");
            }

            return Observable.Create<IObservedChange<object, object?>>(subj =>
            {
                var completed = false;
                var handler = new EventHandler((o, e) =>
                {
                    if (completed)
                    {
                        return;
                    }

                    try
                    {
                        subj.OnNext(new ObservedChange<object, object?>(sender, expression, default));
                    }
                    catch (Exception ex)
                    {
                        subj.OnError(ex);
                        completed = true;
                    }
                });

                ei.AddEventHandler(sender, handler);
                return Disposable.Create(() => ei.RemoveEventHandler(sender, handler));
            });
        }
    }
}
