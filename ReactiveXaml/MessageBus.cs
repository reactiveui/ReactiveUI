using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace ReactiveXaml
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageBus : IMessageBus 
    {
        Dictionary<Tuple<Type, string>, WeakReference> messageBus = 
            new Dictionary<Tuple<Type,string>,WeakReference>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Contract"></param>
        /// <returns></returns>
        public IObservable<T> Listen<T>(string Contract = null)
        {
            IObservable<T> ret = null;
	        this.Log().InfoFormat("Listening to {0}:{1}", typeof(T), Contract);

            withMessageBus(typeof(T), Contract, (mb, tuple) => {
                ret = (IObservable<T>)mb[tuple].Target;
            });

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Contract"></param>
        /// <returns></returns>
        public bool IsRegistered(Type Type, string Contract = null)
        {
            bool ret = false;
            withMessageBus(Type, Contract, (mb, tuple) => {
                ret = mb.ContainsKey(tuple) && mb[tuple].IsAlive;
            });

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Contract"></param>
        public void RegisterMessageSource<T>(IObservable<T> Source, string Contract = null)
        {
            withMessageBus(typeof(T), Contract, (mb, tuple) => {
                mb[tuple] = new WeakReference(Source);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Message"></param>
        /// <param name="Contract"></param>
        public void SendMessage<T>(T Message, string Contract = null)
        {
            withMessageBus(typeof(T), Contract, (mb, tuple) => {
                var subj = mb[tuple].Target as Subject<T>;

                if (subj == null) {
                    subj = new Subject<T>();
                    IObservable<T> prev = mb[tuple].Target as IObservable<T>;
                    if (prev != null) {
                        prev.Subscribe(subj.OnNext, subj.OnError, subj.OnCompleted);
                    }
                    
                    mb[tuple] = new WeakReference(subj);
                }

	    	this.Log().DebugFormat("Sending message to {0}:{1} - {2}", typeof(T), Contract, Message);
                subj.OnNext(Message);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public static IMessageBus Current {
            get { return RxApp.MessageBus; }
        }

        void withMessageBus(Type Type, string Contract, Action<Dictionary<Tuple<Type, string>, WeakReference>, Tuple<Type, string>> block)
        {
            lock(messageBus) {
                var tuple = new Tuple<Type, String>(Type, Contract);
                block(messageBus, tuple);
                if (messageBus.ContainsKey(tuple) && !messageBus[tuple].IsAlive) {
                    messageBus.Remove(tuple);
                }
            }
        }
    }

    public static class MessageBusMixins
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="This"></param>
        /// <param name="source"></param>
        /// <param name="Contract"></param>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public static void RegisterViewModel<T>(this IMessageBus This, T source, string Contract = null)
            where T : IReactiveNotifyPropertyChanged
        {
            string contractName = viewModelContractName(typeof(T), Contract);
            if (This.IsRegistered(typeof(ObservedChange<T, Unit>), contractName)) {
                throw new Exception(typeof(T).FullName + " must be a singleton class or have a unique contract name");
            }

            This.RegisterMessageSource(
                (source as IObservable<PropertyChangedEventArgs>).Select(x => new ObservedChange<T, Unit>() { Sender = source, PropertyName = x.PropertyName }), 
                contractName);

            This.RegisterMessageSource(
                 Observable.Defer(() => Observable.Return(source)), viewModelCurrentValueContractName(typeof(T), Contract));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="This"></param>
        /// <param name="contract"></param>
        /// <returns></returns>
        public static IObservable<ObservedChange<T, Unit>> ListenToViewModel<T>(this IMessageBus This, string contract = null)
        {
            return This.Listen<ObservedChange<T, Unit>>(viewModelContractName(typeof(T), contract));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="This"></param>
        /// <param name="contract"></param>
        /// <returns></returns>
        public static T ViewModelForType<T>(this IMessageBus This, string contract = null)
        {
            return This.Listen<T>(viewModelCurrentValueContractName(typeof(T), contract)).First();
        }

        static string viewModelContractName(Type Type, string contract)
        {
            return Type.FullName + "_" + (contract ?? String.Empty);
        }

        static string viewModelCurrentValueContractName(Type Type, string contract)
        {
            return viewModelContractName(Type, contract) + "__current";
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
