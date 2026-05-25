// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Text.Json.Serialization.Metadata;

using ReactiveUI.Internal;

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
        => new SingleValueObservable<object?>(null);

    /// <inheritdoc />
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo) => new SingleValueObservable<T?>(default);

    /// <inheritdoc />
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. " +
        "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. " +
        "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    public IObservable<Unit> SaveState<T>(T state)
        => SingleValueObservable.Unit;

    /// <inheritdoc />
    public IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo) => SingleValueObservable.Unit;

    /// <inheritdoc />
    public IObservable<Unit> InvalidateState()
        => SingleValueObservable.Unit;
}
