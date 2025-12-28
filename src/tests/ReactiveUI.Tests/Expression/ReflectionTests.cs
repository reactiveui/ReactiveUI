// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

namespace ReactiveUI.Tests.Expression;

public class ReflectionTests
{
    [Test]
    public async Task ExpressionToPropertyNames_WithSimpleProperty_ReturnsPropertyName()
    {
        System.Linq.Expressions.Expression<Func<TestClass, string?>> expr = x => x.Property;

        var result = Reflection.ExpressionToPropertyNames(expr.Body);

        await Assert.That(result).IsEqualTo("Property");
    }

    [Test]
    public async Task ExpressionToPropertyNames_WithNestedProperty_ReturnsChainedNames()
    {
        System.Linq.Expressions.Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;

        var result = Reflection.ExpressionToPropertyNames(expr.Body);

        await Assert.That(result).IsEqualTo("Nested.Property");
    }

    [Test]
    public void ExpressionToPropertyNames_WithNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Reflection.ExpressionToPropertyNames(null));
    }

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

    [Test]
    public async Task GetValueFetcherForProperty_WithField_ReturnsFetcher()
    {
        var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.PublicField))!;

        var fetcher = Reflection.GetValueFetcherForProperty(fieldInfo);

        await Assert.That(fetcher).IsNotNull();
        var testObj = new TestClass { PublicField = 42 };
        var value = fetcher!(testObj, null);
        await Assert.That(value).IsEqualTo(42);
    }

    [Test]
    public void GetValueFetcherForProperty_WithNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Reflection.GetValueFetcherForProperty(null));
    }

    [Test]
    public async Task GetValueFetcherOrThrow_WithProperty_ReturnsFetcher()
    {
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.Property))!;

        var fetcher = Reflection.GetValueFetcherOrThrow(propertyInfo);

        await Assert.That(fetcher).IsNotNull();
    }

    [Test]
    public void GetValueFetcherOrThrow_WithNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Reflection.GetValueFetcherOrThrow(null));
    }

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

    [Test]
    public async Task GetValueSetterForProperty_WithField_ReturnsSetter()
    {
        var fieldInfo = typeof(TestClass).GetField(nameof(TestClass.PublicField))!;

        var setter = Reflection.GetValueSetterForProperty(fieldInfo);

        await Assert.That(setter).IsNotNull();
        var testObj = new TestClass();
        setter(testObj, 99, null);
        await Assert.That(testObj.PublicField).IsEqualTo(99);
    }

    [Test]
    public void GetValueSetterForProperty_WithNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Reflection.GetValueSetterForProperty(null));
    }

    [Test]
    public async Task GetValueSetterOrThrow_WithProperty_ReturnsSetter()
    {
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.Property))!;

        var setter = Reflection.GetValueSetterOrThrow(propertyInfo);

        await Assert.That(setter).IsNotNull();
    }

    [Test]
    public void GetValueSetterOrThrow_WithNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Reflection.GetValueSetterOrThrow(null));
    }

    [Test]
    public async Task TryGetValueForPropertyChain_WithValidChain_GetsValue()
    {
        var obj = new TestClass
        {
            Nested = new TestClass { Property = "nestedValue" }
        };
        System.Linq.Expressions.Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TryGetValueForPropertyChain<string>(out var value, obj, chain);

        await Assert.That(result).IsTrue();
        await Assert.That(value).IsEqualTo("nestedValue");
    }

    [Test]
    public async Task TryGetValueForPropertyChain_WithNullInChain_ReturnsFalse()
    {
        var obj = new TestClass { Nested = null };
        System.Linq.Expressions.Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TryGetValueForPropertyChain<string>(out var value, obj, chain);

        await Assert.That(result).IsFalse();
        await Assert.That(value).IsNull();
    }

    [Test]
    public async Task TrySetValueToPropertyChain_WithValidChain_SetsValue()
    {
        var obj = new TestClass
        {
            Nested = new TestClass()
        };
        System.Linq.Expressions.Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TrySetValueToPropertyChain(obj, chain, "setValue");

        await Assert.That(result).IsTrue();
        await Assert.That(obj.Nested!.Property).IsEqualTo("setValue");
    }

    [Test]
    public async Task TrySetValueToPropertyChain_WithNullTarget_ReturnsFalse()
    {
        var obj = new TestClass { Nested = null };
        System.Linq.Expressions.Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var chain = expr.Body.GetExpressionChain();

        var result = Reflection.TrySetValueToPropertyChain(obj, chain, "setValue", shouldThrow: false);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task ReallyFindType_WithValidTypeName_ReturnsType()
    {
        var typeName = typeof(TestClass).AssemblyQualifiedName!;

        var result = Reflection.ReallyFindType(typeName, false);

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsEqualTo(typeof(TestClass));
    }

    [Test]
    public async Task ReallyFindType_WithInvalidTypeName_ReturnsNull()
    {
        var result = Reflection.ReallyFindType("Invalid.Type.Name", false);

        await Assert.That(result).IsNull();
    }

    [Test]
    public void ReallyFindType_WithInvalidTypeNameAndThrow_Throws()
    {
        Assert.Throws<TypeLoadException>(() => Reflection.ReallyFindType("Invalid.Type.Name", true));
    }

    [Test]
    public async Task GetEventArgsTypeForEvent_WithValidEvent_ReturnsEventArgsType()
    {
        var eventArgsType = Reflection.GetEventArgsTypeForEvent(typeof(TestClass), nameof(TestClass.TestEvent));

        await Assert.That(eventArgsType).IsEqualTo(typeof(EventArgs));
    }

    [Test]
    public void GetEventArgsTypeForEvent_WithInvalidEvent_Throws()
    {
        Assert.Throws<Exception>(() => Reflection.GetEventArgsTypeForEvent(typeof(TestClass), "NonExistentEvent"));
    }

    [Test]
    public void GetEventArgsTypeForEvent_WithNullType_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Reflection.GetEventArgsTypeForEvent(null!, "TestEvent"));
    }

    [Test]
    public async Task IsStatic_WithStaticProperty_ReturnsTrue()
    {
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.StaticProperty))!;

        var result = propertyInfo.IsStatic();

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsStatic_WithInstanceProperty_ReturnsFalse()
    {
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.Property))!;

        var result = propertyInfo.IsStatic();

        await Assert.That(result).IsFalse();
    }

    [Test]
    public void IsStatic_WithNull_Throws()
    {
        PropertyInfo? propertyInfo = null;
        Assert.Throws<ArgumentNullException>(() => propertyInfo!.IsStatic());
    }

    [Test]
    public async Task GetExpressionChain_WithIndexExpression_HandlesIndexer()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), "x");
        var dictProperty = System.Linq.Expressions.Expression.Property(parameter, "Dictionary");
        var indexer = typeof(Dictionary<string, int>).GetProperty("Item")!;
        var keyArg = System.Linq.Expressions.Expression.Constant("key");
        var indexExpr = System.Linq.Expressions.Expression.MakeIndex(dictProperty, indexer, new[] { keyArg });

        var chain = indexExpr.GetExpressionChain();

        await Assert.That(chain).IsNotEmpty();
        var chainList = chain.ToList();
        await Assert.That(chainList.Count).IsEqualTo(2);
        await Assert.That(chainList[0].NodeType).IsEqualTo(System.Linq.Expressions.ExpressionType.MemberAccess);
        await Assert.That(chainList[1].NodeType).IsEqualTo(System.Linq.Expressions.ExpressionType.Index);
    }

    [Test]
    public async Task GetExpressionChain_WithNestedIndexExpression_HandlesChain()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), "x");
        var nestedProperty = System.Linq.Expressions.Expression.Property(parameter, "Nested");
        var dictProperty = System.Linq.Expressions.Expression.Property(nestedProperty, "Dictionary");
        var indexer = typeof(Dictionary<string, int>).GetProperty("Item")!;
        var keyArg = System.Linq.Expressions.Expression.Constant("key");
        var indexExpr = System.Linq.Expressions.Expression.MakeIndex(dictProperty, indexer, new[] { keyArg });

        var chain = indexExpr.GetExpressionChain();

        await Assert.That(chain).IsNotEmpty();
        var chainList = chain.ToList();
        await Assert.That(chainList.Count).IsEqualTo(3);
    }

    [Test]
    public async Task GetMemberInfo_WithIndexExpression_ReturnsIndexer()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), "x");
        var dictProperty = System.Linq.Expressions.Expression.Property(parameter, "Dictionary");
        var indexer = typeof(Dictionary<string, int>).GetProperty("Item")!;
        var keyArg = System.Linq.Expressions.Expression.Constant("key");
        var indexExpr = System.Linq.Expressions.Expression.MakeIndex(dictProperty, indexer, new[] { keyArg });

        var memberInfo = indexExpr.GetMemberInfo();

        await Assert.That(memberInfo).IsNotNull();
        await Assert.That(memberInfo).IsTypeOf<PropertyInfo>();
    }

    [Test]
    public async Task GetMemberInfo_WithConvertExpression_ReturnsUnderlyingMember()
    {
        System.Linq.Expressions.Expression<Func<TestClass, object>> expr = x => (object)x.Property!;

        var memberInfo = expr.Body.GetMemberInfo();

        await Assert.That(memberInfo).IsNotNull();
        await Assert.That(memberInfo!.Name).IsEqualTo("Property");
    }

    [Test]
    public async Task GetMemberInfo_WithConvertCheckedExpression_ReturnsUnderlyingMember()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), "x");
        var member = System.Linq.Expressions.Expression.Field(parameter, "PublicField");
        var convertChecked = System.Linq.Expressions.Expression.ConvertChecked(member, typeof(long));

        var memberInfo = convertChecked.GetMemberInfo();

        await Assert.That(memberInfo).IsNotNull();
        await Assert.That(memberInfo!.Name).IsEqualTo("PublicField");
    }

    [Test]
    public void GetMemberInfo_WithUnsupportedExpression_Throws()
    {
        var constant = System.Linq.Expressions.Expression.Constant(42);

        Assert.Throws<NotSupportedException>(() => constant.GetMemberInfo());
    }

    [Test]
    public async Task GetParent_WithIndexExpression_ReturnsObject()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), "x");
        var dictProperty = System.Linq.Expressions.Expression.Property(parameter, "Dictionary");
        var indexer = typeof(Dictionary<string, int>).GetProperty("Item")!;
        var keyArg = System.Linq.Expressions.Expression.Constant("key");
        var indexExpr = System.Linq.Expressions.Expression.MakeIndex(dictProperty, indexer, new[] { keyArg });

        var parent = indexExpr.GetParent();

        await Assert.That(parent).IsNotNull();
        await Assert.That(parent!.NodeType).IsEqualTo(System.Linq.Expressions.ExpressionType.MemberAccess);
    }

    [Test]
    public async Task GetParent_WithMemberExpression_ReturnsExpression()
    {
        System.Linq.Expressions.Expression<Func<TestClass, string?>> expr = x => x.Nested!.Property;
        var memberExpr = (System.Linq.Expressions.MemberExpression)expr.Body;

        var parent = memberExpr.GetParent();

        await Assert.That(parent).IsNotNull();
        await Assert.That(parent!.NodeType).IsEqualTo(System.Linq.Expressions.ExpressionType.MemberAccess);
    }

    [Test]
    public void GetParent_WithUnsupportedExpression_Throws()
    {
        var constant = System.Linq.Expressions.Expression.Constant(42);

        Assert.Throws<NotSupportedException>(() => constant.GetParent());
    }

    [Test]
    public async Task GetArgumentsArray_WithIndexExpression_ReturnsArguments()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), "x");
        var dictProperty = System.Linq.Expressions.Expression.Property(parameter, "Dictionary");
        var indexer = typeof(Dictionary<string, int>).GetProperty("Item")!;
        var keyArg = System.Linq.Expressions.Expression.Constant("key");
        var indexExpr = System.Linq.Expressions.Expression.MakeIndex(dictProperty, indexer, new[] { keyArg });

        var args = indexExpr.GetArgumentsArray();

        await Assert.That(args).IsNotNull();
        await Assert.That(args!.Length).IsEqualTo(1);
        await Assert.That(args[0]).IsEqualTo("key");
    }

    [Test]
    public async Task GetArgumentsArray_WithMultiDimensionalIndex_ReturnsAllArguments()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TestClass), "x");
        var dictProperty = System.Linq.Expressions.Expression.Property(parameter, "Dictionary");
        var key = System.Linq.Expressions.Expression.Constant("key");
        var indexExpr = System.Linq.Expressions.Expression.Property(dictProperty, "Item", key);

        var args = indexExpr.GetArgumentsArray();

        await Assert.That(args).IsNotNull();
        await Assert.That(args!.Length).IsEqualTo(1);
        await Assert.That(args[0]).IsEqualTo("key");
    }

    [Test]
    public async Task GetArgumentsArray_WithNonIndexExpression_ReturnsNull()
    {
        System.Linq.Expressions.Expression<Func<TestClass, string?>> expr = x => x.Property;

        var args = expr.Body.GetArgumentsArray();

        await Assert.That(args).IsNull();
    }

    public class TestClass
    {
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Public field required for reflection tests")]
        public int PublicField;

        public event EventHandler? TestEvent;

        public static string? StaticProperty { get; set; }

        public string? Property { get; set; }

        public TestClass? Nested { get; set; }

        public int[] Array { get; set; } = [1, 2, 3];

        public List<int> List { get; set; } = [1, 2, 3];

        public Dictionary<string, int> Dictionary { get; set; } = new Dictionary<string, int> { { "key", 42 } };

        public void RaiseTestEvent() => TestEvent?.Invoke(this, EventArgs.Empty);
    }
}
