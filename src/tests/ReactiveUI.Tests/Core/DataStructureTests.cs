// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Core;

/// <summary>
///     Tests for ReactiveUI data structure types.
/// </summary>
public class DataStructureTests
{
    /// <summary>
    ///     Tests ObservedChange constructor and properties.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservedChange_Constructor_StoresAllValues()
    {
        // Arrange
        const string Sender = "test sender";
        var expression = System.Linq.Expressions.Expression.Constant("test");
        const int Value = 42;

        // Act
        var observedChange = new ObservedChange<string, int>(
            Sender,
            expression,
            Value);

        using (Assert.Multiple())
        {
            // Assert
            await Assert.That(observedChange.Sender).IsEqualTo(Sender);
            await Assert.That(observedChange.Expression).IsEqualTo(expression);
            await Assert.That(observedChange.Value).IsEqualTo(Value);
        }
    }

    /// <summary>
    ///     Tests ReactivePropertyChangedEventArgs constructor and properties.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactivePropertyChangedEventArgs_Constructor_StoresValues()
    {
        // Arrange
        const string Sender = "test sender";
        const string PropertyName = "TestProperty";

        // Act
        var eventArgs = new ReactivePropertyChangedEventArgs<string>(
            Sender,
            PropertyName);

        using (Assert.Multiple())
        {
            // Assert
            await Assert.That(eventArgs.Sender).IsEqualTo(Sender);
            await Assert.That(eventArgs.PropertyName).IsEqualTo(PropertyName);
        }
    }
}
