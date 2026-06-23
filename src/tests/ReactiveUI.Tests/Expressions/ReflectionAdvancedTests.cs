// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ReactiveUI.Tests.Expressions;

/// <summary>Advanced tests for Reflection class focusing on uncovered code paths and internal classes.</summary>
public class ReflectionAdvancedTests
{
    /// <summary>The caller name used when exercising caller-based reflection helpers.</summary>
    private const string TestCallerName = "TestCaller";

    /// <summary>The dictionary key used in indexer reflection tests.</summary>
    private const string KeyText = "key";

    /// <summary>The name of the default indexer property.</summary>
    private const string ItemPropertyName = "Item";

    /// <summary>The value stored against the dictionary key in reflection tests.</summary>
    private const int DictionaryValue = 42;

    /// <summary>Verifies that a complex nested expression is rewritten to a member access.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Rewrite_WithComplexExpression_SimplifiesExpression()
    {
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var body = expr.Body;

        var rewritten = Reflection.Rewrite(body);

        await Assert.That(rewritten).IsNotNull();
        await Assert.That(rewritten.NodeType).IsEqualTo(ExpressionType.MemberAccess);
    }

    /// <summary>Verifies that a Convert expression is unwrapped during rewriting.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Rewrite_WithConvertExpression_UnwrapsConvert()
    {
        Expression<Func<TestClass, object>> expr = x => x.Property!;
        var body = expr.Body; // This is a Convert expression

        var rewritten = Reflection.Rewrite(body);

        // The rewriter should unwrap the Convert and return the MemberAccess
        await Assert.That(rewritten).IsNotNull();
    }

    /// <summary>Verifies that converting an index expression to property names includes the indexer key.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExpressionToPropertyNames_WithIndexExpression_IncludesIndexer()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), "x");
        var dictProperty = System.Linq.Expressions.Expression.Property(parameter, "Dictionary");
        var indexer = typeof(Dictionary<string, int>).GetProperty(ItemPropertyName)!;
        var keyArg = System.Linq.Expressions.Expression.Constant("testKey");
        var indexExpr = System.Linq.Expressions.Expression.MakeIndex(dictProperty, indexer, [keyArg]);

        var result = Reflection.ExpressionToPropertyNames(indexExpr);

