using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;

namespace ReactiveUI
{
    /// <summary>
    /// IObservedChange is a generic interface that replaces the non-generic
    /// PropertyChangedEventArgs. Note that it is used for both Changing (i.e.
    /// 'before change') and Changed Observables. In the future, this interface
    /// will be Covariant which will allow simpler casting between specific and
    /// generic changes.
    /// </summary>
    public interface IObservedChange<TSender, TValue>
    {
        /// <summary>
        /// The object that has raised the change.
        /// </summary>
        TSender Sender { get; }

        /// <summary>
        /// The name of the property that has changed on Sender.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// The value of the property that has changed. IMPORTANT NOTE: This
        /// property is often not set for performance reasons, unless you have
        /// explicitly requested an Observable for a property via a method such
        /// as ObservableForProperty. To retrieve the value for the property,
        /// use the Value() extension method.
        /// </summary>
        TValue Value { get; }
    }

    public class ObservedChange<TSender, TValue> : IObservedChange<TSender, TValue>
    {
        public TSender Sender { get; set; }
        public string PropertyName { get; set; }
        public TValue Value { get; set; }
    }

    /// <summary>
    /// IReactiveNotifyPropertyChanged represents an extended version of
    /// INotifyPropertyChanged that also exposes Observables.
    /// </summary>
    public interface IReactiveNotifyPropertyChanged : INotifyPropertyChanged, INotifyPropertyChanging, IEnableLogger
    {
        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed. Note that this should not fire duplicate change notifications if a
        /// property is set to the same value multiple times.
        /// </summary>
        IObservable<IObservedChange<object, object>> Changing { get; }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// Note that this should not fire duplicate change notifications if a
        /// property is set to the same value multiple times.
        /// </summary>
        IObservable<IObservedChange<object, object>> Changed { get; }

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change
        /// notifications.</returns>
        IDisposable SuppressChangeNotifications();
    }

    /// <summary>
    /// IReactiveNotifyPropertyChanged of TSender is a helper interface that adds
    /// typed versions of Changing and Changed.
    /// </summary>
    public interface IReactiveNotifyPropertyChanged<TSender> : IReactiveNotifyPropertyChanged
    {
        new IObservable<IObservedChange<TSender, object>> Changing { get; }
        new IObservable<IObservedChange<TSender, object>> Changed { get; }
    }

    /// <summary>
    /// This interface is implemented by RxUI objects which are given 
    /// IObservables as input - when the input IObservables OnError, instead of 
    /// disabling the RxUI object, we catch the IObservable and pipe it into
    /// this property.
    /// 
    /// Normally this IObservable is implemented with a ScheduledSubject whose 
    /// default Observer is RxApp.DefaultExceptionHandler - this means, that if
    /// you aren't listening to ThrownExceptions and one appears, the exception
    /// will appear on the UI thread and crash the application.
    /// </summary>
    public interface IHandleObservableErrors
    {
        /// <summary>
        /// Fires whenever an exception would normally terminate ReactiveUI 
        /// internal state.
        /// </summary>
        IObservable<Exception> ThrownExceptions { get; }
    }

    /// <summary>
    /// IReactiveCollection represents a collection that can notify when its
    /// contents are changed (either items are added/removed, or the object
    /// itself changes).
    ///
    /// It is important to implement the Changing/Changed from
    /// IReactiveNotifyPropertyChanged semantically as "Fire when *anything* in
    /// the collection or any of its items have changed, in any way".
    /// </summary>
    public interface IReactiveCollection : IEnumerable, INotifyCollectionChanged
    {
        //
        // Collection Tracking
        //

        /// <summary>
        /// Fires when items are added to the collection, once per item added.
        /// Functions that add multiple items such AddRange should fire this
        /// multiple times. The object provided is the item that was added.
        /// </summary>
        IObservable<object> ItemsAdded { get; }

