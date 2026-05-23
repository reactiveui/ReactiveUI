// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;

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
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Legacy naming convention")]
public class INPCObservableForProperty : ICreatesObservableForProperty
{
    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
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

        var target = beforeChanged ? typeof(INotifyPropertyChanging) : typeof(INotifyPropertyChanged);
        return target.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) ? BindingAffinity.Explicit : 0;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName) =>
        GetNotificationForProperty(sender, expression, propertyName, false, false);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
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

        var expectedName = expression.NodeType == ExpressionType.Index ? propertyName + "[]" : propertyName;

        if (beforeChanged && sender is INotifyPropertyChanging before)
        {
            return new BeforeChangeNotification(before, sender, expression, expectedName);
        }

        if (sender is INotifyPropertyChanged after)
        {
            return new ChangeNotification(after, sender, expression, expectedName);
        }

        return NeverObservable<IObservedChange<object?, object?>>.Instance;
    }

    /// <summary>
    /// Determines whether a notified property name matches the observed property (an empty name means "all properties").
    /// </summary>
    /// <param name="notifiedName">The property name carried by the notification.</param>
    /// <param name="expectedName">The observed property name.</param>
    /// <returns><see langword="true"/> if the notification applies to the observed property.</returns>
    private static bool Matches(string? notifiedName, string expectedName) =>
        string.IsNullOrEmpty(notifiedName) ||
        string.Equals(notifiedName, expectedName, StringComparison.InvariantCulture);

    /// <summary>
    /// A single-layer observable over <see cref="INotifyPropertyChanged.PropertyChanged"/>: each subscription attaches
    /// a handler that filters by name and emits the observed change directly, with no intermediate operators.
    /// </summary>
    /// <param name="notifier">The change notifier to hook.</param>
    /// <param name="sender">The object surfaced on the observed change.</param>
    /// <param name="expression">The expression surfaced on the observed change.</param>
    /// <param name="expectedName">The observed property name.</param>
    private sealed class ChangeNotification(
        INotifyPropertyChanged notifier,
        object sender,
        Expression expression,
        string expectedName) : IObservable<IObservedChange<object?, object?>>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IObservedChange<object?, object?>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Subscription(notifier, sender, expression, expectedName, observer);
        }

        /// <summary>Attaches the property-changed handler for the lifetime of the subscription.</summary>
        private sealed class Subscription : IDisposable
        {
            /// <summary>The change notifier this subscription is hooked to.</summary>
            private readonly INotifyPropertyChanged _notifier;

            /// <summary>The object surfaced on the observed change.</summary>
            private readonly object _sender;

            /// <summary>The expression surfaced on the observed change.</summary>
            private readonly Expression _expression;

            /// <summary>The observed property name.</summary>
            private readonly string _expectedName;

            /// <summary>The observer receiving observed changes.</summary>
            private readonly IObserver<IObservedChange<object?, object?>> _observer;

            /// <summary>Initializes a new instance of the <see cref="Subscription"/> class and hooks the event.</summary>
            /// <param name="notifier">The change notifier to hook.</param>
            /// <param name="sender">The object surfaced on the observed change.</param>
            /// <param name="expression">The expression surfaced on the observed change.</param>
            /// <param name="expectedName">The observed property name.</param>
            /// <param name="observer">The observer receiving observed changes.</param>
            public Subscription(
                INotifyPropertyChanged notifier,
                object sender,
                Expression expression,
                string expectedName,
                IObserver<IObservedChange<object?, object?>> observer)
            {
                _notifier = notifier;
                _sender = sender;
                _expression = expression;
                _expectedName = expectedName;
                _observer = observer;
                _notifier.PropertyChanged += OnPropertyChanged;
            }

            /// <inheritdoc/>
            public void Dispose() => _notifier.PropertyChanged -= OnPropertyChanged;

            /// <summary>Filters the changed property name and forwards a matching observed change.</summary>
            /// <param name="sender">The event sender.</param>
            /// <param name="e">The property-changed event arguments.</param>
            private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
            {
                if (!Matches(e.PropertyName, _expectedName))
                {
                    return;
                }

                _observer.OnNext(new ObservedChange<object?, object?>(_sender, _expression, null));
            }
        }
    }

    /// <summary>
    /// A single-layer observable over <see cref="INotifyPropertyChanging.PropertyChanging"/>: each subscription attaches
    /// a handler that filters by name and emits the observed change directly, with no intermediate operators.
    /// </summary>
    /// <param name="notifier">The change notifier to hook.</param>
    /// <param name="sender">The object surfaced on the observed change.</param>
    /// <param name="expression">The expression surfaced on the observed change.</param>
    /// <param name="expectedName">The observed property name.</param>
    private sealed class BeforeChangeNotification(
        INotifyPropertyChanging notifier,
        object sender,
        Expression expression,
        string expectedName) : IObservable<IObservedChange<object?, object?>>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IObservedChange<object?, object?>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Subscription(notifier, sender, expression, expectedName, observer);
        }

        /// <summary>Attaches the property-changing handler for the lifetime of the subscription.</summary>
        private sealed class Subscription : IDisposable
        {
            /// <summary>The change notifier this subscription is hooked to.</summary>
            private readonly INotifyPropertyChanging _notifier;

            /// <summary>The object surfaced on the observed change.</summary>
            private readonly object _sender;

            /// <summary>The expression surfaced on the observed change.</summary>
            private readonly Expression _expression;

            /// <summary>The observed property name.</summary>
            private readonly string _expectedName;

            /// <summary>The observer receiving observed changes.</summary>
            private readonly IObserver<IObservedChange<object?, object?>> _observer;

            /// <summary>Initializes a new instance of the <see cref="Subscription"/> class and hooks the event.</summary>
            /// <param name="notifier">The change notifier to hook.</param>
            /// <param name="sender">The object surfaced on the observed change.</param>
            /// <param name="expression">The expression surfaced on the observed change.</param>
            /// <param name="expectedName">The observed property name.</param>
            /// <param name="observer">The observer receiving observed changes.</param>
            public Subscription(
                INotifyPropertyChanging notifier,
                object sender,
                Expression expression,
                string expectedName,
                IObserver<IObservedChange<object?, object?>> observer)
            {
                _notifier = notifier;
                _sender = sender;
                _expression = expression;
                _expectedName = expectedName;
                _observer = observer;
                _notifier.PropertyChanging += OnPropertyChanging;
            }

            /// <inheritdoc/>
            public void Dispose() => _notifier.PropertyChanging -= OnPropertyChanging;

            /// <summary>Filters the changing property name and forwards a matching observed change.</summary>
            /// <param name="sender">The event sender.</param>
            /// <param name="e">The property-changing event arguments.</param>
            private void OnPropertyChanging(object? sender, PropertyChangingEventArgs e)
            {
                if (!Matches(e.PropertyName, _expectedName))
                {
                    return;
                }

                _observer.OnNext(new ObservedChange<object?, object?>(_sender, _expression, null));
            }
        }
    }
}
