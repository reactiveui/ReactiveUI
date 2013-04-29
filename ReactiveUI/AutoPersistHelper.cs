using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ReactiveUI
{
    public static class AutoPersistHelper
    {
        static MemoizingMRUCache<Type, Dictionary<string, bool>> persistablePropertiesCache = new MemoizingMRUCache<Type, Dictionary<string, bool>>((type, _) => {
            return type.GetProperties()
                .Where(x => x.CustomAttributes.Any(y => typeof(DataMemberAttribute).IsAssignableFrom(y.AttributeType)))
                .ToDictionary(k => k.Name, v => true);
        }, 32);

        static MemoizingMRUCache<Type, bool> dataContractCheckCache = new MemoizingMRUCache<Type, bool>((t, _) => {
            return t.GetCustomAttributes(typeof(DataContractAttribute), true).Any();
        }, 64);

        public static IDisposable AutoPersist<T>(this T This, Func<T, IObservable<Unit>> doPersist, TimeSpan? interval = null)
            where T : IReactiveNotifyPropertyChanged
        {
            return This.AutoPersist(doPersist, Observable.Never<Unit>());
        }

        public static IDisposable AutoPersist<T, TDontCare>(this T This, Func<T, IObservable<Unit>> doPersist, IObservable<TDontCare> manualSaveSignal, TimeSpan? interval = null)
            where T : IReactiveNotifyPropertyChanged
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
                This.Changed.Where(x => persistableProperties.ContainsKey(x.PropertyName)).Select(_ => Unit.Default),
                manualSaveSignal.Select(_ => Unit.Default));

            var autoSaver = saveHint
                .Throttle(interval.Value, RxApp.TaskpoolScheduler)
                .SelectMany(_ => doPersist(This))
                .Publish();

            // NB: This rigamarole is to prevent the initialization of a class 
            // from triggering a save
            var ret = new SingleAssignmentDisposable();
            RxApp.DeferredScheduler.Schedule(() => {
                if (ret.IsDisposed) return;
                ret.Disposable = autoSaver.Connect();
            });

            return ret;
        }

        public static IDisposable AutoPersistCollection<T>(this ReactiveCollection<T> This, Func<T, IObservable<Unit>> doPersist, TimeSpan? interval = null)
            where T : IReactiveNotifyPropertyChanged
        {
            return AutoPersistCollection(This, doPersist, Observable.Never<Unit>(), interval);
        }

        public static IDisposable AutoPersistCollection<T, TDontCare>(this ReactiveCollection<T> This, Func<T, IObservable<Unit>> doPersist, IObservable<TDontCare> manualSaveSignal, TimeSpan? interval = null)
            where T : IReactiveNotifyPropertyChanged
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

        public static IDisposable ActOnEveryObject<T>(this ReactiveCollection<T> This, Action<T> onAdd, Action<T> onRemove)
            where T : IReactiveNotifyPropertyChanged
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