using System;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace ReactiveXaml
{
    public static class ReactiveNotifyPropertyChangedMixin
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSender"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="This"></param>
        /// <param name="property"></param>
        /// <param name="beforeChange"></param>
        /// <returns></returns>
        public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(this TSender This, Expression<Func<TSender, TValue>> property, bool beforeChange = false)
            where TSender : IReactiveNotifyPropertyChanged
        {
            Contract.Requires(This != null);
            Contract.Requires(property != null);

            string prop_name = RxApp.expressionToPropertyName(property);
            var prop_info = RxApp.getPropertyInfoForProperty<TSender>(prop_name);

            This.Log().InfoFormat("Registering change notification for {0:X} on {1}", This.GetHashCode(), prop_name);

            if (beforeChange) {
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSender"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="This"></param>
        /// <param name="property"></param>
        /// <param name="selector"></param>
        /// <param name="beforeChange"></param>
        /// <returns></returns>
        public static IObservable<TRet> ObservableForProperty<TSender, TValue, TRet>(this TSender This, Expression<Func<TSender, TValue>> property, Func<TValue, TRet> selector, bool beforeChange = false)
            where TSender : IReactiveNotifyPropertyChanged
        {           
            Contract.Requires(selector != null);
            return This.ObservableForProperty(property, beforeChange).Select(x => selector(x.Value));
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :