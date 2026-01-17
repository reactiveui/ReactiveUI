// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

using ReactiveUI.WinForms.Tests.Winforms.Mocks;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>
/// Tests to make sure the activation works correctly.
/// </summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]
public class ActivationTests
{
    /// <summary>
    /// Tests activations for view fetcher supports default winforms components.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ActivationForViewFetcherSupportsDefaultWinformsComponents()
    {
        var target = new ReactiveUI.Winforms.ActivationForViewFetcher();
        foreach (var c in new[] { typeof(Control), typeof(UserControl), typeof(Form) })
        {
            await Assert.That(target.GetAffinityForView(c)).IsEqualTo(10);
        }
    }

    /// <summary>
    /// Tests that determines whether this instance [can fetch activator for form].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanFetchActivatorForForm()
    {
        var form = new TestForm();
        var target = new ReactiveUI.Winforms.ActivationForViewFetcher();
        var formActivator = target.GetActivationForView(form);

        await Assert.That(formActivator).IsNotNull();
    }

    /// <summary>
    /// Tests that determines whether this instance [can fetch activator for control].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanFetchActivatorForControl()
    {
        var control = new TestControl();
        var target = new ReactiveUI.Winforms.ActivationForViewFetcher();
        var activator = target.GetActivationForView(control);

        await Assert.That(activator).IsNotNull();
    }

    /// <summary>
    /// Smokes the test windows form.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SmokeTestWindowsForm()
    {
        var target = new ReactiveUI.Winforms.ActivationForViewFetcher();
        using (var form = new TestForm())
        {
            var formActivator = target.GetActivationForView(form);

            int formActivateCount = 0, formDeActivateCount = 0;
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
            await Assert.That(formActivateCount).IsEqualTo(2);

            form.Close();
            await Assert.That(formDeActivateCount).IsEqualTo(2);
        }
    }

    /// <summary>
    /// Smokes the test user control.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SmokeTestUserControl()
    {
        var target = new ReactiveUI.Winforms.ActivationForViewFetcher();
        using (var userControl = new TestControl())
        using (var parent = new TestForm())
        {
            var userControlActivator = target.GetActivationForView(userControl);

            int userControlActivateCount = 0, userControlDeActivateCount = 0;
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
            await Assert.That(userControlActivateCount).IsEqualTo(2);

            // closing the form deactivated the usercontrol
            parent.Close();
            await Assert.That(userControlDeActivateCount).IsEqualTo(2);
        }
    }
}
