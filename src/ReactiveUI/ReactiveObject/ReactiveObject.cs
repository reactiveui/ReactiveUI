// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
#if WINUI3UWP
using Microsoft.UI.Xaml.Data;
#else
using System.ComponentModel;
#endif
using System.Reactive;
using System.Runtime.Serialization;
using System.Threading;
#if WINUI3UWP
using System.Runtime.InteropServices.WindowsRuntime;
#endif

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

#if WINUI3UWP
        private EventRegistrationTokenTable<PropertyChangedEventHandler>
    _propChangedTokenTable = null;
#endif

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
                                                        },
                                                        LazyThreadSafetyMode.PublicationOnly);
            _propertyChangedEventsSubscribed = new Lazy<Unit>(
                                                        () =>
                                                        {
                                                            this.SubscribePropertyChangedEvents();
                                                            return Unit.Default;
                                                        },
                                                        LazyThreadSafetyMode.PublicationOnly);
            _thrownExceptions = new Lazy<IObservable<Exception>>(this.GetThrownExceptionsObservable, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <inheritdoc/>
        public event System.ComponentModel.PropertyChangingEventHandler? PropertyChanging
        {
            add
            {
                _ = _propertyChangingEventsSubscribed.Value;
                PropertyChangingHandler += value;
            }
            remove => PropertyChangingHandler -= value;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged
        {
#if WINUI3UWP
            add
            {
                _ = _propertyChangedEventsSubscribed.Value;
                return EventRegistrationTokenTable<PropertyChangedEventHandler>
                     .GetOrCreateEventRegistrationTokenTable(ref _propChangedTokenTable)
                     .AddEventHandler(value);
            }
            remove => EventRegistrationTokenTable<PropertyChangedEventHandler>
                   .GetOrCreateEventRegistrationTokenTable(ref _propChangedTokenTable)
                   .RemoveEventHandler(value);
#else
            add
            {
                _ = _propertyChangedEventsSubscribed.Value;
                PropertyChangedHandler += value;
            }
            remove => PropertyChangedHandler -= value;
#endif
        }

        private event System.ComponentModel.PropertyChangingEventHandler? PropertyChangingHandler;

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
        void IReactiveObject.RaisePropertyChanging(System.ComponentModel.PropertyChangingEventArgs args) => PropertyChangingHandler?.Invoke(this, args);

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
