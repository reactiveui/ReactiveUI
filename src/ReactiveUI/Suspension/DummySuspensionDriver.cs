// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Text.Json.Serialization.Metadata;

namespace ReactiveUI;

/// <summary>
/// A suspension driver that does not persist any state.
/// </summary>
/// <remarks>
/// <para>
/// This driver is useful for unit tests and for platforms or applications where persistence is undesired.
/// </para>
/// <para>
/// All load operations return <see langword="null"/> (or <see langword="default"/> for the requested type),
/// and all save/invalidate operations complete immediately.
/// </para>
/// </remarks>
public sealed class DummySuspensionDriver : ISuspensionDriver
{
    /// <inheritdoc />
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. " +
        "Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. " +
        "Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    public IObservable<object?> LoadState()
        => Observable.Return((object?)null);

    /// <inheritdoc />
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. " +
        "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. " +
        "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    public IObservable<Unit> SaveState<T>(T state)
        => Observables.Unit;

    /// <inheritdoc />
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo)
    {
        return Observable.Return<T?>(default);
    }

    /// <inheritdoc />
    public IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
    {
        return Observables.Unit;
    }

    /// <inheritdoc />
    public IObservable<Unit> InvalidateState()
        => Observables.Unit;
}
