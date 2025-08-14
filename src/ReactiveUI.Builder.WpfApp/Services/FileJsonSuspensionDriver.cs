// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;

namespace ReactiveUI.Builder.WpfApp.Services;

/// <summary>
/// FileJsonSuspensionDriver.
/// </summary>
/// <seealso cref="ReactiveUI.ISuspensionDriver" />
/// <remarks>
/// Initializes a new instance of the <see cref="FileJsonSuspensionDriver"/> class.
/// </remarks>
/// <param name="path">The path.</param>
public sealed class FileJsonSuspensionDriver(string path) : ISuspensionDriver
{
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    /// <summary>
    /// Invalidates the application state (i.e. deletes it from disk).
    /// </summary>
    /// <returns>
    /// A completed observable.
    /// </returns>
    public IObservable<Unit> InvalidateState() => Observable.Start(
    () =>
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    },
    RxApp.TaskpoolScheduler);

    /// <summary>
    /// Loads the application state from persistent storage.
    /// </summary>
    /// <returns>
    /// An object observable.
    /// </returns>
    public IObservable<object> LoadState() => Observable.Start<object>(
    () =>
    {
        if (!File.Exists(path))
        {
            return new ViewModels.ChatState();
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ViewModels.ChatState>(json) ?? new ViewModels.ChatState();
    },
    RxApp.TaskpoolScheduler);

    /// <summary>
    /// Saves the application state to disk.
    /// </summary>
    /// <param name="state">The application state.</param>
    /// <returns>
    /// A completed observable.
    /// </returns>
    public IObservable<Unit> SaveState(object state) => Observable.Start(
    () =>
    {
        var json = JsonSerializer.Serialize(state, _options);
        File.WriteAllText(path, json);
    },
    RxApp.TaskpoolScheduler);
}
