// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

namespace ReactiveUI.Tests.Winforms;

/// <summary>
/// Tests to make sure the activation works correctly.
/// </summary>
[TestFixture]
public class ActivationTests
{
    /// <summary>
    /// Tests activations for view fetcher supports default winforms components.
    /// </summary>
    [Test]
    public void ActivationForViewFetcherSupportsDefaultWinformsComponents()
    {
        var target = new ReactiveUI.Winforms.ActivationForViewFetcher();
        var supportedComponents = new[] { typeof(Control), typeof(UserControl), typeof(Form) };

        foreach (var c in supportedComponents)
        {
            Assert.That(target.GetAffinityForView(c, Is.EqualTo(10)));
        }
    }

    /// <summary>
    /// Tests that determines whether this instance [can fetch activator for form].
    /// </summary>
    [Test]
    public void CanFetchActivatorForForm()
    {
        var form = new TestForm();
        var target = new ReactiveUI.Winforms.ActivationForViewFetcher();
        var formActivator = target.GetActivationForView(form);

        Assert.That(formActivator, Is.Not.Null);
    }

    /// <summary>
    /// Tests that determines whether this instance [can fetch activator for control].
    /// </summary>
    [Test]
    public void CanFetchActivatorForControl()
    {
        var control = new TestControl();
        var target = new ReactiveUI.Winforms.ActivationForViewFetcher();
        var activator = target.GetActivationForView(control);

        Assert.That(activator, Is.Not.Null);
    }

    /// <summary>
    /// Smokes the test windows form.
    /// </summary>
    [Test]
    public void SmokeTestWindowsForm()
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

            Assert.That(formActivateCount, Is.EqualTo(0));
            Assert.That(formDeActivateCount, Is.EqualTo(0));

            form.Visible = true;
            Assert.That(formActivateCount, Is.EqualTo(1));

            form.Visible = false;
            Assert.That(formActivateCount, Is.EqualTo(1));
            Assert.That(formDeActivateCount, Is.EqualTo(1));

            form.Visible = true;
            Assert.That(formActivateCount, Is.EqualTo(2));

            form.Close();
            Assert.That(formDeActivateCount, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// Smokes the test user control.
    /// </summary>
    [Test]
    public void SmokeTestUserControl()
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
            Assert.That(userControlActivateCount, Is.EqualTo(1));
            userControl.Visible = false;
            Assert.That(userControlDeActivateCount, Is.EqualTo(1));

            userControl.Visible = true;
            Assert.That(userControlActivateCount, Is.EqualTo(2));

            // closing the form deactivated the usercontrol
            parent.Close();
            Assert.That(userControlDeActivateCount, Is.EqualTo(2));
        }
    }
}
