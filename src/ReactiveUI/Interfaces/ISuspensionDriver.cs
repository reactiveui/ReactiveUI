// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text.Json.Serialization.Metadata;

namespace ReactiveUI;

/// <summary>
/// Represents a driver capable of loading and saving application state
/// to persistent storage.
/// </summary>
/// <remarks>
/// <para>
/// This interface supports both legacy reflection-based serialization
/// and trimming/AOT-safe serialization using System.Text.Json source generation.
/// </para>
/// <para>
/// Implementations that support trimming or AOT should prefer the overloads
/// that accept <see cref="JsonTypeInfo{T}"/>.
/// </para>
/// </remarks>
public interface ISuspensionDriver
{
    /// <summary>
    /// Loads the application state from persistent storage.
    /// </summary>
    /// <returns>
    /// An observable that produces the deserialized application state
    /// (or <see langword="null"/>).
    /// </returns>
    /// <remarks>
    /// This member typically relies on reflection-based serialization and is not
    /// trimming or AOT friendly.
    /// </remarks>
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. " +
        "Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. " +
        "Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    IObservable<object?> LoadState();

    /// <summary>
    /// Saves the application state to persistent storage.
    /// </summary>
    /// <typeparam name="T">The type of the application state.</typeparam>
    /// <param name="state">The application state to persist.</param>
    /// <returns>
    /// An observable that completes when the state has been saved.
    /// </returns>
    /// <remarks>
    /// This member typically relies on reflection-based serialization and is not
    /// trimming or AOT friendly.
    /// </remarks>
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. " +
        "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. " +
        "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    IObservable<Unit> SaveState<T>(T state);

    /// <summary>
    /// Loads application state from persistent storage using
    /// source-generated System.Text.Json metadata.
    /// </summary>
    /// <typeparam name="T">The expected state type.</typeparam>
    /// <param name="typeInfo">
    /// The source-generated metadata for <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// An observable that produces the deserialized state
    /// (or <see langword="null"/>).
    /// </returns>
    IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo);

    /// <summary>
    /// Saves application state to persistent storage using
    /// source-generated System.Text.Json metadata.
    /// </summary>
    /// <typeparam name="T">The state type.</typeparam>
    /// <param name="state">The state to persist.</param>
    /// <param name="typeInfo">
    /// The source-generated metadata for <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// An observable that completes when persistence succeeds.
    /// </returns>
    IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo);

    /// <summary>
    /// Invalidates the persisted application state
    /// (for example, by deleting it from disk).
    /// </summary>
    /// <returns>
    /// An observable that completes when the state has been invalidated.
    /// </returns>
    IObservable<Unit> InvalidateState();
}
