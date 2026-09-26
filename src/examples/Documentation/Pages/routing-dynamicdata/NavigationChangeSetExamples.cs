// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

namespace ReactiveUI.Documentation.RoutingDynamicData;

/// <summary>
/// Shows the DynamicData-flavoured navigation mixins from the ReactiveUI.Routing package. Referencing that package
/// also references DynamicData, and therefore System.Reactive, whose own <c>Subscribe(Action&lt;T&gt;)</c> would collide
/// with <c>ReactiveUI.Primitives.SubscribeExtensions.Subscribe(Action&lt;T&gt;)</c> if both were called plainly as
/// <c>.Subscribe(...)</c>. Every subscription below instead calls <c>SubscribePrimitives</c>, the Primitives-specific
/// name for the same method, which has no colliding counterpart in <c>System.Reactive</c>.
/// </summary>
public static class NavigationChangeSetExamples
{
    /// <summary><c>NavigationChanged</c> turns a router's navigation stack into a stream of DynamicData batches, one per push or pop.</summary>
    /// <returns>A task that completes when the navigation is done.</returns>
    public static async Task ObserveNavigationAsDynamicDataChangeSet()
    {
        AppShell shell = new();
        RoutingState router = shell.Router;
        List<IChangeSet<IRoutableViewModel>> batches = [];
        using IDisposable subscription = router.NavigationChanged().SubscribePrimitives(batches.Add);

        _ = await router.Navigate.Execute(new InventoryPageViewModel(shell));
        _ = await router.Navigate.Execute(new ProductDetailPageViewModel(shell, "Kettle"));
        _ = await router.NavigateBack.Execute();

        foreach (IChangeSet<IRoutableViewModel> batch in batches)
        {
            Console.WriteLine($"{batch.Adds} added, {batch.Removes} removed, countChanged={batch.HasCountChanged()}");
        }

        // Output:
        // 0 added, 0 removed, countChanged=False
        // 1 added, 0 removed, countChanged=True
        // 1 added, 0 removed, countChanged=True
        // 0 added, 1 removed, countChanged=True
    }

    /// <summary><c>ToDynamicDataChangeSet</c> re-projects the router's own <c>NavigationChanges</c> stream directly, without the
    /// <c>NavigationChanged</c> shortcut.</summary>
    /// <returns>A task that completes when the navigation is done.</returns>
    public static async Task ConvertNavigationChangesDirectly()
    {
        AppShell shell = new();
        RoutingState router = shell.Router;
        List<IChangeSet<IRoutableViewModel>> batches = [];
        using IDisposable subscription = router.NavigationChanges.ToDynamicDataChangeSet().SubscribePrimitives(batches.Add);

        _ = await router.Navigate.Execute(new InventoryPageViewModel(shell));

        Console.WriteLine(batches[^1].Adds);

        // Output:
        // 1
    }

    /// <summary><c>CountChanged</c> on a stream of <see cref="IChangeSet{T}"/> skips a batch that only replaces the top of the stack.</summary>
    /// <returns>A task that completes when the navigation is done.</returns>
    public static async Task FilterGenericNavigationChangesByCount()
    {
        AppShell shell = new();
        RoutingState router = shell.Router;
        List<IChangeSet<IRoutableViewModel>> countChangingBatches = [];
        using IDisposable subscription = router.NavigationChanged().CountChanged().SubscribePrimitives(countChangingBatches.Add);

        _ = await router.Navigate.Execute(new InventoryPageViewModel(shell));
        router.NavigationStack[0] = new InventoryPageViewModel(shell);
        _ = await router.Navigate.Execute(new ProductDetailPageViewModel(shell, "Kettle"));

        Console.WriteLine(countChangingBatches.Count);

        // Output:
        // 2
    }

    /// <summary>The non-generic overload of <c>CountChanged</c> filters a stream of the non-generic <see cref="IChangeSet"/> the same way.</summary>
    /// <returns>A task that completes when the navigation is done.</returns>
    public static async Task FilterNonGenericNavigationChangesByCount()
    {
        AppShell shell = new();
        RoutingState router = shell.Router;
        IObservable<IChangeSet<IRoutableViewModel>> changeSets = router.NavigationChanged();
        IObservable<IChangeSet> nonGenericChangeSets = changeSets;

        List<IChangeSet> countChangingBatches = [];
        using IDisposable subscription = nonGenericChangeSets.CountChanged().SubscribePrimitives(countChangingBatches.Add);

        _ = await router.Navigate.Execute(new InventoryPageViewModel(shell));
        router.NavigationStack[0] = new InventoryPageViewModel(shell);
        _ = await router.Navigate.Execute(new ProductDetailPageViewModel(shell, "Kettle"));

        Console.WriteLine(countChangingBatches.Count);

        // Output:
        // 2
    }

    /// <summary><c>ActOnEveryObject</c> reports a page entering and leaving the navigation stack.</summary>
    /// <returns>A task that completes when the navigation is done.</returns>
    public static async Task TrackPagesEnteringAndLeavingTheStack()
    {
        AppShell shell = new();
        RoutingState router = shell.Router;
        List<string> log = [];
        using IDisposable subscription = router.NavigationChanged().ActOnEveryObject(
            page => log.Add($"enter {page.UrlPathSegment}"),
            page => log.Add($"leave {page.UrlPathSegment}"));

        _ = await router.Navigate.Execute(new InventoryPageViewModel(shell));
        _ = await router.Navigate.Execute(new ProductDetailPageViewModel(shell, "Kettle"));
        _ = await router.NavigateBack.Execute();

        foreach (string entry in log)
        {
            Console.WriteLine(entry);
        }

        // Output:
        // enter inventory
        // enter inventory/Kettle
        // leave inventory/Kettle
    }
}
