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
    public class WinformsCreatesObservableForProperty : ICreatesObservableForProperty
    {
        static readonly MemoizingMRUCache<Tuple<Type, string>, EventInfo> eventInfoCache = new MemoizingMRUCache<Tuple<Type, string>, EventInfo>((pair, _) => {
            return pair.Item1.GetEvents(System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                .FirstOrDefault(x => x.Name == pair.Item2 + "Changed");
        }, RxApp.SmallCacheLimit);

        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            bool supportsTypeBinding = typeof(Component).IsAssignableFrom(type);
            if (!supportsTypeBinding) return 0;

            lock (eventInfoCache) {
                var ei = eventInfoCache.Get(Tuple.Create(type, propertyName));
                return (beforeChanged == false && ei != null) ?  8 : 0;
            }
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, bool beforeChanged = false)
        {
            var ei = default(EventInfo);

            lock (eventInfoCache) {
                ei = eventInfoCache.Get(Tuple.Create(sender.GetType(), expression.GetMemberInfo().Name));
            }

            return Observable.Create<IObservedChange<object, object>>(subj => {
                bool completed = false;
                var handler = new EventHandler((o, e) => {
                    if (completed) return;
                    try {
                        subj.OnNext(new ObservedChange<object, object>(sender, expression));
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
