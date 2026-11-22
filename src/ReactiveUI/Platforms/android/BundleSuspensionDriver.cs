// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ReactiveUI.Internal;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Loads and saves application state using the platform bundle.</summary>
/// <remarks>
/// <para>
/// Uses trimming/AOT-safe serialization via source-generated <see cref="JsonTypeInfo{T}"/>.
/// For reflection-based serialization, use <c>BundleSuspensionDriverUnsafe</c> from <c>ReactiveUI.Unsafe</c>.
/// </para>
/// </remarks>
public sealed class BundleSuspensionDriver : ISuspensionDriver
{
    /// <summary>The bundle key under which the serialized application state is stored.</summary>
    private const string StateKey = "__state";

    /// <inheritdoc />
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo)
    {
        ArgumentExceptionHelper.ThrowIfNull(typeInfo);

        try
        {
            if (AutoSuspendHelper.LatestBundle is null)
            {
                return Signal.Fail<T?>(
                    new InvalidOperationException("New bundle detected; no persisted state is available."));
            }

            var buffer = AutoSuspendHelper.LatestBundle.GetByteArray(StateKey);
            return buffer is null ? Signal.Fail<T?>(
                    new InvalidOperationException("The persisted state buffer could not be found.")) : new TaskObservable<T?>(DeserializeAsync());

            async Task<T?> DeserializeAsync()
            {
                await using MemoryStream stream = new(buffer, false);
                return await JsonSerializer.DeserializeAsync(stream, typeInfo).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            return Signal.Fail<T?>(ex);
        }
    }

    /// <inheritdoc />
    public IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
    {
        ArgumentExceptionHelper.ThrowIfNull(typeInfo);

        try
        {
            using MemoryStream stream = new();
            JsonSerializer.Serialize(stream, state, typeInfo);

            AutoSuspendHelper.LatestBundle?.PutByteArray(StateKey, stream.ToArray());
            return ImmutableReturnRxVoidSignal.Instance;
        }
        catch (Exception ex)
        {
            return Signal.Fail<RxVoid>(ex);
        }
    }

    /// <inheritdoc />
    public IObservable<RxVoid> InvalidateState()
    {
        try
        {
            AutoSuspendHelper.LatestBundle?.PutByteArray(StateKey, []);
            return ImmutableReturnRxVoidSignal.Instance;
        }
        catch (Exception ex)
        {
            return Signal.Fail<RxVoid>(ex);
        }
    }
}
