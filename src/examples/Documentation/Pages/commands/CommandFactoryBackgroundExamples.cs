// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Commands;

/// <summary>
/// Shows every <c>CreateRunInBackground</c> overload. Unlike <c>Create</c>, its execution logic runs on a background
/// <see cref="ISequencer"/> instead of wherever the caller happens to be, so a UI thread stays free while it runs.
/// </summary>
public static class CommandFactoryBackgroundExamples
{
    /// <summary><c>CreateRunInBackground(Action, ...)</c> has five overloads, adding a <c>canExecute</c> observable, a background sequencer, or an output sequencer in any combination.</summary>
    /// <returns>A task that completes once every log entry has been written.</returns>
    public static async Task LogDeskEventsInBackground()
    {
        LibraryDesk desk = new();
        List<string> log = [];

        using ReactiveCommand<RxVoid, RxVoid> logOpened = ReactiveCommand.CreateRunInBackground(() => log.Add("Desk opened"));
        using ReactiveCommand<RxVoid, RxVoid> logWhileOpen = ReactiveCommand.CreateRunInBackground(() => log.Add("Book lent"), desk.WhenAnyValue(d => d.IsOpen));

        // The third argument is the background ISequencer, where the execute action runs; omitted, it defaults to
        // RxSchedulers.TaskpoolScheduler. The fourth is the output ISequencer, where the result, IsExecuting and
        // ThrownExceptions are delivered; omitted, it defaults to RxSchedulers.MainThreadScheduler.
        using ReactiveCommand<RxVoid, RxVoid> logBackground = ReactiveCommand.CreateRunInBackground(
            () => log.Add("Shelf tidied"),
            desk.WhenAnyValue(d => d.IsOpen),
            Sequencer.Immediate);
        using ReactiveCommand<RxVoid, RxVoid> logBackgroundAndOutput = ReactiveCommand.CreateRunInBackground(
            () => log.Add("Late fee waived"),
            Sequencer.Immediate,
            Sequencer.Immediate);
        using ReactiveCommand<RxVoid, RxVoid> logAll = ReactiveCommand.CreateRunInBackground(
            () => log.Add("Catalogue reindexed"),
            desk.WhenAnyValue(d => d.IsOpen),
            Sequencer.Immediate,
            Sequencer.Immediate);

        _ = await logOpened.Execute();
        _ = await logWhileOpen.Execute();
        _ = await logBackground.Execute();
        _ = await logBackgroundAndOutput.Execute();
        _ = await logAll.Execute();

        foreach (string entry in log)
        {
            Console.WriteLine(entry);
        }

        // Output:
        // Desk opened
        // Book lent
        // Shelf tidied
        // Late fee waived
        // Catalogue reindexed
    }

    /// <summary><c>CreateRunInBackground&lt;TParam&gt;(Action&lt;TParam&gt;, ...)</c>: the same five overloads, with a book id passed through.</summary>
    /// <returns>A task that completes once every book has been flagged.</returns>
    public static async Task FlagOverdueBooksInBackground()
    {
        LibraryDesk desk = new();
        List<int> overdueNotices = [];

        using ReactiveCommand<int, RxVoid> flag = ReactiveCommand.CreateRunInBackground<int>(overdueNotices.Add);
        using ReactiveCommand<int, RxVoid> flagWhileOpen = ReactiveCommand.CreateRunInBackground<int>(overdueNotices.Add, desk.WhenAnyValue(d => d.IsOpen));

        // The background ISequencer is where the execute action runs; omitted, it defaults to RxSchedulers.TaskpoolScheduler.
        using ReactiveCommand<int, RxVoid> flagBackground = ReactiveCommand.CreateRunInBackground<int>(overdueNotices.Add, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);
        using ReactiveCommand<int, RxVoid> flagBackgroundAndOutput = ReactiveCommand.CreateRunInBackground<int>(overdueNotices.Add, Sequencer.Immediate, Sequencer.Immediate);
        using ReactiveCommand<int, RxVoid> flagAll = ReactiveCommand.CreateRunInBackground<int>(overdueNotices.Add, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate, Sequencer.Immediate);

        _ = await flag.Execute(1);
        _ = await flagWhileOpen.Execute(2);
        _ = await flagBackground.Execute(3);
        _ = await flagBackgroundAndOutput.Execute(4);
        _ = await flagAll.Execute(1);

        Console.WriteLine(string.Join(", ", overdueNotices));

        // Output:
        // 1, 2, 3, 4, 1
    }

