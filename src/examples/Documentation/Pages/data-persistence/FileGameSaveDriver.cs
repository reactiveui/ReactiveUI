// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ReactiveUI.Documentation.DataPersistence;

/// <summary>A file-backed <see cref="ISuspensionDriver"/> that saves the game's state as JSON, using source-generated metadata so it needs no reflection.</summary>
/// <param name="path">The file the state is written to and read from.</param>
public sealed class FileGameSaveDriver(string path) : ISuspensionDriver
{
    /// <summary>Deletes the save file, if one exists.</summary>
    /// <returns>A completed signal.</returns>
    public IObservable<RxVoid> InvalidateState() => Signal.Start(
        () =>
        {
            if (!File.Exists(path))
            {
                return;
            }

            File.Delete(path);
        },
        RxSchedulers.TaskpoolScheduler);

    /// <summary>Loads the state using source-generated JSON metadata.</summary>
    /// <typeparam name="T">The state type.</typeparam>
    /// <param name="typeInfo">The source-generated metadata for <typeparamref name="T"/>.</param>
    /// <returns>A signal producing the saved state, or <see langword="null"/> when no save file exists.</returns>
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo) => Signal.Start(
        () => File.Exists(path) ? JsonSerializer.Deserialize(File.ReadAllText(path), typeInfo) : default,
        RxSchedulers.TaskpoolScheduler);

    /// <summary>Not supported: this driver only accepts source-generated metadata, so it never reflects over the saved type.</summary>
    /// <returns>Never returns.</returns>
    /// <exception cref="NotSupportedException">Always thrown; call <see cref="LoadState{T}(JsonTypeInfo{T})"/> instead.</exception>
    [RequiresUnreferencedCode("This driver never reflects; use LoadState<T>(JsonTypeInfo<T>) instead.")]
    [RequiresDynamicCode("This driver never reflects; use LoadState<T>(JsonTypeInfo<T>) instead.")]
    public IObservable<object?> LoadState() =>
        throw new NotSupportedException($"{nameof(FileGameSaveDriver)} requires a JsonTypeInfo<T>; call LoadState<T>(JsonTypeInfo<T>) instead.");

    /// <summary>Saves the state using source-generated JSON metadata.</summary>
    /// <typeparam name="T">The state type.</typeparam>
    /// <param name="state">The state to save.</param>
    /// <param name="typeInfo">The source-generated metadata for <typeparamref name="T"/>.</param>
    /// <returns>A completed signal.</returns>
    public IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo) => Signal.Start(
        () => File.WriteAllText(path, JsonSerializer.Serialize(state, typeInfo)),
        RxSchedulers.TaskpoolScheduler);

    /// <summary>Not supported: this driver only accepts source-generated metadata, so it never reflects over the state's runtime type.</summary>
    /// <typeparam name="T">The state type.</typeparam>
    /// <param name="state">Unused.</param>
    /// <returns>Never returns.</returns>
    /// <exception cref="NotSupportedException">Always thrown; call <see cref="SaveState{T}(T, JsonTypeInfo{T})"/> instead.</exception>
    [RequiresUnreferencedCode("This driver never reflects; use SaveState<T>(T, JsonTypeInfo<T>) instead.")]
    [RequiresDynamicCode("This driver never reflects; use SaveState<T>(T, JsonTypeInfo<T>) instead.")]
    public IObservable<RxVoid> SaveState<T>(T state) =>
        throw new NotSupportedException($"{nameof(FileGameSaveDriver)} requires a JsonTypeInfo<T>; call SaveState<T>(T, JsonTypeInfo<T>) instead.");
}