        await Assert.That(result).Contains("Item[testKey]");
    }

    /// <summary>Verifies that converting a nested property expression returns the full dotted path.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExpressionToPropertyNames_WithNestedProperty_ReturnsFullPath()
    {
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;

        var result = Reflection.ExpressionToPropertyNames(expr.Body);

        await Assert.That(result).IsEqualTo("Nested.Property");
    }

    /// <summary>Verifies that a value fetcher throws when the field value is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueFetcherForProperty_WithNullFieldValue_ThrowsInvalidOperationException()
    {
        var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.NullableField))!;
        var fetcher = Reflection.GetValueFetcherForProperty(fieldInfo);

        var testObj = new TestClass { NullableField = null };

        await Assert.That(() => fetcher!(testObj, null))
            .Throws<InvalidOperationException>();
    }

    /// <summary>Verifies that a value fetcher reads a value through a property indexer.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueFetcherForProperty_WithPropertyIndexer_ReturnsIndexerValue()
    {
        var propertyInfo = typeof(Dictionary<string, int>).GetProperty(ItemPropertyName)!;
        var fetcher = Reflection.GetValueFetcherForProperty(propertyInfo);

        const int ExpectedValue = 42;
        var dict = new Dictionary<string, int> { { KeyText, ExpectedValue } };
        var value = fetcher!(dict, [KeyText]);

        await Assert.That(value).IsEqualTo(ExpectedValue);
    }

    /// <summary>Verifies that requesting a value fetcher for an unsupported member type throws.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueFetcherOrThrow_WithUnsupportedMemberType_Throws()
    {
        var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod))!;

        await Assert.That(() => Reflection.GetValueFetcherOrThrow(methodInfo))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that requesting a value setter for an unsupported member type throws.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueSetterOrThrow_WithUnsupportedMemberType_Throws()
    {
        var methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod))!;

        await Assert.That(() => Reflection.GetValueSetterOrThrow(methodInfo))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that a value setter writes a value through a property indexer.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueSetterForProperty_WithPropertyIndexer_SetsIndexerValue()
    {
        var propertyInfo = typeof(Dictionary<string, int>).GetProperty(ItemPropertyName)!;
        var setter = Reflection.GetValueSetterForProperty(propertyInfo);

        const int ExpectedValue = 99;
        var dict = new Dictionary<string, int>();
        setter!(dict, ExpectedValue, [KeyText]);

        await Assert.That(dict[KeyText]).IsEqualTo(ExpectedValue);
    }

    /// <summary>Verifies that getting all values returns false when a chain link is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>Verifies that getting all values returns every value for a valid chain.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryGetAllValuesForPropertyChain_WithValidChain_ReturnsAllValues()
    {
        var obj = new TestClass { Nested = new() { Property = "nestedValue" } };
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TryGetAllValuesForPropertyChain(out var changeValues, obj, chain);

        const int ExpectedValueCount = 2;
        await Assert.That(result).IsTrue();
        await Assert.That(changeValues.Length).IsEqualTo(ExpectedValueCount);
        await Assert.That(changeValues[1].Value).IsEqualTo("nestedValue");
    }

    /// <summary>Verifies that getting all values for an empty chain throws.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryGetAllValuesForPropertyChain_WithEmptyChain_Throws()
    {
        var obj = new TestClass();
        var emptyChain = Array.Empty<Expression>();

        await Assert.That(() => Reflection.TryGetAllValuesForPropertyChain(out _, obj, emptyChain))
            .Throws<InvalidOperationException>();
    }

    /// <summary>Verifies that setting a value through a valid chain succeeds.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TrySetValueToPropertyChain_WithValidChain_SetsValue()
    {
        var obj = new TestClass { Nested = new() };
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TrySetValueToPropertyChain(obj, chain, "newValue", true);

        await Assert.That(result).IsTrue();
        await Assert.That(obj.Nested!.Property).IsEqualTo("newValue");
    }

    /// <summary>Verifies that setting a value returns false on a null intermediate when not configured to throw.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TrySetValueToPropertyChain_WithShouldThrowFalse_AndNullIntermediate_ReturnsFalse()
    {
        var obj = new TestClass { Nested = null };
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TrySetValueToPropertyChain(obj, chain, "value", false);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that setting a value with an empty chain throws.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TrySetValueToPropertyChain_WithEmptyChain_Throws()
    {
        var obj = new TestClass();
        var emptyChain = Array.Empty<Expression>();

        await Assert.That(() => Reflection.TrySetValueToPropertyChain(obj, emptyChain, "value"))
            .Throws<InvalidOperationException>();
    }

    /// <summary>Verifies that resolving a type by its assembly-qualified name returns the type.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReallyFindType_WithAssemblyQualifiedName_ReturnsType()
    {
        var typeName = typeof(string).AssemblyQualifiedName!;

        var result = Reflection.ReallyFindType(typeName, false);

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsEqualTo(typeof(string));
    }

    /// <summary>Verifies that type resolution caches and returns the same reference on repeated calls.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReallyFindType_CachesResults()
    {
        var typeName = typeof(TestClass).AssemblyQualifiedName!;

        var result1 = Reflection.ReallyFindType(typeName, false);
        var result2 = Reflection.ReallyFindType(typeName, false);

        await Assert.That(result1).IsSameReferenceAs(result2);
    }

    /// <summary>Verifies that the overload check passes when the methods are overloaded.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ThrowIfMethodsNotOverloaded_WithOverloadedMethods_DoesNotThrow()
    {
        var type = typeof(DerivedTestClass);

        await Assert.That(() =>
            Reflection.ThrowIfMethodsNotOverloaded(TestCallerName, type, nameof(DerivedTestClass.TestMethod))).ThrowsNothing();
    }

    /// <summary>Verifies that the overload check throws when the method is missing.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ThrowIfMethodsNotOverloaded_WithMissingMethod_Throws()
    {
        var type = typeof(TestClass);

        await Assert.That(() => Reflection.ThrowIfMethodsNotOverloaded(TestCallerName, type, "NonExistentMethod"))
            .Throws<Exception>();
    }

    /// <summary>Verifies that the overload check accepts an object instance and validates its methods.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ThrowIfMethodsNotOverloaded_WithObject_ChecksMethods()
    {
        var obj = new DerivedTestClass();

        await Assert.That(() =>
            Reflection.ThrowIfMethodsNotOverloaded(TestCallerName, obj, nameof(DerivedTestClass.TestMethod))).ThrowsNothing();
    }

    /// <summary>Verifies that the overload check throws when the object is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ThrowIfMethodsNotOverloaded_WithNullObject_Throws()
    {
        object? obj = null;

        await Assert.That(() => Reflection.ThrowIfMethodsNotOverloaded(TestCallerName, obj!, "AnyMethod"))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that the overload check throws when the methods array is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ThrowIfMethodsNotOverloaded_WithNullMethodsArray_Throws()
    {
        var type = typeof(TestClass);

        await Assert.That(() => Reflection.ThrowIfMethodsNotOverloaded(TestCallerName, type, null!))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies that IsStatic returns true for a static property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsStatic_WithStaticProperty_ReturnsTrue()
    {
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticProperty))!;

        var result = propertyInfo.IsStatic();

        await Assert.That(result).IsTrue();
    }

    /// <summary>Verifies that IsStatic returns true for a get-only static property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsStatic_WithGetOnlyStaticProperty_ReturnsTrue()
    {
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticGetOnlyProperty))!;

        var result = propertyInfo.IsStatic();

        await Assert.That(result).IsTrue();
    }

    /// <summary>A sample class used as the target of advanced reflection tests.</summary>
    public class TestClass
    {
        /// <summary>A nullable field used for reflection-based fetcher tests.</summary>
        [SuppressMessage("Maintainability", "SST1401:Field should be private", Justification = "Public field required for reflection tests")]
        public string? NullableField;

        /// <summary>Gets or sets a static property.</summary>
        public static string? StaticProperty { get; set; }

        /// <summary>Gets a get-only static property.</summary>
        public static string StaticGetOnlyProperty { get; } = "static";

        /// <summary>Gets a sample dictionary used for indexer tests.</summary>
        public Dictionary<string, int> Dictionary { get; } = new() { { KeyText, DictionaryValue } };

        /// <summary>Gets or sets a nested instance.</summary>
        public TestClass? Nested { get; set; }

        /// <summary>Gets or sets a sample property.</summary>
        public string? Property { get; set; }

        /// <summary>Gets a read-only property value.</summary>
        public string ReadOnlyProperty { get; } = "readonly";

        /// <summary>An instance method used to exercise unsupported member type handling.</summary>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:Mark members as static",
            Justification = "Test needs instance method for reflection")]
        public void TestMethod()
        {
            // No-op: only its existence as a method member is needed for reflection tests.
        }
    }

    /// <summary>A derived class that hides <see cref="TestClass.TestMethod"/> for overload-check tests.</summary>
    public class DerivedTestClass : TestClass
    {
        /// <summary>An instance method that hides the base method for overload-check tests.</summary>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:Mark members as static",
            Justification = "Test needs instance method for reflection")]
        public new void TestMethod()
        {
            // No-op: only its existence as a hiding method member is needed for overload-check tests.
        }
    }
}
