// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

#if !MONO
using System.ComponentModel.DataAnnotations;
#endif

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

    /// <summary>Backing event store for property-changing notifications.</summary>
    private PropertyChangingEventHandler? _propertyChangingHandler;

    /// <summary>Backing event store for property-changed notifications.</summary>
    private PropertyChangedEventHandler? _propertyChangedHandler;

    /// <summary>Stores the property-changing observable.</summary>
    private IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>? _changing;

    /// <summary>Stores the property-changed observable.</summary>
    private IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>? _changed;

    /// <summary>Stores the exception observable.</summary>
    private IObservable<Exception>? _thrownExceptions;

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging
    {
        add => AddPropertyChanging(value);
        remove => RemovePropertyChanging(value);
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => AddPropertyChanged(value);
        remove => RemovePropertyChanged(value);
    }

    /// <inheritdoc />
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing => GetChanging();

    /// <inheritdoc />
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed => GetChanged();

    /// <inheritdoc/>
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<Exception> ThrownExceptions => GetThrownExceptions();

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) =>
        _propertyChangingHandler?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) =>
        _propertyChangedHandler?.Invoke(this, args);

    /// <inheritdoc/>
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <summary>Determines if change notifications are enabled or not.</summary>
    /// <returns>A value indicating whether change notifications are enabled.</returns>
    public bool AreChangeNotificationsEnabled() => IReactiveObjectExtensions.AreChangeNotificationsEnabled(this);

    /// <summary>Delays notifications until the return IDisposable is disposed.</summary>
    /// <returns>A disposable which when disposed will send delayed notifications.</returns>
    public IDisposable DelayChangeNotifications() => IReactiveObjectExtensions.DelayChangeNotifications(this);

    /// <summary>Adds a property-changing event handler.</summary>
    /// <param name="handler">The handler to add.</param>
    private void AddPropertyChanging(PropertyChangingEventHandler? handler) =>
        ReactiveNotificationHelpers.AddPropertyChanging(this, ref _propertyChangingEventsSubscribed, ref _propertyChangingHandler, handler);

    /// <summary>Removes a property-changing event handler.</summary>
    /// <param name="handler">The handler to remove.</param>
    private void RemovePropertyChanging(PropertyChangingEventHandler? handler) => _propertyChangingHandler -= handler;

    /// <summary>Adds a property-changed event handler.</summary>
    /// <param name="handler">The handler to add.</param>
    private void AddPropertyChanged(PropertyChangedEventHandler? handler) =>
        ReactiveNotificationHelpers.AddPropertyChanged(this, ref _propertyChangedEventsSubscribed, ref _propertyChangedHandler, handler);

    /// <summary>Removes a property-changed event handler.</summary>
    /// <param name="handler">The handler to remove.</param>
    private void RemovePropertyChanged(PropertyChangedEventHandler? handler) => _propertyChangedHandler -= handler;

    /// <summary>Gets the property-changing observable.</summary>
    /// <returns>The property-changing observable.</returns>
    private IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> GetChanging() =>
        ReactiveNotificationHelpers.GetChanging(this, ref _changing);

    /// <summary>Gets the property-changed observable.</summary>
    /// <returns>The property-changed observable.</returns>
    private IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> GetChanged() =>
        ReactiveNotificationHelpers.GetChanged(this, ref _changed);

    /// <summary>Gets the exception observable.</summary>
    /// <returns>The exception observable.</returns>
    private IObservable<Exception> GetThrownExceptions() =>
        ReactiveNotificationHelpers.GetThrownExceptions(this, ref _thrownExceptions);
}
