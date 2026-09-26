// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.Routing;

/// <summary>Shows routing: a window's <see cref="RoutingState"/> keeps a stack of pages, and the view locator finds the view for the page on top.</summary>
public static class RoutingExamples
{
    /// <summary>Navigating pushes a page and going back pops it; the stack reads like the path in a browser.</summary>
    /// <returns>A task that completes when the user is back on the list.</returns>
    public static async Task NavigateAndGoBack()
    {
        AppShell shell = new();
        TodoListPage list = await OpenListAsync(shell);

        _ = await list.Open.Execute(list.Items[0]);
        Console.WriteLine(Path(shell));

        _ = await shell.Router.NavigateBack.Execute();
        Console.WriteLine(Path(shell));

        // Output:
        // todos > todos/1
        // todos
    }

    /// <summary>Going back is possible only when there is a page to go back to, so the back button follows <c>CanExecute</c>.</summary>
    /// <returns>A task that completes when an item is opened.</returns>
    public static async Task EnableTheBackButton()
    {
        AppShell shell = new();
        using IDisposable subscription = shell.Router.NavigateBack.CanExecute.Subscribe(Console.WriteLine);

        TodoListPage list = await OpenListAsync(shell);
        _ = await list.Open.Execute(list.Items[0]);

        // Output:
        // False
        // True
    }

    /// <summary><c>CanNavigateBack</c> reports whether there is a page to go back to, and emits again only when the answer changes.</summary>
    /// <returns>A task that completes when the second item is opened.</returns>
    public static async Task WatchWhetherYouCanGoBack()
    {
        AppShell shell = new();
        using var subscription = shell.Router.CanNavigateBack.Subscribe(Console.WriteLine);

        var list = await OpenListAsync(shell);
        _ = await list.Open.Execute(list.Items[0]);
        _ = await shell.Router.NavigateBack.Execute();
        _ = await list.Open.Execute(list.Items[1]);

        // Output:
        // False
        // True
        // False
        // True
    }

    /// <summary><c>NavigationStackChanged</c> hands you the whole stack after every change, oldest page first.</summary>
    /// <returns>A task that completes when the user is back on the list.</returns>
    public static async Task WatchTheStack()
    {
        AppShell shell = new();
        using var subscription = shell.Router.NavigationStackChanged
            .Subscribe(static stack => Console.WriteLine(string.Join(" > ", stack.Select(static page => page.UrlPathSegment))));

        var list = await OpenListAsync(shell);
        _ = await list.Open.Execute(list.Items[0]);
        _ = await shell.Router.NavigateBack.Execute();

        // Output:
        // todos
        // todos > todos/1
        // todos
    }

    /// <summary>
    /// A routed host follows <c>CurrentViewModel</c> and asks the view locator for each page's view, which is what
    /// <c>RoutedViewHost</c> does on every platform. Before the first navigation there is no page, and the host shows its
    /// default content.
    /// </summary>
    /// <returns>A task that completes when the user is back on the list.</returns>
    public static async Task ShowTheViewForTheCurrentPage()
    {
        AppShell shell = new();
        IViewLocator locator = ViewLocator.GetCurrent();
        using IDisposable host = shell.Router.CurrentViewModel
            .Select(page => page is null ? null : locator.ResolveView<object>(page, null))
            .Subscribe(static view => Console.WriteLine(view?.GetType().Name ?? "(default content)"));

        TodoListPage list = await OpenListAsync(shell);
        _ = await list.Open.Execute(list.Items[0]);
        _ = await shell.Router.NavigateBack.Execute();

        // Output:
        // (default content)
        // TodoListPageView
        // TodoDetailPageView
        // TodoListPageView
    }

    /// <summary><c>NavigateAndReset</c> replaces the whole stack, as signing out returns the user to a fresh start page.</summary>
    /// <returns>A task that completes when the stack is reset.</returns>
    public static async Task ResetTheStack()
    {
        AppShell shell = new();
        TodoListPage list = await OpenListAsync(shell);
        _ = await list.Open.Execute(list.Items[0]);
        _ = await list.Open.Execute(list.Items[1]);
        Console.WriteLine(Path(shell));

        _ = await shell.Router.NavigateAndReset.Execute(new TodoListPage(shell, list.Items));
        Console.WriteLine(Path(shell));

        // Output:
        // todos > todos/1 > todos/2
        // todos
    }

