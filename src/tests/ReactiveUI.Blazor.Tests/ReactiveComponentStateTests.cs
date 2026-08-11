// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Blazor.Internal;

namespace ReactiveUI.Blazor.Tests;

/// <summary>
/// Tests for <see cref="ReactiveComponentState"/>, the container that owns a reactive Blazor component's activation
/// signals, lifetime subscriptions and first-render subscriptions.
/// </summary>
public class ReactiveComponentStateTests
{
    /// <summary>Verifies that the first-render subscription can be read back after it is assigned.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task FirstRenderSubscriptions_ReturnsTheAssignedSubscription()
    {
        using var state = new ReactiveComponentState();
        using var subscription = new CountingDisposable();

        state.FirstRenderSubscriptions = subscription;

        await Assert.That(state.FirstRenderSubscriptions).IsSameReferenceAs(subscription);
    }

    /// <summary>
    /// Verifies that assigning a new first-render subscription disposes the one it replaces and leaves the replacement
    /// untouched, which is the swap behaviour the property exists to provide.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task FirstRenderSubscriptions_WhenReassigned_DisposesOnlyThePreviousSubscription()
    {
        using var state = new ReactiveComponentState();
        using var original = new CountingDisposable();
        using var replacement = new CountingDisposable();

        state.FirstRenderSubscriptions = original;
        state.FirstRenderSubscriptions = replacement;

        await Assert.That(original.DisposeCount).IsEqualTo(1);
        await Assert.That(replacement.DisposeCount).IsEqualTo(0);
        await Assert.That(state.FirstRenderSubscriptions).IsSameReferenceAs(replacement);
    }

    /// <summary>Verifies that disposing the state releases the first-render subscription it holds.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Dispose_ReleasesTheFirstRenderSubscription()
    {
        var state = new ReactiveComponentState();
        using var subscription = new CountingDisposable();
        state.FirstRenderSubscriptions = subscription;

        state.Dispose();

        await Assert.That(subscription.DisposeCount).IsEqualTo(1);
    }

    /// <summary>
    /// Verifies that a state that never reached its first render still disposes cleanly and completes its activation
    /// signals, so no further activation can be published through it.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Dispose_WithoutAFirstRenderSubscription_ClosesTheActivationSignals()
    {
        using var state = new ReactiveComponentState();

        await Assert.That(state.Dispose).ThrowsNothing();

        await Assert.That(state.NotifyActivated).Throws<ObjectDisposedException>();
        await Assert.That(state.NotifyDeactivated).Throws<ObjectDisposedException>();
    }

    /// <summary>
    /// Verifies that disposal is idempotent: a second call neither throws nor disposes the first-render subscription a
    /// second time, so a component that is disposed twice does not double-release the work it handed over.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Dispose_CalledTwice_ReleasesTheFirstRenderSubscriptionOnce()
    {
        var state = new ReactiveComponentState();
        using var subscription = new CountingDisposable();
        state.FirstRenderSubscriptions = subscription;
        state.Dispose();

        await Assert.That(state.Dispose).ThrowsNothing();

        await Assert.That(subscription.DisposeCount).IsEqualTo(1);
    }

    /// <summary>
    /// A disposable that records every disposal rather than collapsing repeat calls, so a test can tell the difference
    /// between "disposed once" and "disposed again".
    /// </summary>
    private sealed class CountingDisposable : IDisposable
    {
        /// <summary>Gets the number of times <see cref="Dispose"/> has been called.</summary>
        public int DisposeCount { get; private set; }

        /// <inheritdoc/>
        public void Dispose() => DisposeCount++;
    }
}
