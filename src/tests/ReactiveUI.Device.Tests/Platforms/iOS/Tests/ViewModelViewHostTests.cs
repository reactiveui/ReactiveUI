// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests <see cref="ViewModelViewHost"/> hosting resolved controllers in a real UIKit hierarchy.</summary>
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

            await UIKitHarness.UntilAsync(() => child.ParentViewController == host, "the child to be adopted");
            await Assert.That(await MainThread.RunAsync(() => child.View!.Superview == host.View)).IsTrue();
            await Assert.That(child.ViewModel).IsSameReferenceAs(viewModel);
        }
        finally
        {
            await UIKitHarness.ResetAsync();
        }
    }

    /// <summary>Replacing the view model removes the old child controller and adopts the new one.</summary>
    /// <returns>A task representing the test.</returns>
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

            await MainThread.RunAsync(() => host.ViewModel = new TestViewModel("second"));
            await UIKitHarness.UntilAsync(() => created is [_, { ParentViewController: not null }], "the second child");

            await Assert.That(await MainThread.RunAsync(() => created[0].ParentViewController)).IsNull();
            await Assert.That(await MainThread.RunAsync(() => host.ChildViewControllers.Length)).IsEqualTo(1);
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

            await UIKitHarness.UntilAsync(() => placeholder.ParentViewController == host, "the default content");
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
