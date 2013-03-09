
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;


namespace ReactiveUI
{
    public static class WhenAnyMixin 
    {
                                        
        /// <summary>
        /// WhenAny allows you to observe whenever one or more properties on an
        /// object have changed, providing an initial value when the Observable
        /// is set up, unlike ObservableForProperty(). Use this method in
        /// constructors to set up bindings between properties that also need an
        /// initial setup.
        /// </summary>
        public static IObservable<TRet> WhenAny<TSender, TRet, T1>(this TSender This, 
                            Expression<Func<TSender, T1>> property1, 
                            Func<IObservedChange<TSender, T1>, TRet> selector)
        {
                            return This.ObservableForProperty(property1, false, false).Select(selector); 
                    }

        /// <summary>
        /// WhenAny allows you to observe whenever one or more properties on an
        /// object have changed, providing an initial value when the Observable
        /// is set up, unlike ObservableForProperty(). Use this method in
        /// constructors to set up bindings between properties that also need an
        /// initial setup.
        /// </summary>
        public static IObservable<TRet> WhenAnyDynamic<TSender, TRet>(this TSender This, 
                            string[] property1, 
                            Func<IObservedChange<TSender, object>, TRet> selector)
        {
                            return ReactiveNotifyPropertyChangedMixin
                    .SubscribeToExpressionChain<TSender,object>(This, property1, false, false).Select(selector); 
                    }

                                    
        /// <summary>
        /// WhenAny allows you to observe whenever one or more properties on an
        /// object have changed, providing an initial value when the Observable
        /// is set up, unlike ObservableForProperty(). Use this method in
        /// constructors to set up bindings between properties that also need an
        /// initial setup.
        /// </summary>
        public static IObservable<TRet> WhenAny<TSender, TRet, T1,T2>(this TSender This, 
                            Expression<Func<TSender, T1>> property1, 
                            Expression<Func<TSender, T2>> property2, 
                            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, TRet> selector)
        {
                        return Observable.CombineLatest(
                                    This.ObservableForProperty(property1, false, false), 
                                    This.ObservableForProperty(property2, false, false), 
                                selector
            );
                    }

        /// <summary>
        /// WhenAny allows you to observe whenever one or more properties on an
        /// object have changed, providing an initial value when the Observable
        /// is set up, unlike ObservableForProperty(). Use this method in
        /// constructors to set up bindings between properties that also need an
        /// initial setup.
        /// </summary>
        public static IObservable<TRet> WhenAnyDynamic<TSender, TRet>(this TSender This, 
                            string[] property1, 
                            string[] property2, 
                            Func<IObservedChange<TSender, object>, IObservedChange<TSender, object>, TRet> selector)
        {
                        return Observable.CombineLatest(
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property1, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property2, false, false), 
                                selector
            );
                    }

                                    
        /// <summary>
        /// WhenAny allows you to observe whenever one or more properties on an
        /// object have changed, providing an initial value when the Observable
        /// is set up, unlike ObservableForProperty(). Use this method in
        /// constructors to set up bindings between properties that also need an
        /// initial setup.
        /// </summary>
        public static IObservable<TRet> WhenAny<TSender, TRet, T1,T2,T3>(this TSender This, 
                            Expression<Func<TSender, T1>> property1, 
                            Expression<Func<TSender, T2>> property2, 
                            Expression<Func<TSender, T3>> property3, 
                            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, TRet> selector)
        {
                        return Observable.CombineLatest(
                                    This.ObservableForProperty(property1, false, false), 
                                    This.ObservableForProperty(property2, false, false), 
                                    This.ObservableForProperty(property3, false, false), 
                                selector
            );
                    }

        /// <summary>
        /// WhenAny allows you to observe whenever one or more properties on an
        /// object have changed, providing an initial value when the Observable
        /// is set up, unlike ObservableForProperty(). Use this method in
        /// constructors to set up bindings between properties that also need an
        /// initial setup.
        /// </summary>
        public static IObservable<TRet> WhenAnyDynamic<TSender, TRet>(this TSender This, 
                            string[] property1, 
                            string[] property2, 
                            string[] property3, 
                            Func<IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, TRet> selector)
        {
                        return Observable.CombineLatest(
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property1, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property2, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property3, false, false), 
                                selector
            );
                    }

                                    
        /// <summary>
        /// WhenAny allows you to observe whenever one or more properties on an
        /// object have changed, providing an initial value when the Observable
        /// is set up, unlike ObservableForProperty(). Use this method in
        /// constructors to set up bindings between properties that also need an
        /// initial setup.
        /// </summary>
        public static IObservable<TRet> WhenAny<TSender, TRet, T1,T2,T3,T4>(this TSender This, 
                            Expression<Func<TSender, T1>> property1, 
                            Expression<Func<TSender, T2>> property2, 
                            Expression<Func<TSender, T3>> property3, 
                            Expression<Func<TSender, T4>> property4, 
                            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, TRet> selector)
        {
                        return Observable.CombineLatest(
                                    This.ObservableForProperty(property1, false, false), 
                                    This.ObservableForProperty(property2, false, false), 
                                    This.ObservableForProperty(property3, false, false), 
                                    This.ObservableForProperty(property4, false, false), 
                                selector
            );
                    }

        /// <summary>
        /// WhenAny allows you to observe whenever one or more properties on an
        /// object have changed, providing an initial value when the Observable
        /// is set up, unlike ObservableForProperty(). Use this method in
        /// constructors to set up bindings between properties that also need an
        /// initial setup.
        /// </summary>
        public static IObservable<TRet> WhenAnyDynamic<TSender, TRet>(this TSender This, 
                            string[] property1, 
                            string[] property2, 
                            string[] property3, 
                            string[] property4, 
                            Func<IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, TRet> selector)
        {
                        return Observable.CombineLatest(
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property1, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property2, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property3, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property4, false, false), 
                                selector
            );
                    }

        }

    public static class WhenAnyObservableMixin
    {
        public static IObservable<TRet> WhenAnyObservable<TSender, TRet>(this TSender This, Expression<Func<TSender, IObservable<TRet>>> obs1)
        {
            return This.WhenAny(obs1, x => x.Value).Merge();
        }

                                	public static IObservable<TRet> WhenAnyObservable<TSender, TRet>(this TSender This, Expression<Func<TSender, IObservable<TRet>>> obs1, Expression<Func<TSender, IObservable<TRet>>> obs2)
        {
            return This.WhenAny(obs1, obs2, (o1, o2) => new[] {o1.Value, o2.Value})
                .SelectMany(x => x.Merge());
        }
                                	public static IObservable<TRet> WhenAnyObservable<TSender, TRet>(this TSender This, Expression<Func<TSender, IObservable<TRet>>> obs1, Expression<Func<TSender, IObservable<TRet>>> obs2, Expression<Func<TSender, IObservable<TRet>>> obs3)
        {
            return This.WhenAny(obs1, obs2, obs3, (o1, o2, o3) => new[] {o1.Value, o2.Value, o3.Value})
                .SelectMany(x => x.Merge());
        }
                                	public static IObservable<TRet> WhenAnyObservable<TSender, TRet>(this TSender This, Expression<Func<TSender, IObservable<TRet>>> obs1, Expression<Func<TSender, IObservable<TRet>>> obs2, Expression<Func<TSender, IObservable<TRet>>> obs3, Expression<Func<TSender, IObservable<TRet>>> obs4)
        {
            return This.WhenAny(obs1, obs2, obs3, obs4, (o1, o2, o3, o4) => new[] {o1.Value, o2.Value, o3.Value, o4.Value})
                .SelectMany(x => x.Merge());
        }
            }
}
