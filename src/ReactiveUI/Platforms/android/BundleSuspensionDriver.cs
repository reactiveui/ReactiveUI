// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>
/// Loads and saves application state using the platform bundle.
/// </summary>
/// <remarks>
/// <para>
/// This driver supports both legacy reflection-based System.Text.Json serialization
/// and trimming/AOT-safe serialization via source-generated <see cref="JsonTypeInfo{T}"/>.
/// </para>
/// </remarks>
public sealed class BundleSuspensionDriver : ISuspensionDriver
{
    /// <summary>
    /// The bundle key under which the serialized application state is stored.
    /// </summary>
    private const string StateKey = "__state";

    /// <inheritdoc />
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. " +
        "Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. " +
        "Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    public IObservable<object?> LoadState()
    {
        try
        {
            if (AutoSuspendHelper.LatestBundle is null)
            {
                return new ThrowObservable<object?>(
                    new InvalidOperationException("New bundle detected; no persisted state is available."));
            }

            var buffer = AutoSuspendHelper.LatestBundle.GetByteArray(StateKey);
            if (buffer is null)
            {
                return new ThrowObservable<object?>(
                    new InvalidOperationException("The persisted state buffer could not be found."));
            }

            return new TaskObservable<object?>(DeserializeAsync());

            async Task<object?> DeserializeAsync()
            {
                await using MemoryStream stream = new(buffer, false);
                return await JsonSerializer.DeserializeAsync<object>(stream).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            return new ThrowObservable<object?>(ex);
        }
    }

    /// <inheritdoc />
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo)
    {
        ArgumentExceptionHelper.ThrowIfNull(typeInfo);

        try
        {
            if (AutoSuspendHelper.LatestBundle is null)
            {
                return new ThrowObservable<T?>(
                    new InvalidOperationException("New bundle detected; no persisted state is available."));
            }

            var buffer = AutoSuspendHelper.LatestBundle.GetByteArray(StateKey);
            if (buffer is null)
            {
                return new ThrowObservable<T?>(
                    new InvalidOperationException("The persisted state buffer could not be found."));
            }

            return new TaskObservable<T?>(DeserializeAsync());

            async Task<T?> DeserializeAsync()
            {
                await using MemoryStream stream = new(buffer, false);
                return await JsonSerializer.DeserializeAsync(stream, typeInfo).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            return new ThrowObservable<T?>(ex);
        }
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. " +
        "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. " +
        "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    public IObservable<Unit> SaveState<T>(T state)
    {
        try
        {
            using MemoryStream stream = new();
            JsonSerializer.Serialize(stream, state);

            AutoSuspendHelper.LatestBundle?.PutByteArray(StateKey, stream.ToArray());
            return SingleValueObservable.Unit;
        }
        catch (Exception ex)
        {
            return new ThrowObservable<Unit>(ex);
        }
    }

    /// <inheritdoc />
    public IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
    {
        ArgumentExceptionHelper.ThrowIfNull(typeInfo);

        try
        {
            using MemoryStream stream = new();
            JsonSerializer.Serialize(stream, state, typeInfo);

            AutoSuspendHelper.LatestBundle?.PutByteArray(StateKey, stream.ToArray());
            return SingleValueObservable.Unit;
        }
        catch (Exception ex)
        {
            return new ThrowObservable<Unit>(ex);
        }
    }

    /// <inheritdoc />
    public IObservable<Unit> InvalidateState()
    {
        try
        {
            AutoSuspendHelper.LatestBundle?.PutByteArray(StateKey, []);
            return SingleValueObservable.Unit;
        }
        catch (Exception ex)
        {
            return new ThrowObservable<Unit>(ex);
        }
    }
}
