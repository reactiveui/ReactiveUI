// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests <see cref="ViewModelViewHost"/> hosting resolved controllers in a real UIKit hierarchy.</summary>
[NotInParallel(UIKitHarness.WindowKey)]
public class ViewModelViewHostTests
{
    /// <summary>Setting a view model resolves its view, hands it the view model and adopts it as a child controller.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task SettingViewModel_AdoptsTheResolvedController()
    {
        var child = await MainThread.RunAsync(static () => new TestViewController());
        var host = await CreateHostAsync(new((_, _) => child));
        try
        {
            var viewModel = new TestViewModel("hosted");
            await MainThread.RunAsync(() => host.ViewModel = viewModel);

            await UIKitHarness.UntilAsync(() => ReferenceEquals(child.ParentViewController, host), "the child to be adopted");
            await Assert.That(await MainThread.RunAsync(() => ReferenceEquals(child.View!.Superview, host.View))).IsTrue();
            await Assert.That(child.ViewModel).IsSameReferenceAs(viewModel);
        }
        finally
        {
            await UIKitHarness.ResetAsync();
        }
    }

    /// <summary>Replacing the view model removes and disposes the old child controller and adopts the new one.</summary>
    /// <returns>A task representing the test.</returns>
    /// <remarks>
    /// The host owns the controllers it resolves and disposes the one it replaces. A disposed controller no longer keeps
    /// its native object alive, so the test checks the old controller only through managed state and the host's own
    /// hierarchy, never by sending it a message.
    /// </remarks>
    [Test]
    public async Task ReplacingViewModel_SwapsTheChildController()
    {
        List<TestViewController> created = [];
        var host = await CreateHostAsync(new((_, _) =>
        {
            var view = new TestViewController();
            created.Add(view);
            return view;
        }));
        try
        {
            await MainThread.RunAsync(() => host.ViewModel = new TestViewModel("first"));
            await UIKitHarness.UntilAsync(() => created is [{ ParentViewController: not null }], "the first child");
            var first = created[0];
            var firstView = await MainThread.RunAsync(() => first.View!);

            await MainThread.RunAsync(() => host.ViewModel = new TestViewModel("second"));
            await UIKitHarness.UntilAsync(() => created is [_, { ParentViewController: not null }], "the second child");
            var second = created[1];

            await Assert.That(first.IsDisposed).IsTrue();
            await Assert.That(second.IsDisposed).IsFalse();
            await Assert.That(await MainThread.RunAsync(() => host.ChildViewControllers is [var only] && ReferenceEquals(only, second))).IsTrue();
            await Assert.That(await MainThread.RunAsync(() => host.View!.Subviews.Any(v => ReferenceEquals(v, second.View)))).IsTrue();
            await Assert.That(await MainThread.RunAsync(() => host.View!.Subviews.Any(v => ReferenceEquals(v, firstView)))).IsFalse();
        }
        finally
        {
            await UIKitHarness.ResetAsync();
        }
    }

    /// <summary>With no view model, the host shows its default content.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task WithoutViewModel_ShowsTheDefaultContent()
    {
        var host = await CreateHostAsync(new(static (_, _) => null));
        try
        {
            var placeholder = await MainThread.RunAsync(static () => new UIViewController());
            await MainThread.RunAsync(() => host.DefaultContent = placeholder);

            await UIKitHarness.UntilAsync(() => ReferenceEquals(placeholder.ParentViewController, host), "the default content");
        }
        finally
        {
            await UIKitHarness.ResetAsync();
        }
    }

    /// <summary>The view contract reaches the view locator.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task ViewContract_IsPassedToTheLocator()
    {
        const string contract = "compact";
        var locator = new DelegateViewLocator(static (_, _) => new TestViewController());
        var host = await CreateHostAsync(locator);
        try
        {
            await MainThread.RunAsync(() =>
            {
                host.ViewContract = contract;
                host.ViewModel = new TestViewModel("contracted");
            });

            await UIKitHarness.UntilAsync(() => locator.Contracts.Contains(contract), "the contract to reach the locator");
        }
        finally
        {
            await UIKitHarness.ResetAsync();
        }
    }

    /// <summary>A hosted reactive controller activates while it is on screen and deactivates when it leaves.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task HostedController_ActivatesOnScreenAndDeactivatesOffScreen()
    {
        var child = await MainThread.RunAsync(static () => new TestViewController());
        var host = await CreateHostAsync(new((_, _) => child));

        await MainThread.RunAsync(() => host.ViewModel = new TestViewModel("shown"));
        await Eventually.TrueAsync(() => child.Activations == 1, "the hosted controller to activate");

        await UIKitHarness.ResetAsync();
        await Eventually.TrueAsync(() => child.Deactivations == 1, "the hosted controller to deactivate");
    }

    /// <summary>Creates a host with <paramref name="locator"/> and shows it as the window's root.</summary>
    /// <param name="locator">The view locator.</param>
    /// <returns>The host.</returns>
    private static async Task<ViewModelViewHost> CreateHostAsync(DelegateViewLocator locator)
    {
        var host = await MainThread.RunAsync(() => new ViewModelViewHost { ViewLocator = locator });
        await UIKitHarness.ShowAsync(host);
        return host;
    }
}
