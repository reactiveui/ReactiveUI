// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ReactiveUI.Builder.BlazorServer.Models;

namespace ReactiveUI.Builder.BlazorServer.Services;

/// <summary>A suspension driver that persists and restores application state as JSON in a file on disk.</summary>
/// <seealso cref="ISuspensionDriver" />
/// <remarks>
/// Initializes a new instance of the <see cref="FileJsonSuspensionDriver"/> class.
/// </remarks>
/// <param name="path">The path.</param>
public sealed class FileJsonSuspensionDriver(string path) : ISuspensionDriver
{
    /// <summary>The JSON serializer options used when persisting state to disk.</summary>
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    /// <summary>Invalidates the application state (i.e. deletes it from disk).</summary>
    /// <returns>
    /// A completed observable.
    /// </returns>
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

    /// <summary>Loads the application state from persistent storage.</summary>
    /// <returns>
    /// An object observable.
    /// </returns>
    public IObservable<object?> LoadState() => Signal.Start(
        () =>
        {
            if (!File.Exists(path))
            {
                return new();
            }

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ChatState>(json) ?? new ChatState();
        },
        RxSchedulers.TaskpoolScheduler);

    /// <summary>Loads the application state from persistent storage using source-generated JSON metadata.</summary>
    /// <typeparam name="T">The type of state to load.</typeparam>
    /// <param name="typeInfo">The source-generated JSON type info.</param>
    /// <returns>An observable that produces the deserialized state.</returns>
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo) => Signal.Start(
        () =>
        {
            if (!File.Exists(path))
            {
                return default;
            }

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize(json, typeInfo);
        },
        RxSchedulers.TaskpoolScheduler);

    /// <summary>Saves the application state to disk.</summary>
    /// <typeparam name="T">The type of state to save.</typeparam>
    /// <param name="state">The application state.</param>
    /// <returns>
    /// A completed observable.
    /// </returns>
    public IObservable<RxVoid> SaveState<T>(T state) => Signal.Start(
        () =>
        {
            var json = JsonSerializer.Serialize(state, _options);
            File.WriteAllText(path, json);
        },
        RxSchedulers.TaskpoolScheduler);

    /// <summary>Saves the application state to disk using source-generated JSON metadata.</summary>
    /// <typeparam name="T">The type of state to save.</typeparam>
    /// <param name="state">The application state.</param>
    /// <param name="typeInfo">The source-generated JSON type info.</param>
    /// <returns>A completed observable.</returns>
    public IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo) => Signal.Start(
        () =>
        {
            var json = JsonSerializer.Serialize(state, typeInfo);
            File.WriteAllText(path, json);
        },
        RxSchedulers.TaskpoolScheduler);
}
