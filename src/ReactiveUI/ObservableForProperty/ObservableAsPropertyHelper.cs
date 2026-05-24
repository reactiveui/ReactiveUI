// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;
using Splat;

namespace ReactiveUI;

/// <summary>
/// ObservableAsPropertyHelper is a class to help ViewModels implement
/// "output properties", that is, a property that is backed by an
/// Observable. The property will be read-only, but will still fire change
/// notifications. This class can be created directly, but is more often created
/// via the <see cref="OAPHCreationHelperMixin" /> extension methods.
/// </summary>
/// <remarks>
/// <para>
/// Use this helper when the value for a property is derived from one or more observable streams (for example,
/// <c>WhenAnyValue</c>). The helper subscribes to the source observable, tracks the latest value, and raises change
/// notifications through the supplied callbacks.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// _fullName = this.WhenAnyValue(x => x.FirstName, x => x.LastName,
///         (first, last) => $"{first} {last}")
///     .ToProperty(this, x => x.FullName);
///
/// public string FullName => _fullName.Value;
/// ]]>
/// </code>
/// </example>
/// <typeparam name="T">The type.</typeparam>
public sealed class ObservableAsPropertyHelper<T> : IHandleObservableErrors, IDisposable, IEnableLogger
{
    /// <summary>Guards the distinct/skip state and the exception observer state.</summary>
    #if NET9_0_OR_GREATER
    private readonly Lock _gate = new();
    #else
    private readonly object _gate = new();
    #endif

    /// <summary>The scheduler on which value change notifications are delivered.</summary>
    private readonly IScheduler _scheduler;

    /// <summary>Callback invoked before a new value is stored.</summary>
    private readonly Action<T?> _onChanging;

    /// <summary>Callback invoked after a new value is stored.</summary>
    private readonly Action<T?> _onChanged;

    /// <summary>Function that returns the initial value of the property.</summary>
    private readonly Func<T?> _getInitialValue;

    /// <summary>Whether leading values equal to the initial value are skipped (deferred subscription).</summary>
    private readonly bool _skipInitial;

    /// <summary>The raw source observable the helper subscribes to internally (without the initial-value seed).</summary>
    private readonly IObservable<T?> _sourceObservable;

    /// <summary>Broadcasts exceptions thrown during property change handling.</summary>
    [SuppressMessage("Major Code Smell", "S3459:Unassigned members should be removed", Justification = "Mutated in place via Broadcaster methods.")]
    private Broadcaster<Exception> _exceptions;

    /// <summary>Lazily-created observable wrapper over <see cref="_exceptions"/>.</summary>
    private ExceptionStream? _thrownExceptions;

    /// <summary>The most recently produced value from the observable source.</summary>
    private T? _lastValue;

    /// <summary>The previous value seen by the distinct-until-changed gate (source thread).</summary>
    private T? _distinctPrevious;

    /// <summary>Whether <see cref="_distinctPrevious"/> holds a value yet.</summary>
    private bool _hasDistinctPrevious;

    /// <summary>The subscription to the source observable.</summary>
    private IDisposable? _sourceSubscription;

    /// <summary>Flag indicating whether the source observable subscription has been activated.</summary>
    private int _activated;

