// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace ReactiveUI.Tests.Expressions;

/// <summary>Tests for the Reflection.Rewrite expression rewriting logic.</summary>
public class ExpressionRewriterTests
{
    /// <summary>Verifies that an array index expression is rewritten to an Index node.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Rewrite_WithArrayIndex_ReturnsIndexExpression()
    {
        Expression<Func<TestClass, int>> expr = x => x.Array[0];

        var result = Reflection.Rewrite(expr.Body);

        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.Index);
    }

    /// <summary>Verifies that a non-constant array index throws a not supported exception.</summary>
    [Test]
    public void Rewrite_WithArrayIndexNonConstant_Throws()
    {
        // x.Index is a non-constant member access (not foldable to a constant index), so the rewrite is unsupported.
        Expression<Func<TestClass, int>> expr = x => x.Array[x.Index];

        _ = Assert.Throws<NotSupportedException>(() => Reflection.Rewrite(expr.Body));
    }

    /// <summary>Verifies that an array length expression is rewritten to a Length member access.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Rewrite_WithArrayLength_ReturnsMemberAccess()
    {
        Expression<Func<TestClass, int>> expr = x => x.Array.Length;

        var result = Reflection.Rewrite(expr.Body);

        // ArrayLength should be rewritten to MemberAccess of Length property
        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.MemberAccess);
        var memberExpr = (MemberExpression)result;
        await Assert.That(memberExpr.Member.Name).IsEqualTo("Length");
    }

    /// <summary>Verifies that a constant expression is preserved as a Constant node.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Rewrite_WithConstant_ReturnsConstantExpression()
    {
        Expression<Func<string>> expr = () => "test";

        var result = Reflection.Rewrite(expr.Body);

        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.Constant);
    }

    /// <summary>Verifies that a Convert expression is unwrapped to its underlying member access.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Rewrite_WithConvert_ReturnsUnderlyingExpression()
    {
        Expression<Func<TestClass, object>> expr = x => x.Property!;

        var result = Reflection.Rewrite(expr.Body);

        // Convert should be unwrapped to the underlying MemberAccess
        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.MemberAccess);
    }

    /// <summary>Verifies that an index expression with constant arguments is rewritten to an Index node.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Rewrite_WithIndexExpression_ValidatesConstantArguments()
    {
        Expression<Func<TestClass, int>> expr = x => x.List[1];

        var result = Reflection.Rewrite(expr.Body);

        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.Index);
    }

    /// <summary>Verifies that an index expression with non-constant arguments throws a not supported exception.</summary>
    [Test]
    public void Rewrite_WithIndexExpressionNonConstantArguments_Throws()
    {
        // Create an IndexExpression with non-constant arguments
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), "x");
        var listProperty = System.Linq.Expressions.Expression.Property(parameter, "List");
        var indexer = typeof(List<int>).GetProperty("Item")!;
        var nonConstantArg = System.Linq.Expressions.Expression.Parameter(typeof(int), "index");
        var indexExpr = System.Linq.Expressions.Expression.MakeIndex(listProperty, indexer, [nonConstantArg]);

        _ = Assert.Throws<NotSupportedException>(() => Reflection.Rewrite(indexExpr));
    }

    /// <summary>Verifies that a list indexer expression is rewritten to an Index node.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Rewrite_WithListIndexer_ReturnsIndexExpression()
    {
        Expression<Func<TestClass, int>> expr = x => x.List[0];

        var result = Reflection.Rewrite(expr.Body);

        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.Index);
    }

    /// <summary>Verifies that a non-constant list indexer throws a not supported exception.</summary>
    [Test]
    public void Rewrite_WithListIndexerNonConstant_Throws()
    {
        // x.Index is a non-constant member access (not foldable to a constant index), so the rewrite is unsupported.
        Expression<Func<TestClass, int>> expr = x => x.List[x.Index];

        _ = Assert.Throws<NotSupportedException>(() => Reflection.Rewrite(expr.Body));
    }

    /// <summary>Verifies that a member access expression is preserved as a MemberAccess node.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Rewrite_WithMemberAccess_ReturnsMemberExpression()
    {
        Expression<Func<TestClass, string?>> expr = x => x.Property;

        var result = Reflection.Rewrite(expr.Body);

        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.MemberAccess);
    }

    /// <summary>Verifies that a method call without a special name throws a not supported exception.</summary>
    [Test]
    public void Rewrite_WithMethodCallNonSpecialName_Throws()
    {
        Expression<Func<TestClass, string?>> expr = x => x.GetValue();

        _ = Assert.Throws<NotSupportedException>(() => Reflection.Rewrite(expr.Body));
    }

    /// <summary>Verifies that a nested member access expression is preserved as a MemberAccess node.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Rewrite_WithNestedMemberAccess_ReturnsMemberExpression()
    {
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;

        var result = Reflection.Rewrite(expr.Body);

        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.MemberAccess);
    }

    /// <summary>Verifies that rewriting a null expression throws an argument null exception.</summary>
    [Test]
    public void Rewrite_WithNullExpression_Throws() =>
        Assert.Throws<ArgumentNullException>(() => Reflection.Rewrite(null));

    /// <summary>Verifies that a parameter expression is preserved as a Parameter node.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Rewrite_WithParameterExpression_ReturnsParameterExpression()
    {
        Expression<Func<TestClass, TestClass>> expr = x => x;

        var result = Reflection.Rewrite(expr.Body);

        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.Parameter);
    }

    /// <summary>Verifies that a unary expression that is neither ArrayLength nor Convert throws.</summary>
    [Test]
    public void Rewrite_WithUnaryExpressionNotArrayLengthOrConvert_Throws()
    {
        // Create a unary expression that is not ArrayLength or Convert (e.g., Not)
        // This should trigger the unsupported expression path
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(bool), "x");
        var notExpr = System.Linq.Expressions.Expression.Not(parameter);

        _ = Assert.Throws<NotSupportedException>(() => Reflection.Rewrite(notExpr));
    }

    /// <summary>Verifies that an unsupported binary expression throws with a helpful message.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Rewrite_WithUnsupportedBinaryExpression_ThrowsWithHelpfulMessage()
    {
        const int Threshold = 5;
        Expression<Func<int, bool>> expr = x => x > Threshold;

        var ex = Assert.Throws<NotSupportedException>(() => Reflection.Rewrite(expr.Body));
        await Assert.That(ex.Message).Contains("Did you meant to use expressions");
    }

    /// <summary>Verifies that an unsupported expression throws with a descriptive message.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Rewrite_WithUnsupportedExpression_Throws()
    {
        Expression<Func<int, int>> expr = x => x + 1;

        var ex = Assert.Throws<NotSupportedException>(() => Reflection.Rewrite(expr.Body));
        await Assert.That(ex.Message).Contains("Unsupported expression");
        await Assert.That(ex.Message).Contains("Add");
    }

    /// <summary>A sample class used as the target of expression rewriting tests.</summary>
    private sealed class TestClass
    {
        /// <summary>The second element value of the sample array.</summary>
        private const int ArraySecondElement = 2;

        /// <summary>The third element value of the sample array.</summary>
        private const int ArrayThirdElement = 3;

        /// <summary>The first element value of the sample list.</summary>
        private const int ListFirstElement = 4;

        /// <summary>The second element value of the sample list.</summary>
        private const int ListSecondElement = 5;

        /// <summary>The third element value of the sample list.</summary>
        private const int ListThirdElement = 6;

        /// <summary>
        /// Gets or sets a non-constant index (a property access, not a compile-time constant) used to exercise
        /// unsupported non-constant indexer rewrites. The value is irrelevant; the rewrite fails before evaluation.
        /// </summary>
        public int Index { get; set; } = 1;

        /// <summary>Gets a sample array.</summary>
        public int[] Array { get; } = [1, ArraySecondElement, ArrayThirdElement];

        /// <summary>Gets a sample list.</summary>
        public List<int> List { get; } = [ListFirstElement, ListSecondElement, ListThirdElement];

        /// <summary>Gets or sets a nested instance.</summary>
        public TestClass? Nested { get; set; } = null!;

        /// <summary>Gets or sets a sample property.</summary>
        public string? Property { get; set; } = null!;

        /// <summary>Returns the value of <see cref="Property"/>.</summary>
        /// <returns>The property value.</returns>
        public string? GetValue() => Property;
    }
}
