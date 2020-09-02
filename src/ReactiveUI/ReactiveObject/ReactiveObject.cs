﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Runtime.Serialization;
using System.Threading;

namespace ReactiveUI
{
    /// <summary>
    /// ReactiveObject is the base object for ViewModel classes, and it
    /// implements INotifyPropertyChanged. In addition, ReactiveObject provides
    /// Changing and Changed Observables to monitor object changes.
    /// </summary>
    [DataContract]
    public class ReactiveObject : IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors, IReactiveObject
    {
        private readonly Lazy<IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>> _changing;
        private readonly Lazy<IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>> _changed;
        private readonly Lazy<Unit> _propertyChangingEventsSubscribed;
        private readonly Lazy<Unit> _propertyChangedEventsSubscribed;
        private readonly Lazy<IObservable<Exception>> _thrownExceptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveObject"/> class.
        /// </summary>
        public ReactiveObject()
        {
            _changing = new Lazy<IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>>(() => ((IReactiveObject)this).GetChangingObservable(), LazyThreadSafetyMode.PublicationOnly);
            _changed = new Lazy<IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>>(() => ((IReactiveObject)this).GetChangedObservable(), LazyThreadSafetyMode.PublicationOnly);
            _propertyChangingEventsSubscribed = new Lazy<Unit>(
                                                        () =>
                                                        {
                                                            this.SubscribePropertyChangingEvents();
                                                            return Unit.Default;
                                                        }, LazyThreadSafetyMode.PublicationOnly);
            _propertyChangedEventsSubscribed = new Lazy<Unit>(
                                                        () =>
                                                        {
                                                            this.SubscribePropertyChangedEvents();
                                                            return Unit.Default;
                                                        }, LazyThreadSafetyMode.PublicationOnly);
            _thrownExceptions = new Lazy<IObservable<Exception>>(this.GetThrownExceptionsObservable, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add
            {
                _ = _propertyChangingEventsSubscribed.Value;
                PropertyChangingHandler += value;
            }
            remove => PropertyChangingHandler -= value;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _ = _propertyChangedEventsSubscribed.Value;
                PropertyChangedHandler += value;
            }
            remove => PropertyChangedHandler -= value;
        }

        private event PropertyChangingEventHandler? PropertyChangingHandler;

        private event PropertyChangedEventHandler? PropertyChangedHandler;

        /// <inheritdoc />
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing => _changing.Value;

        /// <inheritdoc />
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed => _changed.Value;

        /// <inheritdoc/>
        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions => _thrownExceptions.Value;

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChangingHandler?.Invoke(this, args);

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChangedHandler?.Invoke(this, args);

        /// <inheritdoc/>
        public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

        /// <summary>
        /// Determines if change notifications are enabled or not.
        /// </summary>
        /// <returns>A value indicating whether change notifications are enabled.</returns>
        public bool AreChangeNotificationsEnabled() => IReactiveObjectExtensions.AreChangeNotificationsEnabled(this);

        /// <summary>
        /// Delays notifications until the return IDisposable is disposed.
        /// </summary>
        /// <returns>A disposable which when disposed will send delayed notifications.</returns>
        public IDisposable DelayChangeNotifications() => IReactiveObjectExtensions.DelayChangeNotifications(this);
    }
}

// vim: tw=120 ts=4 sw=4 et :
