using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Splat;

namespace ReactiveUI
{
    public class DependencyObjectObservableForProperty : ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            if (!typeof(DependencyObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo())) return 0;
            return getDependencyProperty(type, propertyName) != null ? 4 : 0;
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, System.Linq.Expressions.Expression expression, bool beforeChanged = false)
        {
            var type = sender.GetType();
            var propertyName = expression.GetMemberInfo().Name;
            var dpd = DependencyPropertyDescriptor.FromProperty(getDependencyProperty(type, propertyName), type);

            if (dpd == null) {
                this.Log().Error("Couldn't find dependency property " + propertyName + " on " + type.Name);
                throw new NullReferenceException("Couldn't find dependency property " + propertyName + " on " + type.Name);
            }

            return Observable.Create<IObservedChange<object, object>>(subj => {
                var handler = new EventHandler((o, e) => {
                    subj.OnNext(new ObservedChange<object, object>(sender, expression));
                });

                dpd.AddValueChanged(sender, handler);
                return Disposable.Create(() => dpd.RemoveValueChanged(sender, handler));
            });
        }

        DependencyProperty getDependencyProperty(Type type, string propertyName)
        {
            var fi = type.GetTypeInfo().GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(x => x.Name == propertyName + "Property" && x.IsStatic);

            if (fi != null) {
                return (DependencyProperty)fi.GetValue(null);
            }

            return null;
        }
    }
}
