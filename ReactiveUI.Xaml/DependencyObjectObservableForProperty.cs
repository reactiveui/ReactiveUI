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
using System.Windows.Data;

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

        public class DPChangeProxy : DependencyObject 
        {

            public Subject<object> Notifications = new Subject<object>();
 
            public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
                "Value",
                typeof(object),
                typeof(DPChangeProxy),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback(OnValueChanged)
                )
            );

            private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                DPChangeProxy proxy = (DPChangeProxy)d;
                proxy.Notifications.OnNext(e.NewValue);
            }

            public double Value
            {
              get { return (double)GetValue(ValueProperty); }
              set { SetValue(ValueProperty, value); }
            }
        }

        public int GetAffinityForObject(Type type, bool beforeChanged = false)
        {
            return typeof(DependencyObject).IsAssignableFrom(type) ? 4 : 0;
        }

        static readonly Dictionary<Type, DependencyProperty> attachedProperties = new Dictionary<Type, DependencyProperty>();
        static readonly Dictionary<object, Tuple<Subject<object>, RefcountDisposeWrapper>> subjects = new Dictionary<object, Tuple<Subject<object>, RefcountDisposeWrapper>>();

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, string propertyName, bool beforeChanged = false)
        {
            Contract.Requires(sender != null && sender is DependencyObject);

            if (beforeChanged == true) return null;

            var dobj = sender as DependencyObject;
            var type = dobj.GetType();

            // Look for the DependencyProperty attached to this property name
#if WINRT
            var pi = type.GetProperty(propertyName + "Property", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (pi != null) {
                goto itWorks;
            }
#endif

            var fi = type.GetField(propertyName + "Property", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (fi == null) {
                this.Log().Debug("Tried to bind DO {0}.{1}, but DP doesn't exist. Binding as POCO object",
                    type.FullName, propertyName);
                var ret = new POCOObservableForProperty();
                return ret.GetNotificationForProperty(sender, propertyName, beforeChanged);
            }

#if !WINRT && !SILVERLIGHT
            return Observable.Create<IObservedChange<object, object>>(subj =>
            {
                var dp = (DependencyProperty)fi.GetValue(null);
                var dpd = DependencyPropertyDescriptor.FromProperty(dp, type);
                Action<object> fn = x => subj.OnNext(new ObservedChange<object,object>(){PropertyName=dp.Name, Sender=dobj, Value=x});
                var proxy = new DPChangeProxy();
                var disposable = proxy.Notifications.Subscribe(fn);
                System.Windows.Data.BindingOperations.SetBinding(proxy, DPChangeProxy.ValueProperty, new Binding(dp.Name){ Source = dobj});
                return Disposable.Create(() =>
                    {
                        disposable.Dispose();
                        BindingOperations.ClearBinding(proxy, DPChangeProxy.ValueProperty);
                    });

            });
#else

        itWorks:
            return Observable.Create<IObservedChange<object, object>>(subj => {
                DependencyProperty attachedProp;

                if (!attachedProperties.ContainsKey(type)) {
                    // NB: There is no way to unregister an attached property, 
                    // we just have to leak it. Luckily it's per-type, so it's
                    // not *that* bad.
                    attachedProp = DependencyProperty.RegisterAttached(
                        "ListenAttached" + propertyName + this.GetHashCode().ToString("{0:x}"),
                        typeof(object), type,
                        new PropertyMetadata(null, (o,e) => subjects[o].Item1.OnNext(o)));
                    attachedProperties[type] = attachedProp;
                } else {
                    attachedProp = attachedProperties[type];
                }

                // Here's the idea for this cracked-out code:
                //
                // The reason we're doing all of this is that we can only 
                // create a single binding between a DependencyObject and its 
                // attached property, yet we could have multiple people 
                // interested in this property. We should only drop the actual
                // Binding once nobody is listening anymore.
                if (!subjects.ContainsKey(sender)) {
                    var disposer = new RefcountDisposeWrapper(
                        Disposable.Create(() => {
#if !SILVERLIGHT && !WINRT
                            // XXX: Apparently it's simply impossible to unset a binding in SL :-/
                            BindingOperations.ClearBinding(dobj, attachedProp);
#endif
                            subjects.Remove(dobj);
                        }));

                    subjects[sender] = Tuple.Create(new Subject<object>(), disposer);

                    var b = new Binding() { Source = dobj, Path = new PropertyPath(propertyName) };
                    BindingOperations.SetBinding(dobj, attachedProp, b);
                } else {
                    subjects[sender].Item2.AddRef();
                }

                var disp = subjects[sender].Item1
                    .Select(x => (IObservedChange<object, object>) new ObservedChange<object, object>() { Sender = x, PropertyName = propertyName })
                    .Subscribe(subj);

                return Disposable.Create(() => {
                    disp.Dispose();
                    subjects[sender].Item2.Release();
                });
            });
#endif
        }
    }
}
