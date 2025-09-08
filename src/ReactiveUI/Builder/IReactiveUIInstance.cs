// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

namespace ReactiveUI.Builder;

/// <summary>
/// IReactiveUIInstance.
/// </summary>
/// <seealso cref="Splat.Builder.IAppInstance" />
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