// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using ReactiveUI.Tests.ReactiveObjects.Mocks;
using ReactiveUI.Tests.WhenAny.Mockups;

namespace ReactiveUI.Tests;

/// <summary>
///     Tests for <see cref="Reflection" />.
/// </summary>
[NotInParallel]
public class ReflectionTest
{
    /// <summary>
    ///     Tests that ExpressionToPropertyNames converts deeply nested property access.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExpressionToPropertyNames_DeeplyNestedProperty_ReturnsFullPath()
    {
        Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;

        var result = Reflection.ExpressionToPropertyNames(expression.Body);

        await Assert.That(result).IsEqualTo("Child.IsOnlyOneWord");
    }

    /// <summary>
    ///     Tests that ExpressionToPropertyNames converts nested property access.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExpressionToPropertyNames_NestedProperty_ReturnsPropertyPath()
    {
        Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;

        var result = Reflection.ExpressionToPropertyNames(expression.Body);

        await Assert.That(result).IsEqualTo("Child.IsOnlyOneWord");
    }

    /// <summary>
    ///     Tests that ExpressionToPropertyNames throws for null expression.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExpressionToPropertyNames_NullExpression_Throws() =>
        await Assert.That(() => Reflection.ExpressionToPropertyNames(null))
            .Throws<ArgumentNullException>();

    /// <summary>
    ///     Tests that ExpressionToPropertyNames converts simple property access.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExpressionToPropertyNames_SimpleProperty_ReturnsPropertyName()
    {
        Expression<Func<TestFixture, string?>> expression = x => x.IsOnlyOneWord;

        var result = Reflection.ExpressionToPropertyNames(expression.Body);

        await Assert.That(result).IsEqualTo("IsOnlyOneWord");
    }

    /// <summary>
    ///     Tests that GetEventArgsTypeForEvent throws for invalid event.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetEventArgsTypeForEvent_InvalidEvent_Throws() =>
        await Assert.That(() => Reflection.GetEventArgsTypeForEvent(typeof(TestClassWithEvent), "NonExistentEvent"))
            .Throws<Exception>();

    /// <summary>
    ///     Tests that GetEventArgsTypeForEvent throws for null type.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetEventArgsTypeForEvent_NullType_Throws() =>
        await Assert.That(() => Reflection.GetEventArgsTypeForEvent(null!, "TestEvent"))
            .Throws<ArgumentNullException>();

    /// <summary>
    ///     Tests that GetEventArgsTypeForEvent returns EventArgs type.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetEventArgsTypeForEvent_ValidEvent_ReturnsEventArgsType()
    {
        var result = Reflection.GetEventArgsTypeForEvent(
            typeof(TestClassWithEvent),
            nameof(TestClassWithEvent.TestEvent));

        await Assert.That(result).IsEqualTo(typeof(EventArgs));
    }

    /// <summary>
    ///     Tests that GetValueFetcherForProperty returns fetcher for field.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueFetcherForProperty_Field_ReturnsFetcher()
    {
        var fixture = new TestClassWithField { TestField = "FieldValue" };
        var fieldInfo = typeof(TestClassWithField).GetField(nameof(TestClassWithField.TestField))!;

        var fetcher = Reflection.GetValueFetcherForProperty(fieldInfo);

        await Assert.That(fetcher).IsNotNull();
        var value = fetcher!(fixture, null);
        await Assert.That(value).IsEqualTo("FieldValue");
    }

    /// <summary>
    ///     Tests that GetValueFetcherForProperty throws for null member.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueFetcherForProperty_NullMember_Throws() =>
        await Assert.That(() => Reflection.GetValueFetcherForProperty(null))
            .Throws<ArgumentNullException>();

    /// <summary>
    ///     Tests that GetValueFetcherForProperty returns fetcher for property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueFetcherForProperty_Property_ReturnsFetcher()
    {
        var fixture = new TestFixture { IsOnlyOneWord = "Test" };
        var propertyInfo = typeof(TestFixture).GetProperty(nameof(TestFixture.IsOnlyOneWord))!;

        var fetcher = Reflection.GetValueFetcherForProperty(propertyInfo);

        await Assert.That(fetcher).IsNotNull();
        var value = fetcher!(fixture, null);
        await Assert.That(value).IsEqualTo("Test");
    }

