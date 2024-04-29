// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace ReactiveUI;

/// <summary>
/// Extension methods associated with the IReactiveObject interface.
/// </summary>
[Preserve(AllMembers = true)]
public static class IReactiveObjectExtensions
{
#if NETSTANDARD || NETFRAMEWORK
    private static readonly ConditionalWeakTable<IReactiveObject, IExtensionState<IReactiveObject>> state = new();
#else
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
        void SubscribePropertyChangingEvents();

        /// <summary>
        /// Raises a property changing event.
        /// </summary>
        /// <param name="propertyName">The name of the property that is changing.</param>
        void RaisePropertyChanging(string propertyName);

        /// <summary>
        /// Subscribe raise property changed events to a property changed
        /// observable. Must be called before raising property changed events.
        /// </summary>
        void SubscribePropertyChangedEvents();

        /// <summary>
        /// Raises a property changed event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        void RaisePropertyChanged(string propertyName);

        /// <summary>
        /// Indicates if we are currently sending change notifications.
        /// </summary>
        /// <returns>If change notifications are being sent.</returns>
        bool AreChangeNotificationsEnabled();

        /// <summary>
        /// Suppress change notifications until the return value is disposed.
        /// </summary>
        /// <returns>A IDisposable which when disposed will re-enable change notifications.</returns>
        IDisposable SuppressChangeNotifications();

        /// <summary>
        /// Are change notifications currently delayed. Used for Observables change notifications only.
        /// </summary>
        /// <returns>If the change notifications are delayed.</returns>
        bool AreChangeNotificationsDelayed();

        /// <summary>
        /// Delay change notifications until the return value is disposed.
        /// </summary>
        /// <returns>A IDisposable which when disposed will re-enable change notifications.</returns>
        IDisposable DelayChangeNotifications();
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
    public static TRet RaiseAndSetIfChanged<TObj, TRet>(
        this TObj reactiveObject,
        ref TRet backingField,
        TRet newValue,
        [CallerMemberName] string? propertyName = null)
        where TObj : IReactiveObject
    {
        propertyName.ArgumentNullExceptionThrowIfNull(nameof(propertyName));

        if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
        {
            return newValue;
        }

        reactiveObject.RaisingPropertyChanging(propertyName!);
        backingField = newValue;
        reactiveObject.RaisingPropertyChanged(propertyName!);
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
    public static void RaisePropertyChanged<TSender>(this TSender reactiveObject, [CallerMemberName] string? propertyName = null)
        where TSender : IReactiveObject
    {
        if (propertyName is not null)
        {
            reactiveObject.RaisingPropertyChanged(propertyName);
        }
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
    public static void RaisePropertyChanging<TSender>(this TSender reactiveObject, [CallerMemberName] string? propertyName = null)
        where TSender : IReactiveObject
    {
        if (propertyName is not null)
        {
            reactiveObject.RaisingPropertyChanging(propertyName);
        }
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
        var s = state.GetValue(reactiveObject, _ => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(reactiveObject));

        s.SubscribePropertyChangingEvents();
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
        var s = state.GetValue(reactiveObject, _ => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(reactiveObject));

        s.SubscribePropertyChangedEvents();
    }

    internal static IObservable<IReactivePropertyChangedEventArgs<TSender>> GetChangedObservable<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var val = state.GetValue(reactiveObject, _ => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(reactiveObject));
        return val.Changed.Cast<IReactivePropertyChangedEventArgs<TSender>>();
    }

    internal static IObservable<IReactivePropertyChangedEventArgs<TSender>> GetChangingObservable<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var val = state.GetValue(reactiveObject, _ => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(reactiveObject));
        return val.Changing.Cast<IReactivePropertyChangedEventArgs<TSender>>();
    }

