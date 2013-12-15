using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;

namespace ReactiveUI
{
    /// <summary>
    /// Generates Observables based on observing INotifyPropertyChanged objects
    /// </summary>
    public class INPCObservableForProperty : ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged)
        {
            var target = beforeChanged ? typeof (INotifyPropertyChanging) : typeof (INotifyPropertyChanged);
            return target.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) ? 5 : 0;
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, string propertyName, bool beforeChanged)
        {
            var before = sender as INotifyPropertyChanging;
            var after = sender as INotifyPropertyChanged;

            if (beforeChanged ? before == null : after == null) {
                return Observable.Never<IObservedChange<object, object>>();
            }

            return Observable.Create<IObservedChange<object, object>>(subj => {
                if (beforeChanged) {
                    var obs = Observable.FromEventPattern<PropertyChangingEventHandler, PropertyChangingEventArgs>(
                        x => before.PropertyChanging += x, x => before.PropertyChanging -= x);

                    return obs.Where(x => x.EventArgs.PropertyName == propertyName)
                        .Select(x => new ObservedChange<object, object>(sender, x.EventArgs.PropertyName))
                        .Subscribe(subj);
                } else {
                    var obs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        x => after.PropertyChanged += x, x => after.PropertyChanged -= x);

                    return obs.Where(x => x.EventArgs.PropertyName == propertyName)
                        .Select(x => new ObservedChange<object, object>(sender, x.EventArgs.PropertyName))
                        .Subscribe(subj);
                }
            });
        }
    }
}