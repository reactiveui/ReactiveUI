using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace ReactiveUI
{
    /// <summary>
    /// MessageBus represents an object that can act as a "Message Bus", a
    /// simple way for ViewModels and other objects to communicate with each
    /// other in a loosely coupled way.
    ///
    /// Specifying which messages go where is done via a combination of the Type
    /// of the message as well as an additional "Contract" parameter; this is a
    /// unique string used to distinguish between messages of the same Type, and
    /// is arbitrarily set by the client. 
    /// </summary>
    public class MessageBus : IMessageBus 
    {
        Dictionary<Tuple<Type, string>, WeakReference> messageBus = 
            new Dictionary<Tuple<Type,string>,WeakReference>();

        /// <summary>
        /// Listen provides an Observable that will fire whenever a Message is
        /// provided for this object via RegisterMessageSource or SendMessage.
        /// </summary>
        /// <typeparam name="T">The type of the message to listen to.</typeparam>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns></returns>
        public IObservable<T> Listen<T>(string contract = null)
        {
            IObservable<T> ret = null;
	        this.Log().InfoFormat("Listening to {0}:{1}", typeof(T), contract);

            ret = setupSubjectIfNecessary<T>(contract);
            return ret;
        }

        /// <summary>
        /// Determins if a particular message Type is registered.
        /// </summary>
        /// <param name="type">The Type of the message to listen to.</param>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns>True if messages have been posted for this message Type.</returns>
        public bool IsRegistered(Type type, string contract = null)
        {
            bool ret = false;
            withMessageBus(type, contract, (mb, tuple) => {
                ret = mb.ContainsKey(tuple) && mb[tuple].IsAlive;
            });

            return ret;
        }

        /// <summary>
        /// Registers an Observable representing the stream of messages to send.
        /// Another part of the code can then call Listen to retrieve this
        /// Observable.
        /// </summary>
        /// <typeparam name="T">The type of the message to listen to.</typeparam>
        /// <param name="source">An Observable that will be subscribed to, and a
        /// message sent out for each value provided.</param>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        public IDisposable RegisterMessageSource<T>(IObservable<T> source, string contract = null)
        {
            return source.Subscribe(setupSubjectIfNecessary<T>(contract));
        }

        /// <summary>
        /// Sends a single message using the specified Type and contract.
        /// Consider using RegisterMessageSource instead if you will be sending
        /// messages in response to other changes such as property changes
        /// or events.
        /// </summary>
        /// <typeparam name="T">The type of the message to send.</typeparam>
        /// <param name="message">The actual message to send</param>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        public void SendMessage<T>(T message, string contract = null)
        {
            setupSubjectIfNecessary<T>(contract).OnNext(message);
        }

        /// <summary>
        /// Returns the Current MessageBus from the RxApp global object.
        /// </summary>
        public static IMessageBus Current {
            get { return RxApp.MessageBus; }
        }

        Subject<T> setupSubjectIfNecessary<T>(string contract)
        {
            Subject<T> ret = null;
            WeakReference subjRef = null;

            withMessageBus(typeof(T), contract, (mb, tuple) => {
                if (mb.TryGetValue(tuple, out subjRef) && subjRef.IsAlive) {
                    ret = (Subject<T>) subjRef.Target;
                    return;
                }

                ret = new Subject<T>();
                mb[tuple] = new WeakReference(ret);
            });

            return ret;
        }

        void withMessageBus(
            Type Type, 
            string Contract, 
            Action<Dictionary<Tuple<Type, string>, WeakReference>, 
            Tuple<Type, string>> block)
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
        /// Registers a ViewModel object to send property change
        /// messages; this allows a ViewModel to listen to another ViewModel's
        /// changes in a loosely-typed manner.
        /// </summary>
        /// <param name="source">The ViewModel to register</param>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <exception cref="Exception"><c>Exception</c>The registered ViewModel
        /// must be the only instance (i.e. not in an ItemsControl)</exception>
        public static void RegisterViewModel<T>(this IMessageBus This, T source, string contract = null)
            where T : IReactiveNotifyPropertyChanged
        {
            string contractName = viewModelContractName(typeof(T), contract);
            if (This.IsRegistered(typeof(ObservedChange<T, object>), contractName)) {
                throw new Exception(typeof(T).FullName + " must be a singleton class or have a unique contract name");
            }

            This.RegisterMessageSource(
                (source as IObservable<PropertyChangedEventArgs>)
                    .Select(x => new ObservedChange<T, object>() { Sender = source, PropertyName = x.PropertyName }), 
                contractName);

            This.RegisterMessageSource(
                 Observable.Defer(() => Observable.Return(source)), 
                 viewModelCurrentValueContractName(typeof(T), contract));
        }

        /// <summary>
        /// Listens to a registered ViewModel's property change notifications.
        /// </summary>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns>An Observable that fires when an object changes and
        /// provides the property name that has changed.</returns>
        public static IObservable<ObservedChange<T, object>> ListenToViewModel<T>(
            this IMessageBus This, 
            string contract = null)
        {
            return This.Listen<ObservedChange<T, object>>(viewModelContractName(typeof(T), contract));
        }

        /// <summary>
        /// Return the current instance of the ViewModel with the specified
        /// type.
        /// </summary>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns>The ViewModel object registered for this type.</returns>
        public static T ViewModelForType<T>(this IMessageBus This, string contract = null)
        {
            return This.Listen<T>(viewModelCurrentValueContractName(typeof(T), contract)).First();
        }

        static string viewModelContractName(Type type, string contract)
        {
            return type.FullName + "_" + (contract ?? String.Empty);
        }

        static string viewModelCurrentValueContractName(Type type, string contract)
        {
            return viewModelContractName(type, contract) + "__current";
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
