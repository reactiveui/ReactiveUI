// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Foundation;

#if UIKIT
using UIKit;
#else
using AppKit;
#endif

namespace ReactiveUI;

/// <summary>
/// Represents an object that knows how to create notifications for a given type of object.
/// Implement this when porting ReactiveUI to a new UI toolkit, or to enable <c>WhenAny*</c>
/// support for another type that can be observed in a unique way.
/// </summary>
/// <remarks>
/// Implementations typically call <see cref="Register(Type, string, int, Func{NSObject, Expression, IObservable{IObservedChange{object, object?}}})"/>
/// during construction to populate supported properties.
/// </remarks>
[Preserve]
public abstract class ObservableForPropertyBase : ICreatesObservableForProperty
{
    /// <summary>
    /// Message used for <see cref="RequiresUnreferencedCodeAttribute"/> annotations on reflection-based event hookup.
    /// </summary>
    private const string RequiresUnreferencedCodeMessage =
        "String-based event hookup uses reflection over members that may be trimmed.";

    /// <summary>
    /// Message used for <see cref="RequiresDynamicCodeAttribute"/> annotations on reflection-based event hookup.
    /// </summary>
    private const string RequiresDynamicCodeMessage =
        "String-based event hookup may require runtime code generation and is not guaranteed to be AOT-compatible.";

    /// <summary>
    /// Synchronization gate protecting <see cref="_config"/> and <see cref="_version"/>.
    /// </summary>
    private readonly object _gate = new();

    /// <summary>
    /// Configuration map keyed by registered type and then by property name.
    /// </summary>
    private readonly Dictionary<Type, Dictionary<string, ObservablePropertyInfo>> _config = [];

    /// <summary>
    /// Cache of the best matching registration for a runtime type and property name.
    /// </summary>
    /// <remarks>
    /// Entries are versioned so that updates via <see cref="Register"/> invalidate previous results without
    /// requiring global cache clearing under lock.
    /// </remarks>
    private readonly ConcurrentDictionary<(RuntimeTypeHandle Type, string Property), CacheEntry> _bestMatchCache = new();

    /// <summary>
    /// Monotonically increasing version for <see cref="_config"/> used to invalidate <see cref="_bestMatchCache"/>.
    /// </summary>
    private int _version;

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(propertyName);

        if (beforeChanged)
        {
            return 0;
        }

        var match = ResolveBestMatch(type, propertyName);
        return match is null ? 0 : match.Affinity;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged = false,
        bool suppressWarnings = false)
    {
        ArgumentExceptionHelper.ThrowIfNull(sender);
        ArgumentExceptionHelper.ThrowIfNull(expression);
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        if (beforeChanged)
        {
            return Observable<IObservedChange<object, object?>>.Never;
        }

        var type = sender.GetType();
        var match = ResolveBestMatch(type, propertyName);

        if (match is null)
        {
            throw new NotSupportedException($"Notifications for {type.Name}.{propertyName} are not supported");
        }

        // Do not invoke user-provided observable factories under lock.
        return match.CreateObservable.Invoke((NSObject)sender, expression);
    }

#if UIKIT
    /// <summary>
    /// Creates an observable sequence that produces a notification each time the given <see cref="UIControlEvent"/>
    /// is raised by the <paramref name="sender"/>.
    /// </summary>
    /// <param name="sender">The native sender.</param>
    /// <param name="expression">The expression associated with the observed change.</param>
    /// <param name="evt">The control event to listen for.</param>
    /// <returns>An observable sequence of observed changes.</returns>
    protected static IObservable<IObservedChange<object, object?>> ObservableFromUIControlEvent(
        NSObject sender,
        Expression expression,
        UIControlEvent evt) =>
        Observable.Create<IObservedChange<object, object?>>(observer =>
        {
            var control = (UIControl)sender;

            // Stable delegate allows deterministic unsubscription.
            void Handler(object? s, EventArgs e) =>
                observer.OnNext(new ObservedChange<object, object?>(sender, expression, default));

            control.AddTarget(Handler, evt);
            return Disposable.Create(() => control.RemoveTarget(Handler, evt));
        });
