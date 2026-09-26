// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Device.Tests;

/// <summary>Hosts controllers in the app's real window and waits for UIKit state, reading it only on the main thread.</summary>
internal static class UIKitHarness
{
    /// <summary>Makes <paramref name="controller"/> the window's root controller, so UIKit shows it and runs its appearance callbacks.</summary>
    /// <param name="controller">The controller to show.</param>
    /// <returns>A task that completes once the controller is the root.</returns>
    internal static Task ShowAsync(UIViewController controller)
    {
        ArgumentNullException.ThrowIfNull(controller);
        return MainThread.RunAsync(() => AppDelegate.Current.Window!.RootViewController = controller);
    }

    /// <summary>Replaces the window's root controller with an empty one, so the previous root disappears.</summary>
    /// <returns>A task that completes once the root is replaced.</returns>
    internal static Task ResetAsync() =>
        MainThread.RunAsync(static () => AppDelegate.Current.Window!.RootViewController = new UIViewController());

    /// <summary>Polls <paramref name="condition"/> on the main thread until it holds or the device test timeout passes.</summary>
    /// <param name="condition">The condition, which reads UIKit state.</param>
    /// <param name="what">What the test waits for, used in the failure message.</param>
    /// <returns>A task that completes when the condition holds.</returns>
    /// <exception cref="TimeoutException">Thrown when the condition does not hold in time.</exception>
    internal static async Task UntilAsync(Func<bool> condition, string what)
    {
        ArgumentNullException.ThrowIfNull(condition);

        var started = System.Diagnostics.Stopwatch.GetTimestamp();
        while (!await MainThread.RunAsync(condition).ConfigureAwait(false))
        {
            if (System.Diagnostics.Stopwatch.GetElapsedTime(started) > SignalAwaiterExtensions.DefaultTimeout)
            {
                throw new TimeoutException($"Timed out after {SignalAwaiterExtensions.DefaultTimeout.TotalSeconds:0}s waiting for {what}.");
            }

            await Task.Delay(TimeSpan.FromMilliseconds(25)).ConfigureAwait(false);
        }
    }
}
