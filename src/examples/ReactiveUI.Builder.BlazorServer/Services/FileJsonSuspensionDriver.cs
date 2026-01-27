// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.BlazorServer.Services;

/// <summary>
/// FileJsonSuspensionDriver.
/// </summary>
/// <seealso cref="ISuspensionDriver" />
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
    RxSchedulers.TaskpoolScheduler);

    /// <summary>
    /// Loads the application state from persistent storage.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the state.
    /// </typeparam>
    /// <param name="typeInfo">
    /// The type information.
    /// </param>
    /// <returns>
    /// An object observable.
    /// </returns>
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo) => throw new NotImplementedException();

    /// <summary>
    /// Returns an observable sequence that emits the current application state when subscribed.
    /// </summary>
    /// <returns>An observable sequence that produces the current state object, or null if no state is available.</returns>
    /// <exception cref="NotImplementedException">The method is not implemented.</exception>
    public IObservable<object?> LoadState() => throw new NotImplementedException();

    /// <summary>
    /// Saves the application state to disk.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the state.
    /// </typeparam>
    /// <param name="state">The application state.</param>
    /// <returns>
    /// A completed observable.
    /// </returns>
    public IObservable<Unit> SaveState<T>(T state) => throw new NotImplementedException();

    /// <summary>
    /// Saves the application state to disk.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the state.
    /// </typeparam>
    /// <param name="state">The application state.</param>
    /// <param name="typeInfo">The type information.</param>
    /// <returns>
    /// A completed observable.
    /// </returns>
    public IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo) => throw new NotImplementedException();
}
