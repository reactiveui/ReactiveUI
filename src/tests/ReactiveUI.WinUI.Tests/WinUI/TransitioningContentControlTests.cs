// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Tests for <see cref="TransitioningContentControl"/>.</summary>
[NotInParallel]
[TestExecutor<WinUITestExecutor>]
public class TransitioningContentControlTests
{
    /// <summary>Verifies the control is a content control, so hosts deriving from it can present a view.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_ProducesAContentControl()
    {
        var control = new TransitioningContentControl();

        _ = await Assert.That(control).IsAssignableTo<ContentControl>();
    }

    /// <summary>Verifies content assigned to the control is retained.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Content_RoundTrips()
    {
        var control = new TransitioningContentControl { Content = "hosted" };

        await Assert.That(control.Content).IsEqualTo("hosted");
    }
}
