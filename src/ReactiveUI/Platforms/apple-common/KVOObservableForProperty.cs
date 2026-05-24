// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using Foundation;
using ReactiveUI.Helpers;

namespace ReactiveUI;

/// <summary>
/// Provides change notifications for Cocoa <see cref="NSObject"/> instances using Key-Value Observing (KVO).
/// </summary>
[Preserve(AllMembers = true)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "KVO is an established acronym and the public type name cannot change without a breaking API change.")]
public sealed class KVOObservableForProperty : ICreatesObservableForProperty
{
    private const int KvoAffinityScore = 15;

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type type, string propertyName) =>
        GetAffinityForObject(type, propertyName, false);

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type? type, string propertyName, bool beforeChanged)
    {
        ArgumentExceptionHelper.ThrowIfNull(type);
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        if (!typeof(NSObject).IsAssignableFrom(type))
        {
            return 0;
        }

        return IsDeclaredOnNSObject(type, propertyName) ? KvoAffinityScore : 0;
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName) =>
        GetNotificationForProperty(sender, expression, propertyName, false, false);

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged) =>
        GetNotificationForProperty(sender, expression, propertyName, beforeChanged, false);

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    [SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "suppressWarnings is part of the ICreatesObservableForProperty interface contract.")]
    public IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged,
        bool suppressWarnings)
    {
        ArgumentExceptionHelper.ThrowIfNull(sender);
        ArgumentExceptionHelper.ThrowIfNull(expression);
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        if (sender is not NSObject)
        {
            throw new ArgumentException("Sender must be an NSObject.", nameof(sender));
        }

        var keyPath = GetCocoaKeyPathUnsafe(sender.GetType(), propertyName);

        return GetNotificationForProperty(
            sender,
            expression,
            propertyName,
            keyPath,
            beforeChanged);
    }

    /// <summary>
    /// Subscribes to KVO change notifications using a pre-resolved observation key (KVO key path).
    /// </summary>
    /// <remarks>
    /// <para>
    /// This helper wires NSObject AddObserver/RemoveObserver patterns
    /// to an <see cref="IObservable{T}"/> sequence and ensures deterministic unsubscription.
    /// </para>
    /// <para>
    /// The returned disposable is idempotent and will remove the observer and release the pinned delegate instance.
    /// </para>
    /// </remarks>
    /// <param name="sender">The object to observe. Must be an <see cref="NSObject"/>.</param>
    /// <param name="expression">
    /// The expression describing the observed member. This value is surfaced in emitted
    /// <see cref="IObservedChange{TSender,TValue}"/> instances.
    /// </param>
    /// <param name="propertyName">The .NET property name being observed.</param>
    /// <param name="observationKey">The Cocoa KVO key path to observe.</param>
    /// <param name="beforeChanged">
    /// If <see langword="true"/>, request notifications using <see cref="NSKeyValueObservingOptions.Old"/>; otherwise
    /// request notifications using <see cref="NSKeyValueObservingOptions.New"/>.
    /// </param>
    /// <returns>
    /// An observable that produces an <see cref="IObservedChange{TSender,TValue}"/> whenever the KVO key path changes.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sender"/>, <paramref name="expression"/>, <paramref name="propertyName"/>, or
    /// <paramref name="observationKey"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="sender"/> is not an <see cref="NSObject"/>.</exception>
    private static IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        string observationKey,
        bool beforeChanged = false)
    {
        ArgumentExceptionHelper.ThrowIfNull(sender);
        ArgumentExceptionHelper.ThrowIfNull(expression);
        ArgumentExceptionHelper.ThrowIfNull(propertyName);
        ArgumentExceptionHelper.ThrowIfNull(observationKey);

        if (sender is not NSObject obj)
        {
            throw new ArgumentException("Sender must be an NSObject.", nameof(sender));
        }

        return new KeyValueObservingObservable(obj, expression, observationKey, beforeChanged);
    }

    /// <summary>
    /// Determines whether the specified member name is declared on the type hierarchy rooted at <see cref="NSObject"/>.
    /// </summary>
    /// <param name="type">The runtime type to inspect.</param>
    /// <param name="propertyName">The member name to test.</param>
    /// <returns>
    /// <see langword="true"/> if the member name is present on the inspected hierarchy; otherwise <see langword="false"/>.
    /// </returns>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    private static bool IsDeclaredOnNSObject(
        Type type,
        string propertyName)
    {
        var monotouchAssemblyName = typeof(NSObject).Assembly.FullName;

        var current = type;
        while (current is not null)
        {
            // Search only public instance members declared at this level.
            var members = current.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            for (var i = 0; i < members.Length; i++)
            {
                if (string.Equals(members[i].Name, propertyName, StringComparison.Ordinal))
                {
                    // Historical heuristic: treat it as Obj-C-backed if it originates from the NSObject assembly.
                    return string.Equals(current.Assembly.FullName, monotouchAssemblyName, StringComparison.Ordinal);
                }
            }

            current = current.BaseType;
        }

        // The member doesn't exist on the hierarchy.
        return false;
    }

    /// <summary>
    /// A fused <see cref="IObservable{T}"/> that wires NSObject Key-Value Observing (KVO) add/remove
    /// observer calls directly inside <see cref="Subscribe"/>, replacing the previous
    /// <c>Observable.Create</c> allocation with a single class-based allocation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// On each subscription a <see cref="BlockObserveValueDelegate"/> is created and pinned via
    /// a <see cref="GCHandle"/> so that the runtime does not relocate or collect it while KVO holds
    /// a reference to it.  Both the observer removal and the handle release are performed atomically
    /// when the returned <see cref="ActionDisposable"/> is disposed.
    /// </para>
    /// <para>
    /// The class is <see langword="sealed"/> and <see langword="internal"/> because it is an
    /// implementation detail of <see cref="KVOObservableForProperty"/> and has no stable public API.
    /// </para>
    /// </remarks>
    private sealed class KeyValueObservingObservable : IObservable<IObservedChange<object?, object?>>
    {
        /// <summary>The NSObject instance that is being observed.</summary>
        private readonly NSObject _obj;

        /// <summary>The expression that describes the observed member; surfaced in every change notification.</summary>
        private readonly Expression _expression;

        /// <summary>The Cocoa KVO key path string used to register and unregister the observer.</summary>
        private readonly string _observationKey;

        /// <summary>
        /// <see langword="true"/> to request <see cref="NSKeyValueObservingOptions.Old"/> notifications
        /// (before-changed); <see langword="false"/> to request <see cref="NSKeyValueObservingOptions.New"/>
        /// notifications (after-changed).
        /// </summary>
        private readonly bool _beforeChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="KeyValueObservingObservable"/>.
        /// </summary>
        /// <param name="obj">The NSObject instance to observe. Must not be <see langword="null"/>.</param>
        /// <param name="expression">The expression describing the observed member. Must not be <see langword="null"/>.</param>
        /// <param name="observationKey">The Cocoa KVO key path. Must not be <see langword="null"/>.</param>
        /// <param name="beforeChanged">
        /// If <see langword="true"/>, observations use <see cref="NSKeyValueObservingOptions.Old"/>;
        /// otherwise <see cref="NSKeyValueObservingOptions.New"/>.
        /// </param>
        internal KeyValueObservingObservable(
            NSObject obj,
            Expression expression,
            string observationKey,
            bool beforeChanged)
        {
            _obj = obj;
            _expression = expression;
            _observationKey = observationKey;
            _beforeChanged = beforeChanged;
        }

        /// <inheritdoc/>
        /// <returns>
        /// An <see cref="IDisposable"/> that, when disposed, removes the KVO observer and frees
        /// the pinned <see cref="GCHandle"/> that kept the delegate alive.
        /// </returns>
        public IDisposable Subscribe(IObserver<IObservedChange<object?, object?>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            // Create a single stable delegate instance; KVO removal requires the same observer instance.
            var callback = new BlockObserveValueDelegate((_, observedObject, __) =>
                observer.OnNext(new ObservedChange<object?, object?>(observedObject, _expression, default)));

            // Ensure the delegate is kept alive for the lifetime of the subscription.
            var handle = GCHandle.Alloc(callback);

            var keyPath = (NSString)_observationKey;

            _obj.AddObserver(
                callback,
                keyPath,
                _beforeChanged ? NSKeyValueObservingOptions.Old : NSKeyValueObservingOptions.New,
                IntPtr.Zero);

            return new ActionDisposable(() =>
            {
                _obj.RemoveObserver(callback, keyPath);
                handle.Free();
            });
        }
    }

    /// <summary>
    /// Maps a .NET property name to an Objective-C selector / KVO key path using reflection over the runtime type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method inspects the runtime type for an exported selector attribute on the getter, and falls back to a
    /// naming convention when no export is found.
    /// </para>
    /// <para>
    /// Trimming/AOT: this method reflects over runtime types and is not trimming-safe.
    /// </para>
    /// </remarks>
    /// <param name="senderType">The runtime type of the sender.</param>
    /// <param name="propertyName">The .NET property name.</param>
    /// <returns>The derived Cocoa key path to use for KVO.</returns>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    private static string GetCocoaKeyPathUnsafe(Type senderType, string propertyName)
    {
        // Note: This logic preserves the original behavior pattern: best-effort attempt and fallback.
        var property = senderType
            .GetTypeInfo()
            .DeclaredProperties
            .FirstOrDefault(p => !p.IsStatic());

        var propIsBoolean = false;

        if (property is not null)
        {
            propIsBoolean = property.PropertyType == typeof(bool);

            var getter = property.GetGetMethod();
            if (getter is not null)
            {
                var export = getter
                    .GetCustomAttributes(inherit: true)
                    .OfType<ExportAttribute>()
                    .FirstOrDefault();

                if (export?.Selector is not null)
                {
                    return export.Selector;
                }
            }
        }

        if (propIsBoolean)
        {
            propertyName = "Is" + propertyName;
        }

        return string.Concat(
            char.ToLowerInvariant(propertyName[0]).ToString(CultureInfo.InvariantCulture),
            propertyName.AsSpan(1));
    }
}
