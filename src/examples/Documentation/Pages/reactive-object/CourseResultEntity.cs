// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.Documentation.ReactiveObjects;

/// <summary>
/// A locked course result that already derives from <see cref="AuditedEntity"/>, the school's data-layer base class.
/// C# allows only one base class, so this type cannot also derive from <see cref="ReactiveObject"/>. Instead it
/// implements <see cref="IReactiveObject"/> itself, the way ReactiveUI's own platform base classes do when they
/// already derive from a UI framework's control class.
/// </summary>
/// <remarks>
/// Implementing <see cref="IReactiveObjectStateSlot"/> as well lets the framework keep this instance's notification
/// state on the instance itself instead of in a shared table; it is optional, but free once you own the class.
/// </remarks>
[System.Diagnostics.DebuggerDisplay("{Course}, Grade = {Grade}")]
public sealed class CourseResultEntity : AuditedEntity, IReactiveNotifyPropertyChanged<CourseResultEntity>, IHandleObservableErrors, IReactiveObject, IReactiveObjectStateSlot
{
    /// <summary>The <see cref="INotifyPropertyChanging.PropertyChanging"/> handlers; subscribing enables the classic event.</summary>
    private PropertyChangingEventHandler? _propertyChanging;

    /// <summary>The <see cref="INotifyPropertyChanged.PropertyChanged"/> handlers; subscribing enables the classic event.</summary>
    private PropertyChangedEventHandler? _propertyChanged;

    /// <summary>This instance's reactive notification state, stored on the instance instead of a shared table.</summary>
    private object? _reactiveStateSlot;

    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging
    {
        add
        {
            this.SubscribePropertyChangingEvents();
            _propertyChanging += value;
        }

        remove => _propertyChanging -= value;
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add
        {
            this.SubscribePropertyChangedEvents();
            _propertyChanged += value;
        }

        remove => _propertyChanged -= value;
    }

    /// <summary>Gets the course the result belongs to.</summary>
    public string Course { get; init; } = string.Empty;

    /// <summary>Gets or sets the recorded grade. The registrar can still correct it before the result is locked.</summary>
    public int Grade
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    public IObservable<IReactivePropertyChangedEventArgs<CourseResultEntity>> Changing => this.GetChangingObservable();

    /// <inheritdoc/>
    public IObservable<IReactivePropertyChangedEventArgs<CourseResultEntity>> Changed => this.GetChangedObservable();

    /// <inheritdoc/>
    public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

    /// <summary>Suppresses notifications until the returned disposable is disposed.</summary>
    /// <returns>A disposable that restores notifications when disposed.</returns>
    public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

    /// <summary>Determines whether change notifications are currently enabled.</summary>
    /// <returns><see langword="true"/> if change notifications are enabled; otherwise, <see langword="false"/>.</returns>
    public bool AreChangeNotificationsEnabled() => IReactiveObjectExtensions.AreChangeNotificationsEnabled(this);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => _propertyChanging?.Invoke(this, args);

    /// <inheritdoc/>
    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => _propertyChanged?.Invoke(this, args);

    /// <inheritdoc/>
    ref object? IReactiveObjectStateSlot.GetReactiveStateSlot() => ref _reactiveStateSlot;
}
