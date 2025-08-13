// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// ISuspensionDriver represents a class that can load/save state to persistent
/// storage. Most platforms have a basic implementation of this class, but you
/// probably want to write your own.
/// </summary>
public interface ISuspensionDriver
{
    /// <summary>
    /// Loads the application state from persistent storage.
    /// </summary>
    /// <returns>An object observable.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("LoadState implementations may use serialization which requires dynamic code generation")]
    [RequiresUnreferencedCode("LoadState implementations may use serialization which may require unreferenced code")]
#endif
    IObservable<object?> LoadState();

    /// <summary>
    /// Saves the application state to disk.
    /// </summary>
    /// <param name="state">The application state.</param>
    /// <returns>A completed observable.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("SaveState implementations may use serialization which requires dynamic code generation")]
    [RequiresUnreferencedCode("SaveState implementations may use serialization which may require unreferenced code")]
#endif
    IObservable<Unit> SaveState(object state);

    /// <summary>
    /// Invalidates the application state (i.e. deletes it from disk).
    /// </summary>
    /// <returns>A completed observable.</returns>
    IObservable<Unit> InvalidateState();
}
