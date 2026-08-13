// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;
using ReactiveUI.Tests.WinUI.Mocks;
using TUnit.Core.Executors;
using WinUIActivationForViewFetcher = ReactiveUI.WinUI.ActivationForViewFetcher;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Tests for the WinUI <see cref="WinUIActivationForViewFetcher"/>.</summary>
/// <remarks>
/// The fetcher is reached through an alias: this namespace sits under <c>ReactiveUI</c>, so an unqualified name
/// would bind to the shared implementation in <c>ReactiveUI</c> before any using directive is consulted.
/// </remarks>
[NotInParallel]
[TestExecutor<WinUITestExecutor>]
public class ActivationForViewFetcherTests
{
    /// <summary>The states a view passes through when it is activated and then deactivated.</summary>
    private static readonly bool[] ActivatedThenDeactivated = [true, false];

    /// <summary>The states a view passes through when it is activated.</summary>
    private static readonly bool[] Activated = [true];

    /// <summary>Verifies every framework element is claimed, since activation is derived from the visual tree.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_ForAFrameworkElement_ClaimsAnExactMatch()
    {
        var fetcher = new WinUIActivationForViewFetcher();

        await Assert.That(fetcher.GetAffinityForView(typeof(ContentControl))).IsEqualTo(BindingAffinity.ExactType);
    }

    /// <summary>Verifies a view that is not part of the visual tree is left to another fetcher.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_ForANonVisualView_ClaimsNothing()
    {
        var fetcher = new WinUIActivationForViewFetcher();

        await Assert.That(fetcher.GetAffinityForView(typeof(ActivatableTestView))).IsEqualTo(0);
    }

    /// <summary>Verifies a view that declares its own lifetime drives the activation stream directly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_ForASelfActivatingView_FollowsItsOwnSignals()
    {
        var fetcher = new WinUIActivationForViewFetcher();
        var view = new ActivatableTestView();
        var states = new List<bool>();

        using var subscription = fetcher.GetActivationForView(view).Subscribe(states.Add);
        view.Activate();
        view.Deactivate();

        await Assert.That(states).IsEquivalentTo(ActivatedThenDeactivated);
    }

    /// <summary>Verifies repeated activations collapse, so a view is not activated twice in a row.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_ForRepeatedActivations_EmitsEachStateOnce()
    {
        var fetcher = new WinUIActivationForViewFetcher();
        var view = new ActivatableTestView();
        var states = new List<bool>();

        using var subscription = fetcher.GetActivationForView(view).Subscribe(states.Add);
        view.Activate();
        view.Activate();

        await Assert.That(states).IsEquivalentTo(Activated);
    }

    /// <summary>Verifies a control's activation stream can be observed without it being in a visual tree yet.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_ForAControlOutsideTheVisualTree_StaysQuiet()
    {
        var fetcher = new WinUIActivationForViewFetcher();
        var control = new ActivatableTestControl();
        var states = new List<bool>();

        using var subscription = fetcher.GetActivationForView(control).Subscribe(states.Add);

        await Assert.That(states).IsEmpty();
    }
}