    /// <summary>Latched once this helper has been disposed.</summary>
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class with no initial value, no deferred subscription, and no scheduler.
    /// </summary>
    /// <param name="observable">The observable to base the property on.</param>
    /// <param name="onChanged">The action called when the property value changes.</param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged)
        : this(observable, onChanged, null, default(T?), false, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class with a specified initial value.
    /// </summary>
    /// <param name="observable">The observable to base the property on.</param>
    /// <param name="onChanged">The action called when the property value changes.</param>
    /// <param name="initialValue">The initial value of the property.</param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        T? initialValue)
        : this(observable, onChanged, null, initialValue, false, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class with a specified initial value and scheduler.
    /// </summary>
    /// <param name="observable">The observable to base the property on.</param>
    /// <param name="onChanged">The action called when the property value changes.</param>
    /// <param name="initialValue">The initial value of the property.</param>
    /// <param name="scheduler">The scheduler on which change notifications are delivered.</param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        T? initialValue,
        IScheduler? scheduler)
        : this(observable, onChanged, null, () => initialValue, false, scheduler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class with a specified initial value and deferred subscription flag.
    /// </summary>
    /// <param name="observable">The observable to base the property on.</param>
    /// <param name="onChanged">The action called when the property value changes.</param>
    /// <param name="initialValue">The initial value of the property.</param>
    /// <param name="deferSubscription">When true, defers source subscription until the first read of Value.</param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        T? initialValue,
        bool deferSubscription)
        : this(observable, onChanged, null, initialValue, deferSubscription, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class.
    /// </summary>
    /// <param name="observable">The observable to base the property on.</param>
    /// <param name="onChanged">The action called when the property value changes.</param>
    /// <param name="initialValue">The initial value of the property.</param>
    /// <param name="deferSubscription">When true, defers source subscription until the first read of Value.</param>
    /// <param name="scheduler">The scheduler on which change notifications are delivered.</param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        T? initialValue,
        bool deferSubscription,
        IScheduler? scheduler)
        : this(observable, onChanged, null, initialValue, deferSubscription, scheduler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class with no changing callback, no initial value, no deferred subscription, and no scheduler.
    /// </summary>
    /// <param name="observable">The observable to base the property on.</param>
    /// <param name="onChanged">The action called when the property value changes.</param>
    /// <param name="onChanging">The action called before the property value changes; may be null.</param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        Action<T?>? onChanging)
        : this(observable, onChanged, onChanging, () => default, false, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class with a specified initial value and changing callback.
    /// </summary>
    /// <param name="observable">The observable to base the property on.</param>
    /// <param name="onChanged">The action called when the property value changes.</param>
    /// <param name="onChanging">The action called before the property value changes; may be null.</param>
    /// <param name="initialValue">The initial value of the property.</param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        Action<T?>? onChanging,
        T? initialValue)
        : this(observable, onChanged, onChanging, () => initialValue, false, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class with changing callback, initial value, and deferred subscription flag.
    /// </summary>
    /// <param name="observable">The observable to base the property on.</param>
    /// <param name="onChanged">The action called when the property value changes.</param>
    /// <param name="onChanging">The action called before the property value changes; may be null.</param>
    /// <param name="initialValue">The initial value of the property.</param>
    /// <param name="deferSubscription">When true, defers source subscription until the first read of Value.</param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        Action<T?>? onChanging,
        T? initialValue,
        bool deferSubscription)
        : this(observable, onChanged, onChanging, () => initialValue, deferSubscription, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class with changing callback, initial value, deferred subscription flag, and scheduler.
    /// </summary>
    /// <param name="observable">The observable to base the property on.</param>
    /// <param name="onChanged">The action called when the property value changes.</param>
    /// <param name="onChanging">The action called before the property value changes; may be null.</param>
    /// <param name="initialValue">The initial value of the property.</param>
    /// <param name="deferSubscription">When true, defers source subscription until the first read of Value.</param>
    /// <param name="scheduler">The scheduler on which change notifications are delivered.</param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        Action<T?>? onChanging,
        T? initialValue,
        bool deferSubscription,
        IScheduler? scheduler)
        : this(observable, onChanged, onChanging, () => initialValue, deferSubscription, scheduler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class with no initial value factory, no deferred subscription, and no scheduler.
    /// </summary>
    /// <param name="observable">The observable to base the property on.</param>
    /// <param name="onChanged">The action called when the property value changes.</param>
    /// <param name="onChanging">The action called before the property value changes; may be null.</param>
    /// <param name="getInitialValue">Factory that returns the initial value; null defaults to returning default(T).</param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        Action<T?>? onChanging,
        Func<T?>? getInitialValue)
        : this(observable, onChanged, onChanging, getInitialValue, false, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class with an initial value factory and deferred subscription flag.
    /// </summary>
    /// <param name="observable">The observable to base the property on.</param>
    /// <param name="onChanged">The action called when the property value changes.</param>
    /// <param name="onChanging">The action called before the property value changes; may be null.</param>
    /// <param name="getInitialValue">Factory that returns the initial value; null defaults to returning default(T).</param>
    /// <param name="deferSubscription">When true, defers source subscription until the first read of Value.</param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        Action<T?>? onChanging,
        Func<T?>? getInitialValue,
        bool deferSubscription)
        : this(observable, onChanged, onChanging, getInitialValue, deferSubscription, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class with an initial value factory and deferred subscription flag.
    /// </summary>
    /// <param name="observable">The observable to base the property on.</param>
    /// <param name="onChanged">The action called when the property value changes.</param>
    /// <param name="getInitialValue">Factory that returns the initial value; null defaults to returning default(T).</param>
    /// <param name="deferSubscription">When true, defers source subscription until the first read of Value.</param>
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        Func<T?> getInitialValue,
        bool deferSubscription)
        : this(observable, onChanged, null, getInitialValue, deferSubscription, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAsPropertyHelper{T}"/> class.
    /// </summary>
    /// <param name="observable">The observable to base the property on.</param>
    /// <param name="onChanged">The action called when the property value changes.</param>
    /// <param name="onChanging">The action called before the property value changes; may be null.</param>
    /// <param name="getInitialValue">Factory that returns the initial value; null defaults to returning default(T).</param>
    /// <param name="deferSubscription">When true, defers source subscription until the first read of Value.</param>
    /// <param name="scheduler">The scheduler on which change notifications are delivered.</param>
    [SuppressMessage(
        "Style",
        "IDE0200:Lambda expression can be removed",
        Justification = "Method group would force eager Lazy initialization.")]
    public ObservableAsPropertyHelper(
        IObservable<T?> observable,
        Action<T?> onChanged,
        Action<T?>? onChanging,
        Func<T?>? getInitialValue,
        bool deferSubscription,
        IScheduler? scheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(observable);
        ArgumentExceptionHelper.ThrowIfNull(onChanged);

        _scheduler = scheduler ?? CurrentThreadScheduler.Instance;
        _onChanging = onChanging ?? NoOp;
        _onChanged = onChanged;
        _getInitialValue = getInitialValue ?? GetDefault;
        _skipInitial = deferSubscription;
        _sourceObservable = observable;

        if (deferSubscription)
        {
            return;
        }

        // Eager: seed the initial value (StartWith semantics) and subscribe immediately.
        var initial = _getInitialValue();
        _lastValue = initial;
        _distinctPrevious = initial;
        _hasDistinctPrevious = true;
        ScheduleDeliver(initial);
        _sourceSubscription = _sourceObservable.Subscribe(new SourceObserver(this));
        _activated = 1;
    }

    /// <summary>
    /// Gets the last provided value from the Observable.
    /// </summary>
    public T Value
    {
        get
        {
            if (Interlocked.CompareExchange(ref _activated, 1, 0) == 0 && !_disposed)
            {
                _lastValue = _getInitialValue();
                var subscription = _sourceObservable.Subscribe(new SourceObserver(this));
                lock (_gate)
                {
                    if (_disposed)
                    {
                        subscription.Dispose();
                    }
                    else
                    {
                        _sourceSubscription = subscription;
                    }
                }
            }

            return _lastValue!;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the ObservableAsPropertyHelper
    /// has subscribed to the source Observable.
    /// Useful for scenarios where you use deferred subscription and want to know if
    /// the ObservableAsPropertyHelper Value has been accessed yet.
    /// </summary>
    public bool IsSubscribed => _activated > 0;

    /// <summary>
    /// Gets an observable which signals whenever an exception would normally terminate ReactiveUI
    /// internal state.
    /// </summary>
    public IObservable<Exception> ThrownExceptions => _thrownExceptions ??= new(this);

    /// <summary>
    /// Gets the source observable. In eager mode it is seeded with the current value so a subscriber observes the
    /// latest value immediately (StartWith semantics); in deferred mode it stays raw so no value is emitted (and the
    /// initial-value factory is not accessed) until <see cref="Value"/> is first read.
    /// </summary>
    internal IObservable<T?> Source => _skipInitial ? _sourceObservable : _sourceObservable.StartWith(_lastValue);

    /// <summary>
    /// Constructs a default ObservableAsPropertyHelper with no initial value and no scheduler.
    /// </summary>
    /// <returns>A default property helper.</returns>
    public static ObservableAsPropertyHelper<T> Default() => new(NeverObservable<T>.Instance, static _ => { }, default, false, null);

    /// <summary>
    /// Constructs a default ObservableAsPropertyHelper with the specified initial value and no scheduler.
    /// </summary>
    /// <param name="initialValue">The initial (and only) value of the property.</param>
    /// <returns>A default property helper.</returns>
    public static ObservableAsPropertyHelper<T> Default(T? initialValue) =>
        new(NeverObservable<T>.Instance, static _ => { }, initialValue!, false, null);

    /// <summary>
    /// Constructs a default ObservableAsPropertyHelper with the specified initial value and scheduler.
    /// </summary>
    /// <param name="initialValue">The initial (and only) value of the property.</param>
    /// <param name="scheduler">The scheduler on which change notifications are delivered.</param>
    /// <returns>A default property helper.</returns>
    public static ObservableAsPropertyHelper<T> Default(T? initialValue, IScheduler? scheduler) =>
        new(NeverObservable<T>.Instance, static _ => { }, initialValue!, false, scheduler);

    /// <summary>
    /// Disposes this ObservableAsPropertyHelper.
    /// </summary>
    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }

        _sourceSubscription?.Dispose();
    }

    /// <summary>The no-op onChanging callback used when none is supplied.</summary>
    /// <param name="value">The unused value.</param>
    private static void NoOp(T? value)
    {
        // Intentionally empty: the default onChanging callback does nothing when the caller supplies none.
    }

    /// <summary>Returns the default value of <typeparamref name="T"/>.</summary>
    /// <returns>The default value.</returns>
    private static T? GetDefault() => default;

    /// <summary>Schedules delivery of a value's change notifications on the configured scheduler.</summary>
    /// <param name="value">The value to deliver.</param>
    private void ScheduleDeliver(T? value) =>
        _scheduler.ScheduleOrInline(
            (Helper: this, Value: value),
            static (_, state) =>
            {
                state.Helper.Deliver(state.Value);
                return EmptyDisposable.Instance;
            });

    /// <summary>Runs the onChanging / store / onChanged sequence for a value.</summary>
    /// <param name="value">The value being delivered.</param>
    private void Deliver(T? value)
    {
        _onChanging(value);
        _lastValue = value;
        _onChanged(value);
    }

    /// <summary>Applies the distinct / skip-initial gate to a source value and schedules delivery when it passes.</summary>
    /// <param name="value">The value produced by the source.</param>
    private void OnSourceNext(T? value)
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            if (_skipInitial && !_hasDistinctPrevious && EqualityComparer<T?>.Default.Equals(value, _getInitialValue()))
            {
                return;
            }

            if (_hasDistinctPrevious && EqualityComparer<T?>.Default.Equals(value, _distinctPrevious))
            {
                return;
            }

            _distinctPrevious = value;
            _hasDistinctPrevious = true;
        }

        ScheduleDeliver(value);
    }

    /// <summary>Schedules delivery of a source error to the exceptions stream.</summary>
    /// <param name="error">The error produced by the source.</param>
    private void OnSourceError(Exception error) =>
        CurrentThreadScheduler.Instance.Schedule(
            (Helper: this, Error: error),
            static (_, state) =>
            {
                state.Helper.DeliverException(state.Error);
                return EmptyDisposable.Instance;
            });

    /// <summary>Delivers an exception to subscribers, or to the default handler when there are none.</summary>
    /// <param name="error">The exception to deliver.</param>
    private void DeliverException(Exception error)
    {
        bool hasObservers;
        lock (_gate)
        {
            hasObservers = _exceptions.HasObservers;
        }

        if (hasObservers)
        {
            _exceptions.Next(error);
            return;
        }

        RxState.DefaultExceptionHandler.OnNext(error);
    }

    /// <summary>Adds an exceptions observer.</summary>
    /// <param name="observer">The observer to add.</param>
    private void AddException(IObserver<Exception> observer)
    {
        lock (_gate)
        {
            _exceptions.Add(observer);
        }
    }

    /// <summary>Removes an exceptions observer.</summary>
    /// <param name="observer">The observer to remove.</param>
    private void RemoveException(IObserver<Exception> observer)
    {
        lock (_gate)
        {
            _exceptions.Remove(observer);
        }
    }

    /// <summary>Routes the source observable's notifications into the helper's gate-guarded state.</summary>
    /// <param name="parent">The owning helper.</param>
    private sealed class SourceObserver(ObservableAsPropertyHelper<T> parent) : IObserver<T?>
    {
        /// <inheritdoc/>
        public void OnNext(T? value) => parent.OnSourceNext(value);

        /// <inheritdoc/>
        public void OnError(Exception error) => parent.OnSourceError(error);

        /// <inheritdoc/>
        public void OnCompleted()
        {
        }
    }

    /// <summary>The <see cref="ThrownExceptions"/> stream.</summary>
    /// <param name="parent">The owning helper.</param>
    private sealed class ExceptionStream(ObservableAsPropertyHelper<T> parent) : IObservable<Exception>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<Exception> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            parent.AddException(observer);
            return new ExceptionSubscription(parent, observer);
        }
    }

    /// <summary>Unsubscribes an exceptions observer on dispose.</summary>
    /// <param name="parent">The owning helper.</param>
    /// <param name="observer">The subscribed observer.</param>
    private sealed class ExceptionSubscription(ObservableAsPropertyHelper<T> parent, IObserver<Exception> observer) : IDisposable
    {
        /// <inheritdoc/>
        public void Dispose() => parent.RemoveException(observer);
    }
}
