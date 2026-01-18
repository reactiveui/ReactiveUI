// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using ReactiveUI.Tests.Utilities.AppBuilder;

namespace ReactiveUI.Tests;

/// <summary>
/// Unit tests for <see cref="PropertyBindingExpressionCompiler"/>.
/// </summary>
/// <remarks>
/// These tests verify expression compilation, analysis, and observable creation logic.
/// </remarks>
[TestExecutor<AppBuilderTestExecutor>]
public class PropertyBindingExpressionCompilerTests
{
    /// <summary>
    /// Verifies that CreateSetThenGet creates a working set-then-get function for a simple property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateSetThenGet_ForSimpleProperty_SetsAndGetsValue()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();
        var view = new TestView();

        Expression<Func<TestView, string?>> expr = v => v.SomeStringProperty;
        var rewritten = Reflection.Rewrite(expr.Body);
        var memberInfo = rewritten.GetMemberInfo();
        var getter = Reflection.GetValueFetcherOrThrow(memberInfo) ?? throw new InvalidOperationException("Getter not found");
        var setter = Reflection.GetValueSetterOrThrow(memberInfo);

        // Act
        var setThenGet = compiler.CreateSetThenGet(rewritten, getter, setter, (_, _) => null);
        var (shouldEmit, value) = setThenGet(view, "TestValue", null);

