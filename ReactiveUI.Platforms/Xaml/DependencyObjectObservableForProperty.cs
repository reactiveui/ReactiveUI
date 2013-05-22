using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
#endif

namespace ReactiveUI.Xaml
{
    public class DependencyObjectObservableForProperty : ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, bool beforeChanged = false)
        {
            return typeof(DependencyObject).IsAssignableFrom(type) ? 4 : 0;
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, string propertyName, bool beforeChanged = false)
        {
            Contract.Requires(sender != null && sender is DependencyObject);
            var type = sender.GetType();

            if (beforeChanged == true) {
                this.Log().Warn("Tried to bind DO {0}.{1}, but DPs can't do beforeChanged. Binding as POCO object",
                    type.FullName, propertyName);

                var ret = new POCOObservableForProperty();
                return ret.GetNotificationForProperty(sender, propertyName, beforeChanged);
            }

            var dpFetcher = getDependencyPropertyFetcher(type, propertyName);
            if (dpFetcher == null) {
                this.Log().Warn("Tried to bind DO {0}.{1}, but DP doesn't exist. Binding as POCO object",
                    type.FullName, propertyName);

                var ret = new POCOObservableForProperty();
                return ret.GetNotificationForProperty(sender, propertyName, beforeChanged);
            }

#if !WINRT && !SILVERLIGHT
            return Observable.Create<IObservedChange<object, object>>(subj => {
                var dp = dpFetcher();
                var dpd = DependencyPropertyDescriptor.FromProperty(dp, type);
                var ev = new EventHandler((o, e) => subj.OnNext(new ObservedChange<object, object>() {Sender = sender, PropertyName = propertyName,}));
                dpd.AddValueChanged(sender, ev);

                return Disposable.Create(() => dpd.RemoveValueChanged(sender, ev));
            });
#else
            var dpAndSubj = createAttachedProperty(type, propertyName);

            BindingOperations.SetBinding(sender as DependencyObject, dpAndSubj.Item1,
                new Binding() { Source = sender as DependencyObject, Path = new PropertyPath(propertyName) });

            return dpAndSubj.Item2
                .Where(x => x == sender)
                .Select(x => (IObservedChange<object, object>) new ObservedChange<object, object>() { Sender = x, PropertyName = propertyName });
#endif
        }

        Func<DependencyProperty> getDependencyPropertyFetcher(Type type, string propertyName)
        {
#if WINRT
            // Look for the DependencyProperty attached to this property name
            var pi = type.GetProperty(propertyName + "Property", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (pi != null) {
                return () => (DependencyProperty)pi.GetValue(null);
            }
#endif

            var fi = type.GetField(propertyName + "Property", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (fi != null) {
                return () => (DependencyProperty)fi.GetValue(null);
            }

            return null;
        }

        static readonly Dictionary<Tuple<Type, string>, Tuple<DependencyProperty, Subject<object>>> attachedListener =
            new Dictionary<Tuple<Type, string>, Tuple<DependencyProperty, Subject<object>>>();

        Tuple<DependencyProperty, Subject<object>> createAttachedProperty(Type type, string propertyName)
        {
            var pair = Tuple.Create(type, propertyName);
            if (attachedListener.ContainsKey(pair)) return attachedListener[pair];

            var subj = new Subject<object>();

            // NB: There is no way to unregister an attached property, 
            // we just have to leak it. Luckily it's per-type, so it's
            // not *that* bad.
            var dp = DependencyProperty.RegisterAttached(
                "ListenAttached" + propertyName + this.GetHashCode().ToString("{0:x}"),
                typeof(object), type,
                new PropertyMetadata(null, (o, e) => subj.OnNext(o)));

            var ret = Tuple.Create(dp, subj);
            attachedListener[pair] = ret;
            return ret;
        }
    }
}
