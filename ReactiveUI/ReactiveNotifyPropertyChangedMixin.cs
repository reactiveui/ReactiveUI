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
                bool beforeChange = false,
                bool skipInitial = true)
        {

            if (This == null) {
                throw new ArgumentNullException("Sender");
            }

            var propertyNames = Reflection.ExpressionToPropertyNames(property);

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

            return SubscribeToExpressionChain<TSender, TValue>(
                This,
                propertyNames,
                beforeChange,
                skipInitial 
                );
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
                bool beforeChange = false,
                bool skipInitial = true)
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

            return SubscribeToExpressionChain<TSender, object>(
                This,
                propertyNames,
                beforeChange,
                skipInitial 
                );

        }


        private static IObservedChange<object, object> 
            observedChangeFor(string propertyName, IObservedChange<object, object> y)
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

        private static IObservable<IObservedChange<object, object>> 
            nestedObservedChanges(string propertyName, IObservedChange<object, object> sourceChange)
        {
            // Make sure a change at a root node propogates events down
            var kicker = observedChangeFor(propertyName, sourceChange);

            // Handle null values in the chain
            if (sourceChange.Value == null)
                return Observable.Empty<IObservedChange<object,object>>();

            // Handle non null values in the chain
            return notifyForProperty(sourceChange.Value, propertyName, false)
                .Select(x => x.fillInValue())
                .StartWith(kicker);
        }

        public static IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TSender, TValue> ( 
                this TSender source,
                IEnumerable<string> propertyNames, 
                bool beforeChange = false,
                bool skipInitial = true
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

            var r = notifier.Select(x => x.fillInValue())
                .Select(x => new ObservedChange<TSender, TValue>()
                {
                    Sender = source
                ,
                    PropertyName = path
                ,
                    Value = (TValue)x.Value
                });

            if (skipInitial)
                r = r.Skip(1);

            return r.DistinctUntilChanged(x=>x.Value);
        }




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
