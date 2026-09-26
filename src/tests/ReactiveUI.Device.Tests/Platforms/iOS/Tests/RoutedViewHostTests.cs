// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Device.Tests;

/// <summary>Tests <see cref="RoutedViewHost"/> mirroring a router's stack into a real navigation controller.</summary>
public class RoutedViewHostTests
{
    /// <summary>The number of controllers after one push onto a one-controller stack.</summary>
    private const int StackAfterPush = 2;

    /// <summary>A router that already has a stack shows it, with the view model's segment as the title.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task ExistingStack_IsPushedWhenTheHostActivates()
    {
        var screen = new TestScreen();
        var first = new TestRoutableViewModel(screen, "first");
        screen.Router.NavigationStack.Add(first);

        var host = await CreateHostAsync(screen);
        try
        {
            await UIKitHarness.UntilAsync(() => host.ViewControllers is [RoutableViewController], "the existing stack to be pushed");

            var top = await MainThread.RunAsync(() => (RoutableViewController)host.TopViewController!);
            await Assert.That(top.ViewModel).IsSameReferenceAs(first);
            await UIKitHarness.UntilAsync(() => top.NavigationItem.Title == "first", "the title to follow the segment");
        }
        finally
        {
            await UIKitHarness.ResetAsync();
        }
    }

    /// <summary>Navigating pushes the new view model's controller, and navigating back pops it.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task NavigateAndNavigateBack_PushAndPopControllers()
    {
        var screen = new TestScreen();
        var first = new TestRoutableViewModel(screen, "first");
        var second = new TestRoutableViewModel(screen, "second");
        screen.Router.NavigationStack.Add(first);

        var host = await CreateHostAsync(screen);
        try
        {
            await UIKitHarness.UntilAsync(() => host.ViewControllers?.Length == 1, "the first controller");

            await MainThread.RunAsync(() => screen.Router.Navigate.Execute(second).FirstValueAsync(out _));
            await UIKitHarness.UntilAsync(() => host.ViewControllers?.Length == StackAfterPush, "the pushed controller");
            await Assert.That(await MainThread.RunAsync(() => ((RoutableViewController)host.TopViewController!).ViewModel)).IsSameReferenceAs(second);

            await MainThread.RunAsync(() => screen.Router.NavigateBack.Execute().FirstValueAsync(out _));
            await UIKitHarness.UntilAsync(() => host.ViewControllers?.Length == 1, "the pop back to the first controller");
            await Assert.That(screen.Router.NavigationStack).Count().IsEqualTo(1);
        }
        finally
        {
            await UIKitHarness.ResetAsync();
        }
    }

    /// <summary>Pushing a controller directly onto the host adds its view model to the router's stack.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task PushingDirectly_SyncsTheRouterStack()
    {
        var screen = new TestScreen();
        var first = new TestRoutableViewModel(screen, "first");
        var pushed = new TestRoutableViewModel(screen, "pushed");
        screen.Router.NavigationStack.Add(first);

        var host = await CreateHostAsync(screen);
        try
        {
            await UIKitHarness.UntilAsync(() => host.ViewControllers?.Length == 1, "the first controller");

            await MainThread.RunAsync(() => host.PushViewController(new RoutableViewController { ViewModel = pushed }, false));

            await Assert.That(screen.Router.NavigationStack).Count().IsEqualTo(StackAfterPush);
            await Assert.That(screen.Router.NavigationStack[^1]).IsSameReferenceAs(pushed);
        }
        finally
        {
            await UIKitHarness.ResetAsync();
        }
    }

    /// <summary>Creates a routed host over <paramref name="screen"/>'s router and shows it as the window's root.</summary>
    /// <param name="screen">The screen that owns the router.</param>
    /// <returns>The host.</returns>
    private static async Task<RoutedViewHost> CreateHostAsync(TestScreen screen)
    {
        var locator = new DelegateViewLocator(static (_, _) => new RoutableViewController());
        var host = await MainThread.RunAsync(() => new RoutedViewHost { Router = screen.Router, ViewLocator = locator });
        await UIKitHarness.ShowAsync(host);
        return host;
    }
}
