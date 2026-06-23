// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;
using ReactiveUI.Primitives.Signals;
using TUnit.Core.Executors;

namespace ReactiveUI.Maui.Tests;

/// <summary>Tests for the generic <see cref="ViewModelViewHost{TViewModel}"/>.</summary>
[NotInParallel]
[TestExecutor<MauiTestExecutor>]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Major Code Smell",
    "S4018:Generic methods should provide type parameters",
    Justification = "Test exercises a generic overload with explicit type arguments.")]
public class ViewModelViewHostGenericTests
{
    /// <summary>The contract string used for view contract tests.</summary>
    private const string TestContract = "TestContract";

    /// <summary>Tests that ViewModelProperty is registered for the generic type.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelProperty_IsRegistered() =>
        await Assert.That(ViewModelViewHost<TestViewModel>.ViewModelProperty).IsNotNull();

    /// <summary>Tests that DefaultContentProperty is registered for the generic type.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultContentProperty_IsRegistered() =>
        await Assert.That(ViewModelViewHost<TestViewModel>.DefaultContentProperty).IsNotNull();

    /// <summary>Tests that ViewContractObservableProperty is registered for the generic type.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContractObservableProperty_IsRegistered() =>
        await Assert.That(ViewModelViewHost<TestViewModel>.ViewContractObservableProperty).IsNotNull();

    /// <summary>Tests that ContractFallbackByPassProperty is registered for the generic type.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ContractFallbackByPassProperty_IsRegistered() =>
        await Assert.That(ViewModelViewHost<TestViewModel>.ContractFallbackByPassProperty).IsNotNull();

    /// <summary>Tests that ViewModel property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_CanBeSetAndRetrieved()
    {
        var viewModel = new TestViewModel();
        var host = new ViewModelViewHost<TestViewModel> { ViewModel = viewModel };

        await Assert.That(host.ViewModel).IsSameReferenceAs(viewModel);
    }

    /// <summary>Tests that ViewModel property through IViewFor interface works correctly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IViewFor_ViewModel_CanBeSetAndRetrieved()
    {
        var viewModel = new TestViewModel();
        IViewFor host = new ViewModelViewHost<TestViewModel> { ViewModel = viewModel };

        await Assert.That(host.ViewModel).IsSameReferenceAs(viewModel);
    }

    /// <summary>Tests that DefaultContent can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultContent_CanBeSetAndRetrieved()
    {
        var defaultView = new Label { Text = "Default" };
        var host = new ViewModelViewHost<TestViewModel> { DefaultContent = defaultView };

        await Assert.That(host.DefaultContent).IsSameReferenceAs(defaultView);
    }

    /// <summary>Tests that ViewContract can be set (updates ViewContractObservable).</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContract_CanBeSet()
    {
        // Act - Setting ViewContract should not throw
        var host = new ViewModelViewHost<TestViewModel> { ViewContract = TestContract };

        // Assert - ViewContractObservable should be updated
        await Assert.That(host.ViewContractObservable).IsNotNull();
    }

    /// <summary>Tests that ContractFallbackByPass can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ContractFallbackByPass_CanBeSetAndRetrieved()
    {
        var host = new ViewModelViewHost<TestViewModel> { ContractFallbackByPass = true };

        await Assert.That(host.ContractFallbackByPass).IsTrue();
    }

    /// <summary>Tests that ViewLocator can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewLocator_CanBeSetAndRetrieved()
    {
        var locator = new DefaultViewLocator();
        var host = new ViewModelViewHost<TestViewModel> { ViewLocator = locator };

        await Assert.That(host.ViewLocator).IsSameReferenceAs(locator);
    }

    /// <summary>Tests that DefaultContent getter returns null when not set.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultContent_WhenNotSet_ReturnsNull()
    {
        var host = new ViewModelViewHost<TestViewModel>();

        await Assert.That(host.DefaultContent).IsNull();
    }

    /// <summary>Tests that ViewContract setter updates ViewContractObservable.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContract_WhenSet_UpdatesViewContractObservable()
    {
        var host = new ViewModelViewHost<TestViewModel>();
        var originalObservable = host.ViewContractObservable;

        host.ViewContract = TestContract;

        // ViewContractObservable should be a different instance after setting ViewContract
        await Assert.That(host.ViewContractObservable).IsNotSameReferenceAs(originalObservable);
    }

    /// <summary>Tests that constructor initializes ViewContractObservable in unit test mode.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_InUnitTestMode_InitializesViewContractObservable()
    {
        var host = new ViewModelViewHost<TestViewModel>();

        // In unit test mode, ViewContractObservable should be initialized to a silent observable.
        await Assert.That(host.ViewContractObservable).IsNotNull();
    }

    /// <summary>Tests that multiple ViewModel assignments update the property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_MultipleAssignments_UpdatesProperty()
    {
        var viewModel1 = new TestViewModel { Name = "First" };
        var viewModel2 = new TestViewModel { Name = "Second" };

        var host = new ViewModelViewHost<TestViewModel> { ViewModel = viewModel1 };

        await Assert.That(host.ViewModel).IsSameReferenceAs(viewModel1);

        host.ViewModel = viewModel2;

        await Assert.That(host.ViewModel).IsSameReferenceAs(viewModel2);
    }

    /// <summary>Tests that setting IViewFor.ViewModel to null works.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IViewFor_ViewModel_CanBeSetToNull()
    {
        IViewFor host = new ViewModelViewHost<TestViewModel> { ViewModel = new() };

        host.ViewModel = null;

        await Assert.That(host.ViewModel).IsNull();
        await Assert.That(((ViewModelViewHost<TestViewModel>)host).ViewModel).IsNull();
    }

    /// <summary>Tests that IViewFor.ViewModel setter works correctly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IViewFor_ViewModelSetter_WorksCorrectly()
    {
        var viewModel = new TestViewModel();
        IViewFor host = new ViewModelViewHost<TestViewModel> { ViewModel = viewModel };
        await Assert.That(host.ViewModel).IsSameReferenceAs(viewModel);
        await Assert.That(((ViewModelViewHost<TestViewModel>)host).ViewModel).IsSameReferenceAs(viewModel);
    }

    /// <summary>Tests that ViewContractObservable can be set.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContractObservable_CanBeSet()
    {
        var observable = Signal.Emit(TestContract);
        var host = new ViewModelViewHost<TestViewModel> { ViewContractObservable = observable };

        await Assert.That(host.ViewContractObservable).IsSameReferenceAs(observable);
    }

    /// <summary>Test view model.</summary>
    private sealed class TestViewModel : ReactiveObject
    {
        /// <summary>Gets or sets the name.</summary>
        public string? Name
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }
}