        /// <summary>
        /// Fires before an item is going to be added to the collection.
        /// </summary>
        IObservable<object> BeforeItemsAdded { get; }

        /// <summary>
        /// Fires once an item has been removed from a collection, providing the
        /// item that was removed.
        /// </summary>
        IObservable<object> ItemsRemoved { get; }

        /// <summary>
        /// Fires before an item will be removed from a collection, providing
        /// the item that will be removed. 
        /// </summary>
        IObservable<object> BeforeItemsRemoved { get; }

        /// <summary>
        /// Fires whenever the number of items in a collection has changed,
        /// providing the new Count.
        /// </summary>
        IObservable<int> CollectionCountChanged { get; }

        /// <summary>
        /// Fires before a collection is about to change, providing the previous
        /// Count.
        /// </summary>
        IObservable<int> CollectionCountChanging { get; }

        /// <summary>
        /// Fires when a collection becomes or stops being empty.
        /// </summary>
        IObservable<bool> IsEmpty { get; }

        //
        // Change Tracking
        //

        /// <summary>
        /// Provides Item Changed notifications for any item in collection that
        /// implements IReactiveNotifyPropertyChanged. This is only enabled when
        /// ChangeTrackingEnabled is set to True.
        /// </summary>
        IObservable<IObservedChange<object, object>> ItemChanging { get; }

        /// <summary>
        /// Provides Item Changing notifications for any item in collection that
        /// implements IReactiveNotifyPropertyChanged. This is only enabled when
        /// </summary>
        IObservable<IObservedChange<object, object>> ItemChanged { get; }

        /// <summary>
        /// Enables the ItemChanging and ItemChanged properties; when this is
        /// enabled, whenever a property on any object implementing
        /// IReactiveNotifyPropertyChanged changes, the change will be
        /// rebroadcast through ItemChanging/ItemChanged.
        /// </summary>
        bool ChangeTrackingEnabled { get; set; }

        /// <summary>
        /// This Observable is equivalent to the NotifyCollectionChanged event,
        /// but fires before the collection is changed
        /// </summary>
        IObservable<NotifyCollectionChangedEventArgs> Changing { get; }

        /// <summary>
        /// This Observable is equivalent to the NotifyCollectionChanged event,
        /// and fires after the collection is changed
        /// </summary>
        IObservable<NotifyCollectionChangedEventArgs> Changed { get; }

        /// <summary>
        /// This Observable is fired when a ShouldReset fires on the collection. This
        /// means that you should forget your previous knowledge of the state
        /// of the collection and reread it.
        /// 
        /// This does *not* mean Clear, and if you interpret it as such, you are
        /// Doing It Wrong.
        /// </summary>
        IObservable<Unit> ShouldReset { get; }
    }

    /// <summary>
    /// IReactiveCollection of T is the typed version of IReactiveCollection and
    /// adds type-specified versions of Observables
    /// </summary>
    public interface IReactiveCollection<T> : IEnumerable<T>, IReactiveCollection
    {
        new IObservable<T> ItemsAdded { get; }

        new IObservable<T> BeforeItemsAdded { get; }

        new IObservable<T> ItemsRemoved { get; }

        new IObservable<T> BeforeItemsRemoved { get; }

        IObservable<IObservedChange<T, object>> ItemChanging { get; }

        IObservable<IObservedChange<T, object>> ItemChanged { get; }
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
        /// RxApp.DeferredScheduler by default.</param>
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

    public enum BindingDirection
    {
        OneWay,
        TwoWay,
        AsyncOneWay,
    }

    public interface IPropertyBindingHook
    {
        bool ExecuteHook(object source, object target, Func<IObservedChange<object, object>[]> getCurrentViewModelProperties, Func<IObservedChange<object, object>[]> getCurrentViewProperties, BindingDirection direction);
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
        T ViewModel { get; set; }
    }

    internal interface IWantsToRegisterStuff
    {                       
        void Register();
    }
}

// vim: tw=120 ts=4 sw=4 et :