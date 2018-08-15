// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    public static class AutoPersistHelper
    {
        private static MemoizingMRUCache<Type, Dictionary<string, bool>> persistablePropertiesCache = new MemoizingMRUCache<Type, Dictionary<string, bool>>((type, _) => {
            return type.GetTypeInfo().DeclaredProperties
                .Where(x => x.CustomAttributes.Any(y => typeof(DataMemberAttribute).GetTypeInfo().IsAssignableFrom(y.AttributeType.GetTypeInfo())))
                .ToDictionary(k => k.Name, v => true);
        }, RxApp.SmallCacheLimit);
        private static MemoizingMRUCache<Type, bool> dataContractCheckCache = new MemoizingMRUCache<Type, bool>((t, _) => {
            return t.GetTypeInfo().GetCustomAttributes(typeof(DataContractAttribute), true).Any();
        }, RxApp.SmallCacheLimit);

        /// <summary>
        /// AutoPersist allows you to automatically call a method when an object
        /// has changed, throttling on a certain interval. Note that this object
        /// must mark its persistable properties via the [DataMember] attribute.
        /// Changes to properties not marked with DataMember will not trigger the
        /// object to be saved.
        /// </summary>
        /// <param name="This">
        /// The reactive object to watch for changes
        /// </param>
        /// <param name="doPersist">
        /// The asynchronous method to call to save the object to disk.
        /// </param>
        /// <param name="interval">
        /// The interval to save the object on. Note that if an object is constantly changing, 
        /// it is possible that it will never be saved.
        /// </param>
        /// <returns>A Disposable to disable automatic persistence.</returns>
        public static IDisposable AutoPersist<T>(this T This, Func<T, IObservable<Unit>> doPersist, TimeSpan? interval = null)
            where T : IReactiveObject
        {
            return This.AutoPersist(doPersist, Observable<Unit>.Never, interval);
        }

        /// <summary>
        /// AutoPersist allows you to automatically call a method when an object
        /// has changed, throttling on a certain interval. Note that this object
        /// must mark its persistable properties via the [DataMember] attribute.
        /// Changes to properties not marked with DataMember will not trigger the
        /// object to be saved.
        /// </summary>
        /// <param name="This">
        /// The reactive object to watch for changes
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
        public static IDisposable AutoPersist<T, TDontCare>(this T This, Func<T, IObservable<Unit>> doPersist, IObservable<TDontCare> manualSaveSignal, TimeSpan? interval = null)
            where T : IReactiveObject
        {
            interval = interval ?? TimeSpan.FromSeconds(3.0);

            lock (dataContractCheckCache) {
                if (!dataContractCheckCache.Get(This.GetType())) {
                    throw new ArgumentException("AutoPersist can only be applied to objects with [DataContract]");
                }
            }

            Dictionary<string, bool> persistableProperties;
            lock (persistablePropertiesCache) {
                persistableProperties = persistablePropertiesCache.Get(This.GetType());
            }

            var saveHint = Observable.Merge(
                This.getChangedObservable().Where(x => persistableProperties.ContainsKey(x.PropertyName)).Select(_ => Unit.Default),
                manualSaveSignal.Select(_ => Unit.Default));

            var autoSaver = saveHint
                .Throttle(interval.Value, RxApp.TaskpoolScheduler)
                .SelectMany(_ => doPersist(This))
                .Publish();

            // NB: This rigamarole is to prevent the initialization of a class
            // from triggering a save
            var ret = new SingleAssignmentDisposable();
            RxApp.MainThreadScheduler.Schedule(() => {
                if (ret.IsDisposed) {
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
        /// <param name="This">
        /// The reactive collection to watch for changes
        /// </param>
        /// <param name="doPersist">
        /// The asynchronous method to call to save the object to disk.
        /// </param>
        /// <param name="interval">
        /// The interval to save the object on. Note that if an object is constantly changing, 
        /// it is possible that it will never be saved.
        /// </param>
        /// <returns>A Disposable to disable automatic persistence.</returns>
        public static IDisposable AutoPersistCollection<TItem>(this ObservableCollection<TItem> This, Func<TItem, IObservable<Unit>> doPersist, TimeSpan? interval = null)
            where TItem : IReactiveObject
        {
            return AutoPersistCollection(This, doPersist, Observable<Unit>.Never, interval);
        }

        /// <summary>
        /// Apply AutoPersistence to all objects in a collection. Items that are
        /// no longer in the collection won't be persisted anymore.
        /// </summary>
        /// <param name="This">
        /// The reactive collection to watch for changes
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
        public static IDisposable AutoPersistCollection<TItem, TDontCare>(this ObservableCollection<TItem> This, Func<TItem, IObservable<Unit>> doPersist, IObservable<TDontCare> manualSaveSignal, TimeSpan? interval = null)
            where TItem : IReactiveObject
        {
            return AutoPersistCollection(new ReadOnlyObservableCollection<TItem>(This), doPersist, manualSaveSignal, interval);
        }

        /// <summary>
        /// Apply AutoPersistence to all objects in a collection. Items that are
        /// no longer in the collection won't be persisted anymore.
        /// </summary>
        /// <param name="This">
        /// The reactive collection to watch for changes
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
        public static IDisposable AutoPersistCollection<TItem, TDontCare>(this ReadOnlyObservableCollection<TItem> This, Func<TItem, IObservable<Unit>> doPersist, IObservable<TDontCare> manualSaveSignal, TimeSpan? interval = null)
            where TItem : IReactiveObject
        {
            return AutoPersistCollection<TItem, ReadOnlyObservableCollection<TItem>, TDontCare>(This, doPersist, manualSaveSignal, interval);
        }

        /// <summary>
        /// Apply AutoPersistence to all objects in a collection. Items that are
        /// no longer in the collection won't be persisted anymore.
        /// </summary>
        /// <param name="This">
        /// The reactive collection to watch for changes
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
        public static IDisposable AutoPersistCollection<TItem, TCollection, TDontCare>(this TCollection This, Func<TItem, IObservable<Unit>> doPersist, IObservable<TDontCare> manualSaveSignal, TimeSpan? interval = null)
            where TItem : IReactiveObject
            where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
        {
            var disposerList = new Dictionary<TItem, IDisposable>();

            var disp = This.ActOnEveryObject<TItem, TCollection>(
                x => {
                    if (disposerList.ContainsKey(x)) {
                        return;
                    }

                    disposerList[x] = x.AutoPersist(doPersist, manualSaveSignal, interval);
                },
                x => {
                    disposerList[x].Dispose();
                    disposerList.Remove(x);
                });

            return Disposable.Create(() => {
                disp.Dispose();
                disposerList.Values.ForEach(x => x.Dispose());
            });
        }

        /// <summary>
        /// Call methods 'onAdd' and 'onRemove' whenever an object is added or
        /// removed from a collection. This class correctly handles both when
        /// a collection is initialized, as well as when the collection is Reset.
        /// </summary>
        /// <param name="This">
        /// The reactive collection to watch for changes
        /// </param>
        /// <param name="onAdd">
        /// A method to be called when an object is added to the collection.
        /// </param>
        /// <param name="onRemove">
        /// A method to be called when an object is removed from the collection.
        /// </param>
        /// <returns>A Disposable that deactivates this behavior.</returns>
        public static IDisposable ActOnEveryObject<TItem>(this ObservableCollection<TItem> This, Action<TItem> onAdd, Action<TItem> onRemove)
            where TItem : IReactiveObject
        {
            return ActOnEveryObject(new ReadOnlyObservableCollection<TItem>(This), onAdd, onRemove);
        }

        /// <summary>
        /// Call methods 'onAdd' and 'onRemove' whenever an object is added or
        /// removed from a collection. This class correctly handles both when
        /// a collection is initialized, as well as when the collection is Reset.
        /// </summary>
        /// <param name="This">
        /// The reactive collection to watch for changes
        /// </param>
        /// <param name="onAdd">
        /// A method to be called when an object is added to the collection.
        /// </param>
        /// <param name="onRemove">
        /// A method to be called when an object is removed from the collection.
        /// </param>
        /// <returns>A Disposable that deactivates this behavior.</returns>
        public static IDisposable ActOnEveryObject<TItem>(this ReadOnlyObservableCollection<TItem> This, Action<TItem> onAdd, Action<TItem> onRemove)
            where TItem : IReactiveObject
        {
            return ActOnEveryObject<TItem, ReadOnlyObservableCollection<TItem>>(This, onAdd, onRemove);
        }

        /// <summary>
        /// Call methods 'onAdd' and 'onRemove' whenever an object is added or
        /// removed from a collection. This class correctly handles both when
        /// a collection is initialized, as well as when the collection is Reset.
        /// </summary>
        /// <param name="This">
        /// The reactive collection to watch for changes
        /// </param>
        /// <param name="onAdd">
        /// A method to be called when an object is added to the collection.
        /// </param>
        /// <param name="onRemove">
        /// A method to be called when an object is removed from the collection.
        /// </param>
        /// <returns>A Disposable that deactivates this behavior.</returns>
        public static IDisposable ActOnEveryObject<TItem, TCollection>(this TCollection This, Action<TItem> onAdd, Action<TItem> onRemove)
            where TItem : IReactiveObject
            where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
        {
            foreach (var v in This) { onAdd(v); }

            var changedDisposable = ActOnEveryObject(This.ToObservableChangeSet<TCollection, TItem>(), onAdd, onRemove);

            return Disposable.Create(() => {
                changedDisposable.Dispose();

                This.ForEach(onRemove);
            });
        }

        /// <summary>
        /// Call methods 'onAdd' and 'onRemove' whenever an object is added or
        /// removed from a collection. This class correctly handles both when
        /// a collection is initialized, as well as when the collection is Reset.
        /// </summary>
        /// <param name="This">
        /// The reactive collection to watch for changes
        /// </param>
        /// <param name="onAdd">
        /// A method to be called when an object is added to the collection.
        /// </param>
        /// <param name="onRemove">
        /// A method to be called when an object is removed from the collection.
        /// </param>
        /// <returns>A Disposable that deactivates this behavior.</returns>
        public static IDisposable ActOnEveryObject<TItem>(this IObservable<IChangeSet<TItem>> This, Action<TItem> onAdd, Action<TItem> onRemove)
            where TItem : IReactiveObject
        {
            return This.Subscribe(changeSet => {
                foreach (var change in changeSet) {
                    switch (change.Reason) {
                        case ListChangeReason.Refresh:
                            foreach (var item in change.Range) { onRemove(item); }
                            foreach (var item in change.Range) { onAdd(item); }
                            break;
                        case ListChangeReason.Clear:
                            foreach (var item in change.Range) { onRemove(item); }
                            break;
                        case ListChangeReason.Add:
                            onAdd(change.Item.Current);
                            break;
                        case ListChangeReason.AddRange:
                            foreach (var item in change.Range) { onAdd(item); }
                            break;
                        case ListChangeReason.Replace:
                            onRemove(change.Item.Previous.Value);
                            onAdd(change.Item.Current);
                            break;
                        case ListChangeReason.Remove:
                            onRemove(change.Item.Current);
                            break;
                        case ListChangeReason.RemoveRange:
                            foreach (var item in change.Range) { onRemove(item); }
                            break;
                    }
                }
            });
        }
    }
}
