// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Dispatching;

namespace ReactiveUI.Maui.Tests;

/// <summary>
/// Test dispatcher that executes actions synchronously for unit testing.
/// Based on MAUI's internal test infrastructure.
/// </summary>
internal class TestDispatcher : IDispatcher
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
    public IDispatcherTimer CreateTimer()
    {
        throw new NotImplementedException("CreateTimer is not supported in test dispatcher");
    }
}
