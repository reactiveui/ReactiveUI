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

#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable SA1401 // Fields should be private
    public class TestClass
    {
        public static string? StaticProperty { get; set; }

        public string? Property { get; set; }

        public int PublicField;

        public TestClass? Nested { get; set; }

        public int[] Array { get; set; } = [1, 2, 3];

        public List<int> List { get; set; } = [1, 2, 3];

        public event EventHandler? TestEvent;

        public void RaiseTestEvent() => TestEvent?.Invoke(this, EventArgs.Empty);
    }
#pragma warning restore SA1401 // Fields should be private
#pragma warning restore SA1201 // Elements should appear in the correct order
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore CA1050 // Declare types in namespaces
}
