using System;
using System.Reactive.Linq;

namespace ReactiveUI
{
    public class IRNPCObservableForProperty : ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, bool beforeChanged = false)
        {
            // NB: Since every IRNPC is also an INPC, we need to bind more 
            // tightly than INPCObservableForProperty, so we return 10 here 
            // instead of one
            return typeof (IReactiveNotifyPropertyChanged).IsAssignableFrom(type) ? 10 : 0;
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, string propertyName, bool beforeChanged = false)
        {
            var irnpc = sender as IReactiveNotifyPropertyChanged;
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