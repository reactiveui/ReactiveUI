// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Tests for <see cref="PlatformOperations"/>.</summary>
[NotInParallel]
[TestExecutor<WinUITestExecutor>]
public class PlatformOperationsTests
{
    /// <summary>Verifies desktop windows report no orientation, so view contracts are never orientation-specific.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetOrientation_ReturnsNull()
    {
        var operations = new PlatformOperations();

        await Assert.That(operations.GetOrientation()).IsNull();
    }
}
