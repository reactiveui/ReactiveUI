// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Builder;
using ReactiveUI.Primitives.Signals;
using ReactiveUI.Tests.Utilities.AppBuilder;
using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.Maui.Tests;

/// <summary>Tests for <see cref="ViewModelViewHost"/>.</summary>
public class ViewModelViewHostTest
{
    /// <summary>Tests that ViewModelProperty is registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelProperty_IsRegistered() =>
        await Assert.That(ViewModelViewHost.ViewModelProperty).IsNotNull();

    /// <summary>Tests that DefaultContentProperty is registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultContentProperty_IsRegistered() =>
        await Assert.That(ViewModelViewHost.DefaultContentProperty).IsNotNull();

    /// <summary>Tests that ViewContractObservableProperty is registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContractObservableProperty_IsRegistered() =>
        await Assert.That(ViewModelViewHost.ViewContractObservableProperty).IsNotNull();

    /// <summary>Tests that ContractFallbackByPassProperty is registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ContractFallbackByPassProperty_IsRegistered() =>
        await Assert.That(ViewModelViewHost.ContractFallbackByPassProperty).IsNotNull();

    /// <summary>Tests that ViewModel property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetAndGet_WorksCorrectly()
    {
        var host = new ViewModelViewHost();
        var viewModel = new TestViewModel();

        host.ViewModel = viewModel;

        await Assert.That(host.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>Tests that DefaultContent property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultContent_SetAndGet_WorksCorrectly()
    {
        var host = new ViewModelViewHost();
        var defaultContent = new Label { Text = "Default" };

        host.DefaultContent = defaultContent;

        await Assert.That(host.DefaultContent).IsEqualTo(defaultContent);
    }

    /// <summary>Tests that ContractFallbackByPass property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ContractFallbackByPass_SetAndGet_WorksCorrectly()
    {
        var host = new ViewModelViewHost { ContractFallbackByPass = true };

        await Assert.That(host.ContractFallbackByPass).IsTrue();

        host.ContractFallbackByPass = false;

        await Assert.That(host.ContractFallbackByPass).IsFalse();
    }

    /// <summary>Tests that ViewLocator property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewLocator_SetAndGet_WorksCorrectly()
    {
        var host = new ViewModelViewHost();
        var viewLocator = new TestViewLocator();

        host.ViewLocator = viewLocator;

        await Assert.That(host.ViewLocator).IsEqualTo(viewLocator);
    }

    /// <summary>Tests that ViewContractObservable property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContractObservable_SetAndGet_WorksCorrectly()
    {
        var host = new ViewModelViewHost();
        var observable = Signal.Emit("contract");

        host.ViewContractObservable = observable;

        await Assert.That(host.ViewContractObservable).IsEqualTo(observable);
    }

    /// <summary>Tests that ResolveViewForViewModel resolves the view and sets the content.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ResolveViewForViewModel_ResolvesAndSetsContent()
    {
        var host = new TestableViewModelViewHost();
        var viewModel = new TestViewModel();
        var view = new TestView();
        host.ViewLocator = new MockViewLocator(view);
        host.ViewModel = viewModel;
        host.SimulateViewModelChange();

        await Assert.That(host.Content).IsEqualTo(view);
        await Assert.That(view.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>Tests that DefaultContent is shown when ViewModel is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultContent_IsShown_WhenViewModelIsNull()
    {
        var host = new TestableViewModelViewHost();
        var defaultContent = new Label();
        host.DefaultContent = defaultContent;

        // Trigger update
        host.ViewModel = new TestViewModel(); // First set to something
        host.ViewModel = null; // Then set to null
        host.SimulateViewModelChange();

        await Assert.That(host.Content).IsEqualTo(defaultContent);
    }

    /// <summary>Resolving a view model with no registered view throws.</summary>
    [Test]
    public void ResolveViewForViewModel_NoViewFound_Throws()
    {
        var host = new TestableViewModelViewHost { ViewLocator = new TestViewLocator(), ViewModel = new TestViewModel() };

        _ = Assert.Throws<InvalidOperationException>(host.SimulateViewModelChange);
    }

    /// <summary>Resolving a view model to a non-View instance throws.</summary>
    [Test]
    public void ResolveViewForViewModel_NonViewInstance_Throws()
    {
        var host = new TestableViewModelViewHost { ViewLocator = new NonViewLocator(), ViewModel = new TestViewModel() };

        _ = Assert.Throws<InvalidOperationException>(host.SimulateViewModelChange);
    }

    /// <summary>When not in a unit test runner, the constructor wires the contract subscription and resolves the (null) view model to the default content.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task Constructor_NotInUnitTestRunner_WiresSubscriptionAndResolves()
    {
        using (ForceNonUnitTestMode())
        {
            // With no view model the constructor's subscription resolves to DefaultContent without throwing.
            var host = new ViewModelViewHost();

            await Assert.That(host.ViewContractObservable).IsNotNull();
        }
    }

    /// <summary>When not in a unit test runner, changing the view model re-resolves and sets the content.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task OnViewModelPropertyChanged_NotInUnitTestRunner_ResolvesView()
    {
        using (ForceNonUnitTestMode())
        {
            var view = new TestView();
            var host = new ViewModelViewHost { ViewLocator = new MockViewLocator(view) };

            var viewModel = new TestViewModel();

            // Setting ViewModel triggers OnViewModelPropertyChanged, which (outside a unit test runner) resolves the view.
            host.ViewModel = viewModel;

            using (Assert.Multiple())
            {
                await Assert.That(host.Content).IsEqualTo(view);
                await Assert.That(view.ViewModel).IsEqualTo(viewModel);
            }
        }
    }

    /// <summary>Resolving with a <see langword="null"/> <see cref="ViewModelViewHost.ViewLocator"/> falls back to the ambient <see cref="ViewLocator.Current"/>.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<ViewModelViewHostViewLocatorExecutor>]
    public async Task ResolveViewForViewModel_NullViewLocator_UsesCurrent()
    {
        var host = new TestableViewModelViewHost { ViewModel = new RegisteredViewModel() };

        host.SimulateViewModelChange();

        await Assert.That(host.Content).IsAssignableTo<RegisteredView>();
    }

    /// <summary>Temporarily overrides the mode detector so the code believes it is not running in a unit test.</summary>
    /// <returns>A disposable that restores the previous mode detector when disposed.</returns>
    private static ActionDisposable ForceNonUnitTestMode()
    {
        ModeDetector.OverrideModeDetector(new AlwaysFalseModeDetector());
        return new(static () => ModeDetector.OverrideModeDetector(new DefaultModeDetector()));
    }

    /// <summary>Test executor that sets up the MAUI environment and registers a view in <see cref="ViewLocator.Current"/> for the null-locator fallback test.</summary>
    [NotInParallel]
    public sealed class ViewModelViewHostViewLocatorExecutor : MauiTestExecutor
    {
        /// <summary>The helper that configures and tears down the ReactiveUI app builder.</summary>
        private readonly AppBuilderTestHelper _helper = new();

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            _helper.Initialize(static builder =>
            {
                _ = builder.WithMaui().WithCoreServices();
                AppLocator.CurrentMutable.Register<IViewFor<RegisteredViewModel>>(static () => new RegisteredView());
            });
        }

        /// <inheritdoc/>
        protected override void CleanUp()
        {
            _helper.CleanUp();
            base.CleanUp();
        }
    }

    /// <summary>Mode detector implementation that always reports it is not running in a unit test runner.</summary>
    private sealed class AlwaysFalseModeDetector : IModeDetector
    {
        /// <summary>Indicates whether the code is running in a unit test runner.</summary>
        /// <returns>Always returns <see langword="false"/>.</returns>
        public bool? InUnitTestRunner() => false;
    }

    /// <summary>A view model that is registered in <see cref="ViewLocator.Current"/> for the fallback test.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "SST1436:Classes should not be empty", Justification = "Marker type for tests.")]
    private sealed class RegisteredViewModel;

    /// <summary>The view resolved for <see cref="RegisteredViewModel"/> via <see cref="ViewLocator.Current"/>.</summary>
    private sealed class RegisteredView : ContentView, IViewFor<RegisteredViewModel>
    {
        /// <inheritdoc/>
        public RegisteredViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (RegisteredViewModel?)value;
        }
    }

    /// <summary>Test view model for testing.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "SST1436:Classes should not be empty", Justification = "Marker type for tests.")]
    private sealed class TestViewModel;

    /// <summary>Test view that implements IViewFor for testing.</summary>
    private sealed class TestView : ContentView, IViewFor<TestViewModel>
    {
        /// <inheritdoc/>
        public TestViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    /// <summary>Mock view locator that always resolves to a fixed view for testing.</summary>
    private sealed class MockViewLocator : IViewLocator
    {
        /// <summary>The view to always resolve to.</summary>
        private readonly IViewFor _view;

        /// <summary>Initializes a new instance of the <see cref="MockViewLocator"/> class.</summary>
        /// <param name="view">The view to always resolve to.</param>
        public MockViewLocator(IViewFor view) => _view = view;

        /// <inheritdoc/>
        public IViewFor<T>? ResolveView<T>(string? contract)
            where T : class => _view as IViewFor<T>;

        /// <inheritdoc/>
        public IViewFor<T>? ResolveView<T>()
            where T : class => _view as IViewFor<T>;

        /// <inheritdoc/>
        [RequiresUnreferencedCode(
            "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
        [RequiresDynamicCode(
            "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, " +
            "or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
        public IViewFor? ResolveView(object? instance, string? contract) => _view;

        /// <inheritdoc/>
        [RequiresUnreferencedCode(
            "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
        [RequiresDynamicCode(
            "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, " +
            "or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
        public IViewFor? ResolveView(object? instance) => _view;
    }

    /// <summary>Testable ViewModelViewHost that exposes the protected view model resolution.</summary>
    private sealed class TestableViewModelViewHost : ViewModelViewHost
    {
        /// <summary>Simulates a view model change by resolving the view for the current view model.</summary>
        public void SimulateViewModelChange() => ResolveViewForViewModel(ViewModel, ViewContract);
    }

    /// <summary>Test view locator for testing.</summary>
    private sealed class TestViewLocator : IViewLocator
    {
        /// <inheritdoc/>
        public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract)
            where TViewModel : class => null;

        /// <inheritdoc/>
        public IViewFor<TViewModel>? ResolveView<TViewModel>()
            where TViewModel : class => null;

        /// <inheritdoc/>
        [RequiresUnreferencedCode(
            "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
        [RequiresDynamicCode(
            "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, " +
            "or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
        public IViewFor? ResolveView(object? instance, string? contract) => null;

        /// <inheritdoc/>
        [RequiresUnreferencedCode(
            "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
        [RequiresDynamicCode(
            "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, " +
            "or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
        public IViewFor? ResolveView(object? instance) => null;
    }

    /// <summary>A view that implements <see cref="IViewFor"/> but is not a MAUI <see cref="View"/>.</summary>
    private sealed class NonView : IViewFor<TestViewModel>
    {
        /// <inheritdoc/>
        public TestViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    /// <summary>A view locator that resolves to a non-View instance.</summary>
    private sealed class NonViewLocator : IViewLocator
    {
        /// <summary>The non-View instance to resolve to.</summary>
        private readonly NonView _view = new();

        /// <inheritdoc/>
        public IViewFor<T>? ResolveView<T>(string? contract)
            where T : class => _view as IViewFor<T>;

        /// <inheritdoc/>
        public IViewFor<T>? ResolveView<T>()
            where T : class => _view as IViewFor<T>;

        /// <inheritdoc/>
        [RequiresUnreferencedCode(
            "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
        [RequiresDynamicCode(
            "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, " +
            "or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
        public IViewFor? ResolveView(object? instance, string? contract) => _view;

        /// <inheritdoc/>
        [RequiresUnreferencedCode(
            "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
        [RequiresDynamicCode(
            "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, " +
            "or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
        public IViewFor? ResolveView(object? instance) => _view;
    }
}
