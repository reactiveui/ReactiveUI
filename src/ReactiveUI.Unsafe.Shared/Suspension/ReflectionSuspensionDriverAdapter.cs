// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Gives an <see cref="ISuspensionDriver"/> the reflection-based members of <see cref="IUnsafeSuspensionDriver"/>.</summary>
/// <param name="inner">The driver that stores the state.</param>
internal sealed class ReflectionSuspensionDriverAdapter(ISuspensionDriver inner) : IUnsafeSuspensionDriver
{
    /// <summary>Trim/AOT diagnostic message for the reflection-based members.</summary>
    private const string ReflectionMessage =
        "Builds System.Text.Json metadata by reflection. Prefer the JsonTypeInfo<T> overloads for trimming or AOT scenarios.";

    /// <inheritdoc />
    [RequiresUnreferencedCode(ReflectionMessage)]
    [RequiresDynamicCode(ReflectionMessage)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<object?> LoadState() => inner.LoadState(GetReflectionTypeInfo<object>());

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo) => inner.LoadState(typeInfo);

    /// <inheritdoc />
    [RequiresUnreferencedCode(ReflectionMessage)]
    [RequiresDynamicCode(ReflectionMessage)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<RxVoid> SaveState<T>(T state) => inner.SaveState(state, GetReflectionTypeInfo<T>());

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo) => inner.SaveState(state, typeInfo);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<RxVoid> InvalidateState() => inner.InvalidateState();

    /// <summary>Gets the reflection-based metadata that <c>JsonSerializer</c> uses for <typeparamref name="T"/> by default.</summary>
    /// <typeparam name="T">The type to describe.</typeparam>
    /// <returns>The metadata for <typeparamref name="T"/>.</returns>
    [RequiresUnreferencedCode(ReflectionMessage)]
    [RequiresDynamicCode(ReflectionMessage)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static JsonTypeInfo<T> GetReflectionTypeInfo<T>() =>
#if NET11_0_OR_GREATER
        JsonSerializerOptions.Default.GetTypeInfo<T>();
#else
        (JsonTypeInfo<T>)JsonSerializerOptions.Default.GetTypeInfo(typeof(T));
#endif
}
