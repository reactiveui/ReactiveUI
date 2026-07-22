// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using ReactiveUI.Tests.ReactiveObjects.Mocks;

namespace ReactiveUI.Tests.Expressions;

/// <summary>Tests for ExpressionMixins utility methods.</summary>
public class ExpressionMixinsTests
{
    /// <summary>Tests that GetMemberInfo handles nested property access.</summary>
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

    /// <summary>Tests that GetMemberInfo returns correct member name.</summary>
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

    /// <summary>The chain normalizes both a nested indexer (receiver rewritten to a parameter placeholder) and a
    /// top-level indexer (receiver already the parameter, left unchanged), covering both branches of the index
    /// normalization.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetExpressionChainNormalizesNestedAndTopLevelIndexers()
    {
        Expression<Func<IndexerRoot, string>> nested = x => x.Leaf[0];
        Expression<Func<IndexerRoot, string>> topLevel = x => x[0];

        // Rewrite turns the get_Item method calls into IndexExpression nodes, which is what GetExpressionChain
        // normalizes. The nested receiver (x.Leaf) is not the parameter, so it is rewritten; the top-level receiver
        // (x) already is the parameter, so it is left as-is.
        const int NestedChainLength = 2;
        const int TopLevelChainLength = 1;

        var nestedChain = Reflection.Rewrite(nested.Body).GetExpressionChain().ToList();
        var topLevelChain = Reflection.Rewrite(topLevel.Body).GetExpressionChain().ToList();

        using (Assert.Multiple())
        {
            await Assert.That(nestedChain).Count().IsEqualTo(NestedChainLength);
            await Assert.That(topLevelChain).Count().IsEqualTo(TopLevelChainLength);
        }
    }

    /// <summary>A root object exposing both a nested indexer and a top-level indexer.</summary>
    private sealed class IndexerRoot
    {
        /// <summary>Gets a nested object that itself exposes an indexer.</summary>
        public IndexerLeaf Leaf { get; } = new();

        /// <summary>Gets the value at the specified index.</summary>
        /// <param name="index">The index.</param>
        /// <returns>An empty string.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "SST2324:A member is declared more accessible than its containing type",
            Justification = "Referenced by an expression tree in the enclosing test type, which requires public access; the private test double is intentional.")]
        public string this[int index] => string.Empty;
    }

    /// <summary>A leaf object exposing an indexer reached through a nested member access.</summary>
    private sealed class IndexerLeaf
    {
        /// <summary>Gets the value at the specified index.</summary>
        /// <param name="index">The index.</param>
        /// <returns>An empty string.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "SST2324:A member is declared more accessible than its containing type",
            Justification = "Referenced by an expression tree in the enclosing test type, which requires public access; the private test double is intentional.")]
        public string this[int index] => string.Empty;
    }
}
