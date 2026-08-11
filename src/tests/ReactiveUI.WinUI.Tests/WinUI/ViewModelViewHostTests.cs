// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.WinUI.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Tests for <see cref="ViewModelViewHost"/>.</summary>
[NotInParallel]
[TestExecutor<WinUITestExecutor>]
public class ViewModelViewHostTests
{
    /// <summary>The content stood in for a view while no view model is set.</summary>
    private const string Placeholder = "no view model";

    /// <summary>The number of lookups a host performs when the contract lookup is allowed to fall back.</summary>
    private const int LookupsWithFallback = 2;

    /// <summary>Verifies the default content round-trips through the CLR accessor.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultContent_RoundTripsThroughTheDependencyProperty()
    {
        var host = new ViewModelViewHost { DefaultContent = Placeholder };

        using (Assert.Multiple())
        {
            await Assert.That(host.DefaultContent).IsEqualTo(Placeholder);
            await Assert.That(host.GetValue(ViewModelViewHost.DefaultContentProperty)).IsEqualTo(Placeholder);
        }
    }

    /// <summary>
    /// Verifies a freshly constructed host presents nothing. The host resolves from its constructor, so the
    /// default content an object initializer assigns arrives too late to be shown; it is picked up the next time
    /// the view model changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_PresentsNothing()
    {
        var host = new ViewModelViewHost { DefaultContent = Placeholder };

        await Assert.That(host.Content).IsNull();
    }

    /// <summary>Verifies the host publishes a view contract stream for consumers to observe.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContractObservable_IsPublishedByTheConstructor()
    {
        var host = new ViewModelViewHost();

        await Assert.That(host.ViewContractObservable).IsNotNull();
    }

    /// <summary>Verifies the view contract setter republishes the contract as an observable.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContract_SetterRepublishesTheContractAsAnObservable()
    {
        const string contract = "wide";
        var host = new ViewModelViewHost { ViewContract = contract };
        string? published = null;

        using var subscription = host.ViewContractObservable.Subscribe(observed => published = observed);

        using (Assert.Multiple())
        {
            await Assert.That(host.ViewContract).IsEqualTo(contract);
            await Assert.That(published).IsEqualTo(contract);
        }
    }

    /// <summary>Verifies the contract fallback flag round-trips through the CLR accessor.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ContractFallbackByPass_RoundTripsThroughTheDependencyProperty()
    {
        var host = new ViewModelViewHost { ContractFallbackByPass = true };

        using (Assert.Multiple())
        {
            await Assert.That(host.ContractFallbackByPass).IsTrue();
            await Assert.That((bool)host.GetValue(ViewModelViewHost.ContractFallbackByPassProperty)).IsTrue();
        }
    }

    /// <summary>Verifies setting a view model puts the resolved view into the host's content.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_ShowsTheResolvedView()
    {
        var view = new PlainTestView();
        var viewModel = new PlainTestViewModel();
        var locator = new StubViewLocator { ContractlessView = view };
        var host = new ViewModelViewHost { DefaultContent = Placeholder, ViewLocator = locator, ViewModel = viewModel };

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsSameReferenceAs(view);
            await Assert.That(view.ViewModel).IsSameReferenceAs(viewModel);
        }
    }

    /// <summary>Verifies clearing the view model returns the host to its default content.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_ClearedAfterResolution_RestoresTheDefaultContent()
    {
        var locator = new StubViewLocator { ContractlessView = new PlainTestView() };
        var host = new ViewModelViewHost { DefaultContent = Placeholder, ViewLocator = locator, ViewModel = new PlainTestViewModel() };

        host.ViewModel = null;

        await Assert.That(host.Content).IsEqualTo(Placeholder);
    }

    /// <summary>Verifies an unresolvable view model leaves the default content in place.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_WithNoRegisteredView_ShowsTheDefaultContent()
    {
        var host = new ViewModelViewHost { DefaultContent = Placeholder, ViewLocator = new StubViewLocator(), ViewModel = new PlainTestViewModel() };

        await Assert.That(host.Content).IsEqualTo(Placeholder);
    }

    /// <summary>Verifies a failed contract lookup is retried without the contract.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_WhenTheContractLookupFails_RetriesWithoutTheContract()
    {
        var locator = new StubViewLocator();

        var host = new ViewModelViewHost { ViewLocator = locator, ViewModel = new PlainTestViewModel() };

        _ = host;
        await Assert.That(locator.RequestedContracts).Count().IsEqualTo(LookupsWithFallback);
    }

    /// <summary>Verifies bypassing the fallback stops the host retrying without the contract.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ContractFallbackByPass_StopsTheRetryWithoutTheContract()
    {
        var locator = new StubViewLocator();

        var host = new ViewModelViewHost { ContractFallbackByPass = true, ViewLocator = locator, ViewModel = new PlainTestViewModel() };

        _ = host;
        _ = await Assert.That(locator.RequestedContracts).HasSingleItem();
    }

    /// <summary>Verifies the host resolves through the registered locator when none is assigned to it.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WinUIViewRegistrationExecutor>]
    public async Task ViewModel_WithNoAssignedLocator_UsesTheRegisteredViewLocator()
    {
        var host = new ViewModelViewHost { ViewModel = new PlainTestViewModel() };

        _ = await Assert.That(host.Content).IsTypeOf<PlainTestView>();
    }
}
