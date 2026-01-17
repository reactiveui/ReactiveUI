// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Core;

/// <summary>
///     Tests for ReactiveUI internal utility classes.
/// </summary>
public class InternalUtilitiesTests
{
    /// <summary>
    ///     Tests that NotAWeakReference always holds strong reference even after GC.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NotAWeakReference_AfterGC_StillAlive()
    {
        // Arrange
        const string target = "test target";
        var weakRef = new NotAWeakReference(target);

        // Act - Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();

        using (Assert.Multiple())
        {
            // Assert - NotAWeakReference always holds strong reference
            await Assert.That(weakRef.IsAlive).IsTrue();
            await Assert.That(weakRef.Target).IsEqualTo(target);
        }
    }

    /// <summary>
    ///     Tests NotAWeakReference Target and IsAlive properties.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NotAWeakReference_Constructor_StoresTarget()
    {
        // Arrange
        const string target = "test target";

        // Act
        var weakRef = new NotAWeakReference(target);

        using (Assert.Multiple())
        {
            // Assert
            await Assert.That(weakRef.Target).IsEqualTo(target);
            await Assert.That(weakRef.IsAlive).IsTrue();
        }
    }
}
