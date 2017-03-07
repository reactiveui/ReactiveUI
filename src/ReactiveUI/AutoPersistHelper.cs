using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Splat;

namespace ReactiveUI
{
    public static class AutoPersistHelper
    {
        static MemoizingMRUCache<Type, Dictionary<string, bool>> persistablePropertiesCache = new MemoizingMRUCache<Type, Dictionary<string, bool>>((type, _) => {
            return type.GetTypeInfo().DeclaredProperties
                .Where(x => x.CustomAttributes.Any(y => typeof(DataMemberAttribute).GetTypeInfo().IsAssignableFrom(y.AttributeType.GetTypeInfo())))
                .ToDictionary(k => k.Name, v => true);
        }, RxApp.SmallCacheLimit);

        static MemoizingMRUCache<Type, bool> dataContractCheckCache = new MemoizingMRUCache<Type, bool>((t, _) => {
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

            var persistableProperties = default(Dictionary<string, bool>);
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
                if (ret.IsDisposed) return;
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
        public static IDisposable AutoPersistCollection<T>(this IReactiveCollection<T> This, Func<T, IObservable<Unit>> doPersist, TimeSpan? interval = null)
            where T : IReactiveObject
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
        public static IDisposable AutoPersistCollection<T, TDontCare>(this IReactiveCollection<T> This, Func<T, IObservable<Unit>> doPersist, IObservable<TDontCare> manualSaveSignal, TimeSpan? interval = null)
            where T : IReactiveObject
        {
            var disposerList = new Dictionary<T, IDisposable>();

            var disp = This.ActOnEveryObject(
                x => {
                    if (disposerList.ContainsKey(x)) return;
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
        public static IDisposable ActOnEveryObject<T>(this IReactiveCollection<T> This, Action<T> onAdd, Action<T> onRemove)
            where T : IReactiveObject
        {
            foreach (var v in This) { onAdd(v); }

            var changingDisp = This.Changing
                .Where(x => x.Action == NotifyCollectionChangedAction.Reset)
                .Subscribe(
                    _ => This.ForEach(x => onRemove(x)));

            var changedDisp = This.Changed.Subscribe(x => {
                switch (x.Action) {
                case NotifyCollectionChangedAction.Add:
                    foreach (T v in x.NewItems) { onAdd(v); }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (T v in x.OldItems) { onRemove(v); }
                    foreach (T v in x.NewItems) { onAdd(v); }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (T v in x.OldItems) { onRemove(v); }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (T v in This) { onAdd(v); }
                    break;
                default:
                    break;
                }
            });

            return Disposable.Create(() => {
                changingDisp.Dispose();
                changedDisp.Dispose();

                This.ForEach(x => onRemove(x));
            });
        }
    }
}
