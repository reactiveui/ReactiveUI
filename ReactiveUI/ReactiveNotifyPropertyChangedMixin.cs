using System;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace ReactiveUI
{
    public static class ReactiveNotifyPropertyChangedMixin
    {
        /// <summary>
        /// ObservableForProperty returns an Observable representing the
        /// property change notifications for a specific property on a
        /// ReactiveObject. This method (unlike other Observables that return
        /// IObservedChange) guarantees that the Value property of
        /// the IObservedChange is set.
        /// </summary>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty'</param>
        /// <param name="beforeChange">If True, the Observable will notify
        /// immediately before a property is going to change.</param>
        /// <returns>An Observable representing the property change
        /// notifications for the given property.</returns>
        public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(
                this TSender This, 
                Expression<Func<TSender, TValue>> property, 
                bool beforeChange = false)
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
        /// ObservableForProperty returns an Observable representing the
        /// property change notifications for a specific property on a
        /// ReactiveObject, running the IObservedChange through a Selector
        /// function.
        /// </summary>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty'</param>
        /// <param name="selector">A Select function that will be run on each
        /// item.</param>
        /// <param name="beforeChange">If True, the Observable will notify
        /// immediately before a property is going to change.</param>
        /// <returns>An Observable representing the property change
        /// notifications for the given property.</returns>
        public static IObservable<TRet> ObservableForProperty<TSender, TValue, TRet>(
                this TSender This, 
                Expression<Func<TSender, TValue>> property, 
                Func<TValue, TRet> selector, 
                bool beforeChange = false)
            where TSender : IReactiveNotifyPropertyChanged
        {           
            Contract.Requires(selector != null);
            return This.ObservableForProperty(property, beforeChange).Select(x => selector(x.Value));
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