#endif

    /// <summary>
    /// Creates an observable sequence that produces a notification each time the specified
    /// <see cref="NSNotificationCenter"/> notification is posted for <paramref name="sender"/>.
    /// </summary>
    /// <param name="sender">The native sender.</param>
    /// <param name="expression">The expression associated with the observed change.</param>
    /// <param name="notification">The notification name.</param>
    /// <returns>An observable sequence of observed changes.</returns>
    protected static IObservable<IObservedChange<object, object?>> ObservableFromNotification(
        NSObject sender,
        Expression expression,
        NSString notification) =>
        Observable.Create<IObservedChange<object, object?>>(observer =>
        {
            var handle = NSNotificationCenter.DefaultCenter.AddObserver(
                notification,
                _ => observer.OnNext(new ObservedChange<object, object?>(sender, expression, default)),
                sender);

            return Disposable.Create(() => NSNotificationCenter.DefaultCenter.RemoveObserver(handle));
        });

    /// <summary>
    /// Creates an observable sequence from an event using reflection-based string event lookup.
    /// </summary>
    /// <remarks>
    /// Prefer the add/remove overloads (for example,
    /// <see cref="ObservableFromEvent{TSender}(TSender, Expression, Action{EventHandler}, Action{EventHandler})"/>)
    /// for trimming/AOT compatibility.
    /// </remarks>
    /// <param name="sender">The native sender.</param>
    /// <param name="expression">The expression associated with the observed change.</param>
    /// <param name="eventName">The event name.</param>
    /// <returns>An observable sequence of observed changes.</returns>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    protected static IObservable<IObservedChange<object, object?>> ObservableFromEvent(
        NSObject sender,
        Expression expression,
        string eventName) =>
        Observable.Create<IObservedChange<object, object?>>(observer =>
            Observable
                .FromEventPattern(sender, eventName)
                .Subscribe(_ => observer.OnNext(new ObservedChange<object, object?>(sender, expression, default))));

    /// <summary>
    /// Creates an observable sequence from an event using explicit add/remove handlers (non-reflection).
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <param name="sender">The sender instance.</param>
    /// <param name="expression">The expression associated with the observed change.</param>
    /// <param name="addHandler">Adds the handler to the event source.</param>
    /// <param name="removeHandler">Removes the handler from the event source.</param>
    /// <returns>An observable sequence of observed changes.</returns>
    protected static IObservable<IObservedChange<object, object?>> ObservableFromEvent<TSender>(
        TSender sender,
        Expression expression,
        Action<EventHandler> addHandler,
        Action<EventHandler> removeHandler)
        where TSender : NSObject =>
        Observable.Create<IObservedChange<object, object?>>(observer =>
        {
            // Stable handler for deterministic unsubscription.
            void Handler(object? s, EventArgs e) =>
                observer.OnNext(new ObservedChange<object, object?>(sender, expression, default));

            addHandler(Handler);
            return Disposable.Create(() => removeHandler(Handler));
        });

    /// <summary>
    /// Creates an observable sequence from a typed event using explicit add/remove handlers (non-reflection).
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TEventArgs">The event args type.</typeparam>
    /// <param name="sender">The sender instance.</param>
    /// <param name="expression">The expression associated with the observed change.</param>
    /// <param name="addHandler">Adds the handler to the event source.</param>
    /// <param name="removeHandler">Removes the handler from the event source.</param>
    /// <returns>An observable sequence of observed changes.</returns>
    protected static IObservable<IObservedChange<object, object?>> ObservableFromEvent<TSender, TEventArgs>(
        TSender sender,
        Expression expression,
        Action<EventHandler<TEventArgs>> addHandler,
        Action<EventHandler<TEventArgs>> removeHandler)
        where TSender : NSObject
        where TEventArgs : EventArgs =>
        Observable.Create<IObservedChange<object, object?>>(observer =>
        {
            // Stable handler for deterministic unsubscription.
            void Handler(object? s, TEventArgs e) =>
                observer.OnNext(new ObservedChange<object, object?>(sender, expression, default));

            addHandler(Handler);
            return Disposable.Create(() => removeHandler(Handler));
        });

    /// <summary>
    /// Registers an observable factory for the specified <paramref name="type"/> and <paramref name="property"/>.
    /// </summary>
    /// <param name="type">The type the property belongs to.</param>
    /// <param name="property">The property name.</param>
    /// <param name="affinity">The affinity score for this registration.</param>
    /// <param name="createObservable">Factory that creates the observable for this property.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="type"/>, <paramref name="property"/>, or <paramref name="createObservable"/> is <see langword="null"/>.
    /// </exception>
    protected void Register(
        Type type,
        string property,
        int affinity,
        Func<NSObject, Expression, IObservable<IObservedChange<object, object?>>> createObservable)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(createObservable);

        lock (_gate)
        {
            if (!_config.TryGetValue(type, out var typeProperties))
            {
                typeProperties = [];
                _config[type] = typeProperties;
            }

            typeProperties[property] = new ObservablePropertyInfo(affinity, createObservable);

            // Invalidate caches by bumping version.
            unchecked
            {
                _version++;
            }
        }
    }

    /// <summary>
    /// Resolves the best registered match for a runtime type and property name, using a versioned cache.
    /// </summary>
    /// <param name="runtimeType">The runtime type to resolve.</param>
    /// <param name="propertyName">The property name to resolve.</param>
    /// <returns>The best matching registration, or <see langword="null"/> if none exists.</returns>
    private ObservablePropertyInfo? ResolveBestMatch(Type runtimeType, string propertyName)
    {
        // Fast path: check cache.
        var key = (runtimeType.TypeHandle, propertyName);
        if (_bestMatchCache.TryGetValue(key, out var cached))
        {
            // If config has not changed since the cached entry was computed, return it.
            if (cached.Version == _version)
            {
                return cached.Info;
            }
        }

        // Slow path: compute under lock against a consistent snapshot of config.
        ObservablePropertyInfo? best = null;
        var versionSnapshot = 0;

        lock (_gate)
        {
            versionSnapshot = _version;

            foreach (var kvp in _config)
            {
                var registeredType = kvp.Key;

                if (!registeredType.IsAssignableFrom(runtimeType))
                {
                    continue;
                }

                if (!kvp.Value.TryGetValue(propertyName, out var info))
                {
                    continue;
                }

                if (best is null || info.Affinity > best.Affinity)
                {
                    best = info;
                }
            }
        }

        // Publish computed value to cache (including null, to avoid repeated scans for unsupported properties).
        _bestMatchCache[key] = new CacheEntry(versionSnapshot, best);
        return best;
    }

    /// <summary>
    /// Represents a cached best-match result for a (runtime type, property) pair.
    /// </summary>
    private readonly record struct CacheEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheEntry"/> struct.
        /// Initializes a new instance of the <see cref="CacheEntry"/> record.
        /// </summary>
        /// <param name="version">The configuration version the entry was computed from.</param>
        /// <param name="info">The resolved property information, or <see langword="null"/> if unsupported.</param>
        public CacheEntry(int version, ObservablePropertyInfo? info) => (Version, Info) = (version, info);

        /// <summary>
        /// Gets the configuration version the entry was computed from.
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// Gets the resolved property information, or <see langword="null"/> if unsupported.
        /// </summary>
        public ObservablePropertyInfo? Info { get; }
    }

    /// <summary>
    /// Describes an observable factory registration for a property, including its affinity.
    /// </summary>
    internal sealed record ObservablePropertyInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservablePropertyInfo"/> class.
        /// </summary>
        /// <param name="affinity">The affinity score for the registration.</param>
        /// <param name="createObservable">The factory for creating the observable for this property.</param>
        public ObservablePropertyInfo(
            int affinity,
            Func<NSObject, Expression, IObservable<IObservedChange<object, object?>>> createObservable) =>
            (Affinity, CreateObservable) = (affinity, createObservable);

        /// <summary>
        /// Gets the affinity score for the registration.
        /// </summary>
        public int Affinity { get; }

        /// <summary>
        /// Gets the observable factory for the registration.
        /// </summary>
        public Func<NSObject, Expression, IObservable<IObservedChange<object, object?>>> CreateObservable { get; }
    }
}
