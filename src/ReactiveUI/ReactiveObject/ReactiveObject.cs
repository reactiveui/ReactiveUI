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
    private readonly Lazy<IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>> _changing;
    private readonly Lazy<IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>> _changed;
    private readonly Lazy<Unit> _propertyChangingEventsSubscribed;
    private readonly Lazy<Unit> _propertyChangedEventsSubscribed;
    private readonly Lazy<IObservable<Exception>> _thrownExceptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveObject"/> class.
    /// </summary>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("ReactiveObject uses extension methods that require dynamic code generation")]
    [RequiresUnreferencedCode("ReactiveObject uses extension methods that may require unreferenced code")]
#endif
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
    public event PropertyChangingEventHandler? PropertyChanging
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
        add
        {
            _ = _propertyChangedEventsSubscribed.Value;
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
    public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing => _changing.Value;

    /// <inheritdoc />
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed => _changed.Value;

    /// <inheritdoc/>
    [IgnoreDataMember]
    [JsonIgnore]
#if !MONO
    [Browsable(false)]
    [Display(Order = -1, AutoGenerateField = false, AutoGenerateFilter = false)]
#endif
    public IObservable<Exception> ThrownExceptions => _thrownExceptions.Value;

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) =>
        PropertyChangingHandler?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) =>
        PropertyChangedHandler?.Invoke(this, args);

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
    public IDisposable DelayChangeNotifications() =>
        IReactiveObjectExtensions.DelayChangeNotifications(this);
}

// vim: tw=120 ts=4 sw=4 et :
