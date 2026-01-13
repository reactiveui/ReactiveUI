// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;

namespace ReactiveUI.Tests.Locator;

/// <summary>
///     Comprehensive test suite for <see cref="DefaultViewLocator" />.
///     Tests cover mapping, unmapping, resolution with contracts, AOT compatibility, and thread safety.
///     Uses <see cref="AppBuilderTestExecutor" /> to ensure proper AppLocator isolation between tests.
/// </summary>
[NotInParallel]
[TestExecutor<AppBuilderTestExecutor>]
public class DefaultViewLocatorTests
{
    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.Map{TViewModel, TView}" /> returns the locator instance
    ///     to support fluent API chaining.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Map_AllowsChaining()
    {
        var locator = new DefaultViewLocator();

        var result = locator
            .Map<TestViewModel, TestView>(() => new TestView())
            .Map<TestViewModel2, TestView2>(() => new TestView2());

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsTypeOf<DefaultViewLocator>();
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.Map{TViewModel, TView}" /> throws
    ///     <see cref="ArgumentNullException" /> when factory parameter is null.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Map_NullFactory_ThrowsArgumentNullException()
    {
        var locator = new DefaultViewLocator();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            locator.Map<TestViewModel, TestView>(null!);
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Verifies that calling <see cref="DefaultViewLocator.Map{TViewModel, TView}" /> multiple times
    ///     for the same view model and contract overwrites the previous mapping.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Map_OverwritesExistingMapping()
    {
        var locator = new DefaultViewLocator();
        var callCount = 0;

        locator
            .Map<TestViewModel, TestView>(() =>
            {
                callCount++;
                return new TestView();
            })
            .Map<TestViewModel, TestView>(() =>
            {
                callCount += 10;
                return new TestView();
            });

        locator.ResolveView<TestViewModel>();

        await Assert.That(callCount).IsEqualTo(10);
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.Map{TViewModel, TView}" /> registers a view factory
    ///     that can be resolved via <see cref="DefaultViewLocator.ResolveView{TViewModel}" />.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Map_RegistersViewFactory()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new TestView());

        var view = locator.ResolveView<TestViewModel>();

        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<TestView>();
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.Map{TViewModel, TView}" /> is thread-safe
    ///     and does not throw when called concurrently from multiple threads.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Map_ThreadSafe_ConcurrentMapsDontThrow()
    {
        var locator = new DefaultViewLocator();
        var tasks = new List<Task>();

        for (var i = 0; i < 100; i++)
        {
            var contract = $"contract{i}";
            tasks.Add(Task.Run(() => { locator.Map<TestViewModel, TestView>(() => new TestView(), contract); }));
        }

        await Task.WhenAll(tasks);

        for (var i = 0; i < 100; i++)
        {
            var view = locator.ResolveView<TestViewModel>($"contract{i}");
            await Assert.That(view).IsNotNull();
        }
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.Map{TViewModel, TView}" /> with contract parameter
    ///     registers contract-specific views that can be resolved separately.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Map_WithContract_RegistersContractSpecificView()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new TestView(), "mobile")
            .Map<TestViewModel, TestViewAlt>(() => new TestViewAlt(), "desktop");

        var mobileView = locator.ResolveView<TestViewModel>("mobile");
        var desktopView = locator.ResolveView<TestViewModel>("desktop");

        await Assert.That(mobileView).IsTypeOf<TestView>();
        await Assert.That(desktopView).IsTypeOf<TestViewAlt>();
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.ResolveView{TViewModel}" /> creates a new view instance
    ///     on each call using the registered factory.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Generic_CreatesNewInstanceOnEachCall()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new TestView());

        var view1 = locator.ResolveView<TestViewModel>();
        var view2 = locator.ResolveView<TestViewModel>();

        await Assert.That(view1).IsNotNull();
        await Assert.That(view2).IsNotNull();
        await Assert.That(ReferenceEquals(view1, view2)).IsFalse();
    }

    /// <summary>
    ///     Verifies that explicit mappings registered via <see cref="DefaultViewLocator.Map{TViewModel, TView}" />
    ///     take priority over service locator registrations.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Generic_ExplicitMappingTakesPriorityOverServiceLocator()
    {
        var resolver = AppLocator.Current as IDependencyResolver;
        ArgumentNullException.ThrowIfNull(resolver);

        resolver.Register(() => new TestViewAlt(), typeof(IViewFor<TestViewModel>));

        try
        {
            var locator = new DefaultViewLocator();
            locator.Map<TestViewModel, TestView>(() => new TestView());

            var view = locator.ResolveView<TestViewModel>();

            await Assert.That(view).IsTypeOf<TestView>();
        }
        finally
        {
            // Clean up registration
            resolver.UnregisterCurrent(typeof(IViewFor<TestViewModel>));
        }
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.ResolveView{TViewModel}" /> falls back
    ///     to querying the service locator when no explicit mapping exists.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Generic_FallsBackToServiceLocator()
    {
        var resolver = AppLocator.Current as IDependencyResolver;
        ArgumentNullException.ThrowIfNull(resolver);

        resolver.Register(() => new TestView(), typeof(IViewFor<TestViewModel>));

        try
        {
            var locator = new DefaultViewLocator();
            var view = locator.ResolveView<TestViewModel>();

            await Assert.That(view).IsNotNull();
            await Assert.That(view).IsTypeOf<TestView>();
        }
        finally
        {
            // Clean up registration
            resolver.UnregisterCurrent(typeof(IViewFor<TestViewModel>));
        }
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.ResolveView{TViewModel}" /> returns null
    ///     when no mapping or service registration exists for the view model type.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Generic_ReturnsNullWhenNoMapping()
    {
        var locator = new DefaultViewLocator();

        var view = locator.ResolveView<TestViewModel>();

        await Assert.That(view).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.ResolveView{TViewModel}" /> with a contract
    ///     falls back to the default mapping when the specific contract is not found.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Generic_WithContract_FallsBackToDefaultMappingWhenContractNotFound()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new TestView());

        var view = locator.ResolveView<TestViewModel>("unknown");

        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<TestView>();
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.ResolveView{TViewModel}" /> with a contract
    ///     falls back to the default service locator registration when the specific contract is not found.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Generic_WithContract_FallsBackToDefaultServiceLocatorWhenContractNotFound()
    {
        var resolver = AppLocator.Current as IDependencyResolver;
        ArgumentNullException.ThrowIfNull(resolver);

        resolver.Register(() => new TestView(), typeof(IViewFor<TestViewModel>));

        try
        {
            var locator = new DefaultViewLocator();
            var view = locator.ResolveView<TestViewModel>("unknown");

            await Assert.That(view).IsNotNull();
            await Assert.That(view).IsTypeOf<TestView>();
        }
        finally
        {
            // Clean up registration
            resolver.UnregisterCurrent(typeof(IViewFor<TestViewModel>));
        }
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.ResolveView{TViewModel}" /> with contract
    ///     uses the explicit mapping registered for that contract.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Generic_WithContract_UsesExplicitMapping()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new TestView(), "mobile");

        var view = locator.ResolveView<TestViewModel>("mobile");

        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<TestView>();
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.ResolveView(object, string)" /> falls back
    ///     to the service locator and sets the view model property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Instance_FallsBackToServiceLocator()
    {
        var resolver = AppLocator.Current as IDependencyResolver;
        ArgumentNullException.ThrowIfNull(resolver);

        resolver.Register(() => new TestView(), typeof(IViewFor<TestViewModel>));

        try
        {
            var locator = new DefaultViewLocator();
            var vm = new TestViewModel();
            var view = locator.ResolveView(vm);

            await Assert.That(view).IsNotNull();
            await Assert.That(view).IsTypeOf<TestView>();
            await Assert.That(view!.ViewModel).IsEqualTo(vm);
        }
        finally
        {
            // Clean up registration
            resolver.UnregisterCurrent(typeof(IViewFor<TestViewModel>));
        }
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.ResolveView(object, string)" /> returns null
    ///     when the instance parameter is null.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Instance_ReturnsNullForNullInstance()
    {
        var locator = new DefaultViewLocator();

        var view = locator.ResolveView(null);

        await Assert.That(view).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.ResolveView(object, string)" /> returns null
    ///     when no mapping or service registration exists for the view model type.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Instance_ReturnsNullWhenNoMappingOrService()
    {
        var locator = new DefaultViewLocator();
        var vm = new TestViewModel();

        var view = locator.ResolveView(vm);

        await Assert.That(view).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.ResolveView(object, string)" /> sets
    ///     the <see cref="IViewFor{T}.ViewModel" /> property on the resolved view instance.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Instance_SetsViewModelProperty()
    {
        var locator = new DefaultViewLocator();
        var vm = new TestViewModel();

        locator.Map<TestViewModel, TestView>(() => new TestView());

        var view = locator.ResolveView(vm);

        await Assert.That(view).IsNotNull();
        await Assert.That(view!.ViewModel).IsEqualTo(vm);
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.ResolveView(object, string)" /> uses
    ///     explicit mappings registered via <see cref="DefaultViewLocator.Map{TViewModel, TView}" />.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Instance_UsesExplicitMapping()
    {
        var locator = new DefaultViewLocator();
        var vm = new TestViewModel();

        locator.Map<TestViewModel, TestView>(() => new TestView());

        var view = locator.ResolveView(vm);

        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<TestView>();
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.ResolveView(object, string)" /> with a contract
    ///     falls back to the default mapping when the specific contract is not found.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Instance_WithContract_FallsBackToDefaultMapping()
    {
        var locator = new DefaultViewLocator();
        var vm = new TestViewModel();

        locator.Map<TestViewModel, TestView>(() => new TestView());

        var view = locator.ResolveView(vm, "unknown");

        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<TestView>();
        await Assert.That(view!.ViewModel).IsEqualTo(vm);
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.ResolveView(object, string)" /> with contract
    ///     uses the contract-specific mapping and sets the view model property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Instance_WithContract_UsesContractMapping()
    {
        var locator = new DefaultViewLocator();
        var vm = new TestViewModel();

        locator.Map<TestViewModel, TestView>(() => new TestView(), "mobile");

        var view = locator.ResolveView(vm, "mobile");

        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<TestView>();
        await Assert.That(view!.ViewModel).IsEqualTo(vm);
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.ResolveView{TViewModel}" /> is thread-safe
    ///     and does not throw when called concurrently from multiple threads.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_ThreadSafe_ConcurrentResolvesDontThrow()
    {
        var locator = new DefaultViewLocator();
        locator.Map<TestViewModel, TestView>(() => new TestView());

        var tasks = new List<Task>();
        for (var i = 0; i < 100; i++)
        {
            tasks.Add(
                Task.Run(async () =>
                {
                    var view = locator.ResolveView<TestViewModel>();
                    await Assert.That(view).IsNotNull();
                }));
        }

        await Task.WhenAll(tasks);
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.Unmap{TViewModel}" /> returns the locator instance
    ///     to support fluent API chaining.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Unmap_AllowsChaining()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new TestView(), "c1")
            .Map<TestViewModel, TestView>(() => new TestView(), "c2");

        var result = locator.Unmap<TestViewModel>("c1")
            .Unmap<TestViewModel>("c2");

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsTypeOf<DefaultViewLocator>();
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.Unmap{TViewModel}" /> does not throw when
    ///     called for a contract that was never registered.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Unmap_NonExistentMapping_DoesNotThrow()
    {
        var locator = new DefaultViewLocator();

        locator.Unmap<TestViewModel>("nonexistent");

        await Assert.That(locator.ResolveView<TestViewModel>("nonexistent")).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.Unmap{TViewModel}" /> removes the default mapping
    ///     when called without a contract parameter.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Unmap_RemovesDefaultMapping()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new TestView());

        await Assert.That(locator.ResolveView<TestViewModel>()).IsNotNull();

        locator.Unmap<TestViewModel>();

        await Assert.That(locator.ResolveView<TestViewModel>()).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.Unmap{TViewModel}" /> removes a previously
    ///     registered mapping for a specific contract.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Unmap_RemovesMappingForContract()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new TestView(), "mobile");

        await Assert.That(locator.ResolveView<TestViewModel>("mobile")).IsNotNull();

        locator.Unmap<TestViewModel>("mobile");

        await Assert.That(locator.ResolveView<TestViewModel>("mobile")).IsNull();
    }

    /// <summary>
    ///     Verifies that <see cref="DefaultViewLocator.Unmap{TViewModel}" /> is thread-safe
    ///     and does not throw when called concurrently from multiple threads.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Unmap_ThreadSafe_ConcurrentUnmapsDontThrow()
    {
        var locator = new DefaultViewLocator();

        for (var i = 0; i < 100; i++)
        {
            locator.Map<TestViewModel, TestView>(() => new TestView(), $"contract{i}");
        }

        var tasks = new List<Task>();
        for (var i = 0; i < 100; i++)
        {
            var contract = $"contract{i}";
            tasks.Add(Task.Run(() => { locator.Unmap<TestViewModel>(contract); }));
        }

        await Task.WhenAll(tasks);

        for (var i = 0; i < 100; i++)
        {
            var view = locator.ResolveView<TestViewModel>($"contract{i}");
            await Assert.That(view).IsNull();
        }
    }

    /// <summary>
    ///     Test view implementing <see cref="IViewFor{TViewModel}" /> for <see cref="TestViewModel" />.
    /// </summary>
    private sealed class TestView : IViewFor<TestViewModel>
    {
        /// <summary>
        ///     Gets or sets the strongly-typed view model.
        /// </summary>
        public TestViewModel? ViewModel { get; set; }

        /// <summary>
        ///     Gets or sets the view model. Implements <see cref="IViewFor.ViewModel" />.
        /// </summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    /// <summary>
    ///     Test view implementing <see cref="IViewFor{TViewModel}" /> for <see cref="TestViewModel2" />.
    /// </summary>
    private sealed class TestView2 : IViewFor<TestViewModel2>
    {
        /// <summary>
        ///     Gets or sets the strongly-typed view model.
        /// </summary>
        public TestViewModel2? ViewModel { get; set; }

        /// <summary>
        ///     Gets or sets the view model. Implements <see cref="IViewFor.ViewModel" />.
        /// </summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel2?)value;
        }
    }

    /// <summary>
    ///     Alternative test view for <see cref="TestViewModel" />, used to test contract-specific mappings.
    /// </summary>
    private sealed class TestViewAlt : IViewFor<TestViewModel>
    {
        /// <summary>
        ///     Gets or sets the strongly-typed view model.
        /// </summary>
        public TestViewModel? ViewModel { get; set; }

        /// <summary>
        ///     Gets or sets the view model. Implements <see cref="IViewFor.ViewModel" />.
        /// </summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    /// <summary>
    ///     Test view model used for testing view locator functionality.
    /// </summary>
    private sealed class TestViewModel : ReactiveObject
    {
    }

    /// <summary>
    ///     Second test view model used for testing multi-mapping scenarios.
    /// </summary>
    private sealed class TestViewModel2 : ReactiveObject
    {
    }
}
