// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ReactiveUI.Internal;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Loads and saves application state using the platform bundle, including reflection-based overloads.</summary>
[DebuggerDisplay("BundleSuspensionDriverUnsafe")]
public sealed class BundleSuspensionDriverUnsafe : IUnsafeSuspensionDriver
{
    /// <summary>The bundle key under which the serialized application state is stored.</summary>
    private const string StateKey = "__state";

    /// <summary>The AOT-safe inner driver for typed operations.</summary>
    private readonly BundleSuspensionDriver _inner = new();

    /// <inheritdoc />
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    public IObservable<object?> LoadState()
    {
        try
        {
            if (AutoSuspendHelper.LatestBundle is null)
            {
                return Signal.Fail<object?>(
                    new InvalidOperationException("New bundle detected; no persisted state is available."));
            }

            var buffer = AutoSuspendHelper.LatestBundle.GetByteArray(StateKey);
            return buffer is null
                ? Signal.Fail<object?>(new InvalidOperationException("The persisted state buffer could not be found."))
                : new TaskObservable<object?>(DeserializeAsync());

            async Task<object?> DeserializeAsync()
            {
                await using MemoryStream stream = new(buffer, false);
                return await JsonSerializer.DeserializeAsync<object>(stream).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            return Signal.Fail<object?>(ex);
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo) => _inner.LoadState(typeInfo);

    /// <inheritdoc />
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    public IObservable<RxVoid> SaveState<T>(T state)
    {
        try
        {
            using MemoryStream stream = new();
            JsonSerializer.Serialize(stream, state);

            AutoSuspendHelper.LatestBundle?.PutByteArray(StateKey, stream.ToArray());
            return ImmutableReturnRxVoidSignal.Instance;
        }
        catch (Exception ex)
        {
            return Signal.Fail<RxVoid>(ex);
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo) => _inner.SaveState(state, typeInfo);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<RxVoid> InvalidateState() => _inner.InvalidateState();
}
