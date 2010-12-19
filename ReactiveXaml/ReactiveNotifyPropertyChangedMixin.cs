using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Concurrency;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace ReactiveXaml
{
    public static class ReactiveNotifyPropertyChangedMixin
    {
        public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(this TSender This, Expression<Func<TSender, TValue>> Property, bool BeforeChange = false)
            where TSender : IReactiveNotifyPropertyChanged
        {
            Contract.Requires(This != null);
            Contract.Requires(Property != null);

            string prop_name = RxApp.expressionToPropertyName(Property);
            var prop_info = RxApp.getPropertyInfoForProperty<TSender>(prop_name);

            This.Log().InfoFormat("Registering change notification for {0:X} on {1}", This.GetHashCode(), prop_name);

            if (BeforeChange) {
                return This.Changing
                    .Where(x => x.PropertyName == prop_name)
                    .Select(x => (IObservedChange<TSender, TValue>) new ObservedChange<TSender, TValue>() { 
                        Sender = This, PropertyName = prop_name, Value = (TValue)prop_info.GetValue(This, null)
                    });
            }

            return This.Changed
                .Where(x => x.PropertyName == prop_name)
                .Select(x => (IObservedChange<TSender, TValue>) new ObservedChange<TSender, TValue>() { 
                    Sender = This, PropertyName = prop_name, Value = (TValue)prop_info.GetValue(This, null)
                });
        }

        public static IObservable<TRet> ObservableForProperty<TSender, TValue, TRet>(this TSender This, Expression<Func<TSender, TValue>> Property, Func<TValue, TRet> Selector, bool BeforeChange = false)
            where TSender : IReactiveNotifyPropertyChanged
        {           
            Contract.Requires(Selector != null);
            return This.ObservableForProperty(Property, BeforeChange).Select(x => Selector(x.Value));
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :