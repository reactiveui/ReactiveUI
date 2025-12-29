// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="TransitioningContentControl"/>.
/// </summary>
[NotInParallel]
public class TransitioningContentControlTest
{
    /// <summary>
    /// Tests that Transition property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Transition_SetAndGet_WorksCorrectly()
    {
        var control = new TransitioningContentControl
        {
            Transition = TransitioningContentControl.TransitionType.Fade
        };

        await Assert.That(control.Transition).IsEqualTo(TransitioningContentControl.TransitionType.Fade);

        control.Transition = TransitioningContentControl.TransitionType.Move;

        await Assert.That(control.Transition).IsEqualTo(TransitioningContentControl.TransitionType.Move);
    }

    /// <summary>
    /// Tests that Direction property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Direction_SetAndGet_WorksCorrectly()
    {
        var control = new TransitioningContentControl
        {
            Direction = TransitioningContentControl.TransitionDirection.Left
        };

        await Assert.That(control.Direction).IsEqualTo(TransitioningContentControl.TransitionDirection.Left);

        control.Direction = TransitioningContentControl.TransitionDirection.Right;

        await Assert.That(control.Direction).IsEqualTo(TransitioningContentControl.TransitionDirection.Right);
    }

    /// <summary>
    /// Tests that Duration property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Duration_SetAndGet_WorksCorrectly()
    {
        var control = new TransitioningContentControl
        {
            Duration = TimeSpan.FromSeconds(0.5)
        };

        await Assert.That(control.Duration).IsEqualTo(TimeSpan.FromSeconds(0.5));

        control.Duration = TimeSpan.FromSeconds(1.0);

        await Assert.That(control.Duration).IsEqualTo(TimeSpan.FromSeconds(1.0));
    }

    /// <summary>
    /// Tests that all transition types are supported.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Transition_AllTypes_CanBeSet()
    {
        var control = new TransitioningContentControl();
        var types = new[]
        {
            TransitioningContentControl.TransitionType.Fade,
            TransitioningContentControl.TransitionType.Move,
            TransitioningContentControl.TransitionType.Slide,
            TransitioningContentControl.TransitionType.Drop,
            TransitioningContentControl.TransitionType.Bounce
        };

        foreach (var type in types)
        {
            control.Transition = type;
            await Assert.That(control.Transition).IsEqualTo(type);
        }
    }

    /// <summary>
    /// Tests that all transition directions are supported.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Direction_AllDirections_CanBeSet()
    {
        var control = new TransitioningContentControl();
        var directions = new[]
        {
            TransitioningContentControl.TransitionDirection.Up,
            TransitioningContentControl.TransitionDirection.Down,
            TransitioningContentControl.TransitionDirection.Left,
            TransitioningContentControl.TransitionDirection.Right
        };

        foreach (var direction in directions)
        {
            control.Direction = direction;
            await Assert.That(control.Direction).IsEqualTo(direction);
        }
    }

    /// <summary>
    /// Tests that TransitionProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task TransitionProperty_IsRegistered()
    {
        await Assert.That(TransitioningContentControl.TransitionProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that TransitionDirectionProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task TransitionDirectionProperty_IsRegistered()
    {
        await Assert.That(TransitioningContentControl.TransitionDirectionProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that TransitionDurationProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task TransitionDurationProperty_IsRegistered()
    {
        await Assert.That(TransitioningContentControl.TransitionDurationProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that OverrideDpi can be set to true.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task OverrideDpi_CanBeSet()
    {
        TransitioningContentControl.OverrideDpi = true;
        await Assert.That(TransitioningContentControl.OverrideDpi).IsTrue();

        TransitioningContentControl.OverrideDpi = false;
        await Assert.That(TransitioningContentControl.OverrideDpi).IsFalse();
    }

    /// <summary>
    /// Tests that Content property can be set.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Content_SetAndGet_WorksCorrectly()
    {
        var control = new TransitioningContentControl();
        var content = new TextBlock { Text = "Test Content" };

        control.Content = content;

        await Assert.That(control.Content).IsEqualTo(content);
    }

    /// <summary>
    /// Tests that control can be created with default values.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Constructor_CreatesControlWithDefaults()
    {
        var control = new TransitioningContentControl();

        await Assert.That(control).IsNotNull();
        await Assert.That(control.Transition).IsEqualTo(TransitioningContentControl.TransitionType.Fade);
        await Assert.That(control.Direction).IsEqualTo(TransitioningContentControl.TransitionDirection.Left);
        await Assert.That(control.Duration).IsEqualTo(TimeSpan.FromSeconds(0.3));
    }
}
