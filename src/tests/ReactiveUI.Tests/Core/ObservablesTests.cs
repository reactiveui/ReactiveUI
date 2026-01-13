// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Core;

/// <summary>
///     Tests for Observables static utility class.
/// </summary>
public class ObservablesTests
{
    /// <summary>
    ///     Tests that Observables.False emits false value.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Observables_False_EmitsFalse()
    {
        // Arrange
        bool? result = null;

        // Act
        Observables.False.Subscribe(x => result = x);

        // Assert
        await Assert.That(result).IsFalse();
    }

    /// <summary>
    ///     Tests that Observables static members are accessible.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Observables_StaticMembers_AreAccessible()
    {
        using (Assert.Multiple())
        {
            // Act & Assert
            await Assert.That(Observables.Unit).IsNotNull();
            await Assert.That(Observables.True).IsNotNull();
            await Assert.That(Observables.False).IsNotNull();
        }
    }

    /// <summary>
    ///     Tests that Observables.True emits true value.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Observables_True_EmitsTrue()
    {
        // Arrange
        bool? result = null;

        // Act
        Observables.True.Subscribe(x => result = x);

        // Assert
        await Assert.That(result).IsTrue();
    }

    /// <summary>
    ///     Tests that Observables.Unit emits Unit.Default value.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Observables_Unit_EmitsUnitDefault()
    {
        // Arrange
        Unit? result = null;

        // Act
        Observables.Unit.Subscribe(x => result = x);

        // Assert
        await Assert.That(result).IsEqualTo(Unit.Default);
    }
}
