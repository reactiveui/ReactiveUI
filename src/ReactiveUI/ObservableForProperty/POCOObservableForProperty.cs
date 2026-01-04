// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace ReactiveUI;

/// <summary>
/// Final fallback implementation for <c>WhenAny</c>-style observation when no observable mechanism is available.
/// </summary>
/// <remarks>
/// <para>
/// This implementation emits exactly one value (the current value at subscription time) and then never emits again.
/// </para>
/// <para>
/// If warnings are enabled, it logs a warning once per (runtime type, property name) pair to help callers detect
/// accidental POCO usage in observation chains.
/// </para>
/// <para>
/// Trimming/AOT: <see cref="ICreatesObservableForProperty"/> is annotated for trimming/AOT in this codebase; this type
/// repeats the required annotations on its public members to satisfy the interface contract.
/// </para>
/// </remarks>
public sealed class POCOObservableForProperty : ICreatesObservableForProperty
{
    /// <summary>
    /// Tracks whether a warning has been logged for a given (runtime type, property name) pair.
    /// </summary>
    /// <remarks>
    /// This is a process-wide cache intended to avoid repeated warnings. It can grow with unique observed pairs.
    /// </remarks>
    private static readonly ConcurrentDictionary<(Type Type, string PropertyName), byte> HasWarned = new();

    /// <inheritdoc />
    /// <remarks>
    /// This fallback returns a very low affinity to ensure it is only used when no more specific implementation applies.
    /// </remarks>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
    {
        ArgumentExceptionHelper.ThrowIfNull(type);
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        return 1;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="sender"/> is <see langword="null"/>.</exception>
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

        if (!suppressWarnings)
        {
            WarnOnce(sender, propertyName);
        }

        // Emit one value, then never complete to preserve legacy WhenAny semantics.
        return Observable
            .Return(new ObservedChange<object, object?>(sender, expression, default), RxSchedulers.MainThreadScheduler)
            .Concat(Observable<IObservedChange<object, object?>>.Never);
    }

    /// <summary>
    /// Logs a POCO observation warning at most once per (runtime type, property name) pair.
    /// </summary>
    /// <param name="sender">The observed object.</param>
    /// <param name="propertyName">The observed property name.</param>
#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private void WarnOnce(object sender, string propertyName)
    {
        // Hot path considerations:
        // - Avoid ContainsKey + indexer (two lookups).
        // - Use TryAdd as the single atomic gate.
        var type = sender.GetType();
        if (!HasWarned.TryAdd((type, propertyName), 0))
        {
            return;
        }

        this.Log().Warn(
            $"The class {type.FullName} property {propertyName} is a POCO type and won't send change notifications, WhenAny will only return a single value!");
    }
}
