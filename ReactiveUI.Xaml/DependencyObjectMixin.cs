using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace ReactiveUI.Xaml
{
    public static class DependencyPropertyMixin
    {
        /// <summary>
        /// Creates an IObservable from an existing dependency property. Note
        /// that this method is somewhat expensive and should not be called
        /// frequently.
        /// </summary>
        /// <param name="property">An Expression specifying the property to use
        /// on the DependencyObject (e.g. x => x.SomeProperty)</param>
        /// <returns>An Observable that fires whenever the DP changes, and never
        /// completes.</returns>
        public static IObservable<ObservedChange<TObj, TRet>> ObservableFromDP<TObj, TRet>(
                this TObj This, 
                Expression<Func<TObj, TRet>> property)
            where TObj : FrameworkElement
        {
            Contract.Requires(This != null);

            // Track down the DP for this property
            var prop_name = RxApp.simpleExpressionToPropertyName(property);
            var fi = typeof(TObj).GetField(prop_name + "Property", BindingFlags.Public | BindingFlags.Static);
            var dp = fi.GetValue(This) as DependencyProperty;

            return new ObservableFromDPHelper<TObj, TRet>(This, dp, prop_name);
        }

        class ObservableFromDPHelper<TObj,TRet> : IObservable<ObservedChange<TObj,TRet>>
            where TObj : FrameworkElement
        {
            TObj source;
            string propName;
            PropertyInfo propGetter;
            Subject<ObservedChange<TObj, TRet>> subject = new Subject<ObservedChange<TObj, TRet>>();

            public ObservableFromDPHelper(TObj dobj, DependencyProperty dp, string propName)
            {
                var b = new Binding(propName) { Source = dobj };
                var prop = System.Windows.DependencyProperty.RegisterAttached(
                    "ListenAttached" + propName + this.GetHashCode().ToString("{0:x}"),
                    typeof(object),
                    typeof(TObj),
                    new PropertyMetadata(new PropertyChangedCallback(onPropertyChanged)));

                source = dobj;
                this.propName = propName;
                propGetter = typeof(TObj).GetProperty(propName);
                dobj.SetBinding(prop, b);
            }

            void onPropertyChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs args)
            {
                subject.OnNext(new ObservedChange<TObj, TRet>() { 
                    PropertyName = propName, 
                    Sender = source,
                    Value = (TRet)propGetter.GetValue(Sender, null)
                });
            }

            public IDisposable Subscribe(IObserver<ObservedChange<TObj, TRet>> observer)
            {
                return subject.Subscribe(observer);
            }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
