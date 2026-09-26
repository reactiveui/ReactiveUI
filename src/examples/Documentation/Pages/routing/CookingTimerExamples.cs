// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Routing;

/// <summary>Shows work that starts when a page gains focus and stops when it loses it, with a recipe app's cooking timer.</summary>
public static class CookingTimerExamples
{
    /// <summary><c>WhenNavigatedTo</c> lets a page start work of its own when it becomes the current page, and clean it up when the user leaves.</summary>
    /// <returns>A task that completes once the timer has been started and stopped.</returns>
    public static async Task StartAndStopWorkOnAPage()
    {
        RecipeShell shell = new();
        RecipeListPage list = new(shell, RecipeBook.Seeded());
        _ = await shell.Router.Navigate.Execute(list);

        RecipeDetailPage detail = (RecipeDetailPage)await list.Open.Execute(list.Recipes[0]);
        using CookingTimerPage timer = (CookingTimerPage)await detail.StartCooking.Execute();

        _ = await timer.Tick.Execute();
        _ = await timer.Tick.Execute();

        _ = await shell.Router.NavigateBack.Execute();

        // Output:
        // Timer started for Pancakes
        // Pancakes: 1s
        // Pancakes: 2s
        // Timer stopped for Pancakes
    }

    /// <summary>
    /// <c>WhenNavigatedToObservable</c> fires each time a page becomes the current one and completes once it leaves the
    /// stack. <c>WhenNavigatingFromObservable</c> fires just before a page stops being the current one.
    /// </summary>
    /// <returns>A task that completes once the detail page has been visited, left and revisited, then removed.</returns>
    public static async Task ObserveArrivalAndDeparture()
    {
        RecipeShell shell = new();
        IReadOnlyList<Recipe> recipes = RecipeBook.Seeded();
        RecipeDetailPage detail = new(shell, recipes[0]);

        using IDisposable arrivals = detail.WhenNavigatedToObservable().Subscribe(
            static _ => Console.WriteLine("Detail arrived"),
            static () => Console.WriteLine("Detail completed"));
        using IDisposable departures = detail.WhenNavigatingFromObservable().Subscribe(
            static _ => Console.WriteLine("Detail left"));

        _ = await shell.Router.Navigate.Execute(detail);
        _ = await shell.Router.Navigate.Execute(new RecipeDetailPage(shell, recipes[1]));
        _ = await shell.Router.NavigateBack.Execute();
        _ = await shell.Router.NavigateBack.Execute();

        // Output:
        // Detail arrived
        // Detail left
        // Detail arrived
        // Detail completed
    }

    /// <summary>
    /// A page can sit on the stack more than once, when the user reaches the same page again. Its
    /// <c>WhenNavigatedToObservable</c> stream completes only when the last copy leaves the stack.
    /// </summary>
    /// <returns>A task that completes once the stack has been reset to another page.</returns>
    public static async Task ObserveAPageThatIsOnTheStackTwice()
    {
        RecipeShell shell = new();
        RecipeListPage list = new(shell, RecipeBook.Seeded());

        using IDisposable arrivals = list.WhenNavigatedToObservable().Subscribe(
            static _ => Console.WriteLine("List arrived"),
            static () => Console.WriteLine("List completed"));

        _ = await shell.Router.Navigate.Execute(list);
        _ = await list.Open.Execute(list.Recipes[0]);

        // An "All recipes" link on the detail page opens the same list page again.
        _ = await shell.Router.Navigate.Execute(list);
        _ = await shell.Router.NavigateBack.Execute();
        Console.WriteLine("Back on the detail page");

        _ = await shell.Router.NavigateAndReset.Execute(new RecipeDetailPage(shell, list.Recipes[1]));

        // Output:
        // List arrived
        // List arrived
        // Back on the detail page
        // List completed
    }
}
