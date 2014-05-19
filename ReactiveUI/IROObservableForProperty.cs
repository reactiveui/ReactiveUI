using System;
using System.Reactive.Linq;
using System.Reflection;

namespace ReactiveUI
{
    /// <summary>
    /// Generates Observables based on observing Reactive objects
    /// </summary>
    public class IROObservableForProperty : ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            // NB: Since every IReactiveObject is also an INPC, we need to bind more 
            // tightly than INPCObservableForProperty, so we return 10 here 
            // instead of one
            return typeof (IReactiveObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) ? 10 : 0;
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, string propertyName, bool beforeChanged = false)
        {
            var iro = sender as IReactiveObject;
            if (iro == null) {
                throw new ArgumentException("Sender doesn't implement IReactiveObject");
            }

            return Observable.Create<IObservedChange<object, object>>(subj => {
                var obs = (beforeChanged ? iro.getChangingObservable() : iro.getChangedObservable());

                return obs
                    .Where(x => x.PropertyName == propertyName)
                    .Select(x => new ObservedChange<object,object>(sender, propertyName))
                    .Subscribe(subj);
            });
        }
    }
}