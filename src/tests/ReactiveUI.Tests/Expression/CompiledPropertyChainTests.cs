// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

namespace ReactiveUI.Tests.Expression;

/// <summary>
/// Tests for internal Compiled PropertyChain classes using reflection to access internal types.
/// </summary>
public class CompiledPropertyChainTests
{
    [Test]
    public async Task CompiledPropertyChain_WithSimpleProperty_GetsValue()
    {
        var obj = new TestViewModel { Name = "test" };
        Expression<Func<TestViewModel, string?>> expr = x => x.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        // Use reflection to create CompiledPropertyChain<TestViewModel, string>
        var chainType = typeof(Reflection).GetNestedType("CompiledPropertyChain`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));
        var instance = Activator.CreateInstance(chainType, [chain])!;

        // Call TryGetValue
        var tryGetValue = chainType.GetMethod("TryGetValue")!;
        var parameters = new object?[] { obj, null };
        var result = (bool)tryGetValue.Invoke(instance, parameters)!;
        var value = (string?)parameters[1];

        await Assert.That(result).IsTrue();
        await Assert.That(value).IsEqualTo("test");
    }

    [Test]
    public async Task CompiledPropertyChain_WithNestedProperty_GetsValue()
    {
        var obj = new TestViewModel
        {
            Child = new TestViewModel { Name = "nested" }
        };
        Expression<Func<TestViewModel, string?>> expr = x => x.Child!.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var chainType = typeof(Reflection).GetNestedType("CompiledPropertyChain`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));
        var instance = Activator.CreateInstance(chainType, [chain])!;

        var tryGetValue = chainType.GetMethod("TryGetValue")!;
        var parameters = new object?[] { obj, null };
        var result = (bool)tryGetValue.Invoke(instance, parameters)!;
        var value = (string?)parameters[1];

        await Assert.That(result).IsTrue();
        await Assert.That(value).IsEqualTo("nested");
    }

    [Test]
    public async Task CompiledPropertyChain_WithNullIntermediate_ReturnsFalse()
    {
        var obj = new TestViewModel { Child = null };
        Expression<Func<TestViewModel, string?>> expr = x => x.Child!.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var chainType = typeof(Reflection).GetNestedType("CompiledPropertyChain`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));
        var instance = Activator.CreateInstance(chainType, [chain])!;

        var tryGetValue = chainType.GetMethod("TryGetValue")!;
        var parameters = new object?[] { obj, null };
        var result = (bool)tryGetValue.Invoke(instance, parameters)!;

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task CompiledPropertyChain_WithNullSource_ReturnsFalse()
    {
        Expression<Func<TestViewModel, string?>> expr = x => x.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var chainType = typeof(Reflection).GetNestedType("CompiledPropertyChain`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));
        var instance = Activator.CreateInstance(chainType, [chain])!;

        var tryGetValue = chainType.GetMethod("TryGetValue")!;
        var parameters = new object?[] { null, null };
        var result = (bool)tryGetValue.Invoke(instance, parameters)!;

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task CompiledPropertyChain_TryGetAllValues_ReturnsAllChanges()
    {
        var obj = new TestViewModel
        {
            Child = new TestViewModel { Name = "nested" }
        };
        Expression<Func<TestViewModel, string?>> expr = x => x.Child!.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var chainType = typeof(Reflection).GetNestedType("CompiledPropertyChain`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));
        var instance = Activator.CreateInstance(chainType, [chain])!;

        var tryGetAllValues = chainType.GetMethod("TryGetAllValues")!;
        var parameters = new object?[] { obj, null };
        var result = (bool)tryGetAllValues.Invoke(instance, parameters)!;
        var changeValues = (IObservedChange<object, object?>[])parameters[1]!;

        await Assert.That(result).IsTrue();
        await Assert.That(changeValues.Length).IsEqualTo(2);
        await Assert.That(changeValues[1].Value).IsEqualTo("nested");
    }

    [Test]
    public async Task CompiledPropertyChain_TryGetAllValues_WithNullIntermediate_ReturnsFalse()
    {
        var obj = new TestViewModel { Child = null };
        Expression<Func<TestViewModel, string?>> expr = x => x.Child!.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var chainType = typeof(Reflection).GetNestedType("CompiledPropertyChain`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));
        var instance = Activator.CreateInstance(chainType, [chain])!;

        var tryGetAllValues = chainType.GetMethod("TryGetAllValues")!;
        var parameters = new object?[] { obj, null };
        var result = (bool)tryGetAllValues.Invoke(instance, parameters)!;
        var changeValues = (IObservedChange<object, object?>[])parameters[1]!;

        await Assert.That(result).IsFalse();

        // The implementation creates an ObservedChange with null value at the failing index
        await Assert.That(changeValues[0].Value).IsNull();
    }

    [Test]
    public async Task CompiledPropertyChain_Constructor_WithEmptyChain_Throws()
    {
        var emptyChain = Array.Empty<System.Linq.Expressions.Expression>();

        var chainType = typeof(Reflection).GetNestedType("CompiledPropertyChain`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));

        await Assert.That(() => Activator.CreateInstance(chainType, [emptyChain]))
            .Throws<TargetInvocationException>();
    }

    [Test]
    public async Task CompiledPropertyChainSetter_WithSimpleProperty_SetsValue()
    {
        var obj = new TestViewModel();
        Expression<Func<TestViewModel, string?>> expr = x => x.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var setterType = typeof(Reflection).GetNestedType("CompiledPropertyChainSetter`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));
        var instance = Activator.CreateInstance(setterType, [chain])!;

        var trySetValue = setterType.GetMethod("TrySetValue")!;
        var parameters = new object?[] { obj, "newValue", true };
        var result = (bool)trySetValue.Invoke(instance, parameters)!;

        await Assert.That(result).IsTrue();
        await Assert.That(obj.Name).IsEqualTo("newValue");
    }

    [Test]
    public async Task CompiledPropertyChainSetter_WithNestedProperty_SetsValue()
    {
        var obj = new TestViewModel
        {
            Child = new TestViewModel()
        };
        Expression<Func<TestViewModel, string?>> expr = x => x.Child!.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var setterType = typeof(Reflection).GetNestedType("CompiledPropertyChainSetter`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));
        var instance = Activator.CreateInstance(setterType, [chain])!;

        var trySetValue = setterType.GetMethod("TrySetValue")!;
        var parameters = new object?[] { obj, "nestedValue", true };
        var result = (bool)trySetValue.Invoke(instance, parameters)!;

        await Assert.That(result).IsTrue();
        await Assert.That(obj.Child!.Name).IsEqualTo("nestedValue");
    }

    [Test]
    public async Task CompiledPropertyChainSetter_WithNullSource_ShouldThrowTrue_Throws()
    {
        Expression<Func<TestViewModel, string?>> expr = x => x.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var setterType = typeof(Reflection).GetNestedType("CompiledPropertyChainSetter`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));
        var instance = Activator.CreateInstance(setterType, [chain])!;

        var trySetValue = setterType.GetMethod("TrySetValue")!;
        var parameters = new object?[] { null, "value", true };

        await Assert.That(() => trySetValue.Invoke(instance, parameters))
            .Throws<TargetInvocationException>();
    }

    [Test]
    public async Task CompiledPropertyChainSetter_WithNullSource_ShouldThrowFalse_ReturnsFalse()
    {
        Expression<Func<TestViewModel, string?>> expr = x => x.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var setterType = typeof(Reflection).GetNestedType("CompiledPropertyChainSetter`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));
        var instance = Activator.CreateInstance(setterType, [chain])!;

        var trySetValue = setterType.GetMethod("TrySetValue")!;
        var parameters = new object?[] { null, "value", false };
        var result = (bool)trySetValue.Invoke(instance, parameters)!;

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task CompiledPropertyChainSetter_WithNullIntermediate_ReturnsFalse()
    {
        var obj = new TestViewModel { Child = null };
        Expression<Func<TestViewModel, string?>> expr = x => x.Child!.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var setterType = typeof(Reflection).GetNestedType("CompiledPropertyChainSetter`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));
        var instance = Activator.CreateInstance(setterType, [chain])!;

        var trySetValue = setterType.GetMethod("TrySetValue")!;
        var parameters = new object?[] { obj, "value", false };
        var result = (bool)trySetValue.Invoke(instance, parameters)!;

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task CompiledPropertyChainSetter_WithDeepNesting_TraversesCorrectly()
    {
        var obj = new TestViewModel
        {
            Child = new TestViewModel
            {
                Child = new TestViewModel()
            }
        };
        Expression<Func<TestViewModel, string?>> expr = x => x.Child!.Child!.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var setterType = typeof(Reflection).GetNestedType("CompiledPropertyChainSetter`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));
        var instance = Activator.CreateInstance(setterType, [chain])!;

        var trySetValue = setterType.GetMethod("TrySetValue")!;
        var parameters = new object?[] { obj, "deepValue", true };
        var result = (bool)trySetValue.Invoke(instance, parameters)!;

        await Assert.That(result).IsTrue();
        await Assert.That(obj.Child!.Child!.Name).IsEqualTo("deepValue");
    }

    [Test]
    public async Task CompiledPropertyChainSetter_WithSingleLevelProperty_SetsDirectly()
    {
        var obj = new TestViewModel();
        Expression<Func<TestViewModel, string?>> expr = x => x.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var setterType = typeof(Reflection).GetNestedType("CompiledPropertyChainSetter`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));
        var instance = Activator.CreateInstance(setterType, [chain])!;

        var trySetValue = setterType.GetMethod("TrySetValue")!;
        var parameters = new object?[] { obj, "directValue", true };
        var result = (bool)trySetValue.Invoke(instance, parameters)!;

        await Assert.That(result).IsTrue();
        await Assert.That(obj.Name).IsEqualTo("directValue");
    }

    [Test]
    public async Task CompiledPropertyChainSetter_Constructor_WithEmptyChain_Throws()
    {
        var emptyChain = Array.Empty<System.Linq.Expressions.Expression>();

        var setterType = typeof(Reflection).GetNestedType("CompiledPropertyChainSetter`2", BindingFlags.NonPublic)!
            .MakeGenericType(typeof(TestViewModel), typeof(string));

        await Assert.That(() => Activator.CreateInstance(setterType, [emptyChain]))
            .Throws<TargetInvocationException>();
    }

    private class TestViewModel : ReactiveObject
    {
        private string? _name;
        private TestViewModel? _child;

        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public TestViewModel? Child
        {
            get => _child;
            set => this.RaiseAndSetIfChanged(ref _child, value);
        }

        public string ReadOnlyProperty { get; } = "readonly";
    }
}