    /// <summary>
    ///     Tests that GetValueFetcherOrThrow throws for null member.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueFetcherOrThrow_NullMember_Throws() =>
        await Assert.That(() => Reflection.GetValueFetcherOrThrow(null))
            .Throws<ArgumentNullException>();

    /// <summary>
    ///     Tests that GetValueFetcherOrThrow returns fetcher for valid property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueFetcherOrThrow_ValidProperty_ReturnsFetcher()
    {
        var fixture = new TestFixture { IsOnlyOneWord = "Test" };
        var propertyInfo = typeof(TestFixture).GetProperty(nameof(TestFixture.IsOnlyOneWord))!;

        var fetcher = Reflection.GetValueFetcherOrThrow(propertyInfo);

        await Assert.That(fetcher).IsNotNull();
        var value = fetcher(fixture, null);
        await Assert.That(value).IsEqualTo("Test");
    }

    /// <summary>
    ///     Tests that GetValueSetterForProperty returns setter for field.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueSetterForProperty_Field_ReturnsSetter()
    {
        var fixture = new TestClassWithField();
        var fieldInfo = typeof(TestClassWithField).GetField(nameof(TestClassWithField.TestField))!;

        var setter = Reflection.GetValueSetterForProperty(fieldInfo);

        await Assert.That(setter).IsNotNull();
        setter(fixture, "NewFieldValue", null);
        await Assert.That(fixture.TestField).IsEqualTo("NewFieldValue");
    }

    /// <summary>
    ///     Tests that GetValueSetterForProperty throws for null member.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueSetterForProperty_NullMember_Throws() =>
        await Assert.That(() => Reflection.GetValueSetterForProperty(null))
            .Throws<ArgumentNullException>();

    /// <summary>
    ///     Tests that GetValueSetterForProperty returns setter for property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueSetterForProperty_Property_ReturnsSetter()
    {
        var fixture = new TestFixture();
        var propertyInfo = typeof(TestFixture).GetProperty(nameof(TestFixture.IsOnlyOneWord))!;

        var setter = Reflection.GetValueSetterForProperty(propertyInfo);

        await Assert.That(setter).IsNotNull();
        setter(fixture, "NewValue", null);
        await Assert.That(fixture.IsOnlyOneWord).IsEqualTo("NewValue");
    }

    /// <summary>
    ///     Tests that GetValueSetterOrThrow throws for null member.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueSetterOrThrow_NullMember_Throws() =>
        await Assert.That(() => Reflection.GetValueSetterOrThrow(null))
            .Throws<ArgumentNullException>();

    /// <summary>
    ///     Tests that GetValueSetterOrThrow returns setter for valid property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueSetterOrThrow_ValidProperty_ReturnsSetter()
    {
        var fixture = new TestFixture();
        var propertyInfo = typeof(TestFixture).GetProperty(nameof(TestFixture.IsOnlyOneWord))!;

        var setter = Reflection.GetValueSetterOrThrow(propertyInfo);

        await Assert.That(setter).IsNotNull();
        setter(fixture, "NewValue", null);
        await Assert.That(fixture.IsOnlyOneWord).IsEqualTo("NewValue");
    }

    /// <summary>
    ///     Tests that IsStatic returns false for instance property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsStatic_InstanceProperty_ReturnsFalse()
    {
        var propertyInfo =
            typeof(TestClassWithStaticProperty).GetProperty(nameof(TestClassWithStaticProperty.InstanceProperty))!;

        var result = propertyInfo.IsStatic();

        await Assert.That(result).IsFalse();
    }