    /// <summary>
    /// Passing an <see cref="ISequencer"/> to the constructor controls which thread navigation results land on.
    /// Awaiting <c>Execute()</c> returns the page on any sequencer, here a background one.
    /// </summary>
    /// <returns>A task that completes once both pages are open.</returns>
    public static async Task AwaitNavigationOnABackgroundSequencer()
    {
        AppShell shell = new(new RoutingState(TaskPoolSequencer.Default));

        IRoutableViewModel first = await shell.Router.Navigate.Execute(new TodoListPage(shell, []));
        IRoutableViewModel second = await shell.Router.Navigate.Execute(new TodoListPage(shell, []));

        Console.WriteLine(first.UrlPathSegment);
        Console.WriteLine(second.UrlPathSegment);
        Console.WriteLine(shell.Router.NavigationStack.Count);

        // Output:
        // todos
        // todos
        // 2
    }

    /// <summary>
    /// <see cref="Sequencer.Immediate"/> delivers the result to a plain subscriber before <c>Execute()</c> returns,
    /// so a console app or a test runs in a fixed order without awaiting.
    /// </summary>
    public static void DeliverNavigationImmediately()
    {
        AppShell shell = new(new RoutingState(Sequencer.Immediate));
        TodoListPage list = new(shell, []);

        IRoutableViewModel? result = null;
        using IDisposable subscription = shell.Router.Navigate.Execute(list).Subscribe(page => result = page);

        Console.WriteLine(result?.UrlPathSegment);

        // Output:
        // todos
    }

    /// <summary>
    /// <c>NavigationChanges</c> reports each add and remove as it happens, rather than just the current page, so a
    /// view can animate a page sliding in or out.
    /// </summary>
    /// <returns>A task that completes when the user is back on the list.</returns>
    public static async Task WatchDetailedChangeSets()
    {
        AppShell shell = new();
        using IDisposable subscription = shell.Router.NavigationChanges.Subscribe(static changeSet =>
        {
            foreach (ReactiveChange<IRoutableViewModel> change in changeSet)
            {
                Console.WriteLine($"{change.Reason}: {change.Current.UrlPathSegment}");
            }
        });

        TodoListPage list = await OpenListAsync(shell);
        _ = await list.Open.Execute(list.Items[0]);
        _ = await shell.Router.NavigateBack.Execute();

        // Output:
        // Add: todos
        // Add: todos/1
        // Remove: todos/1
    }

    /// <summary>
    /// <c>FindViewModelInStack&lt;T&gt;</c> searches from the top of the stack down for the first page of a given
    /// type, which lets a deep page reach back to an ancestor without walking the stack itself.
    /// </summary>
    /// <returns>A task that completes once two detail pages sit above the list.</returns>
    public static async Task FindAPageOfAGivenType()
    {
        AppShell shell = new();
        TodoListPage list = await OpenListAsync(shell);
        _ = await list.Open.Execute(list.Items[0]);
        _ = await list.Open.Execute(list.Items[1]);

        TodoListPage? found = shell.Router.FindViewModelInStack<TodoListPage>();
        Console.WriteLine(found?.UrlPathSegment);

        // Output:
        // todos
    }

    /// <summary><c>GetCurrentViewModel</c> reads the page on top of the stack once, without subscribing to <c>CurrentViewModel</c>.</summary>
    /// <returns>A task that completes once a detail page is on top.</returns>
    public static async Task ReadTheTopOfTheStack()
    {
        AppShell shell = new();
        TodoListPage list = await OpenListAsync(shell);
        _ = await list.Open.Execute(list.Items[0]);

        IRoutableViewModel? current = shell.Router.GetCurrentViewModel();
        Console.WriteLine(current?.UrlPathSegment);

        // Output:
        // todos/1
    }

    /// <summary>Loads the seeded to-do items and navigates to the list page.</summary>
    /// <param name="shell">The window to navigate in.</param>
    /// <returns>The list page.</returns>
    private static async Task<TodoListPage> OpenListAsync(AppShell shell)
    {
        IReadOnlyList<TodoItem> items = await InMemoryTodoStore.CreateSeeded().QueryAsync(CancellationToken.None);
        TodoListPage list = new(shell, items);
        _ = await shell.Router.Navigate.Execute(list);
        return list;
    }

    /// <summary>Joins the segments of the pages on the stack, oldest first.</summary>
    /// <param name="shell">The window whose stack to read.</param>
    /// <returns>The path of pages.</returns>
    private static string Path(AppShell shell) =>
        string.Join(" > ", shell.Router.NavigationStack.Select(static page => page.UrlPathSegment));
}
