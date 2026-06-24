// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Routing;

/// <summary>Tests for <see cref="RoutingStateMixins"/>.</summary>
public class RoutingStateMixinsTests
{
    /// <summary>Searching skips non-matching entries (the type test's false branch) before returning a match (its true branch).</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task FindViewModelInStackReturnsMatchAfterSkippingNonMatches()
    {
        var state = new RoutingState();
        var first = new FirstViewModel();
        state.NavigationStack.Add(first);
        state.NavigationStack.Add(new SecondViewModel());

        // Reverse search visits the SecondViewModel (no match) before the FirstViewModel (match),
        // exercising both branches of the `stack[i] is T` test.
        await Assert.That(state.FindViewModelInStack<FirstViewModel>()).IsEqualTo(first);
    }

    /// <summary>Searching a stack with no matching type returns the default.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task FindViewModelInStackReturnsNullWhenNoMatch()
    {
        var state = new RoutingState();
        state.NavigationStack.Add(new SecondViewModel());

        await Assert.That(state.FindViewModelInStack<FirstViewModel>()).IsNull();
    }

    /// <summary>The current view model is the top of the stack, or null when the stack is empty.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetCurrentViewModelReturnsTopOrNull()
    {
        var state = new RoutingState();

        await Assert.That(state.GetCurrentViewModel()).IsNull();

        var first = new FirstViewModel();
        var second = new SecondViewModel();
        state.NavigationStack.Add(first);
        state.NavigationStack.Add(second);

        await Assert.That(state.GetCurrentViewModel()).IsEqualTo(second);
    }

    /// <summary>A routable view model used to populate the navigation stack.</summary>
    private sealed class FirstViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <inheritdoc/>
        public string? UrlPathSegment => "first";

        /// <inheritdoc/>
        public IScreen HostScreen => null!;
    }

    /// <summary>A second routable view model type used to populate the navigation stack.</summary>
    private sealed class SecondViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <inheritdoc/>
        public string? UrlPathSegment => "second";

        /// <inheritdoc/>
        public IScreen HostScreen => null!;
    }
}
