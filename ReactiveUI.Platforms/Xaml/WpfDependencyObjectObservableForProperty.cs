using System;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace ReactiveUI.Xaml
{
    public class DependencyObjectObservableForProperty : ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            if (!typeof(DependencyObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo())) return 0;
            return getDependencyProperty(type, propertyName) != null ? 4 : 0;
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, string propertyName, bool beforeChanged = false)
        {
            var type = sender.GetType();
            var dpd = DependencyPropertyDescriptor.FromProperty(getDependencyProperty(type, propertyName), sender.GetType());

            return Observable.Create<IObservedChange<object, object>>(subj =>
            {
                var handler = new EventHandler((o, e) =>
                {
                    subj.OnNext(new ObservedChange<object, object>()
                    {
                        Sender = sender,
                        PropertyName = propertyName,
                        Value = null,
                    });
                });

                dpd.AddValueChanged(sender, handler);
                return Disposable.Create(() => dpd.RemoveValueChanged(sender, handler));
            });
        }

        DependencyProperty getDependencyProperty(Type type, string propertyName)
        {
            var fi = type.GetRuntimeFields().FirstOrDefault(x => x.Name == propertyName + "Property" && x.IsStatic);
            if (fi != null)
            {
                return (DependencyProperty)fi.GetValue(null);
            }

            return null;
        }
    }
}