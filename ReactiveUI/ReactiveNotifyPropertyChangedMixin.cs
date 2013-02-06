using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;

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
        /// 'x => x.SomeProperty.SomeOtherProperty'</param>
        /// <param name="beforeChange">If True, the Observable will notify
        /// immediately before a property is going to change.</param>
        /// <returns>An Observable representing the property change
        /// notifications for the given property.</returns>
        public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(
                this TSender This,
                Expression<Func<TSender, TValue>> property,
                bool beforeChange = false)
        {
            var propertyNames = new LinkedList<string>(Reflection.ExpressionToPropertyNames(property));
            var subscriptions = new LinkedList<IDisposable>(propertyNames.Select(x => (IDisposable) null));
            var ret = new Subject<IObservedChange<TSender, TValue>>();

            if (This == null) {
                throw new ArgumentNullException("Sender");
            }

            /* x => x.Foo.Bar.Baz;
             * 
             * Subscribe to This, look for Foo
             * Subscribe to Foo, look for Bar
             * Subscribe to Bar, look for Baz
             * Subscribe to Baz, publish to Subject
             * Return Subject
             * 
             * If Bar changes (notification fires on Foo), resubscribe to new Bar
             * 	Resubscribe to new Baz, publish to Subject
             * 
             * If Baz changes (notification fires on Bar),
             * 	Resubscribe to new Baz, publish to Subject
             */

            return subscribeToExpressionChain<TSender, TValue>(
                This,
                propertyNames,
                beforeChange
                );

            //subscribeToExpressionChain(
            //    This, 
            //    buildPropPathFromNodePtr(propertyNames.First),
            //    This, 
            //    propertyNames.First, 
            //    subscriptions.First, 
            //    beforeChange, 
            //    ret);

            //return Observable.Create<IObservedChange<TSender, TValue>>(x => {
            //    var disp = ret.Subscribe(x);
            //    return () => {
            //        subscriptions.ForEach(y => y.Dispose());
            //        disp.Dispose();
            //    };
            //});
        }

        /// <summary>
        /// ObservableForPropertyDynamic returns an Observable representing the
        /// property change notifications for a specific property on a
        /// ReactiveObject. This method (unlike other Observables that return
        /// IObservedChange) guarantees that the Value property of
        /// the IObservedChange is set.
        /// </summary>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty.SomeOtherProperty'</param>
        /// <param name="beforeChange">If True, the Observable will notify
        /// immediately before a property is going to change.</param>
        /// <returns>An Observable representing the property change
        /// notifications for the given property.</returns>
        public static IObservable<IObservedChange<TSender, object>> ObservableForProperty<TSender>(
                this TSender This,
                string[] property,
                bool beforeChange = false)
        {
            var propertyNames = new LinkedList<string>(property);
            var subscriptions = new LinkedList<IDisposable>(propertyNames.Select(x => (IDisposable) null));
            var ret = new Subject<IObservedChange<TSender, object>>();

            if (This == null) {
                throw new ArgumentNullException("Sender");
            }

            /* x => x.Foo.Bar.Baz;
             * 
             * Subscribe to This, look for Foo
             * Subscribe to Foo, look for Bar
             * Subscribe to Bar, look for Baz
             * Subscribe to Baz, publish to Subject
             * Return Subject
             * 
             * If Bar changes (notification fires on Foo), resubscribe to new Bar
             * 	Resubscribe to new Baz, publish to Subject
             * 
             * If Baz changes (notification fires on Bar),
             * 	Resubscribe to new Baz, publish to Subject
             */

            return subscribeToExpressionChain<TSender, object>(
                This,
                propertyNames,
                beforeChange
                );

        }


        private static IObservedChange<object, object> observedChangeFor(string propertyName, IObservedChange<object, object> y)
        {
            var p = new ObservedChange<object, object>()
            { Sender = y.Value
            , PropertyName = propertyName
            };

            if (y.Value==null)
            {
                return p;
            }
            
            return p.fillInValue();
        }

        private static IObservable<IObservedChange<object, object>> nestedObservedChanges(string propertyName, IObservedChange<object, object> y)
        {
            // Make sure a change at a root node propogates events down
            var kicker = observedChangeFor(propertyName, y);

            // Handle null values in the chain
            if (y.Value == null)
                return Observable.Return(kicker);

            // Handle non null values in the chain
            return notifyForProperty(y.Value, propertyName, false)
                .Select(x => x.fillInValue())
                .StartWith(kicker);
        }

        static IObservable<IObservedChange<TSender, TValue>> subscribeToExpressionChain<TSender, TValue> ( 
                TSender source,
                IEnumerable<string> propertyNames, 
                bool beforeChange
            )
        {
            var path = String.Join(".", propertyNames);

            IObservable<IObservedChange<object, object>> notifier
                = Observable.Return
                    (new ObservedChange<object, object>() 
                        { Value = source });

            notifier = propertyNames.Aggregate(notifier, (n, name) => n
                        .Select(y => nestedObservedChanges(name, y))
                        .Switch());

            return notifier.Select(x => x.fillInValue())
                .Select(x => new ObservedChange<TSender, TValue>()
                { Sender = source
                , PropertyName = path
                , Value = (TValue)x.Value
                }).Skip(1);
        }



        //static void subscribeToExpressionChain<TSender, TValue>(
        //        TSender origSource,
        //        string origPath,
        //        object source,
        //        LinkedListNode<string> propertyNames, 
        //        LinkedListNode<IDisposable> subscriptions, 
        //        bool beforeChange,
        //        Subject<IObservedChange<TSender, TValue>> subject
        //    )
        //{
        //    var current = propertyNames;
        //    var currentSub = subscriptions;
        //    object currentObj = source;
        //    ObservedChange<TSender, TValue> obsCh;

        //    while(current.Next != null) {
        //        Func<object, object> getter = null;

        //        if (currentObj != null) {
        //            getter = Reflection.GetValueFetcherForProperty(currentObj.GetType(), current.Value);

        //            if (getter == null) {
        //                subscriptions.List.Where(x => x != null).ForEach(x => x.Dispose());
        //                throw new ArgumentException(String.Format("Property '{0}' does not exist in expression", current.Value));
        //            }

        //            var capture = new {current, currentObj, getter, currentSub};

        //            var toDispose = new IDisposable[2];

        //            var valGetter = new ObservedChange<object, TValue>() {
        //                Sender = capture.currentObj,
        //                PropertyName = buildPropPathFromNodePtr(capture.current),
        //                Value = default(TValue),
        //            };

        //            TValue prevVal = default(TValue);
        //            bool prevValSet = valGetter.TryGetValue(out prevVal);

        //            // NB: Some notifyForProperty implementations (notably, 
        //            // DependencyProperties) don't actually support beforeChanged,
        //            // but they need to prevent others from claiming it, since 
        //            // POCOObservableForProperty works with all objects. They 
        //            // do this by returning NULL.
        //            var beforePropChangedObs = notifyForProperty(currentObj, capture.current.Value, true) ?? Observable.Return(default(IObservedChange<object, object>));
        //            toDispose[0] = beforePropChangedObs.Subscribe(x => {
        //                prevValSet = valGetter.TryGetValue(out prevVal);
        //            });

        //            toDispose[1] = notifyForProperty(currentObj, capture.current.Value, false).Subscribe(x => {
        //                subscribeToExpressionChain(origSource, origPath, capture.getter(capture.currentObj), capture.current.Next, capture.currentSub.Next, beforeChange, subject);

        //                TValue newVal;
        //                if (!valGetter.TryGetValue(out newVal)) {
        //                    return;
        //                }
                        
        //                if (prevValSet && EqualityComparer<TValue>.Default.Equals(prevVal, newVal)) {
        //                    return;
        //                }

        //                obsCh = new ObservedChange<TSender, TValue>() {
        //                    Sender = origSource,
        //                    PropertyName = origPath,
        //                    Value = default(TValue),
        //                };

        //                TValue obsChVal;
        //                if (obsCh.TryGetValue(out obsChVal)) {
        //                    obsCh.Value = obsChVal;
        //                    subject.OnNext(obsCh);
        //                }
        //            });

        //            currentSub.Value = Disposable.Create(() => { toDispose[0].Dispose(); toDispose[1].Dispose(); });
        //        }

        //        current = current.Next;
        //        currentSub = currentSub.Next;
        //        currentObj = getter != null ? getter(currentObj) : null;
        //    }

        //    if (currentSub.Value != null) {
        //        currentSub.Value.Dispose();
        //    }

        //    if (currentObj == null) {
        //        return;
        //    }

        //    var propName = current.Value;
        //    var finalGetter = Reflection.GetValueFetcherForProperty(currentObj.GetType(), current.Value);

        //    currentSub.Value = notifyForProperty(currentObj, propName, beforeChange).Subscribe(x => {
        //        obsCh = new ObservedChange<TSender, TValue>() {
        //            Sender = origSource,
        //            PropertyName = origPath,
        //            Value = (TValue)finalGetter(currentObj),
        //        };

        //        subject.OnNext(obsCh);
        //    });
        //}

        static readonly MemoizingMRUCache<Tuple<Type, bool>, ICreatesObservableForProperty> notifyFactoryCache =
            new MemoizingMRUCache<Tuple<Type, bool>, ICreatesObservableForProperty>((t, _) => {
                return RxApp.GetAllServices<ICreatesObservableForProperty>()
                    .Aggregate(Tuple.Create(0, (ICreatesObservableForProperty)null), (acc, x) => {
                        int score = x.GetAffinityForObject(t.Item1, t.Item2);
                        return (score > acc.Item1) ? Tuple.Create(score, x) : acc;
                    }).Item2;
            }, 50);

        static IObservable<IObservedChange<object, object>> notifyForProperty(object sender, string propertyName, bool beforeChange)
        {
            var result = default(ICreatesObservableForProperty);
            lock (notifyFactoryCache) {
                result = notifyFactoryCache.Get(Tuple.Create(sender.GetType(), beforeChange));
            }

            if (result == null) {
                throw new Exception(
                    String.Format("Couldn't find a ICreatesObservableForProperty for {0}. This should never happen, your service locator is probably broken.", 
                    sender.GetType()));
            }
            
            return result.GetNotificationForProperty(sender, propertyName, beforeChange);
        }

        static string buildPropPathFromNodePtr(LinkedListNode<string> node)
        {
            var ret = new StringBuilder();
            var current = node;

            while(current.Next != null) {
                ret.Append(current.Value);
                ret.Append('.');
                current = current.Next;
            }

            ret.Append(current.Value);
            return ret.ToString();
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
            where TSender : class
        {           
            Contract.Requires(selector != null);
            return This.ObservableForProperty(property, beforeChange).Select(x => selector(x.Value));
        }

        /* NOTE: This is left here for reference - the real one is expanded out 
         * to 10 parameters in VariadicTemplates.tt */
#if FALSE
        public static IObservable<TRet> WhenAny<TSender, T1, T2, TRet>(this TSender This, 
                Expression<Func<TSender, T1>> property1, 
                Expression<Func<TSender, T2>> property2,
                Func<IObservedChange<TSender, T1>, IObservedChange<TSender, T2>, TRet> selector)
            where TSender : IReactiveNotifyPropertyChanged
        {
            var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", RxApp.expressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;

            var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", RxApp.expressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;

            IObservedChange<TSender, T1> islot1 = slot1;
            IObservedChange<TSender, T2> islot2 = slot2;
            return Observable.CreateWithDisposable<TRet>(subject => {
                subject.OnNext(selector(slot1, slot2));

                return Observable.Merge(
                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2)),
                    This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2))
                ).Subscribe(subject);
            });
        }
#endif
    }
}

// vim: tw=120 ts=4 sw=4 et :
