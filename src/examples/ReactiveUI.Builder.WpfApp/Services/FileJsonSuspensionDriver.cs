// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ReactiveUI.Builder.WpfApp.Models;

namespace ReactiveUI.Builder.WpfApp.Services;

/// <summary>A suspension driver that persists and restores the terminal state as indented JSON on disk.</summary>
/// <seealso cref="ISuspensionDriver" />
/// <remarks>Initializes a new instance of the <see cref="FileJsonSuspensionDriver"/> class.</remarks>
/// <param name="path">The file path the state is persisted to.</param>
public sealed class FileJsonSuspensionDriver(string path) : ISuspensionDriver
{
    /// <summary>The serializer options used when writing state, configured to produce human-readable JSON.</summary>
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    /// <summary>Invalidates the application state by deleting it from disk.</summary>
    /// <returns>A completed observable.</returns>
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
    /// <returns>An observable that produces the loaded state.</returns>
    public IObservable<object?> LoadState() => Signal.Start(
        object? () =>
        {
            if (!File.Exists(path))
            {
                return new TerminalState();
            }

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<TerminalState>(json) ?? new TerminalState();
        },
        RxSchedulers.TaskpoolScheduler);

    /// <summary>Loads the application state using source-generated JSON metadata.</summary>
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
    /// <returns>A completed observable.</returns>
    public IObservable<RxVoid> SaveState<T>(T state) => Signal.Start(
        () => File.WriteAllText(path, JsonSerializer.Serialize(state, _options)),
        RxSchedulers.TaskpoolScheduler);

    /// <summary>Saves the application state to disk using source-generated JSON metadata.</summary>
    /// <typeparam name="T">The type of state to save.</typeparam>
    /// <param name="state">The application state.</param>
    /// <param name="typeInfo">The source-generated JSON type info.</param>
    /// <returns>A completed observable.</returns>
    public IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo) => Signal.Start(
        () => File.WriteAllText(path, JsonSerializer.Serialize(state, typeInfo)),
        RxSchedulers.TaskpoolScheduler);
}