    internal static IObservable<Exception> GetThrownExceptionsObservable<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = state.GetValue(reactiveObject, _ => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(reactiveObject));
        return s.ThrownExceptions;
    }

    internal static void RaisingPropertyChanging<TSender>(this TSender reactiveObject, string propertyName)
        where TSender : IReactiveObject
    {
        propertyName.ArgumentNullExceptionThrowIfNull(nameof(propertyName));

        var s = state.GetValue(reactiveObject, _ => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(reactiveObject));

        s.RaisePropertyChanging(propertyName);
    }

    internal static void RaisingPropertyChanged<TSender>(this TSender reactiveObject, string propertyName)
        where TSender : IReactiveObject
    {
        propertyName.ArgumentNullExceptionThrowIfNull(nameof(propertyName));

        var s = state.GetValue(reactiveObject, _ => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(reactiveObject));

        s.RaisePropertyChanged(propertyName);
    }

    internal static IDisposable SuppressChangeNotifications<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = state.GetValue(reactiveObject, _ => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(reactiveObject));

        return s.SuppressChangeNotifications();
    }

    internal static bool AreChangeNotificationsEnabled<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = state.GetValue(reactiveObject, _ => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(reactiveObject));

        return s.AreChangeNotificationsEnabled();
    }

    internal static IDisposable DelayChangeNotifications<TSender>(this TSender reactiveObject)
        where TSender : IReactiveObject
    {
        var s = state.GetValue(reactiveObject, _ => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(reactiveObject));

        return s.DelayChangeNotifications();
    }

    private class ExtensionState<TSender> : IExtensionState<TSender>
        where TSender : IReactiveObject
    {
        private readonly Lazy<ISubject<Exception>> _thrownExceptions = new(() => new ScheduledSubject<Exception>(Scheduler.Immediate, RxApp.DefaultExceptionHandler));
        private readonly Lazy<Subject<Unit>> _startOrStopDelayingChangeNotifications = new();
        private readonly TSender _sender;
        private readonly Lazy<(ISubject<IReactivePropertyChangedEventArgs<TSender>> subject, IObservable<IReactivePropertyChangedEventArgs<TSender>> observable)> _changing;
        private readonly Lazy<(ISubject<IReactivePropertyChangedEventArgs<TSender>> subject, IObservable<IReactivePropertyChangedEventArgs<TSender>> observable)> _changed;
        private readonly Lazy<ISubject<ReactivePropertyChangingEventArgs<TSender>>> _propertyChanging;
        private readonly Lazy<ISubject<ReactivePropertyChangedEventArgs<TSender>>> _propertyChanged;

        private long _changeNotificationsSuppressed;
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
            _propertyChanging = CreateLazyDelayableEventSubject<ReactivePropertyChangingEventArgs<TSender>>(_sender.RaisePropertyChanging);
            _propertyChanged = CreateLazyDelayableEventSubject<ReactivePropertyChangedEventArgs<TSender>>(_sender.RaisePropertyChanged);
        }

        public IObservable<IReactivePropertyChangedEventArgs<TSender>> Changing => _changing.Value.observable;

        public IObservable<IReactivePropertyChangedEventArgs<TSender>> Changed => _changed.Value.observable;

        public IObservable<Exception> ThrownExceptions => _thrownExceptions.Value;

        public bool AreChangeNotificationsEnabled() => Interlocked.Read(ref _changeNotificationsSuppressed) == 0;

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
        public IDisposable SuppressChangeNotifications()
        {
            Interlocked.Increment(ref _changeNotificationsSuppressed);
            return Disposable.Create(() => Interlocked.Decrement(ref _changeNotificationsSuppressed));
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
        public IDisposable DelayChangeNotifications()
        {
            if (Interlocked.Increment(ref _changeNotificationsDelayed) == 1)
            {
                if (_startOrStopDelayingChangeNotifications.IsValueCreated)
                {
                    _startOrStopDelayingChangeNotifications.Value.OnNext(Unit.Default);
                }
            }

            return Disposable.Create(() =>
            {
                if (Interlocked.Decrement(ref _changeNotificationsDelayed) == 0)
                {
                    if (_startOrStopDelayingChangeNotifications.IsValueCreated)
                    {
                        _startOrStopDelayingChangeNotifications.Value.OnNext(Unit.Default);
                    }
                }
            });
        }

        public void SubscribePropertyChangingEvents() => _ = _propertyChanging.Value;

        public void RaisePropertyChanging(string propertyName)
        {
            if (!AreChangeNotificationsEnabled())
            {
                return;
            }

            var changing = new ReactivePropertyChangingEventArgs<TSender>(_sender, propertyName);
            if (_propertyChanging.IsValueCreated)
            {
                // Do not use NotifyObservable because event exceptions shouldn't be put in ThrownExceptions
                _propertyChanging.Value.OnNext(changing);
            }

            if (_changing.IsValueCreated)
            {
                NotifyObservable(_sender, changing, _changing.Value.subject);
            }
        }

        public void SubscribePropertyChangedEvents() => _ = _propertyChanged.Value;

        public void RaisePropertyChanged(string propertyName)
        {
            if (!AreChangeNotificationsEnabled())
            {
                return;
            }

            var changed = new ReactivePropertyChangedEventArgs<TSender>(_sender, propertyName);
            if (_propertyChanged.IsValueCreated)
            {
                // Do not use NotifyObservable because event exceptions shouldn't be put in ThrownExceptions
                _propertyChanged.Value.OnNext(changed);
            }

            if (_changed.IsValueCreated)
            {
                NotifyObservable(_sender, changed, _changed.Value.subject);
            }
        }

        /// <summary>
        /// Filter a list of change notifications, returning the last change for each PropertyName in original order.
        /// </summary>
        private static IEnumerable<TEventArgs> DistinctEvents<TEventArgs>(IList<TEventArgs> events)
            where TEventArgs : IReactivePropertyChangedEventArgs<TSender>
        {
            if (events.Count <= 1)
            {
                return events;
            }

            var seen = new HashSet<string>();
            var uniqueEvents = new Stack<TEventArgs>(events.Count);

            for (var i = events.Count - 1; i >= 0; i--)
            {
                var propertyName = events[i].PropertyName;
                if (propertyName is not null && seen.Add(propertyName))
                {
                    uniqueEvents.Push(events[i]);
                }
            }

            // Stack enumerates in LIFO order
            return uniqueEvents;
        }

        private void NotifyObservable<T>(TSender rxObj, T item, ISubject<T>? subject)
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

        private Lazy<(ISubject<IReactivePropertyChangedEventArgs<TSender>> changeSubject, IObservable<IReactivePropertyChangedEventArgs<TSender>> changeObservable)> CreateLazyDelayableSubjectAndObservable() =>
            new(() =>
            {
                var changeSubject = new Subject<IReactivePropertyChangedEventArgs<TSender>>();
                var changeObservable = changeSubject
                                       .Buffer(changeSubject.Where(_ => !AreChangeNotificationsDelayed()).Select(_ => Unit.Default)
                                                            .Merge(_startOrStopDelayingChangeNotifications.Value))
                                       .SelectMany(DistinctEvents)
                                       .Publish()
                                       .RefCount();

                return (changeSubject, changeObservable);
            });

        private Lazy<ISubject<TEventArgs>> CreateLazyDelayableEventSubject<TEventArgs>(Action<TEventArgs> raiseEvent)
            where TEventArgs : IReactivePropertyChangedEventArgs<TSender> =>
            new(() =>
            {
                var changeSubject = new Subject<TEventArgs>();
                changeSubject
                    .Buffer(changeSubject.Where(_ => !AreChangeNotificationsDelayed()).Select(_ => Unit.Default)
                                         .Merge(_startOrStopDelayingChangeNotifications.Value))
                    .SelectMany(DistinctEvents)
                    .Subscribe(raiseEvent);

                return changeSubject;
            });
    }
}
