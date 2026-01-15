// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.WinForms.Tests.Winforms.Mocks;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>
/// Tests to make sure the can activate view fetcher works correctly.
/// </summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]
public class CanActivateViewFetcherTests
{
    /// <summary>
    /// Tests return negative for ICanActivate.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanNotFetchActivatorForNonCanActivateableForm()
    {
        var form = new TestFormNotCanActivate();
        var canActivateViewFetcher = new CanActivateViewFetcher();
        var result = await canActivateViewFetcher.GetActivationForView(form).FirstAsync();
        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Tests return positive for ICanActivate.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanGetActivationForViewForCanActivateableFormActivated()
    {
        var canActivateViewFetcher = new CanActivateViewFetcher();
        var result = await canActivateViewFetcher.GetActivationForView(new TestForm(1)).FirstAsync();
        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// Tests return negative for ICanActivate.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanGetActivationForViewForCanActivateableFormDeactivated()
    {
        var canActivateViewFetcher = new CanActivateViewFetcher();
        var result = await canActivateViewFetcher.GetActivationForView(new TestForm(2)).FirstAsync();
        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Tests return positive for ICanActivate.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
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
    public async Task ReturnZeroForNonICanActivateDerivatives()
    {
        var canActivateViewFetcher = new CanActivateViewFetcher();
        var affinity = canActivateViewFetcher.GetAffinityForView(typeof(CanActivateViewFetcherTests));
        await Assert.That(affinity).IsEqualTo(0);
    }

    private class CanActivateStub : ICanActivate
    {
        public IObservable<Unit> Activated { get; } = Observable.Empty<Unit>();

        public IObservable<Unit> Deactivated { get; } = Observable.Empty<Unit>();
    }
}
