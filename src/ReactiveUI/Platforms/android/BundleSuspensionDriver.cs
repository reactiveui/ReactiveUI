// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

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
            // NB: Sometimes OnCreate gives us a null bundle.
            if (AutoSuspendHelper.LatestBundle is null)
            {
                return Observable.Throw<object?>(
                    new InvalidOperationException("New bundle detected; no persisted state is available."));
            }

            var buffer = AutoSuspendHelper.LatestBundle.GetByteArray(StateKey);
            if (buffer is null)
            {
                return Observable.Throw<object?>(
                    new InvalidOperationException("The persisted state buffer could not be found."));
            }

            return Observable.FromAsync(async () =>
            {
                await using var stream = new MemoryStream(buffer, writable: false);
                return await JsonSerializer.DeserializeAsync<object>(stream).ConfigureAwait(false);
            });
        }
        catch (Exception ex)
        {
            return Observable.Throw<object?>(ex);
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
            using var stream = new MemoryStream();
            JsonSerializer.Serialize(stream, state);

            AutoSuspendHelper.LatestBundle?.PutByteArray(StateKey, stream.ToArray());
            return Observables.Unit;
        }
        catch (Exception ex)
        {
            return Observable.Throw<Unit>(ex);
        }
    }

    /// <inheritdoc />
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo)
    {
        ArgumentNullException.ThrowIfNull(typeInfo);

        try
        {
            if (AutoSuspendHelper.LatestBundle is null)
            {
                return Observable.Throw<T?>(
                    new InvalidOperationException("New bundle detected; no persisted state is available."));
            }

            var buffer = AutoSuspendHelper.LatestBundle.GetByteArray(StateKey);
            if (buffer is null)
            {
                return Observable.Throw<T?>(
                    new InvalidOperationException("The persisted state buffer could not be found."));
            }

            return Observable.FromAsync(async () =>
            {
                await using var stream = new MemoryStream(buffer, writable: false);
                return await JsonSerializer.DeserializeAsync(stream, typeInfo).ConfigureAwait(false);
            });
        }
        catch (Exception ex)
        {
            return Observable.Throw<T?>(ex);
        }
    }

    /// <inheritdoc />
    public IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
    {
        ArgumentNullException.ThrowIfNull(typeInfo);

        try
        {
            using var stream = new MemoryStream();
            JsonSerializer.Serialize(stream, state, typeInfo);

            AutoSuspendHelper.LatestBundle?.PutByteArray(StateKey, stream.ToArray());
            return Observables.Unit;
        }
        catch (Exception ex)
        {
            return Observable.Throw<Unit>(ex);
        }
    }

    /// <inheritdoc />
    public IObservable<Unit> InvalidateState()
    {
        try
        {
            AutoSuspendHelper.LatestBundle?.PutByteArray(StateKey, []);
            return Observables.Unit;
        }
        catch (Exception ex)
        {
            return Observable.Throw<Unit>(ex);
        }
    }
}
