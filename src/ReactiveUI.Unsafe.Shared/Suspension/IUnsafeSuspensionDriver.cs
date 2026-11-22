// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Represents a suspension driver that also supports reflection-based state serialization.</summary>
/// <remarks>
/// For trimming and ahead-of-time compilation, prefer <see cref="ISuspensionDriver"/> with source-generated
/// <see cref="System.Text.Json.Serialization.Metadata.JsonTypeInfo{T}"/> overloads.
/// </remarks>
public interface IUnsafeSuspensionDriver : ISuspensionDriver
{
    /// <summary>Saves the application state to persistent storage using reflection-based serialization.</summary>
    /// <typeparam name="T">The type of the application state.</typeparam>
    /// <param name="state">The application state to persist.</param>
    /// <returns>An observable that completes when the state has been saved.</returns>
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    IObservable<RxVoid> SaveState<T>(T state);

    /// <summary>Loads the application state from persistent storage using reflection-based serialization.</summary>
    /// <returns>An observable that produces the deserialized application state (or <see langword="null"/>).</returns>
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    IObservable<object?> LoadState();
}
