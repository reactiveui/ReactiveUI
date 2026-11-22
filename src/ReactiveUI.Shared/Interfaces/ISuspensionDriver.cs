// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text.Json.Serialization.Metadata;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Represents a driver capable of loading and saving application state to persistent storage.</summary>
/// <remarks>
/// <para>
/// This interface supports trimming/AOT-safe serialization using System.Text.Json source generation
/// via <see cref="JsonTypeInfo{T}"/>. For legacy reflection-based serialization, use <c>IUnsafeSuspensionDriver</c>
/// from <c>ReactiveUI.Unsafe</c>.
/// </para>
/// </remarks>
public interface ISuspensionDriver
{
    /// <summary>Saves application state to persistent storage using source-generated System.Text.Json metadata.</summary>
    /// <typeparam name="T">The state type.</typeparam>
    /// <param name="state">The state to persist.</param>
    /// <param name="typeInfo">
    /// The source-generated metadata for <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// An observable that completes when persistence succeeds.
    /// </returns>
    IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo);

    /// <summary>Loads application state from persistent storage using source-generated System.Text.Json metadata.</summary>
    /// <typeparam name="T">The expected state type.</typeparam>
    /// <param name="typeInfo">
    /// The source-generated metadata for <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// An observable that produces the deserialized state
    /// (or <see langword="null"/>).
    /// </returns>
    IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo);

    /// <summary>Invalidates the persisted application state (for example, by deleting it from disk).</summary>
    /// <returns>
    /// An observable that completes when the state has been invalidated.
    /// </returns>
    IObservable<RxVoid> InvalidateState();
}
