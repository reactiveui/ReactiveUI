
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Diagnostics.Contracts;
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
            where TSender : IReactiveNotifyPropertyChanged
        {
			
			var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", RxApp.expressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;

			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1));

                return Observable.Merge(
                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1)) 
                ).Subscribe(subject);
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
            where TSender : IReactiveNotifyPropertyChanged
        {
			
			var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", RxApp.expressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;

			
			var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", RxApp.expressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, T2> islot2 = slot2;

			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2));

                return Observable.Merge(
                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2)), 
                    This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2)) 
                ).Subscribe(subject);
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
            where TSender : IReactiveNotifyPropertyChanged
        {
			
			var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", RxApp.expressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;

			
			var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", RxApp.expressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, T2> islot2 = slot2;

			
			var slot3 = new ObservedChange<TSender, T3>() {
                Sender = This,
                PropertyName = String.Join(".", RxApp.expressionToPropertyNames(property3)),
            };
            T3 slot3Value = default(T3); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, T3> islot3 = slot3;

			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3));

                return Observable.Merge(
                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3)), 
                    This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3)), 
                    This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3)) 
                ).Subscribe(subject);
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
            where TSender : IReactiveNotifyPropertyChanged
        {
			
			var slot1 = new ObservedChange<TSender, T1>() {
                Sender = This,
                PropertyName = String.Join(".", RxApp.expressionToPropertyNames(property1)),
            };
            T1 slot1Value = default(T1); slot1.TryGetValue(out slot1Value); slot1.Value = slot1Value;
            IObservedChange<TSender, T1> islot1 = slot1;

			
			var slot2 = new ObservedChange<TSender, T2>() {
                Sender = This,
                PropertyName = String.Join(".", RxApp.expressionToPropertyNames(property2)),
            };
            T2 slot2Value = default(T2); slot2.TryGetValue(out slot2Value); slot2.Value = slot2Value;
            IObservedChange<TSender, T2> islot2 = slot2;

			
			var slot3 = new ObservedChange<TSender, T3>() {
                Sender = This,
                PropertyName = String.Join(".", RxApp.expressionToPropertyNames(property3)),
            };
            T3 slot3Value = default(T3); slot3.TryGetValue(out slot3Value); slot3.Value = slot3Value;
            IObservedChange<TSender, T3> islot3 = slot3;

			
			var slot4 = new ObservedChange<TSender, T4>() {
                Sender = This,
                PropertyName = String.Join(".", RxApp.expressionToPropertyNames(property4)),
            };
            T4 slot4Value = default(T4); slot4.TryGetValue(out slot4Value); slot4.Value = slot4Value;
            IObservedChange<TSender, T4> islot4 = slot4;

			
            return Observable.Create<TRet>(subject => {
                subject.OnNext(selector(islot1, islot2, islot3, islot4));

                return Observable.Merge(
                    This.ObservableForProperty(property1).Do(x => { lock (slot1) { islot1 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4)), 
                    This.ObservableForProperty(property2).Do(x => { lock (slot2) { islot2 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4)), 
                    This.ObservableForProperty(property3).Do(x => { lock (slot3) { islot3 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4)), 
                    This.ObservableForProperty(property4).Do(x => { lock (slot4) { islot4 = x.fillInValue(); } }).Select(x => selector(islot1, islot2, islot3, islot4)) 
                ).Subscribe(subject);
            });
        }

		}
}
