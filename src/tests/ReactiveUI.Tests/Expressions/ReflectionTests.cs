// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace ReactiveUI.Tests.Expressions;

/// <summary>Tests for the Reflection helper class.</summary>
public class ReflectionTests
{
    /// <summary>The dictionary key used in indexer reflection tests.</summary>
    private const string KeyText = "key";

    /// <summary>The name of the expression parameter used in reflection tests.</summary>
    private const string ParameterName = "x";

    /// <summary>The name of the default indexer property.</summary>
    private const string ItemPropertyName = "Item";

    /// <summary>The name of the dictionary property used in indexer tests.</summary>
    private const string DictionaryPropertyName = "Dictionary";

    /// <summary>The value written when exercising value setters.</summary>
    private const string SetValueText = "setValue";

    /// <summary>The value stored against the dictionary key in reflection tests.</summary>
    private const int DictionaryValue = 42;

    /// <summary>The second element value used in array reflection tests.</summary>
    private const int SecondElement = 2;

    /// <summary>The third element value used in array reflection tests.</summary>
    private const int ThirdElement = 3;

    /// <summary>Verifies that a nested property expression is converted to chained property names.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExpressionToPropertyNames_WithNestedProperty_ReturnsChainedNames()
    {
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;

        var result = Reflection.ExpressionToPropertyNames(expr.Body);

        await Assert.That(result).IsEqualTo("Nested.Property");
    }

    /// <summary>Verifies that converting a null expression to property names throws.</summary>
    [Test]
    public void ExpressionToPropertyNames_WithNull_Throws() =>
        Assert.Throws<ArgumentNullException>(() => Reflection.ExpressionToPropertyNames(null));

    /// <summary>Verifies that a simple property expression is converted to its property name.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExpressionToPropertyNames_WithSimpleProperty_ReturnsPropertyName()
    {
        Expression<Func<TestClass, string?>> expr = x => x.Property;

        var result = Reflection.ExpressionToPropertyNames(expr.Body);

        await Assert.That(result).IsEqualTo("Property");
    }

    /// <summary>Verifies that the arguments array of an index expression is returned.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetArgumentsArray_WithIndexExpression_ReturnsArguments()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), ParameterName);
        var dictProperty = System.Linq.Expressions.Expression.Property(parameter, DictionaryPropertyName);
        var indexer = typeof(Dictionary<string, int>).GetProperty(ItemPropertyName)!;
        var keyArg = System.Linq.Expressions.Expression.Constant(KeyText);
        var indexExpr = System.Linq.Expressions.Expression.MakeIndex(dictProperty, indexer, [keyArg]);

        var args = indexExpr.GetArgumentsArray();

