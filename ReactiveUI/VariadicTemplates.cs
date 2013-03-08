
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

                                    
        /// <summary>
        /// WhenAny allows you to observe whenever one or more properties on an
        /// object have changed, providing an initial value when the Observable
        /// is set up, unlike ObservableForProperty(). Use this method in
        /// constructors to set up bindings between properties that also need an
        /// initial setup.
        /// </summary>
        public static IObservable<TRet> WhenAny<TSender, TRet, T1,T2,T3,T4,T5>(this TSender This, 
                            Expression<Func<TSender, T1>> property1, 
                            Expression<Func<TSender, T2>> property2, 
                            Expression<Func<TSender, T3>> property3, 
                            Expression<Func<TSender, T4>> property4, 
                            Expression<Func<TSender, T5>> property5, 
                            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, IObservedChange<TSender, T5>, TRet> selector)
        {
                        return Observable.CombineLatest(
                                    This.ObservableForProperty(property1, false, false), 
                                    This.ObservableForProperty(property2, false, false), 
                                    This.ObservableForProperty(property3, false, false), 
                                    This.ObservableForProperty(property4, false, false), 
                                    This.ObservableForProperty(property5, false, false), 
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
                            string[] property5, 
                            Func<IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, TRet> selector)
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
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property5, false, false), 
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
        public static IObservable<TRet> WhenAny<TSender, TRet, T1,T2,T3,T4,T5,T6>(this TSender This, 
                            Expression<Func<TSender, T1>> property1, 
                            Expression<Func<TSender, T2>> property2, 
                            Expression<Func<TSender, T3>> property3, 
                            Expression<Func<TSender, T4>> property4, 
                            Expression<Func<TSender, T5>> property5, 
                            Expression<Func<TSender, T6>> property6, 
                            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, IObservedChange<TSender, T5>, IObservedChange<TSender, T6>, TRet> selector)
        {
                        return Observable.CombineLatest(
                                    This.ObservableForProperty(property1, false, false), 
                                    This.ObservableForProperty(property2, false, false), 
                                    This.ObservableForProperty(property3, false, false), 
                                    This.ObservableForProperty(property4, false, false), 
                                    This.ObservableForProperty(property5, false, false), 
                                    This.ObservableForProperty(property6, false, false), 
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
                            string[] property5, 
                            string[] property6, 
                            Func<IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, TRet> selector)
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
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property5, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property6, false, false), 
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
        public static IObservable<TRet> WhenAny<TSender, TRet, T1,T2,T3,T4,T5,T6,T7>(this TSender This, 
                            Expression<Func<TSender, T1>> property1, 
                            Expression<Func<TSender, T2>> property2, 
                            Expression<Func<TSender, T3>> property3, 
                            Expression<Func<TSender, T4>> property4, 
                            Expression<Func<TSender, T5>> property5, 
                            Expression<Func<TSender, T6>> property6, 
                            Expression<Func<TSender, T7>> property7, 
                            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, IObservedChange<TSender, T5>, IObservedChange<TSender, T6>, IObservedChange<TSender, T7>, TRet> selector)
        {
                        return Observable.CombineLatest(
                                    This.ObservableForProperty(property1, false, false), 
                                    This.ObservableForProperty(property2, false, false), 
                                    This.ObservableForProperty(property3, false, false), 
                                    This.ObservableForProperty(property4, false, false), 
                                    This.ObservableForProperty(property5, false, false), 
                                    This.ObservableForProperty(property6, false, false), 
                                    This.ObservableForProperty(property7, false, false), 
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
                            string[] property5, 
                            string[] property6, 
                            string[] property7, 
                            Func<IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, TRet> selector)
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
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property5, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property6, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property7, false, false), 
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
        public static IObservable<TRet> WhenAny<TSender, TRet, T1,T2,T3,T4,T5,T6,T7,T8>(this TSender This, 
                            Expression<Func<TSender, T1>> property1, 
                            Expression<Func<TSender, T2>> property2, 
                            Expression<Func<TSender, T3>> property3, 
                            Expression<Func<TSender, T4>> property4, 
                            Expression<Func<TSender, T5>> property5, 
                            Expression<Func<TSender, T6>> property6, 
                            Expression<Func<TSender, T7>> property7, 
                            Expression<Func<TSender, T8>> property8, 
                            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, IObservedChange<TSender, T5>, IObservedChange<TSender, T6>, IObservedChange<TSender, T7>, IObservedChange<TSender, T8>, TRet> selector)
        {
                        return Observable.CombineLatest(
                                    This.ObservableForProperty(property1, false, false), 
                                    This.ObservableForProperty(property2, false, false), 
                                    This.ObservableForProperty(property3, false, false), 
                                    This.ObservableForProperty(property4, false, false), 
                                    This.ObservableForProperty(property5, false, false), 
                                    This.ObservableForProperty(property6, false, false), 
                                    This.ObservableForProperty(property7, false, false), 
                                    This.ObservableForProperty(property8, false, false), 
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
                            string[] property5, 
                            string[] property6, 
                            string[] property7, 
                            string[] property8, 
                            Func<IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, TRet> selector)
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
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property5, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property6, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property7, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property8, false, false), 
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
        public static IObservable<TRet> WhenAny<TSender, TRet, T1,T2,T3,T4,T5,T6,T7,T8,T9>(this TSender This, 
                            Expression<Func<TSender, T1>> property1, 
                            Expression<Func<TSender, T2>> property2, 
                            Expression<Func<TSender, T3>> property3, 
                            Expression<Func<TSender, T4>> property4, 
                            Expression<Func<TSender, T5>> property5, 
                            Expression<Func<TSender, T6>> property6, 
                            Expression<Func<TSender, T7>> property7, 
                            Expression<Func<TSender, T8>> property8, 
                            Expression<Func<TSender, T9>> property9, 
                            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, IObservedChange<TSender, T5>, IObservedChange<TSender, T6>, IObservedChange<TSender, T7>, IObservedChange<TSender, T8>, IObservedChange<TSender, T9>, TRet> selector)
        {
                        return Observable.CombineLatest(
                                    This.ObservableForProperty(property1, false, false), 
                                    This.ObservableForProperty(property2, false, false), 
                                    This.ObservableForProperty(property3, false, false), 
                                    This.ObservableForProperty(property4, false, false), 
                                    This.ObservableForProperty(property5, false, false), 
                                    This.ObservableForProperty(property6, false, false), 
                                    This.ObservableForProperty(property7, false, false), 
                                    This.ObservableForProperty(property8, false, false), 
                                    This.ObservableForProperty(property9, false, false), 
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
                            string[] property5, 
                            string[] property6, 
                            string[] property7, 
                            string[] property8, 
                            string[] property9, 
                            Func<IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, TRet> selector)
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
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property5, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property6, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property7, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property8, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property9, false, false), 
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
        public static IObservable<TRet> WhenAny<TSender, TRet, T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>(this TSender This, 
                            Expression<Func<TSender, T1>> property1, 
                            Expression<Func<TSender, T2>> property2, 
                            Expression<Func<TSender, T3>> property3, 
                            Expression<Func<TSender, T4>> property4, 
                            Expression<Func<TSender, T5>> property5, 
                            Expression<Func<TSender, T6>> property6, 
                            Expression<Func<TSender, T7>> property7, 
                            Expression<Func<TSender, T8>> property8, 
                            Expression<Func<TSender, T9>> property9, 
                            Expression<Func<TSender, T10>> property10, 
                            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, IObservedChange<TSender, T5>, IObservedChange<TSender, T6>, IObservedChange<TSender, T7>, IObservedChange<TSender, T8>, IObservedChange<TSender, T9>, IObservedChange<TSender, T10>, TRet> selector)
        {
                        return Observable.CombineLatest(
                                    This.ObservableForProperty(property1, false, false), 
                                    This.ObservableForProperty(property2, false, false), 
                                    This.ObservableForProperty(property3, false, false), 
                                    This.ObservableForProperty(property4, false, false), 
                                    This.ObservableForProperty(property5, false, false), 
                                    This.ObservableForProperty(property6, false, false), 
                                    This.ObservableForProperty(property7, false, false), 
                                    This.ObservableForProperty(property8, false, false), 
                                    This.ObservableForProperty(property9, false, false), 
                                    This.ObservableForProperty(property10, false, false), 
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
                            string[] property5, 
                            string[] property6, 
                            string[] property7, 
                            string[] property8, 
                            string[] property9, 
                            string[] property10, 
                            Func<IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, TRet> selector)
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
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property5, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property6, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property7, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property8, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property9, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property10, false, false), 
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
        public static IObservable<TRet> WhenAny<TSender, TRet, T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11>(this TSender This, 
                            Expression<Func<TSender, T1>> property1, 
                            Expression<Func<TSender, T2>> property2, 
                            Expression<Func<TSender, T3>> property3, 
                            Expression<Func<TSender, T4>> property4, 
                            Expression<Func<TSender, T5>> property5, 
                            Expression<Func<TSender, T6>> property6, 
                            Expression<Func<TSender, T7>> property7, 
                            Expression<Func<TSender, T8>> property8, 
                            Expression<Func<TSender, T9>> property9, 
                            Expression<Func<TSender, T10>> property10, 
                            Expression<Func<TSender, T11>> property11, 
                            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, IObservedChange<TSender, T5>, IObservedChange<TSender, T6>, IObservedChange<TSender, T7>, IObservedChange<TSender, T8>, IObservedChange<TSender, T9>, IObservedChange<TSender, T10>, IObservedChange<TSender, T11>, TRet> selector)
        {
                        return Observable.CombineLatest(
                                    This.ObservableForProperty(property1, false, false), 
                                    This.ObservableForProperty(property2, false, false), 
                                    This.ObservableForProperty(property3, false, false), 
                                    This.ObservableForProperty(property4, false, false), 
                                    This.ObservableForProperty(property5, false, false), 
                                    This.ObservableForProperty(property6, false, false), 
                                    This.ObservableForProperty(property7, false, false), 
                                    This.ObservableForProperty(property8, false, false), 
                                    This.ObservableForProperty(property9, false, false), 
                                    This.ObservableForProperty(property10, false, false), 
                                    This.ObservableForProperty(property11, false, false), 
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
                            string[] property5, 
                            string[] property6, 
                            string[] property7, 
                            string[] property8, 
                            string[] property9, 
                            string[] property10, 
                            string[] property11, 
                            Func<IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, TRet> selector)
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
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property5, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property6, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property7, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property8, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property9, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property10, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property11, false, false), 
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
        public static IObservable<TRet> WhenAny<TSender, TRet, T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12>(this TSender This, 
                            Expression<Func<TSender, T1>> property1, 
                            Expression<Func<TSender, T2>> property2, 
                            Expression<Func<TSender, T3>> property3, 
                            Expression<Func<TSender, T4>> property4, 
                            Expression<Func<TSender, T5>> property5, 
                            Expression<Func<TSender, T6>> property6, 
                            Expression<Func<TSender, T7>> property7, 
                            Expression<Func<TSender, T8>> property8, 
                            Expression<Func<TSender, T9>> property9, 
                            Expression<Func<TSender, T10>> property10, 
                            Expression<Func<TSender, T11>> property11, 
                            Expression<Func<TSender, T12>> property12, 
                            Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, IObservedChange<TSender, T3>, IObservedChange<TSender, T4>, IObservedChange<TSender, T5>, IObservedChange<TSender, T6>, IObservedChange<TSender, T7>, IObservedChange<TSender, T8>, IObservedChange<TSender, T9>, IObservedChange<TSender, T10>, IObservedChange<TSender, T11>, IObservedChange<TSender, T12>, TRet> selector)
        {
                        return Observable.CombineLatest(
                                    This.ObservableForProperty(property1, false, false), 
                                    This.ObservableForProperty(property2, false, false), 
                                    This.ObservableForProperty(property3, false, false), 
                                    This.ObservableForProperty(property4, false, false), 
                                    This.ObservableForProperty(property5, false, false), 
                                    This.ObservableForProperty(property6, false, false), 
                                    This.ObservableForProperty(property7, false, false), 
                                    This.ObservableForProperty(property8, false, false), 
                                    This.ObservableForProperty(property9, false, false), 
                                    This.ObservableForProperty(property10, false, false), 
                                    This.ObservableForProperty(property11, false, false), 
                                    This.ObservableForProperty(property12, false, false), 
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
                            string[] property5, 
                            string[] property6, 
                            string[] property7, 
                            string[] property8, 
                            string[] property9, 
                            string[] property10, 
                            string[] property11, 
                            string[] property12, 
                            Func<IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, IObservedChange<TSender, object>, TRet> selector)
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
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property5, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property6, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property7, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property8, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property9, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property10, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property11, false, false), 
                                    ReactiveNotifyPropertyChangedMixin
                        .SubscribeToExpressionChain<TSender,object>(This, property12, false, false), 
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
                                	public static IObservable<TRet> WhenAnyObservable<TSender, TRet>(this TSender This, Expression<Func<TSender, IObservable<TRet>>> obs1, Expression<Func<TSender, IObservable<TRet>>> obs2, Expression<Func<TSender, IObservable<TRet>>> obs3, Expression<Func<TSender, IObservable<TRet>>> obs4, Expression<Func<TSender, IObservable<TRet>>> obs5)
        {
            return This.WhenAny(obs1, obs2, obs3, obs4, obs5, (o1, o2, o3, o4, o5) => new[] {o1.Value, o2.Value, o3.Value, o4.Value, o5.Value})
                .SelectMany(x => x.Merge());
        }
                                	public static IObservable<TRet> WhenAnyObservable<TSender, TRet>(this TSender This, Expression<Func<TSender, IObservable<TRet>>> obs1, Expression<Func<TSender, IObservable<TRet>>> obs2, Expression<Func<TSender, IObservable<TRet>>> obs3, Expression<Func<TSender, IObservable<TRet>>> obs4, Expression<Func<TSender, IObservable<TRet>>> obs5, Expression<Func<TSender, IObservable<TRet>>> obs6)
        {
            return This.WhenAny(obs1, obs2, obs3, obs4, obs5, obs6, (o1, o2, o3, o4, o5, o6) => new[] {o1.Value, o2.Value, o3.Value, o4.Value, o5.Value, o6.Value})
                .SelectMany(x => x.Merge());
        }
                                	public static IObservable<TRet> WhenAnyObservable<TSender, TRet>(this TSender This, Expression<Func<TSender, IObservable<TRet>>> obs1, Expression<Func<TSender, IObservable<TRet>>> obs2, Expression<Func<TSender, IObservable<TRet>>> obs3, Expression<Func<TSender, IObservable<TRet>>> obs4, Expression<Func<TSender, IObservable<TRet>>> obs5, Expression<Func<TSender, IObservable<TRet>>> obs6, Expression<Func<TSender, IObservable<TRet>>> obs7)
        {
            return This.WhenAny(obs1, obs2, obs3, obs4, obs5, obs6, obs7, (o1, o2, o3, o4, o5, o6, o7) => new[] {o1.Value, o2.Value, o3.Value, o4.Value, o5.Value, o6.Value, o7.Value})
                .SelectMany(x => x.Merge());
        }
                                	public static IObservable<TRet> WhenAnyObservable<TSender, TRet>(this TSender This, Expression<Func<TSender, IObservable<TRet>>> obs1, Expression<Func<TSender, IObservable<TRet>>> obs2, Expression<Func<TSender, IObservable<TRet>>> obs3, Expression<Func<TSender, IObservable<TRet>>> obs4, Expression<Func<TSender, IObservable<TRet>>> obs5, Expression<Func<TSender, IObservable<TRet>>> obs6, Expression<Func<TSender, IObservable<TRet>>> obs7, Expression<Func<TSender, IObservable<TRet>>> obs8)
        {
            return This.WhenAny(obs1, obs2, obs3, obs4, obs5, obs6, obs7, obs8, (o1, o2, o3, o4, o5, o6, o7, o8) => new[] {o1.Value, o2.Value, o3.Value, o4.Value, o5.Value, o6.Value, o7.Value, o8.Value})
                .SelectMany(x => x.Merge());
        }
                                	public static IObservable<TRet> WhenAnyObservable<TSender, TRet>(this TSender This, Expression<Func<TSender, IObservable<TRet>>> obs1, Expression<Func<TSender, IObservable<TRet>>> obs2, Expression<Func<TSender, IObservable<TRet>>> obs3, Expression<Func<TSender, IObservable<TRet>>> obs4, Expression<Func<TSender, IObservable<TRet>>> obs5, Expression<Func<TSender, IObservable<TRet>>> obs6, Expression<Func<TSender, IObservable<TRet>>> obs7, Expression<Func<TSender, IObservable<TRet>>> obs8, Expression<Func<TSender, IObservable<TRet>>> obs9)
        {
            return This.WhenAny(obs1, obs2, obs3, obs4, obs5, obs6, obs7, obs8, obs9, (o1, o2, o3, o4, o5, o6, o7, o8, o9) => new[] {o1.Value, o2.Value, o3.Value, o4.Value, o5.Value, o6.Value, o7.Value, o8.Value, o9.Value})
                .SelectMany(x => x.Merge());
        }
                                	public static IObservable<TRet> WhenAnyObservable<TSender, TRet>(this TSender This, Expression<Func<TSender, IObservable<TRet>>> obs1, Expression<Func<TSender, IObservable<TRet>>> obs2, Expression<Func<TSender, IObservable<TRet>>> obs3, Expression<Func<TSender, IObservable<TRet>>> obs4, Expression<Func<TSender, IObservable<TRet>>> obs5, Expression<Func<TSender, IObservable<TRet>>> obs6, Expression<Func<TSender, IObservable<TRet>>> obs7, Expression<Func<TSender, IObservable<TRet>>> obs8, Expression<Func<TSender, IObservable<TRet>>> obs9, Expression<Func<TSender, IObservable<TRet>>> obs10)
        {
            return This.WhenAny(obs1, obs2, obs3, obs4, obs5, obs6, obs7, obs8, obs9, obs10, (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10) => new[] {o1.Value, o2.Value, o3.Value, o4.Value, o5.Value, o6.Value, o7.Value, o8.Value, o9.Value, o10.Value})
                .SelectMany(x => x.Merge());
        }
                                	public static IObservable<TRet> WhenAnyObservable<TSender, TRet>(this TSender This, Expression<Func<TSender, IObservable<TRet>>> obs1, Expression<Func<TSender, IObservable<TRet>>> obs2, Expression<Func<TSender, IObservable<TRet>>> obs3, Expression<Func<TSender, IObservable<TRet>>> obs4, Expression<Func<TSender, IObservable<TRet>>> obs5, Expression<Func<TSender, IObservable<TRet>>> obs6, Expression<Func<TSender, IObservable<TRet>>> obs7, Expression<Func<TSender, IObservable<TRet>>> obs8, Expression<Func<TSender, IObservable<TRet>>> obs9, Expression<Func<TSender, IObservable<TRet>>> obs10, Expression<Func<TSender, IObservable<TRet>>> obs11)
        {
            return This.WhenAny(obs1, obs2, obs3, obs4, obs5, obs6, obs7, obs8, obs9, obs10, obs11, (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11) => new[] {o1.Value, o2.Value, o3.Value, o4.Value, o5.Value, o6.Value, o7.Value, o8.Value, o9.Value, o10.Value, o11.Value})
                .SelectMany(x => x.Merge());
        }
                                	public static IObservable<TRet> WhenAnyObservable<TSender, TRet>(this TSender This, Expression<Func<TSender, IObservable<TRet>>> obs1, Expression<Func<TSender, IObservable<TRet>>> obs2, Expression<Func<TSender, IObservable<TRet>>> obs3, Expression<Func<TSender, IObservable<TRet>>> obs4, Expression<Func<TSender, IObservable<TRet>>> obs5, Expression<Func<TSender, IObservable<TRet>>> obs6, Expression<Func<TSender, IObservable<TRet>>> obs7, Expression<Func<TSender, IObservable<TRet>>> obs8, Expression<Func<TSender, IObservable<TRet>>> obs9, Expression<Func<TSender, IObservable<TRet>>> obs10, Expression<Func<TSender, IObservable<TRet>>> obs11, Expression<Func<TSender, IObservable<TRet>>> obs12)
        {
            return This.WhenAny(obs1, obs2, obs3, obs4, obs5, obs6, obs7, obs8, obs9, obs10, obs11, obs12, (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12) => new[] {o1.Value, o2.Value, o3.Value, o4.Value, o5.Value, o6.Value, o7.Value, o8.Value, o9.Value, o10.Value, o11.Value, o12.Value})
                .SelectMany(x => x.Merge());
        }
            }
}
