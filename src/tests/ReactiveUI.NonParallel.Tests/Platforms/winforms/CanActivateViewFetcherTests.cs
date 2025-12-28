// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Reactive.Testing;

using ReactiveUI.Tests.Winforms;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests to make sure the can activate view fetcher works correctly.
/// </summary>
public class CanActivateViewFetcherTests
{
    /// <summary>
    /// Tests return negative for ICanActivate.
    /// </summary>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public void CanNotFetchActivatorForNonCanActivateableForm()
    {
        var form = new TestFormNotCanActivate();
        var canActivateViewFetcher = new CanActivateViewFetcher();
        canActivateViewFetcher.GetActivationForView(form).AssertEqual(Observable.Return(false));
    }

    /// <summary>
    /// Tests return positive for ICanActivate.
    /// </summary>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public void CanGetActivationForViewForCanActivateableFormActivated()
    {
        var canActivateViewFetcher = new CanActivateViewFetcher();
        canActivateViewFetcher.GetActivationForView(new TestForm(1)).FirstAsync().AssertEqual(Observable.Return(true));
    }

    /// <summary>
    /// Tests return negative for ICanActivate.
    /// </summary>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public void CanGetActivationForViewForCanActivateableFormDeactivated()
    {
        var canActivateViewFetcher = new CanActivateViewFetcher();
        canActivateViewFetcher.GetActivationForView(new TestForm(2)).FirstAsync().AssertEqual(Observable.Return(false));
    }

    /// <summary>
    /// Tests return positive for ICanActivate.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task ReturnPositiveForICanActivate()
    {
        var canActivateViewFetcher = new CanActivateViewFetcher();
        var affinity = canActivateViewFetcher.GetAffinityForView(typeof(ICanActivate));
        await Assert.That(affinity).IsGreaterThan(0);
    }

    /// <summary>
    /// Tests return positive for ICanActivate derivatives.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task ReturnPositiveForICanActivateDerivatives()
    {
        var canActivateViewFetcher = new CanActivateViewFetcher();
        var affinity = canActivateViewFetcher.GetAffinityForView(typeof(CanActivateStub));
        await Assert.That(affinity).IsGreaterThan(0);
    }

    /// <summary>
    /// Tests return zero for non ICanActivate derivatives.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task ReturnZeroForNonICanActivateDerivatives()
    {
        var canActivateViewFetcher = new CanActivateViewFetcher();
        var affinity = canActivateViewFetcher.GetAffinityForView(typeof(CanActivateViewFetcherTests));
        await Assert.That(affinity).IsEqualTo(0);
    }

#pragma warning disable CA1812 // Class is not instantiated

    private class CanActivateStub : ICanActivate
    {
        public IObservable<Unit> Activated { get; } = Observable.Empty<Unit>();

        public IObservable<Unit> Deactivated { get; } = Observable.Empty<Unit>();
    }
}
