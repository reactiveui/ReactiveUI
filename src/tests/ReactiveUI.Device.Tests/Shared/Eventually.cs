// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Device.Tests;

/// <summary>Waits for a condition that a platform lifecycle callback makes true.</summary>
internal static class Eventually
{
    /// <summary>The interval between checks.</summary>
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(25);

    /// <summary>Polls <paramref name="condition"/> until it holds or the device test timeout passes.</summary>
    /// <param name="condition">The condition to wait for.</param>
    /// <param name="what">What the test waits for, used in the failure message.</param>
    /// <returns>A task that completes when the condition holds.</returns>
    /// <exception cref="TimeoutException">Thrown when the condition does not hold in time.</exception>
    internal static async Task TrueAsync(Func<bool> condition, string what)
    {
        ArgumentNullException.ThrowIfNull(condition);

        var started = Stopwatch.GetTimestamp();
        while (!condition())
        {
            if (Stopwatch.GetElapsedTime(started) > SignalAwaiterExtensions.DefaultTimeout)
            {
                throw new TimeoutException($"Timed out after {SignalAwaiterExtensions.DefaultTimeout.TotalSeconds:0}s waiting for {what}.");
            }

            await Task.Delay(PollInterval).ConfigureAwait(false);
        }
    }
}
