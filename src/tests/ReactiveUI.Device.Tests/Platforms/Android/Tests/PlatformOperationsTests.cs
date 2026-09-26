// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests <see cref="PlatformOperations"/> against the emulator's real display.</summary>
public class PlatformOperationsTests
{
    /// <summary>The orientation is the default display's rotation name.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task GetOrientation_ReturnsTheDisplayRotation()
    {
        var orientation = new PlatformOperations().GetOrientation();

        await Assert.That(orientation).IsNotNull();
        await Assert.That(Enum.GetNames<SurfaceOrientation>()).Contains(orientation!);
    }

    /// <summary>The orientation reads the same from a background thread as from the main thread.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task GetOrientation_IsTheSameOnAnyThread()
    {
        var operations = new PlatformOperations();

        var onMain = await MainThread.RunAsync(operations.GetOrientation);
        var onPool = await Task.Run(operations.GetOrientation);

        await Assert.That(onPool).IsEqualTo(onMain);
    }
}
