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
        [Obsolete("Use the Expression-based version instead!")]
        public static IObservable<ObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(this TSender This, string propertyName)
            where TSender : IReactiveNotifyPropertyChanged
        {
            Contract.Requires(This != null); 

            return This.Where(x => x.PropertyName == propertyName)
                       .Select(x => new ObservedChange<TSender, TValue> { Sender = This, PropertyName = x.PropertyName });
        }

        public static IObservable<ObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(this TSender This, Expression<Func<TSender, TValue>> Property, bool BeforeChange = false)
            where TSender : IReactiveNotifyPropertyChanged
        {
            Contract.Requires(This != null);
            Contract.Requires(Property != null);

            string prop_name = RxApp.expressionToPropertyName(Property);
            var prop_info = RxApp.getPropertyInfoForProperty<TSender>(prop_name);

            if (BeforeChange) {
                return This.BeforeChange
                    .Where(x => x.PropertyName == prop_name)
                    .Select(x => new ObservedChange<TSender, TValue>() { 
                        Sender = This, PropertyName = prop_name, Value = (TValue)prop_info.GetValue(This, null)
                    });
            } else {
                return This
                    .Where(x => x.PropertyName == prop_name)
                    .Select(x => new ObservedChange<TSender, TValue>() { 
                        Sender = This, PropertyName = prop_name, Value = (TValue)prop_info.GetValue(This, null)
                    });
            }
        }

        public static IObservable<TRet> ObservableForProperty<TSender, TValue, TRet>(this TSender This, Expression<Func<TSender, TValue>> Property, Func<TValue, TRet> Selector, bool BeforeChange = false)
            where TSender : IReactiveNotifyPropertyChanged
        {
            Contract.Requires(Selector != null);
            return This.ObservableForProperty(Property, BeforeChange).Select(x => Selector(x.Value));
        }
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :
