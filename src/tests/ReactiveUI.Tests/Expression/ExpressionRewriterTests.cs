// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Expression;

public class ExpressionRewriterTests
{
    [Test]
    public async Task Rewrite_WithParameterExpression_ReturnsParameterExpression()
    {
        System.Linq.Expressions.Expression<Func<TestClass, TestClass>> expr = x => x;

        var result = Reflection.Rewrite(expr.Body);

        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.Parameter);
    }

    [Test]
    public async Task Rewrite_WithMemberAccess_ReturnsMemberExpression()
    {
        System.Linq.Expressions.Expression<Func<TestClass, string?>> expr = x => x.Property;

        var result = Reflection.Rewrite(expr.Body);

        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.MemberAccess);
    }

    [Test]
    public async Task Rewrite_WithNestedMemberAccess_ReturnsMemberExpression()
    {
        System.Linq.Expressions.Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;

        var result = Reflection.Rewrite(expr.Body);

        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.MemberAccess);
    }

    [Test]
    public async Task Rewrite_WithConstant_ReturnsConstantExpression()
    {
        System.Linq.Expressions.Expression<Func<string>> expr = () => "test";

        var result = Reflection.Rewrite(expr.Body);

        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.Constant);
    }

    [Test]
    public async Task Rewrite_WithConvert_ReturnsUnderlyingExpression()
    {
        System.Linq.Expressions.Expression<Func<TestClass, object>> expr = x => x.Property!;

        var result = Reflection.Rewrite(expr.Body);

        // Convert should be unwrapped to the underlying MemberAccess
        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.MemberAccess);
    }

    [Test]
    public async Task Rewrite_WithArrayIndex_ReturnsIndexExpression()
    {
        System.Linq.Expressions.Expression<Func<TestClass, int>> expr = x => x.Array[0];

        var result = Reflection.Rewrite(expr.Body);

        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.Index);
    }

    [Test]
    public void Rewrite_WithArrayIndexNonConstant_Throws()
    {
        var index = 0;
        System.Linq.Expressions.Expression<Func<TestClass, int>> expr = x => x.Array[index];

        Assert.Throws<NotSupportedException>(() => Reflection.Rewrite(expr.Body));
    }

    [Test]
    public async Task Rewrite_WithListIndexer_ReturnsIndexExpression()
    {
        System.Linq.Expressions.Expression<Func<TestClass, int>> expr = x => x.List[0];

        var result = Reflection.Rewrite(expr.Body);

        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.Index);
    }

    [Test]
    public void Rewrite_WithListIndexerNonConstant_Throws()
    {
        var index = 0;
        System.Linq.Expressions.Expression<Func<TestClass, int>> expr = x => x.List[index];

        Assert.Throws<NotSupportedException>(() => Reflection.Rewrite(expr.Body));
    }

    [Test]
    public async Task Rewrite_WithArrayLength_ReturnsMemberAccess()
    {
        System.Linq.Expressions.Expression<Func<TestClass, int>> expr = x => x.Array.Length;

        var result = Reflection.Rewrite(expr.Body);

        // ArrayLength should be rewritten to MemberAccess of Length property
        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.MemberAccess);
        var memberExpr = (MemberExpression)result;
        await Assert.That(memberExpr.Member.Name).IsEqualTo("Length");
    }

    [Test]
    public async Task Rewrite_WithUnsupportedExpression_Throws()
    {
        System.Linq.Expressions.Expression<Func<int, int>> expr = x => x + 1;

        var ex = Assert.Throws<NotSupportedException>(() => Reflection.Rewrite(expr.Body));
        await Assert.That(ex!.Message).Contains("Unsupported expression");
        await Assert.That(ex.Message).Contains("Add");
    }

    [Test]
    public async Task Rewrite_WithUnsupportedBinaryExpression_ThrowsWithHelpfulMessage()
    {
        System.Linq.Expressions.Expression<Func<int, bool>> expr = x => x > 5;

        var ex = Assert.Throws<NotSupportedException>(() => Reflection.Rewrite(expr.Body));
        await Assert.That(ex!.Message).Contains("Did you meant to use expressions");
    }

    [Test]
    public void Rewrite_WithNullExpression_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Reflection.Rewrite(null));
    }

    [Test]
    public async Task Rewrite_WithIndexExpression_ValidatesConstantArguments()
    {
        System.Linq.Expressions.Expression<Func<TestClass, int>> expr = x => x.List[1];

        var result = Reflection.Rewrite(expr.Body);

        await Assert.That(result.NodeType).IsEqualTo(ExpressionType.Index);
    }

    [Test]
    public void Rewrite_WithMethodCallNonSpecialName_Throws()
    {
        System.Linq.Expressions.Expression<Func<TestClass, string?>> expr = x => x.GetValue();

        Assert.Throws<NotSupportedException>(() => Reflection.Rewrite(expr.Body));
    }

    [Test]
    public void Rewrite_WithUnaryExpressionNotArrayLengthOrConvert_Throws()
    {
        // Create a unary expression that is not ArrayLength or Convert (e.g., Not)
        // This should trigger the unsupported expression path
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(bool), "x");
        var notExpr = System.Linq.Expressions.Expression.Not(parameter);

        Assert.Throws<NotSupportedException>(() => Reflection.Rewrite(notExpr));
    }

    [Test]
    public void Rewrite_WithIndexExpressionNonConstantArguments_Throws()
    {
        // Create an IndexExpression with non-constant arguments
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), "x");
        var listProperty = System.Linq.Expressions.Expression.Property(parameter, "List");
        var indexer = typeof(List<int>).GetProperty("Item")!;
        var nonConstantArg = System.Linq.Expressions.Expression.Parameter(typeof(int), "index");
        var indexExpr = System.Linq.Expressions.Expression.MakeIndex(listProperty, indexer, new[] { nonConstantArg });

        Assert.Throws<NotSupportedException>(() => Reflection.Rewrite(indexExpr));
    }

    private class TestClass
    {
        public string? Property { get; set; }

        public TestClass? Nested { get; set; }

        public int[] Array { get; set; } = [1, 2, 3];

        public List<int> List { get; set; } = [4, 5, 6];

        public string? GetValue() => Property;
    }
}
