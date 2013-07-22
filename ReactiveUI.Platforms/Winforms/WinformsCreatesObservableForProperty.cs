using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Winforms
{
    public class WinformsCreatesObservableForProperty : ICreatesObservableForProperty
    {
        static readonly MemoizingMRUCache<Tuple<Type, string>, EventInfo> eventInfoCache = new MemoizingMRUCache<Tuple<Type, string>, EventInfo>((pair, _) => {
            return pair.Item1.GetEvents(System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                .FirstOrDefault(x => x.Name == pair.Item2 + "Changed");
        }, RxApp.SmallCacheLimit);

        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            if (!type.FullName.ToLowerInvariant().StartsWith("system.windows.forms")) return 0;

            lock (eventInfoCache) {
                var ei = eventInfoCache.Get(Tuple.Create(type, propertyName));
                return (beforeChanged == false && ei != null) ?  8 : 0;
            }
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, string propertyName, bool beforeChanged = false)
        {
            var type = sender.GetType();
            var ei = default(EventInfo);
            var getter = Reflection.GetValueFetcherOrThrow(type, propertyName);

            lock (eventInfoCache) {
                ei = eventInfoCache.Get(Tuple.Create(type, propertyName));
            }

            return Observable.Create<IObservedChange<object, object>>(subj => {
                bool completed = false;
                var handler = new EventHandler((o, e) => {
                    if (completed) return;
                    try {
                        subj.OnNext(new ObservedChange<object, object>() { Sender = sender, PropertyName = propertyName, Value = getter(sender) });
                    } catch (Exception ex) {
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