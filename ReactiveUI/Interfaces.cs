using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive;
using System.Windows.Input;

namespace ReactiveUI
{
    /// <summary>
    /// IObservedChange is a generic interface that replaces the non-generic
    /// PropertyChangedEventArgs. Note that it is used for both Changing (i.e.
    /// 'before change') and Changed Observables. In the future, this interface
    /// will be Covariant which will allow simpler casting between specific and
    /// generic changes.
    /// </summary>
    public interface IObservedChange<out TSender, out TValue>
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

    /// <summary>
    /// A data-only version of IObservedChange
    /// </summary>
    public class ObservedChange<TSender, TValue> : IObservedChange<TSender, TValue>
    {
        public TSender Sender { get; set; }
        public string PropertyName { get; set; }
        public TValue Value { get; set; }
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
    /// IReactiveCommand represents an ICommand which also notifies when it is
    /// executed (i.e. when Execute is called) via IObservable. Conceptually,
    /// this represents an Event, so as a result this IObservable should never
    /// OnComplete or OnError.
    /// 
    /// In previous versions of ReactiveUI, this interface was split into two
    /// separate interfaces, one to handle async methods and one for "standard"
    /// commands, but these have now been merged - every ReactiveCommand is now
    /// a ReactiveAsyncCommand.
    /// </summary>
    public interface IReactiveCommand : IHandleObservableErrors, IObservable<object>, ICommand, IDisposable, IEnableLogger
    {
        /// <summary>
        /// Registers an asynchronous method to be called whenever the command
        /// is Executed. This method returns an IObservable representing the
        /// asynchronous operation, and is allowed to OnError / should OnComplete.
        /// </summary>
        /// <returns>A filtered version of the Observable which is marshaled 
        /// to the UI thread. This Observable should only report successes and
        /// instead send OnError messages to the ThrownExceptions property.
        /// </returns>
        /// <param name="asyncBlock">The asynchronous method to call.</param>
        IObservable<T> RegisterAsync<T>(Func<object, IObservable<T>> asyncBlock);

