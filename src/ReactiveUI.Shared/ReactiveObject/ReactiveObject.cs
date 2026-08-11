// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

using System.Diagnostics;

#if !MONO
using System.ComponentModel.DataAnnotations;
#endif

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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
[DebuggerDisplay("{Changing}, {Changed}")]
public class ReactiveObject : IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors, IReactiveObject, IReactiveObjectStateSlot
{
    /// <summary>Tracks whether PropertyChanging event subscriptions have been initialized.</summary>
    private bool _propertyChangingEventsSubscribed;

    /// <summary>Tracks whether PropertyChanged event subscriptions have been initialized.</summary>
    private bool _propertyChangedEventsSubscribed;

    /// <summary>Backing handler for the PropertyChanging event.</summary>
    private PropertyChangingEventHandler? _propertyChangingHandler;

    /// <summary>Backing handler for the PropertyChanged event.</summary>
    private PropertyChangedEventHandler? _propertyChangedHandler;

    /// <summary>Stores the property-changing observable.</summary>
    private IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>? _changing;

    /// <summary>Stores the property-changed observable.</summary>
    private IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>? _changed;

    /// <summary>Stores the exception observable.</summary>
    private IObservable<Exception>? _thrownExceptions;

    /// <summary>Stores this instance's extension state directly, avoiding a table lookup.</summary>
    [IgnoreDataMember]
    [SuppressMessage("Design", "SST1424:Make field readonly", Justification = "Mutated in place through the ref returned by GetReactiveStateSlot.")]
    private object? _reactiveStateSlot;

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging
    {
        add => ReactiveNotificationHelpers.AddPropertyChanging(this, ref _propertyChangingEventsSubscribed, ref _propertyChangingHandler, value);
        remove => _propertyChangingHandler -= value;
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => ReactiveNotificationHelpers.AddPropertyChanged(this, ref _propertyChangedEventsSubscribed, ref _propertyChangedHandler, value);
        remove => _propertyChangedHandler -= value;
    }

    /// <inheritdoc />
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing =>
        ReactiveNotificationHelpers.GetChanging(this, ref _changing);

    /// <inheritdoc />
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed =>
        ReactiveNotificationHelpers.GetChanged(this, ref _changed);

    /// <inheritdoc/>
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<Exception> ThrownExceptions =>
        ReactiveNotificationHelpers.GetThrownExceptions(this, ref _thrownExceptions);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) =>
        _propertyChangingHandler?.Invoke(this, args);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) =>
        _propertyChangedHandler?.Invoke(this, args);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <summary>Determines if change notifications are enabled or not.</summary>
    /// <returns>A value indicating whether change notifications are enabled.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AreChangeNotificationsEnabled() => IReactiveObjectExtensions.AreChangeNotificationsEnabled(this);

    /// <summary>Delays notifications until the return IDisposable is disposed.</summary>
    /// <returns>A disposable which when disposed will send delayed notifications.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable DelayChangeNotifications() =>
        IReactiveObjectExtensions.DelayChangeNotifications(this);

    /// <inheritdoc/>
    ref object? IReactiveObjectStateSlot.GetReactiveStateSlot() => ref _reactiveStateSlot;
}
