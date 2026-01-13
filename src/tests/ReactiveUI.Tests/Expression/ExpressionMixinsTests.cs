// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.ReactiveObjects.Mocks;

namespace ReactiveUI.Tests.Expression;

/// <summary>
///     Tests for ExpressionMixins utility methods.
/// </summary>
public class ExpressionMixinsTests
{
    /// <summary>
    ///     Tests that GetMemberInfo handles nested property access.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetMemberInfo_NestedPropertyExpression_ReturnsPropertyName()
    {
        // Arrange
        Expression<Func<TestFixture, int>> expression = x => x.IsNotNullString!.Length;

        // Act
        var memberInfo = expression.Body.GetMemberInfo();

        // Assert
        await Assert.That(memberInfo).IsNotNull();
        await Assert.That(memberInfo!.Name).IsEqualTo("Length");
    }

    /// <summary>
    ///     Tests that GetMemberInfo returns correct member name.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetMemberInfo_PropertyExpression_ReturnsPropertyName()
    {
        // Arrange
        Expression<Func<TestFixture, string?>> expression = x => x.IsNotNullString;

        // Act
        var memberInfo = expression.Body.GetMemberInfo();

        // Assert
        await Assert.That(memberInfo).IsNotNull();
        await Assert.That(memberInfo!.Name).IsEqualTo("IsNotNullString");
    }
}
