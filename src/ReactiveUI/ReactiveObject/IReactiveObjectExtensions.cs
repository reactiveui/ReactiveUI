// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;

using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>
/// Extension methods associated with the IReactiveObject interface.
/// </summary>
[Preserve(AllMembers = true)]
public static class IReactiveObjectExtensions
{
#if NETSTANDARD || NETFRAMEWORK
    /// <summary>Stores per-instance extension state keyed by reactive object.</summary>
    private static readonly ConditionalWeakTable<IReactiveObject, IExtensionState<IReactiveObject>> state = new();
#else
    /// <summary>Stores per-instance extension state keyed by reactive object.</summary>
    private static readonly ConditionalWeakTable<IReactiveObject, IExtensionState<IReactiveObject>> state = [];
#endif

    /// <summary>
    /// Contains the state information about the current status of a Reactive Object.
    /// </summary>
    /// <typeparam name="TSender">The type of the sender of the property changes.</typeparam>
    private interface IExtensionState<out TSender>
        where TSender : IReactiveObject
    {
        /// <summary>
        /// Gets an observable for when a property is changing.
        /// </summary>
        IObservable<IReactivePropertyChangedEventArgs<TSender>> Changing { get; }

        /// <summary>
        /// Gets an observable for when the property has changed.
        /// </summary>
        IObservable<IReactivePropertyChangedEventArgs<TSender>> Changed { get; }

        /// <summary>
        /// Gets a observable for when an exception is thrown.
        /// </summary>
        IObservable<Exception> ThrownExceptions { get; }

        /// <summary>
        /// Subscribe raise property changing events to a property changing
        /// observable. Must be called before raising property changing events.
        /// </summary>
        void SubscribeChanging();

        /// <summary>
        /// Raises a property changing event.
        /// </summary>
        /// <param name="propertyName">The name of the property that is changing.</param>
        void RaiseChanging(string propertyName);

        /// <summary>
        /// Subscribe raise property changed events to a property changed
        /// observable. Must be called before raising property changed events.
        /// </summary>
        void SubscribeChanged();

        /// <summary>
        /// Raises a property changed event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        void RaiseChanged(string propertyName);

        /// <summary>
        /// Indicates if we are currently sending change notifications.
        /// </summary>
        /// <returns>If change notifications are being sent.</returns>
        bool NotificationsEnabled();

        /// <summary>
        /// Suppress change notifications until the return value is disposed.
        /// </summary>
        /// <returns>A IDisposable which when disposed will re-enable change notifications.</returns>
        IDisposable Suppress();

        /// <summary>
        /// Are change notifications currently delayed. Used for Observables change notifications only.
        /// </summary>
        /// <returns>If the change notifications are delayed.</returns>
        bool AreChangeNotificationsDelayed();

        /// <summary>
        /// Delay change notifications until the return value is disposed.
        /// </summary>
        /// <returns>A IDisposable which when disposed will re-enable change notifications.</returns>
        IDisposable Delay();
    }

    /// <summary>
    /// RaiseAndSetIfChanged fully implements a Setter for a read-write
    /// property on a ReactiveObject, using CallerMemberName to raise the notification
    /// and the ref to the backing field to set the property.
    /// </summary>
    /// <typeparam name="TObj">The type of the This.</typeparam>
    /// <typeparam name="TRet">The type of the return value.</typeparam>
    /// <param name="reactiveObject">The <see cref="ReactiveObject"/> raising the notification.</param>
    /// <param name="backingField">A Reference to the backing field for this
    /// property.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="propertyName">The name of the property, usually
    /// automatically provided through the CallerMemberName attribute.</param>
    /// <returns>The newly set value, normally discarded.</returns>
    [SuppressMessage("Design", "CA1045:Do not pass types by reference", Justification = "By default for this operator.")]
    public static TRet RaiseAndSetIfChanged<TObj, TRet>(
        this TObj reactiveObject,
        ref TRet backingField,
        TRet newValue,
        [CallerMemberName] string? propertyName = null)
        where TObj : IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
        {
            return newValue;
        }

