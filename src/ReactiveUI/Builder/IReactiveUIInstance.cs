// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

namespace ReactiveUI.Builder;

/// <summary>
/// Defines an application instance that provides access to schedulers for UI and background operations in a reactive
/// programming context.
/// </summary>
/// <remarks>Implementations of this interface supply schedulers that allow code to schedule work on the UI thread
/// or on background threads, facilitating reactive and asynchronous programming patterns. The specific scheduler
/// implementations may vary depending on the application's execution mode (e.g., normal runtime or unit
/// testing).</remarks>
public interface IReactiveUIInstance : IAppInstance
{
    /// <summary>
    /// Gets a scheduler used to schedule work items that
    /// should be run "on the UI thread". In normal mode, this will be
    /// DispatcherScheduler, and in Unit Test mode this will be Immediate,
    /// to simplify writing common unit tests.
    /// </summary>
    IScheduler? MainThreadScheduler { get; }

    /// <summary>
    /// Gets the a the scheduler used to schedule work items to
    /// run in a background thread. In both modes, this will run on the TPL
    /// Task Pool.
    /// </summary>
    IScheduler? TaskpoolScheduler { get; }
}
