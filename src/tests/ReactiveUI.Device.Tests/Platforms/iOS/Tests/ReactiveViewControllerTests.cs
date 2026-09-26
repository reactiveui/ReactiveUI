// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Device.Tests;

/// <summary>Tests <see cref="ReactiveViewController{TViewModel}"/> activation as UIKit shows and hides it.</summary>
[NotInParallel(UIKitHarness.WindowKey)]
public class ReactiveViewControllerTests
{
    /// <summary>A controller activates when it appears and deactivates when it disappears.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task Controller_ActivatesOnAppearAndDeactivatesOnDisappear()
    {
        var controller = await MainThread.RunAsync(static () => new TestViewController());

        await UIKitHarness.ShowAsync(controller);
        await Eventually.TrueAsync(() => controller.Activations == 1, "the controller to activate");
        await Assert.That(controller.Deactivations).IsEqualTo(0);

        await UIKitHarness.ResetAsync();
        await Eventually.TrueAsync(() => controller.Deactivations == 1, "the controller to deactivate");
    }

    /// <summary>Setting the view model raises <c>Changed</c>.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task SettingViewModel_RaisesChanged()
    {
        var controller = await MainThread.RunAsync(static () => new TestViewController());
        var changed = controller.Changed.FirstValueAsync(out var subscription);
        using (subscription)
        {
            await MainThread.RunAsync(() => controller.ViewModel = new("vm"));

            var change = await changed.WithTimeout("the Changed notification");
            await Assert.That(change.PropertyName).IsEqualTo(nameof(TestViewController.ViewModel));
        }
    }
}
