// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
#if REACTIVE_SHIM
using ReactiveUI.Reactive.Builder;
#else
using ReactiveUI.Builder;
#endif
using ReactiveUI.Tests.Utilities.AppBuilder;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests;

/// <summary>RxVoid tests for <see cref="BindingHookEvaluator"/>.</summary>
/// <remarks>
/// These tests verify hook evaluation logic.
/// Tests use the Executor paradigm to register mock hooks and manage state.
/// </remarks>
[TestExecutor<Executor>]
public class BindingHookEvaluatorTests
{
    /// <summary>Verifies that EvaluateBindingHooks returns true when no rejecting hooks are registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EvaluateBindingHooks_WithNoRejectingHooks_ReturnsTrue()
    {
        // Arrange
        var evaluator = new BindingHookEvaluator();
        var viewModel = new TestViewModel();
        var view = new TestView { ViewModel = viewModel };

        Expression<Func<TestViewModel, int>> viewModelExpression = vm => vm.Property1;
        Expression<Func<TestView, string?>> viewExpr = v => v.SomeStringProperty;

        var rewrittenVm = Reflection.Rewrite(viewModelExpression.Body);
        var rewrittenView = Reflection.Rewrite(viewExpr.Body);

        // Act
        var result = evaluator.EvaluateBindingHooks(
            viewModel,
            view,
            rewrittenVm,
            rewrittenView,
            BindingDirection.OneWay);

        // Assert
        await Assert.That(result).IsTrue();
    }

    /// <summary>Verifies that EvaluateBindingHooks returns false when a hook rejects the binding.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EvaluateBindingHooks_WithRejectingHook_ReturnsFalse()
    {
        // Arrange
        var evaluator = new BindingHookEvaluator();
        var viewModel = new TestViewModel();
        var view = new TestView { ViewModel = viewModel };

        // Use RejectMe property which the hook is configured to reject
        Expression<Func<TestViewModel, int>> viewModelExpression = vm => vm.RejectMe;
        Expression<Func<TestView, string?>> viewExpr = v => v.SomeStringProperty;

        var rewrittenVm = Reflection.Rewrite(viewModelExpression.Body);
        var rewrittenView = Reflection.Rewrite(viewExpr.Body);

        // Act
        var result = evaluator.EvaluateBindingHooks(
            viewModel,
            view,
            rewrittenVm,
            rewrittenView,
            BindingDirection.OneWay);

        // Assert
        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that EvaluateBindingHooks handles null viewModel gracefully.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EvaluateBindingHooks_WithNullViewModel_HandlesGracefully()
    {
        // Arrange
        var evaluator = new BindingHookEvaluator();
        const TestViewModel? viewModel = null;
        var view = new TestView();

        Expression<Func<TestViewModel, int>> viewModelExpression = vm => vm.Property1;
        Expression<Func<TestView, string?>> viewExpr = v => v.SomeStringProperty;

        var rewrittenVm = Reflection.Rewrite(viewModelExpression.Body);
        var rewrittenView = Reflection.Rewrite(viewExpr.Body);

        // Act
        var result = evaluator.EvaluateBindingHooks(
            viewModel,
            view,
            rewrittenVm,
            rewrittenView,
            BindingDirection.OneWay);

        // Assert
        await Assert.That(result).IsTypeOf<bool>();
    }

    /// <summary>Verifies that EvaluateBindingHooks handles complex property chains.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EvaluateBindingHooks_WithChainedProperties_EvaluatesCorrectly()
    {
        // Arrange
        var evaluator = new BindingHookEvaluator();
        var viewModel = new TestViewModel { Model = new() { AnotherProperty = 42 } };
        var view = new TestView { ViewModel = viewModel };

        Expression<Func<TestViewModel, int>> viewModelExpression = vm => vm.Model!.AnotherProperty;
        Expression<Func<TestView, int>> viewExpr = v => v.SomeIntProperty;

        var rewrittenVm = Reflection.Rewrite(viewModelExpression.Body);
        var rewrittenView = Reflection.Rewrite(viewExpr.Body);

        // Act
        var result = evaluator.EvaluateBindingHooks(
            viewModel,
            view,
            rewrittenVm,
            rewrittenView,
            BindingDirection.OneWay);

        // Assert
        await Assert.That(result).IsTrue();
    }

    /// <summary>Verifies that EvaluateBindingHooks returns true when viewModelExpression is null (default behavior).</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EvaluateBindingHooks_WithNullVmExpression_ReturnsTrue()
    {
        // Arrange
        var evaluator = new BindingHookEvaluator();
        var viewModel = new TestViewModel();
        var view = new TestView { ViewModel = viewModel };

        Expression<Func<TestView, string?>> viewExpr = v => v.SomeStringProperty;
        var rewrittenView = Reflection.Rewrite(viewExpr.Body);

        // Act
        var result = evaluator.EvaluateBindingHooks(
            viewModel,
            view,
            null!, // null viewModelExpression
            rewrittenView,
            BindingDirection.OneWay);

        // Assert
        await Assert.That(result).IsTrue();
    }

    /// <summary>Verifies that EvaluateBindingHooks handles TwoWay binding direction.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EvaluateBindingHooks_WithTwoWayBinding_ProcessesCorrectly()
    {
        // Arrange
        var evaluator = new BindingHookEvaluator();
        var viewModel = new TestViewModel();
        var view = new TestView { ViewModel = viewModel };

        Expression<Func<TestViewModel, int>> viewModelExpression = vm => vm.Property1;
        Expression<Func<TestView, string?>> viewExpr = v => v.SomeStringProperty;

        var rewrittenVm = Reflection.Rewrite(viewModelExpression.Body);
        var rewrittenView = Reflection.Rewrite(viewExpr.Body);

        // Act
        var result = evaluator.EvaluateBindingHooks(
            viewModel,
            view,
            rewrittenVm,
            rewrittenView,
            BindingDirection.TwoWay);

        // Assert
        await Assert.That(result).IsTrue();
    }

    /// <summary>Verifies that EvaluateBindingHooks handles AsyncOneWay binding direction.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EvaluateBindingHooks_WithAsyncOneWay_ProcessesCorrectly()
    {
        // Arrange
        var evaluator = new BindingHookEvaluator();
        var viewModel = new TestViewModel();
        var view = new TestView { ViewModel = viewModel };

        Expression<Func<TestViewModel, int>> viewModelExpression = vm => vm.Property1;
        Expression<Func<TestView, int>> viewExpr = v => v.SomeIntProperty;

        var rewrittenVm = Reflection.Rewrite(viewModelExpression.Body);
        var rewrittenView = Reflection.Rewrite(viewExpr.Body);

        // Act
        var result = evaluator.EvaluateBindingHooks(
            viewModel,
            view,
            rewrittenVm,
            rewrittenView,
            BindingDirection.AsyncOneWay);

        // Assert
        await Assert.That(result).IsTrue();
    }

    /// <summary>Test executor for binding hook evaluator tests.</summary>
    public class Executor : BaseAppBuilderTestExecutor
    {
        /// <inheritdoc/>
        protected override void ConfigureAppBuilder(IReactiveUIBuilder builder, TestContext context)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(context);

            _ = builder
                .WithRegistration(r => r.RegisterConstant<IPropertyBindingHook>(new RejectingHook()))
                .WithCoreServices();
        }
    }

