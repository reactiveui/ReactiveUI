using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Reactive;
using Splat;
using System.Linq.Expressions;

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
        /// CurrentThreadScheduler by default.</param>
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
        int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false);

        /// <summary>
        /// Subscribe to notifications on the specified property, given an
        /// object and a property name.
        /// </summary>
        /// <param name="sender">The object to observe.</param>
        /// <param name="expression">The expression on the object to observe.
        /// This will be either a MemberExpression or an IndexExpression
        /// dependending on the property.
        /// </param>
        /// <param name="beforeChanged">If true, signal just before the
        /// property value actually changes. If false, signal after the
        /// property changes.</param>
        /// <returns>An IObservable which is signalled whenever the specified
        /// property on the object changes. If this cannot be done for a
        /// specified value of beforeChanged, return Observable.Never</returns>
        IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, bool beforeChanged = false);
    }

    /// <summary>
    /// This interface is the extensible implementation of IValueConverters for
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
        /// <param name="fromType">The source type to convert from</param>
        /// <param name="toType">The target type to convert to</param>
        /// <returns>A positive integer if TryConvert is supported,
        /// zero or a negative value otherwise</returns>
        int GetAffinityForObjects(Type fromType, Type toType);

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
    /// Implement this as a way to intercept bindings at the time that they are
    /// created and execute an additional action (or to cancel the binding)
    /// </summary>
    public interface IPropertyBindingHook
    {
        /// <summary>
        /// Called when any binding is set up.
        /// </summary>
        /// <returns>If false, the binding is cancelled</returns>
        /// <param name="source">The source ViewModel</param>
        /// <param name="target">The target View (not the actual control)</param>
        /// <param name="getCurrentViewModelProperties">Get current view model properties.</param>
        /// <param name="getCurrentViewProperties">Get current view properties.</param>
        /// <param name="direction">The Binding direction.</param>
        bool ExecuteHook(
            object source, object target,
            Func<IObservedChange<object, object>[]> getCurrentViewModelProperties,
            Func<IObservedChange<object, object>[]> getCurrentViewProperties,
            BindingDirection direction);
    }

    /// <summary>
    /// Use this Interface when you want to mark a control as recieving View
    /// Activation when it doesn't have a backing ViewModel.
    /// </summary>
    public interface IActivatable { }

    /// <summary>
    /// This base class is mostly used by the Framework. Implement <see cref="IViewFor{T}"/>
    /// instead.
    /// </summary>
    public interface IViewFor : IActivatable
    {
        /// <summary>
        ///
        /// </summary>
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
        RoutingState Router { get; }
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
        IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName)
#if MONO
            where TEventArgs : EventArgs
#endif
        ;

    }

    /// <summary>
    /// Implement this to override how RoutedViewHost and ViewModelViewHost
    /// map ViewModels to Views.
    /// </summary>
    public interface IViewLocator : IEnableLogger
    {
        /// <summary>
        /// Determines the view for an associated ViewModel
        /// </summary>
        /// <returns>The view, with the ViewModel property assigned to
        /// viewModel.</returns>
        /// <param name="viewModel">View model.</param>
        /// <param name="contract">Contract.</param>
        IViewFor ResolveView<T>(T viewModel, string contract = null) where T : class;
    }

    /// <summary>
    /// Implement this interface to override how ReactiveUI determines when a
    /// View is activated or deactivated. This is usually only used when porting
    /// ReactiveUI to a new UI framework
    /// </summary>
    public interface IActivationForViewFetcher
    {
        int GetAffinityForView(Type view);
        IObservable<bool> GetActivationForView(IActivatable view);
    }

    internal interface IPlatformOperations
    {
        string GetOrientation();
    }

    internal interface IWantsToRegisterStuff
    {
        void Register(Action<Func<object>, Type> registerFunction);
    }

    /* Nicked from http://caliburnmicro.codeplex.com/wikipage?title=Working%20with%20Windows%20Phone%207%20v1.1
     *
     * Launching - Occurs when a fresh instance of the application is launching.
     * Activated - Occurs when a previously paused/tombstoned app is resumed/resurrected.
     * Deactivated - Occurs when the application is being paused or tombstoned.
     * Closing - Occurs when the application is closing.
     * Continuing - Occurs when the app is continuing from a temporarily paused state.
     * Continued - Occurs after the app has continued from a temporarily paused state.
     * Resurrecting - Occurs when the app is "resurrecting" from a tombstoned state.
     * Resurrected - Occurs after the app has "resurrected" from a tombstoned state.
    */

    /// <summary>
    /// ISuspensionHost represents a standardized version of the events that the
    /// host operating system publishes. Subscribe to these events in order to
    /// handle app suspend / resume.
    /// </summary>
    public interface ISuspensionHost : IReactiveObject
    {
        /// <summary>
        /// Signals when the application is launching new. This can happen when
        /// an app has recently crashed, as well as the first time the app has
        /// been launched. Apps should create their state from scratch.
        /// </summary>
        IObservable<Unit> IsLaunchingNew { get; set; }

        /// <summary>
        /// Signals when the application is resuming from suspended state (i.e.
        /// it was previously running but its process was destroyed).
        /// </summary>
        IObservable<Unit> IsResuming { get; set; }

        /// <summary>
        /// Signals when the application is activated. Note that this may mean
        /// that your process was not actively running before this signal.
        /// </summary>
        IObservable<Unit> IsUnpausing { get; set; }

        /// <summary>
        /// Signals when the application should persist its state to disk.
        /// </summary>
        /// <value>Returns an IDisposable that should be disposed once the
        /// application finishes persisting its state</value>
        IObservable<IDisposable> ShouldPersistState { get; set; }

        /// <summary>
        /// Signals that the saved application state should be deleted, this
        /// usually is called after an app has crashed
        /// </summary>
        IObservable<Unit> ShouldInvalidateState { get; set; }

        /// <summary>
        /// A method that can be used to create a new application state - usually
        /// this method just calls 'new' on an object.
        /// </summary>
        Func<object> CreateNewAppState { get; set; }

        /// <summary>
        /// The current application state - get a typed version of this via 
        /// <see cref="SuspensionHostExtensions.GetAppState{T}(ISuspensionHost)"/>.
        /// The "application state" is a notion entirely defined
        /// via the client application - the framework places no restrictions on
        /// the object other than it can be serialized.
        /// </summary>
        object AppState { get; set; }
    }

    /// <summary>
    /// ISuspensionDriver represents a class that can load/save state to persistent
    /// storage. Most platforms have a basic implementation of this class, but you
    /// probably want to write your own.
    /// </summary>
    public interface ISuspensionDriver
    {
        /// <summary>
        /// Loads the application state from persistent storage
        /// </summary>
        IObservable<object> LoadState();

        /// <summary>
        /// Saves the application state to disk.
        /// </summary>
        IObservable<Unit> SaveState(object state);

        /// <summary>
        /// Invalidates the application state (i.e. deletes it from disk)
        /// </summary>
        IObservable<Unit> InvalidateState();
    }
}
