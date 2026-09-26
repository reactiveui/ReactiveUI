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

    /// <summary>When not in a unit test runner, setting <see cref="ViewModelViewHost.ViewContract"/> after construction re-resolves the view with that contract.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task ViewContract_SetAfterConstruction_ReResolvesWithContract()
    {
        using (ForceNonUnitTestMode())
        {
            var locator = new ContractViewLocator();
            var host = new ViewModelViewHost { ViewLocator = locator, ViewModel = new TestViewModel() };

            await Assert.That(host.Content).IsSameReferenceAs(locator.DefaultView);

            host.ViewContract = ContractViewLocator.WideContract;

            using (Assert.Multiple())
            {
                await Assert.That(host.ViewContract).IsEqualTo(ContractViewLocator.WideContract);
                await Assert.That(host.Content).IsSameReferenceAs(locator.WideView);
            }
        }
    }

    /// <summary>When not in a unit test runner, assigning a new <see cref="ViewModelViewHost.ViewContractObservable"/> switches to it and re-resolves on its contracts.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task ViewContractObservable_SetAfterConstruction_FollowsNewObservable()
    {
        using (ForceNonUnitTestMode())
        {
            var locator = new ContractViewLocator();
            var host = new ViewModelViewHost { ViewLocator = locator, ViewModel = new TestViewModel() };
            var contracts = new Signal<string?>();

            host.ViewContractObservable = contracts;
            contracts.OnNext(ContractViewLocator.WideContract);

            using (Assert.Multiple())
            {
                await Assert.That(host.ViewContract).IsEqualTo(ContractViewLocator.WideContract);
                await Assert.That(host.Content).IsSameReferenceAs(locator.WideView);
            }
        }
    }

    /// <summary>Resolving with a <see langword="null"/> <see cref="ViewModelViewHost.ViewLocator"/> falls back to the ambient <see cref="ViewLocator.GetCurrent"/>.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<ViewModelViewHostViewLocatorExecutor>]
    public async Task ResolveViewForViewModel_NullViewLocator_UsesCurrent()
    {
        var host = new TestableViewModelViewHost { ViewModel = new RegisteredViewModel() };

        host.SimulateViewModelChange();

        await Assert.That(host.Content).IsAssignableTo<RegisteredView>();
    }

    /// <summary>The default host finds a view the app added to the view locator with <c>Map</c>.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ResolveViewForViewModel_MappedView_IsFound()
    {
        var locator = new DefaultViewLocator();
        locator.Map<TestViewModel, TestView>();
        var host = new TestableViewModelViewHost { ViewLocator = locator, ViewModel = new TestViewModel() };

        host.SimulateViewModelChange();

        await Assert.That(host.Content).IsTypeOf<TestView>();
    }

    /// <summary>The default host asks the locator's ahead-of-time safe lookup and never its reflective one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ResolveViewForViewModel_DefaultHost_NeverAsksTheUnsafeLookup()
    {
        var locator = new RecordingViewLocator(new TestView());
        var host = new TestableViewModelViewHost { ViewLocator = locator, ViewModel = new TestViewModel() };

        host.SimulateViewModelChange();

        using (Assert.Multiple())
        {
            await Assert.That(locator.SafeLookups).IsGreaterThan(0);
            await Assert.That(locator.UnsafeLookups).IsEqualTo(0);
        }
    }

    /// <summary>The default host does not ask the service locator, so a view registered only there is not found.</summary>
    [Test]
    [TestExecutor<ServiceLocatorOnlyViewExecutor>]
    public void ResolveViewForViewModel_ViewOnlyInTheServiceLocator_Throws()
    {
        var host = new TestableViewModelViewHost { ViewModel = new RegisteredViewModel() };

        _ = Assert.Throws<InvalidOperationException>(host.SimulateViewModelChange);
    }

    /// <summary>The Unsafe host asks the locator's reflective lookup and never its ahead-of-time safe one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ResolveViewForViewModel_UnsafeHost_AsksTheUnsafeLookup()
    {
        var view = new TestView();
        var locator = new RecordingViewLocator(view);
        var host = new TestableViewModelViewHostUnsafe { ViewLocator = locator, ViewModel = new TestViewModel() };

        host.SimulateViewModelChange();

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsSameReferenceAs(view);
            await Assert.That(locator.UnsafeLookups).IsGreaterThan(0);
            await Assert.That(locator.SafeLookups).IsEqualTo(0);
        }
    }

    /// <summary>The Unsafe host finds a view registered only with the service locator.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<ServiceLocatorOnlyViewExecutor>]
    public async Task ResolveViewForViewModel_UnsafeHostViewOnlyInTheServiceLocator_IsFound()
    {
        var host = new TestableViewModelViewHostUnsafe { ViewModel = new RegisteredViewModel() };

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

    /// <summary>Test executor that sets up the MAUI environment and registers a view in <see cref="ViewLocator.GetCurrent"/> for the null-locator fallback test.</summary>
    [NotInParallel]
    public sealed class ViewModelViewHostViewLocatorExecutor : MauiTestExecutor
    {
        /// <summary>The helper that configures and tears down the ReactiveUI app builder.</summary>
        private readonly AppBuilderTestHelper _helper = new();

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            _helper.Initialize(static builder => _ = builder
                .WithMaui()
                .ConfigureViewLocator(static locator => locator.Map<RegisteredViewModel, RegisteredView>())
                .WithCoreServices());
        }

        /// <inheritdoc/>
        protected override void CleanUp()
        {
            _helper.CleanUp();
            base.CleanUp();
        }
    }

    /// <summary>Test executor that sets up the MAUI environment and registers a view only with the service locator.</summary>
    [NotInParallel]
    public sealed class ServiceLocatorOnlyViewExecutor : MauiTestExecutor
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

    /// <summary>A view model that is registered in <see cref="ViewLocator.GetCurrent"/> for the fallback test.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "SST1436:Classes should not be empty", Justification = "Marker type for tests.")]
    private sealed class RegisteredViewModel;

    /// <summary>The view resolved for <see cref="RegisteredViewModel"/> via <see cref="ViewLocator.GetCurrent"/>.</summary>
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
        public IViewFor? ResolveView<TViewModel>(TViewModel viewModel, string? contract)
            where TViewModel : class => _view;

        /// <inheritdoc/>
        public IViewFor? ResolveView(object? viewModel, string? contract) => _view;

        /// <inheritdoc/>
        [RequiresDynamicCode("Resolves a view from an object.")]
        public IViewFor? ResolveViewUnsafe(object? viewModel, string? contract) => ResolveView(viewModel, contract);
    }

    /// <summary>View locator that resolves a separate view for the <see cref="WideContract"/> contract.</summary>
    private sealed class ContractViewLocator : IViewLocator
    {
        /// <summary>The contract that resolves <see cref="WideView"/>.</summary>
        public const string WideContract = "Wide";

        /// <summary>Gets the view resolved for any contract other than <see cref="WideContract"/>.</summary>
        public TestView DefaultView { get; } = new();

        /// <summary>Gets the view resolved for <see cref="WideContract"/>.</summary>
        public TestView WideView { get; } = new();

        /// <inheritdoc/>
        public IViewFor? ResolveView<TViewModel>(TViewModel viewModel, string? contract)
            where TViewModel : class => Resolve(contract);

        /// <inheritdoc/>
        public IViewFor? ResolveView(object? viewModel, string? contract) => Resolve(contract);

        /// <inheritdoc/>
        [RequiresDynamicCode("Resolves a view from an object.")]
        public IViewFor? ResolveViewUnsafe(object? viewModel, string? contract) => ResolveView(viewModel, contract);

        /// <summary>Picks the view for a contract.</summary>
        /// <param name="contract">The contract to resolve.</param>
        /// <returns>The view for the contract.</returns>
        private TestView Resolve(string? contract) => contract == WideContract ? WideView : DefaultView;
    }

    /// <summary>Testable ViewModelViewHost that exposes the protected view model resolution.</summary>
    private sealed class TestableViewModelViewHost : ViewModelViewHost
    {
        /// <summary>Simulates a view model change by resolving the view for the current view model.</summary>
        public void SimulateViewModelChange() => ResolveViewForViewModel(ViewModel, ViewContract);
    }

    /// <summary>Testable ViewModelViewHostUnsafe that exposes the protected view model resolution.</summary>
    [RequiresDynamicCode("Resolves views through the service locator by the view model's runtime type.")]
    private sealed class TestableViewModelViewHostUnsafe : ViewModelViewHostUnsafe
    {
        /// <summary>Simulates a view model change by resolving the view for the current view model.</summary>
        public void SimulateViewModelChange() => ResolveViewForViewModel(ViewModel, ViewContract);
    }

    /// <summary>A view locator that returns one view and counts which of its lookups the host asked.</summary>
    /// <param name="view">The view every lookup returns.</param>
    private sealed class RecordingViewLocator(IViewFor view) : IViewLocator
    {
        /// <summary>Gets the number of ahead-of-time safe lookups by run-time type.</summary>
        public int SafeLookups { get; private set; }

        /// <summary>Gets the number of reflective lookups.</summary>
        public int UnsafeLookups { get; private set; }

        /// <inheritdoc/>
        public IViewFor? ResolveView<TViewModel>(TViewModel viewModel, string? contract)
            where TViewModel : class => view;

        /// <inheritdoc/>
        public IViewFor? ResolveView(object? viewModel, string? contract)
        {
            SafeLookups++;
            return view;
        }

        /// <inheritdoc/>
        [RequiresDynamicCode("Resolves a view from an object.")]
        public IViewFor? ResolveViewUnsafe(object? viewModel, string? contract)
        {
            UnsafeLookups++;
            return view;
        }
    }

    /// <summary>Test view locator for testing.</summary>
    private sealed class TestViewLocator : IViewLocator
    {
        /// <inheritdoc/>
        public IViewFor? ResolveView<TViewModel>(TViewModel viewModel, string? contract)
            where TViewModel : class => null;

        /// <inheritdoc/>
        public IViewFor? ResolveView(object? viewModel, string? contract) => null;

        /// <inheritdoc/>
        [RequiresDynamicCode("Resolves a view from an object.")]
        public IViewFor? ResolveViewUnsafe(object? viewModel, string? contract) => ResolveView(viewModel, contract);
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
        public IViewFor? ResolveView<TViewModel>(TViewModel viewModel, string? contract)
            where TViewModel : class => _view;

        /// <inheritdoc/>
        public IViewFor? ResolveView(object? viewModel, string? contract) => _view;

        /// <inheritdoc/>
        [RequiresDynamicCode("Resolves a view from an object.")]
        public IViewFor? ResolveViewUnsafe(object? viewModel, string? contract) => ResolveView(viewModel, contract);
    }
}
