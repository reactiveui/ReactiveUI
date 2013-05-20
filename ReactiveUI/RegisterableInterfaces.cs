using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReactiveUI
{
    public enum BindingDirection
    {
        OneWay,
        TwoWay,
        AsyncOneWay,
    }

    /// <summary>
    /// IMessageBus represents an object that can act as a "Message Bus", a
    /// simple way for ViewModels and other objects to communicate with each
    /// other in a loosely coupled way.
    ///
    /// Specifying which messages go where is done via a combination of the Type
    /// of the message as well as an additional "Contract" parameter; this is a
    /// unique string used to distinguish between messages of the same Type, and
    /// is arbitrarily set by the client. 
    /// </summary>
    public interface IMessageBus : IEnableLogger
    {
        /// <summary>
        /// Registers a scheduler for the type, which may be specified at
        /// runtime, and the contract.
        /// </summary>
        /// <remarks>If a scheduler is already registered for the specified
        /// runtime and contract, this will overrwrite the existing
        /// registration.</remarks>
        /// <typeparam name="T">The type of the message to listen to.</typeparam>
        /// <param name="scheduler">The scheduler on which to post the
        /// notifications for the specified type and contract.
        /// RxApp.MainThreadScheduler by default.</param>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        void RegisterScheduler<T>(IScheduler scheduler, string contract = null);

        /// <summary>
        /// Listen provides an Observable that will fire whenever a Message is
        /// provided for this object via RegisterMessageSource or SendMessage.
        /// </summary>
        /// <typeparam name="T">The type of the message to listen to.</typeparam>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns></returns>
        IObservable<T> Listen<T>(string contract = null);

        /// <summary>
        /// ListenIncludeLatest provides an Observable that will fire whenever a Message is
        /// provided for this object via RegisterMessageSource or SendMessage and fire the 
        /// last provided Message immediately if applicable, or null.
        /// </summary>
        /// <typeparam name="T">The type of the message to listen to.</typeparam>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns>An Observable representing the notifications posted to the
        /// message bus.</returns>
        IObservable<T> ListenIncludeLatest<T>(string contract = null);

        /// <summary>
        /// Determines if a particular message Type is registered.
        /// </summary>
        /// <param name="type">The type of the message.</param>
        /// <param name="contract">A unique string to distinguish messages with
        /// identical types (i.e. "MyCoolViewModel") - if the message type is
        /// only used for one purpose, leave this as null.</param>
        /// <returns>True if messages have been posted for this message Type.</returns>
        bool IsRegistered(Type type, string contract = null);

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
        IDisposable RegisterMessageSource<T>(IObservable<T> source, string contract = null);

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
        void SendMessage<T>(T message, string contract = null);
    }

    /// <summary>
    /// ICreatesObservableForProperty represents an object that knows how to
    /// create notifications for a given type of object. Implement this if you
    /// are porting RxUI to a new UI toolkit, or generally want to enable WhenAny
    /// for another type of object that can be observed in a unique way.
    /// </summary>
    public interface ICreatesObservableForProperty : IEnableLogger
    {
        /// <summary>
        /// Returns a positive integer when this class supports 
        /// GetNotificationForProperty for this particular Type. If the method
        /// isn't supported at all, return a non-positive integer. When multiple
        /// implementations return a positive value, the host will use the one
        /// which returns the highest value. When in doubt, return '2' or '0'
        /// </summary>
        /// <param name="type">The type to query for.</param>
        /// <returns>A positive integer if GNFP is supported, zero or a negative
        /// value otherwise</returns>
        int GetAffinityForObject(Type type, bool beforeChanged = false);

        /// <summary>
        /// Subscribe to notifications on the specified property, given an 
        /// object and a property name.
        /// </summary>
        /// <param name="sender">The object to observe.</param>
        /// <param name="propertyName">The property on the object to observe. 
        /// This property will not be a dotted property, only a simple name.
        /// </param>
        /// <param name="beforeChanged">If true, signal just before the 
        /// property value actually changes. If false, signal after the 
        /// property changes.</param>
        /// <returns>An IObservable which is signalled whenever the specified
        /// property on the object changes. If this cannot be done for a 
        /// specified value of beforeChanged, return Observable.Never</returns>
        IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, string propertyName, bool beforeChanged = false);
    }

    /// <summary>
    /// This class is the extensible implementation of IValueConverters for 
    /// Bind and OneWayBind. Implement this to teach Bind and OneWayBind how to
    /// convert between types.
    /// </summary>
    public interface IBindingTypeConverter : IEnableLogger
    {
        /// <summary>
        /// Returns a positive integer when this class supports 
        /// TryConvert for this particular Type. If the method isn't supported at 
        /// all, return a non-positive integer. When multiple implementations 
        /// return a positive value, the host will use the one which returns 
        /// the highest value. When in doubt, return '2' or '0'.
        /// </summary>
        /// <param name="lhs">The left-hand object to compare (i.e. 'from')</param>
        /// <param name="rhs">The right-hand object to compare (i.e. 'to')</param>
        /// <returns>A positive integer if TryConvert is supported, 
        /// zero or a negative value otherwise</returns>
        int GetAffinityForObjects(Type lhs, Type rhs);

        /// <summary>
        /// Convert a given object to the specified type.
        /// </summary>
        /// <param name="from">The object to convert.</param>
        /// <param name="toType">The type to coerce the object to.</param>
        /// <param name="conversionHint">An implementation-defined value, 
        /// usually to specify things like locale awareness.</param>
        /// <returns>An object that is of the type 'to'</returns>
        bool TryConvert(object from, Type toType, object conversionHint, out object result);
    }

    /// <summary>
    /// Implement this to teach Bind and OneWayBind how to guess the most 
    /// "common" property on a given control, so if the caller doesn't specify it,
    /// it'll pick the right control
    /// </summary>
    public interface IDefaultPropertyBindingProvider
    {
        /// <summary>
        /// Given a certain control, figure out the default property to bind to
        /// </summary>
        /// <param name="control">The control to look at.</param>
        /// <returns>A tuple of PropertyName and Affinity for that property.
        /// Use the same rules about affinity as others, but return null if
        /// the property can't be determined.</returns>
        Tuple<string, int> GetPropertyForControl(object control);
    }

    public interface IPropertyBindingHook
    {
        bool ExecuteHook(
            object source, object target, 
            Func<IObservedChange<object, object>[]> getCurrentViewModelProperties, 
            Func<IObservedChange<object, object>[]> getCurrentViewProperties, 
            BindingDirection direction);
    }

    public interface IViewFor
    {
        object ViewModel { get; set; }
    }

    /// <summary>
    /// Implement this interface on your Views to support Routing and Binding.
    /// </summary>
    public interface IViewFor<T> : IViewFor
        where T : class
    {
        /// <summary>
        /// The ViewModel corresponding to this specific View. This should be
        /// a DependencyProperty if you're using XAML.
        /// </summary>
        new T ViewModel { get; set; }
    }

    /// <summary>
    /// IScreen represents any object that is hosting its own routing -
    /// usually this object is your AppViewModel or MainWindow object.
    /// </summary>
    public interface IScreen
    {
        /// <summary>
        /// The Router associated with this Screen.
        /// </summary>
        IRoutingState Router { get; }
    }

    public interface ICreatesCommandBinding
    {
        /// <summary>
        /// Returns a positive integer when this class supports 
        /// BindCommandToObject for this particular Type. If the method
        /// isn't supported at all, return a non-positive integer. When multiple
        /// implementations return a positive value, the host will use the one
        /// which returns the highest value. When in doubt, return '2' or '0'
        /// </summary>
        /// <param name="type">The type to query for.</param>
        /// <param name="hasEventTarget">If true, the host intends to use a custom
        /// event target.</param>
        /// <returns>A positive integer if BCTO is supported, zero or a negative
        /// value otherwise</returns>
        int GetAffinityForObject(Type type, bool hasEventTarget);

        /// <summary>
        /// Bind an ICommand to a UI object, in the "default" way. The meaning 
        /// of this is dependent on the implementation. Implement this if you
        /// have a new type of UI control that doesn't have 
        /// Command/CommandParameter like WPF or has a non-standard event name
        /// for "Invoke".
        /// </summary>
        /// <param name="command">The command to bind</param>
        /// <param name="target">The target object, usually a UI control of 
        /// some kind</param>
        /// <param name="commandParameter">An IObservable source whose latest 
        /// value will be passed as the command parameter to the command. Hosts
        /// will always pass a valid IObservable, but this may be 
        /// Observable.Empty</param>
        /// <returns>An IDisposable which will disconnect the binding when 
        /// disposed.</returns>
        IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter);

        /// <summary>
        /// Bind an ICommand to a UI object to a specific event. This event may
        /// be a standard .NET event, or it could be an event derived in another
        /// manner (i.e. in MonoTouch).
        /// </summary>
        /// <param name="command">The command to bind</param>
        /// <param name="target">The target object, usually a UI control of 
        /// some kind</param>
        /// <param name="commandParameter">An IObservable source whose latest 
        /// value will be passed as the command parameter to the command. Hosts
        /// will always pass a valid IObservable, but this may be 
        /// Observable.Empty</param>
        /// <param name="eventName">The event to bind to.</param>
        /// <returns></returns>
        /// <returns>An IDisposable which will disconnect the binding when 
        /// disposed.</returns>
        IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName);
    }

    internal interface IPlatformOperations
    {
        string GetOrientation();
    }

    internal interface IWantsToRegisterStuff
    {
        void Register(Action<Func<object>, Type> registerFunction);
    }
}
