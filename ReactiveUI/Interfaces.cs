﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
using System.Windows.Input;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// IObservedChange is a generic interface that is returned from WhenAny()
    /// Note that it is used for both Changing (i.e.'before change') 
    /// and Changed Observables.
    /// </summary>
    public interface IObservedChange<out TSender, out TValue>
    {
        /// <summary>
        /// The object that has raised the change.
        /// </summary>
        TSender Sender { get; }

        /// <summary>
        /// The expression of the member that has changed on Sender.
        /// </summary>
        Expression Expression { get; }

        /// <summary>
        /// The value of the property that has changed. IMPORTANT NOTE: This
        /// property is often not set for performance reasons, unless you have
        /// explicitly requested an Observable for a property via a method such
        /// as ObservableForProperty. To retrieve the value for the property,
        /// use the GetValue() extension method.
        /// </summary>
        TValue Value { get; }
    }

    /// <summary>
    /// A data-only version of IObservedChange
    /// </summary>
    public class ObservedChange<TSender, TValue> : IObservedChange<TSender, TValue>
    {
        private bool hasValue;
        private TValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservedChange{TSender, TValue}"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="expression">Expression describing the member.</param>
        /// <param name="value">The value.</param>
        public ObservedChange(TSender sender, Expression expression)
        {
            this.Sender = sender;
            this.Expression = expression;
            this.hasValue = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservedChange{TSender, TValue}"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="expression">Expression describing the member.</param>
        /// <param name="value">The value.</param>
        public ObservedChange(TSender sender, Expression expression, TValue value)
        {
            this.Sender = sender;
            this.Expression = expression;
            this.value = value;
            this.hasValue = true;
        }

        public TSender Sender { get; private set; }

        public Expression Expression { get; private set; }

        public TValue Value
        {
            get
            {
                if (!hasValue) {
                    hasValue = this.TryGetValue(out value);
                }
                return value;
            }
        }
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

    public interface IReactiveCommand : IHandleObservableErrors, ICommand, IDisposable, IEnableLogger
    {
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
    public interface IReactiveCommand<T> : IObservable<T>, IReactiveCommand
    {
        IObservable<T> ExecuteAsync(object parameter = null);
    }

    /// <summary>
    /// IReactivePropertyChangedEventArgs is a generic interface that 
    /// is used to wrap the NotifyPropertyChangedEventArgs and gives 
    /// information about changed properties. It includes also 
    /// the sender of the notification.
    /// Note that it is used for both Changing (i.e.'before change') 
    /// and Changed Observables.
    /// </summary>
    public interface IReactivePropertyChangedEventArgs<out TSender>
    {
        /// <summary>
        /// The name of the property that has changed on Sender.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// The object that has raised the change.
        /// </summary>
        TSender Sender { get; }

        /// <summary>
        /// Gets the value that is changed.
        /// </summary>
        object Value { get; }
    }

    public class ReactivePropertyChangingEventArgs<TSender> : PropertyChangingEventArgs, IReactivePropertyChangedEventArgs<TSender>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePropertyChangingEventArgs{TSender}"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        public ReactivePropertyChangingEventArgs(TSender sender, object value, string propertyName)
            : base(propertyName)
        {
            this.Sender = sender;
            this.Value = value;
        }

        public TSender Sender { get; private set; }

        public object Value { get; private set; }
    }

    public class ReactivePropertyChangedEventArgs<TSender> : PropertyChangedEventArgs, IReactivePropertyChangedEventArgs<TSender>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePropertyChangedEventArgs{TSender}"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        public ReactivePropertyChangedEventArgs(TSender sender, object value, string propertyName)
            : base(propertyName)
        {
            this.Sender = sender;
            this.Value = value;
        }

        public TSender Sender { get; private set; }

        public object Value { get; private set; }
    }

    /// <summary>
    /// IReactiveNotifyPropertyChanged represents an extended version of
    /// INotifyPropertyChanged that also exposes typed Observables.
    /// </summary>
    public interface IReactiveNotifyPropertyChanged<out TSender>
    {
        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed. Note that this should not fire duplicate change notifications if a
        /// property is set to the same value multiple times.
        /// </summary>
        IObservable<IReactivePropertyChangedEventArgs<TSender>> Changing { get; }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// Note that this should not fire duplicate change notifications if a
        /// property is set to the same value multiple times.
        /// </summary>
        IObservable<IReactivePropertyChangedEventArgs<TSender>> Changed { get; }

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
    /// IReactiveNotifyCollectionItemChanged provides notifications for collection item updates, ie when an object in
    /// a collection changes.
    /// </summary>
    public interface IReactiveNotifyCollectionItemChanged<out TSender>
    {
        /// <summary>
        /// Provides Item Changing notifications for any item in collection that
        /// implements IReactiveNotifyPropertyChanged. This is only enabled when
        /// ChangeTrackingEnabled is set to True.
        /// </summary>
        IObservable<IReactivePropertyChangedEventArgs<TSender>> ItemChanging { get; }

        /// <summary>
        /// Provides Item Changed notifications for any item in collection that
        /// implements IReactiveNotifyPropertyChanged. This is only enabled when
        /// ChangeTrackingEnabled is set to True.
        /// </summary>
        IObservable<IReactivePropertyChangedEventArgs<TSender>> ItemChanged { get; }

        /// <summary>
        /// Enables the ItemChanging and ItemChanged properties; when this is
        /// enabled, whenever a property on any object implementing
        /// IReactiveNotifyPropertyChanged changes, the change will be
        /// rebroadcast through ItemChanging/ItemChanged.
        /// </summary>
        bool ChangeTrackingEnabled { get; set; }
    }

    /// <summary>
    /// IReactiveNotifyCollectionChanged of T provides notifications when the contents
    /// of collection are changed (items are added/removed/moved).
    /// </summary>
    public interface IReactiveNotifyCollectionChanged<out T>
    {
        /// <summary>
        /// Fires when items are added to the collection, once per item added.
        /// Functions that add multiple items such AddRange should fire this
        /// multiple times. The object provided is the item that was added.
        /// </summary>
        IObservable<T> ItemsAdded { get; }

        /// <summary>
        /// Fires before an item is going to be added to the collection.
        /// </summary>
        IObservable<T> BeforeItemsAdded { get; }

        /// <summary>
        /// Fires once an item has been removed from a collection, providing the
        /// item that was removed.
        /// </summary>
        IObservable<T> ItemsRemoved { get; }

        /// <summary>
        /// Fires before an item will be removed from a collection, providing
        /// the item that will be removed. 
        /// </summary>
        IObservable<T> BeforeItemsRemoved { get; }

        /// <summary>
        /// Fires before an items moves from one position in the collection to
        /// another, providing the item(s) to be moved as well as source and destination
        /// indices.
        /// </summary>
        IObservable<IMoveInfo<T>> BeforeItemsMoved { get; }

        /// <summary>
        /// Fires once one or more items moves from one position in the collection to
        /// another, providing the item(s) that was moved as well as source and destination
        /// indices.
        /// </summary>
        IObservable<IMoveInfo<T>> ItemsMoved { get; }

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
        /// Fires when the collection count changes, regardless of reason
        /// </summary>
        IObservable<int> CountChanging { get; }

        /// <summary>
        /// Fires when the collection count changes, regardless of reason
        /// </summary>
        IObservable<int> CountChanged { get; }

        IObservable<bool> IsEmptyChanged { get; }

        /// <summary>
        /// This Observable is fired when a ShouldReset fires on the collection. This
        /// means that you should forget your previous knowledge of the state
        /// of the collection and reread it.
        /// 
        /// This does *not* mean Clear, and if you interpret it as such, you are
        /// Doing It Wrong.
        /// </summary>
        IObservable<Unit> ShouldReset { get; }

        IDisposable SuppressChangeNotifications();
    }

    /// IReactiveCollection of T represents a collection that can notify when its
    /// contents are changed (either items are added/removed, or the object
    /// itself changes).
    ///
    /// It is important to implement the Changing/Changed from
    /// IReactiveNotifyPropertyChanged semantically as "Fire when *anything* in
    /// the collection or any of its items have changed, in any way".
    public interface IReactiveCollection<out T> : IReactiveNotifyCollectionChanged<T>, IReactiveNotifyCollectionItemChanged<T>, IEnumerable<T>, INotifyCollectionChanged, INotifyCollectionChanging, IReactiveObject
    {
        void Reset();
    }

    /// <summary>
    /// IReadOnlyReactiveCollection of T represents a read-only collection that can notify when its
    /// contents are changed (either items are added/removed, or the object
    /// itself changes).
    ///
    /// It is important to implement the Changing/Changed from
    /// IReactiveNotifyPropertyChanged semantically as "Fire when *anything* in
    /// the collection or any of its items have changed, in any way".
    /// </summary>
    public interface IReadOnlyReactiveCollection<out T> : IReadOnlyCollection<T>, IReactiveCollection<T>
    {
    }

    /// <summary>
    /// IReadOnlyReactiveList of T represents a read-only list that can notify when its
    /// contents are changed (either items are added/removed, or the object
    /// itself changes).
    ///
    /// It is important to implement the Changing/Changed from
    /// IReactiveNotifyPropertyChanged semantically as "Fire when *anything* in
    /// the collection or any of its items have changed, in any way".
    /// </summary>
    public interface IReadOnlyReactiveList<out T> : IReadOnlyReactiveCollection<T>, IReadOnlyList<T>
    {
        bool IsEmpty { get; } 
    }

    /// <summary>
    /// IReactiveDerivedList represents a collection whose contents will "follow" another
    /// collection; this method is useful for creating ViewModel collections
    /// that are automatically updated when the respective Model collection is updated.
    /// </summary>
    public interface IReactiveDerivedList<out T> : IReadOnlyReactiveList<T>, IDisposable
    {
    }

    /// <summary>
    /// IReactiveList of T represents a list that can notify when its
    /// contents are changed (either items are added/removed, or the object
    /// itself changes).
    ///
    /// It is important to implement the Changing/Changed from
    /// IReactiveNotifyPropertyChanged semantically as "Fire when *anything* in
    /// the collection or any of its items have changed, in any way".
    /// </summary>
    public interface IReactiveList<T> : IReactiveCollection<T>, IList<T>
    {
        bool IsEmpty { get; } 

        void AddRange(IEnumerable<T> collection);

        void InsertRange(int index, IEnumerable<T> collection);

        void RemoveAll(IEnumerable<T> items);

        void RemoveRange(int index, int count);

        void Sort(IComparer<T> comparer = null);

        void Sort(Comparison<T> comparison);

        void Sort(int index, int count, IComparer<T> comparer);
    }

    /// <summary>
    /// Implement this interface for ViewModels that can be navigated to.
    /// </summary>
    public interface IRoutableViewModel : IReactiveObject
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

    public interface ISupportsActivation
    {
        ViewModelActivator Activator { get; }
    }

    public interface ICanActivate
    {
        IObservable<Unit> Activated { get; }
        IObservable<Unit> Deactivated { get; }
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