    /// <summary><c>CreateRunInBackground&lt;TResult&gt;(Func&lt;TResult&gt;, ...)</c>: the same five overloads, returning a count.</summary>
    /// <returns>A task that completes once every count has been read.</returns>
    public static async Task CountBooksOnShelfInBackground()
    {
        LibraryDesk desk = new();
        _ = desk.Borrow(1);

        using ReactiveCommand<RxVoid, int> countAll = ReactiveCommand.CreateRunInBackground(() => desk.Search(string.Empty).Count);
        using ReactiveCommand<RxVoid, int> countLoans = ReactiveCommand.CreateRunInBackground(() => desk.LoanCount, desk.WhenAnyValue(d => d.IsOpen));

        // The background ISequencer is where the execute function runs; omitted, it defaults to RxSchedulers.TaskpoolScheduler.
        using ReactiveCommand<RxVoid, int> countBackground = ReactiveCommand.CreateRunInBackground(() => desk.LoanCount, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);
        using ReactiveCommand<RxVoid, int> countBackgroundAndOutput = ReactiveCommand.CreateRunInBackground(() => desk.Search("Code").Count, Sequencer.Immediate, Sequencer.Immediate);
        using ReactiveCommand<RxVoid, int> countAllOverloads = ReactiveCommand.CreateRunInBackground(() => desk.LoanCount, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate, Sequencer.Immediate);

        Console.WriteLine(await countAll.Execute());
        Console.WriteLine(await countLoans.Execute());
        Console.WriteLine(await countBackground.Execute());
        Console.WriteLine(await countBackgroundAndOutput.Execute());
        Console.WriteLine(await countAllOverloads.Execute());

        // Output:
        // 4
        // 1
        // 1
        // 1
        // 1
    }

    /// <summary><c>CreateRunInBackground&lt;TParam, TResult&gt;(Func&lt;TParam, TResult&gt;, ...)</c>: the same five overloads, searching by term.</summary>
    /// <returns>A task that completes once every search has run.</returns>
    public static async Task SearchCatalogueInBackground()
    {
        LibraryDesk desk = new();

        using ReactiveCommand<string, IReadOnlyList<Book>> search = ReactiveCommand.CreateRunInBackground<string, IReadOnlyList<Book>>(desk.Search);
        using ReactiveCommand<string, IReadOnlyList<Book>> searchWhileOpen = ReactiveCommand.CreateRunInBackground<string, IReadOnlyList<Book>>(desk.Search, desk.WhenAnyValue(d => d.IsOpen));

        // The background ISequencer is where the execute function runs; omitted, it defaults to RxSchedulers.TaskpoolScheduler.
        using ReactiveCommand<string, IReadOnlyList<Book>> searchBackground = ReactiveCommand.CreateRunInBackground<string, IReadOnlyList<Book>>(
            desk.Search,
            desk.WhenAnyValue(d => d.IsOpen),
            Sequencer.Immediate);
        using ReactiveCommand<string, IReadOnlyList<Book>> searchBackgroundAndOutput = ReactiveCommand.CreateRunInBackground<string, IReadOnlyList<Book>>(
            desk.Search,
            Sequencer.Immediate,
            Sequencer.Immediate);
        using ReactiveCommand<string, IReadOnlyList<Book>> searchAllOverloads = ReactiveCommand.CreateRunInBackground<string, IReadOnlyList<Book>>(
            desk.Search,
            desk.WhenAnyValue(d => d.IsOpen),
            Sequencer.Immediate,
            Sequencer.Immediate);

        IReadOnlyList<Book> byTitle = await search.Execute("Refactoring");
        IReadOnlyList<Book> byAuthor = await searchWhileOpen.Execute("Fowler");
        IReadOnlyList<Book> background = await searchBackground.Execute("Pragmatic");
        IReadOnlyList<Book> backgroundAndOutput = await searchBackgroundAndOutput.Execute("Gamma");
        IReadOnlyList<Book> allOverloads = await searchAllOverloads.Execute("Martin");

        Console.WriteLine(byTitle[0].Title);
        Console.WriteLine(byAuthor[0].Title);
        Console.WriteLine(background[0].Title);
        Console.WriteLine(backgroundAndOutput[0].Title);
        Console.WriteLine(allOverloads[0].Title);

        // Output:
        // Refactoring
        // Refactoring
        // The Pragmatic Programmer
        // Design Patterns
        // Clean Code
    }
}
