// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ReactiveUI.Tests.Expressions;

/// <summary>Tests for the internal compiled property chain classes.</summary>
public class CompiledPropertyChainTests
{
    /// <summary>The value assigned to a nested property in compiled property chain tests.</summary>
    private const string NestedValueText = "nested";

    /// <summary>The value assigned to a simple property in compiled property chain tests.</summary>
    private const string ValueText = "value";

    /// <summary>Verifies that the compiled property chain reads a value from a simple property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChain_WithSimpleProperty_GetsValue()
    {
        var obj = new TestViewModel { Name = "test" };
        Expression<Func<TestViewModel, string?>> expr = x => x.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var instance = new Reflection.CompiledPropertyChain<TestViewModel, string>(chain);
        var result = instance.TryGetValue(obj, out var value);

        await Assert.That(result).IsTrue();
        await Assert.That(value).IsEqualTo("test");
    }

    /// <summary>Verifies that the compiled property chain reads a value through a nested property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChain_WithNestedProperty_GetsValue()
    {
        var obj = new TestViewModel { Child = new() { Name = NestedValueText } };
        Expression<Func<TestViewModel, string?>> expr = x => x.Child!.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var instance = new Reflection.CompiledPropertyChain<TestViewModel, string>(chain);
        var result = instance.TryGetValue(obj, out var value);

        await Assert.That(result).IsTrue();
        await Assert.That(value).IsEqualTo(NestedValueText);
    }

