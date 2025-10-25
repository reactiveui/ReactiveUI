// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// ReactiveObject is the base object for ViewModel classes, and it
/// implements INotifyPropertyChanged. In addition, ReactiveObject provides
/// Changing and Changed Observables to monitor object changes.
/// </summary>
[DataContract]
public class ReactiveObject : IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors, IReactiveObject
{
    private bool _propertyChangingEventsSubscribed;
    private bool _propertyChangedEventsSubscribed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveObject"/> class.
    /// </summary>
    public ReactiveObject()
    {
    }

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

    [SuppressMessage("Roslynator", "RCS1159:Use EventHandler<T>", Justification = "Long term design.")]
    private event PropertyChangingEventHandler? PropertyChangingHandler;

    [SuppressMessage("Roslynator", "RCS1159:Use EventHandler<T>", Justification = "Long term design.")]
    private event PropertyChangedEventHandler? PropertyChangedHandler;

    /// <inheritdoc />
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing =>
        field ?? Interlocked.CompareExchange(ref field, ((IReactiveObject)this).GetChangingObservable(), null) ?? field;

    /// <inheritdoc />
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed =>
        field ?? Interlocked.CompareExchange(ref field, ((IReactiveObject)this).GetChangedObservable(), null) ?? field;

    /// <inheritdoc/>
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<Exception> ThrownExceptions =>
        field ?? Interlocked.CompareExchange(ref field, this.GetThrownExceptionsObservable(), null) ?? field;

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) =>
        PropertyChangingHandler?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) =>
        PropertyChangedHandler?.Invoke(this, args);

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("This method uses reflection to access properties by name.")]
    [RequiresDynamicCode("This method uses reflection to access properties by name.")]
#endif
    public IDisposable SuppressChangeNotifications() => // TODO: Create Test
        IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <summary>
    /// Determines if change notifications are enabled or not.
    /// </summary>
    /// <returns>A value indicating whether change notifications are enabled.</returns>
    public bool AreChangeNotificationsEnabled() => // TODO: Create Test
        IReactiveObjectExtensions.AreChangeNotificationsEnabled(this);

    /// <summary>
    /// Delays notifications until the return IDisposable is disposed.
    /// </summary>
    /// <returns>A disposable which when disposed will send delayed notifications.</returns>
    public IDisposable DelayChangeNotifications() =>
        IReactiveObjectExtensions.DelayChangeNotifications(this);
}