    /// <summary>Test binding hook that rejects bindings for the "RejectMe" property.</summary>
    private sealed class RejectingHook : IPropertyBindingHook
    {
        /// <inheritdoc/>
        public bool ExecuteHook(
            object? source,
            object target,
            Func<IObservedChange<object, object>[]> getCurrentViewModelProperties,
            Func<IObservedChange<object, object>[]> getCurrentViewProperties,
            BindingDirection direction)
        {
            var viewModelProperties = getCurrentViewModelProperties();

            // Reject if the property name is "RejectMe"
            return viewModelProperties is null || viewModelProperties.Length == 0 ||
                   viewModelProperties[^1].Expression?.GetMemberInfo()?.Name != "RejectMe";
        }
    }

    /// <summary>Test helper view class.</summary>
    private sealed class TestView : ReactiveObject, IViewFor<TestViewModel>
    {
        /// <summary>Gets or sets the view model.</summary>
        public TestViewModel? ViewModel
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }

        /// <summary>Gets or sets a string property used for binding tests.</summary>
        public string? SomeStringProperty
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets an integer property used for binding tests.</summary>
        public int SomeIntProperty
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    /// <summary>Test helper view model class.</summary>
    private sealed class TestViewModel : ReactiveObject
    {
        /// <summary>Gets or sets the first integer property used for binding tests.</summary>
        public int Property1
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the property that the rejecting hook is configured to reject.</summary>
        public int RejectMe
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the nested model used for chained-property binding tests.</summary>
        public TestModel? Model
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    /// <summary>Test helper model class.</summary>
    private sealed class TestModel : ReactiveObject
    {
        /// <summary>Gets or sets a nested integer property used for chained-property binding tests.</summary>
        public int AnotherProperty
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }
}
