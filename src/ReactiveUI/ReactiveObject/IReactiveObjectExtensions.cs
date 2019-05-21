﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Extension methods associated with the IReactiveObject interface.
    /// </summary>
    [Preserve(AllMembers = true)]
    public static class IReactiveObjectExtensions
    {
        private static readonly ConditionalWeakTable<IReactiveObject, IExtensionState<IReactiveObject>> state = new ConditionalWeakTable<IReactiveObject, IExtensionState<IReactiveObject>>();

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
            /// Gets a observable when a exception is thrown.
            /// </summary>
            IObservable<Exception> ThrownExceptions { get; }

            /// <summary>
            /// Raises a property changing event.
            /// </summary>
            /// <param name="propertyName">The name of the property that is changing.</param>
            void RaisePropertyChanging(string propertyName);

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
            [CallerMemberName] string propertyName = null)
            where TObj : IReactiveObject
        {
            Contract.Requires(propertyName != null);

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
        public static void RaisePropertyChanged<TSender>(this TSender reactiveObject, [CallerMemberName] string propertyName = null)
            where TSender : IReactiveObject
        {
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
        public static void RaisePropertyChanging<TSender>(this TSender reactiveObject, [CallerMemberName] string propertyName = null)
            where TSender : IReactiveObject
        {
            reactiveObject.RaisingPropertyChanging(propertyName);
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
            Contract.Requires(propertyName != null);

            var s = state.GetValue(reactiveObject, _ => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(reactiveObject));

            s.RaisePropertyChanging(propertyName);
        }

        internal static void RaisingPropertyChanged<TSender>(this TSender reactiveObject, string propertyName)
            where TSender : IReactiveObject
        {
            Contract.Requires(propertyName != null);

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

        /// <summary>
        /// Filter a list of change notifications, returning the last change for each PropertyName in original order.
        /// </summary>
        private static IEnumerable<IReactivePropertyChangedEventArgs<TSender>> Dedup<TSender>(IList<IReactivePropertyChangedEventArgs<TSender>> batch)
        {
            if (batch.Count <= 1)
            {
                return batch;
            }

            var seen = new HashSet<string>();
            var unique = new LinkedList<IReactivePropertyChangedEventArgs<TSender>>();

            for (int i = batch.Count - 1; i >= 0; i--)
            {
                if (seen.Add(batch[i].PropertyName))
                {
                    unique.AddFirst(batch[i]);
                }
            }

            return unique;
        }

        private class ExtensionState<TSender> : IExtensionState<TSender>
            where TSender : IReactiveObject
        {
            private readonly Lazy<ISubject<Exception>> _thrownExceptions = new Lazy<ISubject<Exception>>(() => new ScheduledSubject<Exception>(Scheduler.Immediate, RxApp.DefaultExceptionHandler));
            private readonly Lazy<Subject<Unit>> _startDelayNotifications = new Lazy<Subject<Unit>>();
            private readonly TSender _sender;
            private readonly Lazy<(ISubject<IReactivePropertyChangedEventArgs<TSender>> subject, IObservable<IReactivePropertyChangedEventArgs<TSender>> observable)> _changing;
            private readonly Lazy<(ISubject<IReactivePropertyChangedEventArgs<TSender>> subject, IObservable<IReactivePropertyChangedEventArgs<TSender>> observable)> _changed;

            private long _changeNotificationsSuppressed;
            private long _changeNotificationsDelayed;

            /// <summary>
            /// Initializes a new instance of the <see cref="ExtensionState{TSender}"/> class.
            /// </summary>
            /// <param name="sender">The sender.</param>
            public ExtensionState(TSender sender)
            {
                _sender = sender;
                _changing = new Lazy<(ISubject<IReactivePropertyChangedEventArgs<TSender>>, IObservable<IReactivePropertyChangedEventArgs<TSender>>)>(() =>
                {
                    var changingSubject = new Subject<IReactivePropertyChangedEventArgs<TSender>>();
                    var changedObs = changingSubject
                        .Buffer(
                            Observable.Merge(
                                changingSubject.Where(_ => !AreChangeNotificationsDelayed()).Select(_ => Unit.Default), _startDelayNotifications.Value))
                        .SelectMany(Dedup)
                        .Publish()
                        .RefCount();

                    return (changingSubject, changedObs);
                });

                _changed = new Lazy<(ISubject<IReactivePropertyChangedEventArgs<TSender>> subject, IObservable<IReactivePropertyChangedEventArgs<TSender>> observable)>(() =>
                {
                    var changedSubject = new Subject<IReactivePropertyChangedEventArgs<TSender>>();
                    var changedObs = changedSubject
                        .Buffer(
                            Observable.Merge(
                                changedSubject.Where(_ => !AreChangeNotificationsDelayed()).Select(_ => Unit.Default), _startDelayNotifications.Value))
                        .SelectMany(Dedup)
                        .Publish()
                        .RefCount();

                    return (changedSubject, changedObs);
                });
            }

            public IObservable<IReactivePropertyChangedEventArgs<TSender>> Changing => _changing.Value.observable;

            public IObservable<IReactivePropertyChangedEventArgs<TSender>> Changed => _changed.Value.observable;

            public IObservable<Exception> ThrownExceptions => _thrownExceptions.Value;

            public bool AreChangeNotificationsEnabled()
            {
                return Interlocked.Read(ref _changeNotificationsSuppressed) == 0;
            }

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
                    if (_startDelayNotifications.IsValueCreated)
                    {
                        _startDelayNotifications.Value.OnNext(Unit.Default);
                    }
                }

                return Disposable.Create(() =>
                {
                    if (Interlocked.Decrement(ref _changeNotificationsDelayed) == 0)
                    {
                        if (_startDelayNotifications.IsValueCreated)
                        {
                            _startDelayNotifications.Value.OnNext(Unit.Default);
                        }
                    }
                });
            }

            public void RaisePropertyChanging(string propertyName)
            {
                if (!AreChangeNotificationsEnabled())
                {
                    return;
                }

                var changing = new ReactivePropertyChangingEventArgs<TSender>(_sender, propertyName);
                _sender.RaisePropertyChanging(changing);

                if (_changing.IsValueCreated)
                {
                    NotifyObservable(_sender, changing, _changing?.Value.subject);
                }
            }

            public void RaisePropertyChanged(string propertyName)
            {
                if (!AreChangeNotificationsEnabled())
                {
                    return;
                }

                var changed = new ReactivePropertyChangedEventArgs<TSender>(_sender, propertyName);
                _sender.RaisePropertyChanged(changed);

                if (_changed.IsValueCreated)
                {
                    NotifyObservable(_sender, changed, _changed.Value.subject);
                }
            }

            internal void NotifyObservable<T>(TSender rxObj, T item, ISubject<T> subject)
            {
                try
                {
                    subject.OnNext(item);
                }
                catch (Exception ex)
                {
                    rxObj.Log().Error(ex, "ReactiveObject Subscriber threw exception");
                    if (_thrownExceptions.IsValueCreated)
                    {
                        _thrownExceptions.Value.OnNext(ex);
                        return;
                    }

                    throw;
                }
            }
        }
    }
}
