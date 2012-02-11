using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NLog;

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
        static readonly Logger log = LogManager.GetCurrentClassLogger();

        readonly Dictionary<Tuple<Type, string>, NotAWeakReference> messageBus =
            new Dictionary<Tuple<Type, string>, NotAWeakReference>();

        readonly IDictionary<Tuple<Type, string>, IScheduler> schedulerMappings =
            new Dictionary<Tuple<Type, string>, IScheduler>();


        /// <summary>
        /// Registers a scheduler for the type, which may be specified at runtime, and the contract.
        /// </summary>
        /// <remarks>If a scheduler is already registered for the specified runtime and contract, this will overrwrite the existing registration.</remarks>
        /// <typeparam name="T">The type of the message to listen to.</typeparam>
        /// <param name="scheduler">The scheduler on which to post the
        /// notifications for the specified type and contract. RxApp.DeferredScheduler by default.</param>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        public void RegisterScheduler<T>(IScheduler scheduler, string contract = null)
        {
            schedulerMappings[new Tuple<Type, string>(typeof (T), contract)] = scheduler;
        }

        /// <summary>
        /// Listen provides an Observable that will fire whenever a Message is
        /// provided for this object via RegisterMessageSource or SendMessage.
        /// </summary>
        /// <typeparam name="T">The type of the message to listen to.</typeparam>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns>An Observable representing the notifications posted to the
        /// message bus.</returns>
        public IObservable<T> Listen<T>(string contract = null)
        {
            log.Info("Listening to {0}:{1}", typeof (T), contract);

            return SetupSubjectIfNecessary<T>(contract);
        }

        /// <summary>
        /// Determines if a particular message Type is registered.
        /// </summary>
        /// <param name="type">The Type of the message to listen to.</param>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns>True if messages have been posted for this message Type.</returns>
        public bool IsRegistered(Type type, string contract = null)
        {
            bool ret = false;
            WithMessageBus(type, contract, (mb, tuple) => { ret = mb.ContainsKey(tuple) && mb[tuple].IsAlive; });

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
        public IDisposable RegisterMessageSource<T>(
            IObservable<T> source,
            string contract = null)
        {
            return source.Subscribe(SetupSubjectIfNecessary<T>(contract));
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
            SetupSubjectIfNecessary<T>(contract).OnNext(message);
        }

        /// <summary>
        /// Returns the Current MessageBus from the RxApp global object.
        /// </summary>
        public static IMessageBus Current
        {
            get { return RxApp.MessageBus; }
        }

        ISubject<T> SetupSubjectIfNecessary<T>(string contract)
        {
            ISubject<T> ret = null;

            WithMessageBus(typeof (T), contract, (mb, tuple) => {
                NotAWeakReference subjRef;
                if (mb.TryGetValue(tuple, out subjRef) && subjRef.IsAlive) {
                    ret = (ISubject<T>)subjRef.Target;
                    return;
                }

                ret = new ScheduledSubject<T>(GetScheduler(tuple));
                mb[tuple] = new NotAWeakReference(ret);
            });

            return ret;
        }

        void WithMessageBus(
            Type type,
            string contract,
            Action<Dictionary<Tuple<Type, string>, NotAWeakReference>,
                Tuple<Type, string>> block)
        {
            lock (messageBus) {
                var tuple = new Tuple<Type, String>(type, contract);
                block(messageBus, tuple);
                if (messageBus.ContainsKey(tuple) && !messageBus[tuple].IsAlive) {
                    messageBus.Remove(tuple);
                }
            }
        }

        IScheduler GetScheduler(Tuple<Type, string> tuple)
        {
            IScheduler scheduler;
            schedulerMappings.TryGetValue(tuple, out scheduler);
            return scheduler ?? RxApp.DeferredScheduler;
        }
    }

    public static class MessageBusMixins
    {
        /// <summary>
        /// Registers a ViewModel object to send property change
        /// messages; this allows a ViewModel to listen to another ViewModel's
        /// changes in a loosely-typed manner.
        /// </summary>
        /// <param name="This">The message bus this extends.</param>
        /// <param name="source">The ViewModel to register</param>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <exception cref="Exception"><c>Exception</c>The registered ViewModel
        /// must be the only instance (i.e. not in an ItemsControl)</exception>
        public static void RegisterViewModel<T>(this IMessageBus This, T source, string contract = null)
            where T : IReactiveNotifyPropertyChanged
        {
            string contractName = ViewModelContractName(typeof (T), contract);
            if (This.IsRegistered(typeof (ObservedChange<T, object>), contractName)) {
                throw new Exception(typeof (T).FullName + " must be a singleton class or have a unique contract name");
            }

            This.RegisterMessageSource(
                (source.Changed)
                    .Select(x => new ObservedChange<T, object> {Sender = source, PropertyName = x.PropertyName}),
                contractName);

            This.RegisterMessageSource(
                Observable.Defer(() => Observable.Return(source)),
                ViewModelCurrentValueContractName(typeof (T), contract));
        }

        /// <summary>
        /// Listens to a registered ViewModel's property change notifications.
        /// </summary>
        /// <param name="This">The message bus this extends</param>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns>An Observable that fires when an object changes and
        /// provides the property name that has changed.</returns>
        public static IObservable<ObservedChange<T, object>> ListenToViewModel<T>(
            this IMessageBus This,
            string contract = null)
        {
            return This.Listen<ObservedChange<T, object>>(ViewModelContractName(typeof (T), contract));
        }

        /// <summary>
        /// Return the current instance of the ViewModel with the specified
        /// type.
        /// </summary>
        /// <param name="This">The message bus this extends</param>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns>The ViewModel object registered for this type.</returns>
        public static T ViewModelForType<T>(this IMessageBus This, string contract = null)
        {
            return This.Listen<T>(ViewModelCurrentValueContractName(typeof (T), contract)).First();
        }

        static string ViewModelContractName(Type type, string contract)
        {
            return type.FullName + "_" + (contract ?? String.Empty);
        }

        static string ViewModelCurrentValueContractName(Type type, string contract)
        {
            return ViewModelContractName(type, contract) + "__current";
        }
    }

    class NotAWeakReference
    {
        public NotAWeakReference(object target)
        {
            Target = target;
        }

        public object Target { get; private set; }

        public bool IsAlive
        {
            get { return true; }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :