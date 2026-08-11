// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using ReactiveUI.Tests.WinUI.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Tests for the AOT-friendly <see cref="RoutedViewHost{TViewModel}"/>.</summary>
/// <remarks>
/// This host differs from the non-generic one in how it resolves: it asks the locator for the view of a view-model
/// type known at compile time, so nothing here depends on the runtime type of the routed instance.
/// </remarks>
[NotInParallel]
[TestExecutor<WinUITestExecutor>]
public class GenericRoutedViewHostTests
{
    /// <summary>The content stood in for a routed view while nothing is routed.</summary>
    private const string Placeholder = "nothing routed";

    /// <summary>A contract naming a wide layout.</summary>
    private const string WideContract = "wide";

    /// <summary>Verifies the host stretches its content so a routed view fills the window it is placed in.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_StretchesItsContent()
    {
        var host = new RoutedViewHost<RoutedTestViewModel>();

        using (Assert.Multiple())
        {
            await Assert.That(host.HorizontalContentAlignment).IsEqualTo(HorizontalAlignment.Stretch);
            await Assert.That(host.VerticalContentAlignment).IsEqualTo(VerticalAlignment.Stretch);
        }
    }

    /// <summary>Verifies the default content round-trips through the CLR accessor.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultContent_RoundTripsThroughTheDependencyProperty()
    {
        var host = new RoutedViewHost<RoutedTestViewModel> { DefaultContent = Placeholder };

        using (Assert.Multiple())
        {
            await Assert.That(host.DefaultContent).IsEqualTo(Placeholder);
            await Assert.That(host.GetValue(RoutedViewHost<RoutedTestViewModel>.DefaultContentProperty)).IsEqualTo(Placeholder);
        }
    }

    /// <summary>
    /// Verifies a freshly constructed host presents nothing. The host subscribes to its own routing properties
    /// from its constructor, so the default content an object initializer assigns arrives too late to be shown;
    /// it is picked up at the next route change.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_PresentsNothing()
    {
        var host = new RoutedViewHost<RoutedTestViewModel> { DefaultContent = Placeholder };

        await Assert.That(host.Content).IsNull();
    }

    /// <summary>Verifies the router dependency property round-trips through the CLR accessor.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Router_RoundTripsThroughTheDependencyProperty()
    {
        var router = new RoutingState(Sequencer.Immediate);
        var host = new RoutedViewHost<RoutedTestViewModel> { Router = router };

        await Assert.That(host.GetValue(RoutedViewHost<RoutedTestViewModel>.RouterProperty)).IsSameReferenceAs(router);
    }

    /// <summary>Verifies the view contract setter republishes the contract as an observable.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContract_SetterRepublishesTheContractAsAnObservable()
    {
        var host = new RoutedViewHost<RoutedTestViewModel> { ViewContract = WideContract };
        string? published = null;

        using var subscription = host.ViewContractObservable.Subscribe(contract => published = contract);

        using (Assert.Multiple())
        {
            await Assert.That(host.ViewContract).IsEqualTo(WideContract);
            await Assert.That(published).IsEqualTo(WideContract);
        }
    }

    /// <summary>Verifies navigating to a view model puts the resolved view into the host's content.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Navigate_ShowsTheResolvedView()
    {
        var view = new RoutedTestView();
        var router = new RoutingState(Sequencer.Immediate);
        var locator = new StubViewLocator { ContractlessView = view };
        var host = new RoutedViewHost<RoutedTestViewModel> { DefaultContent = Placeholder, ViewLocator = locator, Router = router };
        var viewModel = new RoutedTestViewModel();

        using var navigation = router.Navigate.Execute(viewModel).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsSameReferenceAs(view);
            await Assert.That(view.ViewModel).IsSameReferenceAs(viewModel);
        }
    }

    /// <summary>Verifies the host prefers the view registered for the current contract.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContract_SelectsTheContractSpecificView()
    {
        var contractView = new ContractRoutedTestView();
        var router = new RoutingState(Sequencer.Immediate);
        var locator = new StubViewLocator { ContractlessView = new RoutedTestView(), Contract = WideContract, ContractView = contractView };
        var host = new RoutedViewHost<RoutedTestViewModel> { ViewLocator = locator, Router = router, ViewContract = WideContract };

        using var navigation = router.Navigate.Execute(new RoutedTestViewModel()).Subscribe();

        await Assert.That(host.Content).IsSameReferenceAs(contractView);
    }

    /// <summary>Verifies the host falls back to the contract-free view when the contract resolves nothing.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContract_WithNoContractSpecificView_FallsBackToTheContractlessView()
    {
        const string unknownContract = "narrow";
        var view = new RoutedTestView();
        var router = new RoutingState(Sequencer.Immediate);
        var locator = new StubViewLocator { ContractlessView = view, Contract = WideContract, ContractView = new ContractRoutedTestView() };
        var host = new RoutedViewHost<RoutedTestViewModel> { ViewLocator = locator, Router = router, ViewContract = unknownContract };

        using var navigation = router.Navigate.Execute(new RoutedTestViewModel()).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsSameReferenceAs(view);
            _ = await Assert.That(locator.RequestedContracts).Contains(unknownContract);
        }
    }

    /// <summary>Verifies an unresolvable view model is reported rather than silently leaving the host empty.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Navigate_WithNoRegisteredView_Throws()
    {
        var router = new RoutingState(Sequencer.Immediate);
        var host = new RoutedViewHost<RoutedTestViewModel> { ViewLocator = new StubViewLocator(), Router = router };

        _ = host;

        await Assert.That(() => router.Navigate.Execute(new RoutedTestViewModel()).Subscribe())
            .Throws<InvalidOperationException>();
    }

    /// <summary>Verifies emptying the navigation stack restores the default content.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NavigationStack_Emptied_RestoresTheDefaultContent()
    {
        var router = new RoutingState(Sequencer.Immediate);
        var locator = new StubViewLocator { ContractlessView = new RoutedTestView() };
        var host = new RoutedViewHost<RoutedTestViewModel> { DefaultContent = Placeholder, ViewLocator = locator, Router = router };
        using var navigation = router.Navigate.Execute(new RoutedTestViewModel()).Subscribe();

        router.NavigationStack.Clear();

        await Assert.That(host.Content).IsEqualTo(Placeholder);
    }

    /// <summary>Verifies the host resolves through the registered locator when none is assigned to it.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WinUIViewRegistrationExecutor>]
    public async Task Navigate_WithNoAssignedLocator_UsesTheRegisteredViewLocator()
    {
        var router = new RoutingState(Sequencer.Immediate);
        var host = new RoutedViewHost<RoutedTestViewModel> { Router = router };

        using var navigation = router.Navigate.Execute(new RoutedTestViewModel()).Subscribe();

        _ = await Assert.That(host.Content).IsTypeOf<RoutedTestView>();
    }
}
