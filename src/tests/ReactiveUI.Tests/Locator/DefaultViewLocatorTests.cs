// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Tests.Utilities.AppBuilder;
using Splat;
using TUnit.Core.Executors;

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
    /// <summary>The number of iterations used in concurrency/thread-safety tests.</summary>
    private const int ConcurrentIterations = 100;

    /// <summary>The contract name used to resolve mobile-specific view registrations.</summary>
    private const string MobileContract = "mobile";

    /// <summary>Verifies that <c>Map</c> returns the locator instance to support fluent API chaining.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Map_AllowsChaining()
    {
        var locator = new DefaultViewLocator();

        var result = locator
            .Map<TestViewModel, TestView>(() => new())
            .Map<TestViewModel2, TestView2>(() => new());

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsTypeOf<DefaultViewLocator>();
    }

    /// <summary>Verifies that <c>Map</c> throws <see cref="ArgumentNullException" /> when factory parameter is null.</summary>
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

    /// <summary>Verifies that calling <c>Map</c> multiple times for the same view model and contract overwrites the previous mapping.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Map_OverwritesExistingMapping()
    {
        const int OverwriteIncrement = 10;
        var locator = new DefaultViewLocator();
        var callCount = 0;

        locator
            .Map<TestViewModel, TestView>(() =>
            {
                callCount++;
                return new();
            })
            .Map<TestViewModel, TestView>(() =>
            {
                callCount += OverwriteIncrement;
                return new();
            });

        locator.ResolveView<TestViewModel>();

        await Assert.That(callCount).IsEqualTo(OverwriteIncrement);
    }

    /// <summary>Verifies that <c>Map</c> registers a view factory that can be resolved via <c>ResolveView</c>.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Map_RegistersViewFactory()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new());

        var view = locator.ResolveView<TestViewModel>();

        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<TestView>();
    }

    /// <summary>Verifies that <c>Map</c> is thread-safe and does not throw when called concurrently from multiple threads.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Map_ThreadSafe_ConcurrentMapsDontThrow()
    {
        var locator = new DefaultViewLocator();
        var tasks = new List<Task>();

        for (var i = 0; i < ConcurrentIterations; i++)
        {
            var contract = $"contract{i}";
            tasks.Add(Task.Run(() => locator.Map<TestViewModel, TestView>(() => new(), contract)));
        }

        await Task.WhenAll(tasks);

        for (var i = 0; i < ConcurrentIterations; i++)
        {
            var view = locator.ResolveView<TestViewModel>($"contract{i}");
            await Assert.That(view).IsNotNull();
        }
    }

    /// <summary>Verifies that <c>Map</c> with contract parameter registers contract-specific views that can be resolved separately.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Map_WithContract_RegistersContractSpecificView()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new(), MobileContract)
            .Map<TestViewModel, TestViewAlt>(() => new(), "desktop");

        var mobileView = locator.ResolveView<TestViewModel>(MobileContract);
        var desktopView = locator.ResolveView<TestViewModel>("desktop");

        await Assert.That(mobileView).IsTypeOf<TestView>();
        await Assert.That(desktopView).IsTypeOf<TestViewAlt>();
    }

    /// <summary>Verifies that <c>ResolveView</c> creates a new view instance on each call using the registered factory.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Generic_CreatesNewInstanceOnEachCall()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new());

        var view1 = locator.ResolveView<TestViewModel>();
        var view2 = locator.ResolveView<TestViewModel>();

        await Assert.That(view1).IsNotNull();
        await Assert.That(view2).IsNotNull();
        await Assert.That(ReferenceEquals(view1, view2)).IsFalse();
    }

    /// <summary>Verifies that explicit mappings registered via <c>Map</c> take priority over service locator registrations.</summary>
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
            locator.Map<TestViewModel, TestView>(() => new());

            var view = locator.ResolveView<TestViewModel>();

            await Assert.That(view).IsTypeOf<TestView>();
        }
        finally
        {
            // Clean up registration
            resolver.UnregisterCurrent<IViewFor<TestViewModel>>();
        }
    }

    /// <summary>Verifies that <c>ResolveView</c> falls back to querying the service locator when no explicit mapping exists.</summary>
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
            resolver.UnregisterCurrent<IViewFor<TestViewModel>>();
        }
    }

    /// <summary>Verifies that <c>ResolveView</c> returns null when no mapping or service registration exists for the view model type.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Generic_ReturnsNullWhenNoMapping()
    {
        var locator = new DefaultViewLocator();

        var view = locator.ResolveView<TestViewModel>();

        await Assert.That(view).IsNull();
    }

    /// <summary>Verifies that <c>ResolveView</c> with contract uses the explicit mapping registered for that contract.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Generic_WithContract_UsesExplicitMapping()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new(), MobileContract);

        var view = locator.ResolveView<TestViewModel>(MobileContract);

        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<TestView>();
    }

    /// <summary>Verifies that <see cref="DefaultViewLocator.ResolveView(object, string)" /> falls back to the service locator and sets the view model property.</summary>
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
            resolver.UnregisterCurrent<IViewFor<TestViewModel>>();
        }
    }

    /// <summary>Verifies that <see cref="DefaultViewLocator.ResolveView(object, string)" /> returns null when the instance parameter is null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Instance_ReturnsNullForNullInstance()
    {
        var locator = new DefaultViewLocator();

        var view = locator.ResolveView(null);

        await Assert.That(view).IsNull();
    }

    /// <summary>Verifies that <see cref="DefaultViewLocator.ResolveView(object, string)" /> returns null when no mapping or service registration exists for the view model type.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Instance_ReturnsNullWhenNoMappingOrService()
    {
        var locator = new DefaultViewLocator();
        var vm = new TestViewModel();

        var view = locator.ResolveView(vm);

        await Assert.That(view).IsNull();
    }

    /// <summary>Verifies that <see cref="DefaultViewLocator.ResolveView(object, string)" /> sets the <see cref="IViewFor{T}.ViewModel" /> property on the resolved view instance.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Instance_SetsViewModelProperty()
    {
        var locator = new DefaultViewLocator();
        var vm = new TestViewModel();

        locator.Map<TestViewModel, TestView>(() => new());

        var view = locator.ResolveView(vm);

        await Assert.That(view).IsNotNull();
        await Assert.That(view!.ViewModel).IsEqualTo(vm);
    }

    /// <summary>Verifies that <see cref="DefaultViewLocator.ResolveView(object, string)" /> uses explicit mappings registered via <c>Map</c>.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Instance_UsesExplicitMapping()
    {
        var locator = new DefaultViewLocator();
        var vm = new TestViewModel();

        locator.Map<TestViewModel, TestView>(() => new());

        var view = locator.ResolveView(vm);

        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<TestView>();
    }

    /// <summary>Verifies that <see cref="DefaultViewLocator.ResolveView(object, string)" /> with contract uses the contract-specific mapping and sets the view model property.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_Instance_WithContract_UsesContractMapping()
    {
        var locator = new DefaultViewLocator();
        var vm = new TestViewModel();

        locator.Map<TestViewModel, TestView>(() => new(), MobileContract);

        var view = locator.ResolveView(vm, MobileContract);

        await Assert.That(view).IsNotNull();
        await Assert.That(view).IsTypeOf<TestView>();
        await Assert.That(view!.ViewModel).IsEqualTo(vm);
    }

    /// <summary>Verifies that <c>ResolveView</c> is thread-safe and does not throw when called concurrently from multiple threads.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveView_ThreadSafe_ConcurrentResolvesDontThrow()
    {
        var locator = new DefaultViewLocator();
        locator.Map<TestViewModel, TestView>(() => new());

        var tasks = new List<Task>();
        for (var i = 0; i < ConcurrentIterations; i++)
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

    /// <summary>Verifies that <c>Unmap</c> returns the locator instance to support fluent API chaining.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Unmap_AllowsChaining()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new(), "c1")
            .Map<TestViewModel, TestView>(() => new(), "c2");

        var result = locator.Unmap<TestViewModel>("c1")
            .Unmap<TestViewModel>("c2");

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsTypeOf<DefaultViewLocator>();
    }

    /// <summary>Verifies that <c>Unmap</c> does not throw when called for a contract that was never registered.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Unmap_NonExistentMapping_DoesNotThrow()
    {
        var locator = new DefaultViewLocator();

        locator.Unmap<TestViewModel>("nonexistent");

        await Assert.That(locator.ResolveView<TestViewModel>("nonexistent")).IsNull();
    }

    /// <summary>Verifies that <c>Unmap</c> removes the default mapping when called without a contract parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Unmap_RemovesDefaultMapping()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new());

        await Assert.That(locator.ResolveView<TestViewModel>()).IsNotNull();

        locator.Unmap<TestViewModel>();

        await Assert.That(locator.ResolveView<TestViewModel>()).IsNull();
    }

    /// <summary>Verifies that <c>Unmap</c> removes a previously registered mapping for a specific contract.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Unmap_RemovesMappingForContract()
    {
        var locator = new DefaultViewLocator();

        locator.Map<TestViewModel, TestView>(() => new(), MobileContract);

        await Assert.That(locator.ResolveView<TestViewModel>(MobileContract)).IsNotNull();

        locator.Unmap<TestViewModel>(MobileContract);

        await Assert.That(locator.ResolveView<TestViewModel>(MobileContract)).IsNull();
    }

    /// <summary>Verifies that <c>Unmap</c> is thread-safe and does not throw when called concurrently from multiple threads.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Unmap_ThreadSafe_ConcurrentUnmapsDontThrow()
    {
        var locator = new DefaultViewLocator();

        for (var i = 0; i < ConcurrentIterations; i++)
        {
            locator.Map<TestViewModel, TestView>(() => new(), $"contract{i}");
        }

        var tasks = new List<Task>();
        for (var i = 0; i < ConcurrentIterations; i++)
        {
            var contract = $"contract{i}";
            tasks.Add(Task.Run(() => locator.Unmap<TestViewModel>(contract)));
        }

        await Task.WhenAll(tasks);

        for (var i = 0; i < ConcurrentIterations; i++)
        {
            var view = locator.ResolveView<TestViewModel>($"contract{i}");
            await Assert.That(view).IsNull();
        }
    }

    /// <summary>Test view implementing <see cref="IViewFor{TViewModel}" /> for <see cref="TestViewModel" />.</summary>
    private sealed class TestView : IViewFor<TestViewModel>
    {
        /// <summary>Gets or sets the strongly-typed view model.</summary>
        public TestViewModel? ViewModel { get; set; }

        /// <summary>Gets or sets the view model. Implements <see cref="IViewFor.ViewModel" />.</summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    /// <summary>Test view implementing <see cref="IViewFor{TViewModel}" /> for <see cref="TestViewModel2" />.</summary>
    private sealed class TestView2 : IViewFor<TestViewModel2>
    {
        /// <summary>Gets or sets the strongly-typed view model.</summary>
        public TestViewModel2? ViewModel { get; set; }

        /// <summary>Gets or sets the view model. Implements <see cref="IViewFor.ViewModel" />.</summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel2?)value;
        }
    }

    /// <summary>Alternative test view for <see cref="TestViewModel" />, used to test contract-specific mappings.</summary>
    private sealed class TestViewAlt : IViewFor<TestViewModel>
    {
        /// <summary>Gets or sets the strongly-typed view model.</summary>
        public TestViewModel? ViewModel { get; set; }

        /// <summary>Gets or sets the view model. Implements <see cref="IViewFor.ViewModel" />.</summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    /// <summary>Test view model used for testing view locator functionality.</summary>
    [SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Empty type used as a test marker.")]
    private sealed class TestViewModel : ReactiveObject;

    /// <summary>Second test view model used for testing multi-mapping scenarios.</summary>
    [SuppressMessage(
        "Minor Code Smell",
        "SST1436:Classes should not be empty",
        Justification = "Empty type used as a test marker.")]
    private sealed class TestViewModel2 : ReactiveObject;
}
