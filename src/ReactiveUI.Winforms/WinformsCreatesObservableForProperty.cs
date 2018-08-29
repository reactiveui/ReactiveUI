// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        private static readonly MemoizingMRUCache<Tuple<Type, string>, EventInfo> eventInfoCache = new MemoizingMRUCache<Tuple<Type, string>, EventInfo>(
            (pair, _) =>
        {
            return pair.Item1.GetEvents(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(x => x.Name == pair.Item2 + "Changed");
        }, RxApp.SmallCacheLimit);

        /// <inheritdoc/>
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            bool supportsTypeBinding = typeof(Component).IsAssignableFrom(type);
            if (!supportsTypeBinding)
            {
                return 0;
            }

            lock (eventInfoCache)
            {
                var ei = eventInfoCache.Get(Tuple.Create(type, propertyName));
                return beforeChanged == false && ei != null ? 8 : 0;
            }
        }

        /// <inheritdoc/>
        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false)
        {
            var ei = default(EventInfo);

            lock (eventInfoCache)
            {
                ei = eventInfoCache.Get(Tuple.Create(sender.GetType(), propertyName));
            }

            return Observable.Create<IObservedChange<object, object>>(subj =>
            {
                bool completed = false;
                var handler = new EventHandler((o, e) =>
                {
                    if (completed)
                    {
                        return;
                    }

                    try
                    {
                        subj.OnNext(new ObservedChange<object, object>(sender, expression));
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