        // Assert
        await Assert.That(shouldEmit).IsTrue();
        await Assert.That(value).IsEqualTo("TestValue");
        await Assert.That(view.SomeStringProperty).IsEqualTo("TestValue");
    }

    /// <summary>
    /// Verifies that CreateSetThenGet does not emit when value hasn't changed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateSetThenGet_WhenValueUnchanged_DoesNotEmit()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();
        var view = new TestView { SomeStringProperty = "InitialValue" };

        Expression<Func<TestView, string?>> expr = v => v.SomeStringProperty;
        var rewritten = Reflection.Rewrite(expr.Body);
        var memberInfo = rewritten.GetMemberInfo();
        var getter = Reflection.GetValueFetcherOrThrow(memberInfo) ?? throw new InvalidOperationException("Getter not found");
        var setter = Reflection.GetValueSetterOrThrow(memberInfo);

        // Act
        var setThenGet = compiler.CreateSetThenGet(rewritten, getter, setter, (_, _) => null);
        var (shouldEmit, value) = setThenGet(view, "InitialValue", null);

        // Assert
        await Assert.That(shouldEmit).IsFalse();
        await Assert.That(value).IsEqualTo("InitialValue");
    }

    /// <summary>
    /// Verifies that CreateSetThenGet with a converter converts the value before setting.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateSetThenGet_WithConverter_ConvertsAndSetsValue()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();
        var view = new TestView();

        Expression<Func<TestView, string?>> expr = v => v.SomeStringProperty;
        var rewritten = Reflection.Rewrite(expr.Body);
        var memberInfo = rewritten.GetMemberInfo();
        var getter = Reflection.GetValueFetcherOrThrow(memberInfo) ?? throw new InvalidOperationException("Getter not found");
        var setter = Reflection.GetValueSetterOrThrow(memberInfo);

        // Act
        // Converter that appends "Converted"
        Func<object?, object?, object?[]?, object?> converter = (current, input, _) => input + "Converted";
        var setThenGet = compiler.CreateSetThenGet(rewritten, getter, setter, (_, _) => converter);
        var (shouldEmit, value) = setThenGet(view, "Test", null);

        // Assert
        await Assert.That(shouldEmit).IsTrue();
        await Assert.That(value).IsEqualTo("TestConverted");
        await Assert.That(view.SomeStringProperty).IsEqualTo("TestConverted");
    }

    /// <summary>
    /// Verifies that CreateSetThenGet with a converter does not emit if the converted value matches existing.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateSetThenGet_WithConverter_WhenConvertedValueUnchanged_DoesNotEmit()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();
        var view = new TestView { SomeStringProperty = "TestConverted" };

        Expression<Func<TestView, string?>> expr = v => v.SomeStringProperty;
        var rewritten = Reflection.Rewrite(expr.Body);
        var memberInfo = rewritten.GetMemberInfo();
        var getter = Reflection.GetValueFetcherOrThrow(memberInfo) ?? throw new InvalidOperationException("Getter not found");
        var setter = Reflection.GetValueSetterOrThrow(memberInfo);

        // Act
        // Converter that appends "Converted"
        Func<object?, object?, object?[]?, object?> converter = (current, input, _) => input + "Converted";
        var setThenGet = compiler.CreateSetThenGet(rewritten, getter, setter, (_, _) => converter);
        var (shouldEmit, value) = setThenGet(view, "Test", null); // "Test" -> "TestConverted" which matches current

        // Assert
        await Assert.That(shouldEmit).IsFalse();
        await Assert.That(value).IsEqualTo("TestConverted");
    }

    /// <summary>
    /// Verifies that IsDirectMemberAccess returns true for direct property access.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task IsDirectMemberAccess_ForDirectProperty_ReturnsTrue()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();
        Expression<Func<TestView, string?>> expr = v => v.SomeStringProperty;
        var rewritten = Reflection.Rewrite(expr.Body);

        // Act
        var result = compiler.IsDirectMemberAccess(rewritten);

        // Assert
        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// Verifies that IsDirectMemberAccess returns false for chained property access.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task IsDirectMemberAccess_ForChainedProperty_ReturnsFalse()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();
        Expression<Func<TestViewModel, int>> expr = vm => vm.Model!.AnotherProperty;
        var rewritten = Reflection.Rewrite(expr.Body);

        // Act
        var result = compiler.IsDirectMemberAccess(rewritten);

        // Assert
        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Verifies that ShouldReplayOnHostChanges returns false for IViewFor.ViewModel property chains.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldReplayOnHostChanges_ForViewModelProperty_ReturnsFalse()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();
        Expression<Func<TestView, int>> expr = v => v.ViewModel!.Property1;
        var rewritten = Reflection.Rewrite(expr.Body);
        var chainArray = compiler.GetExpressionChainArray(rewritten.GetParent());

        // Act
        var result = compiler.ShouldReplayOnHostChanges(chainArray);

        // Assert - Should not replay when through ViewModel property
        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Verifies that ShouldReplayOnHostChanges returns true for non-ViewModel property chains.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldReplayOnHostChanges_ForNonViewModelProperty_ReturnsTrue()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();
        Expression<Func<TestViewModel, int>> expr = vm => vm.Model!.AnotherProperty;
        var rewritten = Reflection.Rewrite(expr.Body);
        var chainArray = compiler.GetExpressionChainArray(rewritten.GetParent());

        // Act
        var result = compiler.ShouldReplayOnHostChanges(chainArray);

        // Assert
        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// Verifies that ShouldReplayOnHostChanges returns true when chain is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldReplayOnHostChanges_WithNullChain_ReturnsTrue()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();

        // Act
        var result = compiler.ShouldReplayOnHostChanges(null);

        // Assert
        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// Verifies that GetExpressionChainArray returns null for null input.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetExpressionChainArray_WithNullExpression_ReturnsNull()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();

        // Act
        var result = compiler.GetExpressionChainArray(null);

        // Assert
        await Assert.That(result).IsNull();
    }

    /// <summary>
    /// Verifies that GetExpressionChainArray returns non-null for valid expression.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetExpressionChainArray_WithValidExpression_ReturnsArray()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();
        Expression<Func<TestView, string?>> expr = v => v.SomeStringProperty;
        var rewritten = Reflection.Rewrite(expr.Body);

        // Act
        var result = compiler.GetExpressionChainArray(rewritten);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Length).IsGreaterThan(0);
    }

    /// <summary>
    /// Verifies that CreateDirectSetObservable emits changes when observable updates.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateDirectSetObservable_EmitsChanges()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();
        var view = new TestView();
        var updates = new System.Reactive.Subjects.Subject<string>();

        Expression<Func<TestView, string?>> expr = v => v.SomeStringProperty;
        var rewritten = Reflection.Rewrite(expr.Body);
        var memberInfo = rewritten.GetMemberInfo();
        var getter = Reflection.GetValueFetcherOrThrow(memberInfo) ?? throw new InvalidOperationException("Getter not found");
        var setter = Reflection.GetValueSetterOrThrow(memberInfo);

        // Act
        var observable = compiler.CreateDirectSetObservable<TestView, string?, string>(
            view,
            updates,
            rewritten,
            getter,
            setter,
            (_, _) => null); // No converter

        var emitted = new List<string?>();
        using var sub = observable.Subscribe(x => emitted.Add(x.Value));

        // Emit changes
        updates.OnNext("First");
        updates.OnNext("Second");

        // Assert
        await Assert.That(view.SomeStringProperty).IsEqualTo("Second");
        await Assert.That(emitted.Count).IsEqualTo(2);
        await Assert.That(emitted[0]).IsEqualTo("First");
        await Assert.That(emitted[1]).IsEqualTo("Second");
    }

    /// <summary>
    /// Verifies that CreateDirectSetObservable does not emit when values are unchanged.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateDirectSetObservable_WhenValueUnchanged_DoesNotEmit()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();
        var view = new TestView { SomeStringProperty = "Initial" };
        var updates = new System.Reactive.Subjects.Subject<string>();

        Expression<Func<TestView, string?>> expr = v => v.SomeStringProperty;
        var rewritten = Reflection.Rewrite(expr.Body);
        var memberInfo = rewritten.GetMemberInfo();
        var getter = Reflection.GetValueFetcherOrThrow(memberInfo) ?? throw new InvalidOperationException("Getter not found");
        var setter = Reflection.GetValueSetterOrThrow(memberInfo);

        // Act
        var observable = compiler.CreateDirectSetObservable<TestView, string?, string>(
            view,
            updates,
            rewritten,
            getter,
            setter,
            (_, _) => null);

        var emitted = new List<string?>();
        using var sub = observable.Subscribe(x => emitted.Add(x.Value));

        // Emit same value twice
        updates.OnNext("Initial");
        updates.OnNext("Changed");
        updates.OnNext("Changed");

        // Assert
        await Assert.That(emitted.Count).IsEqualTo(1); // Only "Changed" should emit (not "Initial" or duplicate "Changed")
        await Assert.That(emitted[0]).IsEqualTo("Changed");
    }

    /// <summary>
    /// Verifies that CreateDirectSetObservable applies converter before setting.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateDirectSetObservable_WithConverter_ConvertsAndSetsValue()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();
        var view = new TestView();
        var updates = new System.Reactive.Subjects.Subject<int>();

        Expression<Func<TestView, string?>> expr = v => v.SomeStringProperty;
        var rewritten = Reflection.Rewrite(expr.Body);
        var memberInfo = rewritten.GetMemberInfo();
        var getter = Reflection.GetValueFetcherOrThrow(memberInfo) ?? throw new InvalidOperationException("Getter not found");
        var setter = Reflection.GetValueSetterOrThrow(memberInfo);

        // Act
        // Converter that converts int to string with prefix
        Func<object?, object?, object?[]?, object?> converter = (current, input, _) => "Number:" + input;
        var observable = compiler.CreateDirectSetObservable<TestView, string?, int>(
            view,
            updates,
            rewritten,
            getter,
            setter,
            (_, _) => converter);

        var emitted = new List<string?>();
        using var sub = observable.Subscribe(x => emitted.Add(x.Value));

        updates.OnNext(42);

        // Assert
        await Assert.That(view.SomeStringProperty).IsEqualTo("Number:42");
        await Assert.That(emitted.Count).IsEqualTo(1);
        await Assert.That(emitted[0]).IsEqualTo("Number:42");
    }

    /// <summary>
    /// Verifies that CreateChainedSetObservable emits changes when property chain updates.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateChainedSetObservable_EmitsChanges()
    {
        // Arrange
        var compiler = new PropertyBindingExpressionCompiler();
        var viewModel = new TestViewModel { Model = new TestModel { AnotherProperty = 10 } };
        var updates = new System.Reactive.Subjects.Subject<int>();

        Expression<Func<TestViewModel, int>> expr = vm => vm.Model!.AnotherProperty;
        var rewritten = Reflection.Rewrite(expr.Body);
        var chain = compiler.GetExpressionChainArray(rewritten.GetParent())!;
        var memberInfo = rewritten.GetMemberInfo();
        var getter = Reflection.GetValueFetcherOrThrow(memberInfo) ?? throw new InvalidOperationException("Getter not found");
        var setter = Reflection.GetValueSetterOrThrow(memberInfo);

        // Act
        var observable = compiler.CreateChainedSetObservable<TestViewModel, int, int>(
            viewModel,
            updates,
            rewritten,
            chain,
            getter,
            setter,
            (_, _) => null); // No converter

        var emitted = new List<int>();
        using var sub = observable.Subscribe(x => emitted.Add(x.Value));

        // 1. Emit update
        updates.OnNext(20);
        await Assert.That(viewModel.Model.AnotherProperty).IsEqualTo(20);
        await Assert.That(emitted.Count).IsEqualTo(1);
        await Assert.That(emitted[0]).IsEqualTo(20);

        // 2. Change host (Model) - should not overwrite if property is non-default
        var newModel = new TestModel { AnotherProperty = 100 };
        viewModel.Model = newModel;

        await Assert.That(newModel.AnotherProperty).IsEqualTo(100);

        // 3. Change host (Model) with default value - should overwrite with last observed (20)
        var defaultModel = new TestModel { AnotherProperty = 0 };
        viewModel.Model = defaultModel;

        await Assert.That(defaultModel.AnotherProperty).IsEqualTo(20);
        await Assert.That(emitted.Last()).IsEqualTo(20);
    }

    /// <summary>
    /// Test helper view class.
    /// </summary>
    private class TestView : ReactiveObject, IViewFor<TestViewModel>
    {
        private TestViewModel? _viewModel;
        private string? _someStringProperty;
        private int _someIntProperty;

        public TestViewModel? ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }

        public string? SomeStringProperty
        {
            get => _someStringProperty;
            set => this.RaiseAndSetIfChanged(ref _someStringProperty, value);
        }

        public int SomeIntProperty
        {
            get => _someIntProperty;
            set => this.RaiseAndSetIfChanged(ref _someIntProperty, value);
        }
    }

    /// <summary>
    /// Test helper view model class.
    /// </summary>
    private class TestViewModel : ReactiveObject
    {
        private int _property1;
        private TestModel? _model;

        public int Property1
        {
            get => _property1;
            set => this.RaiseAndSetIfChanged(ref _property1, value);
        }

        public TestModel? Model
        {
            get => _model;
            set => this.RaiseAndSetIfChanged(ref _model, value);
        }
    }

    /// <summary>
    /// Test helper model class.
    /// </summary>
    private class TestModel : ReactiveObject
    {
        private int _anotherProperty;

        public int AnotherProperty
        {
            get => _anotherProperty;
            set => this.RaiseAndSetIfChanged(ref _anotherProperty, value);
        }
    }
}
