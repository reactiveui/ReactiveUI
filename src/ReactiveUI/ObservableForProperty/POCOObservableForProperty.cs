// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;
using Splat;

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
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Legacy naming convention")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Legacy naming convention")]
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
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type type, string propertyName) =>
        GetAffinityForObject(type, propertyName, false);

    /// <inheritdoc />
    /// <remarks>
    /// This fallback returns a very low affinity to ensure it is only used when no more specific implementation applies.
    /// </remarks>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type? type, string propertyName, bool beforeChanged)
    {
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        return type is null ? 0 : 1;
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName) =>
        GetNotificationForProperty(sender, expression, propertyName, false, false);

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged) =>
        GetNotificationForProperty(sender, expression, propertyName, beforeChanged, false);

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when sender is null.</exception>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged,
        bool suppressWarnings)
    {
        ArgumentExceptionHelper.ThrowIfNull(sender);
        ArgumentExceptionHelper.ThrowIfNull(expression);
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        if (!suppressWarnings)
        {
            WarnOnce(sender, propertyName);
        }

        return new SingleScheduledChange(
            new ObservedChange<object, object?>(sender, expression, null),
            RxSchedulers.MainThreadScheduler);
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
        var type = sender.GetType();
        if (!HasWarned.TryAdd((type, propertyName), 0))
        {
            return;
        }

        this.Log().Warn(
            $"The class {type.FullName} property {propertyName} is a POCO type and won't send change notifications, WhenAny will only return a single value!");
    }

    /// <summary>
    /// Emits a single observed change on the supplied scheduler and then stays open without completing (a POCO has no
    /// further change notifications). Replaces <c>Observable.Return(...).Concat(Never)</c> with one tailored layer.
    /// </summary>
    /// <param name="value">The single observed change to emit.</param>
    /// <param name="scheduler">The scheduler the value is emitted on.</param>
    private sealed class SingleScheduledChange(
        IObservedChange<object, object?> value,
        IScheduler scheduler) : IObservable<IObservedChange<object, object?>>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IObservedChange<object, object?>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return scheduler.Schedule(
                (Observer: observer, Value: value),
                static (_, state) =>
                {
                    state.Observer.OnNext(state.Value);
                    return EmptyDisposable.Instance;
                });
        }
    }
}
