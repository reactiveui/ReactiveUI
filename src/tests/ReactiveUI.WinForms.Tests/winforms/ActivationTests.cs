// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.WinForms.Tests.Winforms.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>Tests to make sure the activation works correctly.</summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]
public class ActivationTests
{
    /// <summary>The expected affinity returned for supported WinForms view types.</summary>
    private const int ExpectedAffinity = 10;

    /// <summary>The expected number of activation notifications on the second pass.</summary>
    private const int ExpectedSecondCount = 2;

    /// <summary>Tests activations for view fetcher supports default winforms components.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ActivationForViewFetcherSupportsDefaultWinformsComponents()
    {
        var target = new ActivationForViewFetcher();
        foreach (var c in new[] { typeof(Control), typeof(UserControl), typeof(Form) })
        {
            await Assert.That(target.GetAffinityForView(c)).IsEqualTo(ExpectedAffinity);
        }
    }

    /// <summary>Tests that determines whether this instance [can fetch activator for form].</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanFetchActivatorForForm()
    {
        var form = new TestForm();
        var target = new ActivationForViewFetcher();
        var formActivator = target.GetActivationForView(form);

        await Assert.That(formActivator).IsNotNull();
    }

    /// <summary>Tests that determines whether this instance [can fetch activator for control].</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanFetchActivatorForControl()
    {
        var control = new TestControl();
        var target = new ActivationForViewFetcher();
        var activator = target.GetActivationForView(control);

        await Assert.That(activator).IsNotNull();
    }

    /// <summary>Smokes the test windows form.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SmokeTestWindowsForm()
    {
        var target = new ActivationForViewFetcher();
        using var form = new TestForm();
        var formActivator = target.GetActivationForView(form);

        var formActivateCount = 0;
        var formDeActivateCount = 0;
        formActivator.Subscribe(activated =>
        {
            if (activated)
            {
                formActivateCount++;
            }
            else
            {
                formDeActivateCount++;
            }
        });

        using (Assert.Multiple())
        {
            await Assert.That(formActivateCount).IsEqualTo(0);
            await Assert.That(formDeActivateCount).IsEqualTo(0);
        }

        form.Visible = true;
        await Assert.That(formActivateCount).IsEqualTo(1);

        form.Visible = false;
        using (Assert.Multiple())
        {
            await Assert.That(formActivateCount).IsEqualTo(1);
            await Assert.That(formDeActivateCount).IsEqualTo(1);
        }

        form.Visible = true;
        await Assert.That(formActivateCount).IsEqualTo(ExpectedSecondCount);

        form.Close();
        await Assert.That(formDeActivateCount).IsEqualTo(ExpectedSecondCount);
    }

    /// <summary>Smokes the test user control.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SmokeTestUserControl()
    {
        var target = new ActivationForViewFetcher();
        using var userControl = new TestControl();
        using var parent = new TestForm();
        var userControlActivator = target.GetActivationForView(userControl);

        var userControlActivateCount = 0;
        var userControlDeActivateCount = 0;
        userControlActivator.Subscribe(activated =>
        {
            if (activated)
            {
                userControlActivateCount++;
            }
            else
            {
                userControlDeActivateCount++;
            }
        });

        parent.Visible = true;
        parent.Controls.Add(userControl);

        userControl.Visible = true;
        await Assert.That(userControlActivateCount).IsEqualTo(1);
        userControl.Visible = false;
        await Assert.That(userControlDeActivateCount).IsEqualTo(1);

        userControl.Visible = true;
        await Assert.That(userControlActivateCount).IsEqualTo(ExpectedSecondCount);

        // closing the form deactivated the usercontrol
        parent.Close();
        await Assert.That(userControlDeActivateCount).IsEqualTo(ExpectedSecondCount);
    }

    /// <summary>Tests that view activation is skipped in design mode.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ActivationIsSkippedInDesignMode()
    {
        using var control = new DesignModeTestControl
        {
            Site = new DesignModeSite(),
        };

        _ = control.Handle;

        await Assert.That(control.Activated).IsFalse();
    }

    /// <summary>Tests that view activation is not skipped outside of design mode.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ActivationIsNotSkippedNotInDesignMode()
    {
        using var control = new DesignModeTestControl();

        _ = control.Handle;

        await Assert.That(control.Activated).IsTrue();
    }
}
