// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;

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
    IObservable<object> LoadState();

    /// <summary>
    /// Saves the application state to disk.
    /// </summary>
    /// <param name="state">The application state.</param>
    /// <returns>A completed observable.</returns>
    IObservable<Unit> SaveState(object state);

    /// <summary>
    /// Invalidates the application state (i.e. deletes it from disk).
    /// </summary>
    /// <returns>A completed observable.</returns>
    IObservable<Unit> InvalidateState();
}