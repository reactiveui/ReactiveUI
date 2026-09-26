// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using UIKit;

namespace ReactiveUI.Device.Tests;

/// <summary>Hosts controllers in the app's real window and waits for UIKit state, reading it only on the main thread.</summary>
/// <remarks>
/// The app has one window, so a test that shows a controller owns it until the test ends. Mark every test class that
/// shows controllers with <c>[NotInParallel(UIKitHarness.WindowKey)]</c>, so no other test replaces its root midway.
/// </remarks>
internal static class UIKitHarness
{
    /// <summary>The constraint key for tests that show controllers in the shared window.</summary>
    internal const string WindowKey = "UIKitWindow";

    /// <summary>The interval between checks.</summary>
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(25);

    /// <summary>Makes <paramref name="controller"/> the window's root controller, so UIKit shows it and runs its appearance callbacks.</summary>
    /// <param name="controller">The controller to show.</param>
    /// <returns>A task that completes once the controller is the root.</returns>
    internal static Task ShowAsync(UIViewController controller)
    {
        ArgumentNullException.ThrowIfNull(controller);
        return MainThread.RunAsync(() => IosTestHost.Window.RootViewController = controller);
    }

    /// <summary>Replaces the window's root controller with an empty one, so the previous root disappears.</summary>
    /// <returns>A task that completes once the root is replaced.</returns>
    internal static Task ResetAsync() =>
        MainThread.RunAsync(static () => IosTestHost.Window.RootViewController = new());

    /// <summary>
    /// Lets the main run loop turn a few times, so UIKit runs the appearance callbacks it defers until after a
    /// Core Animation commit, and any work queued on the main thread scheduler runs.
    /// </summary>
    /// <returns>A task that completes after the run loop has turned.</returns>
    internal static async Task SettleAsync()
    {
        const int turns = 4;
        for (var i = 0; i < turns; i++)
        {
            await Task.Delay(PollInterval).ConfigureAwait(false);
            await MainThread.RunAsync(static () => { }).ConfigureAwait(false);
        }
    }

    /// <summary>Polls <paramref name="condition"/> on the main thread until it holds or the device test timeout passes.</summary>
    /// <param name="condition">The condition, which reads UIKit state.</param>
    /// <param name="what">What the test waits for, used in the failure message.</param>
    /// <returns>A task that completes when the condition holds.</returns>
    /// <exception cref="TimeoutException">Thrown when the condition does not hold in time.</exception>
    internal static async Task UntilAsync(Func<bool> condition, string what)
    {
        ArgumentNullException.ThrowIfNull(condition);

        var started = Stopwatch.GetTimestamp();
        while (!await MainThread.RunAsync(condition).ConfigureAwait(false))
        {
            if (Stopwatch.GetElapsedTime(started) > SignalAwaiterExtensions.DefaultTimeout)
            {
                throw new TimeoutException($"Timed out after {SignalAwaiterExtensions.DefaultTimeout.TotalSeconds:0}s waiting for {what}.");
            }

            await Task.Delay(PollInterval).ConfigureAwait(false);
        }
    }
}
