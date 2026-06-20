// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

#if !MONO
using System.ComponentModel.DataAnnotations;
#endif

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// ReactiveObject is the base object for ViewModel classes, and it
/// implements INotifyPropertyChanged. In addition, ReactiveObject provides
/// Changing and Changed Observables to monitor object changes.
/// </summary>
[DataContract]
public abstract record ReactiveRecord : IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors, IReactiveObject
{
    /// <summary>Tracks whether property-changing event subscriptions have been set up.</summary>
    private bool _propertyChangingEventsSubscribed;

    /// <summary>Tracks whether property-changed event subscriptions have been set up.</summary>
    private bool _propertyChangedEventsSubscribed;

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging
    {
        add
        {
            if (!_propertyChangingEventsSubscribed)
            {
                this.SubscribePropertyChangingEvents();
                _propertyChangingEventsSubscribed = true;
            }

            PropertyChangingHandler += value;
        }
        remove => PropertyChangingHandler -= value;
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add
        {
            if (!_propertyChangedEventsSubscribed)
            {
                this.SubscribePropertyChangedEvents();
                _propertyChangedEventsSubscribed = true;
            }

            PropertyChangedHandler += value;
        }
        remove => PropertyChangedHandler -= value;
    }

    /// <summary>Backing event store for property-changing notifications.</summary>
    [SuppressMessage("Roslynator", "RCS1159:Use EventHandler<T>", Justification = "Long term design.")]
    [SuppressMessage(
        "Major Code Smell",
        "S3908:Generic event handlers should be used",
        Justification = "Backs the INotifyPropertyChanging.PropertyChanging interface event.")]
    private event PropertyChangingEventHandler? PropertyChangingHandler;

    /// <summary>Backing event store for property-changed notifications.</summary>
    [SuppressMessage("Roslynator", "RCS1159:Use EventHandler<T>", Justification = "Long term design.")]
    [SuppressMessage(
        "Major Code Smell",
        "S3908:Generic event handlers should be used",
        Justification = "Backs the INotifyPropertyChanged.PropertyChanged interface event.")]
    private event PropertyChangedEventHandler? PropertyChangedHandler;

    /// <inheritdoc />
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing =>
        Volatile.Read(ref field) ??
        Interlocked.CompareExchange(ref field, ((IReactiveObject)this).GetChangingObservable(), null) ?? field;

    /// <inheritdoc />
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed =>
        Volatile.Read(ref field) ??
        Interlocked.CompareExchange(ref field, ((IReactiveObject)this).GetChangedObservable(), null) ?? field;

    /// <inheritdoc/>
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<Exception> ThrownExceptions => Volatile.Read(ref field) ??
                                                      Interlocked.CompareExchange(
                                                          ref field,
                                                          this.GetThrownExceptionsObservable(),
                                                          null) ?? field;

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) =>
        PropertyChangingHandler?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) =>
        PropertyChangedHandler?.Invoke(this, args);

    /// <inheritdoc/>
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <summary>Determines if change notifications are enabled or not.</summary>
    /// <returns>A value indicating whether change notifications are enabled.</returns>
    public bool AreChangeNotificationsEnabled() => IReactiveObjectExtensions.AreChangeNotificationsEnabled(this);

    /// <summary>Delays notifications until the return IDisposable is disposed.</summary>
    /// <returns>A disposable which when disposed will send delayed notifications.</returns>
    public IDisposable DelayChangeNotifications() => IReactiveObjectExtensions.DelayChangeNotifications(this);
}