        /// <summary>
        /// Gets a value indicating whether this instance can execute observable.
        /// </summary>
        /// <value><c>true</c> if this instance can execute observable; otherwise, <c>false</c>.</value>
        IObservable<bool> CanExecuteObservable { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is executing. This 
        /// Observable is guaranteed to always return a value immediately (i.e.
        /// it is backed by a BehaviorSubject), meaning it is safe to determine
        /// the current state of the command via IsExecuting.First()
        /// </summary>
        /// <value><c>true</c> if this instance is executing; otherwise, <c>false</c>.</value>
        IObservable<bool> IsExecuting { get; }

        /// <summary>
        /// Gets a value indicating whether this 
        /// <see cref="ReactiveUI.IReactiveCommand"/> allows concurrent 
        /// execution. If false, the CanExecute of the command will be disabled
        /// while async operations are currently in-flight.
        /// </summary>
        /// <value><c>true</c> if allows concurrent execution; otherwise, <c>false</c>.</value>
        bool AllowsConcurrentExecution { get; }
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
    public interface IReactiveNotifyPropertyChanged<out TSender> : IReactiveNotifyPropertyChanged
    {
        new IObservable<IObservedChange<TSender, object>> Changing { get; }
        new IObservable<IObservedChange<TSender, object>> Changed { get; }
    }

    /// <summary>
    /// IReactiveNotifyCollectionItemChanged provides notifications for collection item updates, ie when an object in
    /// a collection changes.
    /// </summary>
    public interface IReactiveNotifyCollectionItemChanged
    {
        /// <summary>
        /// Provides Item Changing notifications for any item in collection that
        /// implements IReactiveNotifyPropertyChanged. This is only enabled when
        /// ChangeTrackingEnabled is set to True.
        /// </summary>
        IObservable<IObservedChange<object, object>> ItemChanging { get; }

        /// <summary>
        /// Provides Item Changed notifications for any item in collection that
        /// implements IReactiveNotifyPropertyChanged. This is only enabled when
        /// ChangeTrackingEnabled is set to True.
        /// </summary>
        IObservable<IObservedChange<object, object>> ItemChanged { get; }

        /// <summary>
        /// Enables the ItemChanging and ItemChanged properties; when this is
        /// enabled, whenever a property on any object implementing
        /// IReactiveNotifyPropertyChanged changes, the change will be
        /// rebroadcast through ItemChanging/ItemChanged.
        /// </summary>
        bool ChangeTrackingEnabled { get; set; }
    }

    /// <summary>
    /// IReactiveNotifyCollectionItemChanged of T is the typed version of IReactiveNotifyCollectionItemChanged and
    /// adds type-specified versions of Observables
    /// </summary>
    public interface IReactiveNotifyCollectionItemChanged<out T> : IReactiveNotifyCollectionItemChanged
    {
        /// <summary>
        /// Provides Item Changing notifications for any item in collection that
        /// implements IReactiveNotifyPropertyChanged. This is only enabled when
        /// ChangeTrackingEnabled is set to True.
        /// </summary>
        new IObservable<IObservedChange<T, object>> ItemChanging { get; }

        /// <summary>
        /// Provides Item Changed notifications for any item in collection that
        /// implements IReactiveNotifyPropertyChanged. This is only enabled when
        /// ChangeTrackingEnabled is set to True.
        /// </summary>
        new IObservable<IObservedChange<T, object>> ItemChanged { get; }
    }

    /// <summary>
    /// IReactiveCollection provides notifications when the contents
    /// of collection are changed (items are added/removed/moved).
    /// </summary>
    public interface IReactiveNotifyCollectionChanged : INotifyCollectionChanged
    {
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
        /// Fires before an items moves from one position in the collection to
        /// another, providing the item(s) to be moved as well as source and destination
        /// indices.
        /// </summary>
        IObservable<IMoveInfo<object>> BeforeItemsMoved { get; }

        /// <summary>
        /// Fires once one or more items moves from one position in the collection to
        /// another, providing the item(s) that was moved as well as source and destination
        /// indices.
        /// </summary>
        IObservable<IMoveInfo<object>> ItemsMoved { get; }

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
    /// IReactiveNotifyCollectionChanged of T is the typed version of IReactiveNotifyCollectionChanged and
    /// adds type-specified versions of Observables
    /// </summary>
    public interface IReactiveNotifyCollectionChanged<out T> : IReactiveNotifyCollectionChanged
    {
        /// <summary>
        /// Fires when items are added to the collection, once per item added.
        /// Functions that add multiple items such AddRange should fire this
        /// multiple times. The object provided is the item that was added.
        /// </summary>
        new IObservable<T> ItemsAdded { get; }

        /// <summary>
        /// Fires before an item is going to be added to the collection.
        /// </summary>
        new IObservable<T> BeforeItemsAdded { get; }

        /// <summary>
        /// Fires once an item has been removed from a collection, providing the
        /// item that was removed.
        /// </summary>
        new IObservable<T> ItemsRemoved { get; }

        /// <summary>
        /// Fires before an item will be removed from a collection, providing
        /// the item that will be removed. 
        /// </summary>
        new IObservable<T> BeforeItemsRemoved { get; }

        /// <summary>
        /// Fires before an items moves from one position in the collection to
        /// another, providing the item(s) to be moved as well as source and destination
        /// indices.
        /// </summary>
        new IObservable<IMoveInfo<T>> BeforeItemsMoved { get; }

        /// <summary>
        /// Fires once one or more items moves from one position in the collection to
        /// another, providing the item(s) that was moved as well as source and destination
        /// indices.
        /// </summary>
        new IObservable<IMoveInfo<T>> ItemsMoved { get; }
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
    public interface IReactiveCollection : IReactiveNotifyCollectionChanged, IReactiveNotifyCollectionItemChanged, INotifyPropertyChanging, INotifyPropertyChanged, IEnableLogger, IEnumerable
    {
    }

    /// <summary>
    /// IReactiveCollection of T is the typed version of IReactiveCollection and
    /// adds type-specified versions of Observables
    /// </summary>
    public interface IReactiveCollection<T> : IReactiveCollection, ICollection<T>, IReactiveNotifyCollectionChanged<T>, IReactiveNotifyCollectionItemChanged<T>
    {
    }

    /// <summary>
    /// IReadOnlyReactiveCollection represents a read-only collection that can notify when its
    /// contents are changed (either items are added/removed, or the object
    /// itself changes).
    ///
    /// It is important to implement the Changing/Changed from
    /// IReactiveNotifyPropertyChanged semantically as "Fire when *anything* in
    /// the collection or any of its items have changed, in any way".
    /// </summary>
    public interface IReadOnlyReactiveCollection<out T> : IReadOnlyCollection<T>, IReactiveNotifyCollectionChanged<T>, IReactiveNotifyCollectionItemChanged<T>, INotifyPropertyChanging, INotifyPropertyChanged, IEnableLogger
    {
    }

    /// <summary>
    /// IReactiveList represents a read-only list that can notify when its
    /// contents are changed (either items are added/removed, or the object
    /// itself changes).
    ///
    /// It is important to implement the Changing/Changed from
    /// IReactiveNotifyPropertyChanged semantically as "Fire when *anything* in
    /// the collection or any of its items have changed, in any way".
    /// </summary>
    public interface IReadOnlyReactiveList<out T> : IReadOnlyReactiveCollection<T>, IReadOnlyList<T>
    {
    }

    /// <summary>
    /// IReactiveDerivedList repreents a collection whose contents will "follow" another
    /// collection; this method is useful for creating ViewModel collections
    /// that are automatically updated when the respective Model collection is updated.
    /// </summary>
    public interface IReactiveDerivedList<T> : IReadOnlyReactiveList<T>, IDisposable
    {
    }

    /// <summary>
    /// IReactiveList represents a list that can notify when its
    /// contents are changed (either items are added/removed, or the object
    /// itself changes).
    ///
    /// It is important to implement the Changing/Changed from
    /// IReactiveNotifyPropertyChanged semantically as "Fire when *anything* in
    /// the collection or any of its items have changed, in any way".
    /// </summary>
    public interface IReactiveList<T> : IReactiveCollection<T>, IList<T>
    {
    }

    // NB: This is just a name we can bolt extension methods to
    public interface INavigateCommand : IReactiveCommand { }

    public interface IRoutingState : IReactiveNotifyPropertyChanged
    {
        /// <summary>
        /// Represents the current navigation stack, the last element in the
        /// collection being the currently visible ViewModel.
        /// </summary>
        ReactiveList<IRoutableViewModel> NavigationStack { get; }

        /// <summary>
        /// Navigates back to the previous element in the stack.
        /// </summary>
        IReactiveCommand NavigateBack { get; }

        /// <summary>
        /// Navigates to the a new element in the stack - the Execute parameter
        /// must be a ViewModel that implements IRoutableViewModel.
        /// </summary>
        INavigateCommand Navigate { get; }

        /// <summary>
        /// Navigates to a new element and resets the navigation stack (i.e. the
        /// new ViewModel will now be the only element in the stack) - the
        /// Execute parameter must be a ViewModel that implements
        /// IRoutableViewModel.
        /// </summary>
        INavigateCommand NavigateAndReset { get; }

        IObservable<IRoutableViewModel> CurrentViewModel { get; }
    }

    /// <summary>
    /// Implement this interface for ViewModels that can be navigated to.
    /// </summary>
    public interface IRoutableViewModel : IReactiveNotifyPropertyChanged
    {
        /// <summary>
        /// A string token representing the current ViewModel, such as 'login' or 'user'
        /// </summary>
        string UrlPathSegment { get; }

        /// <summary>
        /// The IScreen that this ViewModel is currently being shown in. This
        /// is usually passed into the ViewModel in the Constructor and saved
        /// as a ReadOnly Property.
        /// </summary>
        IScreen HostScreen { get; }
    }

    /// <summary>
    /// Allows an additional string to make view resolution more specific than just a type.
    /// </summary>
    public class ViewContractAttribute : Attribute
    {
        /// <summary>
        /// A unique string that will be used along with the type to resolve a View
        /// </summary>
        public string Contract { get; set; }
    }
}

// vim: tw=120 ts=4 sw=4 et :
