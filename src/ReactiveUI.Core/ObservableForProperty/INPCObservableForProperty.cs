// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ReactiveUI;

/// <summary>
/// Provides an implementation of property change notification observation for objects implementing either
/// INotifyPropertyChanged or INotifyPropertyChanging.
/// </summary>
/// <remarks>This class enables the creation of observables that emit notifications when a property value changes
/// or is about to change on objects that support the standard .NET property change notification interfaces. It is
/// typically used in reactive programming scenarios to monitor property changes in data-binding or MVVM patterns.
/// Reflection is used to inspect runtime types, which may have implications for trimming or ahead-of-time (AOT)
/// compilation.</remarks>
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Legacy naming convention")]
public class INPCObservableForProperty : ICreatesObservableForProperty
{
    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetAffinityForObject(Type type, string propertyName) =>
        GetAffinityForObject(type, propertyName, false);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type? type, string propertyName, bool beforeChanged)
    {
        if (type is null)
        {
            return 0;
        }

        return (beforeChanged ? typeof(INotifyPropertyChanging) : typeof(INotifyPropertyChanged))
            .GetTypeInfo()
            .IsAssignableFrom(type.GetTypeInfo())
            ? BindingAffinity.Explicit
            : 0;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName) =>
        GetNotificationForProperty(sender, expression, propertyName, false, false);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged) =>
        GetNotificationForProperty(sender, expression, propertyName, beforeChanged, false);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged,
        bool suppressWarnings)
    {
        ArgumentExceptionHelper.ThrowIfNull(expression);

        var observedName = expression.NodeType == ExpressionType.Index ? $"{propertyName}[]" : propertyName;

        if (beforeChanged && sender is INotifyPropertyChanging before)
        {
            return new BeforeChangeNotification(before, sender, expression, observedName);
        }

        return sender is INotifyPropertyChanged after
            ? new ChangeNotification(after, sender, expression, observedName)
            : Signal.Silent<IObservedChange<object?, object?>>();
    }

    /// <summary>
    /// A single-layer observable over <see cref="INotifyPropertyChanged.PropertyChanged"/>: each subscription attaches
    /// a handler that filters by name and emits the observed change directly, with no intermediate operators.
    /// </summary>
    /// <param name="notifier">The change notifier to hook.</param>
    /// <param name="sender">The object surfaced on the observed change.</param>
    /// <param name="expression">The expression surfaced on the observed change.</param>
    /// <param name="observedName">The observed property name.</param>
    private sealed class ChangeNotification(
        INotifyPropertyChanged notifier,
        object sender,
        Expression expression,
        string observedName) : IObservable<IObservedChange<object?, object?>>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IObservedChange<object?, object?>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Subscription(notifier, sender, expression, observedName, observer);
        }

        /// <summary>Attaches the property-changed handler for the lifetime of the subscription.</summary>
        private sealed class Subscription : ObservedChangeForwarder, IDisposable
        {
            /// <summary>The change notifier this subscription is hooked to.</summary>
            private readonly INotifyPropertyChanged _notifier;

            /// <summary>Initializes a new instance of the <see cref="Subscription"/> class and hooks the event.</summary>
            /// <param name="notifier">The change notifier to hook.</param>
            /// <param name="sender">The object surfaced on the observed change.</param>
            /// <param name="expression">The expression surfaced on the observed change.</param>
            /// <param name="observedName">The observed property name.</param>
            /// <param name="observer">The observer receiving observed changes.</param>
            public Subscription(
                INotifyPropertyChanged notifier,
                object sender,
                Expression expression,
                string observedName,
                IObserver<IObservedChange<object?, object?>> observer)
                : base(sender, expression, observedName, observer)
            {
                _notifier = notifier;
                _notifier.PropertyChanged += OnPropertyChanged;
            }

            /// <inheritdoc/>
            public void Dispose() => _notifier.PropertyChanged -= OnPropertyChanged;

            /// <summary>Filters the changed property name and forwards a matching observed change.</summary>
            /// <param name="sender">The event sender.</param>
            /// <param name="e">The property-changed event arguments.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) => Forward(e.PropertyName);
        }
    }

    /// <summary>
    /// A single-layer observable over <see cref="INotifyPropertyChanging.PropertyChanging"/>: each subscription attaches
    /// a handler that filters by name and emits the observed change directly, with no intermediate operators.
    /// </summary>
    /// <param name="notifier">The change notifier to hook.</param>
    /// <param name="sender">The object surfaced on the observed change.</param>
    /// <param name="expression">The expression surfaced on the observed change.</param>
    /// <param name="observedName">The observed property name.</param>
    private sealed class BeforeChangeNotification(
        INotifyPropertyChanging notifier,
        object sender,
        Expression expression,
        string observedName) : IObservable<IObservedChange<object?, object?>>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IObservedChange<object?, object?>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Subscription(notifier, sender, expression, observedName, observer);
        }

        /// <summary>Attaches the property-changing handler for the lifetime of the subscription.</summary>
        private sealed class Subscription : ObservedChangeForwarder, IDisposable
        {
            /// <summary>The change notifier this subscription is hooked to.</summary>
            private readonly INotifyPropertyChanging _notifier;

            /// <summary>Initializes a new instance of the <see cref="Subscription"/> class and hooks the event.</summary>
            /// <param name="notifier">The change notifier to hook.</param>
            /// <param name="sender">The object surfaced on the observed change.</param>
            /// <param name="expression">The expression surfaced on the observed change.</param>
            /// <param name="observedName">The observed property name.</param>
            /// <param name="observer">The observer receiving observed changes.</param>
            public Subscription(
                INotifyPropertyChanging notifier,
                object sender,
                Expression expression,
                string observedName,
                IObserver<IObservedChange<object?, object?>> observer)
                : base(sender, expression, observedName, observer)
            {
                _notifier = notifier;
                _notifier.PropertyChanging += OnPropertyChanging;
            }

            /// <inheritdoc/>
            public void Dispose() => _notifier.PropertyChanging -= OnPropertyChanging;

            /// <summary>Filters the changing property name and forwards a matching observed change.</summary>
            /// <param name="sender">The event sender.</param>
            /// <param name="e">The property-changing event arguments.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void OnPropertyChanging(object? sender, PropertyChangingEventArgs e) => Forward(e.PropertyName);
        }
    }

    /// <summary>
    /// The per-subscription state shared by the changed and changing hooks: the observer, the observed property name,
    /// and the single projected change reused for every notification. Derived subscriptions supply only the event
    /// hook-up and tear-down, and call <see cref="Forward"/> from their handler.
    /// </summary>
    /// <param name="sender">The object surfaced on the observed change.</param>
    /// <param name="expression">The expression surfaced on the observed change.</param>
    /// <param name="observedName">The observed property name.</param>
    /// <param name="observer">The observer receiving observed changes.</param>
    private class ObservedChangeForwarder(
        object sender,
        Expression expression,
        string observedName,
        IObserver<IObservedChange<object?, object?>> observer)
    {
        /// <summary>The projected change is constant for this subscription (fixed sender/expression, lazily-read
        /// null value), so it is built once and reused rather than allocated on every matching notification.</summary>
        private readonly IObservedChange<object?, object?> _change = new ObservedChange<object?, object?>(sender, expression, null);

        /// <summary>Forwards the reused observed change when the notification applies to the observed property.</summary>
        /// <param name="notifiedName">
        /// The property name carried by the notification. An empty or <see langword="null"/> name means "every
        /// property", so it matches whatever this subscription observes.
        /// </param>
        protected void Forward(string? notifiedName)
        {
            if (!string.IsNullOrEmpty(notifiedName)
                && !string.Equals(notifiedName, observedName, StringComparison.InvariantCulture))
            {
                return;
            }

            observer.OnNext(_change);
        }
    }
}
