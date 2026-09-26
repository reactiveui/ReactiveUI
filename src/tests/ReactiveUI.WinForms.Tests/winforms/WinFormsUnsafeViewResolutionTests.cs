// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.WinForms.Tests.Winforms.Mocks;
using Splat;
using TUnit.Core.Executors;
#if REACTIVE_SHIM
using WinFormsRoutedViewHost = ReactiveUI.Reactive.Winforms.RoutedControlHost;
using WinFormsRoutedViewHostUnsafe = ReactiveUI.Reactive.Winforms.RoutedControlHostUnsafe;
using WinFormsViewModelViewHost = ReactiveUI.Reactive.Winforms.ViewModelControlHost;
using WinFormsViewModelViewHostUnsafe = ReactiveUI.Reactive.Winforms.ViewModelControlHostUnsafe;
#else
using WinFormsRoutedViewHost = ReactiveUI.Winforms.RoutedControlHost;
using WinFormsRoutedViewHostUnsafe = ReactiveUI.Winforms.RoutedControlHostUnsafe;
using WinFormsViewModelViewHost = ReactiveUI.Winforms.ViewModelControlHost;
using WinFormsViewModelViewHostUnsafe = ReactiveUI.Winforms.ViewModelControlHostUnsafe;
#endif

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>
/// Tests the split between the default WinForms hosts, which ask the view locator's ahead-of-time safe lookup, and
/// their Unsafe twins, which also ask the service locator.
/// </summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]
public class WinFormsUnsafeViewResolutionTests
{
    /// <summary>Verifies the default view model host asks the safe lookup and never the reflective one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelControlHost_ResolvesThroughTheSafeLookup()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        using var target = new WinFormsViewModelViewHost { ViewLocator = viewLocator, ViewModel = new FakeWinformViewModel() };

