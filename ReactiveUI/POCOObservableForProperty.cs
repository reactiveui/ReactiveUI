using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace ReactiveUI
{
    /// <summary>
    /// This class is the final fallback for WhenAny, and will simply immediately
    /// return the value of the type at the time it was created. It will also 
    /// warn the user that this is probably not what they want to do
    /// </summary>
    public class POCOObservableForProperty : ICreatesObservableForProperty 
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
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

            return Observable.Return(new ObservedChange<object, object>(sender, propertyName), RxApp.MainThreadScheduler)
                .Concat(Observable.Never<IObservedChange<object, object>>());
        }
    }
}
