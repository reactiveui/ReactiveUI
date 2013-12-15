using System;
using System.Reactive.Linq;
using System.Reflection;

namespace ReactiveUI
{
    /// <summary>
    /// Generates Observables based on observing Reactive objects
    /// </summary>
    public class IRNPCObservableForProperty : ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            // NB: Since every IRNPC is also an INPC, we need to bind more 
            // tightly than INPCObservableForProperty, so we return 10 here 
            // instead of one
            return typeof (IReactiveNotifyPropertyChanged<object>).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) ? 10 : 0;
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, string propertyName, bool beforeChanged = false)
        {
            var irnpc = sender as IReactiveNotifyPropertyChanged<object>;
            if (irnpc == null) {
                throw new ArgumentException("Sender doesn't implement IReactiveNotifyPropertyChanging");
            }

            return Observable.Create<IObservedChange<object, object>>(subj => {
                var obs = (beforeChanged ? irnpc.Changing : irnpc.Changed);

                return obs
                    .Where(x => x.PropertyName == propertyName)
                    .Subscribe(subj);
            });
        }
    }
}