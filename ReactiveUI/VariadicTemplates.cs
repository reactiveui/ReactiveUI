
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
						var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property1),
            };
            object slot1Value = default(object); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, object> islot1 = slot1;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, T2> islot2 = slot2;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property1),
            };
            object slot1Value = default(object); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, object> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property2),
            };
            object slot2Value = default(object); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, object> islot2 = slot2;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, T2> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, T3>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property3)),
            };
            T3 slot3Value = default(T3); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, T3> islot3 = slot3;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property1),
            };
            object slot1Value = default(object); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, object> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property2),
            };
            object slot2Value = default(object); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, object> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property3),
            };
            object slot3Value = default(object); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, object> islot3 = slot3;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, T2> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, T3>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property3)),
            };
            T3 slot3Value = default(T3); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, T3> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, T4>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property4)),
            };
            T4 slot4Value = default(T4); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, T4> islot4 = slot4;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property1),
            };
            object slot1Value = default(object); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, object> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property2),
            };
            object slot2Value = default(object); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, object> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property3),
            };
            object slot3Value = default(object); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, object> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property4),
            };
            object slot4Value = default(object); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, object> islot4 = slot4;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, T2> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, T3>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property3)),
            };
            T3 slot3Value = default(T3); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, T3> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, T4>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property4)),
            };
            T4 slot4Value = default(T4); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, T4> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, T5>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property5)),
            };
            T5 slot5Value = default(T5); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, T5> islot5 = slot5;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property1),
            };
            object slot1Value = default(object); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, object> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property2),
            };
            object slot2Value = default(object); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, object> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property3),
            };
            object slot3Value = default(object); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, object> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property4),
            };
            object slot4Value = default(object); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, object> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property5),
            };
            object slot5Value = default(object); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, object> islot5 = slot5;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, T2> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, T3>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property3)),
            };
            T3 slot3Value = default(T3); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, T3> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, T4>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property4)),
            };
            T4 slot4Value = default(T4); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, T4> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, T5>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property5)),
            };
            T5 slot5Value = default(T5); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, T5> islot5 = slot5;
						var slot6 = new ObservedChange<TSender, T6>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property6)),
            };
            T6 slot6Value = default(T6); slot6.TryGetValue(out slot6Value); slot6.Value = slot6Value;
            IObservedChange<TSender, T6> islot6 = slot6;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5, islot6));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6)),                     This.ObservableForProperty(property6).Do(x => { lock (slot6) { islot6 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property1),
            };
            object slot1Value = default(object); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, object> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property2),
            };
            object slot2Value = default(object); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, object> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property3),
            };
            object slot3Value = default(object); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, object> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property4),
            };
            object slot4Value = default(object); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, object> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property5),
            };
            object slot5Value = default(object); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, object> islot5 = slot5;
						var slot6 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property6),
            };
            object slot6Value = default(object); slot6.TryGetValue(out slot6Value); slot6.Value = slot6Value;
            IObservedChange<TSender, object> islot6 = slot6;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5, islot6));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6)),                     This.ObservableForProperty(property6).Do(x => { lock (slot6) { islot6 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, T2> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, T3>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property3)),
            };
            T3 slot3Value = default(T3); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, T3> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, T4>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property4)),
            };
            T4 slot4Value = default(T4); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, T4> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, T5>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property5)),
            };
            T5 slot5Value = default(T5); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, T5> islot5 = slot5;
						var slot6 = new ObservedChange<TSender, T6>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property6)),
            };
            T6 slot6Value = default(T6); slot6.TryGetValue(out slot6Value); slot6.Value = slot6Value;
            IObservedChange<TSender, T6> islot6 = slot6;
						var slot7 = new ObservedChange<TSender, T7>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property7)),
            };
            T7 slot7Value = default(T7); slot7.TryGetValue(out slot7Value); slot7.Value = slot7Value;
            IObservedChange<TSender, T7> islot7 = slot7;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7)),                     This.ObservableForProperty(property6).Do(x => { lock (slot6) { islot6 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7)),                     This.ObservableForProperty(property7).Do(x => { lock (slot7) { islot7 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property1),
            };
            object slot1Value = default(object); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, object> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property2),
            };
            object slot2Value = default(object); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, object> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property3),
            };
            object slot3Value = default(object); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, object> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property4),
            };
            object slot4Value = default(object); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, object> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property5),
            };
            object slot5Value = default(object); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, object> islot5 = slot5;
						var slot6 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property6),
            };
            object slot6Value = default(object); slot6.TryGetValue(out slot6Value); slot6.Value = slot6Value;
            IObservedChange<TSender, object> islot6 = slot6;
						var slot7 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property7),
            };
            object slot7Value = default(object); slot7.TryGetValue(out slot7Value); slot7.Value = slot7Value;
            IObservedChange<TSender, object> islot7 = slot7;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7)),                     This.ObservableForProperty(property6).Do(x => { lock (slot6) { islot6 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7)),                     This.ObservableForProperty(property7).Do(x => { lock (slot7) { islot7 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, T2> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, T3>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property3)),
            };
            T3 slot3Value = default(T3); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, T3> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, T4>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property4)),
            };
            T4 slot4Value = default(T4); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, T4> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, T5>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property5)),
            };
            T5 slot5Value = default(T5); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, T5> islot5 = slot5;
						var slot6 = new ObservedChange<TSender, T6>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property6)),
            };
            T6 slot6Value = default(T6); slot6.TryGetValue(out slot6Value); slot6.Value = slot6Value;
            IObservedChange<TSender, T6> islot6 = slot6;
						var slot7 = new ObservedChange<TSender, T7>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property7)),
            };
            T7 slot7Value = default(T7); slot7.TryGetValue(out slot7Value); slot7.Value = slot7Value;
            IObservedChange<TSender, T7> islot7 = slot7;
						var slot8 = new ObservedChange<TSender, T8>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property8)),
            };
            T8 slot8Value = default(T8); slot8.TryGetValue(out slot8Value); slot8.Value = slot8Value;
            IObservedChange<TSender, T8> islot8 = slot8;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8)),                     This.ObservableForProperty(property6).Do(x => { lock (slot6) { islot6 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8)),                     This.ObservableForProperty(property7).Do(x => { lock (slot7) { islot7 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8)),                     This.ObservableForProperty(property8).Do(x => { lock (slot8) { islot8 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property1),
            };
            object slot1Value = default(object); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, object> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property2),
            };
            object slot2Value = default(object); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, object> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property3),
            };
            object slot3Value = default(object); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, object> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property4),
            };
            object slot4Value = default(object); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, object> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property5),
            };
            object slot5Value = default(object); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, object> islot5 = slot5;
						var slot6 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property6),
            };
            object slot6Value = default(object); slot6.TryGetValue(out slot6Value); slot6.Value = slot6Value;
            IObservedChange<TSender, object> islot6 = slot6;
						var slot7 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property7),
            };
            object slot7Value = default(object); slot7.TryGetValue(out slot7Value); slot7.Value = slot7Value;
            IObservedChange<TSender, object> islot7 = slot7;
						var slot8 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property8),
            };
            object slot8Value = default(object); slot8.TryGetValue(out slot8Value); slot8.Value = slot8Value;
            IObservedChange<TSender, object> islot8 = slot8;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8)),                     This.ObservableForProperty(property6).Do(x => { lock (slot6) { islot6 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8)),                     This.ObservableForProperty(property7).Do(x => { lock (slot7) { islot7 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8)),                     This.ObservableForProperty(property8).Do(x => { lock (slot8) { islot8 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, T2> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, T3>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property3)),
            };
            T3 slot3Value = default(T3); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, T3> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, T4>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property4)),
            };
            T4 slot4Value = default(T4); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, T4> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, T5>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property5)),
            };
            T5 slot5Value = default(T5); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, T5> islot5 = slot5;
						var slot6 = new ObservedChange<TSender, T6>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property6)),
            };
            T6 slot6Value = default(T6); slot6.TryGetValue(out slot6Value); slot6.Value = slot6Value;
            IObservedChange<TSender, T6> islot6 = slot6;
						var slot7 = new ObservedChange<TSender, T7>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property7)),
            };
            T7 slot7Value = default(T7); slot7.TryGetValue(out slot7Value); slot7.Value = slot7Value;
            IObservedChange<TSender, T7> islot7 = slot7;
						var slot8 = new ObservedChange<TSender, T8>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property8)),
            };
            T8 slot8Value = default(T8); slot8.TryGetValue(out slot8Value); slot8.Value = slot8Value;
            IObservedChange<TSender, T8> islot8 = slot8;
						var slot9 = new ObservedChange<TSender, T9>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property9)),
            };
            T9 slot9Value = default(T9); slot9.TryGetValue(out slot9Value); slot9.Value = slot9Value;
            IObservedChange<TSender, T9> islot9 = slot9;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property6).Do(x => { lock (slot6) { islot6 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property7).Do(x => { lock (slot7) { islot7 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property8).Do(x => { lock (slot8) { islot8 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property9).Do(x => { lock (slot9) { islot9 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property1),
            };
            object slot1Value = default(object); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, object> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property2),
            };
            object slot2Value = default(object); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, object> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property3),
            };
            object slot3Value = default(object); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, object> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property4),
            };
            object slot4Value = default(object); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, object> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property5),
            };
            object slot5Value = default(object); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, object> islot5 = slot5;
						var slot6 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property6),
            };
            object slot6Value = default(object); slot6.TryGetValue(out slot6Value); slot6.Value = slot6Value;
            IObservedChange<TSender, object> islot6 = slot6;
						var slot7 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property7),
            };
            object slot7Value = default(object); slot7.TryGetValue(out slot7Value); slot7.Value = slot7Value;
            IObservedChange<TSender, object> islot7 = slot7;
						var slot8 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property8),
            };
            object slot8Value = default(object); slot8.TryGetValue(out slot8Value); slot8.Value = slot8Value;
            IObservedChange<TSender, object> islot8 = slot8;
						var slot9 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property9),
            };
            object slot9Value = default(object); slot9.TryGetValue(out slot9Value); slot9.Value = slot9Value;
            IObservedChange<TSender, object> islot9 = slot9;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property6).Do(x => { lock (slot6) { islot6 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property7).Do(x => { lock (slot7) { islot7 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property8).Do(x => { lock (slot8) { islot8 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9)),                     This.ObservableForProperty(property9).Do(x => { lock (slot9) { islot9 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, T2> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, T3>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property3)),
            };
            T3 slot3Value = default(T3); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, T3> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, T4>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property4)),
            };
            T4 slot4Value = default(T4); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, T4> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, T5>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property5)),
            };
            T5 slot5Value = default(T5); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, T5> islot5 = slot5;
						var slot6 = new ObservedChange<TSender, T6>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property6)),
            };
            T6 slot6Value = default(T6); slot6.TryGetValue(out slot6Value); slot6.Value = slot6Value;
            IObservedChange<TSender, T6> islot6 = slot6;
						var slot7 = new ObservedChange<TSender, T7>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property7)),
            };
            T7 slot7Value = default(T7); slot7.TryGetValue(out slot7Value); slot7.Value = slot7Value;
            IObservedChange<TSender, T7> islot7 = slot7;
						var slot8 = new ObservedChange<TSender, T8>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property8)),
            };
            T8 slot8Value = default(T8); slot8.TryGetValue(out slot8Value); slot8.Value = slot8Value;
            IObservedChange<TSender, T8> islot8 = slot8;
						var slot9 = new ObservedChange<TSender, T9>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property9)),
            };
            T9 slot9Value = default(T9); slot9.TryGetValue(out slot9Value); slot9.Value = slot9Value;
            IObservedChange<TSender, T9> islot9 = slot9;
						var slot10 = new ObservedChange<TSender, T10>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property10)),
            };
            T10 slot10Value = default(T10); slot10.TryGetValue(out slot10Value); slot10.Value = slot10Value;
            IObservedChange<TSender, T10> islot10 = slot10;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property6).Do(x => { lock (slot6) { islot6 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property7).Do(x => { lock (slot7) { islot7 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property8).Do(x => { lock (slot8) { islot8 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property9).Do(x => { lock (slot9) { islot9 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property10).Do(x => { lock (slot10) { islot10 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property1),
            };
            object slot1Value = default(object); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, object> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property2),
            };
            object slot2Value = default(object); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, object> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property3),
            };
            object slot3Value = default(object); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, object> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property4),
            };
            object slot4Value = default(object); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, object> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property5),
            };
            object slot5Value = default(object); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, object> islot5 = slot5;
						var slot6 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property6),
            };
            object slot6Value = default(object); slot6.TryGetValue(out slot6Value); slot6.Value = slot6Value;
            IObservedChange<TSender, object> islot6 = slot6;
						var slot7 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property7),
            };
            object slot7Value = default(object); slot7.TryGetValue(out slot7Value); slot7.Value = slot7Value;
            IObservedChange<TSender, object> islot7 = slot7;
						var slot8 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property8),
            };
            object slot8Value = default(object); slot8.TryGetValue(out slot8Value); slot8.Value = slot8Value;
            IObservedChange<TSender, object> islot8 = slot8;
						var slot9 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property9),
            };
            object slot9Value = default(object); slot9.TryGetValue(out slot9Value); slot9.Value = slot9Value;
            IObservedChange<TSender, object> islot9 = slot9;
						var slot10 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property10),
            };
            object slot10Value = default(object); slot10.TryGetValue(out slot10Value); slot10.Value = slot10Value;
            IObservedChange<TSender, object> islot10 = slot10;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property6).Do(x => { lock (slot6) { islot6 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property7).Do(x => { lock (slot7) { islot7 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property8).Do(x => { lock (slot8) { islot8 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property9).Do(x => { lock (slot9) { islot9 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10)),                     This.ObservableForProperty(property10).Do(x => { lock (slot10) { islot10 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, T2> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, T3>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property3)),
            };
            T3 slot3Value = default(T3); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, T3> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, T4>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property4)),
            };
            T4 slot4Value = default(T4); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, T4> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, T5>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property5)),
            };
            T5 slot5Value = default(T5); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, T5> islot5 = slot5;
						var slot6 = new ObservedChange<TSender, T6>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property6)),
            };
            T6 slot6Value = default(T6); slot6.TryGetValue(out slot6Value); slot6.Value = slot6Value;
            IObservedChange<TSender, T6> islot6 = slot6;
						var slot7 = new ObservedChange<TSender, T7>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property7)),
            };
            T7 slot7Value = default(T7); slot7.TryGetValue(out slot7Value); slot7.Value = slot7Value;
            IObservedChange<TSender, T7> islot7 = slot7;
						var slot8 = new ObservedChange<TSender, T8>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property8)),
            };
            T8 slot8Value = default(T8); slot8.TryGetValue(out slot8Value); slot8.Value = slot8Value;
            IObservedChange<TSender, T8> islot8 = slot8;
						var slot9 = new ObservedChange<TSender, T9>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property9)),
            };
            T9 slot9Value = default(T9); slot9.TryGetValue(out slot9Value); slot9.Value = slot9Value;
            IObservedChange<TSender, T9> islot9 = slot9;
						var slot10 = new ObservedChange<TSender, T10>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property10)),
            };
            T10 slot10Value = default(T10); slot10.TryGetValue(out slot10Value); slot10.Value = slot10Value;
            IObservedChange<TSender, T10> islot10 = slot10;
						var slot11 = new ObservedChange<TSender, T11>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property11)),
            };
            T11 slot11Value = default(T11); slot11.TryGetValue(out slot11Value); slot11.Value = slot11Value;
            IObservedChange<TSender, T11> islot11 = slot11;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property6).Do(x => { lock (slot6) { islot6 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property7).Do(x => { lock (slot7) { islot7 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property8).Do(x => { lock (slot8) { islot8 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property9).Do(x => { lock (slot9) { islot9 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property10).Do(x => { lock (slot10) { islot10 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property11).Do(x => { lock (slot11) { islot11 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property1),
            };
            object slot1Value = default(object); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, object> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property2),
            };
            object slot2Value = default(object); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, object> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property3),
            };
            object slot3Value = default(object); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, object> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property4),
            };
            object slot4Value = default(object); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, object> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property5),
            };
            object slot5Value = default(object); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, object> islot5 = slot5;
						var slot6 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property6),
            };
            object slot6Value = default(object); slot6.TryGetValue(out slot6Value); slot6.Value = slot6Value;
            IObservedChange<TSender, object> islot6 = slot6;
						var slot7 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property7),
            };
            object slot7Value = default(object); slot7.TryGetValue(out slot7Value); slot7.Value = slot7Value;
            IObservedChange<TSender, object> islot7 = slot7;
						var slot8 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property8),
            };
            object slot8Value = default(object); slot8.TryGetValue(out slot8Value); slot8.Value = slot8Value;
            IObservedChange<TSender, object> islot8 = slot8;
						var slot9 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property9),
            };
            object slot9Value = default(object); slot9.TryGetValue(out slot9Value); slot9.Value = slot9Value;
            IObservedChange<TSender, object> islot9 = slot9;
						var slot10 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property10),
            };
            object slot10Value = default(object); slot10.TryGetValue(out slot10Value); slot10.Value = slot10Value;
            IObservedChange<TSender, object> islot10 = slot10;
						var slot11 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property11),
            };
            object slot11Value = default(object); slot11.TryGetValue(out slot11Value); slot11.Value = slot11Value;
            IObservedChange<TSender, object> islot11 = slot11;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property6).Do(x => { lock (slot6) { islot6 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property7).Do(x => { lock (slot7) { islot7 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property8).Do(x => { lock (slot8) { islot8 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property9).Do(x => { lock (slot9) { islot9 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property10).Do(x => { lock (slot10) { islot10 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11)),                     This.ObservableForProperty(property11).Do(x => { lock (slot11) { islot11 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, T2> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, T3>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property3)),
            };
            T3 slot3Value = default(T3); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, T3> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, T4>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property4)),
            };
            T4 slot4Value = default(T4); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, T4> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, T5>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property5)),
            };
            T5 slot5Value = default(T5); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, T5> islot5 = slot5;
						var slot6 = new ObservedChange<TSender, T6>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property6)),
            };
            T6 slot6Value = default(T6); slot6.TryGetValue(out slot6Value); slot6.Value = slot6Value;
            IObservedChange<TSender, T6> islot6 = slot6;
						var slot7 = new ObservedChange<TSender, T7>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property7)),
            };
            T7 slot7Value = default(T7); slot7.TryGetValue(out slot7Value); slot7.Value = slot7Value;
            IObservedChange<TSender, T7> islot7 = slot7;
						var slot8 = new ObservedChange<TSender, T8>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property8)),
            };
            T8 slot8Value = default(T8); slot8.TryGetValue(out slot8Value); slot8.Value = slot8Value;
            IObservedChange<TSender, T8> islot8 = slot8;
						var slot9 = new ObservedChange<TSender, T9>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property9)),
            };
            T9 slot9Value = default(T9); slot9.TryGetValue(out slot9Value); slot9.Value = slot9Value;
            IObservedChange<TSender, T9> islot9 = slot9;
						var slot10 = new ObservedChange<TSender, T10>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property10)),
            };
            T10 slot10Value = default(T10); slot10.TryGetValue(out slot10Value); slot10.Value = slot10Value;
            IObservedChange<TSender, T10> islot10 = slot10;
						var slot11 = new ObservedChange<TSender, T11>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property11)),
            };
            T11 slot11Value = default(T11); slot11.TryGetValue(out slot11Value); slot11.Value = slot11Value;
            IObservedChange<TSender, T11> islot11 = slot11;
						var slot12 = new ObservedChange<TSender, T12>() {
                Sender = This,
                PropertyName = String.Join(".", Reflection.ExpressionToPropertyNames(property12)),
            };
            T12 slot12Value = default(T12); slot12.TryGetValue(out slot12Value); slot12.Value = slot12Value;
            IObservedChange<TSender, T12> islot12 = slot12;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property6).Do(x => { lock (slot6) { islot6 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property7).Do(x => { lock (slot7) { islot7 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property8).Do(x => { lock (slot8) { islot8 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property9).Do(x => { lock (slot9) { islot9 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property10).Do(x => { lock (slot10) { islot10 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property11).Do(x => { lock (slot11) { islot11 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property12).Do(x => { lock (slot12) { islot12 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12))                 ).Subscribe(subject);
            });
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
						var slot1 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property1),
            };
            object slot1Value = default(object); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, object> islot1 = slot1;
						var slot2 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property2),
            };
            object slot2Value = default(object); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, object> islot2 = slot2;
						var slot3 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property3),
            };
            object slot3Value = default(object); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, object> islot3 = slot3;
						var slot4 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property4),
            };
            object slot4Value = default(object); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, object> islot4 = slot4;
						var slot5 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property5),
            };
            object slot5Value = default(object); slot5.TryGetValue(out slot5Value); slot5.Value = slot5Value;
            IObservedChange<TSender, object> islot5 = slot5;
						var slot6 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property6),
            };
            object slot6Value = default(object); slot6.TryGetValue(out slot6Value); slot6.Value = slot6Value;
            IObservedChange<TSender, object> islot6 = slot6;
						var slot7 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property7),
            };
            object slot7Value = default(object); slot7.TryGetValue(out slot7Value); slot7.Value = slot7Value;
            IObservedChange<TSender, object> islot7 = slot7;
						var slot8 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property8),
            };
            object slot8Value = default(object); slot8.TryGetValue(out slot8Value); slot8.Value = slot8Value;
            IObservedChange<TSender, object> islot8 = slot8;
						var slot9 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property9),
            };
            object slot9Value = default(object); slot9.TryGetValue(out slot9Value); slot9.Value = slot9Value;
            IObservedChange<TSender, object> islot9 = slot9;
						var slot10 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property10),
            };
            object slot10Value = default(object); slot10.TryGetValue(out slot10Value); slot10.Value = slot10Value;
            IObservedChange<TSender, object> islot10 = slot10;
						var slot11 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property11),
            };
            object slot11Value = default(object); slot11.TryGetValue(out slot11Value); slot11.Value = slot11Value;
            IObservedChange<TSender, object> islot11 = slot11;
						var slot12 = new ObservedChange<TSender, object>() {
                Sender = This,
                PropertyName = String.Join(".", property12),
            };
            object slot12Value = default(object); slot12.TryGetValue(out slot12Value); slot12.Value = slot12Value;
            IObservedChange<TSender, object> islot12 = slot12;
			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12));

                return Observable.Merge(                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property5).Do(x => { lock (slot5) { islot5 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property6).Do(x => { lock (slot6) { islot6 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property7).Do(x => { lock (slot7) { islot7 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property8).Do(x => { lock (slot8) { islot8 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property9).Do(x => { lock (slot9) { islot9 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property10).Do(x => { lock (slot10) { islot10 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property11).Do(x => { lock (slot11) { islot11 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12)),                     This.ObservableForProperty(property12).Do(x => { lock (slot12) { islot12 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4, islot5, islot6, islot7, islot8, islot9, islot10, islot11, islot12))                 ).Subscribe(subject);
            });
        }

		}
}
