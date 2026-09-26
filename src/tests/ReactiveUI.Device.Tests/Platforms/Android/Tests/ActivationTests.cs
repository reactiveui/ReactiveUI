// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AndroidResult = Android.App.Result;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests activation of reactive activities and fragments as Android drives their real lifecycle.</summary>
public class ActivationTests
{
    /// <summary>A reactive activity activates when it resumes and deactivates when it pauses.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task ReactiveActivity_ActivatesOnResumeAndDeactivatesOnPause()
    {
        var activity = await ActivityLauncher.StartAsync<ActivatingActivity>();

        await Eventually.TrueAsync(() => activity.Activations == 1, "the activity to activate");
        await Assert.That(activity.Deactivations).IsEqualTo(0);

        await ActivityLauncher.FinishAsync(activity);

        await Eventually.TrueAsync(() => activity.Deactivations == 1, "the activity to deactivate");
    }

    /// <summary>The <c>Activated</c> and <c>Deactivated</c> streams follow the activity's pause and resume.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task ReactiveActivity_DeactivatedFiresWhenTheActivityPauses()
    {
        var activity = await ActivityLauncher.StartAsync<ActivatingActivity>();
        var deactivated = activity.Deactivated.FirstValueAsync(out var subscription);
        using (subscription)
        {
            await ActivityLauncher.FinishAsync(activity);

            _ = await deactivated.WithTimeout("the Deactivated signal");
        }
    }

    /// <summary>Starting an activity for a result completes with the result the other activity set.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task ReactiveActivity_StartActivityForResultAsync_ReturnsTheResult()
    {
        const int requestCode = 7;
        var activity = await ActivityLauncher.StartAsync<ActivatingActivity>();
        try
        {
            var pending = await MainThread.RunAsync(() => activity.StartActivityForResultAsync(typeof(ResultSourceActivity), requestCode));

            var (resultCode, data) = await pending.WithTimeout("the activity result");

            await Assert.That(resultCode).IsEqualTo(AndroidResult.Ok);
            await Assert.That(data?.GetIntExtra(ResultSourceActivity.AnswerKey, 0)).IsEqualTo(ResultSourceActivity.Answer);

            // Starting the other activity paused this one, and returning resumed it.
            const int activationsAfterRoundTrip = 2;
            await Eventually.TrueAsync(() => activity.Activations == activationsAfterRoundTrip, "the activity to activate again");
            await Assert.That(activity.Deactivations).IsEqualTo(1);
        }
        finally
        {
            await ActivityLauncher.FinishAsync(activity);
        }
    }

    /// <summary>An AppCompat reactive activity and its AndroidX reactive fragment both activate.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task ReactiveAppCompatActivity_AndFragment_Activate()
    {
        var activity = await ActivityLauncher.StartAsync<CompatHostActivity>();
        try
        {
            await Eventually.TrueAsync(() => activity.Activations == 1, "the AppCompat activity to activate");
            await Eventually.TrueAsync(() => activity.Fragment.Activations == 1, "the fragment to activate");
        }
        finally
        {
            await ActivityLauncher.FinishAsync(activity);
        }

        await Eventually.TrueAsync(() => activity.Fragment.Deactivations == 1, "the fragment to deactivate");
    }

    /// <summary>Setting a reactive activity's view model raises <c>Changed</c>.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task ReactiveActivity_SettingViewModel_RaisesChanged()
    {
        var activity = await ActivityLauncher.StartAsync<ActivatingActivity>();
        try
        {
            var changed = activity.Changed.FirstValueAsync(out var subscription);
            using (subscription)
            {
                await MainThread.RunAsync(() => activity.ViewModel = new("vm"));

                var change = await changed.WithTimeout("the Changed notification");
                await Assert.That(change.PropertyName).IsEqualTo(nameof(ActivatingActivity.ViewModel));
            }
        }
        finally
        {
            await ActivityLauncher.FinishAsync(activity);
        }
    }
}