        using (Assert.Multiple())
        {
            await Assert.That(target.CurrentView).IsTypeOf<FakeWinformsView>();
            await Assert.That(viewLocator.SafeLookups).IsGreaterThan(0);
            await Assert.That(viewLocator.UnsafeLookups).IsEqualTo(0);
        }
    }

    /// <summary>Verifies the Unsafe view model host asks the reflective lookup and never the safe one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelControlHostUnsafe_ResolvesThroughTheUnsafeLookup()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        using var target = new WinFormsViewModelViewHostUnsafe { ViewLocator = viewLocator, ViewModel = new FakeWinformViewModel() };

        using (Assert.Multiple())
        {
            await Assert.That(target.CurrentView).IsTypeOf<FakeWinformsView>();
            await Assert.That(viewLocator.UnsafeLookups).IsGreaterThan(0);
            await Assert.That(viewLocator.SafeLookups).IsEqualTo(0);
        }
    }

    /// <summary>Verifies the default view model host shows a view the app added to the view locator with <c>Map</c>.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelControlHost_ShowsAMappedView()
    {
        var viewLocator = new DefaultViewLocator();
        viewLocator.Map<MappedOnlyViewModel>(static () => new MappedOnlyView());
        var viewModel = new MappedOnlyViewModel();
        using var target = new WinFormsViewModelViewHost { ViewLocator = viewLocator, ViewModel = viewModel };

        using (Assert.Multiple())
        {
            await Assert.That(target.CurrentView).IsTypeOf<MappedOnlyView>();
            await Assert.That(((MappedOnlyView)target.CurrentView!).ViewModel).IsSameReferenceAs(viewModel);
        }
    }

    /// <summary>Verifies the default routed host asks the safe lookup and never the reflective one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedControlHost_ResolvesThroughTheSafeLookup()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var router = new RoutingState(Sequencer.Immediate);
        using var target = new WinFormsRoutedViewHost { Router = router, ViewLocator = viewLocator };

        using var navigation = router.Navigate.Execute(new FakeWinformViewModel()).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(target.Controls.OfType<FakeWinformsView>().Count()).IsEqualTo(1);
            await Assert.That(viewLocator.SafeLookups).IsGreaterThan(0);
            await Assert.That(viewLocator.UnsafeLookups).IsEqualTo(0);
        }
    }

    /// <summary>Verifies the Unsafe routed host asks the reflective lookup and never the safe one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedControlHostUnsafe_ResolvesThroughTheUnsafeLookup()
    {
        var viewLocator = new FakeViewLocator { LocatorFunc = static _ => new FakeWinformsView() };
        var router = new RoutingState(Sequencer.Immediate);
        using var target = new WinFormsRoutedViewHostUnsafe { Router = router, ViewLocator = viewLocator };

        using var navigation = router.Navigate.Execute(new FakeWinformViewModel()).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(target.Controls.OfType<FakeWinformsView>().Count()).IsEqualTo(1);
            await Assert.That(viewLocator.UnsafeLookups).IsGreaterThan(0);
            await Assert.That(viewLocator.SafeLookups).IsEqualTo(0);
        }
    }

    /// <summary>Verifies the default routed host shows a view the app added to the view locator with <c>Map</c>.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedControlHost_ShowsAMappedView()
    {
        var viewLocator = new DefaultViewLocator();
        viewLocator.Map<MappedOnlyViewModel>(static () => new MappedOnlyView());
        var router = new RoutingState(Sequencer.Immediate);
        using var target = new WinFormsRoutedViewHost { Router = router, ViewLocator = viewLocator };
        var viewModel = new MappedOnlyViewModel();

        using var navigation = router.Navigate.Execute(viewModel).Subscribe();

        var view = target.Controls.OfType<MappedOnlyView>().Single();
        await Assert.That(view.ViewModel).IsSameReferenceAs(viewModel);
    }

    /// <summary>Verifies the default view model host does not find a view registered only with the service locator.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelControlHost_ViewOnlyInTheServiceLocator_IsNotFound()
    {
        RegisterSplatOnlyView();
        using var target = new WinFormsViewModelViewHost { ViewLocator = new DefaultViewLocator(), ViewModel = new SplatOnlyViewModel() };

        await Assert.That(target.CurrentView).IsNull();
    }

    /// <summary>Verifies the Unsafe view model host finds a view registered only with the service locator.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelControlHostUnsafe_ViewOnlyInTheServiceLocator_IsFound()
    {
        RegisterSplatOnlyView();
        var viewModel = new SplatOnlyViewModel();
        using var target = new WinFormsViewModelViewHostUnsafe { ViewLocator = new DefaultViewLocator(), ViewModel = viewModel };

        using (Assert.Multiple())
        {
            await Assert.That(target.CurrentView).IsTypeOf<SplatOnlyView>();
            await Assert.That(((SplatOnlyView)target.CurrentView!).ViewModel).IsSameReferenceAs(viewModel);
        }
    }

    /// <summary>Verifies the default routed host does not find a view registered only with the service locator.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedControlHost_ViewOnlyInTheServiceLocator_IsNotFound()
    {
        RegisterSplatOnlyView();
        var router = new RoutingState(Sequencer.Immediate);
        using var target = new WinFormsRoutedViewHost { Router = router, ViewLocator = new DefaultViewLocator() };

        using var navigation = router.Navigate.Execute(new SplatOnlyViewModel()).Subscribe();

        await Assert.That(target.Controls.OfType<SplatOnlyView>().Count()).IsEqualTo(0);
    }

    /// <summary>Verifies the Unsafe routed host finds a view registered only with the service locator.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedControlHostUnsafe_ViewOnlyInTheServiceLocator_IsFound()
    {
        RegisterSplatOnlyView();
        var router = new RoutingState(Sequencer.Immediate);
        using var target = new WinFormsRoutedViewHostUnsafe { Router = router, ViewLocator = new DefaultViewLocator() };
        var viewModel = new SplatOnlyViewModel();

        using var navigation = router.Navigate.Execute(viewModel).Subscribe();

        var view = target.Controls.OfType<SplatOnlyView>().Single();
        await Assert.That(view.ViewModel).IsSameReferenceAs(viewModel);
    }

    /// <summary>Registers <see cref="SplatOnlyView"/> for <see cref="SplatOnlyViewModel"/> with the service locator only.</summary>
    private static void RegisterSplatOnlyView() =>
        AppLocator.CurrentMutable.Register<IViewFor<SplatOnlyViewModel>>(static () => new SplatOnlyView());
}
