// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using ReactiveUI.Internal;
using Splat;

namespace ReactiveUI;

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
internal sealed class ExtensionState<TSender> : IExtensionState<TSender>
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

        var raisesEvent = _propertyChanging.IsValueCreated;
        var raisesObservable = _changing.IsValueCreated;
        if (!raisesEvent && !raisesObservable)
        {
            // No PropertyChanging listener and no Changing observer: don't allocate the event args.
            return;
        }

        ReactivePropertyChangingEventArgs<TSender> changing = new(_sender, propertyName);
        if (raisesEvent)
        {
            _propertyChanging.Value.OnNext(changing);
        }

        if (!raisesObservable)
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

        var raisesEvent = _propertyChanged.IsValueCreated;
        var raisesObservable = _changed.IsValueCreated;
        if (!raisesEvent && !raisesObservable)
        {
            // No PropertyChanged listener and no Changed observer: don't allocate the event args.
            return;
        }

        ReactivePropertyChangedEventArgs<TSender> changed = new(_sender, propertyName);
        if (raisesEvent)
        {
            _propertyChanged.Value.OnNext(changed);
        }

        if (!raisesObservable)
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
        new(() => new(
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
    private Lazy<DelayableNotificationSubject<TEventArgs>> CreateLazyDelayableEventSubject<TEventArgs>(
        Action<TEventArgs> raiseEvent)
        where TEventArgs : IReactivePropertyChangedEventArgs<TSender> =>
        new(() =>
        {
            var changeSubject =
                new DelayableNotificationSubject<TEventArgs>(AreChangeNotificationsDelayed, DistinctEvents);
            changeSubject.Subscribe(new DelegateObserver<TEventArgs>(raiseEvent));
            return changeSubject;
        });
}