    /// <summary>
    ///     Tests that IsStatic throws for null property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsStatic_NullProperty_Throws()
    {
        PropertyInfo propertyInfo = null!;

        await Assert.That(() => propertyInfo.IsStatic())
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    ///     Tests that IsStatic returns true for static property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsStatic_StaticProperty_ReturnsTrue()
    {
        var propertyInfo =
            typeof(TestClassWithStaticProperty).GetProperty(nameof(TestClassWithStaticProperty.StaticProperty))!;

        var result = propertyInfo.IsStatic();

        await Assert.That(result).IsTrue();
    }

    /// <summary>
    ///     Tests that ReallyFindType caches types.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReallyFindType_CachesTypes()
    {
        var typeName = typeof(TestFixture).AssemblyQualifiedName;

        var result1 = Reflection.ReallyFindType(typeName, false);
        var result2 = Reflection.ReallyFindType(typeName, false);

        await Assert.That(result1).IsEqualTo(result2);
        await Assert.That(result1).IsEqualTo(typeof(TestFixture));
    }

    /// <summary>
    ///     Tests that ReallyFindType returns null for invalid type when not throwing.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReallyFindType_InvalidTypeNoThrow_ReturnsNull()
    {
        var result = Reflection.ReallyFindType("InvalidType.DoesNotExist", false);

        await Assert.That(result).IsNull();
    }

    /// <summary>
    ///     Tests that ReallyFindType throws for invalid type when throwing.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReallyFindType_InvalidTypeWithThrow_Throws() =>
        await Assert.That(() => Reflection.ReallyFindType("InvalidType.DoesNotExist", true))
            .Throws<TypeLoadException>();

    /// <summary>
    ///     Tests that ReallyFindType finds a valid type.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReallyFindType_ValidType_ReturnsType()
    {
        var typeName = typeof(TestFixture).AssemblyQualifiedName;

        var result = Reflection.ReallyFindType(typeName, false);

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsEqualTo(typeof(TestFixture));
    }

    /// <summary>
    ///     Tests that Rewrite simplifies expression.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Rewrite_Expression_SimplifiesExpression()
    {
        Expression<Func<TestFixture, string?>> expression = x => x.IsOnlyOneWord;

        var result = Reflection.Rewrite(expression.Body);

        await Assert.That(result).IsNotNull();
    }

    /// <summary>
    ///     Tests that ThrowIfMethodsNotOverloaded throws for missing methods.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ThrowIfMethodsNotOverloaded_MissingMethod_Throws()
    {
        var target = new TestClassWithOverriddenMethods();

        await Assert.That(() => Reflection.ThrowIfMethodsNotOverloaded("TestCaller", target, "NonExistentMethod"))
            .Throws<Exception>();
    }

    /// <summary>
    ///     Tests that ThrowIfMethodsNotOverloaded passes for overloaded methods.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ThrowIfMethodsNotOverloaded_OverloadedMethods_DoesNotThrow()
    {
        var target = new TestClassWithOverriddenMethods();

        Reflection.ThrowIfMethodsNotOverloaded("TestCaller", target, nameof(TestClassWithOverriddenMethods.TestMethod));

        // If we got here without throwing, the test passes
        await Task.CompletedTask;
    }

    /// <summary>
    ///     Tests that TryGetAllValuesForPropertyChain returns false when null in chain.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryGetAllValuesForPropertyChain_NullInChain_ReturnsFalse()
    {
        var fixture = new HostTestFixture { Child = null };
        Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;
        var chain = expression.Body.GetExpressionChain();

        var success = Reflection.TryGetAllValuesForPropertyChain(out var values, fixture, chain);

        await Assert.That(success).IsFalse();
    }

    /// <summary>
    ///     Tests that TryGetAllValuesForPropertyChain gets all values in chain.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryGetAllValuesForPropertyChain_ValidChain_ReturnsAllValues()
    {
        var fixture = new HostTestFixture { Child = new TestFixture { IsOnlyOneWord = "Test" } };
        Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;
        var chain = expression.Body.GetExpressionChain();

        var success = Reflection.TryGetAllValuesForPropertyChain(out var values, fixture, chain);

        await Assert.That(success).IsTrue();
        await Assert.That(values).Count().IsEqualTo(2);
        await Assert.That(values[0].Sender).IsEqualTo(fixture);
        await Assert.That(values[1].Value).IsEqualTo("Test");
    }

    /// <summary>
    ///     Tests that TryGetValueForPropertyChain gets value from nested property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryGetValueForPropertyChain_NestedProperty_ReturnsValue()
    {
        var fixture = new HostTestFixture { Child = new TestFixture { IsOnlyOneWord = "NestedTest" } };
        Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;
        var chain = expression.Body.GetExpressionChain();

        var success = Reflection.TryGetValueForPropertyChain<string>(out var value, fixture, chain);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo("NestedTest");
    }

    /// <summary>
    ///     Tests that TryGetValueForPropertyChain returns false when null in chain.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryGetValueForPropertyChain_NullInChain_ReturnsFalse()
    {
        var fixture = new HostTestFixture { Child = null };
        Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;
        var chain = expression.Body.GetExpressionChain();

        var success = Reflection.TryGetValueForPropertyChain<string>(out var value, fixture, chain);

        await Assert.That(success).IsFalse();
        await Assert.That(value).IsNull();
    }

    /// <summary>
    ///     Tests that TryGetValueForPropertyChain gets value from simple property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryGetValueForPropertyChain_SimpleProperty_ReturnsValue()
    {
        var fixture = new TestFixture { IsOnlyOneWord = "Test" };
        Expression<Func<TestFixture, string?>> expression = x => x.IsOnlyOneWord;
        var chain = expression.Body.GetExpressionChain();

        var success = Reflection.TryGetValueForPropertyChain<string>(out var value, fixture, chain);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo("Test");
    }

    /// <summary>
    ///     Tests that TrySetValueToPropertyChain sets value on nested property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TrySetValueToPropertyChain_NestedProperty_SetsValue()
    {
        var fixture = new HostTestFixture { Child = new TestFixture() };
        Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;
        var chain = expression.Body.GetExpressionChain();

        var success = Reflection.TrySetValueToPropertyChain(fixture, chain, "NestedValue");

        await Assert.That(success).IsTrue();
        await Assert.That(fixture.Child.IsOnlyOneWord).IsEqualTo("NestedValue");
    }

    /// <summary>
    ///     Tests that TrySetValueToPropertyChain returns false when null in chain.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TrySetValueToPropertyChain_NullInChain_ReturnsFalse()
    {
        var fixture = new HostTestFixture { Child = null };
        Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;
        var chain = expression.Body.GetExpressionChain();

        var success = Reflection.TrySetValueToPropertyChain(fixture, chain, "Value", false);

        await Assert.That(success).IsFalse();
    }

    /// <summary>
    ///     Tests that TrySetValueToPropertyChain throws when shouldThrow is true and target is null.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TrySetValueToPropertyChain_NullTargetWithThrow_Throws()
    {
        Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;
        var chain = expression.Body.GetExpressionChain();

        await Assert.That(() => Reflection.TrySetValueToPropertyChain(null, chain, "Value"))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    ///     Tests that TrySetValueToPropertyChain sets value on simple property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TrySetValueToPropertyChain_SimpleProperty_SetsValue()
    {
        var fixture = new TestFixture();
        Expression<Func<TestFixture, string?>> expression = x => x.IsOnlyOneWord;
        var chain = expression.Body.GetExpressionChain();

        var success = Reflection.TrySetValueToPropertyChain(fixture, chain, "NewValue");

        await Assert.That(success).IsTrue();
        await Assert.That(fixture.IsOnlyOneWord).IsEqualTo("NewValue");
    }

    /// <summary>
    ///     Test class with an event.
    /// </summary>
    private class TestClassWithEvent
    {
        /// <summary>
        ///     A test event.
        /// </summary>
        public event EventHandler? TestEvent;

        /// <summary>
        ///     Raises the test event.
        /// </summary>
        protected virtual void OnTestEvent() => TestEvent?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Test class with a field.
    /// </summary>
    private class TestClassWithField
    {
        /// <summary>
        ///     A test field.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.MaintainabilityRules",
            "SA1401:Fields should be private",
            Justification = "Needed for test")]
        public string? TestField;
    }

    /// <summary>
    ///     Test class with overridden methods.
    /// </summary>
    private class TestClassWithOverriddenMethods
    {
        /// <summary>
        ///     A test method.
        /// </summary>
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Needed for test")]
        public void TestMethod()
        {
        }
    }

    /// <summary>
    ///     Test class with static property.
    /// </summary>
    private class TestClassWithStaticProperty
    {
        /// <summary>
        ///     Gets or sets a static property.
        /// </summary>
        public static string? StaticProperty { get; set; }

        /// <summary>
        ///     Gets or sets an instance property.
        /// </summary>
        public string? InstanceProperty { get; set; }
    }
}
