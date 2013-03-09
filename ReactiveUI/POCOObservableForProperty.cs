using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace ReactiveUI
{
    public class POCOObservableForProperty : ICreatesObservableForProperty 
    {
        public int GetAffinityForObject(Type type, bool beforeChanged = false)
        {
            return 1;
        }

        static readonly Dictionary<Type, bool> hasWarned = new Dictionary<Type, bool>();
        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, string propertyName, bool beforeChanged = false)
        {
            var type = sender.GetType();
            if (!hasWarned.ContainsKey(type)) {
                this.Log().Warn(
                    "{0} is a POCO type and won't send change notifications, WhenAny will only return a single value!",
                    type.FullName);
                hasWarned[type] = true;
            }

            return Observable.Return((IObservedChange<object, object>) new ObservedChange<object, object>() {
                Sender = sender, PropertyName = propertyName
            }, RxApp.DeferredScheduler)
                .Concat(Observable.Never<IObservedChange<object, object>>());
        }
    }
}
