// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization.Metadata;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Provides a no-op implementation of <see cref="IUnsafeSuspensionDriver"/>.</summary>
public sealed class DummySuspensionDriverUnsafe : IUnsafeSuspensionDriver
{
    /// <inheritdoc />
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<object?> LoadState() =>
        new ReturnSignal<object?>(null, Sequencer.Immediate);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo) =>
        new ReturnSignal<T?>(default, Sequencer.Immediate);

    /// <inheritdoc />
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<RxVoid> SaveState<T>(T state) =>
        ImmutableReturnRxVoidSignal.Instance;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo) =>
        ImmutableReturnRxVoidSignal.Instance;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<RxVoid> InvalidateState() =>
        ImmutableReturnRxVoidSignal.Instance;
}
