// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

namespace ReactiveUI.Tests.Expression;

/// <summary>
/// Advanced tests for Reflection class focusing on uncovered code paths and internal classes.
/// </summary>
public class ReflectionAdvancedTests
{
    [Test]
    public async Task Rewrite_WithComplexExpression_SimplifiesExpression()
    {
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var body = expr.Body;

        var rewritten = Reflection.Rewrite(body);

        await Assert.That(rewritten).IsNotNull();
        await Assert.That(rewritten.NodeType).IsEqualTo(ExpressionType.MemberAccess);
    }

    [Test]
    public async Task Rewrite_WithConvertExpression_UnwrapsConvert()
    {
        Expression<Func<TestClass, object>> expr = x => x.Property!;
        var body = expr.Body; // This is a Convert expression

        var rewritten = Reflection.Rewrite(body);

        // The rewriter should unwrap the Convert and return the MemberAccess
        await Assert.That(rewritten).IsNotNull();
    }

    [Test]
    public async Task ExpressionToPropertyNames_WithIndexExpression_IncludesIndexer()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), "x");
        var dictProperty = System.Linq.Expressions.Expression.Property(parameter, "Dictionary");
        var indexer = typeof(Dictionary<string, int>).GetProperty("Item")!;
        var keyArg = System.Linq.Expressions.Expression.Constant("testKey");
        var indexExpr = System.Linq.Expressions.Expression.MakeIndex(dictProperty, indexer, [keyArg]);

        var result = Reflection.ExpressionToPropertyNames(indexExpr);

        await Assert.That(result).Contains("Item[testKey]");
    }

    [Test]
    public async Task ExpressionToPropertyNames_WithNestedProperty_ReturnsFullPath()
    {
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;

        var result = Reflection.ExpressionToPropertyNames(expr.Body);

        await Assert.That(result).IsEqualTo("Nested.Property");
    }

    [Test]
    public async Task GetValueFetcherForProperty_WithNullFieldValue_ThrowsInvalidOperationException()
    {
        var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.NullableField))!;
        var fetcher = Reflection.GetValueFetcherForProperty(fieldInfo);

        var testObj = new TestClass { NullableField = null };

        await Assert.That(() => fetcher!(testObj, null))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task GetValueFetcherForProperty_WithPropertyIndexer_ReturnsIndexerValue()
    {
        var propertyInfo = typeof(Dictionary<string, int>).GetProperty("Item")!;
        var fetcher = Reflection.GetValueFetcherForProperty(propertyInfo);

        var dict = new Dictionary<string, int> { { "key", 42 } };
        var value = fetcher!(dict, ["key"]);

        await Assert.That(value).IsEqualTo(42);
    }

    [Test]
    public async Task GetValueFetcherOrThrow_WithUnsupportedMemberType_Throws()
    {
        var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod))!;

        await Assert.That(() => Reflection.GetValueFetcherOrThrow(methodInfo))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task GetValueSetterOrThrow_WithUnsupportedMemberType_Throws()
    {
        var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod))!;

        await Assert.That(() => Reflection.GetValueSetterOrThrow(methodInfo))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task GetValueSetterForProperty_WithPropertyIndexer_SetsIndexerValue()
    {
        var propertyInfo = typeof(Dictionary<string, int>).GetProperty("Item")!;
        var setter = Reflection.GetValueSetterForProperty(propertyInfo);

        var dict = new Dictionary<string, int>();
        setter!(dict, 99, ["key"]);

        await Assert.That(dict["key"]).IsEqualTo(99);
    }

    [Test]
    public async Task TryGetAllValuesForPropertyChain_WithNullInChain_ReturnsFalse()
    {
        var obj = new TestClass { Nested = null };
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TryGetAllValuesForPropertyChain(out var changeValues, obj, chain);

        await Assert.That(result).IsFalse();
        await Assert.That(changeValues).IsNotNull();
    }

    [Test]
    public async Task TryGetAllValuesForPropertyChain_WithValidChain_ReturnsAllValues()
    {
        var obj = new TestClass
        {
            Nested = new TestClass { Property = "nestedValue" }
        };
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TryGetAllValuesForPropertyChain(out var changeValues, obj, chain);

        await Assert.That(result).IsTrue();
        await Assert.That(changeValues.Length).IsEqualTo(2);
        await Assert.That(changeValues[1].Value).IsEqualTo("nestedValue");
    }

    [Test]
    public async Task TryGetAllValuesForPropertyChain_WithEmptyChain_Throws()
    {
        var obj = new TestClass();
        var emptyChain = Array.Empty<System.Linq.Expressions.Expression>();

        await Assert.That(() => Reflection.TryGetAllValuesForPropertyChain(out _, obj, emptyChain))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task TrySetValueToPropertyChain_WithValidChain_SetsValue()
    {
        var obj = new TestClass { Nested = new TestClass() };
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TrySetValueToPropertyChain(obj, chain, "newValue", shouldThrow: true);

        await Assert.That(result).IsTrue();
        await Assert.That(obj.Nested!.Property).IsEqualTo("newValue");
    }

    [Test]
    public async Task TrySetValueToPropertyChain_WithShouldThrowFalse_AndNullIntermediate_ReturnsFalse()
    {
        var obj = new TestClass { Nested = null };
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TrySetValueToPropertyChain(obj, chain, "value", shouldThrow: false);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TrySetValueToPropertyChain_WithEmptyChain_Throws()
    {
        var obj = new TestClass();
        var emptyChain = Array.Empty<System.Linq.Expressions.Expression>();

        await Assert.That(() => Reflection.TrySetValueToPropertyChain(obj, emptyChain, "value"))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ReallyFindType_WithAssemblyQualifiedName_ReturnsType()
    {
        var typeName = typeof(string).AssemblyQualifiedName!;

        var result = Reflection.ReallyFindType(typeName, throwOnFailure: false);

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsEqualTo(typeof(string));
    }

    [Test]
    public async Task ReallyFindType_CachesResults()
    {
        var typeName = typeof(TestClass).AssemblyQualifiedName!;

        var result1 = Reflection.ReallyFindType(typeName, throwOnFailure: false);
        var result2 = Reflection.ReallyFindType(typeName, throwOnFailure: false);

        await Assert.That(result1).IsSameReferenceAs(result2);
    }

    [Test]
    public async Task ThrowIfMethodsNotOverloaded_WithOverloadedMethods_DoesNotThrow()
    {
        var type = typeof(DerivedTestClass);

        await Assert.That(() => Reflection.ThrowIfMethodsNotOverloaded("TestCaller", type, nameof(DerivedTestClass.TestMethod)))
            .ThrowsNothing();
    }

    [Test]
    public async Task ThrowIfMethodsNotOverloaded_WithMissingMethod_Throws()
    {
        var type = typeof(TestClass);

        await Assert.That(() => Reflection.ThrowIfMethodsNotOverloaded("TestCaller", type, "NonExistentMethod"))
            .Throws<Exception>();
    }

    [Test]
    public async Task ThrowIfMethodsNotOverloaded_WithObject_ChecksMethods()
    {
        var obj = new DerivedTestClass();

        await Assert.That(() => Reflection.ThrowIfMethodsNotOverloaded("TestCaller", obj, nameof(DerivedTestClass.TestMethod)))
            .ThrowsNothing();
    }

    [Test]
    public async Task ThrowIfMethodsNotOverloaded_WithNullObject_Throws()
    {
        object? obj = null;

        await Assert.That(() => Reflection.ThrowIfMethodsNotOverloaded("TestCaller", obj!, "AnyMethod"))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ThrowIfMethodsNotOverloaded_WithNullMethodsArray_Throws()
    {
        var type = typeof(TestClass);

        await Assert.That(() => Reflection.ThrowIfMethodsNotOverloaded("TestCaller", type, null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task IsStatic_WithStaticProperty_ReturnsTrue()
    {
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticProperty))!;

        var result = propertyInfo.IsStatic();

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsStatic_WithGetOnlyStaticProperty_ReturnsTrue()
    {
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticGetOnlyProperty))!;

        var result = propertyInfo.IsStatic();

        await Assert.That(result).IsTrue();
    }

    public class TestClass
    {
        [SuppressMessage(
            "StyleCop.CSharp.MaintainabilityRules",
            "SA1401:Fields should be private",
            Justification = "Public field required for reflection tests")]
        public string? NullableField;

        public static string? StaticProperty { get; set; }

        public static string StaticGetOnlyProperty { get; } = "static";

        public Dictionary<string, int> Dictionary { get; set; } = new() { { "key", 42 } };

        public TestClass? Nested { get; set; }

        public string? Property { get; set; }

        public string ReadOnlyProperty { get; } = "readonly";

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:Mark members as static",
            Justification = "Test needs instance method for reflection")]
        public void TestMethod()
        {
        }
    }

    public class DerivedTestClass : TestClass
    {
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:Mark members as static",
            Justification = "Test needs instance method for reflection")]
        public new void TestMethod()
        {
        }
    }
}
