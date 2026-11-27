// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Windows;

#if REACTIVE_SHIM
using IThreadAffineSequencer = ReactiveUI.Primitives.Concurrency.IThreadAffineSequencer;

namespace ReactiveUI.Reactive.Builder;
#else
namespace ReactiveUI.Builder;
#endif

/// <summary>Schedules work on <see cref="DispatcherSequencer.Main"/>, looked up on every call.</summary>
/// <remarks>This lets ReactiveUI be configured before the WPF <see cref="Application"/> exists.</remarks>
internal sealed class DeferredWpfMainThreadSequencer : ISequencer, IThreadAffineSequencer
{
    /// <summary>The shared instance.</summary>
    internal static readonly DeferredWpfMainThreadSequencer Instance = new();

    /// <summary>Initializes a new instance of the <see cref="DeferredWpfMainThreadSequencer"/> class.</summary>
    private DeferredWpfMainThreadSequencer()
    {
    }

    /// <inheritdoc/>
    public DateTimeOffset Now => TimeProvider.System.GetUtcNow();

#if !REACTIVE_SHIM
    /// <inheritdoc/>
    public long Timestamp => System.Diagnostics.Stopwatch.GetTimestamp();
#endif

    /// <inheritdoc/>
    /// <remarks>Returns <see langword="false"/> while no <see cref="Application"/> exists, because no UI thread exists yet.</remarks>
    public bool CheckAccess() => Application.Current?.Dispatcher.CheckAccess() == true;

#if REACTIVE_SHIM
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action) =>
        DispatcherSequencer.Main.Schedule(state, action);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action) =>
        DispatcherSequencer.Main.Schedule(state, dueTime, action);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action) =>
        DispatcherSequencer.Main.Schedule(state, dueTime, action);
#else
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Schedule(IWorkItem item) => DispatcherSequencer.Main.Schedule(item);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Schedule(IWorkItem item, long dueTimestamp) => DispatcherSequencer.Main.Schedule(item, dueTimestamp);
#endif
}