        await Assert.That(args).IsNotNull();
        await Assert.That(args!.Length).IsEqualTo(1);
        await Assert.That(args[0]).IsEqualTo(KeyText);
    }

    /// <summary>Verifies that the arguments array of a multi-dimensional index expression is returned.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetArgumentsArray_WithMultiDimensionalIndex_ReturnsAllArguments()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), ParameterName);
        var dictProperty = System.Linq.Expressions.Expression.Property(parameter, DictionaryPropertyName);
        var key = System.Linq.Expressions.Expression.Constant(KeyText);
        var indexExpr = System.Linq.Expressions.Expression.Property(dictProperty, ItemPropertyName, key);

        var args = indexExpr.GetArgumentsArray();

        await Assert.That(args).IsNotNull();
        await Assert.That(args!.Length).IsEqualTo(1);
        await Assert.That(args[0]).IsEqualTo(KeyText);
    }

    /// <summary>Verifies that the arguments array is null for a non-index expression.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetArgumentsArray_WithNonIndexExpression_ReturnsNull()
    {
        Expression<Func<TestClass, string?>> expr = x => x.Property;

        var args = expr.Body.GetArgumentsArray();

        await Assert.That(args is null).IsTrue();
    }

    /// <summary>Verifies that resolving the event args type for an invalid event throws.</summary>
    [Test]
    public void GetEventArgsTypeForEvent_WithInvalidEvent_Throws() => Assert.Throws<Exception>(() =>
        Reflection.GetEventArgsTypeForEvent(typeof(TestClass), "NonExistentEvent"));

    /// <summary>Verifies that resolving the event args type with a null type throws.</summary>
    [Test]
    public void GetEventArgsTypeForEvent_WithNullType_Throws() =>
        Assert.Throws<ArgumentNullException>(() => Reflection.GetEventArgsTypeForEvent(null!, "TestEvent"));

    /// <summary>Verifies that resolving the event args type for a valid event returns the expected type.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetEventArgsTypeForEvent_WithValidEvent_ReturnsEventArgsType()
    {
        var eventArgsType = Reflection.GetEventArgsTypeForEvent(typeof(TestClass), nameof(TestClass.TestEvent));

        await Assert.That(eventArgsType).IsEqualTo(typeof(EventArgs));
    }

    /// <summary>Verifies that building an expression chain handles an indexer link.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetExpressionChain_WithIndexExpression_HandlesIndexer()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), ParameterName);
        var dictProperty = System.Linq.Expressions.Expression.Property(parameter, DictionaryPropertyName);
        var indexer = typeof(Dictionary<string, int>).GetProperty(ItemPropertyName)!;
        var keyArg = System.Linq.Expressions.Expression.Constant(KeyText);
        var indexExpr = System.Linq.Expressions.Expression.MakeIndex(dictProperty, indexer, [keyArg]);

        var chain = indexExpr.GetExpressionChain();

        const int ExpectedChainCount = 2;
        await Assert.That(chain).IsNotEmpty();
        var chainList = chain.ToList();
        await Assert.That(chainList.Count).IsEqualTo(ExpectedChainCount);
        await Assert.That(chainList[0].NodeType).IsEqualTo(ExpressionType.MemberAccess);
        await Assert.That(chainList[1].NodeType).IsEqualTo(ExpressionType.Index);
    }

    /// <summary>Verifies that building an expression chain handles a nested indexer chain.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetExpressionChain_WithNestedIndexExpression_HandlesChain()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), ParameterName);
        var nestedProperty = System.Linq.Expressions.Expression.Property(parameter, "Nested");
        var dictProperty = System.Linq.Expressions.Expression.Property(nestedProperty, DictionaryPropertyName);
        var indexer = typeof(Dictionary<string, int>).GetProperty(ItemPropertyName)!;
        var keyArg = System.Linq.Expressions.Expression.Constant(KeyText);
        var indexExpr = System.Linq.Expressions.Expression.MakeIndex(dictProperty, indexer, [keyArg]);

        var chain = indexExpr.GetExpressionChain();

        const int ExpectedChainCount = 3;
        await Assert.That(chain).IsNotEmpty();
        var chainList = chain.ToList();
        await Assert.That(chainList.Count).IsEqualTo(ExpectedChainCount);
    }

    /// <summary>Verifies that GetMemberInfo unwraps a ConvertChecked expression to its underlying member.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetMemberInfo_WithConvertCheckedExpression_ReturnsUnderlyingMember()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), ParameterName);
        var member = System.Linq.Expressions.Expression.Field(parameter, "PublicField");
        var convertChecked = System.Linq.Expressions.Expression.ConvertChecked(member, typeof(long));

        var memberInfo = convertChecked.GetMemberInfo();

        await Assert.That(memberInfo).IsNotNull();
        await Assert.That(memberInfo!.Name).IsEqualTo("PublicField");
    }

    /// <summary>Verifies that GetMemberInfo unwraps a Convert expression to its underlying member.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetMemberInfo_WithConvertExpression_ReturnsUnderlyingMember()
    {
        Expression<Func<TestClass, object>> expr = x => x.Property!;

        var memberInfo = expr.Body.GetMemberInfo();

        await Assert.That(memberInfo).IsNotNull();
        await Assert.That(memberInfo!.Name).IsEqualTo("Property");
    }

    /// <summary>Verifies that GetMemberInfo returns the indexer property for an index expression.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetMemberInfo_WithIndexExpression_ReturnsIndexer()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), ParameterName);
        var dictProperty = System.Linq.Expressions.Expression.Property(parameter, DictionaryPropertyName);
        var indexer = typeof(Dictionary<string, int>).GetProperty(ItemPropertyName)!;
        var keyArg = System.Linq.Expressions.Expression.Constant(KeyText);
        var indexExpr = System.Linq.Expressions.Expression.MakeIndex(dictProperty, indexer, [keyArg]);

        var memberInfo = indexExpr.GetMemberInfo();

        await Assert.That(memberInfo).IsNotNull();
        await Assert.That(memberInfo).IsTypeOf<PropertyInfo>();
    }

    /// <summary>Verifies that GetMemberInfo throws for an unsupported expression.</summary>
    [Test]
    public void GetMemberInfo_WithUnsupportedExpression_Throws()
    {
        const int ConstantValue = 42;
        var constant = System.Linq.Expressions.Expression.Constant(ConstantValue);

        _ = Assert.Throws<NotSupportedException>(() => constant.GetMemberInfo());
    }

    /// <summary>Verifies that GetParent returns the object expression of an index expression.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetParent_WithIndexExpression_ReturnsObject()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), ParameterName);
        var dictProperty = System.Linq.Expressions.Expression.Property(parameter, DictionaryPropertyName);
        var indexer = typeof(Dictionary<string, int>).GetProperty(ItemPropertyName)!;
        var keyArg = System.Linq.Expressions.Expression.Constant(KeyText);
        var indexExpr = System.Linq.Expressions.Expression.MakeIndex(dictProperty, indexer, [keyArg]);

        var parent = indexExpr.GetParent();

        await Assert.That(parent).IsNotNull();
        await Assert.That(parent!.NodeType).IsEqualTo(ExpressionType.MemberAccess);
    }

    /// <summary>Verifies that GetParent returns the parent expression of a member expression.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetParent_WithMemberExpression_ReturnsExpression()
    {
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var memberExpr = (MemberExpression)expr.Body;

        var parent = memberExpr.GetParent();

        await Assert.That(parent).IsNotNull();
        await Assert.That(parent!.NodeType).IsEqualTo(ExpressionType.MemberAccess);
    }

    /// <summary>Verifies that GetParent throws for an unsupported expression.</summary>
    [Test]
    public void GetParent_WithUnsupportedExpression_Throws()
    {
        const int ConstantValue = 42;
        var constant = System.Linq.Expressions.Expression.Constant(ConstantValue);

        _ = Assert.Throws<NotSupportedException>(() => constant.GetParent());
    }

    /// <summary>Verifies that a value fetcher reads a value from a field.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueFetcherForProperty_WithField_ReturnsFetcher()
    {
        var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.PublicField))!;

        var fetcher = Reflection.GetValueFetcherForProperty(fieldInfo);

        const int ExpectedFieldValue = 42;
        await Assert.That(fetcher).IsNotNull();
        var testObj = new TestClass { PublicField = ExpectedFieldValue };
        var value = fetcher!(testObj, null);
        await Assert.That(value).IsEqualTo(ExpectedFieldValue);
    }

    /// <summary>Verifies that requesting a value fetcher with a null member throws.</summary>
    [Test]
    public void GetValueFetcherForProperty_WithNull_Throws() =>
        Assert.Throws<ArgumentNullException>(() => Reflection.GetValueFetcherForProperty(null));

    /// <summary>Verifies that a value fetcher reads a value from a property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueFetcherForProperty_WithProperty_ReturnsFetcher()
    {
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.Property))!;

        var fetcher = Reflection.GetValueFetcherForProperty(propertyInfo);

        await Assert.That(fetcher).IsNotNull();
        var testObj = new TestClass { Property = "test" };
        var value = fetcher!(testObj, null);
        await Assert.That(value).IsEqualTo("test");
    }

    /// <summary>Verifies that requesting a value fetcher or throw with a null member throws.</summary>
    [Test]
    public void GetValueFetcherOrThrow_WithNull_Throws() =>
        Assert.Throws<ArgumentNullException>(() => Reflection.GetValueFetcherOrThrow(null));

    /// <summary>Verifies that GetValueFetcherOrThrow returns a fetcher for a property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueFetcherOrThrow_WithProperty_ReturnsFetcher()
    {
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.Property))!;

        var fetcher = Reflection.GetValueFetcherOrThrow(propertyInfo);

        await Assert.That(fetcher).IsNotNull();
    }

    /// <summary>Verifies that a value setter writes a value to a field.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueSetterForProperty_WithField_ReturnsSetter()
    {
        var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.PublicField))!;

        var setter = Reflection.GetValueSetterForProperty(fieldInfo);

        const int ExpectedFieldValue = 99;
        await Assert.That(setter).IsNotNull();
        var testObj = new TestClass();
        setter(testObj, ExpectedFieldValue, null);
        await Assert.That(testObj.PublicField).IsEqualTo(ExpectedFieldValue);
    }

    /// <summary>Verifies that requesting a value setter with a null member throws.</summary>
    [Test]
    public void GetValueSetterForProperty_WithNull_Throws() =>
        Assert.Throws<ArgumentNullException>(() => Reflection.GetValueSetterForProperty(null));

    /// <summary>Verifies that a value setter writes a value to a property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueSetterForProperty_WithProperty_ReturnsSetter()
    {
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.Property))!;

        var setter = Reflection.GetValueSetterForProperty(propertyInfo);

        await Assert.That(setter).IsNotNull();
        var testObj = new TestClass();
        setter(testObj, "newValue", null);
        await Assert.That(testObj.Property).IsEqualTo("newValue");
    }

    /// <summary>Verifies that requesting a value setter or throw with a null member throws.</summary>
    [Test]
    public void GetValueSetterOrThrow_WithNull_Throws() =>
        Assert.Throws<ArgumentNullException>(() => Reflection.GetValueSetterOrThrow(null));

    /// <summary>Verifies that GetValueSetterOrThrow returns a setter for a property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueSetterOrThrow_WithProperty_ReturnsSetter()
    {
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.Property))!;

        var setter = Reflection.GetValueSetterOrThrow(propertyInfo);

        await Assert.That(setter).IsNotNull();
    }

    /// <summary>Verifies that IsStatic returns false for an instance property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsStatic_WithInstanceProperty_ReturnsFalse()
    {
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.Property))!;

        var result = propertyInfo.IsStatic();

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that IsStatic throws when the property info is null.</summary>
    [Test]
    public void IsStatic_WithNull_Throws()
    {
        PropertyInfo? propertyInfo = null;
        _ = Assert.Throws<ArgumentNullException>(() => propertyInfo!.IsStatic());
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

    /// <summary>Verifies that resolving an invalid type name returns null when not configured to throw.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReallyFindType_WithInvalidTypeName_ReturnsNull()
    {
        var result = Reflection.ReallyFindType("Invalid.Type.Name", false);

        await Assert.That(result).IsNull();
    }

    /// <summary>Verifies that resolving an invalid type name throws when configured to throw.</summary>
    [Test]
    public void ReallyFindType_WithInvalidTypeNameAndThrow_Throws() =>
        Assert.Throws<TypeLoadException>(() => Reflection.ReallyFindType("Invalid.Type.Name", true));

    /// <summary>Verifies that resolving a valid type name returns the expected type.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReallyFindType_WithValidTypeName_ReturnsType()
    {
        var typeName = typeof(TestClass).AssemblyQualifiedName!;

        var result = Reflection.ReallyFindType(typeName, false);

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsEqualTo(typeof(TestClass));
    }

    /// <summary>Verifies that getting a value returns false when a chain link is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryGetValueForPropertyChain_WithNullInChain_ReturnsFalse()
    {
        var obj = new TestClass { Nested = null };
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TryGetValueForPropertyChain<string>(out var value, obj, chain);

        await Assert.That(result).IsFalse();
        await Assert.That(value).IsNull();
    }

    /// <summary>Verifies that getting a value through a valid chain succeeds.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryGetValueForPropertyChain_WithValidChain_GetsValue()
    {
        var obj = new TestClass { Nested = new() { Property = "nestedValue" } };
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TryGetValueForPropertyChain<string>(out var value, obj, chain);

        await Assert.That(result).IsTrue();
        await Assert.That(value).IsEqualTo("nestedValue");
    }

    /// <summary>Verifies that setting a value returns false when the target is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TrySetValueToPropertyChain_WithNullTarget_ReturnsFalse()
    {
        var obj = new TestClass { Nested = null };
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TrySetValueToPropertyChain(obj, chain, SetValueText, false);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that setting a value through a valid chain succeeds.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TrySetValueToPropertyChain_WithValidChain_SetsValue()
    {
        var obj = new TestClass { Nested = new() };
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TrySetValueToPropertyChain(obj, chain, SetValueText);

        await Assert.That(result).IsTrue();
        await Assert.That(obj.Nested!.Property).IsEqualTo(SetValueText);
    }

    /// <summary>An empty expression chain has no member to resolve and throws.</summary>
    [Test]
    public void TryGetValueForPropertyChain_EmptyChain_Throws() =>
        Assert.Throws<InvalidOperationException>(() =>
            Reflection.TryGetValueForPropertyChain<int>(out _, new object(), []));

    /// <summary>A null intermediate value in the chain yields a failed lookup.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task TryGetValueForPropertyChain_NullIntermediate_ReturnsFalse()
    {
        Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TryGetValueForPropertyChain<string>(out var value, null, chain);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsFalse();
            await Assert.That(value).IsNull();
        }
    }

    /// <summary>Setting a read-only member without throwing returns false.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task TrySetValueToPropertyChain_ReadOnlyMember_ReturnsFalse()
    {
        Expression<Func<TestClass, int[]>> expr = x => x.Array;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TrySetValueToPropertyChain<int[]>(new TestClass(), chain, [], shouldThrow: false);

        await Assert.That(result).IsFalse();
    }

    /// <summary>A sample class used as the target of reflection tests.</summary>
    public class TestClass
    {
        /// <summary>A field used for reflection-based fetcher and setter tests.</summary>
        [SuppressMessage("Maintainability", "SST1401:Field should be private", Justification = "Public field required for reflection tests")]
        public int PublicField;

        /// <summary>An event used for event reflection tests.</summary>
        public event EventHandler? TestEvent;

        /// <summary>Gets or sets a static property.</summary>
        public static string? StaticProperty { get; set; }

        /// <summary>Gets a sample array.</summary>
        public int[] Array { get; } = [1, SecondElement, ThirdElement];

        /// <summary>Gets a sample dictionary used for indexer tests.</summary>
        public Dictionary<string, int> Dictionary { get; } = new() { { KeyText, DictionaryValue } };

        /// <summary>Gets a sample list.</summary>
        public List<int> List { get; } = [1, SecondElement, ThirdElement];

        /// <summary>Gets or sets a nested instance.</summary>
        public TestClass? Nested { get; set; }

        /// <summary>Gets or sets a sample property.</summary>
        public string? Property { get; set; }

        /// <summary>Raises the <see cref="TestEvent"/> event.</summary>
        public void RaiseTestEvent() => TestEvent?.Invoke(this, EventArgs.Empty);
    }
}
