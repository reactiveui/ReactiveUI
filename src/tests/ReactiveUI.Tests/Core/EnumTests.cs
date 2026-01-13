// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Core;

/// <summary>
///     Tests for ReactiveUI enum types to ensure backwards compatibility.
/// </summary>
public class EnumTests
{
    /// <summary>
    ///     Tests BindingDirection enum values for backwards compatibility.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [SuppressMessage(
        "Usage",
        "TUnitAssertions0005:Assert.That(...) should not be used with a constant value",
        Justification = "Verifying enum values remain constant for backwards compatibility")]
    public async Task BindingDirection_EnumValues_AreConstant()
    {
        using (Assert.Multiple())
        {
            // Assert
            await Assert.That((int)BindingDirection.OneWay).IsEqualTo(0);
            await Assert.That((int)BindingDirection.TwoWay).IsEqualTo(1);
            await Assert.That((int)BindingDirection.AsyncOneWay).IsEqualTo(2);
        }
    }

    /// <summary>
    ///     Tests TriggerUpdate enum values for backwards compatibility.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [SuppressMessage(
        "Usage",
        "TUnitAssertions0005:Assert.That(...) should not be used with a constant value",
        Justification = "Verifying enum values remain constant for backwards compatibility")]
    public async Task TriggerUpdate_EnumValues_AreConstant()
    {
        using (Assert.Multiple())
        {
            // Assert
            await Assert.That((int)TriggerUpdate.ViewToViewModel).IsEqualTo(0);
            await Assert.That((int)TriggerUpdate.ViewModelToView).IsEqualTo(1);
        }
    }
}
