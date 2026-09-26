// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Dispatching;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// A minimal <see cref="IDispatcher"/> that runs every action in place. The examples pass this to
/// <c>WithMauiScheduler</c> and <c>UseReactiveUI</c> so they build a main-thread scheduler without a running MAUI
/// app, which has no dispatcher for the calling thread to find.
/// </summary>
[System.Diagnostics.DebuggerDisplay("ImmediateDispatcher")]
public sealed class ImmediateDispatcher : IDispatcher
{
    /// <inheritdoc/>
    public bool IsDispatchRequired => false;

    /// <inheritdoc/>
    public bool Dispatch(Action action)
    {
        action();
        return true;
    }

    /// <inheritdoc/>
    public bool DispatchDelayed(TimeSpan delay, Action action)
    {
        action();
        return true;
    }

    /// <inheritdoc/>
    public IDispatcherTimer CreateTimer() => throw new NotSupportedException("This example never schedules delayed work.");
}