    /// <summary>Verifies that the compiled property chain returns false when an intermediate value is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChain_WithNullIntermediate_ReturnsFalse()
    {
        var obj = new TestViewModel { Child = null };
        Expression<Func<TestViewModel, string?>> expr = x => x.Child!.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var instance = new Reflection.CompiledPropertyChain<TestViewModel, string>(chain);
        var result = instance.TryGetValue(obj, out _);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that the compiled property chain returns false when the source object is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChain_WithNullSource_ReturnsFalse()
    {
        Expression<Func<TestViewModel, string?>> expr = x => x.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var instance = new Reflection.CompiledPropertyChain<TestViewModel, string>(chain);
        var result = instance.TryGetValue(null, out _);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that TryGetAllValues returns the observed change for every link in the chain.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChain_TryGetAllValues_ReturnsAllChanges()
    {
        var obj = new TestViewModel { Child = new() { Name = NestedValueText } };
        Expression<Func<TestViewModel, string?>> expr = x => x.Child!.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var instance = new Reflection.CompiledPropertyChain<TestViewModel, string>(chain);
        var result = instance.TryGetAllValues(obj, out var changeValues);

        const int ExpectedChangeCount = 2;
        await Assert.That(result).IsTrue();
        await Assert.That(changeValues.Length).IsEqualTo(ExpectedChangeCount);
        await Assert.That(changeValues[1].Value).IsEqualTo(NestedValueText);
    }

    /// <summary>Verifies that TryGetAllValues returns false when an intermediate value in the chain is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChain_TryGetAllValues_WithNullIntermediate_ReturnsFalse()
    {
        var obj = new TestViewModel { Child = null };
        Expression<Func<TestViewModel, string?>> expr = x => x.Child!.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var instance = new Reflection.CompiledPropertyChain<TestViewModel, string>(chain);
        var result = instance.TryGetAllValues(obj, out var changeValues);

        await Assert.That(result).IsFalse();

        // The implementation creates an ObservedChange with null value at the failing index
        await Assert.That(changeValues[0].Value).IsNull();
    }

    /// <summary>Verifies that constructing a compiled property chain with an empty chain throws.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChain_Constructor_WithEmptyChain_Throws()
    {
        var emptyChain = Array.Empty<Expression>();

        await Assert.That(() => new Reflection.CompiledPropertyChain<TestViewModel, string>(emptyChain))
            .Throws<InvalidOperationException>();
    }

    /// <summary>Verifies that the compiled property chain setter writes a value to a simple property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChainSetter_WithSimpleProperty_SetsValue()
    {
        var obj = new TestViewModel();
        Expression<Func<TestViewModel, string?>> expr = x => x.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var setter = new Reflection.CompiledPropertyChainSetter<TestViewModel, string>(chain);
        var result = setter.TrySetValue(obj, "newValue");

        await Assert.That(result).IsTrue();
        await Assert.That(obj.Name).IsEqualTo("newValue");
    }

    /// <summary>Verifies that the compiled property chain setter writes a value through a nested property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChainSetter_WithNestedProperty_SetsValue()
    {
        var obj = new TestViewModel { Child = new() };
        Expression<Func<TestViewModel, string?>> expr = x => x.Child!.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var setter = new Reflection.CompiledPropertyChainSetter<TestViewModel, string>(chain);
        var result = setter.TrySetValue(obj, "nestedValue");

        await Assert.That(result).IsTrue();
        await Assert.That(obj.Child!.Name).IsEqualTo("nestedValue");
    }

    /// <summary>Verifies that the setter throws on a null source when configured to throw.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChainSetter_WithNullSource_ShouldThrowTrue_Throws()
    {
        Expression<Func<TestViewModel, string?>> expr = x => x.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var setter = new Reflection.CompiledPropertyChainSetter<TestViewModel, string>(chain);

        await Assert.That(() => setter.TrySetValue(null, ValueText))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the setter returns false on a null source when configured not to throw.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChainSetter_WithNullSource_ShouldThrowFalse_ReturnsFalse()
    {
        Expression<Func<TestViewModel, string?>> expr = x => x.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var setter = new Reflection.CompiledPropertyChainSetter<TestViewModel, string>(chain);
        var result = setter.TrySetValue(null, ValueText, false);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that the setter returns false when an intermediate value in the chain is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChainSetter_WithNullIntermediate_ReturnsFalse()
    {
        var obj = new TestViewModel { Child = null };
        Expression<Func<TestViewModel, string?>> expr = x => x.Child!.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var setter = new Reflection.CompiledPropertyChainSetter<TestViewModel, string>(chain);
        var result = setter.TrySetValue(obj, ValueText, false);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that the setter traverses a deeply nested chain before writing the value.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChainSetter_WithDeepNesting_TraversesCorrectly()
    {
        var obj = new TestViewModel { Child = new() { Child = new() } };
        Expression<Func<TestViewModel, string?>> expr = x => x.Child!.Child!.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var setter = new Reflection.CompiledPropertyChainSetter<TestViewModel, string>(chain);
        var result = setter.TrySetValue(obj, "deepValue");

        await Assert.That(result).IsTrue();
        await Assert.That(obj.Child!.Child!.Name).IsEqualTo("deepValue");
    }

    /// <summary>Verifies that the setter writes directly to a single-level property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChainSetter_WithSingleLevelProperty_SetsDirectly()
    {
        var obj = new TestViewModel();
        Expression<Func<TestViewModel, string?>> expr = x => x.Name;
        var chain = expr.Body.GetExpressionChain().ToArray();

        var setter = new Reflection.CompiledPropertyChainSetter<TestViewModel, string>(chain);
        var result = setter.TrySetValue(obj, "directValue");

        await Assert.That(result).IsTrue();
        await Assert.That(obj.Name).IsEqualTo("directValue");
    }

    /// <summary>Verifies that constructing a compiled property chain setter with an empty chain throws.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CompiledPropertyChainSetter_Constructor_WithEmptyChain_Throws()
    {
        var emptyChain = Array.Empty<Expression>();

        await Assert.That(() => new Reflection.CompiledPropertyChainSetter<TestViewModel, string>(emptyChain))
            .Throws<InvalidOperationException>();
    }

    /// <summary>A reactive view model used as the target of compiled property chain tests.</summary>
    private sealed class TestViewModel : ReactiveObject
    {
        /// <summary>Gets or sets the name value.</summary>
        public string? Name
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the nested child view model.</summary>
        public TestViewModel? Child
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets a read-only property value.</summary>
        [SuppressMessage(
            "Design",
            "SST2324:Member is public but its containing type is not publicly reachable",
            Justification = "the public surface is required for interface/reflection binding; the containing test double is an intentionally non-public detail.")]
        public string ReadOnlyProperty { get; } = "readonly";
    }
}
