// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

namespace ReactiveUI.Tests.Winforms;

/// <summary>
/// Tests to make sure the activation works correctly.
/// </summary>
public class ActivationTests : AppBuilderTestBase
{
    /// <summary>
    /// Tests activations for view fetcher supports default winforms components.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task ActivationForViewFetcherSupportsDefaultWinformsComponents() =>
        await RunAppBuilderTestAsync(() =>
        {
            var target = new ReactiveUI.Winforms.ActivationForViewFetcher();
            var supportedComponents = new[] { typeof(Control), typeof(UserControl), typeof(Form) };

            foreach (var c in supportedComponents)
            {
                Assert.Equal(10, target.GetAffinityForView(c));
            }
        });

    /// <summary>
    /// Tests that determines whether this instance [can fetch activator for form].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task CanFetchActivatorForForm() =>
        await RunAppBuilderTestAsync(() =>
        {
            var form = new TestForm();
            var target = new ReactiveUI.Winforms.ActivationForViewFetcher();
            var formActivator = target.GetActivationForView(form);

            Assert.NotNull(formActivator);
        });

    /// <summary>
    /// Tests that determines whether this instance [can fetch activator for control].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task CanFetchActivatorForControl() =>
        await RunAppBuilderTestAsync(() =>
        {
            var control = new TestControl();
            var target = new ReactiveUI.Winforms.ActivationForViewFetcher();
            var activator = target.GetActivationForView(control);

            Assert.NotNull(activator);
        });

    /// <summary>
    /// Smokes the test windows form.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SmokeTestWindowsForm() =>
        await RunAppBuilderTestAsync(() =>
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

                Assert.Equal(0, formActivateCount);
                Assert.Equal(0, formDeActivateCount);

                form.Visible = true;
                Assert.Equal(1, formActivateCount);

                form.Visible = false;
                Assert.Equal(1, formActivateCount);
                Assert.Equal(1, formDeActivateCount);

                form.Visible = true;
                Assert.Equal(2, formActivateCount);

                form.Close();
                Assert.Equal(2, formDeActivateCount);
            }
        });

    /// <summary>
    /// Smokes the test user control.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SmokeTestUserControl() =>
        await RunAppBuilderTestAsync(() =>
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
                Assert.Equal(1, userControlActivateCount);
                userControl.Visible = false;
                Assert.Equal(1, userControlDeActivateCount);

                userControl.Visible = true;
                Assert.Equal(2, userControlActivateCount);

                // closing the form deactivated the usercontrol
                parent.Close();
                Assert.Equal(2, userControlDeActivateCount);
            }
        });
}
