using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace ReactiveXaml
{
    public static class DependencyPropertyMixin
    {
        /// <summary>
        ///  
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="This"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IObservable<ObservedChange<TObj, TRet>> ObservableFromDP<TObj, TRet>(this TObj This, Expression<Func<TObj, TRet>> property)
            where TObj : FrameworkElement
        {
            Contract.Requires(This != null);

            // Track down the DP for this property
            var prop_name = RxApp.expressionToPropertyName(property);
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