        reactiveObject.RaisingPropertyChanging(propertyName);
        backingField = newValue;
        reactiveObject.RaisingPropertyChanged(propertyName);
        return newValue;
    }

    /// <summary>
    /// Use this method in your ReactiveObject classes when creating custom
    /// properties where raiseAndSetIfChanged doesn't suffice.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <param name="reactiveObject">The instance of ReactiveObject on which the property has changed.</param>
    /// <param name="propertyName">
    /// A string representing the name of the property that has been changed.
    /// Leave <c>null</c> to let the runtime set to caller member name.
    /// </param>
    public static void RaisePropertyChanged<TSender>(
        this TSender reactiveObject,
        [CallerMemberName] string? propertyName = null)
        where TSender : IReactiveObject
    {
        if (propertyName is null)
        {
            return;
        }

        reactiveObject.RaisingPropertyChanged(propertyName);
    }

    /// <summary>
    /// Use this method in your ReactiveObject classes when creating custom
    /// properties where raiseAndSetIfChanged doesn't suffice.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <param name="reactiveObject">The instance of ReactiveObject on which the property has changed.</param>
    /// <param name="propertyName">
    /// A string representing the name of the property that has been changed.
    /// Leave <c>null</c> to let the runtime set to caller member name.
    /// </param>
    public static void RaisePropertyChanging<TSender>(
        this TSender reactiveObject,
        [CallerMemberName] string? propertyName = null)
        where TSender : IReactiveObject
    {
        if (propertyName is null)
        {
            return;
        }

        reactiveObject.RaisingPropertyChanging(propertyName);
    }

    /// <summary>
    /// Use this method for enabling classic PropertyChanging events when you
    /// are implementing IReactiveObject manually.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <param name="reactiveObject">The instance of IReactiveObject which should propagate property changes.</param>
    public static void SubscribePropertyChangingEvents<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = state.GetValue(
            reactiveObject,
            _ => (IExtensionState<IReactiveObject>)(object)new ExtensionState<TSender>(reactiveObject));

        s.SubscribeChanging();
    }

    /// <summary>
    /// Use this method for enabling classic PropertyChanged events when you
    /// are implementing IReactiveObject manually.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <param name="reactiveObject">The instance of IReactiveObject which should propagate property changes.</param>
    public static void SubscribePropertyChangedEvents<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = state.GetValue(
            reactiveObject,
            _ => (IExtensionState<IReactiveObject>)(object)new ExtensionState<TSender>(reactiveObject));

        s.SubscribeChanged();
    }

    /// <summary>
    /// Returns an observable sequence that signals when a property on the specified reactive object has changed.
    /// </summary>
    /// <remarks>The returned observable emits events for all property changes on the provided reactive
    /// object. Subscribers can use this to react to changes in any property. The observable completes only when the
    /// reactive object is disposed, if applicable.</remarks>
    /// <typeparam name="TSender">The type of the reactive object that implements IReactiveObject.</typeparam>
    /// <param name="reactiveObject">The reactive object to observe for property change notifications. Cannot be null.</param>
    /// <returns>An observable sequence of property change event arguments for the specified reactive object. The sequence emits
    /// a value each time a property changes.</returns>
    internal static IObservable<IReactivePropertyChangedEventArgs<TSender>> GetChangedObservable<TSender>(
        this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var val = state.GetValue(
            reactiveObject,
            _ => (IExtensionState<IReactiveObject>)(object)new ExtensionState<TSender>(reactiveObject));
        return new ChangeArgsCastObservable<TSender>(val.Changed);
    }

    /// <summary>
    /// Returns an observable sequence that signals before a property value changes on the specified reactive object.
    /// </summary>
    /// <remarks>Subscribers can use the returned observable to react to property changes before the values
    /// are updated. This is useful for scenarios where actions need to be taken prior to a property's value being
    /// modified.</remarks>
    /// <typeparam name="TSender">The type of the reactive object that implements the IReactiveObject interface.</typeparam>
    /// <param name="reactiveObject">The reactive object to observe for property changing notifications. Must implement IReactiveObject.</param>
    /// <returns>An observable sequence of IReactivePropertyChangedEventArgs{TSender} that emits a value each time a property on
    /// the reactive object is about to change.</returns>
    internal static IObservable<IReactivePropertyChangedEventArgs<TSender>> GetChangingObservable<TSender>(
        this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var val = state.GetValue(
            reactiveObject,
            _ => (IExtensionState<IReactiveObject>)(object)new ExtensionState<TSender>(reactiveObject));
        return new ChangeArgsCastObservable<TSender>(val.Changing);
    }

    /// <summary>
    /// Returns an observable sequence that emits exceptions thrown by the specified reactive object during property
    /// change notifications or command executions.
    /// </summary>
    /// <remarks>Subscribers can use the returned observable to monitor and handle exceptions that occur
    /// within the reactive object's reactive operations. This is useful for centralized error handling in reactive UI
    /// or data models.</remarks>
    /// <typeparam name="TSender">The type of the reactive object that implements the IReactiveObject interface.</typeparam>
    /// <param name="reactiveObject">The reactive object for which to observe thrown exceptions. Must implement IReactiveObject.</param>
    /// <returns>An observable sequence of Exception objects representing errors thrown by the reactive object. The sequence
    /// completes when the reactive object is disposed.</returns>
    internal static IObservable<Exception> GetThrownExceptionsObservable<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = state.GetValue(
            reactiveObject,
            _ => (IExtensionState<IReactiveObject>)(object)new ExtensionState<TSender>(reactiveObject));
        return s.ThrownExceptions;
    }

    /// <summary>
    /// Raises the PropertyChanging event for the specified property on the given reactive object.
    /// </summary>
    /// <typeparam name="TSender">The type of the reactive object that implements IReactiveObject.</typeparam>
    /// <param name="reactiveObject">The reactive object on which to raise the PropertyChanging event.</param>
    /// <param name="propertyName">The name of the property for which the PropertyChanging event is raised. Cannot be null.</param>
    internal static void RaisingPropertyChanging<TSender>(this TSender reactiveObject, string propertyName)
        where TSender : IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        var s = state.GetValue(
            reactiveObject,
            _ => (IExtensionState<IReactiveObject>)(object)new ExtensionState<TSender>(reactiveObject));

        s.RaiseChanging(propertyName);
    }

    /// <summary>
    /// Raises the PropertyChanged event for the specified property on the given reactive object.
    /// </summary>
    /// <typeparam name="TSender">The type of the reactive object that implements IReactiveObject.</typeparam>
    /// <param name="reactiveObject">The reactive object on which to raise the PropertyChanged event.</param>
    /// <param name="propertyName">The name of the property for which the PropertyChanged event is raised. Cannot be null.</param>
    internal static void RaisingPropertyChanged<TSender>(this TSender reactiveObject, string propertyName)
        where TSender : IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        var s = state.GetValue(
            reactiveObject,
            _ => (IExtensionState<IReactiveObject>)(object)new ExtensionState<TSender>(reactiveObject));

        s.RaiseChanged(propertyName);
    }

    /// <summary>
    /// Temporarily suppresses change notifications for the specified reactive object.
    /// </summary>
    /// <remarks>While change notifications are suppressed, property change and other reactive notifications
    /// will not be raised. Dispose the returned object to resume normal notification behavior. This method is typically
    /// used to batch multiple changes and avoid triggering notifications for each individual change.</remarks>
    /// <typeparam name="TSender">The type of the reactive object for which to suppress change notifications. Must implement <see
    /// cref="IReactiveObject"/>.</typeparam>
    /// <param name="reactiveObject">The reactive object whose change notifications are to be suppressed.</param>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, restores change notifications for the specified object.</returns>
    internal static IDisposable SuppressChangeNotifications<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = state.GetValue(
            reactiveObject,
            _ => (IExtensionState<IReactiveObject>)(object)new ExtensionState<TSender>(reactiveObject));

        return s.Suppress();
    }

    /// <summary>
    /// Determines whether change notifications are currently enabled for the specified reactive object.
    /// </summary>
    /// <typeparam name="TSender">The type of the reactive object. Must implement <see cref="IReactiveObject"/>.</typeparam>
    /// <param name="reactiveObject">The reactive object to check for change notification support. Cannot be null.</param>
    /// <returns>true if change notifications are enabled for the specified object; otherwise, false.</returns>
    internal static bool AreChangeNotificationsEnabled<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = state.GetValue(
            reactiveObject,
            _ => (IExtensionState<IReactiveObject>)(object)new ExtensionState<TSender>(reactiveObject));

        return s.NotificationsEnabled();
    }

    /// <summary>
    /// Temporarily suspends change notifications for the specified reactive object until the returned disposable is
    /// disposed.
    /// </summary>
    /// <remarks>Use this method to batch multiple changes to a reactive object and suppress intermediate
    /// change notifications. Change notifications are resumed automatically when the returned disposable is disposed.
    /// This is useful for improving performance or preventing unnecessary updates when making several changes in quick
    /// succession.</remarks>
    /// <typeparam name="TSender">The type of the reactive object for which change notifications are to be delayed. Must implement <see
    /// cref="IReactiveObject"/>.</typeparam>
    /// <param name="reactiveObject">The reactive object whose change notifications will be delayed. Cannot be null.</param>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, resumes change notifications for the specified object.</returns>
    internal static IDisposable DelayChangeNotifications<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = state.GetValue(
            reactiveObject,
            _ => (IExtensionState<IReactiveObject>)(object)new ExtensionState<TSender>(reactiveObject));

        return s.Delay();
    }

    /// <summary>
    /// Manages the state and change notification logic for a reactive object extension, including suppression and
    /// delaying of property change notifications.
    /// </summary>
    /// <remarks>This class provides mechanisms to control and observe property change notifications for
    /// reactive objects, supporting scenarios where notifications need to be temporarily suppressed or delayed. It is
    /// intended for internal use by reactive extensions to manage notification lifecycles and exception
    /// handling.</remarks>
    /// <typeparam name="TSender">The type of the reactive object that this extension state is associated with. Must implement <see
    /// cref="IReactiveObject"/>.</typeparam>
    private sealed class ExtensionState<TSender> : IExtensionState<TSender>
        where TSender : IReactiveObject
    {
        /// <summary>Lazily initialized subject for routing thrown exceptions to subscribers.</summary>
        private readonly Lazy<IReactiveSubject<Exception>> _thrownExceptions = new(static () =>
            new ScheduledSubject<Exception>(Scheduler.Immediate, RxState.DefaultExceptionHandler));

        /// <summary>The reactive object that owns this extension state.</summary>
        private readonly TSender _sender;

        /// <summary>Lazily initialized delayable sink for property changing notifications (also the Changing observable).</summary>
        private readonly Lazy<DelayableNotificationSubject<IReactivePropertyChangedEventArgs<TSender>>> _changing;

        /// <summary>Lazily initialized delayable sink for property changed notifications (also the Changed observable).</summary>
        private readonly Lazy<DelayableNotificationSubject<IReactivePropertyChangedEventArgs<TSender>>> _changed;

        /// <summary>Lazily initialized delayable sink that raises PropertyChanging events.</summary>
        private readonly Lazy<DelayableNotificationSubject<ReactivePropertyChangingEventArgs<TSender>>> _propertyChanging;

        /// <summary>Lazily initialized delayable sink that raises PropertyChanged events.</summary>
        private readonly Lazy<DelayableNotificationSubject<ReactivePropertyChangedEventArgs<TSender>>> _propertyChanged;

        /// <summary>Reference count of active change notification suppressions; zero means notifications are enabled.</summary>
        private long _changeNotificationsSuppressed;

        /// <summary>Reference count of active change notification delays; greater than zero means notifications are buffered.</summary>
        private long _changeNotificationsDelayed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionState{TSender}"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public ExtensionState(TSender sender)
        {
            _sender = sender;
            _changing = CreateLazyDelayableSubjectAndObservable();
            _changed = CreateLazyDelayableSubjectAndObservable();
            _propertyChanging =
                CreateLazyDelayableEventSubject<ReactivePropertyChangingEventArgs<TSender>>(
                    _sender.RaisePropertyChanging);
            _propertyChanged =
                CreateLazyDelayableEventSubject<ReactivePropertyChangedEventArgs<TSender>>(
                    _sender.RaisePropertyChanged);
        }

        /// <summary>
        /// Gets an observable sequence that signals before a property value changes on the object.
        /// </summary>
        /// <remarks>Subscribers receive notifications immediately before a property value is about to
        /// change. This can be used to react to impending changes or to perform validation or cancellation logic. The
        /// sequence emits an event for each property change, providing information about the sender and the property
        /// being changed.</remarks>
        public IObservable<IReactivePropertyChangedEventArgs<TSender>> Changing => _changing.Value;

        /// <summary>
        /// Gets an observable sequence that signals when a property value on the object has changed.
        /// </summary>
        /// <remarks>Subscribers receive notifications each time a property on the object changes. The
        /// event arguments provide details about the sender and the property that changed. This observable does not
        /// emit notifications for changes that do not affect property values.</remarks>
        public IObservable<IReactivePropertyChangedEventArgs<TSender>> Changed => _changed.Value;

        /// <summary>
        /// Gets an observable sequence of exceptions that have been thrown by the component.
        /// </summary>
        /// <remarks>Subscribers can use this sequence to monitor and react to errors that occur during
        /// the component's operation. The sequence completes when the component is disposed or no further exceptions
        /// will be emitted.</remarks>
        public IObservable<Exception> ThrownExceptions => _thrownExceptions.Value;

        /// <summary>
        /// Determines whether change notifications are currently enabled.
        /// </summary>
        /// <returns><see langword="true"/> if change notifications are enabled; otherwise, <see langword="false"/>.</returns>
        public bool NotificationsEnabled() => Interlocked.Read(ref _changeNotificationsSuppressed) == 0;

        /// <summary>
        /// Determines whether change notifications are currently delayed.
        /// </summary>
        /// <remarks>This method can be used to check if change notifications are temporarily suspended,
        /// which may occur during batch updates or other operations that require notification suppression.</remarks>
        /// <returns><see langword="true"/> if change notifications are being delayed; otherwise, <see langword="false"/>.</returns>
        public bool AreChangeNotificationsDelayed() => Interlocked.Read(ref _changeNotificationsDelayed) > 0;

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// If this method is called multiple times it will reference count
        /// and not perform notification until all values returned are disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change
        /// notifications.</returns>
        public IDisposable Suppress()
        {
            Interlocked.Increment(ref _changeNotificationsSuppressed);
            return new ActionDisposable(() => Interlocked.Decrement(ref _changeNotificationsSuppressed));
        }

        /// <summary>
        /// When this method is called, an object will not dispatch change
        /// Observable notifications until the return value is disposed.
        /// When the Disposable it will dispatched all queued notifications.
        /// If this method is called multiple times it will reference count
        /// and not perform notification until all values returned are disposed.
        /// </summary>
        /// <returns>An object that, when disposed, re-enables Observable change
        /// notifications.</returns>
        public IDisposable Delay()
        {
            if (Interlocked.Increment(ref _changeNotificationsDelayed) == 1)
            {
                FlushDelayed();
            }

            return new ActionDisposable(() =>
            {
                if (Interlocked.Decrement(ref _changeNotificationsDelayed) != 0)
                {
                    return;
                }

                FlushDelayed();
            });
        }

        /// <summary>
        /// Subscribes to property changing events for the current instance.
        /// </summary>
        /// <remarks>Calling this method ensures that property changing notifications are initialized and
        /// will be raised when applicable. This is typically used to enable change tracking or to allow external
        /// handlers to respond to property changes.</remarks>
        public void SubscribeChanging() => _ = _propertyChanging.Value;

        /// <summary>
        /// Raises a property changing notification for the specified property.
        /// </summary>
        /// <remarks>This method notifies subscribers that a property is about to change. Change
        /// notifications are only raised if change notifications are currently enabled. Use this method to support data
        /// binding or other scenarios where consumers need to react before a property value changes.</remarks>
        /// <param name="propertyName">The name of the property for which the change notification is raised. Cannot be null or empty.</param>
        public void RaiseChanging(string propertyName)
        {
            if (!NotificationsEnabled())
            {
                return;
            }

            ReactivePropertyChangingEventArgs<TSender> changing = new(_sender, propertyName);
            if (_propertyChanging.IsValueCreated)
            {
                _propertyChanging.Value.OnNext(changing);
            }

            if (!_changing.IsValueCreated)
            {
                return;
            }

            NotifyObservable(_sender, changing, _changing.Value);
        }

        /// <summary>
        /// Subscribes to property changed events for the current instance.
        /// </summary>
        /// <remarks>Call this method to ensure that property change notifications are set up. This is
        /// typically required before observing property changes through event handlers or data binding
        /// mechanisms.</remarks>
        public void SubscribeChanged() => _ = _propertyChanged.Value;

        /// <summary>
        /// Notifies subscribers that the value of a specified property has changed.
        /// </summary>
        /// <remarks>This method raises property change notifications to observers if change notifications
        /// are currently enabled. Use this method to inform listeners that a property value has been updated, typically
        /// within property setters.</remarks>
        /// <param name="propertyName">The name of the property that changed. Cannot be null or empty.</param>
        public void RaiseChanged(string propertyName)
        {
            if (!NotificationsEnabled())
            {
                return;
            }

            ReactivePropertyChangedEventArgs<TSender> changed = new(_sender, propertyName);
            if (_propertyChanged.IsValueCreated)
            {
                _propertyChanged.Value.OnNext(changed);
            }

            if (!_changed.IsValueCreated)
            {
                return;
            }

            NotifyObservable(_sender, changed, _changed.Value);
        }

        /// <summary>Filters a list of change notifications, returning the last change for each PropertyName in original order.</summary>
        /// <typeparam name="TEventArgs">The type of the change notification arguments.</typeparam>
        /// <param name="events">The change notifications to filter.</param>
        /// <returns>The last change notification for each property name, in original order.</returns>
        private static IEnumerable<TEventArgs> DistinctEvents<TEventArgs>(IList<TEventArgs> events)
            where TEventArgs : IReactivePropertyChangedEventArgs<TSender>
        {
            if (events.Count <= 1)
            {
                return events;
            }

            HashSet<string> seen = [];
            Stack<TEventArgs> uniqueEvents = new(events.Count);

            for (var i = events.Count - 1; i >= 0; i--)
            {
                var propertyName = events[i].PropertyName;
                if (propertyName is not null && seen.Add(propertyName))
                {
                    uniqueEvents.Push(events[i]);
                }
            }

            return uniqueEvents;
        }

        /// <summary>Flushes a lazily-created delayable sink if it has been created.</summary>
        /// <typeparam name="T">The sink's notification type.</typeparam>
        /// <param name="sink">The lazily-created sink.</param>
        private static void FlushIfCreated<T>(Lazy<DelayableNotificationSubject<T>> sink)
        {
            if (!sink.IsValueCreated)
            {
                return;
            }

            sink.Value.Flush();
        }

        /// <summary>Flushes any notifications buffered while change notifications were delayed.</summary>
        private void FlushDelayed()
        {
            FlushIfCreated(_propertyChanging);
            FlushIfCreated(_propertyChanged);
            FlushIfCreated(_changing);
            FlushIfCreated(_changed);
        }

        /// <summary>
        /// Notifies the specified subject with the provided item, handling any exceptions that occur during
        /// notification.
        /// </summary>
        /// <remarks>If an exception is thrown by a subscriber during notification, the exception is
        /// logged and, depending on the state of the internal exception handler, may be rethrown or forwarded to an
        /// exception subject.</remarks>
        /// <typeparam name="T">The type of the item to be sent to the subject.</typeparam>
        /// <param name="rxObj">The sender object associated with the notification. Used for logging if an exception occurs.</param>
        /// <param name="item">The item to send to the subject's observers.</param>
        /// <param name="subject">The subject to be notified. If null, no notification is sent.</param>
        private void NotifyObservable<T>(TSender rxObj, T item, DelayableNotificationSubject<T>? subject)
        {
            try
            {
                subject?.OnNext(item);
            }
            catch (Exception ex)
            {
                rxObj.Log().Error(ex, "ReactiveObject Subscriber threw exception");
                if (!_thrownExceptions.IsValueCreated)
                {
                    throw;
                }

                _thrownExceptions.Value.OnNext(ex);
            }
        }

        /// <summary>
        /// Creates a lazily initialized tuple containing a subject for change notifications and an observable sequence
        /// of distinct change events.
        /// </summary>
        /// <remarks>The returned observable buffers and emits change events based on the current delay
        /// settings. Subscribers receive only distinct change events. The subject and observable are created only when
        /// first accessed.</remarks>
        /// <returns>A lazy-initialized tuple consisting of an <see cref="DelayableNotificationSubject{T}"/> for <c>IReactivePropertyChangedEventArgs&lt;TSender&gt;</c>
        /// for publishing change notifications and an <see cref="IObservable{T}"/> for <c>IReactivePropertyChangedEventArgs&lt;TSender&gt;</c>
        /// that emits distinct change events, respecting any configured delay in change notifications.</returns>
        private Lazy<DelayableNotificationSubject<IReactivePropertyChangedEventArgs<TSender>>>
            CreateLazyDelayableSubjectAndObservable() =>
            new(() => new DelayableNotificationSubject<IReactivePropertyChangedEventArgs<TSender>>(
                AreChangeNotificationsDelayed,
                DistinctEvents));

        /// <summary>
        /// Creates a lazily initialized subject for event notifications that supports delayed change notification
        /// delivery.
        /// </summary>
        /// <remarks>The returned subject buffers and batches event notifications when change
        /// notifications are delayed, and emits them when delivery resumes. This allows consumers to subscribe to event
        /// streams that respect delayed notification semantics.</remarks>
        /// <typeparam name="TEventArgs">The type of event arguments associated with the event notifications. Must implement
        /// IReactivePropertyChangedEventArgs{TSender}.</typeparam>
        /// <param name="raiseEvent">An action to invoke for each event notification emitted by the subject.</param>
        /// <returns>A Lazy object that initializes an ISubject{TEventArgs} for publishing event notifications when first
        /// accessed.</returns>
        private Lazy<DelayableNotificationSubject<TEventArgs>> CreateLazyDelayableEventSubject<TEventArgs>(Action<TEventArgs> raiseEvent)
            where TEventArgs : IReactivePropertyChangedEventArgs<TSender> =>
            new(() =>
            {
                var changeSubject = new DelayableNotificationSubject<TEventArgs>(AreChangeNotificationsDelayed, DistinctEvents);
                changeSubject.Subscribe(new DelegateObserver<TEventArgs>(raiseEvent));
                return changeSubject;
            });
    }

    /// <summary>
    /// Re-types the reactive object's change-argument stream from the non-generic <see cref="IReactiveObject"/> form to
    /// the caller's <typeparamref name="TSender"/>. Specialised to <see cref="GetChangedObservable{TSender}"/> and
    /// <see cref="GetChangingObservable{TSender}"/>.
    /// </summary>
    /// <typeparam name="TSender">The reactive object type observed.</typeparam>
    /// <param name="source">The source change-argument stream.</param>
    private sealed class ChangeArgsCastObservable<TSender>(IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> source)
        : IObservable<IReactivePropertyChangedEventArgs<TSender>>
        where TSender : IReactiveObject
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IReactivePropertyChangedEventArgs<TSender>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer));
        }

        /// <summary>Re-types each change-argument value to the caller's sender type.</summary>
        /// <param name="downstream">The observer receiving re-typed change arguments.</param>
        private sealed class Sink(IObserver<IReactivePropertyChangedEventArgs<TSender>> downstream)
            : IObserver<IReactivePropertyChangedEventArgs<IReactiveObject>>
        {
            /// <inheritdoc/>
            public void OnNext(IReactivePropertyChangedEventArgs<IReactiveObject> value) =>
                downstream.OnNext((IReactivePropertyChangedEventArgs<TSender>)(object)value);

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }
}
