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
public record ReactiveRecord : IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors, IReactiveObject
{
    private bool _propertyChangingEventsSubscribed;
    private bool _propertyChangedEventsSubscribed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveRecord"/> class.
    /// </summary>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("ReactiveRecord constructor uses extension methods that require dynamic code generation")]
    [RequiresUnreferencedCode("ReactiveRecord constructor uses extension methods that may require unreferenced code")]
#endif
    public ReactiveRecord()
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
    public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing => // TODO: Create Test
        Volatile.Read(ref field) ?? Interlocked.CompareExchange(ref field, ((IReactiveObject)this).GetChangingObservable(), null) ?? field;

    /// <inheritdoc />
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed => // TODO: Create Test
        Volatile.Read(ref field) ?? Interlocked.CompareExchange(ref field, ((IReactiveObject)this).GetChangedObservable(), null) ?? field;

    /// <inheritdoc/>
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<Exception> ThrownExceptions => Volatile.Read(ref field) ?? Interlocked.CompareExchange(ref field, this.GetThrownExceptionsObservable(), null) ?? field;

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChangingHandler?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChangedHandler?.Invoke(this, args);

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("SuppressChangeNotifications uses extension methods that require dynamic code generation")]
    [RequiresUnreferencedCode("SuppressChangeNotifications uses extension methods that may require unreferenced code")]
#endif
    public IDisposable SuppressChangeNotifications() => // TODO: Create Test
        IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <summary>
    /// Determines if change notifications are enabled or not.
    /// </summary>
    /// <returns>A value indicating whether change notifications are enabled.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("AreChangeNotificationsEnabled uses extension methods that require dynamic code generation")]
    [RequiresUnreferencedCode("AreChangeNotificationsEnabled uses extension methods that may require unreferenced code")]
#endif
    public bool AreChangeNotificationsEnabled() => // TODO: Create Test
            IReactiveObjectExtensions.AreChangeNotificationsEnabled(this);

    /// <summary>
    /// Delays notifications until the return IDisposable is disposed.
    /// </summary>
    /// <returns>A disposable which when disposed will send delayed notifications.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("DelayChangeNotifications uses extension methods that require dynamic code generation")]
    [RequiresUnreferencedCode("DelayChangeNotifications uses extension methods that may require unreferenced code")]
#endif
    public IDisposable DelayChangeNotifications() => // TODO: Create Test
            IReactiveObjectExtensions.DelayChangeNotifications(this);
}
