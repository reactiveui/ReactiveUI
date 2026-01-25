// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text.Json.Serialization.Metadata;

namespace ReactiveUI;

/// <summary>
/// Provides a no-op implementation of the ISuspensionDriver interface for scenarios where application state persistence
/// is not required.
/// </summary>
/// <remarks>This class can be used in testing or development environments where state loading and saving are
/// unnecessary. All methods complete immediately without performing any actual serialization or storage operations. No
/// state is persisted or restored when using this driver.</remarks>
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
