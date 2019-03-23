﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.Serialization;

using DynamicData;
using DynamicData.Binding;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Helper extension method class associated with the AutoPersist related functionality.
    /// </summary>
    public static class AutoPersistHelper
    {
        private static readonly MemoizingMRUCache<Type, Dictionary<string, bool>> persistablePropertiesCache = new MemoizingMRUCache<Type, Dictionary<string, bool>>(
            (type, _) =>
        {
            return type.GetTypeInfo().DeclaredProperties
                .Where(x => x.CustomAttributes.Any(y => typeof(DataMemberAttribute).GetTypeInfo().IsAssignableFrom(y.AttributeType.GetTypeInfo())))
                .ToDictionary(k => k.Name, v => true);
        }, RxApp.SmallCacheLimit);

        private static readonly MemoizingMRUCache<Type, bool> dataContractCheckCache = new MemoizingMRUCache<Type, bool>(
            (t, _) =>
        {
            return t.GetTypeInfo().GetCustomAttributes(typeof(DataContractAttribute), true).Any();
        }, RxApp.SmallCacheLimit);

        /// <summary>
        /// AutoPersist allows you to automatically call a method when an object
        /// has changed, throttling on a certain interval. Note that this object
        /// must mark its persistable properties via the [DataMember] attribute.
        /// Changes to properties not marked with DataMember will not trigger the
        /// object to be saved.
        /// </summary>
        /// <typeparam name="T">The reactive object type.</typeparam>
        /// <param name="this">
        /// The reactive object to watch for changes.
        /// </param>
        /// <param name="doPersist">
        /// The asynchronous method to call to save the object to disk.
        /// </param>
        /// <param name="interval">
        /// The interval to save the object on. Note that if an object is constantly changing,
        /// it is possible that it will never be saved.
        /// </param>
        /// <returns>A Disposable to disable automatic persistence.</returns>
        public static IDisposable AutoPersist<T>(this T @this, Func<T, IObservable<Unit>> doPersist, TimeSpan? interval = null)
            where T : IReactiveObject
        {
            return @this.AutoPersist(doPersist, Observable<Unit>.Never, interval);
        }

        /// <summary>
        /// AutoPersist allows you to automatically call a method when an object
        /// has changed, throttling on a certain interval. Note that this object
        /// must mark its persistable properties via the [DataMember] attribute.
        /// Changes to properties not marked with DataMember will not trigger the
        /// object to be saved.
        /// </summary>
        /// <typeparam name="T">The reactive object type.</typeparam>
        /// <typeparam name="TDontCare">The save signal type.</typeparam>
        /// <param name="this">
        /// The reactive object to watch for changes.
        /// </param>
        /// <param name="doPersist">
        /// The asynchronous method to call to save the object to disk.
        /// </param>
        /// <param name="manualSaveSignal">
        /// When invoked, the object will be saved regardless of whether it has changed.
        /// </param>
        /// <param name="interval">
        /// The interval to save the object on. Note that if an object is constantly changing,
        /// it is possible that it will never be saved.
        /// </param>
        /// <returns>A Disposable to disable automatic persistence.</returns>
        public static IDisposable AutoPersist<T, TDontCare>(this T @this, Func<T, IObservable<Unit>> doPersist, IObservable<TDontCare> manualSaveSignal, TimeSpan? interval = null)
            where T : IReactiveObject
        {
            interval = interval ?? TimeSpan.FromSeconds(3.0);

            if (!dataContractCheckCache.Get(@this.GetType()))
            {
                throw new ArgumentException("AutoPersist can only be applied to objects with [DataContract]");
            }

            Dictionary<string, bool> persistableProperties = persistablePropertiesCache.Get(@this.GetType());

            var saveHint = Observable.Merge(
                @this.GetChangedObservable().Where(x => persistableProperties.ContainsKey(x.PropertyName)).Select(_ => Unit.Default),
                manualSaveSignal.Select(_ => Unit.Default));

            var autoSaver = saveHint
                .Throttle(interval.Value, RxApp.TaskpoolScheduler)
                .SelectMany(_ => doPersist(@this))
                .Publish();

            // NB: This rigamarole is to prevent the initialization of a class
            // from triggering a save
            var ret = new SingleAssignmentDisposable();
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                if (ret.IsDisposed)
                {
                    return;
                }

                ret.Disposable = autoSaver.Connect();
            });

            return ret;
        }

        /// <summary>
        /// Apply AutoPersistence to all objects in a collection. Items that are
        /// no longer in the collection won't be persisted anymore.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="this">
        /// The reactive collection to watch for changes.
        /// </param>
        /// <param name="doPersist">
        /// The asynchronous method to call to save the object to disk.
        /// </param>
        /// <param name="interval">
        /// The interval to save the object on. Note that if an object is constantly changing,
        /// it is possible that it will never be saved.
        /// </param>
        /// <returns>A Disposable to disable automatic persistence.</returns>
        public static IDisposable AutoPersistCollection<TItem>(this ObservableCollection<TItem> @this, Func<TItem, IObservable<Unit>> doPersist, TimeSpan? interval = null)
            where TItem : IReactiveObject
        {
            return AutoPersistCollection(@this, doPersist, Observable<Unit>.Never, interval);
        }

        /// <summary>
        /// Apply AutoPersistence to all objects in a collection. Items that are
        /// no longer in the collection won't be persisted anymore.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <typeparam name="TDontCare">The return signal type.</typeparam>
        /// <param name="this">
        /// The reactive collection to watch for changes.
        /// </param>
        /// <param name="doPersist">
        /// The asynchronous method to call to save the object to disk.
        /// </param>
        /// <param name="manualSaveSignal">
        /// When invoked, the object will be saved regardless of whether it has changed.
        /// </param>
        /// <param name="interval">
        /// The interval to save the object on. Note that if an object is constantly changing,
        /// it is possible that it will never be saved.
        /// </param>
        /// <returns>A Disposable to disable automatic persistence.</returns>
        public static IDisposable AutoPersistCollection<TItem, TDontCare>(this ObservableCollection<TItem> @this, Func<TItem, IObservable<Unit>> doPersist, IObservable<TDontCare> manualSaveSignal, TimeSpan? interval = null)
            where TItem : IReactiveObject
        {
            return AutoPersistCollection<TItem, ObservableCollection<TItem>, TDontCare>(@this, doPersist, manualSaveSignal, interval);
        }

        /// <summary>
        /// Apply AutoPersistence to all objects in a collection. Items that are
        /// no longer in the collection won't be persisted anymore.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <typeparam name="TDontCare">The signal type.</typeparam>
        /// <param name="this">
        /// The reactive collection to watch for changes.
        /// </param>
        /// <param name="doPersist">
        /// The asynchronous method to call to save the object to disk.
        /// </param>
        /// <param name="manualSaveSignal">
        /// When invoked, the object will be saved regardless of whether it has changed.
        /// </param>
        /// <param name="interval">
        /// The interval to save the object on. Note that if an object is constantly changing,
        /// it is possible that it will never be saved.
        /// </param>
        /// <returns>A Disposable to disable automatic persistence.</returns>
        public static IDisposable AutoPersistCollection<TItem, TDontCare>(this ReadOnlyObservableCollection<TItem> @this, Func<TItem, IObservable<Unit>> doPersist, IObservable<TDontCare> manualSaveSignal, TimeSpan? interval = null)
            where TItem : IReactiveObject
        {
            return AutoPersistCollection<TItem, ReadOnlyObservableCollection<TItem>, TDontCare>(@this, doPersist, manualSaveSignal, interval);
        }

        /// <summary>
        /// Apply AutoPersistence to all objects in a collection. Items that are
        /// no longer in the collection won't be persisted anymore.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <typeparam name="TCollection">The collection type.</typeparam>
        /// <typeparam name="TDontCare">The signal type.</typeparam>
        /// <param name="this">
        /// The reactive collection to watch for changes.
        /// </param>
        /// <param name="doPersist">
        /// The asynchronous method to call to save the object to disk.
        /// </param>
        /// <param name="manualSaveSignal">
        /// When invoked, the object will be saved regardless of whether it has changed.
        /// </param>
        /// <param name="interval">
        /// The interval to save the object on. Note that if an object is constantly changing,
        /// it is possible that it will never be saved.
        /// </param>
        /// <returns>A Disposable to disable automatic persistence.</returns>
        public static IDisposable AutoPersistCollection<TItem, TCollection, TDontCare>(this TCollection @this, Func<TItem, IObservable<Unit>> doPersist, IObservable<TDontCare> manualSaveSignal, TimeSpan? interval = null)
            where TItem : IReactiveObject
            where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
        {
            var disposerList = new Dictionary<TItem, IDisposable>();

            var disp = @this.ActOnEveryObject<TItem, TCollection>(
                x =>
                {
                    if (disposerList.ContainsKey(x))
                    {
                        return;
                    }

                    disposerList[x] = x.AutoPersist(doPersist, manualSaveSignal, interval);
                },
                x =>
                {
                    disposerList[x].Dispose();
                    disposerList.Remove(x);
                });

            return Disposable.Create(() =>
            {
                disp.Dispose();
                disposerList.Values.ForEach(x => x.Dispose());
            });
        }

        /// <summary>
        /// Call methods 'onAdd' and 'onRemove' whenever an object is added or
        /// removed from a collection. This class correctly handles both when
        /// a collection is initialized, as well as when the collection is Reset.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="this">
        /// The reactive collection to watch for changes.
        /// </param>
        /// <param name="onAdd">
        /// A method to be called when an object is added to the collection.
        /// </param>
        /// <param name="onRemove">
        /// A method to be called when an object is removed from the collection.
        /// </param>
        /// <returns>A Disposable that deactivates this behavior.</returns>
        public static IDisposable ActOnEveryObject<TItem>(this ObservableCollection<TItem> @this, Action<TItem> onAdd, Action<TItem> onRemove)
            where TItem : IReactiveObject
        {
            return ActOnEveryObject<TItem, ObservableCollection<TItem>>(@this, onAdd, onRemove);
        }

        /// <summary>
        /// Call methods 'onAdd' and 'onRemove' whenever an object is added or
        /// removed from a collection. This class correctly handles both when
        /// a collection is initialized, as well as when the collection is Reset.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="this">
        /// The reactive collection to watch for changes.
        /// </param>
        /// <param name="onAdd">
        /// A method to be called when an object is added to the collection.
        /// </param>
        /// <param name="onRemove">
        /// A method to be called when an object is removed from the collection.
        /// </param>
        /// <returns>A Disposable that deactivates this behavior.</returns>
        public static IDisposable ActOnEveryObject<TItem>(this ReadOnlyObservableCollection<TItem> @this, Action<TItem> onAdd, Action<TItem> onRemove)
            where TItem : IReactiveObject
        {
            return ActOnEveryObject<TItem, ReadOnlyObservableCollection<TItem>>(@this, onAdd, onRemove);
        }

        /// <summary>
        /// Call methods 'onAdd' and 'onRemove' whenever an object is added or
        /// removed from a collection. This class correctly handles both when
        /// a collection is initialized, as well as when the collection is Reset.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <typeparam name="TCollection">The collection type.</typeparam>
        /// <param name="this">
        /// The reactive collection to watch for changes.
        /// </param>
        /// <param name="onAdd">
        /// A method to be called when an object is added to the collection.
        /// </param>
        /// <param name="onRemove">
        /// A method to be called when an object is removed from the collection.
        /// </param>
        /// <returns>A Disposable that deactivates this behavior.</returns>
        public static IDisposable ActOnEveryObject<TItem, TCollection>(this TCollection @this, Action<TItem> onAdd, Action<TItem> onRemove)
            where TItem : IReactiveObject
            where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
        {
            foreach (var v in @this)
            {
                onAdd(v);
            }

            var changedDisposable = ActOnEveryObject(@this.ToObservableChangeSet<TCollection, TItem>(), onAdd, onRemove);

            return Disposable.Create(() =>
            {
                changedDisposable.Dispose();

                @this.ForEach(onRemove);
            });
        }

        /// <summary>
        /// Call methods 'onAdd' and 'onRemove' whenever an object is added or
        /// removed from a collection. This class correctly handles both when
        /// a collection is initialized, as well as when the collection is Reset.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="this">
        /// The reactive collection to watch for changes.
        /// </param>
        /// <param name="onAdd">
        /// A method to be called when an object is added to the collection.
        /// </param>
        /// <param name="onRemove">
        /// A method to be called when an object is removed from the collection.
        /// </param>
        /// <returns>A Disposable that deactivates this behavior.</returns>
        public static IDisposable ActOnEveryObject<TItem>(this IObservable<IChangeSet<TItem>> @this, Action<TItem> onAdd, Action<TItem> onRemove)
            where TItem : IReactiveObject
        {
            return @this.Subscribe(changeSet =>
            {
                foreach (var change in changeSet)
                {
                    switch (change.Reason)
                    {
                        case ListChangeReason.Refresh:
                            foreach (var item in change.Range)
                            {
                                onRemove(item);
                            }

                            foreach (var item in change.Range)
                            {
                                onAdd(item);
                            }

                            break;
                        case ListChangeReason.Clear:
                            foreach (var item in change.Range)
                            {
                                onRemove(item);
                            }

                            break;
                        case ListChangeReason.Add:
                            onAdd(change.Item.Current);
                            break;
                        case ListChangeReason.AddRange:
                            foreach (var item in change.Range)
                            {
                                onAdd(item);
                            }

                            break;
                        case ListChangeReason.Replace:
                            onRemove(change.Item.Previous.Value);
                            onAdd(change.Item.Current);
                            break;
                        case ListChangeReason.Remove:
                            onRemove(change.Item.Current);
                            break;
                        case ListChangeReason.RemoveRange:
                            foreach (var item in change.Range)
                            {
                                onRemove(item);
                            }

                            break;
                    }
                }
            });
        }
    }
}
