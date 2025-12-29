// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Winforms;

/// <summary>
/// Tests for <see cref="PlatformOperations"/>.
/// </summary>
[NotInParallel]
public class PlatformOperationsTest
{
    /// <summary>
    /// Tests that GetOrientation returns null on Windows.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetOrientation_ReturnsNull()
    {
        var platformOps = new ReactiveUI.Winforms.PlatformOperations();

        var orientation = platformOps.GetOrientation();

        await Assert.That(orientation).IsNull();
    }
}
