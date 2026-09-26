// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;

namespace ReactiveUI.Device.Tests;

/// <summary>Starts and finishes real activities through the test instrumentation.</summary>
internal static class ActivityLauncher
{
    /// <summary>Gets the application context of the app under test.</summary>
    internal static Context TargetContext => TestInstrumentation.Current.TargetContext!;

    /// <summary>Starts <typeparamref name="TActivity"/> and waits until it is resumed.</summary>
    /// <typeparam name="TActivity">The activity type, declared in the manifest by its <c>[Activity]</c> attribute.</typeparam>
    /// <returns>The resumed activity.</returns>
    internal static Task<TActivity> StartAsync<TActivity>()
        where TActivity : Activity =>
        Task.Run(static () =>
        {
            var intent = new Intent(TargetContext, typeof(TActivity));
            _ = intent.AddFlags(ActivityFlags.NewTask);

            // StartActivitySync blocks until the activity is resumed and must not run on the main thread.
            return (TActivity)TestInstrumentation.Current.StartActivitySync(intent)!;
        });

    /// <summary>Finishes <paramref name="activity"/> and waits for the main thread to go idle.</summary>
    /// <param name="activity">The activity to finish.</param>
    /// <returns>A task that completes once the finish has been processed.</returns>
    internal static async Task FinishAsync(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);

        await MainThread.RunAsync(activity.Finish).ConfigureAwait(false);
        await Task.Run(TestInstrumentation.Current.WaitForIdleSync).ConfigureAwait(false);
    }
}
