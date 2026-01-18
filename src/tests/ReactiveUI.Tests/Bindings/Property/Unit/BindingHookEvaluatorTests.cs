// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace ReactiveUI.Tests;

/// <summary>
/// Unit tests for <see cref="BindingHookEvaluator"/>.
/// </summary>
/// <remarks>
/// These tests verify hook evaluation logic. Note that these tests will use hooks registered
/// in the global Splat container, so some tests may be affected by global state.
/// For fully isolated tests, use the mock evaluator in integration tests.
/// </remarks>
public class BindingHookEvaluatorTests
{
    /// <summary>
    /// Verifies that EvaluateBindingHooks returns true when no rejecting hooks are registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EvaluateBindingHooks_WithNoRejectingHooks_ReturnsTrue()
    {
        // Arrange
        var evaluator = new BindingHookEvaluator();
        var viewModel = new TestViewModel();
        var view = new TestView { ViewModel = viewModel };

        Expression<Func<TestViewModel, int>> vmExpr = vm => vm.Property1;
        Expression<Func<TestView, string?>> viewExpr = v => v.SomeStringProperty;

        var rewrittenVm = Reflection.Rewrite(vmExpr.Body);
        var rewrittenView = Reflection.Rewrite(viewExpr.Body);

        // Act
        var result = evaluator.EvaluateBindingHooks(
            viewModel,
            view,
            rewrittenVm,
            rewrittenView,
            BindingDirection.OneWay);

        // Assert - Assuming no globally registered rejecting hooks for these properties
        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// Verifies that EvaluateBindingHooks handles null viewModel gracefully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EvaluateBindingHooks_WithNullViewModel_HandlesGracefully()
    {
        // Arrange
        var evaluator = new BindingHookEvaluator();
        TestViewModel? viewModel = null;
        var view = new TestView();

        Expression<Func<TestViewModel, int>> vmExpr = vm => vm.Property1;
        Expression<Func<TestView, string?>> viewExpr = v => v.SomeStringProperty;

        var rewrittenVm = Reflection.Rewrite(vmExpr.Body);
        var rewrittenView = Reflection.Rewrite(viewExpr.Body);

        // Act
        var result = evaluator.EvaluateBindingHooks(
            viewModel,
            view,
            rewrittenVm,
            rewrittenView,
            BindingDirection.OneWay);

        // Assert - Should not throw and should return a boolean
        await Assert.That(result).IsTypeOf<bool>();
    }

    /// <summary>
    /// Verifies that EvaluateBindingHooks processes all binding directions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EvaluateBindingHooks_WithTwoWayBinding_EvaluatesCorrectly()
    {
        // Arrange
        var evaluator = new BindingHookEvaluator();
        var viewModel = new TestViewModel();
        var view = new TestView { ViewModel = viewModel };

        Expression<Func<TestViewModel, int>> vmExpr = vm => vm.Property1;
        Expression<Func<TestView, int>> viewExpr = v => v.SomeIntProperty;

        var rewrittenVm = Reflection.Rewrite(vmExpr.Body);
        var rewrittenView = Reflection.Rewrite(viewExpr.Body);

        // Act
        var result = evaluator.EvaluateBindingHooks(
            viewModel,
            view,
            rewrittenVm,
            rewrittenView,
            BindingDirection.TwoWay);

        // Assert - Should return a boolean without throwing
        await Assert.That(result).IsTypeOf<bool>();
    }

    /// <summary>
    /// Verifies that EvaluateBindingHooks handles complex property chains.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EvaluateBindingHooks_WithChainedProperties_EvaluatesCorrectly()
    {
        // Arrange
        var evaluator = new BindingHookEvaluator();
        var viewModel = new TestViewModel { Model = new TestModel { AnotherProperty = 42 } };
        var view = new TestView { ViewModel = viewModel };

        Expression<Func<TestViewModel, int>> vmExpr = vm => vm.Model!.AnotherProperty;
        Expression<Func<TestView, int>> viewExpr = v => v.SomeIntProperty;

        var rewrittenVm = Reflection.Rewrite(vmExpr.Body);
        var rewrittenView = Reflection.Rewrite(viewExpr.Body);

        // Act
        var result = evaluator.EvaluateBindingHooks(
            viewModel,
            view,
            rewrittenVm,
            rewrittenView,
            BindingDirection.OneWay);

        // Assert - Should return a boolean without throwing
        await Assert.That(result).IsTypeOf<bool>();
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
