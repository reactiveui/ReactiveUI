// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace ReactiveUI.Tests;

/// <summary>
/// Unit tests for <see cref="PropertyBindingExpressionCompiler"/>.
/// </summary>
/// <remarks>
/// These tests verify expression compilation, analysis, and observable creation logic.
/// </remarks>
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
